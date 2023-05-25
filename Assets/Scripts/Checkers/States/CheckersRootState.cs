using System.Threading.Tasks;
using Checkers.Services;
using Global.StateMachine.Base.Enums;
using Global.StateMachine.States;
using Global.StateMachine.States.Base;
using Server.Services;
using Source.Scripts.Core.StateMachine;
using Source.Scripts.Core.StateMachine.States.Base;
using UnityEngine;

namespace Checkers.States {
    public class CheckersRootState : BaseGameState<CheckersRootState>,
                                     IEntryState,
                                     IExitState {
        private readonly NakamaService _nakamaService;
        private readonly MainCheckersOnlineService _mainCheckersOnlineService;

        public override GameContext Context() => GameContext.Checkers;

        public CheckersRootState(StateProvider stateProvider,
                                 NakamaService nakamaService,
                                 MainCheckersOnlineService mainCheckersOnlineService) {
            _nakamaService = nakamaService;
            _mainCheckersOnlineService = mainCheckersOnlineService;

            stateProvider.InitializeNewState(this, GameContext.Checkers);
        }

        public override void RegisterState(StateMachine<GameContext, Trigger> stateMachine) {
            stateMachine
                .RegisterState(GameContext.Checkers, this)
                .SetupInternals(state => {
                    state.InternalTransition(Trigger.AfterSceneLoadTrigger, AfterSceneLoad);
                });
        }

        public async Task OnEntry() {
            var matchId =  PlayerPrefs.GetString("SelectedMatchId");

            if (string.IsNullOrEmpty(matchId)) return;
            await _nakamaService.JoinMatch(matchId);
        }

        public async Task AfterSceneLoad() {
            _mainCheckersOnlineService.Initialize();
        }

        public async Task OnExit() {
        }
    }
}