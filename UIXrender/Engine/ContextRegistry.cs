using System.Collections.Concurrent;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Engine;

// Maps a registered ContextID to the handler it should receive deliveries on via
// SendBuffer -- the smallest coherent slice of "Engine core" (see
// logs/UIXrender/EngineCore.md) needed for StartRenderThread and SendBuffer to mean
// anything together. Holds an ordinary delegate now (not a raw callback pointer) --
// pointer marshaling, where it's still needed for native callers, lives entirely in
// Interop/EngineApi.cs.
internal static class ContextRegistry
{
    private static readonly ConcurrentDictionary<uint, BufferReceivedHandler> s_contexts = new();

    public static void Register(ContextID id, BufferReceivedHandler handler) => s_contexts[id.value] = handler;

    public static void Unregister(ContextID id) => s_contexts.TryRemove(id.value, out _);

    public static bool TryGet(ContextID id, out BufferReceivedHandler handler) => s_contexts.TryGetValue(id.value, out handler);
}
