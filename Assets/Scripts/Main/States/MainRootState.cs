using Global.StateMachine.Base.Enums;
using Global.StateMachine.States;
using Global.StateMachine.States.Base;
using Source.Scripts.Core.StateMachine;

namespace Main.States {
    public class MainRootState : BaseGameState<MainRootState> {
        private readonly StateProvider _stateProvider;

        public MainRootState(StateProvider stateProvider) {
            _stateProvider = stateProvider;
            
            InitializeState();
        }
        
        public override GameContext Context() => GameContext.Main;

        public override void RegisterState(StateMachine<GameContext, Trigger> stateMachine) {
            stateMachine
                .RegisterState(GameContext.Main, this);
        }
        
        private void InitializeState() {
            _stateProvider.InitializeNewState(this, GameContext.Main);
        }
    }
}