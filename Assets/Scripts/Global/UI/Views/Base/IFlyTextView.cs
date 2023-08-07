using Core.MVP.Base.Interfaces;
using Global.Enums;

namespace Global.UI.Views.Base {
    public interface IFlyTextView : IWindowView {
        void InitializeView();
        void ShowFlyText(string text);
        void SetBackgroundColor(PawnColor color);
    }
}