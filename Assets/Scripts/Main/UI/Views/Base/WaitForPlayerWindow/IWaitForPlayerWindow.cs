using Core.MVP.Base.Interfaces;

namespace Main.UI.Views.Base.WaitForPlayerWindow {
    public interface IWaitForPlayerWindow : IWindowView {
        void SetYourName(string text);
        void SetOpponentName(string text);
        void SetYourWins(string text);
        void SetOpponentWins(string text);
    }
}