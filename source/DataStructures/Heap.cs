using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructures
{
    [Serializable]
    public class Heap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>,
                                        ICollection,
                                        IReadOnlyCollection<KeyValuePair<TKey, TValue>>
                                            where TKey : IComparable<TKey>
    {
        public int Count { get; private set; }
        public int Capasity { get { return _entries.Length; } }
        object ICollection.SyncRoot { get { return _syncRoot; } }
        public bool IsSynchronized { get { return false; } }

        private const int MIN_LENGTH = 4;
        private int _version;
        public readonly HeapType HeapType;
        private object _syncRoot;

        private Entry[] _entries;

        public Heap(HeapType heapType)
        {
            HeapType = heapType;
            _version = 0;
            _syncRoot = new object();
        }
        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (_entries == null)
            {
                _entries = new Entry[MIN_LENGTH];
            }
            int entryIndex = Count;
            var entry = new Entry
            {
                Key = key,
                Value = value,
            };
            if (Count >= _entries.Length)
            {
                IncreaseCapasity();
            }
            _entries[entryIndex] = entry;
            SiftUp(entryIndex);
            _version++;
            Count++;
        }

        public KeyValuePair<TKey, TValue> Extract()
        {
            var result = Peek();
            if (!result.HasValue)
            {
                throw new InvalidOperationException("Heap is empty.");
            }
            _entries[0] = _entries[Count - 1];
            _entries[Count - 1] = null;
            Count--;
            SiftDown(0);
            _version++;
            return result.Value;
        }

        public KeyValuePair<TKey, TValue>? Peek()
        {
            if (Count == 0) { return null; }
            return (KeyValuePair<TKey, TValue>)_entries[0];
        }

        public List<KeyValuePair<TKey, TValue>> FindByKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            var result = new List<KeyValuePair<TKey, TValue>>();
            Find(key, 0, result);
            return result;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var items = this.FindByKey(item.Key);
            return items.Count > 0 && items.Contains(item);
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                _entries[i] = null;
            }
            _version++;
            Count = 0;
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(_entries, 0, array, arrayIndex, _entries.Length);
        }

        #region Private

        private void SiftUp(int entryIndex)
        {
            var entry = _entries[entryIndex];
            if (entryIndex == 0)
            {
                return;
            }
            int currentIndex = entryIndex;
            while (currentIndex > 0)
            {
                int parentIndex = (int)((currentIndex + 1) / 2) - 1;
                var parent = _entries[parentIndex];
                if (Compare(parent.Key, entry.Key) > 0)
                {
                    Swap(parentIndex, currentIndex);
                    currentIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        private void SiftDown(int entryIndex)
        {
            int currentIndex = entryIndex;
            while (currentIndex < Count)
            {
                var entry = _entries[currentIndex];
                int leftIndex = currentIndex * 2 + 1;
                if (leftIndex >= Count)
                {
                    return;
                }
                int rightIndex = leftIndex + 1;
                var left = _entries[leftIndex];
                var right = rightIndex < Count ? _entries[rightIndex] : null;
                if (Compare(left.Key, entry.Key) >= 0 && (right == null || Compare(right.Key, entry.Key) >= 0))
                {
                    return;
                }
                int nextIndex = 0;
                if (right == null || Compare(left.Key, right.Key) < 0)
                {
                    Swap(currentIndex, leftIndex);
                    nextIndex = leftIndex;
                }
                else if (right != null && Compare(right.Key, entry.Key) < 0)
                {
                    Swap(currentIndex, rightIndex);
                    nextIndex = rightIndex;
                }
                currentIndex = nextIndex;
            }
        }

        private void Swap(int parentIndex, int childIndex)
        {
            //#if DEBUG
            //			if ((int)(childIndex - 1) / 2 != parentIndex)
            //			{
            //				throw new InvalidOperationException("You can swap only parent with child!");
            //			}
            //#endif
            var parent = _entries[parentIndex];
            _entries[parentIndex] = _entries[childIndex];
            _entries[childIndex] = parent;
        }

        private void IncreaseCapasity()
        {
            var entries = new Entry[(int)(_entries.Length * 1.5)];
            Array.Copy(_entries, entries, _entries.Length);
            _entries = entries;
        }

        private void Find(TKey key, int index, IList<KeyValuePair<TKey, TValue>> storage)
        {
            if (index >= Count) { return; }
            int compareResult = Compare(_entries[index].Key, key);
            if (compareResult > 0) { return; }
            if (compareResult == 0)
            {
                storage.Add((KeyValuePair<TKey, TValue>)_entries[index]);
            }
            Find(key, index * 2 + 1, storage);
            Find(key, index * 2 + 2, storage);
        }

        private int Compare(TKey key1, TKey key2)
        {
            var result = key1.CompareTo(key2);
            return HeapType == HeapType.Min ? result : -1 * result;
        }
        #endregion

        private class Entry
        {
            public TValue Value { get; set; }
            public TKey Key { get; set; }

            public static explicit operator KeyValuePair<TKey, TValue>(Entry entry)
            {
                return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        #region IEnumerable
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly int _heapVersion;
            private Heap<TKey, TValue> _heap;
            private KeyValuePair<TKey, TValue> _current;
            private int _currentIndex;

            public Enumerator(Heap<TKey, TValue> heap)
            {
                _heapVersion = heap._version;
                _heap = heap;
                _current = new KeyValuePair<TKey, TValue>();
                _currentIndex = -1;
            }

            public KeyValuePair<TKey, TValue> Current { get { return _current; } }

            object IEnumerator.Current { get { return _current; } }

            public void Dispose()
            {
                Reset();
                _heap = null;
            }

            public bool MoveNext()
            {
                ThrowOnChange();
                if (_heap._entries != null && _heap._entries.Length > 0 && _currentIndex < _heap.Count - 1)
                {
                    _currentIndex++;
                    _current = (KeyValuePair<TKey, TValue>)_heap._entries[_currentIndex];
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                ThrowOnChange();
                _current = new KeyValuePair<TKey, TValue>();
                _currentIndex = -1;
            }

            private void ThrowOnChange()
            {
                if (_heapVersion != _heap._version)
                { throw new InvalidOperationException("Collection was changed during enumeration"); }
            }
        }
        #endregion
    }
}
