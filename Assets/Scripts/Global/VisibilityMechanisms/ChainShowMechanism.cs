using System;
using System.Collections.Generic;
using Core.MVP.ShowStates.Interfaces;
using UnityEngine;

namespace Global.VisibilityMechanisms {
    public class ChainShowMechanism : IShowMechanism {
        private readonly List<IShowMechanism> _showMechanisms;

        public ChainShowMechanism(params IShowMechanism[] mechanisms) {
            _showMechanisms = new List<IShowMechanism>(mechanisms);
        }

        public void Show(GameObject controlObject, Action onShow = null) {
            foreach (var mechanism in _showMechanisms) mechanism.Show(controlObject, onShow);
        }

        public void ShowImmediate(GameObject controlObject) {
            foreach (var mechanism in _showMechanisms) mechanism.ShowImmediate(controlObject);
        }
    }
}