using System;
using Core.MVP.Base.Interfaces;

namespace Checkers.UI.Views.Base {
    public interface IMatchWindow : IWindowView {
        void ProvideCamera(UnityEngine.Camera camera);
        void SubscribeToHowToPlayButton(Action callback);
        void SubscribeToFleeButton(Action callback);
        void SetYourName(string yourName);
        void SetOpponentName(string opponentName);
        void GetLostCheсker(bool isPlayer);
        void SetTimerTime(int time);
        void ResetBars();
    }
}