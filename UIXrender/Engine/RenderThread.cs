using System;
using System.Threading;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Engine;

// Backs SpRenderThreadInit/SpRenderThreadUninit: spins up a genuine OS thread the CLR
// didn't create, which invokes the registered buffer-processing callback once (with
// synthetic data, proving the round trip) and then waits for shutdown -- this is the
// exact mechanism every Remote*/Local*Callback class in UIX.RenderApi depends on via
// LocalChannel.Connect() -> SpRenderThreadInit. See logs/UIXrender/EngineCore.md. Not
// yet the full message-pump loop (that's the larger "Engine core" subsystem, still
// ahead) -- this proves the interop mechanism, not the whole dispatch engine.
internal sealed unsafe class RenderThread
{
    private readonly ContextID _contextId;
    private readonly IntPtr _callback;
    private readonly IntPtr _callbackData;
    private readonly ManualResetEventSlim _shutdown = new(false);
    private readonly Thread _thread;

    private RenderThread(ContextID contextId, IntPtr callback, IntPtr callbackData)
    {
        _contextId = contextId;
        _callback = callback;
        _callbackData = callbackData;
        _thread = new Thread(Run) { IsBackground = true, Name = $"UIXrender.RenderThread[{contextId.value}]" };
    }

    public static RenderThread Start(in InitArgs args)
    {
        var thread = new RenderThread(args.idContext, args.pfnProcessBuffer, args.pvProcessData);
        ContextRegistry.Register(args.idContext, args.pfnProcessBuffer, args.pvProcessData);
        thread._thread.Start();
        return thread;
    }

    private void Run()
    {
        if (_callback != IntPtr.Zero)
        {
            var info = new BufferInfo
            {
                idContextSrc = _contextId,
                idContextDest = _contextId,
                idBuffer = RENDERHANDLE.NULL,
                nFlags = 0,
                cbSizeBuffer = 0,
            };
            var fn = (delegate* unmanaged<IntPtr, uint, BufferInfo*, void*, int>)_callback;
            fn(_callbackData, _contextId.value, &info, null);
        }

        _shutdown.Wait();
    }

    public void Stop()
    {
        ContextRegistry.Unregister(_contextId);
        _shutdown.Set();
        _thread.Join();
        _shutdown.Dispose();
    }
}
