using System;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.Protocol;
using Microsoft.Iris.Render.Subsystems.Remote;

namespace Microsoft.Iris.Render.Engine;

// The idiomatic managed API for UIXrender's core transport -- the single thing both
// Interop/EngineApi.cs's [UnmanagedCallersOnly] shims (for native callers) and managed
// callers like UIX.RenderApi call into, so there is exactly one implementation of each
// operation. The buffer-delivery path (StartRenderThread/SendBuffer) is entirely
// pointer-free. The remainder (Invoke, the SpRemote* family, ObjectRelease) deal in
// opaque IntPtr *handles* the caller round-trips back to us -- never dereferenced here --
// plus, for Invoke only, a raw function pointer the caller asked us to run; that one spot
// is the sole `unsafe` in this type. See logs/UIXrender/EngineCore.md and FullSurface.md.
public static class EngineService
{
    // ---- buffer delivery (pointer-free) ----------------------------------------------

    public static IRenderThreadHandle StartRenderThread(ContextID contextId, BufferReceivedHandler onBufferReceived)
        => RenderThread.Start(contextId, onBufferReceived);

    public static HRESULT SendBuffer(ContextID sourceContext, ContextID destContext, RENDERHANDLE bufferHandle, BufferFlags flags, ReadOnlySpan<byte> data)
    {
        if (!ContextRegistry.TryGet(destContext, out BufferReceivedHandler handler))
            return HRESULT.E_FAIL;

        handler(sourceContext, bufferHandle, flags, data);
        return HRESULT.S_OK;
    }

    // ---- message pump ----------------------------------------------------------------

    // There is no Win32 message queue behind this reimplementation yet (no window is
    // created anywhere), and LocalChannel -- the path real Zune uses -- never peeks: it
    // relies on the SendBuffer callback instead. So a peek always reports "no message".
    // Returns the WorkResult flags value (0 = nothing processed / no new message).
    public static uint PeekMessage(uint filterMin, uint filterMax, uint removeMsg) => 0;

    // Real timeout wait (there is no message to wake early for, per PeekMessage's note),
    // so "wait for a message or the timeout" collapses to waiting out the timeout.
    public static void WaitMessage(uint timeoutMs)
    {
        if (timeoutMs != 0)
            System.Threading.Thread.Sleep((int)Math.Min(timeoutMs, int.MaxValue));
    }

    // Runs a deferred callback. The only in-repo caller (IRenderEngine.InterThreadWake)
    // passes a null function pointer purely to wake a pump -- and there is no blocking
    // pump here -- so a null pointer is a well-defined no-op rather than an error.
    public static unsafe HRESULT Invoke(ContextID context, IntPtr pfnInvoke, IntPtr pvArgs, bool synchronous)
    {
        if (pfnInvoke == IntPtr.Zero)
            return HRESULT.S_OK;

        if (synchronous)
        {
            ((delegate* unmanaged<IntPtr, void>)pfnInvoke)(pvArgs);
        }
        else
        {
            IntPtr fn = pfnInvoke;
            IntPtr args = pvArgs;
            System.Threading.ThreadPool.QueueUserWorkItem(_ => ((delegate* unmanaged<IntPtr, void>)fn)(args));
        }
        return HRESULT.S_OK;
    }

    // ---- remote channel (opaque IntPtr handles, never dereferenced) -------------------

    public static HRESULT RemoteCreateServerStreams(string session, TransportProtocol protocol, out IntPtr sendStream, out IntPtr receiveStream)
        => RemoteServerConnection.CreateServerStreams(protocol, session, out sendStream, out receiveStream);

    public static HRESULT RemoteWaitServerStreamsConnected(TransportProtocol protocol, IntPtr sendStream)
        => RemoteServerConnection.WaitConnected(protocol, sendStream);

    public static HRESULT RemoteServerInit(IntPtr sendStream, ContextID context, BufferReceivedHandler onRemoteToLocal, out IntPtr session)
        => RemoteServerConnection.ServerInit(sendStream, context, onRemoteToLocal, out session);

    public static HRESULT RemoteServerUninit(IntPtr session, bool forceShutdown, out ShutdownReason reason)
        => RemoteServerConnection.ServerUninit(session, forceShutdown, out reason);

    // Backs SpObjectRelease in the managed-direct path, whose only callers release the
    // remote stream handles minted by RemoteCreateServerStreams.
    public static void ReleaseRemoteStream(IntPtr streamHandle)
        => RemoteServerConnection.ReleaseHandle(streamHandle);

    // ---- effects ---------------------------------------------------------------------

    // HLSL effect compilation is a graphics-API-specific service with no cross-platform
    // abstraction available under this project's dependency policy -- so this reports
    // "not implemented" honestly rather than faking a compiled blob. See
    // logs/UIXrender/FullSurface.md, decision 1.
    public static HRESULT Dx9CompileEffect() => HRESULT.E_NOTIMPL;
}
