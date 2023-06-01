using Core.MVP.Base.Interfaces;

namespace Global.UI.Views.Base {
    public interface IRulesWindow : IWindowView {
        void ShowCloseButton();
        void HideCloseButton();
    }
}