using System;
using System.Collections.Generic;
using Checkers.Services;
using Checkers.Settings;
using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using Global;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
using Global.Services;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Nakama;
using Nakama.TinyJson;
using Server;
using Server.Services;
using UnityEngine;
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
        private MessageService _messageService;
        private GlobalScope _globalScope;
        
        private bool _needNicknameInitialize;
        private const string TurnId = "TurnTimer";
        private const string PauseId = "PauseId";
        private long _timerStartTime;
        private long _remainTime;
        private int _defaultTurnTime = 20;
        private int _pauseTime = 10;

        public MatchWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
            _sceneSettings = Resolve<MainCheckerSceneSettings>(GameContext.Checkers);
            _timerService = Resolve<TimerService>(GameContext.Project);
            _messageService = Resolve<MessageService>(GameContext.Project);
            _globalScope = Resolve<GlobalScope>(GameContext.Project);

            _sceneSettings.PawnMover.OnTurn += CaptureChecker;
            _nakamaService.SubscribeToMessages(OnChatMessage);
            
            View.SetPauseStateView(false);
        }

        protected override async UniTask LoadContent() {
            _updateService.RegisterUpdate(this);

            View.SubscribeToHowToPlayButton(OnHowToPlayClick);
            View.SubscribeToFleeButton(OnFleeClick);
            
            View.ProvideCamera(UnityEngine.Camera.main);
            _needNicknameInitialize = true;

            _sceneSettings.PawnMover.OnTurnEnd += TurnChange;
            _sceneSettings.TurnHandler.OnEndGame += (s, r) => _timerService.RemoveTimer(TurnId);
            _timerService.StartTimer(TurnId, _defaultTurnTime, TurnTimeOut, false, View.SetTimerTime);
            _timerStartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            _remainTime = _defaultTurnTime;
            
            SetBarsPosition();
            ApplicationQuit.SubscribeOnQuit(PauseGame);
            ApplicationQuit.SubscribeOnResume(ResumeGame);        
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
                View.SetYourName(_appConfig.OpponentDisplayName);
            }
            else {
                View.SetYourName(me.User.DisplayName);
                View.SetOpponentName(_appConfig.OpponentDisplayName);
            }
        }

        public override async UniTask Dispose() {
            _updateService.UnregisterUpdate(this);
            _timerService.RemoveTimer(TurnId);
            ApplicationQuit.UnSubscribeOnQuit(PauseGame);
            ApplicationQuit.UnSubscribeOnResume(ResumeGame);
        }

        public Vector3 GetSendPawnCheckersBarPosition() {
            bool isPlayer = _sceneSettings.TurnHandler.Turn == _sceneSettings.TurnHandler.YourColor;

            return View.GetSendPawnPosition(isPlayer);
        }

        private void CaptureChecker(TurnData turnData) {
            if (!turnData.Capture) return;
            
            bool isYourTurn = _sceneSettings.TurnHandler.Turn == _sceneSettings.TurnHandler.YourColor;
            View.GetLostCheсker(isYourTurn);
        }

        private void TurnChange() {
            _timerService.ResetTimer(TurnId, _defaultTurnTime);
            _timerStartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            _remainTime = _defaultTurnTime;
        }

        private void TurnTimeOut() {
            var winner = _sceneSettings.TurnHandler.Turn == PawnColor.Black ? PawnColor.Black : PawnColor.White;
            _sceneSettings.TurnHandler.OnEndGame?.Invoke(winner, WinLoseReason.Timeout);
        }

        private async UniTask SetBarsPosition() {
            await UniTask.Delay(50);
           
            bool isWhite = _sceneSettings.TurnHandler.StartingPawnColor == _sceneSettings.TurnHandler.YourColor;
            View.ResetBars(isWhite);

            _sceneSettings._playerBar = View.GetSendPawnPosition(isWhite);
            _sceneSettings._opponentBar = View.GetSendPawnPosition(!isWhite);
        }

        private async void PauseGame() {
            _timerService.RemoveTimer(TurnId);
            
            var continueTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            _remainTime = _defaultTurnTime - (continueTime - _timerStartTime);

            var opponentUserId = !string.IsNullOrEmpty(_appConfig.OpponentUserId)
                ? _appConfig.OpponentUserId
                : _globalScope.ApproveSenderId;
            
            await _messageService.SendPauseInfo(opponentUserId, "True");
        }
        
        private void OnChatMessage(IApiChannelMessage message) {
            var content = message.Content.FromJson<Dictionary<string, string>>();
            
            var profile = _nakamaService.GetMe();
            if (content.TryGetValue("TargetUser", out var targetUser)) {
                if (profile.User.Id != targetUser) return;
            }
            
            if (content.TryGetValue("Pause", out var pauseValue)) {
                var continueTime = DateTimeOffset.Now.ToUnixTimeSeconds();

                if (!string.IsNullOrEmpty(pauseValue)) {
                    _timerService.RemoveTimer(TurnId);
                    
                    _remainTime = _defaultTurnTime - (continueTime - _timerStartTime);
                    
                    View.SetPauseStateView(true);
                    _timerService.StartTimer(PauseId, _pauseTime, TurnTimeOut, false, View.SetPauseTime);
                    return;
                }
                
                _timerService.RemoveTimer(PauseId);
                View.SetPauseStateView(false);
                
                _timerStartTime = continueTime - _remainTime;
                _timerService.StartTimer(TurnId, _remainTime, TurnTimeOut, false, View.SetTimerTime);
            }
        }

        private void ResumeGame() {
            var continueTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            _timerStartTime = continueTime - _remainTime;
            
            _timerService.StartTimer(TurnId, _remainTime, TurnTimeOut, false, View.SetTimerTime);
        }
    }
}