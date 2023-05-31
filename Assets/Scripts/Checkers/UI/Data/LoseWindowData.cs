using Core.MVP.Base.Interfaces;
using Global.Window.Enums;

namespace Checkers.UI.Data {
    public class LoseWindowData : IWindowData {
        public WinLoseReason Reason;
    }
}