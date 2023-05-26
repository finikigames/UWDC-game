using Cysharp.Threading.Tasks;
using DG.Tweening;
using Global.Services;
using Global.StateMachine;
using Global.StateMachine.Base.Enums;
using Server.Services;
using UnityEngine.SceneManagement;
using Zenject;

namespace Preloader {
    public class EntryPoint : IInitializable {
        private readonly ProfileGetService _getService;
        private readonly NakamaService _nakamaService;
        private readonly GameStateMachine _gameStateMachine;

        public EntryPoint(ProfileGetService getService,
                          NakamaService nakamaService,
                          GameStateMachine gameStateMachine) {
            _getService = getService;
            _nakamaService = nakamaService;
            _gameStateMachine = gameStateMachine;
        }
        
        public void Initialize() {
            InitializeInternal();
        }

        private async UniTask InitializeInternal() {
            DOTween.Init();
            
            _getService.Initialize();
            var profile = _getService.GetProfile();
            
            _nakamaService.ProvideData(profile);
            
            _gameStateMachine.Initialize();

            await _nakamaService.CommonInitialize();

            var currentScene = SceneManager.GetActiveScene().buildIndex;
            await SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);

            await _gameStateMachine.Fire(Trigger.MainTrigger);

            await SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}