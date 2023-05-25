using Cysharp.Threading.Tasks;

namespace Core.MVP.Base.Interfaces {
    public interface IView {
        UniTask ShowView();
        UniTask Hide();
        void HideImmediate();
        void Dispose();
    }
}