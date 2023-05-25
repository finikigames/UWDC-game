using Core.AssetManager;
using Global.Services.Context;
using Global.Services.Scheduler;
using Global.StateMachine.States;
using Global.Window;
using Global.Window.Signals;
using UnityEngine;
using Zenject;

namespace Global {
    public class ProjectInstaller : MonoInstaller {
        [SerializeField] private GameObject _sceneLoaderOverlay;

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