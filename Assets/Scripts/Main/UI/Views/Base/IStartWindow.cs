using Core.MVP.Base.Interfaces;
using EnhancedUI.EnhancedScroller;

namespace Main.UI.Views.Base {
    public interface IStartWindow : IWindowView {
        string SearchingPlayer { get; }
        void SetScrollerDelegate(IEnhancedScrollerDelegate deleg);
        void ReloadData();
    }
}