using System;
using EnhancedUI.EnhancedScroller;
using Main.UI.Presenters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI.Views.Implementations {
    public class StartWindowUserCellView : EnhancedScrollerCellView {
        [SerializeField] private TextMeshProUGUI _userNickname;
        [SerializeField] private Button _clickButton;
        [SerializeField] private TextMeshProUGUI _buttonText;

        public float Height;
        
        public void SetNickname(string nickname) {
            _userNickname.text = nickname;
        }

        public void Init() {
            _buttonText.text = "в бой";
        }
        
        public void SubscribeOnClick(UserInfoData data,
                                     Action<UserInfoData, StartWindowUserCellView> sendCallback) {
            _clickButton.onClick.RemoveAllListeners();
            _clickButton.onClick.AddListener(() => {
                sendCallback?.Invoke(data, this);
            });
        }
        
        public void SetSendText() {
            _buttonText.text = "отправлено";
        }

        public void OnReject() {
            _buttonText.text = "отказался";
        }
    }
}