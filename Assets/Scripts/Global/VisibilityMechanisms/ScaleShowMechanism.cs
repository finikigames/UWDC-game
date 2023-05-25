using System;
using Core.MVP.ShowStates.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace Global.VisibilityMechanisms {
    public class ScaleShowMechanism : IShowMechanism {
        public void Show(GameObject controlObject, Action onShow = null) {
            var transform = controlObject.transform;

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.3f);
        }

        public void ShowImmediate(GameObject controlObject) {
            var transform = controlObject.transform;

            transform.localScale = Vector3.zero;
        }
    }
}