﻿// The MIT License (MIT)
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

public class CircleOutline : ModifiedShadow {
    [SerializeField]
    int m_circleCount = 2;

    [SerializeField]
    int m_firstSample = 4;

    [SerializeField]
    int m_sampleIncrement = 2;

    public int circleCount {
        get { return m_circleCount; }

        set {
            m_circleCount = Mathf.Max(value, 1);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public int firstSample {
        get { return m_firstSample; }

        set {
            m_firstSample = Mathf.Max(value, 2);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public int sampleIncrement {
        get { return m_sampleIncrement; }

        set {
            m_sampleIncrement = Mathf.Max(value, 1);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate() {
        base.OnValidate();
        circleCount = m_circleCount;
        firstSample = m_firstSample;
        sampleIncrement = m_sampleIncrement;
    }
#endif

    public override void ModifyVertices(List<UIVertex> verts) {
        if (!IsActive())
            return;

        int total = (m_firstSample * 2 + m_sampleIncrement * (m_circleCount - 1))
            * m_circleCount / 2;
        verts.Capacity = verts.Count * (total + 1);
        int original = verts.Count;
        var count = 0;
        int sampleCount = m_firstSample;
        float dx = effectDistance.x / circleCount;
        float dy = effectDistance.y / circleCount;
        for (var i = 1; i <= m_circleCount; i++) {
            float rx = dx * i;
            float ry = dy * i;
            float radStep = 2 * Mathf.PI / sampleCount;
            float rad = i % 2 * radStep * 0.5f;
            for (var j = 0; j < sampleCount; j++) {
                int next = count + original;
                ApplyShadow(verts,
                    effectColor,
                    count,
                    next,
                    rx * Mathf.Cos(rad),
                    ry * Mathf.Sin(rad));
                count = next;
                rad += radStep;
            }
            sampleCount += m_sampleIncrement;
        }
    }
}