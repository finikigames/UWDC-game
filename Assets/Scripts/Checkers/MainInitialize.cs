using System;
using Checkers.UI.Data;
using Cysharp.Threading.Tasks;
using Global.ConfigTemplate;
using Global.StateMachine;
using Global.StateMachine.Base.Enums;
using Global.Window.Enums;
using Global.Window.Signals;
using Server.Services;
using UnityEngine.SceneManagement;
using Zenject;

namespace Checkers {
    public class MainInitialize : IInitializable,
                                  IDisposable {
        private readonly SignalBus _signalBus;
        private readonly GameStateMachine _gameStateMachine;
        private readonly NakamaService _nakamaService;
        private readonly AppConfig _appConfig;

        public MainInitialize(SignalBus signalBus,
                              GameStateMachine gameStateMachine,
                              NakamaService nakamaService,
                              AppConfig appConfig) {
            _signalBus = signalBus;
            _gameStateMachine = gameStateMachine;
            _nakamaService = nakamaService;
            _appConfig = appConfig;
        }
        
        public void Initialize() {
            _signalBus.Subscribe<ToMainSignal>(LoadSceneInternal);
        }

        private async void LoadSceneInternal() {
            await LoadYourAsyncScene();
        }
        
        public async UniTask LoadYourAsyncScene() {
            await _nakamaService.GoOnline();
            
            var currentScene = SceneManager.GetActiveScene();

            _appConfig.GameEnded = false;
            _appConfig.InMatch = false;
            
            _signalBus.Fire(new CloseWindowSignal(WindowKey.MatchWindow));
            await _nakamaService.RemoveAllParties();
            await _nakamaService.LeaveCurrentMatch();
            
            await SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);

            await _gameStateMachine.Fire(Trigger.MainTrigger);

            await SceneManager.UnloadSceneAsync(currentScene);
        }

        public void Dispose() {
            _signalBus.Unsubscribe<ToMainSignal>(LoadSceneInternal);
        }
    }
}