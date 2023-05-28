using Cysharp.Threading.Tasks;
using Global.Context;
using Global.Window.Base;
using Main.UI.Data.WinWindow;
using Main.UI.Views.Base.WinWindow;
using UnityEngine.Scripting;

namespace Main.UI.Presenters.WinWindow {
    [Preserve]
    public class WinWindowPresenter : BaseWindowPresenter<IWinWindow, WinWindowData> {
        public WinWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
        }
    }
}