﻿using System;
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
using UnityEngine.Scripting;
using Zenject;

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
        private SignalBus _signalBus;

        private bool _needNicknameInitialize;
        private bool _needPauseGame;
        private bool _needResumeGame;

        private const string TurnId = "TurnTimer";

        private float _remainTime;
        private long _pauseStartTime;

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
            _signalBus = Resolve<SignalBus>(GameContext.Checkers);
        }

        protected override async UniTask LoadContent() {
            _sceneSettings.PawnMover.OnTurn += CaptureChecker;
            _nakamaService.SubscribeToMessages(OnChatMessage);
            
            _updateService.RegisterUpdate(this);

            View.SubscribeToHowToPlayButton(OnHowToPlayClick);
            View.SubscribeToFleeButton(OnFleeClick);
            
            View.ProvideCamera(UnityEngine.Camera.main);
            _needNicknameInitialize = true;

            _sceneSettings.PawnMover.OnTurnEnd += TurnChange;
            _sceneSettings.TurnHandler.OnEndGame += (s, r) => _timerService.RemoveTimer(TurnId);
            _timerService.StartTimer(TurnId, _appConfig.TurnTime, TurnTimeOut, false, View.SetTimerTime);
            _remainTime = _appConfig.TurnTime;
            
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
            if (_needNicknameInitialize)
            {
                _needNicknameInitialize = false;
                var me = _nakamaService.GetMe();

                if (_appConfig.PawnColor == PawnColor.Black) {
                    View.SetOpponentName(me.User.DisplayName);
                    View.SetYourName(_appConfig.OpponentDisplayName);
                }
                else {
                    View.SetYourName(me.User.DisplayName);
                    View.SetOpponentName(_appConfig.OpponentDisplayName);
                }
            }

            if (_needPauseGame) {
                _signalBus.Fire(new OpenWindowSignal(WindowKey.PauseWindow, new PauseWindowData()));
                _needPauseGame = false;
            }

            if (!_needResumeGame) return;
            
            _signalBus.Fire(new CloseWindowSignal(WindowKey.PauseWindow));
            _needResumeGame = false;
        }

        public override async UniTask Dispose() {
            _updateService.UnregisterUpdate(this);
            _timerService.RemoveTimer(TurnId);
            ApplicationQuit.UnSubscribeOnQuit(PauseGame);
            ApplicationQuit.UnSubscribeOnResume(ResumeGame);
        }

        private void CaptureChecker(TurnData turnData) {
            if (!turnData.Capture) return;
            
            bool isYourTurn = _sceneSettings.TurnHandler.Turn == _sceneSettings.TurnHandler.YourColor;
            View.GetLostCheсker(isYourTurn);
        }

        private void TurnChange() {
            _timerService.ResetTimer(TurnId, _appConfig.TurnTime);
            _remainTime = _appConfig.TurnTime;
        }

        private void TurnTimeOut() {
            var winner = _sceneSettings.TurnHandler.Turn == PawnColor.Black ? PawnColor.Black : PawnColor.White;
            _sceneSettings.TurnHandler.OnEndGame?.Invoke(winner, WinLoseReason.Timeout);
        }

        private void PauseTimeOut() {
            _sceneSettings.TurnHandler.OnEndGame?.Invoke(_sceneSettings.TurnHandler.YourColor, WinLoseReason.Timeout);
        }

        private async UniTask SetBarsPosition() {
            await UniTask.Delay(50);
           
            bool isWhite = _sceneSettings.TurnHandler.StartingPawnColor == _sceneSettings.TurnHandler.YourColor;
            View.ResetBars(isWhite);

            _sceneSettings._playerBar = View.GetSendPawnPosition(isWhite);
            _sceneSettings._opponentBar = View.GetSendPawnPosition(!isWhite);
        }

        private async void PauseGame() {
            _remainTime = _timerService.GetTime(TurnId);
            _timerService.RemoveTimer(TurnId);

            var continueTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            _pauseStartTime = continueTime;

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

            if (!content.TryGetValue("Pause", out var pauseValue)) return;

            if (!string.IsNullOrEmpty(pauseValue)) {
                _remainTime = _timerService.GetTime(TurnId);
                _timerService.RemoveTimer(TurnId);

                _needPauseGame = true;
                return;
            }
                
            _needResumeGame = true;
                
            _timerService.StartTimer(TurnId, _remainTime, TurnTimeOut, false, View.SetTimerTime);
        }

        private async void ResumeGame() {
            var continueTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            
            if (continueTime - _pauseStartTime >= _appConfig.PauseTime) {
                PauseTimeOut();
                return;
            }
            
            _timerService.StartTimer(TurnId, _remainTime, TurnTimeOut, false, View.SetTimerTime);
            var opponentUserId = !string.IsNullOrEmpty(_appConfig.OpponentUserId)
                ? _appConfig.OpponentUserId
                : _globalScope.ApproveSenderId;
            
            await _messageService.SendPauseInfo(opponentUserId, "");

        }
    }
}