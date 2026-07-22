using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Interop;

// [UnmanagedCallersOnly] methods can't take `string` params (not blittable) -- callers
// take raw pointers and go through here instead, so the "which encoding" decision (read
// from each P/Invoke call site's CharSet, see per-module logs) is made in one place.
public static unsafe class NativeString
{
    public static string AnsiToString(byte* p) => p == null ? null : Marshal.PtrToStringAnsi((IntPtr)p);

    public static string UniToString(char* p) => p == null ? null : Marshal.PtrToStringUni((IntPtr)p);
}
