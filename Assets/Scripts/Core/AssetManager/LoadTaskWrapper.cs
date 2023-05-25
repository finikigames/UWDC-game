using System.Threading.Tasks;
using Core.AssetManager.Interfaces;

namespace Core.AssetManager {
    public class LoadTaskWrapper<TLoad> : ILoadTaskWrapper {
        public Task<TLoad> Task;
    }
}