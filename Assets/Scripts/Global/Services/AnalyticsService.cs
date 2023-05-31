using Unity.Services.Core;
using UnityEngine.Analytics;
using Zenject;

namespace Global.Services {
    public class AnalyticsService : IInitializable {
        public async void Initialize() {
            //await UnityServices.InitializeAsync();
        }

        public void SessionStart() {
            //Analytics.CustomEvent("sessionStart");
        }
    }
}