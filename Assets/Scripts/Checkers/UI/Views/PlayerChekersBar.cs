using Core.Extensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views {
    public class PlayerChekersBar : MonoBehaviour {
        [SerializeField] private Transform _checkersContainer;
        private int _lostCheckers = 0;

        public void ResetBar() {
            foreach (Transform checker in _checkersContainer) {
                checker.gameObject.GetComponent<Image>().color = Color.white;
            }

            _lostCheckers = 0;
        }

        public void DecreaseСhecker() {
            var childIndex = _checkersContainer.childCount - _lostCheckers - 1;
            var checker = _checkersContainer.GetChild(childIndex);
            checker.gameObject.GetComponent<Image>().DOFade(0, 0.5f);
            _lostCheckers++;
        }

        public Vector3 GetPosition() {
            return _checkersContainer.AsRectTransform().position;
        }
    }
}