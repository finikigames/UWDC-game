using Core.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Global.ConfigTemplate;
using Global.Services;
using Global.StateMachine;
using Global.StateMachine.Base.Enums;
using Global.UI.Data;
using Global.Window;
using Global.Window.Enums;
using Global.Window.Signals;
using Server;
using Server.Services;
using UnityEngine.SceneManagement;
using Zenject;

namespace Preloader {
    public class EntryPoint : IInitializable {
        private readonly ProfileGetService _getService;
        private readonly NakamaService _nakamaService;
        private readonly GameStateMachine _gameStateMachine;
        private readonly GlobalMessageListener _messageListener;
        private readonly AppConfig _appConfig;
        private readonly WindowService _windowService;
        private readonly SignalBus _signalBus;

        public EntryPoint(ProfileGetService getService,
                          NakamaService nakamaService,
                          GameStateMachine gameStateMachine,
                          GlobalMessageListener messageListener,
                          AppConfig appConfig,
                          WindowService windowService,
                          SignalBus signalBus) {
            _getService = getService;
            _nakamaService = nakamaService;
            _gameStateMachine = gameStateMachine;
            _messageListener = messageListener;
            _appConfig = appConfig;
            _windowService = windowService;
            _signalBus = signalBus;
        }
        
        public void Initialize() {
            _appConfig.Reset();
            InitializeInternal();
        }

        private async UniTask InitializeInternal() {
            DOTween.Init();
            
            _getService.Initialize();
            var profile = _getService.GetProfile();
            
            _nakamaService.ProvideData(profile);
            
            _gameStateMachine.Initialize();

            await _nakamaService.CommonInitialize();
            
            _messageListener.Initialize();

            /*if (!PlayerPrefsX.GetBool("NotFirstStart")) {
                PlayerPrefsX.SetBool("NotFirstStart", true);
                _signalBus.Fire(new OpenWindowSignal(WindowKey.RulesWindow, new RulesWindowData()));
            }*/

            var currentScene = SceneManager.GetActiveScene().buildIndex;
            await SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);

            await _gameStateMachine.Fire(Trigger.MainTrigger);

            await SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}