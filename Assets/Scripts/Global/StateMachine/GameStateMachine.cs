using System;
using System.Threading.Tasks;
using Global.StateMachine.Base.Enums;
using Global.StateMachine.States;
using Source.Scripts.Core.StateMachine;
using Source.Scripts.Core.StateMachine.States.Base;

namespace Global.StateMachine {
    public class GameStateMachine : IDisposable,
                                    ISubStateHolder<GameContext, Trigger> {
        private readonly StateProvider _stateProvider;
        private StateMachine<GameContext, Trigger> _stateMachine;

        public GameStateMachine(StateProvider stateProvider) {
            _stateProvider = stateProvider;
            InitializeStates();
        }

        public void Initialize() {
        }
        
        public void InitializeStates() {
            _stateMachine = new StateMachine<GameContext, Trigger>(GameContext.Preloader);

            _stateProvider.SubscribeToRegisterState(RegisterState);
        }

        public void RegisterSubState<TStateConcrete>(GameContext gameContext, 
                                                     GameContext subGameContext,
                                                     TStateConcrete stateConcrete) 
            where TStateConcrete : BaseState<GameContext, Trigger> {
            _stateMachine.RegisterSubStateFor(gameContext, subGameContext, stateConcrete);
        }

        public void UnRegisterSubState(GameContext state, GameContext subGameContext) {
            _stateMachine.UnRegisterSubStateFor(state, subGameContext);
        }

        public async Task Fire(Trigger trigger) {
            await _stateMachine.Fire(trigger);
        }

        public async Task ForceExit() {
            await _stateMachine.ForceExit();
        }

        private void RegisterState(GameContext gameContext) {
            if (_stateProvider.IsRegisteredState(gameContext)) _stateMachine.UnregisterState(gameContext);

            var stateObject = _stateProvider.GetInitializedState(gameContext);

            stateObject.RegisterState(_stateMachine);
        }
        
        public void Dispose() {
            ForceExit();
        }
    }
}