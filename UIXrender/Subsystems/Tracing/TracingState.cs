namespace Microsoft.Iris.Render.Subsystems.Tracing;

// Minimal real state for SpInitializeTracing/SpUninitializeTracing. The rest of the
// tracing surface (SpUpdateTraceSettings, SpLogTrace) is a later session's work -- see
// the subsystem ordering in the approved plan.
internal static class TracingState
{
    public static bool IsInitialized { get; private set; }

    public static void Initialize() => IsInitialized = true;
    public static void Uninitialize() => IsInitialized = false;
}
