using System;
using System.Collections.Concurrent;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Engine;

// Maps a registered ContextID to the buffer-processing callback it should receive
// deliveries on, via SpBufferOpen -- the smallest coherent slice of "Engine core" (see
// logs/UIXrender/EngineCore.md) needed for SpRenderThreadInit and SpBufferOpen to mean
// anything together. Callback stored as a raw IntPtr (not a typed function pointer
// field) so nothing here needs to be unsafe/deal with function-pointer-as-generic-arg
// concerns; callers cast at the point they actually invoke it.
internal static class ContextRegistry
{
    internal readonly struct Entry
    {
        public readonly IntPtr Callback;
        public readonly IntPtr CallbackData;

        public Entry(IntPtr callback, IntPtr callbackData)
        {
            Callback = callback;
            CallbackData = callbackData;
        }
    }

    private static readonly ConcurrentDictionary<uint, Entry> s_contexts = new();

    public static void Register(ContextID id, IntPtr callback, IntPtr callbackData)
        => s_contexts[id.value] = new Entry(callback, callbackData);

    public static void Unregister(ContextID id) => s_contexts.TryRemove(id.value, out _);

    public static bool TryGet(ContextID id, out Entry entry) => s_contexts.TryGetValue(id.value, out entry);
}
