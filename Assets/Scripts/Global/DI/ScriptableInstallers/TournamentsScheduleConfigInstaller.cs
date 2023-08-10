using Global.ConfigTemplate;
using UnityEngine;
using Zenject;

namespace Global.DI.ScriptableInstallers {
    [CreateAssetMenu(fileName = "TournamentsScheduleConfigInstaller", menuName = "Installers/Checkers/TournamentsScheduleConfigInstaller")]
    public class TournamentsScheduleConfigInstaller : ScriptableObjectInstaller<TournamentsScheduleConfigInstaller> {
        public TournamentsScheduleConfig Config;
        
        public override void InstallBindings() {
            Container
                .BindInterfacesAndSelfTo<TournamentsScheduleConfig>()
                .FromInstance(Config)
                .AsSingle();
        }
    }
}