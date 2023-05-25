using System.Collections.Generic;
using Global.Window.Enums;
using UnityEngine;
using Zenject;

namespace Global.Window {
    public class UIRegistry : MonoBehaviour {
        private Dictionary<UIContainerType, GameObject> _containers = new();

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        [Inject]
        private void Construct(WindowService windowService) {
            windowService.RegisterContainers(_containers);
        }
    }
}
