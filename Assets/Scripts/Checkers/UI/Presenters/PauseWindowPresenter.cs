using Checkers.Settings;
using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Cysharp.Threading.Tasks;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
using Global.Services.Timer;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Server;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class PauseWindowPresenter : BaseWindowPresenter<IPauseWindow, PauseWindowData> {
        private TimerService _timerService;
        private MainCheckerSceneSettings _sceneSettings;
        private AppConfig _appConfig;
        private MessageService _messageService;

        private const string PauseId = "PauseId";

        public PauseWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _timerService = Resolve<TimerService>(GameContext.Project);
            _sceneSettings = Resolve<MainCheckerSceneSettings>(GameContext.Checkers);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
            _messageService = Resolve<MessageService>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            _timerService.StartTimer(PauseId, _appConfig.PauseTime, PauseTimeOutForWaiting, false, View.SetPauseTime);
        }

        private void PauseTimeOutForWaiting() {
            if (_appConfig.GameEnded) return;
            var winner = _sceneSettings.TurnHandler.YourColor == PawnColor.Black ? PawnColor.White : PawnColor.Black;
            _sceneSettings.TurnHandler.EndGame(winner, WinLoseReason.Timeout);
            
            CloseThisWindow();
        }

        public override void Dispose() {
            _timerService.RemoveTimer(PauseId);
        }
    }
}