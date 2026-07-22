using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Engine;

namespace Microsoft.Iris.Render.Interop;

// [UnmanagedCallersOnly] exports matching the core transport DllImport("UIXRender.dll")
// declarations in UIX.RenderApi/Microsoft/Iris/Render/Protocol/EngineApi.cs. See
// logs/UIXrender/EngineCore.md for the reasoning behind SpInit/SpUninit being stubs and
// SpWrapBufferProc being a pass-through rather than a real wrapper.
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

        var thread = RenderThread.Start(*argsRender);
        var handle = GCHandle.Alloc(thread, GCHandleType.Normal);
        *pThread = GCHandle.ToIntPtr(handle);
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRenderThreadUninit")]
    public static HRESULT SpRenderThreadUninit(IntPtr pThread)
    {
        if (pThread == IntPtr.Zero)
            return HRESULT.E_INVALIDARG;

        var handle = GCHandle.FromIntPtr(pThread);
        if (handle.Target is RenderThread thread)
            thread.Stop();
        handle.Free();
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpBufferOpen")]
    public static HRESULT SpBufferOpen(BufferInfo* phdrData, void* pvData)
    {
        if (phdrData == null)
            return HRESULT.E_INVALIDARG;

        if (!ContextRegistry.TryGet(phdrData->idContextDest, out ContextRegistry.Entry entry) || entry.Callback == IntPtr.Zero)
            return HRESULT.E_FAIL;

        var fn = (delegate* unmanaged<IntPtr, uint, BufferInfo*, void*, int>)entry.Callback;
        fn(entry.CallbackData, phdrData->idContextDest.value, phdrData, pvData);
        return HRESULT.S_OK;
    }
}
