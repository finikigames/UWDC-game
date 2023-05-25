using System;
using UnityEngine;

namespace Core
{
    public abstract class UnitySingleton<T> : UnityComponent where T : Component
    {
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateSingleton);
        public static T Instance => LazyInstance.Value;

        private static T CreateSingleton()
        {
            var ownerObject = new GameObject($"{typeof(T).Name} (singleton)");
            var instance = ownerObject.AddComponent<T>();
            DontDestroyOnLoad(ownerObject);
            return instance;
        }
    }
}