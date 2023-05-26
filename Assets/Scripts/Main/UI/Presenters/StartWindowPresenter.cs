using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Global.Context;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
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
                                        IEnhancedScrollerDelegate {
        private readonly NakamaService _nakamaService;
        private readonly TimerService _timerService;
        private readonly MainUIConfig _mainUIConfig;

        private string _globalGroupName = "globalGroup";

        private List<UserInfoData> _userInfoDatas;
        private IApiGroup _globalGroupInfo;

        private Action<string> _onUserPlayClick;

        public StartWindowPresenter(ContextService service) : base(service) {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _timerService = Resolve<TimerService>(GameContext.Project);
            _mainUIConfig = Resolve<MainUIConfig>(GameContext.Main);
        }

        protected override async UniTask LoadContent() {
            var group = await _nakamaService.CreateGroup(_globalGroupName);
            await _nakamaService.JoinGroup(group.Id);
            _globalGroupInfo = await _nakamaService.GetGroupInfo(_globalGroupName);

            _userInfoDatas = new List<UserInfoData>();

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
            await _nakamaService.SendPartyToUser(userId, party);
        }

        private void PartyPresenceListener(IPartyPresenceEvent presenceEvent) {
            
        }

        private async void MessagesListener(IApiChannelMessage m) {
            var content = m.Content.FromJson<Dictionary<string, string>>();
            if (content.TryGetValue("partyId", out var value)) {
                Debug.Log($"Get a party with a id {value}");
                await _nakamaService.JoinParty(value);
            }
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
            _nakamaService.UnsubscribeFromMessages(MessagesListener);
            _nakamaService.UnsubscribeFromPartyPresence(PartyPresenceListener);
        }
    }
}