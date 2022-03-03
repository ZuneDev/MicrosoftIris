// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.InvertElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render
{
    public class InvertElement : EffectOperation
    {
        public InvertElement(string stName)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            this.m_stName = stName;
        }

        public InvertElement() => this.m_typeOperation = EffectOperationType.Invert;
    }
}
