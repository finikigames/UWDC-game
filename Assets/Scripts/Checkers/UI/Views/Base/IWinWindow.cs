﻿using System;
using Core.MVP.Base.Interfaces;

namespace Checkers.UI.Views.Base {
    public interface IWinWindow : IWindowView {
        void SubscribeToContinue(Action callback);
        void SetReasonText(string text);
    }
}