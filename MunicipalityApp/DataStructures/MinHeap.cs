using System;
using System.Collections.Generic;

namespace MunicipalityApp.DataStructures
{

    //  Min-heap (binary heap) priority queue with configurable comparer.
    public class MinHeap<T>
    {
        private readonly List<T> _data = new List<T>();
        private readonly IComparer<T> _comparer;
        public MinHeap() : this(Comparer<T>.Default) { }

        // Creates a new min-heap using the specified comparer for ordering.
        public MinHeap(IComparer<T> comparer) { _comparer = comparer; }

        // Number of items currently in the heap.
        public int Count => _data.Count;

        // Inserts an item into the heap.
        public void Push(T item)
        {
            _data.Add(item);
            SiftUp(_data.Count - 1);
        }
        // Returns the minimum element without removing it.
        public T Peek()
        {
            if (_data.Count == 0) throw new InvalidOperationException("Heap is empty");
            return _data[0];
        }

        // Removes and returns the minimum element.
        public T Pop()
        {
            if (_data.Count == 0) throw new InvalidOperationException("Heap is empty");
            var root = _data[0];
            var last = _data[^1];
            _data.RemoveAt(_data.Count - 1);
            if (_data.Count > 0)
            {
                _data[0] = last;
                SiftDown(0);
            }
            return root;
        }
        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (_comparer.Compare(_data[i], _data[p]) >= 0) break;
                (_data[i], _data[p]) = (_data[p], _data[i]);
                i = p;
            }
        }
        private void SiftDown(int i)
        {
            while (true)
            {
                int l = 2 * i + 1, r = l + 1, s = i;
                if (l < _data.Count && _comparer.Compare(_data[l], _data[s]) < 0) s = l;
                if (r < _data.Count && _comparer.Compare(_data[r], _data[s]) < 0) s = r;
                if (s == i) break;
                (_data[i], _data[s]) = (_data[s], _data[i]);
                i = s;
            }
        }
    }
}
