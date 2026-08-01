using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Iris.Render.OpenGL.Animation;
using Microsoft.Iris.Render.OpenGL.Rendering;
using Microsoft.Iris.Render.OpenGL.Sound;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
// Disambiguate from Microsoft.Iris.Render.WindowOptions (enum) and this class's Window property.
using SilkWindowOptions = Silk.NET.Windowing.WindowOptions;
using SilkWindowFactory = Silk.NET.Windowing.Window;

namespace Microsoft.Iris.Render.OpenGL.Engine
{
    /// <summary>
    /// In-process, OpenGL-backed render engine. Owns a single Silk.NET window; native
    /// events are pumped on demand on the app/UI thread, while a dedicated render thread
    /// owns the GL context and paints/advances animation independently, so a busy app
    /// thread (a long dispatcher callback, a blocking RPC) doesn't stall painting with it
    /// -- see logs/UIX.RenderApi.OpenGL/Implementation.md for the design writeup.
    ///
    /// The render thread is event-driven, not a continuous loop: it sits idle (touching
    /// no shared state, holding no lock) until something actually needs a frame -- an
    /// explicit invalidation (FlushBatch/RenderNowIfPossible, the same two hooks the
    /// original single-threaded engine used) or a playing animation asking to keep going
    /// after this frame. An earlier version of this engine rendered continuously at
    /// vsync regardless of activity, which meant the render thread re-acquired
    /// GLRenderSession.SyncRoot dozens of times a second even while the screen was
    /// completely static -- a lock convoy against every single app-thread property/tree
    /// mutation, badly regressing responsiveness (reported: many-millisecond page loads,
    /// multi-second "Not Responding" on actions that touched a lot of UI). See the
    /// dated log entries in logs/UIX.RenderApi.OpenGL/Implementation.md for both the
    /// original mistake and this fix.
    /// </summary>
    public sealed class GLRenderEngine : IRenderEngine
    {
        private readonly IRenderHost m_host;
        private readonly IWindow m_silkWindow;
        private readonly GLRenderSession m_session;
        private readonly GLRenderWindow m_window;

        private GL? m_gl;
        private SceneRenderer? m_renderer;
        private GLDisplayManager? m_displayManager;
        private IInputContext? m_inputContext;
        private GLInputTranslator? m_inputTranslator;
        private Thread? m_renderThread;
        private volatile bool m_shuttingDown;

        // Starts signaled so the render thread draws once as soon as it comes up
        // (there's real content to show only once OnLoad/window Initialize complete,
        // but drawing an early blank frame or two is harmless). Reset at the top of
        // each loop iteration; re-Set by FlushBatch/RenderNowIfPossible, a handful of
        // window events, or by the loop itself when a frame just drawn still has a
        // playing animation. The 250ms Wait timeout is a low-cost safety net, not the
        // primary path -- it self-heals any invalidation hook this design missed
        // without reintroducing a tight loop (4Hz is imperceptible as background
        // polling, versus the 60Hz lock contention this replaces).
        private readonly ManualResetEventSlim m_renderRequested = new(true);
        private const int RenderFallbackPollMs = 250;

        private GraphicsRenderingQuality m_quality;
        private SoundDeviceType m_soundType;
        private volatile bool m_wakeRequested;

        private readonly ManualResetEventSlim m_windowLoaded = new(false);

