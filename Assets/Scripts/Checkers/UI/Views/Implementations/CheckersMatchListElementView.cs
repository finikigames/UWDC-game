using System;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views.Implementations {
    public class CheckersMatchListElementView : EnhancedScrollerCellView {
        [SerializeField] private Image _bgImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Button _chooseButton;
        [SerializeField] private TextMeshProUGUI _playersCountText;

        public float Height;

        public void Initialize(string id, Action<CheckersMatchListElementView, string> onChoose) {
            _nameText.text = id;
            
            _chooseButton.onClick.RemoveAllListeners();
            _chooseButton.onClick.AddListener(() => onChoose?.Invoke(this, id));
        }

        public void SetPlayersCount(int count) {
            _playersCountText.text = $"{count.ToString()}/2";
        }

        public void SetChoose() {
            _bgImage.color = Color.cyan;
        }

        public void RemoveChoose() {
            _bgImage.color = Color.white;
        }
    }
}