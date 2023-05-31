using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Cysharp.Threading.Tasks;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using UnityEngine.Scripting;
using Zenject;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class WinWindowPresenter : BaseWindowPresenter<IWinWindow, WinWindowData> {
        private SignalBus _signalBus;

        public WinWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _signalBus = Resolve<SignalBus>(GameContext.Checkers);
        }

        protected override async UniTask LoadContent() {
            View.SubscribeToContinue(ToMain);
            
            var reasonText = GetWinReasonText(WindowData.Reason);
            View.SetReasonText(reasonText);
        }

        private void ToMain() {
            _signalBus.Fire(new CloseWindowSignal(WindowKey.WinWindow));
            _signalBus.Fire(new ToMainSignal());
        }
        
        private string GetWinReasonText(WinLoseReason reason) {
            return reason switch {
                WinLoseReason.Concide => "Ваш противник решил, что вы сильнее и сдался",
                WinLoseReason.Rule => "Ваш противник уступил вам в ваших навыках",
                WinLoseReason.Timeout => "Ваш противник задумался и решил отдохнуть",
                _ => ""
            };
        }
    }
}