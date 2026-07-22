using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop;

// Bit-for-bit mirror of EngineApi.BufferFlags/EngineApi.BufferInfo
// (UIX.RenderApi/Microsoft/Iris/Render/Protocol/EngineApi.cs).
[Flags]
public enum BufferFlags
{
    IsBatch = 1,
    CopyData = 2,
    Valid = CopyData | IsBatch,
}

[StructLayout(LayoutKind.Sequential)]
public struct BufferInfo
{
    public ContextID idContextSrc;
    public ContextID idContextDest;
    public RENDERHANDLE idBuffer;
    public BufferFlags nFlags;
    public uint cbSizeBuffer;
}
