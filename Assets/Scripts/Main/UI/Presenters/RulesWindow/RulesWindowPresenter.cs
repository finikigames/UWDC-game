using Cysharp.Threading.Tasks;
using Global.Context;
using Global.Window.Base;
using Main.UI.Data.RulesWindow;
using Main.UI.Views.Base.RulesWindow;
using UnityEngine.Scripting;

namespace Main.UI.Presenters.RulesWindow {
    [Preserve]
    public class RulesWindowPresenter : BaseWindowPresenter<IRulesWindow, RulesWindowData> {
        public RulesWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
        }
    }
}