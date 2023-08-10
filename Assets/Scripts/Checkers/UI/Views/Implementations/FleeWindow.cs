using System;
using Checkers.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views.Implementations {
    public class FleeWindow : BaseWindow,
                              IFleeWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _fleeButton;
        [SerializeField] private Button _returnButton;
        
        protected override void OnEnable() {
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void SubscribeToReturnButton(Action callback) {
            _returnButton.onClick.RemoveAllListeners();
            _returnButton.onClick.AddListener(() => callback?.Invoke());
            
        }

        public void SubscribeToFleeButton(Action callback) {
            _fleeButton.onClick.RemoveAllListeners();
            _fleeButton.onClick.AddListener(() => callback?.Invoke());
        }
    }
}