using Global.ConfigTemplate;
using UnityEngine;
using Zenject;

namespace Global.DI.Installers.ScriptableInstallers {
    [CreateAssetMenu(fileName = "Windows Config", menuName = "Configs/Windows Config")]
    public class WindowsConfigsInstaller : ScriptableObjectInstaller<WindowsConfigsInstaller> {
        public WindowsConfigs WindowsConfig;

        public override void InstallBindings() {
            Container
                .BindInterfacesAndSelfTo<WindowsConfigs>()
                .FromInstance(WindowsConfig)
                .AsSingle();
        }
    }
}
