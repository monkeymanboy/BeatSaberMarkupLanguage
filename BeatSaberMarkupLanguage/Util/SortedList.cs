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
        private readonly List<T> _list;
        private readonly IComparer<T> _comparer;

        public SortedList(IComparer<T> comparer)
        {
            _list = new List<T>();
            _comparer = comparer;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => ((IList)_list).IsReadOnly;

        public bool IsFixedSize => ((IList)_list).IsFixedSize;

        public object SyncRoot => ((IList)_list).SyncRoot;

        public bool IsSynchronized => ((IList)_list).IsSynchronized;

        object IList.this[int index] { get => ((IList)_list)[index]; set => ((IList)_list)[index] = value; }

        public T this[int index] { get => _list[index]; set => _list[index] = value; }

        public void Add(T item) => AddInternal(item);

        public void Clear() => _list.Clear();

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item) => throw new NotSupportedException();

        public bool Remove(T item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();

        public int Add(object value) => AddInternal((T)value);

        public bool Contains(object value) => ((IList)_list).Contains(value);

        public int IndexOf(object value) => ((IList)_list).IndexOf(value);

        public void Insert(int index, object value) => throw new NotSupportedException();

        public void Remove(object value) => ((IList)_list).Remove(value);

        public void CopyTo(Array array, int index) => ((IList)_list).CopyTo(array, index);

        private int AddInternal(T item)
        {
            int index = _list.BinarySearch(item, _comparer);
            index = index >= 0 ? index : ~index;

            _list.Insert(index, item);

            return index;
        }
    }
}
