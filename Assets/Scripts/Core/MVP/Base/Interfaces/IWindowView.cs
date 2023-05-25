using System;

namespace Core.MVP.Base.Interfaces {
    public interface IWindowView : IView {
        void ChangeHeader(string header);
        void Initialize(string uid);
        Action<string> OnDestroyView { get; set; }
        Action OnClickCloseButton { get; set; }
    }
}