using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Core.MVP.Base.Interfaces;
using Global.ConfigTemplate;
using Global.Window.Enums;
using Global.Window.Interfaces;
using Global.Window.Signals;
using UnityEngine;
using Zenject;

namespace Global.Window {
    public class WindowService : IWindowService,
                                 IDisposable {
        private readonly SignalBus _signalBus;
        private readonly WindowsConfigs _config;
        private readonly WindowFactory _windowFactory;
        private Dictionary<UIContainerType, GameObject> _containers;
        private readonly Dictionary<WindowKey, UIWindow> _windowsByUid;
        private readonly Dictionary<WindowKey, UIWindow> _activeWindows;

        public WindowService(SignalBus signalBus,
                             WindowsConfigs config,
                             WindowFactory windowFactory) {
            _signalBus = signalBus;
            _config = config;
            _windowFactory = windowFactory;

            _windowsByUid = new();
            SubscribeSignals();
        }
        
        public void RegisterContainers(Dictionary<UIContainerType, GameObject> containers) {
            _containers = containers;
        }

        private async void OpenWindow(WindowKey key, IWindowData data) {
            var windowPair = await PreloadWindow(key);

            var window = windowPair.window;
            
            //TODO check state = opened
            await window.Initialize(data, windowPair.isInitialized);
            window.Open();
        }

        private async Task<(UIWindow window, bool isInitialized)> PreloadWindow(WindowKey key) {
            var config = GetWindowConfig(key);

            UIWindow window;
            bool isInitialized = false;
            if (_windowsByUid.ContainsKey(key)) {
                window = _windowsByUid[key];
                isInitialized = true;
            }
            else {
                Transform transform = GetUIContainer(UIContainerType.WindowsContainer);

                window = await _windowFactory.Create(config, transform, key, Vector3.zero);
                window.SubscribeDestroy(OnDestroyView);
                
                _windowsByUid.Add(key, window);   
            }

            window.InitializeDependencies();
            window.PreloadInitialize();
            window.CloseImmediate();
            
            if (!isInitialized) {
                await window.InitializeOnce();
            }
            
            return (window, isInitialized);
        }

        public bool IsWindowLoaded(WindowKey key) => _windowsByUid.ContainsKey(key);

        private async Task CloseWindow(WindowKey key) {
            if (!_windowsByUid.TryGetValue(key, out UIWindow window)) {
                throw new WarningException($"Window is not active, uid: {key}");
            }

            if (CanBeClosed(window)) {
                await window.Close();
            }
            else {
                throw new WarningException($"Can't close the window with uid: {key}");
            }
        }


        private void SubscribeSignals() {
            _signalBus.Subscribe<OpenWindowSignal>(signal => OpenWindow(signal.Key, signal.Data));
            _signalBus.Subscribe<CloseWindowSignal>(signal => CloseWindow(signal.Key));
            _signalBus.Subscribe<PreloadWindowSignal>(signal => PreloadWindow(signal.Key));
        }
        
        private WindowConfig GetWindowConfig(WindowKey key) {
            _config.WindowsConfig.TryGetValue(key, out WindowConfig windowData);
            if (windowData == null) {
                throw new WarningException("WindowConfig not found");
            }

            return windowData;
        }

        private Transform GetUIContainer(UIContainerType type) {
            _containers.TryGetValue(type, out GameObject container);

            //TODO default container or throw exception
            return container != null ? container.transform : new GameObject().transform;
        }
        
        private void OnDestroyView(string uid) {
        }
        
        private static bool CanBeClosed(UIWindow window) 
            => window?.State == WindowState.Opened;

        public void Dispose() {
            foreach (var windowPair in _windowsByUid) {
                CloseWindow(windowPair.Key);
            }
        }
    }
}
