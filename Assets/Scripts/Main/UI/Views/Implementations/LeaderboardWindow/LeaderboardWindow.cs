using EnhancedUI.EnhancedScroller;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using Main.UI.Views.Base.LeaderboardWindow;
using UnityEngine;

namespace Main.UI.Views.Implementations.LeaderboardWindow {
    public class LeaderboardWindow : BaseWindow,
                                     ILeaderboardWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private EnhancedScroller _scroller;

        public int EndDataIndex => _scroller.EndDataIndex;
        public int StartDataIndex => _scroller.StartDataIndex;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;

            ChangeShowMechanism(new FadeShowMechanism(_group));
            ChangeHideMechanism(new FadeHideMechanism(_group));
        }

        public void SetScrollerDelegate(IEnhancedScrollerDelegate deleg) {
            _scroller.Delegate = deleg;
        }
        
        public void ReloadData() {
            _scroller.ReloadData();
        }
    }
}