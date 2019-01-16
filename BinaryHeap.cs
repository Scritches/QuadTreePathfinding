using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Foundry.Fluent;

namespace QTTest2
{
    public class BinaryHeap<T> : ICollection<T> where T : IComparable<T>
    {
        private T[] _backingStore;
        private int _count;

        public BinaryHeap()
        {
            _backingStore = new T[15];
            _count = 0;
        }

        public BinaryHeap(int capacity)
        {
            _backingStore = new T[capacity];
            _count = 0;
        }

        #region Public Members

        public T Pop()
        {
            unchecked
            {
                if (this.Count == 0) return default(T);
                T returnItem = _backingStore[0];
                SinkItem(0);
                return returnItem;
            }
        }

        public T Peek()
        {
            if (this.Count == 0) return default(T);
            return _backingStore[0];
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
                this.Add(item);
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            unchecked
            {
                GrowIfNeeded();
                _backingStore[this.Count] = item;
                BubbleItem(this.Count);
                _count++;
            }
        }

        public void Clear()
        {
            _count = 0;
            _backingStore = new T[15];
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _count; i++)
                if (_backingStore[i].Equals(item)) return true;
            
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_backingStore, array, this.Count);
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return _backingStore.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            int index = Array.FindIndex(_backingStore, i => i.Equals(item));
            if (index == -1) return false;

            SinkItem(index);
            return true;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _backingStore)
                if (item != null)
                    yield return item;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private Members

        private void SinkItem(int i)
        {
            int swapIndex = i;

            _count--;
            _backingStore[i] = _backingStore[this.Count];

            do
            {
                // check to see if 'i' has 1, 2, or no children
                if ((i * 2 + 2) <= this.Count)
                {
                    // 2 children
                    if (_backingStore[i].IsGreaterOrEqual(_backingStore[i * 2 + 1]))
                        swapIndex = i * 2 + 1;
                    if (_backingStore[swapIndex].IsGreaterOrEqual(_backingStore[i * 2 + 2]))
                        swapIndex = i * 2 + 2;

                }
                else if ((i * 2 + 1) <= this.Count)
                {
                    // 1 child
                    if(_backingStore[i].IsGreaterOrEqual(_backingStore[i*2+1]))
                        swapIndex = i * 2 + 1;
                }

                if (swapIndex == i) break;

                T temp = _backingStore[i];
                _backingStore[i] = _backingStore[swapIndex];
                _backingStore[swapIndex] = temp;

                i = swapIndex;
            } while (true);
        }

        private void BubbleItem(int index)
        {
            while (index != 0)
            {
                int parentIndex = (index - 1) / 2;

                if (_backingStore[index].IsLesser(_backingStore[parentIndex]))
                {
                    T temp = _backingStore[parentIndex];
                    _backingStore[parentIndex] = _backingStore[index];
                    _backingStore[index] = temp;
                    index = parentIndex;
                }
                else { break; }
            }
        }

        private void GrowIfNeeded()
        {
            if (this.Count > _backingStore.Length - 1)
                Array.Resize<T>(ref _backingStore, _backingStore.Length * 2);
        }

        #endregion

    }
}