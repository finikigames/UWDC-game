using System;
using Core.MVP.Base.Interfaces;
using EnhancedUI.EnhancedScroller;

namespace Checkers.UI.Views.Base {
    public interface IMatchesListView : IWindowView {
        Action OnMatchCreate { get; set; }
        Action OnMatchJoin { get; set; }
        Action OnMatchesRefresh { get; set; }
        void Initialize();
        void SetScrollerDelegate(IEnhancedScrollerDelegate scrollerDelegate);
        void ApplyData();
    }
}