using UnityEngine.Analytics;
using Zenject;

namespace Global.Services {
    public class AnalyticsService : IInitializable {
        public void Initialize() {
            
        }

        public void SessionStart() {
            //Analytics.CustomEvent("sessionStart");
        }
    }
}