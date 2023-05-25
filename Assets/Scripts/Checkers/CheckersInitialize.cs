using System;
using System.Threading.Tasks;
using Checkers.UI.Data;
using Core.Extensions;
using UnityEngine.SceneManagement;
using Zenject;

namespace Checkers {
    public class CheckersInitialize : IInitializable,
                                      IDisposable {
        private readonly SignalBus _signalBus;

        public CheckersInitialize(SignalBus signalBus) {
            _signalBus = signalBus;
        }
        
        public void Initialize() {
            _signalBus.Subscribe<ToCheckersMetaSignal>(async signal => await LoadYourAsyncScene(signal));
        }

        public async Task LoadYourAsyncScene(ToCheckersMetaSignal signal) {
            var currentScene = SceneManager.GetActiveScene();

            PlayerPrefsX.SetBool("WithPlayer", signal.WithPlayer);
            await UnityExtensions.LoadSceneAsync("CheckersMain_online", LoadSceneMode.Additive);
        }
        
        public void Dispose() {
            _signalBus.TryUnsubscribe<ToCheckersMetaSignal>(async signal => await LoadYourAsyncScene(signal));
        }
    }
}