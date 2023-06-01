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
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class PauseWindowPresenter : BaseWindowPresenter<IPauseWindow, PauseWindowData> {
        private TimerService _timerService;
        private MainCheckerSceneSettings _sceneSettings;
        private AppConfig _appConfig;

        private const string PauseId = "PauseId";

        public PauseWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _timerService = Resolve<TimerService>(GameContext.Project);
            _sceneSettings = Resolve<MainCheckerSceneSettings>(GameContext.Checkers);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            _timerService.StartTimer(PauseId, _appConfig.PauseTime, PauseTimeOutForWaiting, false, View.SetPauseTime);
        }

        private void PauseTimeOutForWaiting() {
            var winner = _sceneSettings.TurnHandler.YourColor == PawnColor.Black ? PawnColor.White : PawnColor.Black;
            _sceneSettings.TurnHandler.OnEndGame?.Invoke(winner, WinLoseReason.Timeout);
            
            CloseThisWindow();
        }

        public override async UniTask Dispose() {
            _timerService.RemoveTimer(PauseId);
        }
    }
}