//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;

namespace HTC.UnityPlugin.Utility
{
    public class OrderedIndexedTable<TKey, TValue> : IndexedTable<TKey, TValue>
    {
        public OrderedIndexedTable() : base(new OrderedIndexedSet<TKey>()) { }

        public void Insert(int index, TKey keyItem, TValue valueItem)
        {
            m_IndexedSet.Insert(index, keyItem);
            m_Dictionary.Add(keyItem, valueItem);
        }

        public void Insert(int index, KeyValuePair<TKey, TValue> item)
        {
            m_IndexedSet.Insert(index, item.Key);
            m_Dictionary.Add(item.Key, item.Value);
        }

        public TKey GetFirstKey() { return m_IndexedSet[0]; }

        public bool TryGetFirstKey(out TKey item)
        {
            if (m_Dictionary.Count == 0)
            {
                item = default(TKey);
                return false;
            }
            else
            {
                item = GetFirstKey();
                return true;
            }
        }

        public TKey GetLastKey() { return m_IndexedSet[m_Dictionary.Count - 1]; }

        public bool TryGetLastKey(out TKey item)
        {
            if (m_Dictionary.Count == 0)
            {
                item = default(TKey);
                return false;
            }
            else
            {
                item = GetLastKey();
                return true;
            }
        }

        public TValue GetFirstValue() { return m_Dictionary[m_IndexedSet[0]]; }

        public bool TryGetFirstValue(out TValue item)
        {
            if (m_Dictionary.Count == 0)
            {
                item = default(TValue);
                return false;
            }
            else
            {
                item = GetFirstValue();
                return true;
            }
        }

        public TValue GetLastValue() { return m_Dictionary[m_IndexedSet[m_Dictionary.Count - 1]]; }

        public bool TryGetLastValue(out TValue item)
        {
            if (m_Dictionary.Count == 0)
            {
                item = default(TValue);
                return false;
            }
            else
            {
                item = GetLastValue();
                return true;
            }
        }
    }
}