        public GLRenderEngine(EngineInfo engineInfo, IRenderHost renderHost)
        {
            m_host = renderHost;

            var options = SilkWindowOptions.Default;
            options.Title = "Iris";
            options.API = new GraphicsAPI(
                ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(3, 3));
            options.IsVisible = true;
            options.ShouldSwapAutomatically = true;
            m_silkWindow = SilkWindowFactory.Create(options);

            m_session = new GLRenderSession();
            m_window = new GLRenderWindow(m_silkWindow, m_session);

            m_silkWindow.Load += OnLoad;
            m_silkWindow.Initialize();
            m_windowLoaded.Wait();

            m_silkWindow.Resize += _ => { m_window.RaiseResize(); WakeWaitLoop(); RequestRenderFrame(); };
            m_silkWindow.Move += _ => { m_window.RaiseMove(); WakeWaitLoop(); };
            m_silkWindow.Closing += () => { m_window.RaiseClose(); WakeWaitLoop(); };
            m_silkWindow.FocusChanged += active => { m_window.RaiseActivation(active); WakeWaitLoop(); RequestRenderFrame(); };

            // Hand the GL context off to a dedicated render thread. Window creation and
            // native event pumping (ProcessNativeEvents/WaitForWork's DoEvents calls)
            // stay on this thread: the project uses Silk.NET.Windowing.Sdl, and SDL's
            // documented threading rule -- OS-enforced on macOS specifically -- is that
            // window creation and event pumping must stay on the thread that initialized
            // the video subsystem. That can't be verified on an actual Mac from this
            // environment, so it's called out here rather than silently assumed.
            m_silkWindow.ClearContext();
            m_renderThread = new Thread(RenderThreadMain) { IsBackground = true, Name = "GLRenderThread" };
            m_renderThread.Start();
        }

        public IRenderSession Session => m_session;
        public IRenderWindow Window => m_window;
        public IDisplayManager DisplayManager => m_displayManager ?? throw new InvalidOperationException("Engine not initialized");

        public void Initialize(
            GraphicsDeviceType typeGraphics,
            GraphicsRenderingQuality renderingQuality,
            SoundDeviceType typeSound)
        {
            m_quality = renderingQuality;
            m_soundType = typeSound;
            // Window/GL creation is completed when GLRenderWindow.Initialize() runs and
            // Silk raises Load (see OnLoad); we only latch the requested settings here.
        }

        private void OnLoad()
        {
            m_gl = m_silkWindow.CreateOpenGL();
            m_renderer = new SceneRenderer(m_gl);
            m_displayManager = new GLDisplayManager(m_silkWindow);
            m_session.GraphicsDevice = new GLGraphicsDevice(m_gl, m_quality,
                // Once the render thread takes the GL context (right after this method
                // returns -- see the constructor), nothing can draw inline here on
                // whatever thread calls RenderNowIfPossible anymore -- wake the render
                // thread instead and let it draw on its own thread. This also matches
                // the original protocol's own semantics for "RenderNowIfPossible" better
                // than a synchronous inline draw would: RemoteNtDevice.
                // SendRenderNowIfPossible fires an async, no-reply message at the
                // (separately-threaded) native engine and never waits for a frame.
                renderNow: RequestRenderFrame);
            m_session.SoundDevice = new GLSoundDevice(m_soundType);
            m_inputContext = m_silkWindow.CreateInput();
            m_inputTranslator = new GLInputTranslator(m_inputContext, (GLInputSystem)m_session.InputSystem, m_window, WakeWaitLoop);

            m_windowLoaded.Set();
            // Do NOT raise Load here: this runs during the engine constructor, before
            // Microsoft.Iris.Session.Form exists to subscribe to IRenderWindow.LoadEvent
            // (Form's ctor needs session.GetRenderWindow(), which needs the engine to
            // already exist). Raising it here means Form's OnRenderWindowLoad -- and the
            // whole UIForm.OnLoad -> OnInitialize -> Zone/markup chain -- silently never
            // fires. GLRenderWindow.Initialize() (invoked from Form.InitializeWindow,
            // which runs after Form has subscribed) raises it at the right time instead.
        }

        /// <summary>
        /// The render thread's own loop: paints and advances animation independently of
        /// whatever the app/UI thread's dispatcher is doing, but only when something
        /// actually needs a frame -- see m_renderRequested's doc comment. Replaces Silk's
        /// own Render event/DoRender pump entirely (this engine no longer uses either --
        /// see the constructor's ClearContext/MakeCurrent handoff).
        /// </summary>
        private void RenderThreadMain()
        {
            m_silkWindow.MakeCurrent();
            var clock = Stopwatch.StartNew();
            double last = 0;

            while (!m_shuttingDown && !m_silkWindow.IsClosing)
            {
                m_renderRequested.Wait(RenderFallbackPollMs);
                m_renderRequested.Reset();
                if (m_shuttingDown || m_silkWindow.IsClosing)
                    break;

                double now = clock.Elapsed.TotalSeconds;
                bool keepAnimating = DrawFrame(now - last);
                last = now;
                m_silkWindow.SwapBuffers(); // blocks until the next vsync (VSync=true)

                // Still mid-animation: go again next iteration without waiting for a
                // fresh external invalidation, so playback stays smooth. Nothing sets
                // this once animations finish, so the thread goes back to idle/blocked
                // above rather than continuing to loop.
                if (keepAnimating)
                    m_renderRequested.Set();
            }

            // Let the main thread reclaim the context for teardown (see Dispose).
            m_silkWindow.ClearContext();
        }

