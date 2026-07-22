using System;

namespace Microsoft.Iris.Render.Interop.Com;

// Several UIXrender exports take a COM callback interface (IUIXListCallbacks,
// IRawUIXServices, IRichTextCallbacks, IImeCallbacks). Their managed declarations use
// [MarshalAs(UnmanagedType.Interface)], but an [UnmanagedCallersOnly] method cannot
// accept a non-blittable parameter -- what actually arrives on the wire either way is a
// pointer to the object's vtable-bearing COM instance. So UIXrender receives IntPtr and
// calls through the vtable here.
//
// This is the "reconstruct the interface rather than invent a raw pointer type" approach
// CLAUDE.md's *COM objects* section prescribes, applied in the callee direction: slot
// numbering starts at 3 because every one of these interfaces is
// ComInterfaceType.InterfaceIsIUnknown, so slots 0/1/2 are
// QueryInterface/AddRef/Release, and the declared methods follow in declaration order.
internal static unsafe class ComVtable
{
    // Slot 0/1/2 of IUnknown.
    public const int SlotQueryInterface = 0;
    public const int SlotAddRef = 1;
    public const int SlotRelease = 2;

    // First slot available to a derived interface's own methods.
    public const int FirstMethodSlot = 3;

    public static void* Slot(IntPtr comObject, int slot)
    {
        if (comObject == IntPtr.Zero)
            return null;

        void** vtable = *(void***)comObject;
        return vtable[slot];
    }

    public static uint AddRef(IntPtr comObject)
    {
        void* fn = Slot(comObject, SlotAddRef);
        return fn == null ? 0 : ((delegate* unmanaged<IntPtr, uint>)fn)(comObject);
    }

    public static uint Release(IntPtr comObject)
    {
        void* fn = Slot(comObject, SlotRelease);
        return fn == null ? 0 : ((delegate* unmanaged<IntPtr, uint>)fn)(comObject);
    }
}
