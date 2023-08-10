using System;
using Core.MVP.Base.Interfaces;
using Core.Observable;
using Cysharp.Threading.Tasks;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Enums;
using Zenject;

namespace Global.Window.Base {
    public abstract class BasePresenterNEW<TView, TData> : IBasePresenter<WindowKey>
                                                           where TView : IView
                                                           where TData : IWindowData {
        private Observable<WindowState> _state;
        protected TData WindowData;
        protected TView View;

        protected WindowState State => _state.Value;
        
        private ContextService _contextService;
        [Inject]
        protected SignalBus SignalBus;
        
        protected BasePresenterNEW(ContextService service) {
            _contextService = service;
        }
        
        protected T Resolve<T>(GameContext context) {
            var container = _contextService.ResolveContainer(context);
            return container.Resolve<T>();
        }

        public void ProvideState(Observable<WindowState> state) {
            _state = state;
        }

        public virtual void PreloadInitialize() {
        }

        public virtual async UniTask Initialize(IWindowData data, WindowKey key, bool isInit) {
            WindowData = (TData) data;

            await InitializeData();
        }

        public virtual async UniTask InitializeOnce() {
        }

        public void InitializeView(IView view) {
            View = (TView) view;
        }

        public async UniTask Open() {
            await LoadContent();
            await View.ShowView();
        }

        public void SubscribeToSignal<TSignal>(Action<TSignal> action) {
            SignalBus.Subscribe(action);
        }

        public async UniTask Close() {
            await View.HideView();
        }

        protected virtual async UniTask InitializeData() {}
        
        protected virtual async UniTask LoadContent() {}

        public virtual void InitDependencies() {
        }

        public virtual void Dispose() {
        }

        protected void FireSignal<TSignal>(TSignal signal) {
            SignalBus.Fire(signal);
        }
    }
}