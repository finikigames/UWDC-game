﻿using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI.Views.Implementations.LeaderboardWindow {
    public class LeaderboardUserCellView : EnhancedScrollerCellView {
        [SerializeField] private TextMeshProUGUI _userNickname;
        [SerializeField] private TextMeshProUGUI _winText;
        [SerializeField] private Image _frameSprite;
        [SerializeField] private Image _youFrame;
        
        public float Height;

        public void SetYouFrame(Sprite frameSprite, bool you) {
            if (!you) {
                _youFrame.gameObject.SetActive(false);
                return;
            }

            _youFrame.sprite = frameSprite;
            _youFrame.gameObject.SetActive(true);
        }
        
        public void Init(string nicknameText, string winText) {
            _userNickname.text = nicknameText;
            _winText.text = winText;
        }

        public void ChangeSprite(Sprite frameSprite) {
            _frameSprite.sprite = frameSprite;
        }
    }
}