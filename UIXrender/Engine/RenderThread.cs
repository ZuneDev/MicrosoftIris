using System;
using System.Threading;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Engine;

// Backs EngineService.StartRenderThread: spins up a genuine OS thread the CLR didn't
// create, which invokes the registered handler once (with synthetic data, proving the
// round trip) and then waits for shutdown -- this is the exact mechanism every
// Remote*/Local*Callback class in UIX.RenderApi depends on via LocalChannel.Connect().
// See logs/UIXrender/EngineCore.md. Not yet the full message-pump loop (that's the
// larger "Engine core" subsystem, still ahead) -- this proves the interop mechanism,
// not the whole dispatch engine. No pointers here at all now -- callers (native or
// managed) each supply an ordinary BufferReceivedHandler delegate.
internal sealed class RenderThread : IRenderThreadHandle
{
    private readonly BufferReceivedHandler _handler;
    private readonly ManualResetEventSlim _shutdown = new(false);
    private readonly Thread _thread;

    public ContextID ContextId { get; }

    private RenderThread(ContextID contextId, BufferReceivedHandler handler)
    {
        ContextId = contextId;
        _handler = handler;
        _thread = new Thread(Run) { IsBackground = true, Name = $"UIXrender.RenderThread[{contextId.value}]" };
    }

    public static RenderThread Start(ContextID contextId, BufferReceivedHandler handler)
    {
        var thread = new RenderThread(contextId, handler);
        ContextRegistry.Register(contextId, handler);
        thread._thread.Start();
        return thread;
    }

    private void Run()
    {
        _handler?.Invoke(ContextId, RENDERHANDLE.NULL, default, ReadOnlySpan<byte>.Empty);
        _shutdown.Wait();
    }

    public void Dispose()
    {
        ContextRegistry.Unregister(ContextId);
        _shutdown.Set();
        _thread.Join();
        _shutdown.Dispose();
    }
}
