using System;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI.Views.Implementations {
    public class StartWindowUserCellView : EnhancedScrollerCellView {
        [SerializeField] private TextMeshProUGUI _userNickname;
        [SerializeField] private Button _clickButton;

        public float Height;
        
        public void SetNickname(string nickname) {
            _userNickname.text = nickname;
        }
        
        public void SubscribeOnClick(string userId, Action<string> callback, Action<string> nameCallback) {
            _clickButton.onClick.RemoveAllListeners();
            _clickButton.onClick.AddListener(() => {
                callback?.Invoke(userId);
                nameCallback?.Invoke(_userNickname.text);
            });
        }
    }
}