﻿using Global.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using UnityEngine;

namespace Global.UI.Views.Implementations {
    public class RulesWindow : BaseWindow, 
                               IRulesWindow {
        [SerializeField] private CanvasGroup _group;

        protected override void OnEnable() {
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void ShowCloseButton() {
            _closeButton.gameObject.SetActive(true);
        }

        public void HideCloseButton() {
            _closeButton.gameObject.SetActive(false);
        }
    }
}