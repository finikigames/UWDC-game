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
    public class FleeWindowPresenter : BaseWindowPresenter<IFleeWindow, FleeWindowData> {
        public FleeWindowPresenter(ContextService service) : base(service) {
        }
        
        protected override async UniTask LoadContent() {
            View.SubscribeToReturnButton(() => {
                CloseThisWindow();
                View.Hide(null);
            });
            View.SubscribeToFleeButton(() => {
                FireSignal(new CloseWindowSignal(WindowKey.FleeWindow));
                FireSignal(new CloseWindowSignal(WindowKey.MatchWindow));
                FireSignal(new ToMainSignal());
            });
        }
    }
}