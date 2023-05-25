using Checkers.States;
using Checkers.UI.Data;
using Zenject;

namespace Checkers.DI.MonoInstallers {
    public class CheckersMetaInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container
                .BindInterfacesAndSelfTo<CheckersInitialize>()
                .AsSingle()
                .NonLazy();

            Container
                .DeclareSignal<ToCheckersMetaSignal>();
        }
    }
}