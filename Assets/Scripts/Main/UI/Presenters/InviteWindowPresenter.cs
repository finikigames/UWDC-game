using Core.Extensions;
using Cysharp.Threading.Tasks;
using Global;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
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
        private AppConfig _appConfig;
        private GlobalScope _globalScope;

        public InviteWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _signalBus = Resolve<SignalBus>(GameContext.Main);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
            _globalScope = Resolve<GlobalScope>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            var data = WindowData.DisplayName;
            
            View.SubscribeToApply(async () => {
                var senderUserId = WindowData.SenderId;
                await _nakamaService.CreateMatch(WindowData.PartyId);
                await _nakamaService.SendUserConfirmation(WindowData.PartyId, senderUserId);
                _signalBus.Fire(new CloseWindowSignal(WindowKey.InviteWindow));
                PlayerPrefsX.SetBool("Matchmaking", false);
                _signalBus.Fire(new ToCheckersMetaSignal{WithPlayer = true});
                
                _appConfig.PawnColor = (int)PawnColor.Black;
                _appConfig.OpponentDisplayName = WindowData.DisplayName;
            });
            
            View.SubscribeToDecline(async () => {
                if (_globalScope.ReceivedInvites.Count > 0) {
                    
                }
                else {
                    CloseThisWindow();
                }
            });
            
            View.ChangeName(data);
        }
    }
}