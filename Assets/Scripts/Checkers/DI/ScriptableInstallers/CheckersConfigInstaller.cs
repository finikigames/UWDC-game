using Checkers.ConfigTemplate;
using UnityEngine;
using Zenject;

namespace Checkers.DI.ScriptableInstallers {
    [CreateAssetMenu(menuName = "Installers/Checkers/Checkers config installer", fileName = "CheckersConfigInstaller")]
    public class CheckersConfigInstaller : ScriptableObjectInstaller<CheckersConfigInstaller> {
        public CheckersConfig CheckersConfig;
        public CheckersUIConfig CheckersUIConfig;
        
        public override void InstallBindings() {
            Container
                .BindInstance(CheckersConfig)
                .AsSingle();

            Container
                .BindInstance(CheckersUIConfig)
                .AsSingle();
        }
    }
}