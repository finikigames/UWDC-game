﻿using System;
using Core.MVP.Base.Interfaces;

namespace Main.UI.Views.Base {
    public interface IInviteWindow : IWindowView {
        void SubscribeToApply(Action callback);
        void SubscribeToDecline(Action callback);
        void ChangeName(string data);
    }
}