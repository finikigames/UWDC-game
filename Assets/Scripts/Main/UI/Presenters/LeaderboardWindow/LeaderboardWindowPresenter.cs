using System.Collections.Generic;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Main.ConfigTemplate;
using Main.UI.Data.LeaderboardWindow;
using Main.UI.Views.Base.LeaderboardWindow;
using Main.UI.Views.Implementations.LeaderboardWindow;
using Server.Services;
using UnityEngine.Scripting;

namespace Main.UI.Presenters.LeaderboardWindow {
    [Preserve]
    public class LeaderboardWindowPresenter : BaseWindowPresenter<ILeaderboardWindow, LeaderboardWindowData>,
                                              IEnhancedScrollerDelegate,
                                              IUpdatable {
        private MainUIConfig _mainUIConfig;
        private NakamaService _nakamaService;
        private IUpdateService _updateService;
        
        private List<LeaderboardInfoData> _userInfoDatas;
        private int _meIndex;
        
        private string _tournamentId = "4ec4f126-3f9d-11e7-84ef-b7c182b36521";
        
        public LeaderboardWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _mainUIConfig = Resolve<MainUIConfig>(GameContext.Main);
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            _userInfoDatas = new List<LeaderboardInfoData>();
            
            _updateService.RegisterUpdate(this);
            
            View.SetScrollerDelegate(this);
            
            var list = await _nakamaService.ListTournamentRecordsAroundOwner(_tournamentId, null);

            int i = 0;
            foreach (var record in list.Records) {
                string score = record.Score;
                if (string.IsNullOrEmpty(record.Score)) {
                    score = "0";
                }

                if (_nakamaService.GetMe().User.Id == record.OwnerId) {
                    _meIndex = i;
                }
                
                var leaderboardInfo = new LeaderboardInfoData {
                    Rank = record.Rank,
                    Nickname = record.Username,
                    Score = score,
                    OwnerId = record.OwnerId
                };
                
                _userInfoDatas.Add(leaderboardInfo);
                i++;
            }

            View.ReloadData();
            View.JumpToIndex(_meIndex);
        }

        public int GetNumberOfCells(EnhancedScroller scroller) {
            return _userInfoDatas.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
            return _mainUIConfig.LeaderboardPrefab.Height;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            LeaderboardUserCellView view = scroller.GetCellView(_mainUIConfig.LeaderboardPrefab) as LeaderboardUserCellView;

            var data = _userInfoDatas[dataIndex];

            if (data.Rank == "1") {
                view.ChangeSprite(_mainUIConfig.GoldFrame);
            }
            else if (data.Rank == "2") {
                view.ChangeSprite(_mainUIConfig.SilverFrame);
            }
            else if (data.Rank == "3") {
                view.ChangeSprite(_mainUIConfig.BronzeFrame);
            }
            else {
                view.ChangeSprite(_mainUIConfig.NormalFrame);
            }
            
            view.SetYouFrame(_mainUIConfig.YourFrame, false);
            
            if (_nakamaService.GetMe().User.Id == data.OwnerId) {
                view.SetYouFrame(_mainUIConfig.YourFrame, true);
            }
            
            view.Init($"{data.Rank}. {data.Nickname}", data.Score.ToString());

            return view;
        }

        public void CustomUpdate() {
            
        }

        public override void Dispose() {
            _updateService.UnregisterUpdate(this);
        }
    }
}