using System;
using Core;
using Core.MVP.Base.Interfaces;
using Core.MVP.ShowStates.Interfaces;
using Cysharp.Threading.Tasks;

namespace Global.VisibilityMechanisms {
    public abstract class BaseView : UnityComponent,
        IView {
        public Action<string> OnDestroyView { get; set; }
        public string Uid;

        private IHideMechanism _hideMechanism;
        private IShowMechanism _showMechanism;

        public virtual void Initialize(string uid) {
            Uid = uid;
            _hideMechanism.HideImmediate(gameObject);
        }

        /// <summary>
        ///     You can override this method to change hide|show mechanism
        /// </summary>
        protected virtual void OnEnable() {
            _hideMechanism = new SetActiveHideMechanism();
            _showMechanism = new SetActiveShowMechanism();
        }

        public virtual async UniTask ShowView(Action onShow = null) {
            _showMechanism.Show(gameObject, onShow);
        }

        public virtual async UniTask HideView(Action onHide = null) {
            _hideMechanism.Hide(gameObject, onHide);
        }

        public void HideImmediate() {
            _hideMechanism.HideImmediate(gameObject);
        }

        public void ShowImmediate() {
            _showMechanism.ShowImmediate(gameObject);
        }
        
        public virtual void Dispose() { }

        protected void ChangeShowMechanism(IShowMechanism showMechanism) {
            _showMechanism = showMechanism;
        }

        protected void ChangeHideMechanism(IHideMechanism hideMechanism) {
            _hideMechanism = hideMechanism;
        }

        public virtual async UniTask InitializeOnce() {
        }
    }
}