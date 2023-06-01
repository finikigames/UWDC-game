using System;
using System.Collections.Generic;
using Core.Extensions;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Global;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
using Global.Extensions;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.Window;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Main.ConfigTemplate;
using Main.UI.Data;
using Main.UI.Data.WaitForPlayerWindow;
using Main.UI.Views.Base;
using Main.UI.Views.Implementations;
using Nakama;
using Nakama.TinyJson;
using Server.Services;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace Main.UI.Presenters {
    [Preserve]
    public class StartWindowPresenter : BaseWindowPresenter<IStartWindow, StartWindowData>,
                                        IEnhancedScrollerDelegate,
                                        IUpdatable {
        private NakamaService _nakamaService;
        private TimerService _timerService;
        private MainUIConfig _mainUIConfig;
        private IUpdateService _updateService;
        private AppConfig _appConfig;
        private SignalBus _signalBus;
        private WindowService _windowService;

        private string _globalGroupName = "globalGroup";
        private string _tournamentId = "4ec4f126-3f9d-11e7-84ef-b7c182b36521";

        private List<UserInfoData> _userInfoDatas;
        private IApiGroup _globalGroupInfo;

        private Action<string> _onUserPlayClick;
        private Action<string> _onOpponentFind;
        private Action<StartWindowUserCellView> _onSendInvite;

        private bool _unhandledInvite;
        private bool _needLoad;
        private string _partyId;
        private string _inviteDisplayName;
        private string _approveSenderId;
        private string _inviteSenderUserId;

        private HashSet<StartWindowUserCellView> _hashSet = new();

        public StartWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _signalBus = Resolve<SignalBus>(GameContext.Main);
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _timerService = Resolve<TimerService>(GameContext.Project);
            _mainUIConfig = Resolve<MainUIConfig>(GameContext.Main);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
            _windowService = Resolve<WindowService>(GameContext.Project);
        }

        public override async UniTask InitializeOnce() {
            View.Init();
            View.OnTextChange(OnUsersUpdate);
        }

        protected override async UniTask LoadContent() {
            var group = await _nakamaService.CreateGroup(_globalGroupName);
            await _nakamaService.JoinGroup(group.Id);
            await _nakamaService.JoinChat(group.Id);
            _globalGroupInfo = await _nakamaService.GetGroupInfo(_globalGroupName);

            _userInfoDatas = new List<UserInfoData>();

            _updateService.RegisterUpdate(this);
            
            _onUserPlayClick = null;
            _onUserPlayClick += SendPartyToUser;

            await _nakamaService.JoinTournament(_tournamentId);
            
            var wins = await _nakamaService.ListStorageObjects<PlayerResults>("players", "wins");
            var loses = await _nakamaService.ListStorageObjects<PlayerResults>("players", "loses");

            View.SetWinsCount(wins.Data.Count);
            View.SetLosesCount(loses.Data.Count);
            
            View.OnStartClick(OnStartClick);

            var tournament = await _nakamaService.GetTournament(_tournamentId);

            var whenEnded = tournament.GetRemainingTime();
            
            _timerService.StartTimer("tournamentTime", whenEnded, () => {
                View.SetTimeTournament("Недоступно");
            }, false, current => {
                var time = TimeSpan.FromSeconds(current);
                
                var timeToDisplay = time.ToString(@"hh\:mm\:ss");
                
                View.SetTimeTournament(timeToDisplay);
            });
            
            _onOpponentFind = null;
            _onOpponentFind += (name) => _appConfig.Opponent = name;
            
            _onSendInvite = null;
            _onSendInvite += SendInvite;
            
            View.SetScrollerDelegate(this);

            _nakamaService.SubscribeToMessages(MessagesListener);
            
            OnUsersUpdate();
            _timerService.StartTimer("updateUsersTimer", 10, OnUsersUpdate, true);
            
            _hashSet.Clear();
        }

        private void OnStartClick() {
            _signalBus.Fire(new OpenWindowSignal(WindowKey.WaitForPlayerWindow, new WaitForPlayerWindowData()));
        }

        private async void SendPartyToUser(string userId) {
            var party = await _nakamaService.CreateParty();
            await _nakamaService.CreateMatch(party.Id);

            _appConfig.PawnColor = (int)PawnColor.White;
            await _nakamaService.SendPartyToUser(userId, party);
            _appConfig.OpponentUserId = userId;
        }

        private void MessagesListener(IApiChannelMessage m) {
            var content = m.Content.FromJson<Dictionary<string, string>>();

            var profile = _nakamaService.GetMe();
            if (content.TryGetValue("senderUserId", out var senderUserId)) {
                if (profile.User.Id == senderUserId) return;
            }

            if (content.TryGetValue("targetUserId", out var targetUserId)) {
                if (profile.User.Id != targetUserId) return;
            }
            
            if (content.TryGetValue("approveMatchInvite", out var matchAndPartyId)) {
                _needLoad = true;
                _approveSenderId = content["senderUserId"];
            }
            
            if (content.TryGetValue("partyId", out var value)) {
                _partyId = value;
                _inviteSenderUserId = senderUserId;
                _inviteDisplayName = content["senderDisplayName"];
                _unhandledInvite = true;

                _appConfig.PawnColor = (int)PawnColor.Black;
                Debug.Log($"Get a party with a id {value}");
            }
        }

        public void CustomUpdate() {
            CheckInvite();

            CheckNeedLoad();
        }

        private async void CheckNeedLoad() {
            if (!_needLoad) return;
            _needLoad = false;

            await _nakamaService.RemoveAllPartiesExcept(_approveSenderId);
            await LoadParty();
        }

        private void CheckInvite() {
            if (!_unhandledInvite) return;
            _unhandledInvite = false;

            _signalBus.Fire(new OpenWindowSignal(WindowKey.InviteWindow, new InviteWindowData {
                PartyId = _partyId,
                DisplayName = _inviteDisplayName,
                SenderId = _inviteSenderUserId
            }));
        }

        private async UniTask LoadParty() {
            PlayerPrefsX.SetBool("Matchmaking", false);
            _signalBus.Fire(new ToCheckersMetaSignal{WithPlayer = true});
        }
        
        private async void OnUsersUpdate() {
            await GetUsers();
        }

        private async UniTask GetUsers() {
            var groupId = _globalGroupInfo.Id;
            var users = await _nakamaService.GetGroupUsersWithoutMe(groupId, 100);

            _userInfoDatas.Clear();
            
            int onlineCounter = 0;
            foreach (var user in users) {
                if (!user.User.Online) continue;

                var id = user.User.Id;
                var username = user.User.DisplayName;

                var usernameLower = username.ToLower();
                var searchingLower = View.SearchingPlayer.ToLower();
                
                if (usernameLower.Contains(searchingLower))
                {
                    var userInfo = new UserInfoData {
                        UserId = id,
                        Username = username
                    };

                    _userInfoDatas.Add(userInfo);
                }
                onlineCounter++;
            }

            View.SetAllMembersCount(users.Count);
            View.SetOnlineMembersCount(onlineCounter);
            View.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller) {
            return _userInfoDatas.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
            return _mainUIConfig.Prefab.Height;
        }

        private void SendInvite(StartWindowUserCellView view) {
            if (!_hashSet.Contains(view)) {
                _hashSet.Add(view);
            }
            
            view.SetSendText();
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            StartWindowUserCellView view = scroller.GetCellView(_mainUIConfig.Prefab) as StartWindowUserCellView;

            var data = _userInfoDatas[dataIndex];
            
            view.Init();
            view.SetNickname(data.Username);
            view.SubscribeOnClick(data.UserId, _onUserPlayClick, _onOpponentFind, _onSendInvite);

            if (_hashSet.Contains(view)) {
                view.SetSendText();
            }

            return view;
        }

        public override async UniTask Dispose() {
            _timerService.RemoveTimer("tournamentTime");
            _timerService.RemoveTimer("updateUsersTimer");
            
            _updateService.UnregisterUpdate(this);
            
            _nakamaService.UnsubscribeFromMessages(MessagesListener);
        }
    }
}