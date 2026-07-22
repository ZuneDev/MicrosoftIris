using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Engine;

// Every UIXrender export that hands an opaque "pointer" back to a caller (schema objects,
// XML readers, rich-text objects, bitmaps, data queries, ...) uses this: the managed
// object is pinned by a GCHandle and the caller only ever sees the GCHandle's IntPtr.
// This is the same convention SpRenderThreadInit already established for its thread
// handle -- centralised here so ~10 subsystems don't each re-derive it.
//
// Deliberately not a "real" pointer: the original native library handed out genuine C++
// object pointers, but nothing outside UIXrender is allowed to dereference these (the
// managed declarations all type them as opaque IntPtr/HANDLE), so an indirection is
// both safe and much harder to corrupt.
internal static class HandleTable
{
    public static IntPtr Alloc(object value) =>
        value == null ? IntPtr.Zero : GCHandle.ToIntPtr(GCHandle.Alloc(value, GCHandleType.Normal));

    public static T Get<T>(IntPtr handle) where T : class =>
        handle == IntPtr.Zero ? null : GCHandle.FromIntPtr(handle).Target as T;

    public static bool TryGet<T>(IntPtr handle, out T value) where T : class
    {
        value = Get<T>(handle);
        return value != null;
    }

    public static void Free(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return;

        GCHandle gc = GCHandle.FromIntPtr(handle);
        if (gc.IsAllocated)
        {
            (gc.Target as IDisposable)?.Dispose();
            gc.Free();
        }
    }
}
