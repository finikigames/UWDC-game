using System.Collections.Generic;

namespace Core.Pools {
    public class Pool<T> where T : class, new() {
        private readonly Queue<T> _pool;
        private readonly List<T> _taken;

        public Pool(int capacity = 5) {
            _pool = new Queue<T>(capacity);
            _taken = new List<T>(capacity);
        }

        public T Get() {
            if (_pool.TryDequeue(out var result)) {
                _taken.Add(result);
                return result;
            }

            var newValue = new T();
            _taken.Add(newValue);
            return newValue;
        }

        public void Release(T value) {
            _taken.Remove(value);
            _pool.Enqueue(value);
        }
    }
}