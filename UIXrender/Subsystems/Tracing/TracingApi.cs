using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Subsystems.Tracing;

// [UnmanagedCallersOnly] exports matching two of the DllImport("UIXRender.dll")
// declarations in UIX/Microsoft/Iris/OS/NativeApi.cs.
public static class TracingApi
{
    [UnmanagedCallersOnly(EntryPoint = "SpInitializeTracing")]
    public static void SpInitializeTracing() => TracingState.Initialize();

    [UnmanagedCallersOnly(EntryPoint = "SpUninitializeTracing")]
    public static void SpUninitializeTracing() => TracingState.Uninitialize();
}
