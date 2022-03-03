// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IHwndHostWindow
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    public interface IHwndHostWindow : IDisposable
    {
        ColorF BackgroundColor { get; set; }

        Point ClientPosition { get; set; }

        IntPtr Hwnd { get; }

        bool Visible { get; set; }

        Size WindowSize { get; set; }

        event EventHandler OnHandleChanged;
    }
}
