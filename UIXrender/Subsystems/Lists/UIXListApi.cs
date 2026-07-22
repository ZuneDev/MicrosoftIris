using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.Com;

namespace Microsoft.Iris.Render.Subsystems.Lists;

// [UnmanagedCallersOnly] exports for the SpUIXList* family in
// UIX/Microsoft/Iris/OS/NativeApi.cs.
//
// The `type` argument passed to IUIXListCallbacks.ListChanged is NOT guessed: the managed
// receiver (UIX/Microsoft/Iris/CodeModel/Cpp/DllProxyList.cs, ListChanged) casts it
// directly to Microsoft.Iris.Data.UIListContentsChangeType, so the numbering below is
// that enum's, read off the decompiled source.
public static unsafe class UIXListApi
{
    private enum ChangeType
    {
        Add = 0,
        AddRange = 1,
        Remove = 2,
        Move = 3,
        Insert = 4,
        InsertRange = 5,
        Clear = 6,
        Modified = 7,
        Reset = 8,
    }

    private static uint OK => (uint)HRESULT.S_OK.hr;
    private static uint InvalidArg => unchecked((uint)HRESULT.E_INVALIDARG.hr);
    private static uint Fail => unchecked((uint)HRESULT.E_FAIL.hr);

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListRegisterCallbacks")]
    public static uint SpUIXListRegisterCallbacks(IntPtr nativeList, IntPtr listener)
    {
        if (!HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;

        // The list holds the callback for as long as it's registered, so it takes a
        // reference -- otherwise the caller could release its own last reference and
        // leave us calling into freed memory on the next change notification.
        ComVtable.AddRef(listener);
        list.RegisterListener(listener);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListUnregisterCallbacks")]
    public static uint SpUIXListUnregisterCallbacks(IntPtr nativeList, IntPtr listener)
    {
        if (!HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;

        list.UnregisterListener(listener);
        ComVtable.Release(listener);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListAdd")]
    public static uint SpUIXListAdd(IntPtr nativeList, UIXVariant* item, int* count)
    {
        if (item == null || count == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;

        *count = list.Add(*item);
        NotifyListChanged(list, ChangeType.Add, -1, list.Count - 1, 1);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListInsert")]
    public static uint SpUIXListInsert(IntPtr nativeList, int index, UIXVariant* item)
    {
        if (item == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        if (!list.Insert(index, *item))
            return Fail;

        NotifyListChanged(list, ChangeType.Insert, -1, index, 1);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListRemove")]
    public static uint SpUIXListRemove(IntPtr nativeList, UIXVariant* item)
    {
        if (item == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;

        int index = list.IndexOf(*item);
        if (index < 0 || !list.RemoveAt(index))
            return Fail;

        NotifyListChanged(list, ChangeType.Remove, index, -1, 1);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListRemoveAt")]
    public static uint SpUIXListRemoveAt(IntPtr nativeList, int index)
    {
        if (!HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        if (!list.RemoveAt(index))
            return Fail;

        NotifyListChanged(list, ChangeType.Remove, index, -1, 1);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListMove")]
    public static uint SpUIXListMove(IntPtr nativeList, int oldIndex, int newIndex)
    {
        if (!HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        if (!list.Move(oldIndex, newIndex))
            return Fail;

        NotifyListChanged(list, ChangeType.Move, oldIndex, newIndex, 1);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListClear")]
    public static uint SpUIXListClear(IntPtr nativeList)
    {
        if (!HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;

        int previousCount = list.Count;
        list.Clear();
        NotifyListChanged(list, ChangeType.Clear, -1, -1, previousCount);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListGetItem")]
    public static uint SpUIXListGetItem(IntPtr nativeList, int index, UIXVariant* item)
    {
        if (item == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        if (!list.TryGet(index, out UIXVariant value))
            return Fail;

        *item = value;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListSetItem")]
    public static uint SpUIXListSetItem(IntPtr nativeList, int index, UIXVariant* item)
    {
        if (item == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        if (!list.TrySet(index, *item))
            return Fail;

        NotifyListChanged(list, ChangeType.Modified, index, index, 1);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListGetCount")]
    public static uint SpUIXListGetCount(IntPtr nativeList, int* count)
    {
        if (count == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        *count = list.Count;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListIndexOf")]
    public static uint SpUIXListIndexOf(IntPtr nativeList, UIXVariant* item, int* index)
    {
        if (item == null || index == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        *index = list.IndexOf(*item);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListIsItemAvailable")]
    public static uint SpUIXListIsItemAvailable(IntPtr nativeList, int index, int* isAvailable)
    {
        if (isAvailable == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        *isAvailable = list.IsItemAvailable(index) ? 1 : 0;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListFetchSlowData")]
    public static uint SpUIXListFetchSlowData(IntPtr nativeList, int index)
    {
        if (!HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        if (!list.FetchSlowData(index))
            return Fail;

        NotifySlowDataAcquireComplete(list, index);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListWantSlowDataRequests")]
    public static uint SpUIXListWantSlowDataRequests(IntPtr nativeList, int* wantSlowDataRequests)
    {
        if (wantSlowDataRequests == null || !HandleTable.TryGet(nativeList, out UIXList list))
            return InvalidArg;
        *wantSlowDataRequests = list.WantSlowDataRequests ? 1 : 0;
        return OK;
    }

    // Visual create/release notifications are the managed side telling the store which
    // slots are on screen. Nothing is evicted on release yet (this store keeps everything
    // resident), but the slot is validated so a bad index is reported rather than ignored.
    [UnmanagedCallersOnly(EntryPoint = "SpUIXListNotifyVisualsCreated")]
    public static uint SpUIXListNotifyVisualsCreated(IntPtr nativeList, int index) =>
        HandleTable.TryGet(nativeList, out UIXList list) && (uint)index < (uint)list.Count ? OK : InvalidArg;

    [UnmanagedCallersOnly(EntryPoint = "SpUIXListNotifyVisualsReleased")]
    public static uint SpUIXListNotifyVisualsReleased(IntPtr nativeList, int index) =>
        HandleTable.TryGet(nativeList, out UIXList list) && (uint)index < (uint)list.Count ? OK : InvalidArg;

    // ---- listener dispatch -----------------------------------------------------------

    private static void NotifyListChanged(UIXList list, ChangeType type, int oldIndex, int newIndex, int count)
    {
        foreach (IntPtr listener in list.Listeners)
        {
            void* fn = ComVtable.Slot(listener, ComVtable.FirstMethodSlot + 0); // ListChanged
            if (fn != null)
                ((delegate* unmanaged<IntPtr, int, int, int, int, void>)fn)(listener, (int)type, oldIndex, newIndex, count);
        }
    }

    private static void NotifySlowDataAcquireComplete(UIXList list, int index)
    {
        foreach (IntPtr listener in list.Listeners)
        {
            void* fn = ComVtable.Slot(listener, ComVtable.FirstMethodSlot + 1); // SlowDataAcquireComplete
            if (fn != null)
                ((delegate* unmanaged<IntPtr, int, void>)fn)(listener, index);
        }
    }
}
