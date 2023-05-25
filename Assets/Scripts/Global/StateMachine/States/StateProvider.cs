using System;
using System.Collections.Generic;
using Global.StateMachine.Base.Enums;
using Source.Scripts.Core.StateMachine.States.Base;

namespace Global.StateMachine.States {
    public class StateProvider {
        private readonly HashSet<GameContext> _registeredStates;
        private readonly Dictionary<GameContext, BaseState<GameContext, Trigger>> _stateImplementations;
        private Action<GameContext> OnChangedState;

        public StateProvider() {
            _stateImplementations = new Dictionary<GameContext, BaseState<GameContext, Trigger>>();
            _registeredStates = new HashSet<GameContext>();
        }

        public void SubscribeToRegisterState(Action<GameContext> callback) {
            OnChangedState += callback;
        }

        public void InitializeNewState(BaseState<GameContext, Trigger> stateObject, GameContext gameContext) {
            if (IsRegisteredState(gameContext)) {
                _stateImplementations.Remove(gameContext);
                _stateImplementations.Add(gameContext, stateObject);
                OnChangedState?.Invoke(gameContext);
                return;
            }

            _stateImplementations.Add(gameContext, stateObject);
            OnChangedState?.Invoke(gameContext);
        }

        public BaseState<GameContext, Trigger> GetInitializedState(GameContext gameContext) {
            _registeredStates.Add(gameContext);
            return _stateImplementations[gameContext];
        }

        public bool IsRegisteredState(GameContext gameContext) {
            return _registeredStates.Contains(gameContext);
        }
    }
}