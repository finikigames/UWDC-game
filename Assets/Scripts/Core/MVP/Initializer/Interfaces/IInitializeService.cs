namespace Core.MVP.Initializer.Interfaces {
    public interface IInitializeService {
        void SubscribeToInitialize(IInitializeUnit initializeUnit);
        
        void SubscribeToDispose(IDisposableUnit disposeUnit);
    }
}