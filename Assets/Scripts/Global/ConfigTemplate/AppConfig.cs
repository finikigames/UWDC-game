using System;
using Global.Enums;
using UnityEngine;

namespace Global.ConfigTemplate {
    [Serializable]
    public class AppConfig {
        public float PauseTime;
        public float TurnTime;
        
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

        public void Reset() {
            PawnColor = 0;
            OpponentDisplayName = "";
            OpponentUserId = "";
            InMatch = false;
            InSearch = false;
        }
    }
}