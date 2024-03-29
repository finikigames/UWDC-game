﻿using System;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using Main.UI.Views.Base.WaitForPlayerWindow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI.Views.Implementations.WaitForPlayerWindow {
    public class WaitForPlayerWindow : BaseWindow,
                                       IWaitForPlayerWindow {
        [SerializeField] private CanvasGroup _group;

        [SerializeField] private TextMeshProUGUI _yourDisplayName;

        [SerializeField] private TextMeshProUGUI _opponentDisplayName;

        [SerializeField] private TextMeshProUGUI _yourWins;

        [SerializeField] private TextMeshProUGUI _opponentWins;

        [SerializeField] private Button _returnButton;

        [SerializeField] private TextMeshProUGUI _timerText;

        [SerializeField] private TextMeshProUGUI _returnText;
        [SerializeField] private TextMeshProUGUI _opponentExistText;
        
        protected override void OnEnable() {
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void ShowReturnButton() {
            _returnButton.enabled = true;
            _returnText.gameObject.SetActive(true);
        }

        public void HideReturnButton() {
            _returnButton.enabled = false;
            _returnText.gameObject.SetActive(false);
        }

        public void SubscribeToReturnButton(Action callback) {
            _returnButton.onClick.RemoveAllListeners();
            _returnButton.onClick.AddListener(() => callback?.Invoke());
        }

        public void SetYourName(string text) {
            _yourDisplayName.text = text;
        }

        public void SetOpponentName(string text) {
            _opponentDisplayName.text = text;
        }

        public void SetYourWins(string text) {
            _yourWins.text = text;
        }

        public void SetOpponentWins(string text) {
            _opponentWins.text = text;
        }

        public void SetTimerText(string text) {
            _timerText.text = text;
        }

        public void SetOpponentExistState(bool state) {
            var text = state ? "Вы уже выиграли у этого пользователя, за победу над ним вы не получите очки" : "";

            _opponentExistText.text = text;
        }
    }
}