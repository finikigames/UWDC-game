using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using Global.ConfigTemplate;
using Global.Context;
using Global.Enums;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Nakama;
using Server.Services;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class MatchWindowPresenter : BaseWindowPresenter<IMatchWindow, MatchWindowData>,
                                        IUpdatable {
        private NakamaService _nakamaService;
        private AppConfig _appConfig;
        private IUpdateService _updateService;
        
        private bool _needNicknameInitialize;

        public MatchWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            _updateService.RegisterUpdate(this);
            _nakamaService.SubscribeToMatchPresence(OnMatchPresence);

            View.SubscribeToHowToPlayButton(OnHowToPlayClick);
            View.SubscribeToFleeButton(OnFleeClick);
            
            View.ProvideCamera(UnityEngine.Camera.main);
        }

        private void OnMatchPresence(IMatchPresenceEvent obj) {
            if (_nakamaService.GetCurrentMatchPlayersCount() != 2) return;

            _needNicknameInitialize = true;
        }

        private void OnFleeClick() {
            FireSignal(new OpenWindowSignal(WindowKey.FleeWindow, new FleeWindowData()));
        }

        private void OnHowToPlayClick() {
            FireSignal(new OpenWindowSignal(WindowKey.RulesWindow, new RulesWindowData()));
        }

        public void CustomUpdate() {
            if (!_needNicknameInitialize) return;

            _needNicknameInitialize = false;
            
            var opponent = _nakamaService.GetOpponent();
            var me = _nakamaService.GetMe();

            if (_appConfig.PawnColor == (int) PawnColor.Black) {
                View.SetOpponentName(me.User.DisplayName);
                View.SetYourName(opponent.Username);
            }
            else {
                View.SetYourName(me.User.DisplayName);
                View.SetOpponentName(opponent.Username);
            }
        }

        public override void Dispose() {
            _updateService.UnregisterUpdate(this);
        }
    }
}