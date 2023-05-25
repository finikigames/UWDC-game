using System;
using UnityEngine;

namespace Core.MVP.ShowStates.Interfaces {
    public interface IShowMechanism {
        void Show(GameObject controlObject, Action onShow = null);
        void ShowImmediate(GameObject controlObject);
    }
}