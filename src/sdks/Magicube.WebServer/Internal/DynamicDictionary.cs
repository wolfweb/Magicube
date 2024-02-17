using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Magicube.WebServer.Internal {
    public class DynamicDictionary : DynamicObject, IDictionary<string, object> {
        private readonly IDictionary<string, object> _dictionary = new DefaultValueDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public void Add(string key, object value) {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(string key) {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys {
            get { return _dictionary.Keys; }
        }

        public bool Remove(string key) {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value) {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<object> Values {
            get { return _dictionary.Values; }
        }

        public object this[string key] {
            get {
                object value;
                _dictionary.TryGetValue(key, out value);
                return value;
            }
            set { _dictionary[key] = value; }
        }

        public void Add(KeyValuePair<string, object> item) {
            _dictionary.Add(item);
        }

        public void Clear() {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item) {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly {
            get { return _dictionary.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item) {
            return _dictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dictionary.GetEnumerator();
        }

        public class DefaultValueDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
            private readonly IDictionary<TKey, TValue> _dictionary;

            public DefaultValueDictionary() {
                _dictionary = new Dictionary<TKey, TValue>();
            }

            public DefaultValueDictionary(IDictionary<TKey, TValue> dictionary) {
                _dictionary = dictionary;
            }

            public DefaultValueDictionary(IEqualityComparer<TKey> comparer) {
                _dictionary = new Dictionary<TKey, TValue>(comparer);
            }

            public void Add(TKey key, TValue value) {
                _dictionary.Add(key, value);
            }

            public bool ContainsKey(TKey key) {
                return _dictionary.ContainsKey(key);
            }

            public ICollection<TKey> Keys {
                get { return _dictionary.Keys; }
            }

            public bool Remove(TKey key) {
                return _dictionary.Remove(key);
            }

            public bool TryGetValue(TKey key, out TValue value) {
                return _dictionary.TryGetValue(key, out value);
            }

            public ICollection<TValue> Values {
                get { return _dictionary.Values; }
            }

            public TValue this[TKey key] {
                get {
                    TValue value;
                    _dictionary.TryGetValue(key, out value);
                    return value;
                }
                set { _dictionary[key] = value; }
            }

            public void Add(KeyValuePair<TKey, TValue> item) {
                _dictionary.Add(item);
            }

            public void Clear() {
                _dictionary.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item) {
                return _dictionary.Contains(item);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
                _dictionary.CopyTo(array, arrayIndex);
            }

            public int Count {
                get { return _dictionary.Count; }
            }

            public bool IsReadOnly {
                get { return _dictionary.IsReadOnly; }
            }

            public bool Remove(KeyValuePair<TKey, TValue> item) {
                return _dictionary.Remove(item);
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
                return _dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return _dictionary.GetEnumerator();
            }
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            result = _dictionary[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            if (_dictionary.ContainsKey(binder.Name))
                _dictionary[binder.Name] = value;
            else
                _dictionary.Add(binder.Name, value);

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            if (_dictionary.ContainsKey(binder.Name) && _dictionary[binder.Name] is Delegate) {
                var delegateValue = (Delegate)_dictionary[binder.Name];
                result = delegateValue.DynamicInvoke(args);
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }
    }
}
