using Core.MVP.Base.Interfaces;
using EnhancedUI.EnhancedScroller;

namespace Main.UI.Views.Base {
    public interface IStartWindow : IWindowView {
        void Init();
        void SetScrollerDelegate(IEnhancedScrollerDelegate deleg);
        void ReloadData();
        void SetAllMembersCount(int count);
        void SetOnlineMembersCount(int count);
    }
}