using System;
using Global.Enums;

namespace Global.ConfigTemplate {
    [Serializable]
    public class AppConfig {
        public float PauseTime;
        public float TurnTime;
        public float RemainTime;

        [NonSerialized] 
        public bool GameEnded;
        [NonSerialized] 
        public bool MyLeaveTimerExpired;
        [NonSerialized]
        public PawnColor PawnColor;
        [NonSerialized]
        public string OpponentDisplayName;
        [NonSerialized]
        public string OpponentUserId;
        [NonSerialized]
        public bool InMatch;
        [NonSerialized]
        public bool InSearch;
        [NonSerialized]
        public bool Leave;

        public void Reset() {
            PawnColor = 0;
            OpponentDisplayName = "";
            OpponentUserId = "";
            InMatch = false;
            InSearch = false;
            MyLeaveTimerExpired = false;
            GameEnded = false;
            Leave = false;
        }
    }
}