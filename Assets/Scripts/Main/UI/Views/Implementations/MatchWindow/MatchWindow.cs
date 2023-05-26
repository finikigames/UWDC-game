using Global.VisibilityMechanisms;
using Global.Window.Base;
using Main.UI.Views.Base.MatchWindow;
using UnityEngine;

namespace Main.UI.Views.Implementations.MatchWindow {
    public class MatchWindow : BaseWindow, 
                               IMatchWindow {
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