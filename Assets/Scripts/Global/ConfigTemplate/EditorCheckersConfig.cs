using System;
using UnityEngine.Serialization;

namespace Global.ConfigTemplate {
    [Serializable]
    public class EditorCheckersConfig {
        public string UserId;
        public string FirstName;
        public string LastName;
        [FormerlySerializedAs("Nickname")]
        public string Username;
    }
}