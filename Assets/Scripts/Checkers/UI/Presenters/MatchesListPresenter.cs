using System.Collections.Generic;
using System.Linq;
using Checkers.ConfigTemplate;
using Checkers.UI.Data;
using Checkers.UI.Views.Implementations;
using Checkers.UI.Views.Interfaces;
using Core.Extensions;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Nakama;
using Server.Services;
using UnityEngine;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class MatchesListPresenter : BaseWindowPresenter<IMatchesListView, MatchesListData>,
                                        IEnhancedScrollerDelegate {
        private readonly CheckersUIConfig _checkersConfig;
        private readonly NakamaService _nakamaService;
        private List<IApiMatch> _matchesList;

        private string _selectedMatchId;
        private CheckersMatchListElementView _currentElement;

        private IMatch _match;
        
        public MatchesListPresenter(ContextService service) : base(service) {
            _checkersConfig = Resolve<CheckersUIConfig>(GameContext.Project);
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
        }

        protected override async UniTask InitializeData() {
            
        }

        protected override async UniTask LoadContent() {
            View.SetScrollerDelegate(this);
        }

        public override async UniTask InitializeOnce() {
            View.OnMatchCreate += OnMatchCreate;
            View.OnMatchJoin += OnMatchJoin;
            View.OnMatchesRefresh += RefreshMatches;

            LoadMatches();
        }

        private async void RefreshMatches() {
            await LoadMatches();
        }

        private async UniTask LoadMatches() {
            var matches = await _nakamaService.GetMatchesList();
            _matchesList = matches.Matches.ToList();

            View.ApplyData();
        }

        private async void OnMatchCreate() {
            _match = await _nakamaService.CreateMatch(SystemInfo.deviceModel);

            PlayerPrefsX.SetEnum("YourColor", PawnColor.White);
            await _nakamaService.JoinMatch(_match.Id);
            
            FireSignal(new CloseWindowSignal(WindowKey.MatchesList));
            
            FireSignal(new ToCheckersMetaSignal{WithPlayer = true});
        }

        private async void OnMatchJoin() {
            if (string.IsNullOrEmpty(_selectedMatchId)) return;
            
            FireSignal(new CloseWindowSignal(WindowKey.MatchesList));
            
            PlayerPrefsX.SetEnum("YourColor", PawnColor.Black);
            PlayerPrefs.SetString("SelectedMatchId", _selectedMatchId);
            FireSignal(new ToCheckersMetaSignal{WithPlayer = true});
        }

        public int GetNumberOfCells(EnhancedScroller scroller) {
            return _matchesList?.Count ?? 0;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
            return _checkersConfig.ListPrefab.Height;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            CheckersMatchListElementView cell = scroller.GetCellView(_checkersConfig.ListPrefab) as CheckersMatchListElementView;

            var matchData = _matchesList[dataIndex];
            
            cell.Initialize(matchData.MatchId, OnChoose);

            return cell;
        }

        private void OnChoose(CheckersMatchListElementView cell, string matchId) {
            if (_selectedMatchId == matchId) {
                _currentElement.RemoveChoose();
                _selectedMatchId = string.Empty;
            }
            else {
                if (_currentElement != null) {
                    _currentElement.RemoveChoose();
                }
                _currentElement = cell;
                    
                cell.SetChoose();

                _selectedMatchId = matchId;
            }
        }
    }
}