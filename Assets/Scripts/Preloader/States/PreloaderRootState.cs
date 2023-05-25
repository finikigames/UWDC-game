using Global.StateMachine.Base.Enums;
using Global.StateMachine.States;
using Global.StateMachine.States.Base;
using Source.Scripts.Core.StateMachine;

namespace Preloader.States {
    public class PreloaderRootState : BaseGameState<PreloaderRootState> {
        private readonly StateProvider _stateProvider;

        public PreloaderRootState(StateProvider stateProvider) {
            _stateProvider = stateProvider;
            
            InitializeState();
        }
        
        public override GameContext Context() => GameContext.Preloader;

        public override void RegisterState(StateMachine<GameContext, Trigger> stateMachine) {
            stateMachine
                .RegisterState(GameContext.Preloader, this)
                .Permit(Trigger.MainTrigger, GameContext.Main);
        }
        
        private void InitializeState() {
            _stateProvider.InitializeNewState(this, GameContext.Preloader);
        }
    }
}