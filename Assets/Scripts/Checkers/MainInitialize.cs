using System;
using Checkers.UI.Data;
using Cysharp.Threading.Tasks;
using Global.StateMachine;
using Global.StateMachine.Base.Enums;
using UnityEngine.SceneManagement;
using Zenject;

namespace Checkers {
    public class MainInitialize : IInitializable,
                                  IDisposable {
        private readonly SignalBus _signalBus;
        private readonly GameStateMachine _gameStateMachine;

        public MainInitialize(SignalBus signalBus,
            GameStateMachine gameStateMachine) {
            _signalBus = signalBus;
            _gameStateMachine = gameStateMachine;
        }
        
        public void Initialize() {
            _signalBus.Subscribe<ToMainSignal>(async _ => await LoadYourAsyncScene());
        }
        
        public async UniTask LoadYourAsyncScene() {
            var currentScene = SceneManager.GetActiveScene();

            await SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);

            await _gameStateMachine.Fire(Trigger.MainTrigger);

            await SceneManager.UnloadSceneAsync(currentScene);
        }
        
        public void Dispose() {
            _signalBus.TryUnsubscribe<ToMainSignal>(async _ => await LoadYourAsyncScene());
        }
    }
}