using System;
using System.Collections.Generic;
using Core.Utils;

namespace Core.Collections
{
    public class DictionaryEnumNonBoxing<T, T1> where T : Enum
    {
        private readonly Dictionary<int, T1> _dictionary;
        
        public Dictionary<int, T1> Dictionary => _dictionary;

        public DictionaryEnumNonBoxing()
        {
            _dictionary = new Dictionary<int, T1>();
        }
        /* Example
            foreach (var element in _dictionary.Dictionary)
            {
                var key = _boxing.ConvertToKey(element.Key);
            }
         */

        public DictionaryEnumNonBoxing(int capacity)
        {
            _dictionary = new Dictionary<int, T1>(capacity);
        }

        public void Add(T key, T1 value)
        {
            var intKey = ConvertKey(key);
            
            _dictionary.Add(intKey, value);
        }

        public void Remove(T key)
        {
            var intKey = ConvertKey(key);

            _dictionary.Remove(intKey);
        }
        
        /// <summary>
        /// Use this when you work with dictionary keys outside this class object
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int ConvertKey(T key)
        {
            return CastTo<int>.From(key);
        }

        public T ConvertToKey(int key)
        {
            return CastTo<T>.From(key);
        }
        
        public T1 this[T i]
        {
            get => _dictionary[ConvertKey(i)];
            set => _dictionary[ConvertKey(i)] = value;
        }
    }
}