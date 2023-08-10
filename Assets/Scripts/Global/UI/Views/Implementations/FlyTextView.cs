using System;
using DG.Tweening;
using Global.Enums;
using Global.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Global.UI.Views.Implementations {
    public class FlyTextView : BaseWindow, 
                               IFlyTextView {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _flyText;
        [SerializeField] private Image _bg;
        [SerializeField] private Sprite _bgGreen;
        [SerializeField] private Sprite _bgRed;

        [Inject] private readonly SignalBus _signalBus;

        private bool _shown;
        
        protected override void OnEnable() {
            ChangeShowMechanism(new FadeShowMechanism(_group));
            ChangeHideMechanism(new FadeHideMechanism(_group));
        }

        public void InitializeView() {
            _bg.sprite = _bgGreen;
        }

        public void ShowFlyText(string text) {
            if (!_shown) {
                _flyText.text = text;
                _shown = true;
                
                DOTween.Sequence()
                    .Append(DOTween.To(() => _group.alpha, x => _group.alpha = x, 1f, .3f))
                    .SetDelay(1.5f)
                    .Append(DOTween.To(() => _group.alpha, x => _group.alpha = x, 0f, .6f))
                    .OnComplete(() => {
                        _signalBus.Fire(new CloseWindowSignal(WindowKey.FlyText));
                        _shown = false;
                    });
            }
        }

        public void SetBackgroundColor(PawnColor color) {
            _bg.sprite = color == PawnColor.White ? _bgGreen : _bgRed;
        }
    }
}