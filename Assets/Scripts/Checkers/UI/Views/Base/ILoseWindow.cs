using System;
using Core.MVP.Base.Interfaces;

namespace Checkers.UI.Views.Base {
    public interface ILoseWindow : IWindowView {
        void SubscribeToContinue(Action callback);
    }
}