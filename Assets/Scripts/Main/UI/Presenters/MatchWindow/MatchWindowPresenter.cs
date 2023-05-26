using Cysharp.Threading.Tasks;
using Global.Context;
using Global.Window.Base;
using Main.UI.Data.MatchWindow;
using Main.UI.Views.Base.MatchWindow;
using UnityEngine.Scripting;

namespace Main.UI.Presenters.MatchWindow {
    [Preserve]
    public class MatchWindowPresenter : BaseWindowPresenter<IMatchWindow, MatchWindowData> {
        public MatchWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
        }
    }
}