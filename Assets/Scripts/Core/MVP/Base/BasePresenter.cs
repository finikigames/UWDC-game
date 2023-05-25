using System;
using Core.MVP.Base.Interfaces;
using Core.MVP.Initializer.Interfaces;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Core.MVP.Base {
    public abstract class BasePresenter<T> : IInitializeUnit,
                                             IInitializable,
                                             IDisposableUnit where T : IView {
        [Inject]
        private IInitializeService _initializeService;

        [Inject] protected T View;

        [Inject] private SignalBus _signalBus;

        /// <summary>
        ///     Override this method if you don't want to let your presenter to
        ///     initialize automatically or just want to write some custom logic.
        /// </summary>
        /// <remarks>
        ///     You can't just use this method to perform initializing,
        ///     because you want to do initializing at certain moment of time.
        /// </remarks>
        public virtual void Initialize() {
            _initializeService.SubscribeToInitialize(this);
            _initializeService.SubscribeToDispose(this);
        }

        public void FireSignal<TSignal>(TSignal signal) {
            _signalBus.TryFire(signal);
        }

        public void SubscribeToSignal<TSignal>(Action<TSignal> signal) {
            _signalBus.Subscribe(signal);
        }

        public void UnsubscribeFromSignal<TSignal>(Action<TSignal> signal) {
            _signalBus.TryUnsubscribe(signal);
        }

        public void ProvideInitializeService(IInitializeService initializeService) {
            _initializeService = initializeService;
        }

        /// <summary>
        ///     There is performs actual initialization of
        ///     presenter e.g. View initializing, Database
        ///     operations, model subscribing to the view.
        /// </summary>
        public virtual void InitializeUnit() { }

        public virtual void DisposeUnit() { }
        
        public async UniTask Open() {
            await View.ShowView();
            // await View.Show();
        }

        public async UniTask Close()
        {
            // await View.Hide();
            // View.Cleanup();
        }
    }
}