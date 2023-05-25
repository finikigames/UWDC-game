using System;
using Core.MVP.ShowStates.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace Global.VisibilityMechanisms {
    public class FadeHideMechanism : IHideMechanism {
        private readonly CanvasGroup _group;

        public FadeHideMechanism(CanvasGroup group) {
            _group = group;
        }
        
        public void Hide(GameObject controlObject, Action onShow = null) {
            _group.DOFade(0, 0.5f);
            _group.blocksRaycasts = false;
            _group.interactable = false;
        }

        public void HideImmediate(GameObject controlObject) {
            _group.alpha = 0;
            _group.blocksRaycasts = false;
            _group.interactable = false;
        }
    }
}