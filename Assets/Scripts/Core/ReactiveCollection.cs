using System;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class ReactiveCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _list;

        public delegate void OnValueChange(T newValue, T oldValue);
        public delegate void OnValueAdd(T value);
        public delegate void OnValueRemove(T value);

        private OnValueChange _onValueChange;
        private OnValueAdd _onValueAdd;
        private OnValueRemove _onValueRemove;

        public int Count => _list.Count;

        public List<T> List => _list;

        public ReactiveCollection() => 
            _list = new List<T>();

        public ReactiveCollection(int size) => 
            _list = new List<T>(size);

        public void SubscribeValueChange(OnValueChange callback) => _onValueChange += callback;

        public void SubscribeOnRemove(OnValueRemove callback) => _onValueRemove += callback;

        public void SubscribeOnAdd(OnValueAdd callback) => _onValueAdd += callback;
        
        public void UnsubscribeValueChange(OnValueChange callback) => _onValueChange -= callback;

        public void UnsubscribeOnRemove(OnValueRemove callback) => _onValueRemove -= callback;

        public void UnsubscribeOnAdd(OnValueAdd callback) => _onValueAdd -= callback;

        public void UnsubscribeValueChangeAll() => _onValueChange = null;

        public void UnsubscribeOnAddAll() => _onValueAdd = null;

        public void UnsubscribeOnValueRemoveAll() => _onValueRemove = null;

        public void UnsubscribeAll()
        {
            UnsubscribeValueChangeAll();
            UnsubscribeOnAddAll();
            UnsubscribeOnValueRemoveAll();
        }

        public void Remove(T value)
        {
            _list.Remove(value);
            _onValueRemove?.Invoke(value);
        }

        public void Add(T value)
        {
            _list.Add(value);
            _onValueAdd?.Invoke(value);
        }
        
        public T this[int index]
        {
            get => _list[index];
            set
            {
                var oldValue = _list[index];
                _list[index] = value;
                _onValueChange?.Invoke(value, oldValue);
            }
        }

        public void Clear() => _list.Clear();


        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int FindIndex(Predicate<T> value)
        {
            return _list.FindIndex(value);
        }
    }

}