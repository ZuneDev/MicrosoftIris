// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.IRichTextCallbacks
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS
{
    [Guid("3AC74642-83C9-4548-871E-6A37F600673D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRichTextCallbacks
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT InvalidateContent();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT SelectionChanged(int begin, int end);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT CreateCaret(int width, int height);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT SetCaretPos(int x, int y);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT ShowCaret(bool visible);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT SetCursor(EditCursorType type);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT TextChanged();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT MaxLengthExceeded();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT SetTimer(uint id, uint timeout);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT KillTimer(uint id);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT SetScrollRange(
          int whichBar,
          int minPosition,
          int extent,
          int viewExtent,
          int scrollPosition);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT EnableScrollbar(int whichBar, ScrollbarEnableFlags value);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT ClientToWindow(Point pt, out Point ppt);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT ClientToScreen(Point pt, out Point ppt);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT LinkClicked(int start, int end);
    }
}
