using System;
using System.Collections.Generic;
using Core.MVP.Initializer.Interfaces;

namespace Core.MVP.Initializer {
    public abstract class BaseInitializeService : IInitializeService {
        private readonly List<IInitializeUnit> _initializables;
        private readonly List<IDisposableUnit> _disposableUnits;

        public Action OnInitialize;
        public Action OnDispose;
        
        protected BaseInitializeService() {
            _initializables = new List<IInitializeUnit>();
            _disposableUnits = new List<IDisposableUnit>();
        }

        public void SubscribeToInitialize(IInitializeUnit initializeUnit) {
            _initializables.Add(initializeUnit);
        }

        public void SubscribeToDispose(IDisposableUnit disposeUnit) {
            _disposableUnits.Add(disposeUnit);
        }

        public void InitializeUnit() {
            foreach (var initializable in _initializables) {
                initializable.InitializeUnit();
            }
            
            OnInitialize?.Invoke();
        }

        public void DisposeUnit() {
            foreach (var disposable in _disposableUnits) {
                disposable.DisposeUnit();                
            }
            
            OnDispose?.Invoke();
        }
    }
}