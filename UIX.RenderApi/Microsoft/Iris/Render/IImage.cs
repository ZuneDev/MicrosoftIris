// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IImage
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    public interface IImage : ISharedRenderObject
    {
        Size Size { get; }

        ImageFormat Format { get; }

        string Identifier { get; }

        bool LoadContent(ImageFormat format, Size size, int Stride, IntPtr Data);
    }
}
