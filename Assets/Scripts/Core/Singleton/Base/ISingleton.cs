namespace Core.Singleton.Base {
    public interface ISingleton<T> {
        static T Instance { get; set; }
    }
}