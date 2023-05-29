using Core.AssetManager;
using Core.Ticks;
using Global.Context;
using Global.Scheduler;
using Global.Services;
using Global.Services.Timer;
using Global.StateMachine;
using Global.StateMachine.States;
using Global.Window;
using Global.Window.Signals;
using UnityEngine;
using Zenject;

namespace Global.DI.MonoInstallers {
    public class ProjectInstaller : MonoInstaller {
        [SerializeField] private GameObject _sceneLoaderOverlay;
        [SerializeField] private UpdateService _updateService;
        
        public override void InstallBindings() {
            SignalBusInstaller.Install(Container);

            InstallApp();
            InstallAppServices();
            InstallCommonViews();
            InstallHelpers();
            InstallUseCases();
            InstallFactories();
            InstallSignals();

            Container
                .BindInterfacesAndSelfTo<AnalyticsService>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<UpdateService>()
                .FromInstance(_updateService)
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<TimerService>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<GameStateMachine>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<ProfileGetService>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<AddressableAssetService>()
                .AsSingle()
                .NonLazy();
        }

        private void InstallApp() {
            Container
                .Bind<StateProvider>()
                .AsSingle();
        }

        private void InstallAppServices() {
            Container
                .BindInterfacesAndSelfTo<ContextService>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ProjectContextProvider>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<SchedulerService>()
                .AsSingle();
                
            Container
                .BindInterfacesAndSelfTo<WindowService>()
                .AsSingle()
                .NonLazy();
        }

        private void InstallCommonViews() {
        }

        private void InstallHelpers() {
        }

        private void InstallUseCases() {
        }

        private void InstallFactories() {
            Container
                .Bind<WindowFactory>()
                .AsSingle()
                .NonLazy();
        }

        private void InstallSignals() {
            Container
                .DeclareSignal<PreloadWindowSignal>();
            
            Container
                .DeclareSignal<OpenWindowSignal>();

            Container
                .DeclareSignal<CloseWindowSignal>();
        }
    }
}