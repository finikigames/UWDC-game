using EnhancedUI.EnhancedScroller;
using Global.VisibilityMechanisms;
using Global.Window.Base;
using Main.UI.Views.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI.Views.Implementations {
    public class StartWindow : BaseWindow, 
                               IStartWindow {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private EnhancedScroller _scroller;
        [SerializeField] private Button _startButton;
        [SerializeField] private TextMeshProUGUI _allMembersCount;
        [SerializeField] private TextMeshProUGUI _onlineMembersCount;
        [SerializeField] private TextMeshProUGUI _winsCountText;
        [SerializeField] private TextMeshProUGUI _looseCountText;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private GameObject _usersRoot;
        [SerializeField] private GameObject _faqRoot;
        [SerializeField] private Button _faqButton;
        [SerializeField] private Button _usersButton;
        [SerializeField] private GameObject _faqButtonOutline;
        [SerializeField] private GameObject _usersButtonOutline;
        [SerializeField] private TMP_InputField _searchInputField;

        protected override void OnEnable() {
            _showState = Core.MVP.Base.Enums.ShowState.Hidden;
             
            ChangeShowMechanism(new ChainShowMechanism(
                new FadeShowMechanism(_group),
                new CustomShowMechanism(ShowBlack)));
            ChangeHideMechanism(new ChainHideMechanism(
                new FadeHideMechanism(_group),
                new CustomHideMechanism(HideBlack)));
        }

        public void SetScrollerDelegate(IEnhancedScrollerDelegate deleg) {
            _scroller.Delegate = deleg;
        }

        public void ReloadData() {
            _scroller.ReloadData();
        }
    }
}