using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Cysharp.Threading.Tasks;
using Global.Context;
using Global.Window.Base;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class WinWindowPresenter : BaseWindowPresenter<IWinWindow, WinWindowData> {
        public WinWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
            View.SubscribeToClose(ToMain);
        }

        private void ToMain() {
            FireSignal(new ToMainSignal());
        }
    }
}