using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Global.VisibilityMechanisms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Global.Window.Base {
    public class BaseWindow : BaseView {
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] protected Button _closeButton;
        [SerializeField] protected Button _closeAdditionalArea;

        [SerializeField] protected Image _blackBg;

        public Action OnClickCloseButton {
            get;
            set;
        }

        #region BaseView

        protected override void OnEnable() {
            ChangeShowMechanism(new ChainShowMechanism(
                new ScaleShowMechanism(),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new ScaleHideMechanism(),
                new CustomHideMechanism(HideBlack)));
        }

        #endregion

        #region IWindowView

        public void ChangeHeader(string header) {
            _headerText.text = header;
        }

        #endregion

        public void SubscribeToClose(Action onClose) {
            _closeButton.onClick.AddListener(onClose.Invoke);
        }
        
        [Serializable]
        public class WindowSettings {
            public float fadeTime;
        }

        #region BaseWindow

        public override async UniTask ShowView(Action onShow = null) {
            base.ShowView(onShow);
            
            Initialize();
            
            if (_closeButton != null) {
                _closeButton.onClick.RemoveAllListeners();
                _closeButton.onClick.AddListener(() => OnClickCloseButton?.Invoke());
                // TEMP remove after all views is moved to the new ui system
                if (OnClickCloseButton == null) {
                    _closeButton.onClick.AddListener(() => HideView());
                }
            }

            if (_closeAdditionalArea != null) {
                _closeAdditionalArea.onClick.RemoveAllListeners();
                _closeAdditionalArea.onClick.AddListener(() => OnClickCloseButton?.Invoke());
                // TEMP remove after all views is moved to the new ui system
                if (OnClickCloseButton == null) {
                    _closeAdditionalArea.onClick.AddListener(() => HideView());
                }
            }
        }

        public virtual void Initialize() { }

        protected void ShowBlack() {
            _blackBg.enabled = true;
            _blackBg.DOFade(0.8f, 0.3f);
        }

        protected void HideBlack() {
            DOTween.Sequence()
                .Append(_blackBg.DOFade(0, 0.3f))
                .AppendCallback(() => { _blackBg.enabled = true; });
        }

        #endregion
    }
}