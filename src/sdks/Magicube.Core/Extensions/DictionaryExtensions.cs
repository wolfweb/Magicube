using System;
using System.Collections.Generic;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class DictionaryExtensions {
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) {
            if (!dict.ContainsKey(key)) {
                dict.Add(new KeyValuePair<TKey, TValue>(key, value));
            } else {
                dict[key] = value;
            }

            return dict[key];
        }
    
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory) {
            if (!dict.ContainsKey(key)) {
                dict.Add(new KeyValuePair<TKey, TValue>(key, addValue));
            } else {
                dict[key] = updateValueFactory(key, dict[key]);
            }

            return dict[key];
        }

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory) {
            if (!dict.ContainsKey(key)) {
                dict.Add(new KeyValuePair<TKey, TValue>(key, addValueFactory(key)));
            } else {
                dict[key] = updateValueFactory(key, dict[key]);
            }

            return dict[key];
        }


        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) {
            if (!dict.ContainsKey(key)) {
                dict.Add(new KeyValuePair<TKey, TValue>(key, value));
            }

            return dict[key];
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueFactory) {
            if (!dict.ContainsKey(key)) {
                dict.Add(new KeyValuePair<TKey, TValue>(key, valueFactory(key)));
            }

            return dict[key];
        }

        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value) {
            if (dictionary.TryGetValue(key, out var valueObj) && valueObj is T variable) {
                value = variable;
                return true;
            }

            value = default;
            return false;
        }
    }
}
