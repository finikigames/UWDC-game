namespace Core.MVP.Initializer.Interfaces {
    public interface IInitializeUnit {
        void InitializeUnit();

        void ProvideInitializeService(IInitializeService initializeService);
    }
}