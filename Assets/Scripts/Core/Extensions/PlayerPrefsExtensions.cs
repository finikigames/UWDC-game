using Core.Utils;
using UnityEngine;

namespace Core.Extensions {
    public static class PlayerPrefsX {
        public static bool GetBool(string key) {
            return PlayerPrefs.GetInt(key) == 1;
        }

        public static void SetBool(string key, bool value) {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static void SetEnum<T>(string key, T value) where T : System.Enum {
            var intValue = CastTo<int>.From(value);
            
            PlayerPrefs.SetInt(key, intValue);
        }

        public static T GetEnum<T>(string key) where T : System.Enum {
            var intValue = PlayerPrefs.GetInt(key);

            return CastTo<T>.From(intValue);
        }
    }
}