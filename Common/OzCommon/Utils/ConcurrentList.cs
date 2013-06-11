using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzCommon.Utils
{
    public class ConcurrentList<T> : IEnumerable<T>
    {
        List<T> innerList;
        object sync;

        public ConcurrentList()
        {
            sync = new object();
            innerList = new List<T>();
        }

        public int Count
        {
            get
            {
                lock (sync)
                    return innerList.Count;
            }
        }

        public bool TryAdd(T item)
        {
            lock (sync)
            {
                if (innerList.Contains(item))
                {
                    return false;
                }
                innerList.Add(item);
                return true;
            }
        }

        public void Add(T item)
        {
            lock (sync)
                innerList.Add(item);
        }

        public void Clear()
        {
            lock (sync)
                innerList.Clear();
        }

        public bool Contains(T item)
        {
            lock (sync)
                return innerList.Contains(item);
        }

        public bool Remove(T item)
        {
            lock (sync)
                return innerList.Remove(item);
        }

        public bool TryRemove(T item)
        {
            lock (sync)
            {
                if (!innerList.Contains(item))
                {
                    return false;
                }
                innerList.Remove(item);
                return true;
            }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            lock (sync)
            {
                return new List<T>(innerList).GetEnumerator();
                //var concurrentEnumerator = new ConcurrentEnumerator<T>(innerList.GetEnumerator(), sync);
                //return concurrentEnumerator;
            }

        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
