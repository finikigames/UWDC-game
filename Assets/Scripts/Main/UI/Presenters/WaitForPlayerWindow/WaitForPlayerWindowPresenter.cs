using System.Collections.Generic;
using Core.Extensions;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using Global;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
using Global.Scheduler.Base;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Main.UI.Data;
using Main.UI.Data.WaitForPlayerWindow;
using Main.UI.Views.Base.WaitForPlayerWindow;
using Nakama;
using Nakama.TinyJson;
using Server.Services;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace Main.UI.Presenters.WaitForPlayerWindow {
    [Preserve]
    public class WaitForPlayerWindowPresenter : BaseWindowPresenter<IWaitForPlayerWindow, WaitForPlayerWindowData>,
                                                IUpdatable {
        private NakamaService _nakamaService;
        private IUpdateService _updateService;
        private SignalBus _signalBus;
        private AppConfig _appConfig;
        private TimerService _timerService;
        private ISchedulerService _schedulerService;

        private bool _needLoad;

        private IMatchmakerTicket _matchmakerTicket;
        private IMatchmakerMatched _matched;
        private IChannel _matchChannel;

        private int _matchmakingValue;
        private string _opponentId;

        public WaitForPlayerWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
            _signalBus = Resolve<SignalBus>(GameContext.Main);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
            _timerService = Resolve<TimerService>(GameContext.Project);
            _schedulerService = Resolve<ISchedulerService>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            _matchmakerTicket = await _nakamaService.AddMatchmaker();
            
            View.SubscribeToReturnButton(OnReturnClick);
            
            View.ShowReturnButton();

            var winsCount = await _nakamaService.ListStorageObjects<PlayerResults>("players", "wins");

            var me = _nakamaService.GetMe();
            View.SetYourName(me.User.DisplayName);
            View.SetYourWins(winsCount.Data.Count.ToString());
            View.SetOpponentName("?");
            View.SetOpponentWins("?");
            
            _updateService.RegisterUpdate(this);
            _nakamaService.SubscribeToMatchmakerMatched(OnMatchmakerMatched);
        }

        private void OnReturnClick() {
            CloseThisWindow();
        }

        public void CustomUpdate() {
            if (!_needLoad) return;
            _needLoad = false;

            AsyncLoad();
        }

        private async UniTask AsyncLoad() {
            View.HideReturnButton();

            _matchChannel = await _nakamaService.JoinChat(_matched.MatchId, ChannelType.DirectMessage, false);

            var users = _matched.Users;

            var me = _nakamaService.GetMe();
            string opponentId = string.Empty;
            foreach (var user in users) {
                if (user.Presence.UserId == me.User.Id) continue;

                opponentId = user.Presence.UserId;
                break;
            }

            _appConfig.OpponentUserId = opponentId;
            
            var opponentUserInfo = await _nakamaService.GetUserInfo(opponentId);

            var opponentWinsCount = await _nakamaService.ListStorageObjects<PlayerResults>("players", "wins", opponentId);
            View.SetOpponentWins(opponentWinsCount.Data.Count.ToString());
            
            _appConfig.Opponent = opponentUserInfo.DisplayName;
            View.SetOpponentName(opponentUserInfo.DisplayName);
            
            _nakamaService.SubscribeToMessages(OnChatMessage);
            CloseThisWindow();

            var value = Random.Range(0, 1000000);
            _matchmakingValue = value;
            _opponentId = opponentId;
            _nakamaService.SendMatchmakingInfo(opponentId, _matchmakingValue.ToString());

            _timerService.StartTimer("waiting_for_play", 5, null, false, time => View.SetTimerText(time.ToString()));
            
            _schedulerService
                .StartSequence()
                .Append(5, () => {
                    _timerService.RemoveTimer("waiting_for_play");
                    
                    StartLoad();
                });
        }

        private async UniTask StartLoad() {
            var matchId = _matched.MatchId;
            await _nakamaService.CreateMatch(matchId);
            PlayerPrefsX.SetBool("Matchmaking", true);
            _signalBus.Fire(new ToCheckersMetaSignal{WithPlayer = true});
        }

        private void OnChatMessage(IApiChannelMessage message) {
            var content = message.Content.FromJson<Dictionary<string, string>>();
            
            var profile = _nakamaService.GetMe();
            if (content.TryGetValue("TargetUser", out var targetUser)) {
                if (profile.User.Id != targetUser) return;
            }
            
            if (content.TryGetValue("ValueDropped", out var senderValue)) {
                if (_matchmakingValue > int.Parse(senderValue)) {
                    _appConfig.PawnColor = (int)PawnColor.White;
                    return;
                }

                if (_matchmakingValue < int.Parse(senderValue)) {
                    _appConfig.PawnColor = (int)PawnColor.Black;
                    return;
                }
                
                var value = Random.Range(0, 1000000);
                _matchmakingValue = value;
                _nakamaService.SendMatchmakingInfo(_opponentId, _matchmakingValue.ToString());
            }
        }

        private void OnMatchmakerMatched(IMatchmakerMatched matched) {
            _needLoad = true;
            _matched = matched;
            _matchmakerTicket = null;
            
            View.HideReturnButton();
        }
        
        public override async UniTask Dispose() {
            _updateService.UnregisterUpdate(this);
            if (_matchmakerTicket != null) {
                await _nakamaService.RemoveMatchmaker(_matchmakerTicket);
            }

            _nakamaService.UnsubscribeFromMessages(OnChatMessage);
            _nakamaService.UnsubscribeMatchmakerMatched(OnMatchmakerMatched);
        }
    }
}