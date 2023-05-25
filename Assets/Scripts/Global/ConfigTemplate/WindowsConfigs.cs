using System;
using System.Collections.Generic;
using System.Linq;
using Global.Window.Enums;
using UnityEngine;
using Zenject;

namespace Global.ConfigTemplate {
    [Serializable]
    public class WindowsConfigs : IInitializable {
        [SerializeField] private List<WindowConfig> _windowsConfig;

        public Dictionary<WindowKey, WindowConfig> WindowsConfig;

        public void Initialize() {
            WindowsConfig = _windowsConfig.ToDictionary(m => m.Key, m => m);

            foreach (var window in _windowsConfig) {
                if (String.IsNullOrEmpty(window.Uid)) {
                    window.Uid = Guid.NewGuid().ToString();   
                }
            }
        }
    }
}
