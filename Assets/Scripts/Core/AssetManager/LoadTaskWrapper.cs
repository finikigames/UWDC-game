using Core.AssetManager.Interfaces;
using Cysharp.Threading.Tasks;

namespace Core.AssetManager {
    public class LoadTaskWrapper<TLoad> : ILoadTaskWrapper {
        public UniTask<TLoad> Task;
    }
}