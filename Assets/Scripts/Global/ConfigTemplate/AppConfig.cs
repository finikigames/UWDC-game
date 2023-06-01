using System;
using Global.Enums;
using UnityEngine;

namespace Global.ConfigTemplate {
    [Serializable]
    public class AppConfig {
        public float PauseTime;
        public float TurnTime;
        
        [HideInInspector]
        public PawnColor PawnColor;
        [HideInInspector]
        public string OpponentDisplayName;
        [HideInInspector]
        public string OpponentUserId;
        [HideInInspector]
        public bool InMatch;
        [HideInInspector]
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