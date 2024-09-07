using System;
using System.Collections;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.Util
{
    /// <summary>
    /// List that orders items using the provided <see cref="IComparer{T}"/> when added.
    /// </summary>
    /// <typeparam name="T">List element type.</typeparam>
    internal class SortedList<T> : IList, IList<T>, IReadOnlyList<T>
    {
        private readonly List<T> list;
        private readonly IComparer<T> comparer;

        public SortedList(IComparer<T> comparer)
        {
            this.list = new List<T>();
            this.comparer = comparer;
        }

        public int Count => list.Count;

        public bool IsReadOnly => ((IList)list).IsReadOnly;

        public bool IsFixedSize => ((IList)list).IsFixedSize;

        public object SyncRoot => ((IList)list).SyncRoot;

        public bool IsSynchronized => ((IList)list).IsSynchronized;

        object IList.this[int index] { get => ((IList)list)[index]; set => ((IList)list)[index] = value; }

        public T this[int index] { get => list[index]; set => list[index] = value; }

        public void Add(T item) => AddInternal(item);

        public void Clear() => list.Clear();

        public bool Contains(T item) => list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T item) => throw new NotSupportedException();

        public bool Remove(T item) => list.Remove(item);

        public void RemoveAt(int index) => list.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();

        public int Add(object value) => AddInternal((T)value);

        public bool Contains(object value) => ((IList)list).Contains(value);

        public int IndexOf(object value) => ((IList)list).IndexOf(value);

        public void Insert(int index, object value) => throw new NotSupportedException();

        public void Remove(object value) => ((IList)list).Remove(value);

        public void CopyTo(Array array, int index) => ((IList)list).CopyTo(array, index);

        private int AddInternal(T item)
        {
            int index = list.BinarySearch(item, comparer);
            index = index >= 0 ? index : ~index;

            list.Insert(index, item);

            return index;
        }
    }
}
