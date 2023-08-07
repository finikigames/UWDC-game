using System;
using Core.MVP.Base.Interfaces;
using UnityEngine;

namespace Checkers.UI.Views.Base {
    public interface IMatchWindow : IWindowView {
        void ProvideCamera(UnityEngine.Camera camera);
        void SubscribeToHowToPlayButton(Action callback);
        void SubscribeToFleeButton(Action callback);
        void SetYourName(string yourName);
        void SetOpponentName(string opponentName);
        void GetLostCheсker(bool isPlayer);
        void SetTimerTime(float time);
        void ResetBars(bool isWhite);
        Vector3 GetSendPawnPosition(bool isPlayer);
    }
}