using System;
using Core.MVP.ShowStates.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace Global.VisibilityMechanisms {
    public class ScaleHideMechanism : IHideMechanism {
        public void Hide(GameObject controlObject, Action onClose = null) {
            var transform = controlObject.transform;

            DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, 0.3f))
                .AppendCallback(() => { onClose?.Invoke(); });
        }

        public void HideImmediate(GameObject controlObject) {
            var transform = controlObject.transform;

            transform.localScale = Vector3.zero;
        }
    }
}