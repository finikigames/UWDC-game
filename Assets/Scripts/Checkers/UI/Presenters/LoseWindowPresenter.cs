using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Cysharp.Threading.Tasks;
using Global.Context;
using Global.Window.Base;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class LoseWindowPresenter : BaseWindowPresenter<ILoseWindow, LoseWindowData> {
        public LoseWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
        }
    }
}