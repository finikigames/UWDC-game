using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core.AssetManager {
    public interface IAssetService {
        Task Initialize();
        Task<T> Load<T>(AssetReference assetReference) where T : class;
        bool IsLoading<T>(AssetReference assetReference) where T : class;
        Task<T> Load<T>(string address) where T : class;
        //Task<List<T>> LoadByLabel<T>(IEnumerable<string> keys) where T : class;
        Task<List<T>> Load<T>(List<AssetReference> assetReferences) where T : class;
        Task<SceneInstance> LoadScene(AssetReference sceneReference, LoadSceneMode sceneMode);
        void Release(AssetReference assetReference);
        void Release(List<AssetReference> assetReferences);
        void ReleaseAll();
        Task<long> GetDownloadSize(string address);
        Task<long> GetDownloadSize(AssetReference assetReference);
        Task<long> GetDownloadSize(IEnumerable<AssetReference> assetReferences);
        bool TryGetScene(AssetReference reference, out SceneInstance scene);
        bool TryGetAsset<T>(AssetReference reference, out T asset) where T : class;
        bool ResourceExists(object key);
        List<string> GetAllKeys();
        void RemoveHandle(AssetReference asset);
    }
}