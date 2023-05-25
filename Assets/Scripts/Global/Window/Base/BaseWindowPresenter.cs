﻿using System.Threading.Tasks;
using Core.MVP.Base.Interfaces;
using Global.Services.Context;
using Global.Window.Enums;
using Global.Window.Signals;

namespace Global.Window.Base {
    public class BaseWindowPresenter<TView, TData> : BasePresenterNEW<TView, TData>
                                                     where TView : IWindowView
                                                     where TData : IWindowData {
        protected BaseWindowPresenter(ContextService service) : base(service) {
        }

        public override async Task Initialize(IWindowData data, WindowKey key, bool isInit) {
            WindowData = (TData) data;

            await InitializeData();
            View.OnClickCloseButton = null;
            View.OnClickCloseButton += () => SignalBus.Fire(new CloseWindowSignal(key));
        }
    }
}