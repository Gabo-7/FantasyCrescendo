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
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {
    public static class TransformExtensions {
        public static void SetX(this Transform transform,
                                float x,
                                bool local = false) {
            transform.SetPositionLocation(0, x, local);
        }

        public static void SetY(this Transform transform,
                                float y,
                                bool local = false) {
            transform.SetPositionLocation(1, y, local);
        }

        public static void SetZ(this Transform transform,
                                float z,
                                bool local = false) {
            transform.SetPositionLocation(2, z, local);
        }

        public static void Reset(this Transform transform) {
            Check.NotNull(transform).localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        /// <summary> Copys the position and rotation of another transform onto one. </summary>
        /// <param name="transform"> </param>
        /// <param name="target"> </param>
        /// <exception cref="ArgumentNullException"> <paramref name="transform" />
        /// or <paramref name="target" /> are null </exception>
        public static void Copy(this Transform transform, Transform target) {
            Check.NotNull(transform);
            Check.NotNull(target);
            transform.position = target.position;
            transform.rotation = target.rotation;
        }

        /// <summary> Finds the lowest common ancestor between two Transforms. Returns null if either are null or both are not part
        /// of the same Transform hiearchy. </summary>
        /// <param name="transform"> the first transform </param>
        /// <param name="other"> the second transform </param>
        /// <returns> the lowest common ancestor between the two transforms </returns>
        public static Transform FindCommonAncestor(this Transform transform,
                                                   Transform other) {
            if (!transform || !other || transform.root != other.root)
                return null;
            var s1 = new HashSet<Transform>();
            var s2 = new HashSet<Transform>();
            Transform t1 = transform;
            Transform t2 = other;
            while (t1 || t2) {
                if (t1) {
                    if (s2.Contains(t1))
                        return t1;
                    s1.Add(t1);
                    t1 = t1.parent;
                }
                if (t2) {
                    if (s1.Contains(t2))
                        return t2;
                    s2.Add(t2);
                    t2 = t2.parent;
                }
            }
            return null;
        }

        static void SetPositionLocation(this Transform transform,
                                        int component,
                                        float value,
                                        bool local) {
            Check.NotNull(transform);
            Vector3 position = local
                ? transform.localPosition
                : transform.position;
            position[component] = value;
            if (local)
                transform.localPosition = position;
            else
                transform.position = position;
        }
    }
}
