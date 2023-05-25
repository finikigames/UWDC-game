using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core.AssetManager {
    public interface IAssetService {
        UniTask Initialize();
        UniTask<T> Load<T>(AssetReference assetReference) where T : class;
        bool IsLoading<T>(AssetReference assetReference) where T : class;
        UniTask<T> Load<T>(string address) where T : class;
        //UniTask<List<T>> LoadByLabel<T>(IEnumerable<string> keys) where T : class;
        UniTask<List<T>> Load<T>(List<AssetReference> assetReferences) where T : class;
        UniTask<SceneInstance> LoadScene(AssetReference sceneReference, LoadSceneMode sceneMode);
        void Release(AssetReference assetReference);
        void Release(List<AssetReference> assetReferences);
        void ReleaseAll();
        UniTask<long> GetDownloadSize(string address);
        UniTask<long> GetDownloadSize(AssetReference assetReference);
        UniTask<long> GetDownloadSize(IEnumerable<AssetReference> assetReferences);
        bool TryGetScene(AssetReference reference, out SceneInstance scene);
        bool TryGetAsset<T>(AssetReference reference, out T asset) where T : class;
        bool ResourceExists(object key);
        List<string> GetAllKeys();
        void RemoveHandle(AssetReference asset);
    }
}