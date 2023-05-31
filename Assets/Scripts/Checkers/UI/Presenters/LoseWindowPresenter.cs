using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Core.Extensions;
using Cysharp.Threading.Tasks;
using Global;
using Global.ConfigTemplate;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Server.Services;
using UnityEngine.Scripting;
using Zenject;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class LoseWindowPresenter : BaseWindowPresenter<ILoseWindow, LoseWindowData> {
        private SignalBus _signalBus;
        private NakamaService _nakamaService;
        private AppConfig _appConfig;

        public LoseWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _signalBus = Resolve<SignalBus>(GameContext.Checkers);
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            View.SubscribeToContinue(ToMain);
            
            var reasonText = GetLoseReasonText(WindowData.Reason);
            View.SetReasonText(reasonText);

            var isMatchmaking = PlayerPrefsX.GetBool("Matchmaking");

            if (isMatchmaking) {
                var opponentId = _appConfig.OpponentUserId;
                
                var list = await _nakamaService.ListStorageObjects<PlayerResults>("players", "loses");

                foreach (var element in list.Data) {
                    if (element == opponentId) return;
                }
                list.Data.Add(opponentId);

                await _nakamaService.WriteStorageObject("players", "loses", list);
            }
        }
        
        private void ToMain() {
            _signalBus.Fire(new CloseWindowSignal(WindowKey.LoseWindow));
            _signalBus.Fire(new ToMainSignal());
        }
        
        private string GetLoseReasonText(WinLoseReason reason) {
            return reason switch {
                WinLoseReason.Concide => "Вы решили, что противник сильнее и сдались",
                WinLoseReason.Rule => "Вы оказались чуть неудачливей, но это не повод сдаваться",
                WinLoseReason.Timeout => "Вы немного задумались, над чем же?",
                _ => ""
            };
        }
    }
}