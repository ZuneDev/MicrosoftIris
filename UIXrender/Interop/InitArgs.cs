using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop;

// Bit-for-bit mirror of EngineApi.InitArgs
// (UIX.RenderApi/Microsoft/Iris/Render/Protocol/EngineApi.cs). The managed side declares
// pfnTimeout as a delegate (TimeoutEventHandler); the CLR marshals a delegate field to a
// plain function pointer before the struct crosses into native code, so it's IntPtr here
// -- same wire bytes, no delegate type on our side.
[StructLayout(LayoutKind.Sequential)]
public struct InitArgs
{
    public uint cbSize;
    public ContextID idContext;
    public int cItemsPerGroupBits;
    public int cGroupBits;
    public IntPtr pfnProcessBuffer;
    public IntPtr pvProcessData;
    public RENDERHANDLE idObjectBrokerClass;
    public IntPtr pfnTimeout;
    public IntPtr pvTimeoutData;
    public uint nTimeOutSec;
}
