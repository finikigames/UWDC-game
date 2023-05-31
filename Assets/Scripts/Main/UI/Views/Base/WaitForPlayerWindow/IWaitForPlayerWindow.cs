using System;
using Core.MVP.Base.Interfaces;

namespace Main.UI.Views.Base.WaitForPlayerWindow {
    public interface IWaitForPlayerWindow : IWindowView {
        void ShowReturnButton();
        void HideReturnButton();
        void SubscribeToReturnButton(Action callback);
        void SetYourName(string text);
        void SetOpponentName(string text);
        void SetYourWins(string text);
        void SetOpponentWins(string text);
        void SetTimerText(string text);
    }
}