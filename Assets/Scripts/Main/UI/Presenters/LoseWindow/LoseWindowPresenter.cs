using Cysharp.Threading.Tasks;
using Global.Context;
using Global.Window.Base;
using Main.UI.Data.LoseWindow;
using Main.UI.Views.Base.LoseWindow;
using UnityEngine.Scripting;

namespace Main.UI.Presenters.LoseWindow {
    [Preserve]
    public class LoseWindowPresenter : BaseWindowPresenter<ILoseWindow, LoseWindowData> {
        public LoseWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
        }
    }
}