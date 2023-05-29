﻿using System;
using Checkers.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views.Implementations {
    public class MatchWindow : BaseWindow, 
                               IMatchWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _howToPlayButton;
        [SerializeField] private Button _fleeButton;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TextMeshProUGUI _opponentName;
        [SerializeField] private TextMeshProUGUI _yourName;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void ProvideCamera(UnityEngine.Camera camera) {
            _canvas.worldCamera = camera;
        }

        public void SubscribeToHowToPlayButton(Action callback) {
            _howToPlayButton.onClick.RemoveAllListeners();
            _howToPlayButton.onClick.AddListener(() => callback?.Invoke());
        }

        public void SubscribeToFleeButton(Action callback) {
            _fleeButton.onClick.RemoveAllListeners();
            _fleeButton.onClick.AddListener(() => callback?.Invoke());
        }

        public void SetYourName(string yourName) {
            _yourName.text = yourName;
        }

        public void SetOpponentName(string opponentName) {
            _opponentName.text = opponentName;
        }
    }
}