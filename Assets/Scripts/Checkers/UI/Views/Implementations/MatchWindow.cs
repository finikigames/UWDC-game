using System;
using Checkers.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Checkers.UI.Views.Implementations {
    public class MatchWindow : BaseWindow, 
                               IMatchWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _howToPlayButton;
        [SerializeField] private Button _fleeButton;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TextMeshProUGUI _opponentName;
        [SerializeField] private TextMeshProUGUI _yourName;
        [SerializeField] private TextMeshProUGUI _turnTimer;
        [SerializeField] private PlayerChekersBar _opponentChekersBar;
        [SerializeField] private PlayerChekersBar _playerChekersBar;

        private bool _tweenStarted;
        private bool _isWhite;
        private float _currentTime;
        private readonly int _borderTime = 5;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public override void Initialize() {
            _turnTimer.color = Color.white;
            _turnTimer.transform.localScale = Vector3.one;
        }

        public void ProvideCamera(UnityEngine.Camera camera) {
            _canvas.worldCamera = camera;
        }

        public void SubscribeToHowToPlayButton(Action callback) {
            _howToPlayButton.onClick.RemoveAllListeners();
            _howToPlayButton.onClick.AddListener(() => callback?.Invoke());
        }

        public void SubscribeToFleeButton(Action callback) {
            _fleeButton.onClick.RemoveAllListeners();
            _fleeButton.onClick.AddListener(() => callback?.Invoke());
        }

        public void SetYourName(string yourName) {
            _yourName.text = yourName;
        }

        public void SetOpponentName(string opponentName) {
            _opponentName.text = opponentName;
        }

        public void GetLostCheсker(bool isPlayer) {
            var bar = isPlayer ? _opponentChekersBar : _playerChekersBar;

            if (!_isWhite) {
                bar = isPlayer ? _playerChekersBar : _opponentChekersBar;
            }
            
            bar.DecreaseСhecker();
        }

        public void ResetBars(bool isWhite) {
            _isWhite = isWhite;

            _playerChekersBar.ResetBar();
            _opponentChekersBar.ResetBar();
        }

        public Vector3 GetSendPawnPosition(bool isPlayer) {
            var bar = isPlayer ? _playerChekersBar.GetPosition() : _opponentChekersBar.GetPosition();
            return bar;
        }
        
        public void SetTimerTime(int time) {
            _turnTimer.text = time.ToString();
            _currentTime = time;
            
            if (time <= _borderTime && !_tweenStarted) {
                _tweenStarted = true;
                ActivateTween();
            }
        }
        
        private void ActivateTween() {
            DOTween.Sequence()
                .Join(_turnTimer.DOColor(Color.red, 0))
                .Append(_turnTimer.transform.DOScale(1.25f, .5f).SetEase(Ease.InOutSine))
                .Append(_turnTimer.transform.DOScale(1f, .5f).SetEase(Ease.InOutSine))
                .OnComplete(CheckStatus);
        }

        private void CheckStatus() {
            if (_currentTime <= _borderTime) {
                ActivateTween();
            }
            else {
                _turnTimer.color = Color.white;
                _turnTimer.transform.localScale = Vector3.one;

                _tweenStarted = false;
            }
        }
    }
}