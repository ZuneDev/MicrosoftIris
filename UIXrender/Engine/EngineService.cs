using System;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.Protocol;
using Microsoft.Iris.Render.Subsystems.Os;
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

    // Drains the render thread's message queue (running any deferred work posted to it) and
    // reports WorkResult flags: ProcessedMessage (1) if work ran, else None (0). Never
    // NewUserMessage (2) -- that flag drives Win32 TranslateMessage/DispatchMessage, which
    // needs a real HWND this backend-agnostic pump doesn't have; leaving it unset routes
    // ProcessNativeEvents down its non-Win32 branch. See logs/UIXrender/Rendering.md.
    public static uint PeekMessage(uint filterMin, uint filterMax, uint removeMsg) =>
        MessagePump.Peek() ? 1u : 0u;

    // Blocks the render thread until work is posted (e.g. InterThreadWake) or the timeout
    // elapses -- a real wait that returns early on a wake, not a fixed sleep.
    public static void WaitMessage(uint timeoutMs) => MessagePump.Wait(timeoutMs);

    // A null function pointer is InterThreadWake: post a wake so a blocked WaitMessage
    // returns. A real pointer is a deferred invoke -- run inline when synchronous, else
    // queue it to run on the pump (render) thread the next time it peeks, rather than on a
    // random threadpool thread.
    public static HRESULT Invoke(ContextID context, IntPtr pfnInvoke, IntPtr pvArgs, bool synchronous)
    {
        if (pfnInvoke == IntPtr.Zero)
        {
            MessagePump.PostWake();
            return HRESULT.S_OK;
        }

        if (synchronous)
        {
            RunInvoke(pfnInvoke, pvArgs);
        }
        else
        {
            IntPtr fn = pfnInvoke;
            IntPtr args = pvArgs;
            MessagePump.Post(() => RunInvoke(fn, args));
        }
        return HRESULT.S_OK;
    }

    private static unsafe void RunInvoke(IntPtr fn, IntPtr args) =>
        ((delegate* unmanaged<IntPtr, void>)fn)(args);

    // Registers a windowing backend that feeds OS window/input messages into the pump.
    // UIXrender core never creates a window itself (that would require a concrete GLFW/SDL
    // backend, breaking backend-agnosticism); a host or optional backend package supplies
    // one through this seam. See logs/UIXrender/Rendering.md.
    public static void SetWindowMessageSource(IWindowMessageSource source) =>
        MessagePump.SetWindowSource(source);

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
