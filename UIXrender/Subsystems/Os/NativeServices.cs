using System;
using Microsoft.Iris.Render.Interop.Com;

namespace Microsoft.Iris.Render.Subsystems.Os;

// The registered IRawUIXServices callback -- how UIXrender calls *back into* the managed
// framework (UIX.dll). Registered via SpRegisterNativeServicesCallbacks.
//
// Slot numbers below are IRawUIXServices' declaration order
// (UIX/Microsoft/Iris/OS/IRawUIXServices.cs) offset by the three IUnknown slots. They are
// read off that file, not guessed; the interface is
// [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)], so there are no IDispatch slots
// in between.
internal static unsafe class NativeServices
{
    private const int SlotCrash = ComVtable.FirstMethodSlot + 0;
    private const int SlotNotifyChangeForObject = ComVtable.FirstMethodSlot + 1;
    private const int SlotAllocateString = ComVtable.FirstMethodSlot + 2;
    private const int SlotCopyString = ComVtable.FirstMethodSlot + 3;
    private const int SlotPinString = ComVtable.FirstMethodSlot + 4;
    private const int SlotUnpinString = ComVtable.FirstMethodSlot + 5;
    private const int SlotReleaseString = ComVtable.FirstMethodSlot + 6;
    private const int SlotAllocateImageFromUri = ComVtable.FirstMethodSlot + 7;
    private const int SlotAllocateImageFromBits = ComVtable.FirstMethodSlot + 8;
    private const int SlotRemoveCachedImage = ComVtable.FirstMethodSlot + 9;
    private const int SlotReleaseImage = ComVtable.FirstMethodSlot + 10;
    private const int SlotRegisterDataProvider = ComVtable.FirstMethodSlot + 11;
    private const int SlotGetDataMapping = ComVtable.FirstMethodSlot + 12;
    private const int SlotNotifyChangeForDataObject = ComVtable.FirstMethodSlot + 13;
    private const int SlotGetAppWindowHandle = ComVtable.FirstMethodSlot + 14;
    private const int SlotReportError = ComVtable.FirstMethodSlot + 15;
    private const int SlotLowPriorityDeferredInvoke = ComVtable.FirstMethodSlot + 16;

    private static IntPtr s_services;

    public static bool IsRegistered => s_services != IntPtr.Zero;

    public static void Register(IntPtr services)
    {
        Unregister();
        if (services != IntPtr.Zero)
        {
            ComVtable.AddRef(services);
            s_services = services;
        }
    }

    public static void Unregister()
    {
        if (s_services != IntPtr.Zero)
        {
            ComVtable.Release(s_services);
            s_services = IntPtr.Zero;
        }
    }

    // Pins the framework's string for the handle and returns a pointer to its characters.
    // This is what makes SpCreateNativeString able to materialise real text rather than an
    // empty placeholder: the text lives on the managed side, and this is the only way to
    // read it.
    public static char* PinString(ulong handle)
    {
        void* fn = ComVtable.Slot(s_services, SlotPinString);
        return fn == null ? null : ((delegate* unmanaged<IntPtr, ulong, char*>)fn)(s_services, handle);
    }

    public static void UnpinString(ulong handle)
    {
        void* fn = ComVtable.Slot(s_services, SlotUnpinString);
        if (fn != null)
            ((delegate* unmanaged<IntPtr, ulong, void>)fn)(s_services, handle);
    }

    public static void ReleaseString(ulong handle)
    {
        void* fn = ComVtable.Slot(s_services, SlotReleaseString);
        if (fn != null)
            ((delegate* unmanaged<IntPtr, ulong, void>)fn)(s_services, handle);
    }

    public static ulong AllocateString(char* value, out int length)
    {
        length = 0;
        void* fn = ComVtable.Slot(s_services, SlotAllocateString);
        if (fn == null)
            return 0;

        fixed (int* pLength = &length)
            return ((delegate* unmanaged<IntPtr, char*, int*, ulong>)fn)(s_services, value, pLength);
    }

    public static void ReleaseImage(ulong handle)
    {
        void* fn = ComVtable.Slot(s_services, SlotReleaseImage);
        if (fn != null)
            ((delegate* unmanaged<IntPtr, ulong, void>)fn)(s_services, handle);
    }

    public static void ReportError(bool isWarning, char* message)
    {
        void* fn = ComVtable.Slot(s_services, SlotReportError);
        if (fn != null)
            ((delegate* unmanaged<IntPtr, int, char*, void>)fn)(s_services, isWarning ? 1 : 0, message);
    }

    public static IntPtr GetAppWindowHandle()
    {
        void* fn = ComVtable.Slot(s_services, SlotGetAppWindowHandle);
        return fn == null ? IntPtr.Zero : ((delegate* unmanaged<IntPtr, IntPtr>)fn)(s_services);
    }
}
