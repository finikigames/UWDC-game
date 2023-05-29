using System;
using Checkers.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views.Implementations {
    public class WinWindow : BaseWindow, 
                             IWinWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _continueButton;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }
        
        public void SubscribeToContinue(Action callback) {
            _continueButton.onClick.AddListener(() => callback?.Invoke());
        }
    }
}