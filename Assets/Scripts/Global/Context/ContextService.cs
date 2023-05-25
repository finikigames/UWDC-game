using System;
using System.Collections.Generic;
using Global.Services.Context.Base;
using Global.StateMachine.Base.Enums;
using Unity.Collections;
using Zenject;

namespace Global.Services.Context {
    public class ContextService : IContextService,
                                  IDisposable {
        private HashSet<GameContext> _contexts = new ();
        private Dictionary<GameContext, DiContainer> _containers = new ();
        
        private Action<GameContext> OnContextRegister { get; set; }
        private Action<DiContainer> OnContainerRegister { get; set; }
        public Action<GameContext> OnContextUnRegister { get; set; }

        public bool IsContextAvailable(GameContext context)
            => _contexts.Contains(context);

        public void SubscribeOnRegister(Action<GameContext> onRegister) {
            OnContextRegister += onRegister;
        }
        
        public void SubscribeOnRegister(Action<GameContext> onRegister, GameContext gameContext) {
            OnContextRegister += onRegister;

            if (IsContextAvailable(gameContext)) {
                onRegister?.Invoke(gameContext);
            }
        }
        
        public void SubscribeOnRegister(Action<DiContainer> onRegister, GameContext gameContext) {
            OnContainerRegister += onRegister;
            
            if (_containers.ContainsKey(gameContext)) {
                var container = _containers[gameContext];
                onRegister?.Invoke(container);
            }
        }
        
        public void UnsubscribeOnRegister(Action<DiContainer> onRegister) {
            OnContainerRegister -= onRegister;
        }

        public void UnsubscribeOnRegister(Action<GameContext> onRegister) {
            OnContextRegister -= onRegister;
        }

        public DiContainer ResolveContainer(GameContext context)
            => _containers[context];

        public void Register(GameContext context, DiContainer diContainer) {
            _contexts.Add(context);
            _containers.Add(context, diContainer);
            
            OnContextRegister?.Invoke(context);
            OnContainerRegister?.Invoke(diContainer);
        }

        public void UnRegister(GameContext context) {
            _contexts.Remove(context);
            _containers.Remove(context);
            
            OnContextUnRegister?.Invoke(context);
        }

        public void Dispose() {
            foreach (var context in _contexts) {
                _containers.Remove(context);
            
                OnContextUnRegister?.Invoke(context);
            }
            
            _contexts.Clear();
        }
    }
}