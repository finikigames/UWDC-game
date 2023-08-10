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

            foreach (var record in list.Records) {
                var leaderboardInfo = new LeaderboardInfoData {
                    Rank = record.Rank,
                    Nickname = record.Username,
                    Score = record.Score
                };
                
                _userInfoDatas.Add(leaderboardInfo);
            }
            
            View.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller) {
            return _userInfoDatas.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
            return _mainUIConfig.LeaderboardPrefab.Height;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            LeaderboardUserCellView view = scroller.GetCellView(_mainUIConfig.Prefab) as LeaderboardUserCellView;

            var data = _userInfoDatas[dataIndex];
            
            view.Init($"{data.Rank}. {data.Nickname}", data.Score.ToString());

            return view;
        }

        public void CustomUpdate() {
            
        }

        public override async UniTask Dispose() {
            _updateService.UnregisterUpdate(this);
        }
    }
}