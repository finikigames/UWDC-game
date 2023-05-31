using System;
using Core.Extensions;
using Cysharp.Threading.Tasks;
using Global.StateMachine;
using Global.StateMachine.Base.Enums;
using Global.Window.Enums;
using Global.Window.Signals;
using Main.UI.Data;
using UnityEngine.SceneManagement;
using Zenject;

namespace Main {
    public class CheckersInitialize : IInitializable,
                                      IDisposable {
        private readonly SignalBus _signalBus;
        private readonly GameStateMachine _gameStateMachine;

        public CheckersInitialize(SignalBus signalBus,
                                  GameStateMachine gameStateMachine) {
            _signalBus = signalBus;
            _gameStateMachine = gameStateMachine;
        }
        
        public void Initialize() {
            _signalBus.Subscribe<ToCheckersMetaSignal>(LoadSceneInternal);
        }

        private async void LoadSceneInternal(ToCheckersMetaSignal signal) {
            await LoadYourAsyncScene(signal);
        }

        public async UniTask LoadYourAsyncScene(ToCheckersMetaSignal signal) {
            var currentScene = SceneManager.GetActiveScene();

            _signalBus.Fire(new CloseWindowSignal(WindowKey.StartWindow));
            PlayerPrefsX.SetBool("WithPlayer", signal.WithPlayer);
            await SceneManager.LoadSceneAsync("CheckersMain_online", LoadSceneMode.Additive);

            await _gameStateMachine.Fire(Trigger.CheckersTrigger);

            await SceneManager.UnloadSceneAsync(currentScene);

            await _gameStateMachine.Fire(Trigger.AfterSceneLoadTrigger);
        }

        public void Dispose() {
            _signalBus.Unsubscribe<ToCheckersMetaSignal>(LoadSceneInternal);
        }
    }
}