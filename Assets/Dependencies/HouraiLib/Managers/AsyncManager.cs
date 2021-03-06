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
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse {
    /// <summary> A Singleton for managing a number of asynchronous operations </summary>
    public sealed class AsyncManager : Singleton<AsyncManager> {
        // Set of all asynchronous operations managed by the manager
        readonly List<AsyncOperation> _operations = new List<AsyncOperation>();

        /// <summary> The overall progress of all of the asynchronous actions. Shown as a ratio in the range [0.0, 1.0] </summary>
        public float Progress {
            get {
                return OperationsInProgress > 0
                    ? 1.0f
                    : _operations.Average(op => op.progress);
            }
        }

        /// <summary> The number of operations in progress currently </summary>
        public int OperationsInProgress {
            get { return _operations.Count; }
        }

        static event Action WaitingSynchronousActions;

        /// <summary> Add a async operation to manage. Can optionally provide a callback to be called once the operation is
        /// finished. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="operation" /> is null </exception>
        /// <param name="operation"> the operation to manage </param>
        /// <param name="callback"> optional parameter, if not null, will be called after finish executing </param>
        public void AddOperation(AsyncOperation operation,
                                 Action callback = null) {
            Check.NotNull(operation);
            _operations.Add(operation);
            StartCoroutine(WaitForOperation(operation, callback));
        }

        /// <summary> Adds a resource request to manage. Can optionally provide a callback to be called once the operation is
        /// finished. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="request" /> is null </exception>
        /// <typeparam name="T"> the type of object loaded by </typeparam>
        /// <param name="request"> the ResourceRequest to manage </param>
        /// <param name="callback"> optional parameter, if not null, will be called after finish executing </param>
        public void AddOpreation<T>(ResourceRequest request,
                                    Action<T> callback = null) where T : Object {
            Check.NotNull(request);
            _operations.Add(request);
            StartCoroutine(WaitForResource(request, callback));
        }

        public static void AddSynchronousAction(Action action) {
            WaitingSynchronousActions += action;
        }

        protected override void Awake() {
            base.Awake();
            Flush();
        }

        void Start() { Flush(); }

        void Update() { Flush(); }

        static void Flush() {
            if (WaitingSynchronousActions == null)
                return;
            WaitingSynchronousActions();
            WaitingSynchronousActions = null;
        }

        IEnumerator WaitForOperation(AsyncOperation operation, Action callback) {
            yield return operation;
            _operations.Remove(operation);
            if (callback != null)
                callback();
        }

        IEnumerator WaitForResource<T>(ResourceRequest request,
                                       Action<T> callback) where T : Object {
            yield return request;
            _operations.Remove(request);
            if (callback == null)
                yield break;
            var obj = request.asset as T;
            callback(obj);
        }
    }
}
