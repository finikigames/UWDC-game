using System;
using Global.StateMachine.Base.Enums;
using Unity.Collections;
using Zenject;

namespace Global.Services.Context.Base {
    public interface IContextService {
        void SubscribeOnRegister(Action<GameContext> onRegister);
        void SubscribeOnRegister(Action<GameContext> onRegister, GameContext gameContext);
        void SubscribeOnRegister(Action<DiContainer> onRegister, GameContext gameContext);
        void UnsubscribeOnRegister(Action<DiContainer> onRegister);
        void UnsubscribeOnRegister(Action<GameContext> onRegister);
        Action<GameContext> OnContextUnRegister { get; set; }
        bool IsContextAvailable(GameContext context);
        DiContainer ResolveContainer(GameContext context);
        void Register(GameContext context, DiContainer diContainer);
        void UnRegister(GameContext context);
    }
}