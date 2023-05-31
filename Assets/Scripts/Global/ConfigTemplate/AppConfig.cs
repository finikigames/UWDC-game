using System;
using UnityEngine.Serialization;

namespace Global.ConfigTemplate {
    [Serializable]
    public class AppConfig {
        public int PawnColor;
        [FormerlySerializedAs("Opponent")] public string OpponentDisplayName;
        public string OpponentUserId;
    }
}