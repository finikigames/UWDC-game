using System.Threading.Tasks;

namespace Core.MVP.Base.Interfaces {
    public interface IBasePresenter<TKey> where TKey : System.Enum {
        Task Initialize(IWindowData data, TKey key, bool isInit = false);
        Task Open();
        Task Close();
        void PreloadInitialize();
        void InitDependencies();
        void InitializeView(IView view);
        void Dispose();
        Task InitializeOnce();
    }
}
