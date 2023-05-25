using System;

namespace Core.MVP.ShowStates.Interfaces {
    public interface IShow {
        void Show(Action onShow = null);
    }
}