using System;
using Core.MVP.Base.Interfaces;

namespace Checkers.UI.Views.Base {
    public interface IMatchWindow : IWindowView
    {
        void SubscribeToHowToPlayButton(Action callback);
    }
}