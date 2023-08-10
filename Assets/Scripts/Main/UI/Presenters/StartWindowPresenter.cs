using System;
using System.Collections.Generic;
using System.Linq;
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
using Global.UI.Data;
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
using Server;
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
        private GlobalScope _globalScope;
        private MessageService _messageService;
        private TournamentsScheduleConfig _tournamentsConfig;

        private string _globalGroupName = "globalGroup";
        private string _tournamentId = "4ec4f126-3f9d-11e7-84ef-b7c182b36521";

        private TournamentsScheduleData _currentTournament;
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
            _tournamentsConfig = Resolve<TournamentsScheduleConfig>(GameContext.Project);
        }

        public override async UniTask InitializeOnce() {
            View.Init();
            View.OnTextChange(OnUsersUpdate);
        }

        protected override async UniTask LoadContent()
        {
            _timerService.ResetTimer("nearestTournamentTime");
            _timerService.ResetTimer("tournamentTime");
            
            var group = await _nakamaService.CreateGroup(_globalGroupName);
            await _nakamaService.JoinGroup(group.Id);
            var channel = await _nakamaService.JoinChat(group.Id);
            
            _messageService.InitializeGlobalChannel(channel);
            _globalGroupInfo = await _nakamaService.GetGroupInfo(_globalGroupName);

            _userInfoDatas = new List<UserInfoData>();

            _updateService.RegisterUpdate(this);

            var tournament = await _nakamaService.GetTournament(_tournamentId);
            
            await _nakamaService.JoinTournament(_tournamentId);
            
            var wins = await _nakamaService.ListStorageObjects<PlayerResults>("players", "wins");
            var loses = await _nakamaService.ListStorageObjects<PlayerResults>("players", "loses");

            View.SetWinsCount(wins.Data.Count);
            View.SetLosesCount(loses.Data.Count);
            
            View.OnStartClick(OnStartClick);
            
            SetTimer(tournament);

            _onSendInviteClick = null;
            _onSendInviteClick += OnSendInviteClick;
            
            View.SetScrollerDelegate(this);

            OnUsersUpdate();
            _timerService.StartTimer("updateUsersTimer", 10, OnUsersUpdate, true);
            
            _globalScope.SendedInvites.Clear();
        }

        private bool HasActiveTournament() {            
            foreach (var tournamentData in _tournamentsConfig.Datas) {
                if (tournamentData.Day == (int)DateTime.Now.DayOfWeek) {
                    var start = DateTime.Parse(tournamentData.StartTime);
                    var end = DateTime.Parse(tournamentData.FinishTime);

                    if (start <= DateTime.UtcNow && end > DateTime.UtcNow) {
                        View.EnablePlayButton();
                        _currentTournament = tournamentData;
                        return true;
                    }
                }
                else {
                    View.DisablePlayButton();
                }
            }

            return false;
        }

        private void SetTimer(IApiTournament tournament) {
            var whenEnded = tournament.GetRemainingTime();

            if (HasActiveTournament()) {
                _timerService.StartTimer("tournamentTime", whenEnded, () => { }, 
                    false, 
                    current => {
                        var diff = DateTime.Parse(_currentTournament.FinishTime) - DateTime.UtcNow;
                        var timeToDisplay = String.Format("{0:D2}:{1:D2}:{2:D2}", diff.Hours, diff.Minutes, diff.Seconds);
                        
                        View.EnablePlayButton();
                        View.SetTimeTournament(timeToDisplay); 
                    });
            }
            else {
                _timerService.StartTimer("nearestTournamentTime", whenEnded, () => {
                        _timerService.StartTimer("tournamentTime", whenEnded, () => { }, 
                            false, 
                            current => {
                                var diff = DateTime.Parse(_currentTournament.FinishTime) - DateTime.UtcNow;
                                var timeToDisplay = String.Format("{0:D2}:{1:D2}:{2:D2}", diff.Hours, diff.Minutes, diff.Seconds);
                        
                                View.EnablePlayButton();
                                View.SetTimeTournament(timeToDisplay); 
                            });
                    }, 
                    false, 
                    current => {
                        var nearest = FindNearestDate();
                        var diff = nearest - DateTime.UtcNow;
                        var timeToDisplay = String.Format("{0:D2}:{1:D2}:{2:D2}", diff.Hours, diff.Minutes, diff.Seconds);
                        View.SetTimeTournament("До начала: " + timeToDisplay);
                    });
            }
        }

        private DateTime FindNearestDate() {
            var nearest = DateTime.Parse(_tournamentsConfig.Datas[0].StartTime);
            _currentTournament = _tournamentsConfig.Datas[0];

            var list = _tournamentsConfig.Datas.OrderBy(z => z.Day)
                .ToList();

            var forwardList = list.Where(x => x.Day >= (int) DateTime.UtcNow.DayOfWeek)
                .OrderBy(y => y.StartTime)
                .ToList();

            if (forwardList.Count != 0) {
                nearest = DateTime.Parse(forwardList.First().StartTime);
                _currentTournament = forwardList.First();
            }
            else {
                nearest = DateTime.Parse(list.First().StartTime);
                _currentTournament = list.First();
            }

            return nearest;
        }

        private void OnStartClick() {
            _signalBus.Fire(new OpenWindowSignal(WindowKey.WaitForPlayerWindow, new WaitForPlayerWindowData()));
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

            _globalScope.ReceivedInvites.Remove(inviteData.Key);

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
            _timerService.RemoveTimer("tournamentTime");
            _timerService.RemoveTimer("updateUsersTimer");

            _updateService.UnregisterUpdate(this);
        }
    }
}