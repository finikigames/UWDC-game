using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Core.AssetManager.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core.AssetManager {
    public class AddressableAssetService : IAssetService {
        private readonly Dictionary<string, AsyncOperationHandle> _assets;
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _scenes;
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles;
        private readonly Dictionary<(Type, AssetReference), ILoadTaskWrapper> _loadCache;

        public AddressableAssetService() {
            _assets = new Dictionary<string, AsyncOperationHandle>();
            _scenes = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();
            _handles = new Dictionary<string, List<AsyncOperationHandle>>();
            _loadCache = new Dictionary<(Type, AssetReference), ILoadTaskWrapper>();
        }

        public Task Initialize() {
            var source = new TaskCompletionSource<bool>();
            Addressables.InitializeAsync().Completed += _ => source.TrySetResult(true);
            return source.Task;
        }

        #region Load

        public bool IsLoading<T>(AssetReference assetReference) where T : class {
            var cacheKey = GetCacheKey<T>(assetReference);

            return _loadCache.ContainsKey(cacheKey);
        }

        public (Type, AssetReference) GetCacheKey<T>(AssetReference assetReference) {
            return (typeof(T), assetReference);
        }

        public async Task<T> Load<T>(AssetReference assetReference) where T : class {
            if (!GuardKey(assetReference, out string key)) {
                throw new InvalidKeyException(key);
            }
            
            if (ContainsAsset(key)) {
                return _assets[key].Result as T;
            }

            var cacheKey = GetCacheKey<T>(assetReference);
            if (_loadCache.ContainsKey(cacheKey)) {
                var task = (LoadTaskWrapper<T>) _loadCache[cacheKey];
                var otherResult = await task.Task;
                return otherResult;
            }
            
            var assetHandle = assetReference.LoadAssetAsync<T>();

            var loadTask = new LoadTaskWrapper<T> {
                Task = assetHandle.Task
            };
            _loadCache.Add(cacheKey, loadTask);

            return await LoadWithCache(
                key,
                assetHandle,
                completed => {
                    _assets[key] = completed;
                    _loadCache.Remove(cacheKey);
                });
        }
        public async Task<T> Load<T>(string address) where T : class {
            if (!GuardKey(address, out string key)) {
                throw new InvalidKeyException(key);
            }

            return await LoadWithCache(
                key,
                Addressables.LoadAssetAsync<T>(address),
                completed => { _assets[key] = completed; });
        }
        public async Task<List<T>> Load<T>(List<AssetReference> assetReferences) where T : class {
            var loadOps = new List<AsyncOperationHandle>();
            foreach (AssetReference asset in assetReferences) {
                if (!GuardKey(asset, out string key)) {
                    throw new InvalidKeyException(key);
                }

                if (ContainsAsset(key)) continue;

                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(asset);
                AddHandle(key, handle, completed => { _assets[key] = completed; });

                loadOps.Add(handle);
            }

            AsyncOperationHandle<IList<AsyncOperationHandle>> group
                = Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true);
            await group.Task;

            var instances = loadOps.Select(x => (T) x.Result).ToList();
            return instances;
        }

        /*
        public async Task<List<T>> LoadByLabel<T>(IEnumerable<string> keys) where T : class {
            IList<IResourceLocation> locations
                = await Addressables.LoadResourceLocationsAsync(keys,
                    Addressables.MergeMode.Union, typeof(T));

            var loadOps = new List<AsyncOperationHandle>(locations.Count);

            foreach (IResourceLocation location in locations) {
                string key = location.PrimaryKey;
                if (ContainsAsset(key)) continue;

                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(location);
                AddHandle(key, handle, completed => { _assets[key] = completed; });

                loadOps.Add(handle);
            }

            AsyncOperationHandle<IList<AsyncOperationHandle>> group
                = Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true);
            await group.Task;

            var instances = loadOps.Select(x => (T) x.Result).ToList();
            return instances;
        }*/

        public async Task<SceneInstance> LoadScene(AssetReference sceneReference, LoadSceneMode sceneMode) {
            if (!GuardKey(sceneReference, out string key)) {
                throw new InvalidKeyException(key);
            }

            if (_scenes.TryGetValue(key, out AsyncOperationHandle<SceneInstance> cached)) {
                return cached.Result;
            }

            AsyncOperationHandle<SceneInstance> handle
                = Addressables.LoadSceneAsync(sceneReference, sceneMode, false);

            AddHandle(key, handle, completed => { _scenes[key] = completed; });
            return await handle.Task;
        }

        private async Task<T> LoadWithCache<T>(string key,
            AsyncOperationHandle<T> handle,
            Action<AsyncOperationHandle<T>> callback) where T : class {
            
            if (_assets.TryGetValue(key, out AsyncOperationHandle cached)) {
                return cached.Result as T;
            }

            AddHandle(key, handle, callback);
            return await handle.Task;
        }

        #endregion

        #region Release

        public void Release(AssetReference assetReference) {
            Debug.Log($"Try release: {assetReference.RuntimeKey}");

            if (!GuardKey(assetReference, out string key)) {
                throw new InvalidKeyException(key);
            }

            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> cached)) return;
            
            foreach (AsyncOperationHandle handle in cached) {
                Debug.Log($"Release: {handle.DebugName}, {key}");
                Addressables.Release(handle);
            }

            Debug.Log($"scenes: {_scenes.Count}, assets: {_assets.Count}, handles: {_handles.Count}");

            _scenes.Remove(key);
            _assets.Remove(key);
            _handles.Remove(key);
        }
        public void Release(List<AssetReference> assetReferences) {
            assetReferences.ForEach(Release);
        }

        public void ReleaseAll() {
            foreach (List<AsyncOperationHandle> handles in _handles.Values) {
                foreach (AsyncOperationHandle handle in handles) {
                    Addressables.Release(handle);
                    Debug.Log($"Release: {handle.DebugName}");
                }
            }

            _assets.Clear();
            _handles.Clear();
            _scenes.Clear();
        }

        #endregion

        #region Get download size

        public async Task<long> GetDownloadSize(string address)
            => await GetDownloadSize(Addressables.GetDownloadSizeAsync(address));
        public async Task<long> GetDownloadSize(AssetReference assetReference)
            => await GetDownloadSize(Addressables.GetDownloadSizeAsync(assetReference));
        public async Task<long> GetDownloadSize(IEnumerable<AssetReference> assetReferences)
            => await GetDownloadSize(Addressables.GetDownloadSizeAsync(assetReferences));

        private static async Task<long> GetDownloadSize(AsyncOperationHandle<long> handle) {
            long size = await handle.Task;
            Addressables.Release(handle);
            return size;
        }

        #endregion

        public bool TryGetScene(AssetReference reference, out SceneInstance scene) {
            scene = default;

            if (!GuardKey(reference, out string key)) {
                return false;
            }

            if (_scenes.TryGetValue(key, out AsyncOperationHandle<SceneInstance> value)) {
                scene = value.Result;
                return true;
            }

            Debug.LogWarning($"No scene with key {key}");
            return false;
        }

        public bool TryGetAsset<T>(AssetReference reference, out T asset) where T : class {
            asset = default;

            if (!GuardKey(reference, out string key)) {
                return false;
            }

            if (_assets.TryGetValue(key, out AsyncOperationHandle value)) {
                asset = value.Result as T;
                return true;
            }

            Debug.LogWarning($"No asset with key {key}");
            return false;
        }

        public bool ResourceExists(object key) {
            foreach (IResourceLocator l in Addressables.ResourceLocators) {
                if (l.Locate(key, typeof(object), out _))
                    return true;
            }

            return false;
        }

        public List<string> GetAllKeys() {
            var keys = Addressables.ResourceLocators.OfType<ResourceLocationMap>()
                .SelectMany(locationMap =>
                    locationMap.Locations.Keys.Select(key => key.ToString())
                ).ToList();

            return keys;
        }

        public void RemoveHandle(AssetReference asset) {
            if (!GuardKey(asset, out string key)) {
                throw new InvalidKeyException(key);
            }

            _scenes.Remove(key);
            _assets.Remove(key);
            _handles.Remove(key);

            Debug.Log($"scenes: {_scenes.Count}, assets: {_assets.Count}, handles: {_handles.Count}");
        }

        private void AddHandle<T>(string key, 
            AsyncOperationHandle<T> handle, 
            Action<AsyncOperationHandle<T>> callback) {
            
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> handles)) {
                handles = new List<AsyncOperationHandle>();
                _handles[key] = handles;
            }

            handle.Completed += completed => {
                if (ErrorHandle(handle, out string error)) {
                    throw new WarningException(error);
                }

                callback(completed);

            };

            handles.Add(handle);
        }

        private static bool ErrorHandle(AsyncOperationHandle handle, out string result) {
            string dlError = GetDownloadError(handle);

            result = dlError ?? string.Empty;
            return !string.IsNullOrEmpty(result);
        }

        private static string GetDownloadError(AsyncOperationHandle fromHandle) {
            if (fromHandle.Status != AsyncOperationStatus.Failed)
                return null;

            Exception e = fromHandle.OperationException;
            while (e != null) {
                if (e is RemoteProviderException remoteException)
                    return remoteException.WebRequestResult.Error;
                e = e.InnerException;
            }
            return null;
        }

        private bool ContainsAsset(string key)
            => _assets.ContainsKey(key) && _assets.TryGetValue(key, out _);
        private bool ContainsScene(string key)
            => _scenes.ContainsKey(key) && _scenes.TryGetValue(key, out _);

        private static bool GuardKey(string key, out string result) {
            result = key ?? string.Empty;
            return !string.IsNullOrEmpty(result);
        }
        private static bool GuardKey(AssetReference reference, out string result) {
            if (reference == null) {
                throw new ArgumentNullException(nameof(reference));
            }

            result = reference.RuntimeKey.ToString();

            return !string.IsNullOrEmpty(result);
        }
    }
}