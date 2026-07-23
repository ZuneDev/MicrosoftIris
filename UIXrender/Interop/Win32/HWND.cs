using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Win32;

// Bit-for-bit mirror of Microsoft.Iris.Render.HWND (UIX.RenderApi/Microsoft/Iris/Render/HWND.cs).
[StructLayout(LayoutKind.Sequential)]
public struct HWND
{
    public IntPtr h;
    public static readonly HWND NULL = new() { h = IntPtr.Zero };
}
