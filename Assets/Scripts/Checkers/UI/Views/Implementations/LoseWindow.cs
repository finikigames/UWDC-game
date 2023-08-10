using System;
using Checkers.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views.Implementations {
    public class LoseWindow : BaseWindow, 
                              ILoseWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _reasonText;

        protected override void OnEnable() {
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void SubscribeToContinue(Action callback) {
            _continueButton.onClick.RemoveAllListeners();
            _continueButton.onClick.AddListener(() => callback?.Invoke());
        }
        
        public void SetReasonText(string text) {
            _reasonText.text = text;
        }
    }
}