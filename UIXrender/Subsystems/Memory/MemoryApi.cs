using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Engine;

namespace Microsoft.Iris.Render.Subsystems.Memory;

// [UnmanagedCallersOnly] exports matching SpMemAlloc/SpMemFree/SpFreeDib
// (UIX/Microsoft/Iris/OS/NativeApi.cs). Real logic: plain unmanaged heap allocation, no
// stubbing needed -- there's nothing platform-specific about a heap allocator.
public static unsafe class MemoryApi
{
    // `bool zeroMemory` in the original DllImport has no [MarshalAs] override, so the CLR
    // marshals it as the default 4-byte Win32 BOOL, not 1 byte -- `int` here, not `byte`.
    [UnmanagedCallersOnly(EntryPoint = "SpMemAlloc")]
    public static IntPtr SpMemAlloc(uint cb, int zeroMemory)
    {
        if (cb == 0)
            return IntPtr.Zero;

        void* p = zeroMemory != 0 ? NativeMemory.AllocZeroed(cb) : NativeMemory.Alloc(cb);
        return (IntPtr)p;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpMemFree")]
    public static void SpMemFree(IntPtr pv)
    {
        if (pv != IntPtr.Zero)
            NativeMemory.Free((void*)pv);
    }

    // Frees a text bitmap produced by SpRichTextRasterize -- the only "DIB" this
    // reimplementation hands out. The handle wraps an unmanaged ARGB buffer (TextBitmap),
    // disposed here. See logs/UIXrender/Rendering.md.
    [UnmanagedCallersOnly(EntryPoint = "SpFreeDib")]
    public static void SpFreeDib(IntPtr hdib) => HandleTable.Free(hdib);
}
