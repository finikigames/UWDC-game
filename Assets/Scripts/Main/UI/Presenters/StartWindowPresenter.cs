using Global.Services.Context;
using Global.Window.Base;
using Main.UI.Data;
using Main.UI.Views.Base;
using UnityEngine.Scripting;

namespace Main.UI.Presenters {
    [Preserve]
    public class StartWindowPresenter : BaseWindowPresenter<IStartWindow, StartWindowData> {
        public StartWindowPresenter(ContextService service) : base(service) {
            
        }
    }
}