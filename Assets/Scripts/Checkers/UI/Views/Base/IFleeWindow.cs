using System;
using Core.MVP.Base.Interfaces;

namespace Checkers.UI.Views.Base {
    public interface IFleeWindow : IWindowView {
        void Hide(Action callback = null);
        void SubscribeToReturnButton(Action callback);
        void SubscribeToFleeButton(Action callback);
    }
}