using Core.MVP.Base.Interfaces;
using EnhancedUI.EnhancedScroller;

namespace Main.UI.Views.Base.LeaderboardWindow {
    public interface ILeaderboardWindow : IWindowView {
        int EndDataIndex { get; }
        int StartDataIndex { get; }
        void SetScrollerDelegate(IEnhancedScrollerDelegate deleg);
        void ReloadData();
        void JumpToIndex(int meIndex);
    }
}