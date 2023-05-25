using System;
using Core.MVP.ShowStates.Interfaces;
using UnityEngine;

namespace Global.VisibilityMechanisms {
    public class SetActiveHideMechanism : IHideMechanism {
        public void Hide(GameObject controlObject, Action _) {
            controlObject.SetActive(false);
        }

        public void HideImmediate(GameObject controlObject) {
            controlObject.SetActive(false);
        }
    }
}