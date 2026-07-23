using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Win32;

// Bit-for-bit mirror of Microsoft.Iris.OS.Win32Api.HANDLE (UIX/Microsoft/Iris/OS/Win32Api.cs)
// -- the opaque handle type NativeApi.cs's rich-text/simple-text exports use (hRto/hSto).
[StructLayout(LayoutKind.Sequential)]
public struct HANDLE
{
    public IntPtr h;
    public static readonly HANDLE NULL = new() { h = IntPtr.Zero };
}
