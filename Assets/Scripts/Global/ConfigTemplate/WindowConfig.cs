using System;
using Global.StateMachine.Base.Enums;
using Global.Window.Enums;
using UnityEngine.AddressableAssets;
using TypeReferences;
using UnityEngine;

namespace Global.ConfigTemplate {
    [Serializable]
    public class WindowConfig {
        public string Uid;
        public AssetReference PrefabReference;
        public WindowKey Key;
        public TypeReference Controller => _controller;
        public int Priority;
        public bool IsFullScreen;
        public bool CanClose;
        public GameContext Context;
        
        [TypeOptions(ShowAllTypes = true)]
        [SerializeField] private TypeReference _controller;
    }
}