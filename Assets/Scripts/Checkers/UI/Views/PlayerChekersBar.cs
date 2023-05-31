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
                checker.gameObject.GetComponent<Image>().color =new Color(1,1,1,0);
            }

            _lostCheckers = 0;
        }

        public void DecreaseСhecker() {
            var childIndex = _lostCheckers;
            var checker = _checkersContainer.GetChild(childIndex);
            checker.gameObject.GetComponent<Image>().DOFade(1, 1f);
            _lostCheckers++;
        }

        public Vector3 GetPosition() {
            return _checkersContainer.AsRectTransform().position;
        }
    }
}