        // TEMPORARY diagnostic tracing (logs/UIX.RenderApi.OpenGL/Implementation.md,
        // 2026-07-31 responsiveness investigation): same ZUNE_PERFTRACE gate as
        // Dispatcher.MainLoop, so the locked tree-walk, the unlocked GL flush, and
        // app-side dispatch/script execution all show up in one comparable log. Remove
        // once the actual bottleneck is confirmed.
        private static readonly bool PerfTraceEnabled = Environment.GetEnvironmentVariable("ZUNE_PERFTRACE") == "1";
        private const long PerfTraceThresholdMs = 8;

        /// <returns>Whether a playing animation wants another frame right after this one.</returns>
        private bool DrawFrame(double deltaSeconds)
        {
            if (m_renderer == null || !m_window.IsLoaded)
                return false;

            bool keepAnimating;
            var sw = PerfTraceEnabled ? Stopwatch.StartNew() : null;

            // Locked only for the fast part: advancing animation and walking the tree
            // just records draw commands (matrix math, property reads -- see
            // SceneRenderer's class doc comment), no GL calls and no image decode. This
            // used to hold the lock for the whole frame including GL submission and
            // GLImage.EnsureUploaded's potential synchronous decode, which meant a single
            // slow-to-load image blocked every app-thread scene mutation for the decode's
            // duration -- see logs/UIX.RenderApi.OpenGL/Implementation.md for the
            // regression this caused and why it's split this way now.
            lock (m_session.SyncRoot)
            {
                // Nothing else in this in-process GL engine ever called
                // IAnimationSystem.PulseTimeAdvance -- the original native engine drove
                // that pulse itself (outside managed code, via the remote/message
                // protocol this backend doesn't use), so every GLKeyframeAnimation sat
                // forever at its t=0 keyframe once played. Drive it here with real
                // elapsed time instead. Clamp to avoid a pulse following a long idle gap
                // (e.g. the render thread was idle for a while between frames, or the
                // user took a while to click something) jumping a freshly-started
                // animation straight to its end state.
                var animMs = (int)(Math.Min(deltaSeconds, 0.1) * 1000);
                var animSystem = (GLAnimationSystem)m_session.AnimationSystem;
                animSystem.PulseTimeAdvance(animMs);

                m_renderer.BeginFrame(m_window.Width, m_window.Height, m_window.BackgroundColor);
                m_window.Root.Render(m_renderer, Matrix4X4<float>.Identity, 1f);

                keepAnimating = animSystem.HasPlayingAnimations;
            }

            if (sw != null && sw.ElapsedMilliseconds >= PerfTraceThresholdMs)
                Console.Error.WriteLine($"[PERFTRACE] DrawFrame locked section: {sw.ElapsedMilliseconds}ms");

            // Unlocked: the actual GL submission, including any first-time image
            // upload/decode. Nothing here touches GLRenderSession.SyncRoot.
            sw?.Restart();
            m_renderer.Flush();
            if (sw != null && sw.ElapsedMilliseconds >= PerfTraceThresholdMs)
                Console.Error.WriteLine($"[PERFTRACE] SceneRenderer.Flush (unlocked): {sw.ElapsedMilliseconds}ms");

            return keepAnimating;
        }

        /// <summary>
        /// Wakes the render thread to draw a frame soon. Cheap and non-blocking --
        /// safe to call from any thread, any time (including before the render thread
        /// exists yet, since m_renderRequested starts pre-signaled).
        /// </summary>
        private void RequestRenderFrame() => m_renderRequested.Set();

        private bool m_didWork = true;
        public bool ProcessNativeEvents()
        {
            m_silkWindow.DoEvents();
            if (!m_didWork)
                return false;

            m_didWork = false;
            return true;
        }

