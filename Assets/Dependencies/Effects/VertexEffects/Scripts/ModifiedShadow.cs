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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> The behaviour of this class is almost the same as the original except: 1. It absorbs version differences. 2.
/// It corrects the calculation of vertex list capacity. </summary>
public class ModifiedShadow : Shadow {
    protected new void ApplyShadow(List<UIVertex> verts,
                                   Color32 color,
                                   int start,
                                   int end,
                                   float x,
                                   float y) {
        UIVertex vt;

        // The capacity calculation of the original version seems wrong.
        int neededCpacity = verts.Count + (end - start);
        if (verts.Capacity < neededCpacity)
            verts.Capacity = neededCpacity;

        for (int i = start; i < end; ++i) {
            vt = verts[i];
            verts.Add(vt);

            Vector3 v = vt.position;
            v.x += x;
            v.y += y;
            vt.position = v;
            Color32 newColor = color;
            if (useGraphicAlpha)
                newColor.a = (byte) (newColor.a * verts[i].color.a / 255);
            vt.color = newColor;
            verts[i] = vt;
        }
    }

#if UNITY_5_2 && !UNITY_5_2_1pX
    public override void ModifyMesh(Mesh mesh)
    {
        if (!this.IsActive())
            return;

        using (var vh = new VertexHelper(mesh))
        {
            ModifyMesh(vh);
            vh.FillMesh(mesh);
        }
    }
#endif

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
#if UNITY_5_2_1pX || UNITY_5_3_OR_NEWER
    public override void ModifyMesh(VertexHelper vh)
#else
    public void ModifyMesh(VertexHelper vh)
#endif
    {
        if (!IsActive())
            return;

        var list = new List<UIVertex>();
        vh.GetUIVertexStream(list);

        ModifyVertices(list);

#if UNITY_5_2_1pX || UNITY_5_3
        vh.Clear();
#endif
        vh.AddUIVertexTriangleStream(list);
    }

    public virtual void ModifyVertices(List<UIVertex> verts) { }
#endif
}