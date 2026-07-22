using System;
using System.Runtime.InteropServices;

namespace UIXrender.Interop.Tests;

// Minimal, self-contained P/Invoke declarations for the Phase 0 spike, matching
// UIX.RenderApi/Microsoft/Iris/Render/Protocol/EngineApi.cs's originals ABI-for-ABI.
// Doesn't reference EngineApi.cs directly: it's `internal` in its own assembly, and this
// harness is meant to exercise the published UIXrender.dll the same way an arbitrary
// external caller would, not ride on internals visibility. ContextID/RENDERHANDLE are
// plain `uint` here rather than their real wrapper structs -- same single-field
// sequential layout, so the ABI is identical and there's no need to duplicate those
// wrapper types just for this harness.
internal static unsafe class NativeMethods
{
    private const string Dll = "UIXrender.dll";

    [StructLayout(LayoutKind.Sequential)]
    public struct InitArgs
    {
        public uint cbSize;
        public uint idContext;
        public int cItemsPerGroupBits;
        public int cGroupBits;
        public IntPtr pfnProcessBuffer;
        public IntPtr pvProcessData;
        public uint idObjectBrokerClass;
        public IntPtr pfnTimeout;
        public IntPtr pvTimeoutData;
        public uint nTimeOutSec;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BufferInfo
    {
        public uint idContextSrc;
        public uint idContextDest;
        public uint idBuffer;
        public uint nFlags;
        public uint cbSizeBuffer;
    }

    public delegate int MessageBufferEventHandler(IntPtr pData, uint hContext, BufferInfo* pBufferInfo, void* pvBufferData);

    [DllImport(Dll)]
    public static extern void SpInitializeTracing();

    [DllImport(Dll)]
    public static extern void SpUninitializeTracing();

    [DllImport(Dll)]
    public static extern int SpWrapBufferProc(MessageBufferEventHandler pfnProcessBufferProc, out IntPtr ppNativeProc);

    [DllImport(Dll)]
    public static extern int SpRenderThreadInit(ref InitArgs argsRender, out IntPtr pThread);

    [DllImport(Dll)]
    public static extern int SpRenderThreadUninit(IntPtr pThread);
}
