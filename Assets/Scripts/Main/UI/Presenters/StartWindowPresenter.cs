using System;
using System.Collections.Generic;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
using Global.Services;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Main.ConfigTemplate;
using Main.UI.Data;
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
        private ProfileGetService _profileService;
        private IUpdateService _updateService;
        private AppConfig _appConfig;
        private SignalBus _signalBus;

        private string _globalGroupName = "globalGroup";

        private List<UserInfoData> _userInfoDatas;
        private IApiGroup _globalGroupInfo;

        private Action<string> _onUserPlayClick;
        private Action<string> _onOpponentFind;

        private bool _unhandledInvite;
        private bool _needLoad;
        private string _partyId;
        private string _inviteDisplayName;
        private string _approveSenderId;
        private string _inviteSenderUserId;

        public StartWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _signalBus = Resolve<SignalBus>(GameContext.Main);
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _timerService = Resolve<TimerService>(GameContext.Project);
            _mainUIConfig = Resolve<MainUIConfig>(GameContext.Main);
            _profileService = Resolve<ProfileGetService>(GameContext.Project);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
        }

        public override async UniTask InitializeOnce()
        {
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
            
            _onOpponentFind = null;
            _onOpponentFind += (name) => _appConfig.Opponent = name;
            
            View.SetScrollerDelegate(this);

            _nakamaService.SubscribeToMessages(MessagesListener);
            _nakamaService.SubscribeToPartyPresence(PartyPresenceListener);
            
            OnUsersUpdate();
            _timerService.StartTimer("updateUsersTimer", 10, OnUsersUpdate, true);
        }

        private async void SendPartyToUser(string userId) {
            var party = await _nakamaService.CreateParty();
            await _nakamaService.CreateMatch(party.Id);

            _appConfig.PawnColor = (int)PawnColor.White;
            await _nakamaService.SendPartyToUser(userId, party);

            //_signalBus.Fire(new CloseWindowSignal(WindowKey.StartWindow));
            //_signalBus.Fire(new ToCheckersMetaSignal{WithPlayer = true});
        }

        private void PartyPresenceListener(IPartyPresenceEvent presenceEvent) {
            
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
            _signalBus.Fire(new CloseWindowSignal(WindowKey.StartWindow));
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

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            StartWindowUserCellView view = scroller.GetCellView(_mainUIConfig.Prefab) as StartWindowUserCellView;

            var data = _userInfoDatas[dataIndex];
            view.SetNickname(data.Username);
            view.SubscribeOnClick(data.UserId, _onUserPlayClick, _onOpponentFind);
            
            return view;
        }

        public override void Dispose() {
            _timerService.RemoveTimer("updateUsersTimer");
            
            _updateService.UnregisterUpdate(this);
            
            _nakamaService.UnsubscribeFromMessages(MessagesListener);
            _nakamaService.UnsubscribeFromPartyPresence(PartyPresenceListener);
        }
    }
}