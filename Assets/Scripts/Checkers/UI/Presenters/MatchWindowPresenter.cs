﻿using Checkers.Services;
using Checkers.Settings;
using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Server.Services;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class MatchWindowPresenter : BaseWindowPresenter<IMatchWindow, MatchWindowData>,
                                        IUpdatable {
        private NakamaService _nakamaService;
        private AppConfig _appConfig;
        private IUpdateService _updateService;
        private MainCheckerSceneSettings _sceneSettings;
        private TimerService _timerService;
        
        private bool _needNicknameInitialize;
        private const string TurnId = "TurnTimer";

        public MatchWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
            _sceneSettings = Resolve<MainCheckerSceneSettings>(GameContext.Checkers);
            _timerService = Resolve<TimerService>(GameContext.Project);

            _sceneSettings.PawnMover.OnTurn += CaptureChecker;
        }

        protected override async UniTask LoadContent() {
            _updateService.RegisterUpdate(this);

            View.SubscribeToHowToPlayButton(OnHowToPlayClick);
            View.SubscribeToFleeButton(OnFleeClick);
            
            View.ProvideCamera(UnityEngine.Camera.main);
            _needNicknameInitialize = true;
            View.ResetBars();

            _sceneSettings.PawnMover.OnTurnEnd += TurnChange;
            _sceneSettings.TurnHandler.OnEndGame += (s) => _timerService.RemoveTimer(TurnId);
            _timerService.StartTimer(TurnId, 10f, TurnTimeOut, false, View.SetTimerTime);
        }

        private void OnFleeClick() {
            FireSignal(new OpenWindowSignal(WindowKey.FleeWindow, new FleeWindowData()));
        }

        private void OnHowToPlayClick() {
            FireSignal(new OpenWindowSignal(WindowKey.RulesWindow, new RulesWindowData()));
        }

        public void CustomUpdate() {
            if (!_needNicknameInitialize) return;

            _needNicknameInitialize = false;
            var me = _nakamaService.GetMe();

            if (_appConfig.PawnColor == (int) PawnColor.Black) {
                View.SetOpponentName(me.User.DisplayName);
                View.SetYourName(_appConfig.Opponent);
            }
            else {
                View.SetYourName(me.User.DisplayName);
                View.SetOpponentName(_appConfig.Opponent);
            }
        }

        public override void Dispose() {
            _updateService.UnregisterUpdate(this);
            _timerService.RemoveTimer(TurnId);
        }

        private void CaptureChecker(TurnData turnData) {
            if (!turnData.Capture) return;
            
            bool isPlayer = _sceneSettings.TurnHandler.Turn == _sceneSettings.TurnHandler.YourColor;
            
            View.GetLostCheсker(isPlayer);
        }

        private void TurnChange() {
            _timerService.ResetTimer(TurnId);
        }

        private void TurnTimeOut() {
            var winner = _sceneSettings.TurnHandler.Turn == PawnColor.Black ? PawnColor.Black : PawnColor.White;
            _sceneSettings.TurnHandler.OnEndGame?.Invoke(winner);
        }
    }
}