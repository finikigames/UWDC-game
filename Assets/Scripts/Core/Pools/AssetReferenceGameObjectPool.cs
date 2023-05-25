using System;
using System.Collections.Generic;
using Core.AssetManager;
using Core.Pools.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Core.Pools {
    public class AssetReferenceGameObjectPool : IPool {
        public int NumTotal => _realSize;
        public int NumActive => _taken.Count;
        public int NumInactive => NumTotal - NumActive;

        private Transform _parent;
        private Action<GameObject> _resetProcess;
        private Action<GameObject> _instantiateProcess;
        
        private int _realSize;
        private readonly IAssetService _assetService;
        private readonly int _initialSize;

        private readonly PoolExpander _expander;
        private readonly Queue<GameObject> _objects;
        private readonly List<GameObject> _taken;

        private readonly AssetReference _assetReferencePrefab;
        private GameObject _prefab;

        public AssetReferenceGameObjectPool(IAssetService assetService,
                                            AssetReference assetReferencePrefab, 
                                            ExpandType expandType, 
                                            ExpandType expandPercent, 
                                            int size = 10) {
            _assetService = assetService;
            _initialSize = size;
            _assetReferencePrefab = assetReferencePrefab;

            _expander = new PoolExpander(expandType, expandPercent, this);
            _objects = new Queue<GameObject>();
            _taken = new List<GameObject>();
        }

        public async UniTask LoadPrefab() {
            _prefab = await _assetService.Load<GameObject>(_assetReferencePrefab);
        }

        public IPool SetParentContainer(Transform parent) {
            _parent = parent;
            return this;
        }

        public IPool SetInstantiateProcess(Action<GameObject> process) {
            _instantiateProcess = process;
            return this;
        }

        public IPool SetResetProcess(Action<GameObject> process) {
            _resetProcess = process;
            return this;
        }
        
        public void InitialFill() {
            Instantiate(_initialSize);
        }

        public T Take<T>(bool worldPositionStays = true) where T : MonoBehaviour {
            var go = Take();
            var component = go.GetComponent<T>();
            
            if (component is IPoolReset poolReset) {
                poolReset.Reset();
            }
            
            go.transform.SetParent(_parent, worldPositionStays);
            return component;
        }

        public T GetFirst<T>() where T : MonoBehaviour {
            _expander.CheckExpand();
            
            var go = _objects.Peek();
            _taken.Add(go);
            go.transform.SetParent(_parent);
            return go.GetComponent<T>();
        }

        public void Release(MonoBehaviour obj) {
            var gameObject = obj.gameObject;
            _resetProcess?.Invoke(gameObject);
            gameObject.SetActive(false);
            if (obj is IPoolReset poolReset) {
                poolReset.Reset();
            }
            _objects.Enqueue(gameObject);
            _taken.Remove(gameObject);
        }

        public void Release(GameObject gameObject) {
            gameObject.SetActive(false);
            _objects.Enqueue(gameObject);
            _taken.Remove(gameObject);
        }

        public void Dispose() {
            for (int i = 0; i < _objects.Count; i++) {
                var obj = _objects.Dequeue();
                
                Object.Destroy(obj);
            }
            
            _assetService.Release(_assetReferencePrefab);
            
            _taken.Clear();
            _objects.Clear();
        }

        public void Instantiate(int count) {
            for (var i = 0; i < count; i++) {
                var go = Object.Instantiate(_prefab, _parent);
                _instantiateProcess?.Invoke(go);
                go.SetActive(false);
                _objects.Enqueue(go);
            }

            _realSize += count;
        }

        private GameObject Take() {
            _expander.CheckExpand();
            
            var go = _objects.Dequeue();
            _taken.Add(go);
            go.SetActive(true);
            return go;
        }
    }
}