using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;

namespace Main.UI.Views.Implementations.LeaderboardWindow {
    public class LeaderboardUserCellView : EnhancedScrollerCellView {
        [SerializeField] private TextMeshProUGUI _userNickname;
        [SerializeField] private TextMeshProUGUI _winText;
        
        public float Height;
        
        public void Init(string nicknameText, string winText) {
            _userNickname.text = nicknameText;
            _winText.text = winText;
        }
    }
}