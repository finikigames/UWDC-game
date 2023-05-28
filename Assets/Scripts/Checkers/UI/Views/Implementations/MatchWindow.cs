using System;
using Checkers.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views.Implementations {
    public class MatchWindow : BaseWindow, 
                               IMatchWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _howToPlayButton;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void SubscribeToHowToPlayButton(Action callback) {
            _howToPlayButton.onClick.RemoveAllListeners();
            _howToPlayButton.onClick.AddListener(() => callback?.Invoke());
        }
    }
}