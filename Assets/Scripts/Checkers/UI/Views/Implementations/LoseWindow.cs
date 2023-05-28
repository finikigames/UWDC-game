using Checkers.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using UnityEngine;

namespace Checkers.UI.Views.Implementations {
    public class LoseWindow : BaseWindow, 
                              ILoseWindow {
        [SerializeField] private CanvasGroup _group;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }
    }
}