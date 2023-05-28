using Global.VisibilityMechanisms;
using Global.Window.Base;
using Main.UI.Views.Base.WaitForPlayerWindow;
using UnityEngine;

namespace Main.UI.Views.Implementations.WaitForPlayerWindow {
    public class WaitForPlayerWindow : BaseWindow, 
                              IWaitForPlayerWindow {
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