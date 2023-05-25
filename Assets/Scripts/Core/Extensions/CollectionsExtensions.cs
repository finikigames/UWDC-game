using System.Collections.Generic;

namespace Core.Extensions {
    public static class CollectionsExtensions { 
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : new() {
            TValue value;
            
            if (dict.ContainsKey(key)) {
                value = dict[key];
            }
            else {
                value = new();
                
                dict.Add(key, value);
            }

            return value;
        }

        public static bool EnsureIndexBounds<T>(this List<T> list, int index) {
            return index >= 0 && index < list.Count;
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) {
            if (dictionary.ContainsKey(key)) {
                dictionary.Remove(key);

                return true;
            }

            return false;
        }

        public static bool TryRemove<TKey>(this HashSet<TKey> hashSet, TKey key) {
            if (hashSet.Contains(key)) {
                hashSet.Remove(key);

                return true;
            }

            return false;
        }
    }
}