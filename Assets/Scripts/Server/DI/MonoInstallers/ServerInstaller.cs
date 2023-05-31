using Server.Services;
using Zenject;

namespace Server.DI.MonoInstallers {
    public class ServerInstaller : MonoInstaller<ServerInstaller> {
        public override void InstallBindings() {
            Container
                .BindInterfacesAndSelfTo<NakamaService>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<GlobalMessageListener>()
                .AsSingle()
                .NonLazy();
        }
    }
}