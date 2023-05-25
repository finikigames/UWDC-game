using System;

namespace Core.Utils
{
    public class SingleInvokeDelegate<T, U>
        where T : class, Delegate
    {
        private T _methodDelegate;
        private U _obj;

        private T _internalDelegate;

        private Action<U, T> _getter;
        private Action<U, T> _setter;
        
        public SingleInvokeDelegate<T, U> Init(Action<U, T> getter, Action<U, T> setter)
        {
            _getter = getter;
            _setter = setter;
            return this;
        }

        public SingleInvokeDelegate<T, U> Subscribe(T methodDelegate)
        {
            _methodDelegate = methodDelegate;
            _setter(_obj, _methodDelegate);
            return this;
        }

        public void Unsubscribe()
        {
            var newDelegate = (T)Delegate.Remove(_internalDelegate, _methodDelegate);

            _setter(_obj, newDelegate);
        }
    }
}