using Cysharp.Threading.Tasks;
using Global.Context;
using Global.UI.Data;
using Global.UI.Views.Base;
using Global.Window.Base;
using UnityEngine.Scripting;

namespace Global.UI.Presenters {
    [Preserve]
    public class RulesWindowPresenter : BaseWindowPresenter<IRulesWindow, RulesWindowData> {
        public RulesWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
        }
    }
}