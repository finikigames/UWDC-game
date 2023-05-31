using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Main.UI.Views {
    [Preserve]
    public class LogoView : MonoBehaviour {
        [SerializeField] private Button _logoButton;
        
        [DllImport("__Internal")]
        private static extern void OpenNewTab(string url);

        public void openIt(string url)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             OpenNewTab(url);
#endif
        }

        private void OnEnable() {
            _logoButton.onClick.RemoveAllListeners();
            _logoButton.onClick.AddListener(() => openIt("https://finiki.games/"));
        }
    }
}