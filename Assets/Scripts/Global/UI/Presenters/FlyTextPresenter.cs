using Cysharp.Threading.Tasks;
using Global.Context;
using Global.UI.Data;
using Global.UI.Views.Base;
using Global.Window.Base;
using UnityEngine.Scripting;

namespace Global.UI.Presenters {
    [Preserve]
    public class FlyTextPresenter : BaseWindowPresenter<IFlyTextView, FlyTextData> {
        
        public FlyTextPresenter(ContextService service) : base(service) { }

        protected override async UniTask LoadContent() {
            View.InitializeView();
            View.ShowFlyText(WindowData.FlyText);
            View.SetBackgroundColor(WindowData.PawnColor);
        }
    }
}