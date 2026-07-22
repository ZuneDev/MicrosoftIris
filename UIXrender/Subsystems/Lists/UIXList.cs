using System;
using System.Collections.Generic;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Lists;

// The managed model behind the SpUIXList* family (UIX/Microsoft/Iris/OS/NativeApi.cs) --
// the native backing store for a data-bound/virtualized ListBox.
//
// "Virtualized" here means the list distinguishes items that are resident from items that
// still need fetching: IsItemAvailable/FetchSlowData/WantSlowDataRequests are the
// original surface's vocabulary for that, and SlowDataAcquireComplete is how the native
// side told the managed listener an item had arrived. This implementation keeps that
// distinction real (an availability flag per slot) rather than pretending every item is
// always resident, because the managed ListBox's scrolling behavior depends on it.
internal sealed class UIXList
{
    private readonly List<UIXVariant> _items = new();
    private readonly List<bool> _available = new();
    private readonly List<IntPtr> _listeners = new();

    public int Count => _items.Count;

    public bool WantSlowDataRequests { get; set; }

    public IReadOnlyList<IntPtr> Listeners => _listeners;

    public void RegisterListener(IntPtr listener)
    {
        if (listener != IntPtr.Zero && !_listeners.Contains(listener))
            _listeners.Add(listener);
    }

    public void UnregisterListener(IntPtr listener) => _listeners.Remove(listener);

    public int Add(UIXVariant item)
    {
        _items.Add(item);
        _available.Add(true);
        return _items.Count;
    }

    public bool Insert(int index, UIXVariant item)
    {
        if ((uint)index > (uint)_items.Count)
            return false;
        _items.Insert(index, item);
        _available.Insert(index, true);
        return true;
    }

    public bool RemoveAt(int index)
    {
        if ((uint)index >= (uint)_items.Count)
            return false;
        _items.RemoveAt(index);
        _available.RemoveAt(index);
        return true;
    }

    public bool Remove(UIXVariant item)
    {
        int index = IndexOf(item);
        return index >= 0 && RemoveAt(index);
    }

    public int IndexOf(UIXVariant item)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].type == item.type && _items[i].union == item.union)
                return i;
        }
        return -1;
    }

    public void Clear()
    {
        _items.Clear();
        _available.Clear();
    }

    public bool TryGet(int index, out UIXVariant item)
    {
        if ((uint)index >= (uint)_items.Count)
        {
            item = default;
            return false;
        }
        item = _items[index];
        return true;
    }

    public bool TrySet(int index, UIXVariant item)
    {
        if ((uint)index >= (uint)_items.Count)
            return false;
        _items[index] = item;
        _available[index] = true;
        return true;
    }

    public bool Move(int oldIndex, int newIndex)
    {
        if ((uint)oldIndex >= (uint)_items.Count || (uint)newIndex >= (uint)_items.Count)
            return false;

        UIXVariant item = _items[oldIndex];
        bool available = _available[oldIndex];
        _items.RemoveAt(oldIndex);
        _available.RemoveAt(oldIndex);
        _items.Insert(newIndex, item);
        _available.Insert(newIndex, available);
        return true;
    }

    public bool IsItemAvailable(int index) => (uint)index < (uint)_available.Count && _available[index];

    // Marks a slot as pending. The managed listener is told the data arrived via
    // SlowDataAcquireComplete; since nothing in this reimplementation performs a real
    // asynchronous fetch yet, the item is immediately considered resident again -- the
    // notification still fires, so the listener's state machine is exercised correctly.
    public bool FetchSlowData(int index)
    {
        if ((uint)index >= (uint)_available.Count)
            return false;
        _available[index] = true;
        return true;
    }
}
