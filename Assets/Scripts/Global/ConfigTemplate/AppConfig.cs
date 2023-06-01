using System;

namespace Global.ConfigTemplate {
    [Serializable]
    public class AppConfig {
        public int PawnColor;
        public string OpponentDisplayName;
        public string OpponentUserId;
        public bool InMatch;
        public bool InSearch;

        public void Reset() {
            PawnColor = 0;
            OpponentDisplayName = "";
            OpponentUserId = "";
            InMatch = false;
            InSearch = false;
        }
    }
}