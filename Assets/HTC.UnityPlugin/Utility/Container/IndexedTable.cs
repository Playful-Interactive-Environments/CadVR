//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HTC.UnityPlugin.Utility
{
    public interface IIndexedTableReadOnly<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        int Count { get; }
        ICollection<TKey> Keys { get; }
        ICollection<TValue> Values { get; }

        TValue this[TKey key] { get; }

        TKey GetKeyByIndex(int index);
        TValue GetValueByIndex(int index);
        bool ContainsKey(TKey key);
        bool TryGetValue(TKey key, out TValue value);
        void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex);
        IIndexedSetReadOnly<TKey> AsIndexedSet();
    }

    public class IndexedTable<TKey, TValue> : IDictionary<TKey, TValue>, IIndexedTableReadOnly<TKey, TValue>
    {
        protected readonly Dictionary<TKey, TValue> m_Dictionary = new Dictionary<TKey, TValue>();

        protected IndexedSet<TKey> m_IndexedSet { get; private set; }

        public IndexedTable() : this(new IndexedSet<TKey>()) { }

        public IndexedTable(IndexedSet<TKey> indexedSet)
        {
            m_IndexedSet = indexedSet;
        }

        public int Count { get { return m_Dictionary.Count; } }

        public bool IsReadOnly { get { return false; } }

        public TKey GetKeyByIndex(int index)
        {
            return m_IndexedSet[index];
        }

        public TValue GetValueByIndex(int index)
        {
            return m_Dictionary[m_IndexedSet[index]];
        }

        public ICollection<TKey> Keys { get { return m_Dictionary.Keys; } }

        public ICollection<TValue> Values { get { return m_Dictionary.Values; } }

        public TValue this[TKey key]
        {
            get { return m_Dictionary[key]; }
            set
            {
                m_Dictionary[key] = value;
                m_IndexedSet.AddUnique(key);
            }
        }

        public void Add(TKey key, TValue value = default(TValue))
        {
            m_Dictionary.Add(key, value);
            m_IndexedSet.Add(key);
        }

        public bool AddUniqueKey(TKey key, TValue value = default(TValue))
        {
            if (m_Dictionary.ContainsKey(key)) { return false; }

            m_Dictionary.Add(key, value);
            if (m_IndexedSet.Contains(key))
            {
                UnityEngine.Debug.Log("AddUniqueKey assert m_IndexedSet shouldn't contains key");
            }
            m_IndexedSet.Add(key);

            return true;
        }

        public bool ContainsKey(TKey key)
        {
            return m_Dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (!m_Dictionary.Remove(key))
            {
                if (m_IndexedSet.Contains(key))
                {
                    UnityEngine.Debug.Log("Remove assert m_IndexedSet shouldn't contains key");
                }
                return false;
            }
            //if (!m_Dictionary.Remove(key)) { return false; }

            m_IndexedSet.Remove(key);

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Dictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return (m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!(m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item)) { return false; }

            m_IndexedSet.Remove(item.Key);

            return true;
        }

        private Predicate<TKey> InternalPredicate(Predicate<TKey> match)
        {
            return (item) =>
            {
                if (match(item))
                {
                    m_Dictionary.Remove(item);
                    return true;
                }
                return false;
            };
        }

        public void RemoveAll(Predicate<TKey> match)
        {
            m_IndexedSet.RemoveAll(InternalPredicate(match));
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            m_Dictionary.Clear();
            m_IndexedSet.Clear();
        }

        public ReadOnlyCollection<TKey> AsReadOnly()
        {
            return m_IndexedSet.AsReadOnly();
        }

        public IIndexedSetReadOnly<TKey> AsIndexedSet()
        {
            return m_IndexedSet;
        }
    }
}