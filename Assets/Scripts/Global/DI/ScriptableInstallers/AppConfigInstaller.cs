using Global.ConfigTemplate;
using UnityEngine;
using Zenject;

namespace Global.DI.ScriptableInstallers {
    [CreateAssetMenu(menuName = "Configs/AppConfigInstaller")]
    public class AppConfigInstaller : ScriptableObjectInstaller<AppConfigInstaller> {
        public AppConfig Config;

        public override void InstallBindings() {
            Container
                .BindInstance(Config)
                .AsSingle();
        }
    }
}