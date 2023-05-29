using Cysharp.Threading.Tasks;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Main.UI.Data;
using Main.UI.Views.Base;
using Server.Services;
using UnityEngine.Scripting;
using Zenject;

namespace Main.UI.Presenters {
    [Preserve]
    public class InviteWindowPresenter : BaseWindowPresenter<IInviteWindow, InviteWindowData> {
        private NakamaService _nakamaService;
        private SignalBus _signalBus;

        public InviteWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _signalBus = Resolve<SignalBus>(GameContext.Main);
        }

        protected override async UniTask LoadContent() {
            var data = WindowData.DisplayName;
            
            View.SubscribeToApply(async () => {
                var userId = _nakamaService.GetMe().User.Id;
                await _nakamaService.SendUserConfirmation(WindowData.PartyId, userId);
                _signalBus.Fire(new CloseWindowSignal(WindowKey.InviteWindow));
                _signalBus.Fire(new CloseWindowSignal(WindowKey.StartWindow));
                _signalBus.Fire(new ToCheckersMetaSignal{WithPlayer = true});
            });
            View.ChangeName(data);
        }
    }
}