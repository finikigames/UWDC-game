using Checkers.Services;
using Checkers.Settings;
using Global;
using Zenject;

namespace Checkers.DI.MonoInstallers {
    public class CheckersMainInstaller : MonoInstaller {
        public MainCheckerSceneSettings CheckerSceneSettings;
        
        public override void InstallBindings() {
            Container
                .BindInstance(CheckerSceneSettings)
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<MainCheckerService>()
                .AsSingle()
                .NonLazy();
        }
    }
}