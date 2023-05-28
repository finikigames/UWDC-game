using Checkers.Services;
using Checkers.Settings;
using Checkers.States;
using Global;
using Zenject;

namespace Checkers.DI.MonoInstallers {
    public class CheckersOnlineInstaller : MonoInstaller {
        public MainCheckerSceneSettings CheckerSceneSettings;
        
        public override void InstallBindings() {
            Container
                .BindInstance(CheckerSceneSettings)
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<MainCheckersOnlineService>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<CheckersRootState>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<MainInitialize>()
                .AsSingle();
        }
    }
}