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
    public class WinWindowPresenter : BaseWindowPresenter<IWinWindow, WinWindowData> {
        private SignalBus _signalBus;
        private NakamaService _nakamaService;
        private AppConfig _appConfig;
        
        private string _tournamentId = "4ec4f126-3f9d-11e7-84ef-b7c182b36521";

        public WinWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _signalBus = Resolve<SignalBus>(GameContext.Checkers);
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            View.SubscribeToContinue(ToMain);
            
            var isMatchmaking = PlayerPrefsX.GetBool("Matchmaking");

            if (isMatchmaking) {
                var opponentId = _appConfig.OpponentUserId;
                
                var list = await _nakamaService.ListStorageObjects<PlayerResults>("players", "wins");

                foreach (var element in list.Data) {
                    if (element == opponentId) return;
                }
                list.Data.Add(opponentId);

                await _nakamaService.WriteStorageObject("players", "wins", list);
                
                await _nakamaService.SubmitTournamentScore(_tournamentId, null, list.Data.Count, 0);
            }
        }

        private void ToMain() {
            _signalBus.Fire(new CloseWindowSignal(WindowKey.WinWindow));
            _signalBus.Fire(new ToMainSignal());
        }
    }
}