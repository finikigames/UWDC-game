using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Global.Services {
    public class ApplicationQuit : MonoBehaviour {
        private static Action _onPause { get; set; }
        private static Action _onResume { get; set; }
        [DllImport("__Internal")]
        private static extern void registerVisibilityChangeEvent();
 
        void Start() {
            registerVisibilityChangeEvent();
        }
 
        [Preserve]
        void OnVisibilityChange(string visibilityState) {
            var focus = visibilityState == "visible";
            if (!focus) {
                WantsToQuit();
                return;
            }

            _onResume?.Invoke();
        }
        
        private void OnApplicationFocus(bool focus) {
            if (!focus) {
                WantsToQuit();
                return;
            }

            _onResume?.Invoke();
        }

        private void OnApplicationPause(bool pause) {
            if (pause) {
                WantsToQuit();
                return;
            }

            _onResume?.Invoke();
        }

        private static bool WantsToQuit() {
            _onPause?.Invoke();
            return true;
        }

        public static void SubscribeOnQuit(Action callback) {
            _onPause += callback;
        }

        public static void UnSubscribeOnQuit(Action callback) {
            _onPause -= callback;
        }

        public static void SubscribeOnResume(Action callback) {
            _onResume += callback;
        }

        public static void UnSubscribeOnResume(Action callback) {
            _onResume -= callback;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart() {
            Application.wantsToQuit += WantsToQuit;
        }
    }
}