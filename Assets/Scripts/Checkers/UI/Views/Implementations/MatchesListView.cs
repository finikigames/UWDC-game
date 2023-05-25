using System;
using Checkers.UI.Views.Interfaces;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views.Implementations {
    public class MatchesListView : BaseWindow,
                                   IMatchesListView {
        [SerializeField]
        private CanvasGroup _group;

        [SerializeField]
        private Button _matchCreateButton;

        [SerializeField]
        private Button _matchJoinButton;

        [SerializeField]
        private Button _matchesRefreshButton;

        [SerializeField]
        private EnhancedScroller _scroller;

        public Action OnMatchCreate {
            get;
            set;
        }

        public Action OnMatchJoin {
            get;
            set;
        }

        public Action OnMatchesRefresh {
            get;
            set;
        }

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public override void Initialize() {
            _matchJoinButton.onClick.RemoveAllListeners();
            _matchJoinButton.onClick.AddListener(() => OnMatchJoin?.Invoke());
            
            _matchCreateButton.onClick.RemoveAllListeners();
            _matchCreateButton.onClick.AddListener(() => OnMatchCreate?.Invoke());
            
            _matchesRefreshButton.onClick.RemoveAllListeners();
            _matchesRefreshButton.onClick.AddListener(() => OnMatchesRefresh?.Invoke());
        }

        public override async UniTask Hide() {
            base.Hide(null);
        }

        public void SetScrollerDelegate(IEnhancedScrollerDelegate scrollerDelegate) {
            _scroller.Delegate = scrollerDelegate;
        }

        public void ApplyData() {
            _scroller.ReloadData();
        }
    }
}