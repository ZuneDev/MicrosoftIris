using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Os;

// [UnmanagedCallersOnly] exports for the string/image marshaling helpers and the native
// services registration in UIX/Microsoft/Iris/OS/NativeApi.cs.
//
// The model here follows the original's vocabulary exactly: a "handle" (ulong) is the
// *framework's* identifier for a managed string/image, and a "native string"/"native
// image" is an object on this side that shadows it. Converting between them therefore
// requires calling back into the framework through the registered IRawUIXServices --
// which is what makes SpCreateNativeString able to produce real text (via PinString)
// rather than an empty placeholder.
public static unsafe class MarshalApi
{
    internal sealed class NativeStringObject(string value, ulong handle)
    {
        public string Value { get; } = value;
        public ulong Handle { get; } = handle;
    }

    internal sealed class NativeImageObject(string source, ulong handle)
    {
        public string Source { get; } = source;
        public ulong Handle { get; } = handle;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRegisterNativeServicesCallbacks")]
    public static uint SpRegisterNativeServicesCallbacks(IntPtr rawServices)
    {
        NativeServices.Register(rawServices);
        return (uint)HRESULT.S_OK.hr;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUnregisterNativeServicesCallbacks")]
    public static void SpUnregisterNativeServicesCallbacks() => NativeServices.Unregister();

    // ---- strings ---------------------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpGetStringHandle")]
    public static void SpGetStringHandle(IntPtr nativeString, ulong* handle)
    {
        if (handle == null)
            return;
        *handle = HandleTable.TryGet(nativeString, out NativeStringObject value) ? value.Handle : 0UL;
    }

    // "Convert to managed" and "get the handle" are the same operation here: the native
    // string was created from a framework handle, so it already knows the managed
    // identity it shadows. Kept as two exports because the managed side declares two --
    // and the body is duplicated rather than delegated because an [UnmanagedCallersOnly]
    // method cannot be called from C# at all (CS8901).
    [UnmanagedCallersOnly(EntryPoint = "SpConvertStringToManaged")]
    public static void SpConvertStringToManaged(IntPtr nativeString, ulong* handle)
    {
        if (handle == null)
            return;
        *handle = HandleTable.TryGet(nativeString, out NativeStringObject value) ? value.Handle : 0UL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpCreateNativeString")]
    public static int SpCreateNativeString(ulong handle, int length, IntPtr* nativeString)
    {
        if (nativeString == null)
            return 0;

        *nativeString = IntPtr.Zero;

        // Read the framework's characters through the registered services callback. Without
        // a registration there's no way to reach the text, so the creation genuinely fails
        // rather than silently producing an empty string that would look like valid data.
        if (!NativeServices.IsRegistered)
            return 0;

        char* pinned = NativeServices.PinString(handle);
        if (pinned == null)
            return 0;

        try
        {
            string value = length > 0 ? new string(pinned, 0, length) : NativeString.UniToString(pinned);
            *nativeString = HandleTable.Alloc(new NativeStringObject(value, handle));
            return 1;
        }
        finally
        {
            NativeServices.UnpinString(handle);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpCopyString")]
    public static void SpCopyString(char* source, char* destination, uint length)
    {
        if (source == null || destination == null || length == 0)
            return;

        // Copies at most `length` characters and always terminates, so a caller-sized
        // buffer can't be overrun by a longer source.
        uint i = 0;
        for (; i < length - 1 && source[i] != '\0'; i++)
            destination[i] = source[i];
        destination[i] = '\0';
    }

    // ---- images ----------------------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpGetImageHandle")]
    public static void SpGetImageHandle(IntPtr nativeImage, ulong* handle)
    {
        if (handle == null)
            return;
        *handle = HandleTable.TryGet(nativeImage, out NativeImageObject image) ? image.Handle : 0UL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpConvertImageToManaged")]
    public static HRESULT SpConvertImageToManaged(IntPtr nativeImage, ulong* handle)
    {
        if (handle == null)
            return HRESULT.E_INVALIDARG;

        if (!HandleTable.TryGet(nativeImage, out NativeImageObject image))
        {
            *handle = 0UL;
            return HRESULT.E_INVALIDARG;
        }

        *handle = image.Handle;
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpCreateNativeImage")]
    public static HRESULT SpCreateNativeImage(ulong handle, char* source, IntPtr* nativeImage)
    {
        if (nativeImage == null)
            return HRESULT.E_INVALIDARG;

        *nativeImage = HandleTable.Alloc(new NativeImageObject(NativeString.UniToString(source), handle));
        return HRESULT.S_OK;
    }
}
