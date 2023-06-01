using Core.MVP.Base.Interfaces;

namespace Checkers.UI.Views.Base {
    public interface IPauseWindow : IWindowView {
        void SetPauseTime(float time);
    }
}