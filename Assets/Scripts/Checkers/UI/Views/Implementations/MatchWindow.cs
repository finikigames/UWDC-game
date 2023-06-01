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
        [SerializeField] private TextMeshProUGUI _pauseTimer;
        [SerializeField] private PlayerChekersBar _opponentChekersBar;
        [SerializeField] private PlayerChekersBar _playerChekersBar;
        [SerializeField] private Transform _pauseBody;

        private bool _tweenStarted;
        private bool _isWhite;

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

        public void SetTimerTime(int time) {
            _turnTimer.text = time.ToString();
            
            if (time <= 5 && !_tweenStarted)
            {
                _tweenStarted = true;
                _turnTimer.DOColor(Color.red, 1);
                _turnTimer.transform.DOScale(1.25f, .5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void SetPauseTime(int time) {
            _pauseTimer.text = time.ToString();
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

        public void SetPauseStateView(bool state) {
            _pauseBody.gameObject.SetActive(state);
        }
    }
}