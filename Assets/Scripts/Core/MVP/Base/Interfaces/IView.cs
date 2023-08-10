using System;
using Cysharp.Threading.Tasks;

namespace Core.MVP.Base.Interfaces {
    public interface IView {
        UniTask ShowView(Action onShow = null);
        UniTask HideView(Action onHide = null);
        void HideImmediate();
        void Dispose();
    }
}