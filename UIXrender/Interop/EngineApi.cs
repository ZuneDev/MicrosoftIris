#if NETCOREAPP

using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop.Protocol;
using Microsoft.Iris.Render.Interop.Win32;

namespace Microsoft.Iris.Render.Interop;

// [UnmanagedCallersOnly] exports matching the core transport DllImport("UIXRender.dll")
// declarations in UIX.RenderApi/Microsoft/Iris/Render/Protocol/EngineApi.cs. This is
// the *only* place in UIXrender that deals with raw pointers/unmanaged function
// pointers -- everything it calls into (Microsoft.Iris.Render.Engine.EngineService) is
// ordinary, pointer-free managed C#. See logs/UIXrender/EngineCore.md for why
// SpInit/SpUninit are stubs and SpWrapBufferProc is a pass-through rather than a real
// wrapper, and logs/UIXrender/FullSurface.md for the rest of this file (added later).
public static unsafe class EngineApi
{
    [UnmanagedCallersOnly(EntryPoint = "SpInit")]
    public static HRESULT SpInit(InitArgs* args) => HRESULT.S_OK;

    [UnmanagedCallersOnly(EntryPoint = "SpUninit")]
    public static HRESULT SpUninit() => HRESULT.S_OK;

    [UnmanagedCallersOnly(EntryPoint = "SpWrapBufferProc")]
    public static HRESULT SpWrapBufferProc(IntPtr pfnProcessBufferProc, IntPtr* ppNativeProc)
    {
        if (ppNativeProc == null)
            return HRESULT.E_INVALIDARG;

        *ppNativeProc = pfnProcessBufferProc;
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRenderThreadInit")]
    public static HRESULT SpRenderThreadInit(InitArgs* argsRender, IntPtr* pThread)
    {
        if (argsRender == null || pThread == null)
            return HRESULT.E_INVALIDARG;

        ContextID contextId = argsRender->idContext;
        IntPtr nativeCallback = argsRender->pfnProcessBuffer;
        IntPtr callbackData = argsRender->pvProcessData;

        // Adapts the raw native function pointer into the idiomatic BufferReceivedHandler
        // shape EngineService deals in -- all pointer reconstruction for this direction
        // lives here, not in Engine/.
        BufferReceivedHandler handler = (source, bufferHandle, flags, data) =>
        {
            if (nativeCallback == IntPtr.Zero)
                return;
            fixed (byte* pData = data)
            {
                var info = new BufferInfo
                {
                    idContextSrc = source,
                    idContextDest = contextId,
                    idBuffer = bufferHandle,
                    nFlags = flags,
                    cbSizeBuffer = (uint)data.Length,
                };
                var fn = (delegate* unmanaged<IntPtr, uint, BufferInfo*, void*, int>)nativeCallback;
                fn(callbackData, contextId.value, &info, pData);
            }
        };

        IRenderThreadHandle thread = EngineService.StartRenderThread(contextId, handler);
        *pThread = GCHandle.ToIntPtr(GCHandle.Alloc(thread, GCHandleType.Normal));
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRenderThreadUninit")]
    public static HRESULT SpRenderThreadUninit(IntPtr pThread)
    {
        if (pThread == IntPtr.Zero)
            return HRESULT.E_INVALIDARG;

        GCHandle handle = GCHandle.FromIntPtr(pThread);
        (handle.Target as IDisposable)?.Dispose();
        handle.Free();
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpBufferOpen")]
    public static HRESULT SpBufferOpen(BufferInfo* phdrData, void* pvData)
    {
        if (phdrData == null)
            return HRESULT.E_INVALIDARG;

        var span = new ReadOnlySpan<byte>(pvData, (int)phdrData->cbSizeBuffer);
        return EngineService.SendBuffer(phdrData->idContextSrc, phdrData->idContextDest, phdrData->idBuffer, phdrData->nFlags, span);
    }

    // Real, but a documented no-op: this reimplementation has no Win32 message queue to
    // pump (no window is created anywhere in this repo yet), and LocalChannel.Connect()
    // -- the only path real Zune uses -- never calls this (see logs/UIXrender/EngineCore.md's
    // SpInit/SpUninit note for the same "IGMM_STANDARD messaging model" open question).
    // Always reports "no message available" rather than guessing at pump semantics.
    [UnmanagedCallersOnly(EntryPoint = "SpPeekMessage")]
    public static HRESULT SpPeekMessage(MSG* msg, HWND hwnd, uint nMsgFilterMin, uint nMsgFilterMax, uint wRemoveMsg, uint* nResult)
    {
        if (msg == null || nResult == null)
            return HRESULT.E_INVALIDARG;
        *msg = default;
        *nResult = EngineService.PeekMessage(nMsgFilterMin, nMsgFilterMax, wRemoveMsg);
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpWaitMessage")]
    public static HRESULT SpWaitMessage(uint nTimeOutMs, IntPtr unused)
    {
        EngineService.WaitMessage(nTimeOutMs);
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpInvoke")]
    public static HRESULT SpInvoke(ContextID idContext, IntPtr pfnInvoke, IntPtr pvArgs, int synchronous) =>
        EngineService.Invoke(idContext, pfnInvoke, pvArgs, synchronous != 0);

    [UnmanagedCallersOnly(EntryPoint = "SpRemoteCreateServerStreams")]
    public static HRESULT SpRemoteCreateServerStreams(char* stSession, TransportProtocol nProtocol, IntPtr* pSendStream, IntPtr* pReceiveStream)
    {
        if (pSendStream == null || pReceiveStream == null)
            return HRESULT.E_INVALIDARG;

        HRESULT hr = EngineService.RemoteCreateServerStreams(NativeString.UniToString(stSession), nProtocol, out IntPtr send, out IntPtr receive);
        *pSendStream = send;
        *pReceiveStream = receive;
        return hr;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRemoteWaitServerStreamsConnected")]
    public static HRESULT SpRemoteWaitServerStreamsConnected(TransportProtocol nProtocol, IntPtr pSendStream, IntPtr pReceiveStream) =>
        EngineService.RemoteWaitServerStreamsConnected(nProtocol, pSendStream);

    [UnmanagedCallersOnly(EntryPoint = "SpRemoteServerInit")]
    public static HRESULT SpRemoteServerInit(IntPtr pSendStream, IntPtr pReceiveStream, InitArgs argsSend, IntPtr* pSession)
    {
        if (pSession == null)
            return HRESULT.E_INVALIDARG;

        // Adapt the raw native process-buffer function pointer into a BufferReceivedHandler,
        // exactly as SpRenderThreadInit does -- RemoteServerConnection deals only in the
        // pointer-free handler shape.
        ContextID contextId = argsSend.idContext;
        IntPtr nativeCallback = argsSend.pfnProcessBuffer;
        IntPtr callbackData = argsSend.pvProcessData;
        BufferReceivedHandler handler = nativeCallback == IntPtr.Zero
            ? null
            : (source, bufferHandle, flags, data) =>
            {
                fixed (byte* pData = data)
                {
                    var info = new BufferInfo
                    {
                        idContextSrc = source,
                        idContextDest = contextId,
                        idBuffer = bufferHandle,
                        nFlags = flags,
                        cbSizeBuffer = (uint)data.Length,
                    };
                    var fn = (delegate* unmanaged<IntPtr, uint, BufferInfo*, void*, int>)nativeCallback;
                    fn(callbackData, source.value, &info, pData);
                }
            };

        HRESULT hr = EngineService.RemoteServerInit(pSendStream, contextId, handler, out IntPtr session);
        *pSession = session;
        return hr;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRemoteServerUninit")]
    public static HRESULT SpRemoteServerUninit(IntPtr pSession, int fForceShutdown, ShutdownReason* nShutdownReason)
    {
        if (nShutdownReason == null)
            return HRESULT.E_INVALIDARG;

        HRESULT hr = EngineService.RemoteServerUninit(pSession, fForceShutdown != 0, out ShutdownReason reason);
        *nShutdownReason = reason;
        return hr;
    }

    // Deliberately not implemented: HLSL effect compilation exists only as a
    // graphics-API-specific service (d3dcompiler_47.dll / Silk.NET.Direct3D.Compilers),
    // and Silk.NET has no cross-platform abstraction over it -- so per this project's
    // "abstractions, not specific graphics APIs" dependency policy there is nothing
    // correct to call here yet. Reports failure honestly (with empty out-params, so a
    // caller that ignores the HRESULT still sees a well-defined "no blob") rather than
    // pretending success. See logs/UIXrender/FullSurface.md, decision 1.
    // TODO: revisit once this project has a rendering backend of its own to compile for.
    [UnmanagedCallersOnly(EntryPoint = "SpDx9CompileEffect")]
    public static HRESULT SpDx9CompileEffect(byte* stEffect, byte* stDefines, IntPtr* pErrorString, IntPtr* pErrorBuffer, IntPtr* pEffectBlob, uint* effectBlobSize, IntPtr* pEffectBlobBuffer)
    {
        // Empty out-params so a caller ignoring the HRESULT still sees a well-defined
        // "no blob"; the not-implemented decision itself lives in EngineService.
        if (pErrorString != null) *pErrorString = IntPtr.Zero;
        if (pErrorBuffer != null) *pErrorBuffer = IntPtr.Zero;
        if (pEffectBlob != null) *pEffectBlob = IntPtr.Zero;
        if (effectBlobSize != null) *effectBlobSize = 0;
        if (pEffectBlobBuffer != null) *pEffectBlobBuffer = IntPtr.Zero;
        return EngineService.Dx9CompileEffect();
    }

    // Generic COM release: calls IUnknown::Release through the object's own vtable
    // (slot 2, after QueryInterface/AddRef) rather than assuming anything about what
    // concrete type pUnknown is -- this export exists precisely to release arbitrary
    // IUnknown-shaped pointers handed across the boundary.
    [UnmanagedCallersOnly(EntryPoint = "SpObjectRelease")]
    public static void SpObjectRelease(IntPtr pUnknown)
    {
        if (pUnknown == IntPtr.Zero)
            return;
        void* vtbl = *(void**)pUnknown;
        var release = (delegate* unmanaged<IntPtr, uint>)(*((void**)vtbl + 2));
        release(pUnknown);
    }
}

#endif
