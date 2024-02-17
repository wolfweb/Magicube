using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Magicube.Core {
    [Serializable]
    public class TransferContext : IDictionary<string, object> {
        private readonly IDictionary<string, object> _map;

        public TransferContext(IEqualityComparer<string> comparer = null) {
            _map = new Dictionary<string, object>(comparer);
        }

        #region 
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            return _map.GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item) {
            _map.TryAdd(item.Key, item.Value);
        }

        public void Clear() {
            _map.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item) {
            return _map.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
            foreach (var kvp in _map) {
                array[arrayIndex] = new KeyValuePair<string, object>(kvp.Key, _map[kvp.Key]);
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<string, object> item) {
            return _map.Remove(item.Key);
        }

        public int Count => _map.Count;

        public bool IsReadOnly => true;

        public bool ContainsKey(string key) {
            return _map.ContainsKey(key);
        }

        public void Add(string key, object value) {
            _map.Add(key, value);
        }

        public bool Remove(string key) {
            return _map.Remove(key);
        }

        public bool TryGetValue(string key, out object value) {
            return _map.TryGetValue(key, out value);
        }

        public object this[string key] {
            get {
                object v;
                _map.TryGetValue(key, out v);
                return v;
            }
            set { _map[key] = value; }
        }

        public ICollection<string> Keys => _map.Keys;
        public ICollection<object> Values => _map.Values;
        #endregion

        #region 
        public TransferContext TryAdd(string key, object value) {
            if (!ContainsKey(key)) {
                if (!IsActionDelegate(value.GetType())) {
                    Add(key, value);
                } else throw new InvalidOperationException("not support delegate value");
            }
            return this;
        }

        public object TryGet(string key) {
            object value;
            if (TryGetValue(key, out value)) return value;

            return default;
        }

        public T TryGet<T>(string key, Func<object, T> func = null) {
            object value;
            if (TryGetValue(key, out value)) {
                if (func != null) return func.Invoke(value);
                try {
                    return (T)value;
                } catch (InvalidCastException e) {
                    Trace.WriteLine($"{e.Message}");
                    try {
                        if (typeof(T).IsValueType) {
                            var converter = TypeDescriptor.GetConverter(typeof(T));
                            return (T)converter.ConvertFromString(value.ToString());
                        } else {
                            return Json.Parse<T>(value.ToString());
                        }
                    } catch (Exception ex) {
                        throw new Exception($"Get key {key} error",ex);
                    }
                }
            }
            return default(T);
        }

        public IDictionary<string, object> ToDictionary() {
            return this;
        }

        private bool IsActionDelegate(Type sourceType) {
            if (sourceType.IsSubclassOf(typeof(MulticastDelegate)) && sourceType.GetMethod("Invoke").ReturnType == typeof(void))
                return true;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}