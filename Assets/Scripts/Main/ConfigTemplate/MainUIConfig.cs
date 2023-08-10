using System;
using Main.UI.Views.Implementations;
using Main.UI.Views.Implementations.LeaderboardWindow;

namespace Main.ConfigTemplate {
    [Serializable]
    public class MainUIConfig {
        public StartWindowUserCellView Prefab;
        public LeaderboardUserCellView LeaderboardPrefab;
    }
}