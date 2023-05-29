using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Cysharp.Threading.Tasks;
using Global.Context;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class WinWindowPresenter : BaseWindowPresenter<IWinWindow, WinWindowData> {
        public WinWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
            View.SubscribeToContinue(ToMain);
        }

        private void ToMain() {
            FireSignal(new CloseWindowSignal(WindowKey.WinWindow));
            FireSignal(new ToMainSignal());
        }
    }
}