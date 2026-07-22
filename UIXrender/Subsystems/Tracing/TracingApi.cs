using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;

namespace Microsoft.Iris.Render.Subsystems.Tracing;

// [UnmanagedCallersOnly] exports matching the DllImport("UIXRender.dll") tracing
// declarations in UIX/Microsoft/Iris/OS/NativeApi.cs.
public static unsafe class TracingApi
{
    [UnmanagedCallersOnly(EntryPoint = "SpInitializeTracing")]
    public static void SpInitializeTracing() => TracingState.Initialize();

    [UnmanagedCallersOnly(EntryPoint = "SpUninitializeTracing")]
    public static void SpUninitializeTracing() => TracingState.Uninitialize();

    // The three `bool` params have no [MarshalAs] override in the original DllImport, so
    // the CLR marshals each as the default 4-byte Win32 BOOL -- `int` here, not `byte`.
    [UnmanagedCallersOnly(EntryPoint = "SpUpdateTraceSettings")]
    public static void SpUpdateTraceSettings(char* debugTraceFile, char* writeLinePrefix, int sendOutputToDebugger, int showCategories, int timedWriteLines) =>
        TracingState.UpdateSettings(NativeString.UniToString(debugTraceFile), NativeString.UniToString(writeLinePrefix), sendOutputToDebugger != 0, showCategories != 0, timedWriteLines != 0);

    [UnmanagedCallersOnly(EntryPoint = "SpLogTrace")]
    public static void SpLogTrace(char* categoryName, char* message, int indentLevel) =>
        TracingState.LogTrace(NativeString.UniToString(categoryName), NativeString.UniToString(message), indentLevel);
}
