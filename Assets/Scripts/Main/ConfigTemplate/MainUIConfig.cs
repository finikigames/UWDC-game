using System;
using Main.UI.Views.Implementations;
using Main.UI.Views.Implementations.LeaderboardWindow;
using UnityEngine;

namespace Main.ConfigTemplate {
    [Serializable]
    public class MainUIConfig {
        public StartWindowUserCellView Prefab;
        public LeaderboardUserCellView LeaderboardPrefab;

        public Sprite GoldFrame;
        public Sprite SilverFrame;
        public Sprite BronzeFrame;
        public Sprite YourFrame;
        public Sprite NormalFrame;
    }
}