        private readonly ManualResetEventSlim m_wakeEvent = new(false);

        public void WaitForWork(uint nTimeoutInMsecs)
        {
            // There's no native message queue to block on here (in-process GL engine),
            // so approximate the original's SpWaitMessage: block until WakeWaitLoop is
            // called (cross-thread via InterThreadWake, or same-thread by a native
            // window/input event that DoEvents() just pumped -- see below) or the
            // timeout elapses, polling Silk's event pump on a short cadence in between
            // so window/input events aren't starved for the whole wait. uint.MaxValue is
            // TimeoutManager's "no pending timeout" sentinel (see
            // TimeoutManager.NextTimeoutMillis) -- treat it as "wait until woken".
            //
            // nTimeoutInMsecs can legitimately be very large (a distant scheduled
            // timeout, e.g. >100s) when nothing is due soon. Silk's input/window
            // callbacks run synchronously inside DoEvents(), so GLInputTranslator and
            // the window-event lambdas above call WakeWaitLoop() as soon as they hand
            // work to the dispatcher's queues -- that flips m_wakeRequested, which the
            // loop condition below observes on its very next check. Without that, a
            // mouse click or keypress during a long wait would sit queued but unseen by
            // the dispatcher until the full timeout elapsed, making the UI look hung.
            const int pollSliceMs = 15;
            long deadline = nTimeoutInMsecs >= int.MaxValue
                ? long.MaxValue
                : Environment.TickCount64 + nTimeoutInMsecs;

            while (!m_wakeRequested && Environment.TickCount64 < deadline)
            {
                int waitMs = deadline == long.MaxValue
                    ? pollSliceMs
                    : (int)Math.Min(pollSliceMs, Math.Max(0, deadline - Environment.TickCount64));
                m_wakeEvent.Wait(waitMs);
                m_wakeEvent.Reset();
                m_silkWindow.DoEvents();
            }
            m_wakeRequested = false;

            // A wait just completed (woken, timed out, or events were pumped above) --
            // let the next RPC-priority ProcessNativeEvents call report "did work" once,
            // so anything newly queued gets processed before the loop tries to sleep again.
            m_didWork = true;
        }

        public void InterThreadWake() => WakeWaitLoop();

        /// <summary>
        /// Ends a pending <see cref="WaitForWork"/> early. Called both cross-thread (the
        /// dispatcher's <c>InterThreadWake</c> contract) and same-thread, from within
        /// <see cref="ProcessNativeEvents"/>'s Silk callbacks, whenever a native event
        /// just handed the dispatcher new work.
        /// </summary>
        private void WakeWaitLoop()
        {
            m_wakeRequested = true;
            m_wakeEvent.Set();
        }

        // Was RenderNow() (an inline GL draw on whatever thread called this). The GL
        // context lives only on the render thread now, so this wakes it instead of
        // drawing inline -- see RequestRenderFrame's doc comment.
        public void FlushBatch() => RequestRenderFrame();

        public bool IsGraphicsDeviceAvailable(GraphicsDeviceType type, bool fFilterRecommended)
            => type == GraphicsDeviceType.Direct3D9 || type == GraphicsDeviceType.Gdi;

        public bool IsSoundDeviceAvailable(SoundDeviceType type)
            => type == SoundDeviceType.WaveAudio || type == SoundDeviceType.DirectSound8;

        public void Dispose()
        {
            // Stop the render thread and let it release the GL context (see
            // RenderThreadMain's trailing ClearContext) before this thread touches any
            // GL-owned resource below. The thread may be idle-waiting on
            // m_renderRequested (up to RenderFallbackPollMs) rather than mid-frame, so
            // wake it explicitly instead of relying on that timeout to notice
            // m_shuttingDown.
            m_shuttingDown = true;
            m_renderRequested.Set();
            m_renderThread?.Join();
            m_silkWindow.MakeCurrent();

            m_inputTranslator?.Dispose();
            m_renderer?.Dispose();
            m_session.Dispose();
            if (!m_silkWindow.IsClosing)
                m_silkWindow.Close();
            m_silkWindow.Dispose();
            m_windowLoaded.Dispose();
            m_wakeEvent.Dispose();
            m_renderRequested.Dispose();
        }
    }
}
