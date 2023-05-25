#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Core.Pools.Base;
using UnityEditor;

namespace Core.Utils {
    public static class RichPoolRegistry<T> {
        private static readonly Dictionary<T, IPool> _pools = new();

        static RichPoolRegistry() {
            EditorApplication.playModeStateChanged += change => _pools.Clear();
        }

        public static IEnumerable<KeyValuePair<T, IPool>> Pools => _pools;
        public static event Action<T, IPool> PoolAdded = delegate { };

        public static event Action<T, IPool> PoolRemoved = delegate { };

        public static void Add(T type, IPool memoryPool) {
            //TODO Temporary
            if (_pools.ContainsKey(type)) _pools.Remove(type);
            _pools.Add(type, memoryPool);
            PoolAdded(type, memoryPool);
        }

        public static void Remove(T type) {
            var pool = _pools[type];
            _pools.Remove(type);
            PoolRemoved(type, pool);
        }
    }
}
#endif