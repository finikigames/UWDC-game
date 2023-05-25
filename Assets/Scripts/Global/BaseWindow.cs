using System;
using System.Threading.Tasks;
using Core.MVP.Base.Enums;
using DG.Tweening;
using Global.VisibilityMechanisms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Global.UI {
    public class BaseWindow : BaseView {
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] protected Button _closeButton;

        [SerializeField] protected Image _blackBg;

        private WindowSettings _windowSettings;

        public Action OnClickCloseButton {
            get;
            set;
        }

        [Inject]
        public void Construct(WindowSettings windowSettings) {
            _windowSettings = windowSettings;
        }

        #region BaseView

        protected override void OnEnable() {
            _showState = ShowState.Hidden;
            
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

        public virtual async Task ShowView() {
            Open();
        }

        public virtual void Open() {
            Initialize();

            Show();

            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() => OnClickCloseButton?.Invoke());
            // TEMP remove after all views is moved to the new ui system
            _closeButton.onClick.AddListener(() => Close());
        }

        protected virtual void Close(Action onClose = null) {
            Hide(onClose);
        }

        public virtual void Initialize() { }

        protected void ShowBlack() {
            _blackBg.enabled = true;
            _blackBg.DOFade(0.8f, _windowSettings.fadeTime);
        }

        protected void HideBlack() {
            DOTween.Sequence()
                .Append(_blackBg.DOFade(0, _windowSettings.fadeTime))
                .AppendCallback(() => { _blackBg.enabled = true; });
        }

        #endregion
    }
}