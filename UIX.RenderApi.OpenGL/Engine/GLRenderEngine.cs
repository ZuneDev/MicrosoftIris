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
            m_silkWindow.Resize += _ => m_window.RaiseResize();
            m_silkWindow.Move += _ => m_window.RaiseMove();
            m_silkWindow.Closing += m_window.RaiseClose;
            m_silkWindow.FocusChanged += m_window.RaiseActivation;
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
            m_inputTranslator = new GLInputTranslator(m_inputContext, (GLInputSystem)m_session.InputSystem, m_window);

            m_windowLoaded.Set();
            m_window.RaiseLoad();
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

        public bool ProcessNativeEvents()
        {
            m_silkWindow.DoEvents();
            return !m_silkWindow.IsClosing;
        }

        public void WaitForWork(uint nTimeoutInMsecs)
        {
            // Cooperative wait: return promptly if another thread requested a wake.
            uint waited = 0;
            const uint slice = 5;
            while (waited < nTimeoutInMsecs && !m_wakeRequested)
            {
                Thread.Sleep((int)Math.Min(slice, nTimeoutInMsecs - waited));
                waited += slice;
            }
            m_wakeRequested = false;
        }

        public void InterThreadWake() => m_wakeRequested = true;

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
        }
    }
}
