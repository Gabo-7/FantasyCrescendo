// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace HouraiTeahouse.Console {
    /// <summary> A fixed size FIFO (First In First Out) Queue. Will dequeue elements until within a defined limit. </summary>
    /// <typeparam name="T"> the type of the elements contained by the queue </typeparam>
    public class FixedSizeQueue<T> : IEnumerable<T> {
        public const int DefaultSize = 100;

        // The underlying queue backing FixedQueue objects
        readonly Queue<T> _queue;
        int _limit;

        /// <summary> Initializes a new instance of FixedQueue{T} that is empty and default starting limit of 100. </summary>
        public FixedSizeQueue() {
            _limit = DefaultSize;
            _queue = new Queue<T>();
        }

        /// <summary> Initializes a new instance of FixedQueue{T} that is empty with a specified starting limit. </summary>
        /// <exception cref="ArgumentException"> <paramref name="size" /> is negative. </exception>
        /// <param name="size"> the limit on the size of the instance </param>
        public FixedSizeQueue(int size) {
            Check.Argument("size", size >= 0);
            _limit = size;
            _queue = new Queue<T>();
        }

        /// <summary> Initializes a new instance of FixedQueue{T} from a known collection or enumerable. </summary>
        /// <remarks> If the enumerable is empty or null, the resultant instance will be empty. Elements will be added in order
        /// they are enumerated from the object's enumerator. </remarks>
        /// <param name="size"> the limit on the size of the instance </param>
        /// <param name="collection"> the source collection/enumerable to include. </param>
        /// <exception cref="ArgumentException"> <paramref name="size" /> is negative. </exception>
        public FixedSizeQueue(int size, IEnumerable<T> collection) {
            Check.Argument("size", size >= 0);
            _limit = size;
            _queue = new Queue<T>();
            if (collection == null)
                return;
            foreach (T obj in collection)
                Enqueue(obj);
        }

        /// <summary> The limit on the number of elements an instance can contain </summary>
        public int Limit {
            get { return _limit; }
            set {
                _limit = value;
                CapactityCheck();
            }
        }

        public int Count {
            get { return _queue.Count; }
        }

        /// <summary> Adds an object to the end of the FixedQueue{T}. Will remove elements if this puts over the specfied limit. </summary>
        /// <param name="obj"> The object to add to the FixedQueue{T}. The value can be null for reference types. </param>
        public void Enqueue(T obj) {
            _queue.Enqueue(obj);
            CapactityCheck();
        }

        /// <summary> Removes and returns object from the beginning of the FixedQueue{T}. </summary>
        /// <returns> The object that is removed from the beginning of the FixedQueue{T} </returns>
        /// <exception cref="InvalidOperationException"> the FixedQueue{T} is empty </exception>
        public T Dequeue() { return Check.NotEmpty(_queue).Dequeue(); }

        /// <summary> Returns the object at the beginning of the FixedQueue{T} without removing it. </summary
        /// <returns> the object at the beginning of the FixedQueue{T} </returns>
        /// <exception cref="InvalidOperationException"> the FixedQueue{T} is empty </exception>
        /// >
        public T Peek() { return Check.NotEmpty(_queue).Peek(); }

        /// <summary> Removes all objects from the FixedQueue{T}. </summary>
        public void Clear() { _queue.Clear(); }

        /// <summary> Helper funciton to check whether the FixedQueue{T} is over capacity or not. </summary>
        void CapactityCheck() {
            if (_queue.Count <= Limit)
                return;
            lock (this) {
                // Remove elements until under the limit.
                while (_queue.Count > _limit)
                    _queue.Dequeue();
            }
        }

        #region IEnumerable Implementation

        public IEnumerator<T> GetEnumerator() { return _queue.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion
    }
}