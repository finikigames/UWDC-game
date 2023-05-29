using Global.Context;
using Global.Window.Base;
using Main.UI.Data;
using Main.UI.Views.Base;
using UnityEngine.Scripting;

namespace Main.UI.Presenters {
    [Preserve]
    public class InviteWindowPresenter : BaseWindowPresenter<IInviteWindow, InviteWindowData> {
        protected InviteWindowPresenter(ContextService service) : base(service) {
        }
    }
}