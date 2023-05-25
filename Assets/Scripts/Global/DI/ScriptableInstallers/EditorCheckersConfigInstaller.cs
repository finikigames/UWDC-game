using Checkers.ConfigTemplate;
using UnityEngine;
using Zenject;

namespace Global.DI {
    [CreateAssetMenu(menuName = "Configs/EditorCheckersConfig")]
    public class EditorCheckersConfigInstaller : ScriptableObjectInstaller<EditorCheckersConfigInstaller> {
        public EditorCheckersConfig CheckersConfig;

        public override void InstallBindings() {
            Container
                .BindInstance(CheckersConfig)
                .AsSingle();
        }
    }
}