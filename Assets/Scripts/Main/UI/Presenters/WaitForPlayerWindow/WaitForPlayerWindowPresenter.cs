using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Main.UI.Data.WaitForPlayerWindow;
using Main.UI.Views.Base.WaitForPlayerWindow;
using Nakama;
using Server.Services;
using UnityEngine;
using UnityEngine.Scripting;

namespace Main.UI.Presenters.WaitForPlayerWindow {
    [Preserve]
    public class WaitForPlayerWindowPresenter : BaseWindowPresenter<IWaitForPlayerWindow, WaitForPlayerWindowData>,
                                                IUpdatable {
        private readonly NakamaService _nakamaService;
        private IMatchmakerTicket _matchmakerTicket;

        public WaitForPlayerWindowPresenter(ContextService service) : base(service) {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            _matchmakerTicket = await _nakamaService.AddMatchmaker();
            
            _nakamaService.SubscribeToMatchmakerMatched(OnMatchmakerMatched);
        }

        public void CustomUpdate() {
            
        }

        private void OnMatchmakerMatched(IMatchmakerMatched matched) {
            Debug.Log(matched.Users);
        }
        
        public override async UniTask Dispose() {
            await _nakamaService.RemoveMatchmaker(_matchmakerTicket);
            _nakamaService.UnsubscribeMatchmakerMatched(OnMatchmakerMatched);
        }
    }
}