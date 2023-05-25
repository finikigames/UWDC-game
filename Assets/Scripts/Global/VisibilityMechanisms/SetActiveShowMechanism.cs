using System;
using Core.MVP.ShowStates.Interfaces;
using UnityEngine;

namespace Global.VisibilityMechanisms {
    public class SetActiveShowMechanism : IShowMechanism {
        public void Show(GameObject controlObject, Action _) {
            controlObject.SetActive(true);
        }

        public void ShowImmediate(GameObject controlObject) {
            controlObject.SetActive(true);
        }
    }
}