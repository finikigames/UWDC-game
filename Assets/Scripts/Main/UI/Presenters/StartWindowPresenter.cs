using System;
using System.Collections.Generic;
using Checkers.UI.Data;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Global.Context;
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

namespace Main.UI.Presenters {
    [Preserve]
    public class StartWindowPresenter : BaseWindowPresenter<IStartWindow, StartWindowData>,
                                        IEnhancedScrollerDelegate,
                                        IUpdatable {
        private readonly NakamaService _nakamaService;
        private readonly TimerService _timerService;
        private readonly MainUIConfig _mainUIConfig;
        private readonly ProfileGetService _profileService;
        private readonly IUpdateService _updateService;

        private string _globalGroupName = "globalGroup";

        private List<UserInfoData> _userInfoDatas;
        private IApiGroup _globalGroupInfo;

        private Action<string> _onUserPlayClick;

        private bool _needPartyLoad;
        private string _partyId;

        public StartWindowPresenter(ContextService service) : base(service) {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _timerService = Resolve<TimerService>(GameContext.Project);
            _mainUIConfig = Resolve<MainUIConfig>(GameContext.Main);
            _profileService = Resolve<ProfileGetService>(GameContext.Project);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            var group = await _nakamaService.CreateGroup(_globalGroupName);
            await _nakamaService.JoinChat(group.Id);
            await _nakamaService.JoinGroup(group.Id);
            _globalGroupInfo = await _nakamaService.GetGroupInfo(_globalGroupName);

            _userInfoDatas = new List<UserInfoData>();

            _updateService.RegisterUpdate(this);
            
            _onUserPlayClick = null;
            _onUserPlayClick += SendPartyToUser;
            
            View.SetScrollerDelegate(this);

            _nakamaService.SubscribeToMessages(MessagesListener);
            _nakamaService.SubscribeToPartyPresence(PartyPresenceListener);
            
            
            
            OnUsersUpdate();
            _timerService.StartTimer("updateUsersTimer", 10, OnUsersUpdate, true);
        }

        private async void SendPartyToUser(string userId) {
            var party = await _nakamaService.CreateParty();
            await _nakamaService.CreateMatch(party.Id);
            await _nakamaService.SendPartyToUser(userId, party);
            
            FireSignal(new CloseWindowSignal(WindowKey.StartWindow));
            FireSignal(new ToCheckersMetaSignal{WithPlayer = true});
        }

        private void PartyPresenceListener(IPartyPresenceEvent presenceEvent) {
            
        }

        private async void MessagesListener(IApiChannelMessage m) {
            var content = m.Content.FromJson<Dictionary<string, string>>();

            if (content.TryGetValue("senderUserId", out var currentUserId)) {
                var profile = _profileService.GetProfile();

                if (profile.UserId == currentUserId) return;
            }
            
            if (content.TryGetValue("partyId", out var value)) {
                _partyId = value;
                _needPartyLoad = true;
                Debug.Log($"Get a party with a id {value}");
            }
        }

        public void CustomUpdate() {
            if (!_needPartyLoad) return;
            _needPartyLoad = false;

            LoadParty();
        }

        private async UniTask LoadParty() {
            await _nakamaService.JoinParty(_partyId);
            await _nakamaService.CreateMatch(_partyId);
                
            FireSignal(new CloseWindowSignal(WindowKey.StartWindow));
            FireSignal(new ToCheckersMetaSignal{WithPlayer = true});
        }
        
        private async void OnUsersUpdate() {
            await GetUsers();
        }

        private async UniTask GetUsers() {
            var groupId = _globalGroupInfo.Id;
            var users = await _nakamaService.GetGroupUsersWithoutMe(groupId, 100);

            _userInfoDatas.Clear();
            
            foreach (var user in users) {
                if (!user.User.Online) continue;

                var id = user.User.Id;
                var username = user.User.Username;

                var userInfo = new UserInfoData {
                    UserId = id,
                    Username = username
                };
                
                _userInfoDatas.Add(userInfo);
            }
            
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
            view.SubscribeOnClick(data.UserId, _onUserPlayClick);
            
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