using EnhancedUI.EnhancedScroller;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using Main.UI.Views.Base;
using UnityEngine;

namespace Main.UI.Views.Implementations {
    public class StartWindow : BaseWindow, 
                               IStartWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private EnhancedScroller _scroller;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void SetScrollerDelegate(IEnhancedScrollerDelegate deleg) {
            _scroller.Delegate = deleg;
        }

        public void ReloadData() {
            _scroller.ReloadData();
        }
    }
}