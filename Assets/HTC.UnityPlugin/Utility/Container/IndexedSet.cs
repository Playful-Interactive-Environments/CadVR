//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HTC.UnityPlugin.Utility
{
    public class IndexedSet<T> : IList<T>
    {
        private readonly List<T> m_List = new List<T>();
        private readonly Dictionary<T, int> m_Dictionary = new Dictionary<T, int>();

        public int Count { get { return m_List.Count; } }

        public bool IsReadOnly { get { return false; } }

        public T this[int index]
        {
            get { return m_List[index]; }
            set
            {
                T item = m_List[index];
                m_Dictionary.Remove(item);
                m_List[index] = value;
                m_Dictionary.Add(item, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            m_List.Add(item);
            m_Dictionary.Add(item, m_List.Count - 1);
        }

        public bool AddUnique(T item)
        {
            if (m_Dictionary.ContainsKey(item)) { return false; }

            m_List.Add(item);
            m_Dictionary.Add(item, m_List.Count - 1);

            return true;
        }

        public bool Remove(T item)
        {
            int index = -1;
            if (!m_Dictionary.TryGetValue(item, out index)) { return false; }

            RemoveAt(index);
            return true;
        }

        public void Clear()
        {
            m_List.Clear();
            m_Dictionary.Clear();
        }

        public bool Contains(T item)
        {
            return m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            int index = -1;
            m_Dictionary.TryGetValue(item, out index);
            return index;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Not supported, because this container does not guarantee ordering.");
        }

        public void RemoveAt(int index)
        {
            T item = m_List[index];
            m_Dictionary.Remove(item);
            if (index == m_List.Count - 1)
            {
                m_List.RemoveAt(index);
            }
            else
            {
                int replaceItemIndex = m_List.Count - 1;
                T replaceItem = m_List[replaceItemIndex];
                m_List[index] = replaceItem;
                m_Dictionary[replaceItem] = index;
                m_List.RemoveAt(replaceItemIndex);
            }
        }

        public void RemoveAll(Predicate<T> match)
        {
            if (match == null) { return; }

            var removed = 0;

            for (int i = 0, imax = m_List.Count; i < imax; ++i)
            {
                if (match(m_List[i]))
                {
                    m_Dictionary.Remove(m_List[i]);
                    ++removed;
                }
                else
                {
                    if (removed != 0)
                    {
                        m_Dictionary[m_List[i]] = i - removed;
                        m_List[i - removed] = m_List[i];
                    }
                }
            }

            for (; removed > 0; --removed)
            {
                m_List.RemoveAt(m_List.Count - 1);
            }
        }

        public void Sort(Comparison<T> sortLayoutFunction)
        {
            m_List.Sort(sortLayoutFunction);
            for (int i = m_List.Count - 1; i >= 0; --i)
            {
                m_Dictionary[m_List[i]] = i;
            }
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return m_List.AsReadOnly();
        }
    }
}