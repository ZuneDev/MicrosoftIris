using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Win32;

// Bit-for-bit mirror of Microsoft.Iris.Render.Internal.Win32Api.MSG
// (UIX.RenderApi/Microsoft/Iris/Render/Internal/Win32Api.cs) -- the variant EngineApi.cs's
// SpPeekMessage actually resolves to (hwnd is the Render-namespace HWND wrapper, not a bare IntPtr).
[StructLayout(LayoutKind.Sequential)]
public struct MSG
{
    public HWND hwnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public int pt_x;
    public int pt_y;
}
