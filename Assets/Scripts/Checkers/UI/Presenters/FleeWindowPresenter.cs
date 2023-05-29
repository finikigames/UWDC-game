using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Cysharp.Threading.Tasks;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Server.Services;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class FleeWindowPresenter : BaseWindowPresenter<IFleeWindow, FleeWindowData> {
        private NakamaService _nakamaService;

        public FleeWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            View.SubscribeToReturnButton(() => {
                CloseThisWindow();
                View.Hide(null);
            });
            View.SubscribeToFleeButton(() => {
                FireSignal(new CloseWindowSignal(WindowKey.FleeWindow));
                FireSignal(new ToMainSignal());
            });
        }
    }
}