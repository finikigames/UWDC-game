using System;
using Core.MVP.Base.Interfaces;

namespace Checkers.UI.Views.Base {
    public interface IFleeWindow : IWindowView {
        void SubscribeToReturnButton(Action callback);
        void SubscribeToFleeButton(Action callback);
    }
}