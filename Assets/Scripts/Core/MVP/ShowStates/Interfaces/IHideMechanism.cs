using System;
using UnityEngine;

namespace Core.MVP.ShowStates.Interfaces {
    public interface IHideMechanism {
        void Hide(GameObject controlObject, Action onClose = null);
        void HideImmediate(GameObject controlObject);
    }
}