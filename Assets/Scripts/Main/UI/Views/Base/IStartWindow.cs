using Core.MVP.Base.Interfaces;
using EnhancedUI.EnhancedScroller;

namespace Main.UI.Views.Base {
    public interface IStartWindow : IWindowView {
        void SetScrollerDelegate(IEnhancedScrollerDelegate deleg);
        void ReloadData();
    }
}