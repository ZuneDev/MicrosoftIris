using System;
using System.Threading;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
// Disambiguate from Microsoft.Iris.Render.WindowOptions (enum) and this class's Window property.
using SilkWindowOptions = Silk.NET.Windowing.WindowOptions;
using SilkWindowFactory = Silk.NET.Windowing.Window;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// In-process, OpenGL-backed render engine. Owns a single Silk.NET window and its GL
    /// context, pumps native events on demand and renders the visual tree each frame.
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

            m_silkWindow.Render += OnRender;
            m_silkWindow.Resize += _ => { m_window.RaiseResize(); WakeWaitLoop(); };
            m_silkWindow.Move += _ => { m_window.RaiseMove(); WakeWaitLoop(); };
            m_silkWindow.Closing += () => { m_window.RaiseClose(); WakeWaitLoop(); };
            m_silkWindow.FocusChanged += active => { m_window.RaiseActivation(active); WakeWaitLoop(); };
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
            m_session.GraphicsDevice = new GLGraphicsDevice(m_gl, m_quality, RenderNow);
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

        private void OnRender(double deltaSeconds)
        {
            if (m_renderer == null)
                return;
            m_renderer.BeginFrame(m_window.Width, m_window.Height, m_window.BackgroundColor);
            m_window.Root.Render(m_renderer, Matrix4X4<float>.Identity, 1f);
        }

        private void RenderNow()
        {
            if (m_window.IsLoaded && !m_silkWindow.IsClosing)
                m_silkWindow.DoRender();
        }

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

        public void FlushBatch() => RenderNow();

        public bool IsGraphicsDeviceAvailable(GraphicsDeviceType type, bool fFilterRecommended)
            => type == GraphicsDeviceType.Direct3D9 || type == GraphicsDeviceType.Gdi;

        public bool IsSoundDeviceAvailable(SoundDeviceType type)
            => type == SoundDeviceType.WaveAudio || type == SoundDeviceType.DirectSound8;

        public void Dispose()
        {
            m_inputTranslator?.Dispose();
            m_renderer?.Dispose();
            m_session.Dispose();
            if (!m_silkWindow.IsClosing)
                m_silkWindow.Close();
            m_silkWindow.Dispose();
            m_windowLoaded.Dispose();
            m_wakeEvent.Dispose();
        }
    }
}
