using Cysharp.Threading.Tasks;

namespace Core.MVP.Base.Interfaces {
    public interface IBasePresenter<TKey> where TKey : System.Enum {
        UniTask Initialize(IWindowData data, TKey key, bool isInit = false);
        UniTask Open();
        UniTask Close();
        void PreloadInitialize();
        void InitDependencies();
        void InitializeView(IView view);
        void Dispose();
        UniTask InitializeOnce();
    }
}
