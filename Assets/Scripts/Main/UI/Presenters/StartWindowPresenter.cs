﻿using System;
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
using Global.Services;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.UI.Data;
using Global.Window;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Main.ConfigTemplate;
using Main.UI.Data;
using Main.UI.Data.LeaderboardWindow;
using Main.UI.Data.WaitForPlayerWindow;
using Main.UI.Views.Base;
using Main.UI.Views.Implementations;
using Nakama;
using Server;
using Server.Services;
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
        private GlobalScope _globalScope;
        private MessageService _messageService;

        private string _tournamentId = "4ec4f126-3f9d-11e7-84ef-b7c182b36521";

        private List<UserInfoData> _userInfoDatas;
        private IApiGroup _globalGroupInfo;

        private Action<UserInfoData, StartWindowUserCellView> _onSendInviteClick;

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
            _globalScope = Resolve<GlobalScope>(GameContext.Project);
            _messageService = Resolve<MessageService>(GameContext.Project);
        }

        public override async UniTask InitializeOnce() {
            View.Init();
            View.OnTextChange(OnUsersUpdate);
        }

        protected override async UniTask LoadContent() {
            ApplicationQuit.SubscribeOnQuit(GoOffline);
            ApplicationQuit.SubscribeOnResume(GoOnline);

            var channel = await _nakamaService.JoinChatByName(_appConfig.GlobalGroupName);
            
            _messageService.InitializeGlobalChannel(channel);
            
            _globalGroupInfo = await _nakamaService.GetGroupInfo(_appConfig.GlobalGroupName);

            _userInfoDatas = new List<UserInfoData>();

            _updateService.RegisterUpdate(this);

            var tournament = await _nakamaService.GetTournament(_tournamentId);
            
            await _nakamaService.JoinTournament(_tournamentId);
            
            var wins = await _nakamaService.ListStorageObjects<PlayerResults>("players", "wins");
            var loses = await _nakamaService.ListStorageObjects<PlayerResults>("players", "loses");

            View.SetWinsCount(wins.Data.Count);
            View.SetLosesCount(loses.Data.Count);
            
            View.OnStartClick(OnStartClick);
            View.OnLeaderboardClick(OnLeaderboardClick);

            var whenEnded = tournament.GetRemainingTime();
            
            _timerService.StartTimer("tournamentTime", whenEnded, () => {
                View.SetTimeTournament("Недоступно");
            }, false, current => {
                var time = TimeSpan.FromSeconds(current);
                
                var timeToDisplay = time.ToString(@"hh\:mm\:ss");
                
                View.SetTimeTournament(timeToDisplay);
            });

            _onSendInviteClick = null;
            _onSendInviteClick += OnSendInviteClick;
            
            View.SetScrollerDelegate(this);

            OnUsersUpdate();
            _timerService.StartTimer("updateUsersTimer", 10, OnUsersUpdate, true);
            
            _globalScope.SendedInvites.Clear();
        }

        private async void GoOffline() {
            await _nakamaService.GoOffline();
        }

        private async void GoOnline() {
            await _nakamaService.GoOnline();
        }

        private void OnStartClick() {
            _signalBus.Fire(new OpenWindowSignal(WindowKey.WaitForPlayerWindow, new WaitForPlayerWindowData()));
        }

        private void OnLeaderboardClick() {
            _signalBus.Fire(new OpenWindowSignal(WindowKey.LeaderboardWindow, new LeaderboardWindowData()));
        }

        public void CustomUpdate() {
            CheckInvite();

            CheckNeedLoad();
        }

        private async void CheckNeedLoad() {
            if (!_globalScope.ApprovedMatchAndNeedLoad) return;
            _globalScope.ApprovedMatchAndNeedLoad = false;

            if (!_globalScope.SendedInvites.ContainsKey(_appConfig.OpponentUserId)) return;
            
            var inviteData = _globalScope.SendedInvites[_appConfig.OpponentUserId];
            _appConfig.OpponentDisplayName = inviteData.DisplayName;
            await DeclineAllReceivedSignals();
            await DeclineAllSendedSignals();
            _globalScope.SendedInvites.Clear();
            await _nakamaService.RemoveAllPartiesExcept(_appConfig.OpponentUserId);
            await LoadParty();
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

        private async UniTask DeclineAllReceivedSignals()
        {
            UniTask[] tasks = new UniTask[_globalScope.ReceivedInvites.Count];
            int i = 0;
            foreach (var pair in _globalScope.ReceivedInvites)
            {
                tasks[i] = _messageService.SendDeclineInviteReceived(pair.Key);
                i++;
            }
            
            _globalScope.ReceivedInvites.Clear();

            await UniTask.WhenAll(tasks);
        }

        private async void CheckInvite() {
            if (_globalScope.ReceivedInvites.Count == 0) return;
            if (_windowService.IsWindowOpened(WindowKey.InviteWindow)) return;

            KeyValuePair<string, InviteData> inviteData = default;

            foreach (var invitePair in _globalScope.ReceivedInvites) {
                inviteData = invitePair;
                break;
            }

            _signalBus.Fire(new OpenWindowSignal(WindowKey.InviteWindow, new InviteWindowData {
                InviteData = inviteData.Value
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
                        DisplayName = username
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

        private async void OnSendInviteClick(UserInfoData data, StartWindowUserCellView view) {
            if (_globalScope.SendedInvites.ContainsKey(data.UserId)) return;
            
            view.SetSendText();
            
            var party = await _nakamaService.CreateParty();
            await _nakamaService.CreateMatch(party.Id);

            _appConfig.PawnColor = (int)PawnColor.White;

            await _messageService.SendPartyToUser(data.UserId, party);
            
            var inviteData = new InviteData {
                UserId = data.UserId,
                DisplayName = data.DisplayName,
                MatchId = party.Id
            };
            
            _globalScope.SendedInvites.Add(data.UserId, inviteData);
            
            _signalBus.Fire(new OpenWindowSignal(WindowKey.FlyText, new FlyTextData { FlyText = "отправлено" }));
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            StartWindowUserCellView view = scroller.GetCellView(_mainUIConfig.Prefab) as StartWindowUserCellView;

            var data = _userInfoDatas[dataIndex];
            
            view.Init();
            view.SetNickname(data.DisplayName);
            view.SubscribeOnClick(data, _onSendInviteClick);

            if (_globalScope.SendedInvites.ContainsKey(data.UserId)) {
                view.SetSendText();
            }

            return view;
        }

        public override async UniTask Dispose() {
            ApplicationQuit.UnSubscribeOnQuit(GoOffline);
            ApplicationQuit.UnSubscribeOnResume(GoOnline);
            
            _timerService.RemoveTimer("tournamentTime");
            _timerService.RemoveTimer("updateUsersTimer");

            _updateService.UnregisterUpdate(this);
        }
    }
}