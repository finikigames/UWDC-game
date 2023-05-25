using System;
using Global.Context;
using Global.Context.Base;
using Global.StateMachine.Base.Enums;
using Source.Scripts.Core.StateMachine.States.Base;
using Zenject;

namespace Global.StateMachine.States.Base {
    public abstract class BaseGameState<TState> : BaseState<GameContext, Trigger>,
                                                  IContext,
                                                  IInitializable,
                                                  IDisposable where TState : BaseGameState<TState> {
        private IContextService _contextService;
        private DiContainer _diContainer;
        private ISubStateHolder<GameContext, Trigger> _subStateHolder;

        [Inject]
        private void Construct(IContextService contextService,
                               DiContainer diContainer,
                               ISubStateHolder<GameContext, Trigger> subStateHolder) {
            _contextService = contextService;
            _diContainer = diContainer;
            _subStateHolder = subStateHolder;
            
            RegisterContext();
        }
        
        public abstract GameContext Context();

        public virtual void Initialize() {
            if (this is ISubState<GameContext> subState) {
                _subStateHolder.RegisterSubState(subState.RootState, Context(), this);
            }
        }

        public void RegisterContext() {
            _contextService.Register(Context(), _diContainer);
        }

        public void UnRegisterContext() {
            _contextService.UnRegister(Context());
        }

        public virtual void Dispose() {
            UnRegisterContext();
            
            if (this is ISubState<GameContext> subState) {
                _subStateHolder.UnRegisterSubState(subState.RootState, Context());
            }
        }
    }
}