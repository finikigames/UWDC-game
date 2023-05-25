using System;
using UnityEngine.Serialization;

namespace Checkers.ConfigTemplate {
    [Serializable]
    public class EditorCheckersConfig {
        public string UserId;
        public string FirstName;
        public string LastName;
        [FormerlySerializedAs("Nickname")]
        public string Username;
    }
}