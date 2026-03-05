using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class ListNonAlloc<T> : IEnumerable<T>
    {
        T[] _array;
        int _count;
        int _expandSize;
        ListNonAllocEnumerator _enumerator;

        class ListNonAllocEnumerator : IEnumerator<T>
        {
            ListNonAlloc<T> _list;
            int _currentIndex = -1;

            public ListNonAllocEnumerator(ListNonAlloc<T> list)
            {
                _list = list;
            }

            public T Current => _list[_currentIndex];

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _list.Count;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            public void Dispose()
            {

            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            _enumerator.Reset();
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <param name="expandSize">How much the array will expand when adding at max capacity.</param>
        public ListNonAlloc(int capacity, int expandSize = 20)
        {
            _array = new T[capacity];
            _count = 0;
            _expandSize = expandSize;
            _enumerator = new ListNonAllocEnumerator(this);
        }

        public int Count => _count;

        public int Capacity => _array.Length;

        public void Add(T item)
        {
            if (_count < _array.Length)
            {
                _array[_count] = item;
                _count++;
            }
            else
            {
                var newCapacity = _array.Length + _expandSize;

                var newArray = new T[newCapacity];
                Array.Copy(_array, newArray, _array.Length);

                _array = newArray;
                _array[_count] = item;
                _count++;
            }
        }

        public void Remove(T item)
        {
            int index = -1;

            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_array[i], item))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                for (int i = index; i < _count - 1; i++)
                {
                    _array[i] = _array[i + 1];
                }
                _count--;
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _count)
            {
                for (int i = index; i < _count - 1; i++)
                {
                    _array[i] = _array[i + 1];
                }
                _count--;
            }
            else
            {
                Debug.LogError("Index out of bounds.");
            }
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_array[i], item))
                {
                    return true;
                }
            }

            return false;
        }

        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < _count)
                {
                    return _array[index];
                }
                else
                {
                    Debug.LogError("Index out of bounds.");
                    return default(T);
                }
            }
            set
            {
                if (index >= 0 && index < _count)
                {
                    _array[index] = value;
                }
                else
                {
                    Debug.LogError("Index out of bounds.");
                }
            }
        }

        public void Clear()
        {
            _count = 0;
        }
    }
}