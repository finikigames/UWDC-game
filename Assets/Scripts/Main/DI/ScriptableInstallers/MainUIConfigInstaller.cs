using Main.ConfigTemplate;
using UnityEngine;
using Zenject;

namespace Main.DI.ScriptableInstallers {
    [CreateAssetMenu(menuName = "Configs/MainUIConfigInstaller", fileName = "MainUIConfigInstaller")]
    public class MainUIConfigInstaller : ScriptableObjectInstaller<MainUIConfigInstaller> {
        public MainUIConfig Config;        
        
        public override void InstallBindings() {
            Container
                .BindInstance(Config)
                .AsSingle();
        }
    }
}