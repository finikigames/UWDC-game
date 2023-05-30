using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.UI.Views {
    public class PlayerChekersBar : MonoBehaviour {
        [SerializeField] private Transform _checkersContainer;
        private int _lostCheckers = 0;

        public void DecreaseСhecker() {
            var childIndex = _checkersContainer.childCount - _lostCheckers - 1;
            var checker = _checkersContainer.GetChild(childIndex);
            checker.gameObject.GetComponent<Image>().DOFade(0, 0.5f);
            _lostCheckers++;
        }
    }
}