using Checkers.UI.Data;
using Cysharp.Threading.Tasks;
using Global.Context.Base;
using Global.StateMachine;
using Global.StateMachine.Base.Enums;
using Global.Window.Enums;
using Global.Window.Signals;
using Server.Services;
using UnityEngine.SceneManagement;
using Zenject;

namespace Checkers {
    public class MainInitialize : IInitializable {
        private SignalBus _signalBus;
        private readonly GameStateMachine _gameStateMachine;
        private readonly NakamaService _nakamaService;

        public MainInitialize(GameStateMachine gameStateMachine,
                              NakamaService nakamaService,
                              IContextService contextService) {
            _signalBus = contextService.ResolveContainer(GameContext.Checkers).Resolve<SignalBus>();
            _gameStateMachine = gameStateMachine;
            _nakamaService = nakamaService;
        }
        
        public void Initialize() {
            _signalBus.Subscribe<ToMainSignal>(async _ => await LoadYourAsyncScene());
        }
        
        public async UniTask LoadYourAsyncScene() {
            var currentScene = SceneManager.GetActiveScene();
            
            _signalBus.Fire(new CloseWindowSignal(WindowKey.MatchWindow));
            await _nakamaService.RemoveAllParties();
            await _nakamaService.LeaveCurrentMatch();
            
            await SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);

            await _gameStateMachine.Fire(Trigger.MainTrigger);

            await SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}