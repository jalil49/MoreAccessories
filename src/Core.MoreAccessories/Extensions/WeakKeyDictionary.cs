using System;
using System.Collections;
using System.Collections.Generic;

namespace MoreAccessoriesKOI
{
    public class WeakReference<T> : WeakReference
    {
        public WeakReference(T reference) : base(reference) { }

        public new T Target { get { return (T)base.Target; } }
    }

    public class HashedWeakReference<T> : WeakReference<T>
    {
        private readonly int _hashCode;

        public HashedWeakReference(T reference) : base(reference)
        {
            _hashCode = reference.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj != null && _hashCode == obj.GetHashCode();
        }
    }

    public class WeakKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<HashedWeakReference<TKey>, TValue> _dictionary = new Dictionary<HashedWeakReference<TKey>, TValue>();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void Remove(object key)
        {
            _dictionary.Remove(new HashedWeakReference<TKey>((TKey)key));
        }

        public object this[object key] { get { return _dictionary[new HashedWeakReference<TKey>((TKey)key)]; } set { _dictionary[new HashedWeakReference<TKey>((TKey)key)] = (TValue)value; } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(new HashedWeakReference<TKey>(item.Key), item.Value);
        }

        public bool Contains(object key)
        {
            return false;
        }

        public void Add(object key, object value)
        {
            _dictionary.Add(new HashedWeakReference<TKey>((TKey)key), (TValue)value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public int Count { get { return _dictionary.Count; } }
        public bool IsReadOnly { get { return false; } }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(new HashedWeakReference<TKey>(key));
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(new HashedWeakReference<TKey>(key), value);
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(new HashedWeakReference<TKey>(key));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(new HashedWeakReference<TKey>(key), out value);
        }

        public TValue this[TKey key] { get { return _dictionary[new HashedWeakReference<TKey>(key)]; } set { _dictionary[new HashedWeakReference<TKey>(key)] = value; } }
        public ICollection<TKey> Keys { get { return null; } }
        public ICollection<TValue> Values { get { return _dictionary.Values; } }

        public void Purge()
        {
            var newDic = new Dictionary<HashedWeakReference<TKey>, TValue>();
            foreach (var pair in _dictionary)
            {
                if (pair.Key.IsAlive)
                    newDic.Add(pair.Key, pair.Value);
            }
            _dictionary = newDic;
        }

        public class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly IEnumerator<KeyValuePair<HashedWeakReference<TKey>, TValue>> _enumerator;

            public Enumerator(WeakKeyDictionary<TKey, TValue> dictionary)
            {
                _enumerator = dictionary._dictionary.GetEnumerator();
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    var p = _enumerator.Current;
                    return new KeyValuePair<TKey, TValue>(p.Key.Target, p.Value);
                }
            }
            object IEnumerator.Current { get { return Current; } }
        }
    }
}
