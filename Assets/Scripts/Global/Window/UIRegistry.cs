using System;
using System.Collections.Generic;
using Core.Extensions;
using Global.Window.Enums;
using UnityEngine;
using Zenject;

namespace Global.Window {
    [Serializable]
    public class UIContainerToGameObjectElement {
        public UIContainerType Key;
        public GameObject Holder;
    }

    public class UIRegistry : MonoBehaviour {
        [SerializeField]
        private List<UIContainerToGameObjectElement> _containers = new();

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        [Inject]
        private void Construct(WindowService windowService) {
            windowService.RegisterContainers(_containers);
        }
    }
}
