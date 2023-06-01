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
using Server;
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
        private MessageService _messageService;

        public InviteWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _signalBus = Resolve<SignalBus>(GameContext.Main);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
            _globalScope = Resolve<GlobalScope>(GameContext.Project);
            _messageService = Resolve<MessageService>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            var data = WindowData.InviteData;
            
            View.SubscribeToApply(async () => {
                var senderUserId = data.UserId;
                await _nakamaService.CreateMatch(data.MatchId);
                await _nakamaService.RemoveAllParties();
                await _messageService.SendUserConfirmation(data.MatchId, senderUserId);
                _globalScope.ApproveSenderId = senderUserId;
                _signalBus.Fire(new CloseWindowSignal(WindowKey.InviteWindow));
                PlayerPrefsX.SetBool("Matchmaking", false);
                
                _appConfig.PawnColor = PawnColor.Black;
                _appConfig.OpponentDisplayName = data.DisplayName;
                
                _signalBus.Fire(new ToCheckersMetaSignal{WithPlayer = true});
            });
            
            View.SubscribeToDecline(async () => {
                await _messageService.SendDeclineInviteSended(data.UserId);
                
                CloseThisWindow();
            });
            
            View.ChangeName(data.DisplayName);
        }
    }
}