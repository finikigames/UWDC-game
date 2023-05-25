using System;
using System.Collections.Generic;
using Core;
using Core.MVP.Base.Enums;
using Core.MVP.Base.Interfaces;
using Core.MVP.ShowStates.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Global.VisibilityMechanisms {
    public abstract class BaseView : UnityComponent,
                                     IHide, 
                                     IShow,
                                     IView {
        public Action<string> OnDestroyView { get; set; }
        public string Uid;
        
        [SerializeField, HideInInspector]
        protected List<Button> _buttons = new();
        
        private IHideMechanism _hideMechanism;
        private IShowMechanism _showMechanism;

        protected ShowState _showState;

        public ShowState ShowState => _showState;
        
        public virtual void Initialize(string uid) {
            Uid = uid;
            _hideMechanism.HideImmediate(gameObject);
        }

        /// <summary>
        ///     You can override this method to change hide|show mechanism
        /// </summary>
        protected virtual void OnEnable() {
            _showState = ShowState.Shown;

            _hideMechanism = new SetActiveHideMechanism();
            _showMechanism = new SetActiveShowMechanism();
        }

        public void Hide(Action onHide = null) {
            if (_showState != ShowState.Hidden) {
                _hideMechanism.Hide(gameObject, onHide);
                _showState = ShowState.Hidden;
            }
        }

        public void HideImmediate() {
            if (_showState != ShowState.Hidden) {
                _hideMechanism.HideImmediate(gameObject);
                _showState = ShowState.Hidden;
            }
        }

        public virtual void Dispose() {
            
        }

        public void ShowImmediate() {
            if (_showState != ShowState.Shown) {
                _showMechanism.ShowImmediate(gameObject);
                _showState = ShowState.Shown;
            }
        }

        public void Show(Action onShow = null) {
            if (_showState != ShowState.Shown) {
                _showMechanism.Show(gameObject, onShow);
                _showState = ShowState.Shown;
            }
        }

        protected void ChangeShowMechanism(IShowMechanism showMechanism) {
            _showMechanism = showMechanism;
        }

        protected void ChangeHideMechanism(IHideMechanism hideMechanism) {
            _hideMechanism = hideMechanism;
        }

        public virtual async UniTask ShowView() {
        }
        
        public virtual async UniTask Hide() {
        }
    }
}