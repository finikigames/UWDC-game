using System;
using Core.MVP.Base.Interfaces;
using EnhancedUI.EnhancedScroller;

namespace Main.UI.Views.Base {
    public interface IStartWindow : IWindowView {
        string SearchingPlayer { get; }
        void Init();
        void OnTextChange(Action callback);
        void OnStartClick(Action callback);
        void ClearScroller();
        void SetScrollerDelegate(IEnhancedScrollerDelegate deleg);
        void ReloadData();
        void SetAllMembersCount(int count);
        void SetOnlineMembersCount(int count);
    }
}