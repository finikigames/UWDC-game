using System;

namespace Global.ConfigTemplate {
    [Serializable]
    public class AppConfig {
        public int PawnColor;
        public string OpponentDisplayName;
        public string OpponentUserId;
        public bool InMatch;
    }
}