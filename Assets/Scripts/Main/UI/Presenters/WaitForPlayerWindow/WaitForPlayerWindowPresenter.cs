﻿using Core.Ticks.Interfaces;
using Cysharp.Threading.Tasks;
using Global.ConfigTemplate;
using Global.Context;
using Global.StateMachine.Base.Enums;
using Global.Window.Base;
using Main.UI.Data;
using Main.UI.Data.WaitForPlayerWindow;
using Main.UI.Views.Base.WaitForPlayerWindow;
using Nakama;
using Server.Services;
using UnityEngine.Scripting;
using Zenject;

namespace Main.UI.Presenters.WaitForPlayerWindow {
    [Preserve]
    public class WaitForPlayerWindowPresenter : BaseWindowPresenter<IWaitForPlayerWindow, WaitForPlayerWindowData>,
                                                IUpdatable {
        private NakamaService _nakamaService;
        private IUpdateService _updateService;
        private SignalBus _signalBus;
        private AppConfig _appConfig;

        private bool _needLoad;

        private IMatchmakerTicket _matchmakerTicket;
        private IMatchmakerMatched _matched;

        public WaitForPlayerWindowPresenter(ContextService service) : base(service) {
        }

        public override void InitDependencies() {
            _nakamaService = Resolve<NakamaService>(GameContext.Project);
            _updateService = Resolve<IUpdateService>(GameContext.Project);
            _signalBus = Resolve<SignalBus>(GameContext.Main);
            _appConfig = Resolve<AppConfig>(GameContext.Project);
        }

        protected override async UniTask LoadContent() {
            _matchmakerTicket = await _nakamaService.AddMatchmaker();
            
            _updateService.RegisterUpdate(this);
            _nakamaService.SubscribeToMatchmakerMatched(OnMatchmakerMatched);
        }

        public void CustomUpdate() {
            if (!_needLoad) return;
            _needLoad = false;

            StartLoad();
        }

        private async UniTask StartLoad() {
            var matchId = _matched.MatchId;
            await _nakamaService.CreateMatch(matchId);
            CloseThisWindow();
            _signalBus.Fire(new ToCheckersMetaSignal{WithPlayer = true});
        }

        private void OnMatchmakerMatched(IMatchmakerMatched matched) {
            _needLoad = true;
            _matched = matched;
            _matchmakerTicket = null;
        }
        
        public override async UniTask Dispose() {
            _updateService.UnregisterUpdate(this);
            if (_matchmakerTicket != null) {
                await _nakamaService.RemoveMatchmaker(_matchmakerTicket);
            }

            _nakamaService.UnsubscribeMatchmakerMatched(OnMatchmakerMatched);
        }
    }
}