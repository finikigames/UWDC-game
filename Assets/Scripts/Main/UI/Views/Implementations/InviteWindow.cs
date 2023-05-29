using System;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using Main.UI.Views.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI.Views.Implementations {
    public class InviteWindow : BaseWindow,
                                IInviteWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _applyButton;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }
        
        public void SubscribeToApply(Action callback) {
            _applyButton.onClick.AddListener(() => callback?.Invoke());
        }
    }
}