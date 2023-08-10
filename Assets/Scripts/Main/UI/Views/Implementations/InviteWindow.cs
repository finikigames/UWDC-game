using System;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using Main.UI.Views.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI.Views.Implementations {
    public class InviteWindow : BaseWindow,
                                IInviteWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _declineButton;
        [SerializeField] private TextMeshProUGUI _mainText;

        protected override void OnEnable() {
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void SubscribeToDecline(Action callback) {
            _declineButton.onClick.RemoveAllListeners();
            _declineButton.onClick.AddListener(() => callback?.Invoke());
        }
        
        public void SubscribeToApply(Action callback) {
            _applyButton.onClick.RemoveAllListeners();
            _applyButton.onClick.AddListener(() => callback?.Invoke());
        }

        public void ChangeName(string data) {
            _mainText.text =
                $"Игрок {data} приглашает вас на дружеский матч, результат которого не будет записан в вашу статистику.";
        }
    }
}