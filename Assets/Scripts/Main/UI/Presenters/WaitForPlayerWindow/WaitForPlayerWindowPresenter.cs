using System.Collections.Generic;
using Core.Extensions;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using Global;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
using Global.Scheduler.Base;
using Global.Services;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Main.UI.Data;
using Main.UI.Data.WaitForPlayerWindow;
using Main.UI.Views.Base.WaitForPlayerWindow;
using Nakama;
using Nakama.TinyJson;
using Server;
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
        private MessageService _messageService;
        private GlobalScope _globalScope;

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
            _messageService = Resolve<MessageService>(GameContext.Project);
            _globalScope = Resolve<GlobalScope>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            ApplicationQuit.SubscribeOnQuit(CloseThisWindow);
            _matchmakerTicket = await _nakamaService.AddMatchmaker();
            
            View.SubscribeToReturnButton(OnReturnClick);

            _appConfig.InSearch = true;
            
            View.ShowReturnButton();

            await DeclineAllSendedSignals();
            var winsCount = await _nakamaService.ListStorageObjects<PlayerResults>("players", "wins");

            var me = _nakamaService.GetMe();
            View.SetYourName(me.User.DisplayName);
            View.SetYourWins(winsCount.Data.Count.ToString());
            View.SetOpponentName("?");
            View.SetOpponentWins("?");
            
            _updateService.RegisterUpdate(this);
            _nakamaService.SubscribeToMatchmakerMatched(OnMatchmakerMatched);
            
            _timerService.StartUpTimer("waiting_for_play", 99, null, false, time => View.SetTimerText($"поиск матча: {time.ToString()}"));
        }

        private void OnReturnClick() {
            CloseThisWindow();
        }

        public void CustomUpdate()
        {
            if (!_needLoad) return;
            _needLoad = false;

            AsyncLoad();
        }

        private async UniTask AsyncLoad() {
            string matchId = string.Empty;
            await _nakamaService.GoOffline();
            
            foreach (var user in _matched.Users) {
                matchId += user.Presence.Username;
            }
            await _nakamaService.CreateMatch(matchId);
            
            View.HideReturnButton();

            var users = _matched.Users;

            var me = _nakamaService.GetMe();
            string opponentId = string.Empty;
            foreach (var user in users) {
                if (user.Presence.UserId == me.User.Id) continue;

                opponentId = user.Presence.UserId;
                break;
            }

            _appConfig.OpponentUserId = opponentId;
            
            _nakamaService.SubscribeToMessages(OnChatMessage);

            var value = Random.Range(0, 1000000);
            _matchmakingValue = value;
            
            Debug.Log($"[Color getter] Get a random value of {value}");
            
            _opponentId = opponentId;
            await _messageService.SendMatchmakingInfo(opponentId, _matchmakingValue.ToString());
            
            var opponentUserInfo = await _nakamaService.GetUserInfo(opponentId);

            var opponentWinsCount = await _nakamaService.ListStorageObjects<PlayerResults>("players", "wins", opponentId);
            View.SetOpponentWins(opponentWinsCount.Data.Count.ToString());
            
            _appConfig.OpponentDisplayName = opponentUserInfo.DisplayName;
            View.SetOpponentName(opponentUserInfo.DisplayName);

            _timerService.RemoveTimer("waiting_for_play");
            _timerService.StartTimer("await_start_game", 5, null, false, time => View.SetTimerText(time.ToString()));
            
            _schedulerService
                .StartSequence()
                .Append(5, () => {
                    CloseThisWindow();
                    Debug.Log("[Color getter] Started loading");
                    _timerService.RemoveTimer("await_start_game");
                    
                    StartLoad();
                });
        }
        
        private async UniTask DeclineAllSendedSignals() {
            UniTask[] tasks = new UniTask[_globalScope.SendedInvites.Count];
            int i = 0;
            foreach (var pair in _globalScope.SendedInvites)
            {
                tasks[i] = _messageService.SendDeclineInviteSended(pair.Key);
                i++;
            }

            _globalScope.SendedInvites.Clear();
            await UniTask.WhenAll(tasks);
        }

        private void StartLoad() {
            PlayerPrefsX.SetBool("Matchmaking", true);
            _signalBus.Fire(new ToCheckersMetaSignal{WithPlayer = true});
        }

        private async void OnChatMessage(IApiChannelMessage message) {
            var content = message.Content.FromJson<Dictionary<string, string>>();
            
            var profile = _nakamaService.GetMe();
            if (content.TryGetValue("senderUserId", out var senderUserId)) {
                if (profile.User.Id == senderUserId) return;
            }
            
            if (content.TryGetValue("targetUserId", out var targetUser)) {
                if (profile.User.Id != targetUser) return;
            }
            
            if (content.TryGetValue("valueDropped", out var senderValue)) {
                Debug.Log($"[Color getter] Opponent value {senderValue}, my value {_matchmakingValue}");
                
                if (_matchmakingValue > int.Parse(senderValue)) {
                    Debug.Log("[Color getter] Setting white");
                    _appConfig.PawnColor = (int)PawnColor.White;
                    return;
                }

                if (_matchmakingValue < int.Parse(senderValue)) {
                    Debug.Log("[Color getter] Setting black");
                    _appConfig.PawnColor = PawnColor.Black;
                    return;
                }
                
                Debug.Log("[Color getter] From equal");
                
                var value = Random.Range(0, 1000000);
                _matchmakingValue = value;
                await _messageService.SendMatchmakingInfo(_opponentId, _matchmakingValue.ToString());
            }
        }

        private void OnMatchmakerMatched(IMatchmakerMatched matched) {
            _needLoad = true;
            _matched = matched;
            _matchmakerTicket = null;
            
            View.HideReturnButton();
        }
        
        public override async UniTask Dispose() {
            ApplicationQuit.UnSubscribeOnQuit(CloseThisWindow);
            _timerService.RemoveTimer("waiting_for_play");
            _updateService.UnregisterUpdate(this);
            if (_matchmakerTicket != null) {
                await _nakamaService.RemoveMatchmaker(_matchmakerTicket);
            }

            _appConfig.InSearch = false;
            _nakamaService.UnsubscribeFromMessages(OnChatMessage);
            _nakamaService.UnsubscribeMatchmakerMatched(OnMatchmakerMatched);
        }
    }
}