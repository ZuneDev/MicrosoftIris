using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Iris.Render.Subsystems.Os;

// The render engine's message pump, backing SpPeekMessage/SpWaitMessage/SpInvoke.
// Backend-agnostic: a per-process message queue plus a wait handle -- no windowing or GPU
// dependency. This is exactly and only what the render loop uses the pump for (verified
// from UIX.RenderApi/.../Internal/RenderEngine.cs):
//
//   WaitForWork(timeout)   -> Wait(timeout): block until work is posted or the timeout.
//   ProcessNativeEvents()  -> Peek(): drain queued work, report whether any ran.
//   InterThreadWake()      -> PostWake(): unblock a blocked Wait from another thread.
//
// Deferred work (an async SpInvoke, a deferred IME message) is posted as an Action and
// runs on the pump-owning thread the next time it Peeks -- which is the whole point of a
// "deferred invoke": marshal the call onto the render thread instead of running it on the
// caller's.
//
// A pluggable IWindowMessageSource lets a host that owns an OS window (or a separate,
// optional windowing backend that never ships in UIXrender core) feed real window/input
// messages into the pump during Peek, without this file referencing any windowing library.
internal static class MessagePump
{
    private static readonly ConcurrentQueue<Action> s_queue = new();
    private static readonly ManualResetEventSlim s_wake = new(false);
    private static volatile IWindowMessageSource s_windowSource;

    public static void SetWindowSource(IWindowMessageSource source) => s_windowSource = source;

    // Enqueue work to run on the pump thread, and wake a blocked Wait.
    public static void Post(Action work)
    {
        if (work != null)
            s_queue.Enqueue(work);
        s_wake.Set();
    }

    // Wake a blocked Wait without queuing work (InterThreadWake).
    public static void PostWake() => s_wake.Set();

    // Drains and runs all queued work on the calling (pump-owning) thread, letting a
    // registered window source inject OS messages first. Returns true if any work ran.
    public static bool Peek()
    {
        bool didWork = false;

        s_windowSource?.Pump();

        while (s_queue.TryDequeue(out Action work))
        {
            work?.Invoke();
            didWork = true;
        }

        // Reset only when the queue is genuinely empty, then re-check to close the race
        // with a Post that enqueued between the drain and the reset (lost-wakeup guard).
        if (s_queue.IsEmpty)
        {
            s_wake.Reset();
            if (!s_queue.IsEmpty)
                s_wake.Set();
        }

        return didWork;
    }

    // Blocks until work is pending or the timeout elapses. Returns immediately if work is
    // already queued, so a Post that raced ahead of the Wait is never missed.
    public static void Wait(uint timeoutMs)
    {
        if (!s_queue.IsEmpty)
            return;

        s_wake.Wait(timeoutMs > int.MaxValue ? int.MaxValue : (int)timeoutMs);
    }
}

// The seam a windowing backend implements to feed OS window/input messages into the pump.
// Implementations translate their native events and MessagePump.Post them; Pump() is
// called once per Peek and must not block. Kept dependency-free on purpose -- UIXrender
// core references only this interface, never a concrete windowing library.
public interface IWindowMessageSource
{
    void Pump();
}
