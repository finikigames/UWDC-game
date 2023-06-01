using Checkers.UI.Views.Base;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using TMPro;
using UnityEngine;

namespace Checkers.UI.Views.Implementations {
    public class PauseWindow : BaseWindow,
                               IPauseWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _timerText;
        
        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }
        
        public void SetPauseTime(int time) {
            _timerText.text = time.ToString();
        }
    }
}