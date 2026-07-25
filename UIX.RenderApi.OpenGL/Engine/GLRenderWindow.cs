using System;
using Microsoft.Iris.Input;
using Silk.NET.Maths;
using SilkWindow = Silk.NET.Windowing.IWindow;
using SilkWindowState = Silk.NET.Windowing.WindowState;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// <see cref="IRenderWindow"/> implemented over a Silk.NET window. Owns the root
    /// visual container and re-raises the Iris window events from Silk callbacks.
    /// </summary>
    public sealed class GLRenderWindow : IRenderWindow
    {
        private readonly SilkWindow m_window;
        private readonly GLVisualContainer m_root;
        private Size m_initialClientSize = new Size(1024, 768);

        public GLRenderWindow(SilkWindow window, GLRenderSession session)
        {
            m_window = window;
            m_root = new GLVisualContainer(session, null!, isRoot: true);
            m_root.RegisterUsage(this);
        }

        internal GLVisualContainer Root => m_root;

        // ---- Geometry ------------------------------------------------------------
        public int Left => m_window.Position.X;
        public int Top => m_window.Position.Y;
        public int Right => m_window.Position.X + m_window.Size.X;
        public int Bottom => m_window.Position.Y + m_window.Size.Y;
        public int Width => m_window.Size.X;
        public int Height => m_window.Size.Y;

        public HWND WindowHandle => new HWND(m_window.Native?.Win32?.Hwnd ?? IntPtr.Zero);

        public Size ClientSize
        {
            get => new Size(m_window.Size.X, m_window.Size.Y);
            set => m_window.Size = new Vector2D<int>(value.Width, value.Height);
        }

        public Size InitialClientSize
        {
            get => m_initialClientSize;
            set => m_initialClientSize = value;
        }

        public FormPlacement InitialPlacement
        {
            set { /* honored at creation time via InitialClientSize/Position */ }
        }

        public FormPlacement FinalPlacement => new FormPlacement
        {
            NormalPosition = new Rectangle(Left, Top, Width, Height),
            MaximizedLocation = Point.Zero,
            ShowState = (uint)m_window.WindowState,
        };

        public int MinResizeWidth { get; set; }
        public int MaxResizeWidth { get; set; }

        public Point Position
        {
            get => new Point(m_window.Position.X, m_window.Position.Y);
            set => m_window.Position = new Vector2D<int>(value.X, value.Y);
        }

        public string Text
        {
            get => m_window.Title;
            set => m_window.Title = value ?? string.Empty;
        }

        public Cursor Cursor { get; set; } = Cursor.Default;
        public Cursor IdleCursor { get; set; } = Cursor.Default;

        public bool Visible
        {
            get => m_window.IsVisible;
            set => m_window.IsVisible = value;
        }

        public bool IsLoaded { get; internal set; }
        public ColorF BackgroundColor { get; set; } = new ColorF(0f, 0f, 0f, 1f);
        public bool EnableExternalDragDrop { get; set; }
        public bool IsDragInProgress { get; set; }
        public IDisplay? CurrentDisplay { get; set; }
        public bool FullScreenExclusive { get; set; }
        public bool ActivationState { get; internal set; } = true;

        public WindowState WindowState
        {
            get => m_window.WindowState switch
            {
                SilkWindowState.Minimized => WindowState.Minimized,
                SilkWindowState.Maximized => WindowState.Maximized,
                _ => WindowState.Normal,
            };
            set => m_window.WindowState = value switch
            {
                WindowState.Minimized => SilkWindowState.Minimized,
                WindowState.Maximized => SilkWindowState.Maximized,
                _ => SilkWindowState.Normal,
            };
        }

        public FormStyleInfo Styles { get; set; }
        public HWND AppNotifyWindow { set { /* native app-notify sink; unused in-process */ } }

        public IVisualContainer VisualRoot => m_root;

        // ---- Methods -------------------------------------------------------------
        public void Initialize()
        {
            // Creates the OS window + GL context; Silk raises Load, which the engine
            // handles to build the GL device and then re-raises LoadEvent.
            m_window.Size = new Vector2D<int>(m_initialClientSize.Width, m_initialClientSize.Height);
            m_window.Initialize();
        }

        public void SetIcon(string sModuleName, uint nResourceID, IconFlags nOptions) { /* TODO(stage 3): load icon */ }
        public void SetEdgeImages(bool fActiveEdges, ShadowEdgePart[] edges) { /* TODO(stage 3): window shadow edges */ }
        public void SetWindowOptions(WindowOptions options, bool enable) { /* TODO(stage 3): map to Silk window flags */ }
        public void SetMouseIdleOptions(Size sizeMouseIdleTolerance, uint nMouseIdleDelay) { }
        public void SetCapture(IRawInputSite captureSite, bool state) { }
        public void SetDragDropResult(uint nDragOverResult, uint nDragDropResult) { }

        public void ClientToScreen(ref Point point)
        {
            point = new Point(point.X + Left, point.Y + Top);
        }

        public void ScreenToClient(ref Point point)
        {
            point = new Point(point.X - Left, point.Y - Top);
        }

        public void ForceMouseIdle(bool fIdle) => MouseIdleEvent?.Invoke(fIdle);
        public void LockMouseActive(bool fActive) { }
        public void RefreshHitTarget() { }
        public void TakeFocus() { }
        public void TakeForeground(bool fForce) { }
        public void BringToTop() { }
        public void Restore() => WindowState = WindowState.Normal;
        public void TemporarilyExitExclusiveMode() { }

        public void Close(FormCloseReason fcrCloseReason)
        {
            CloseRequestEvent?.Invoke();
            m_window.Close();
        }

        public IHwndHostWindow CreateHwndHostWindow() => new GLHwndHostWindow();

        // ---- Events --------------------------------------------------------------
        public event LocationChangedHandler? LocationChangedEvent;
        public event SizeChangedHandler? SizeChangedEvent;
        public event MonitorChangedHandler? MonitorChangedEvent;
        public event WindowStateChangedHandler? WindowStateChangedEvent;
        public event SysCommandHandler? SysCommandEvent;
        public event MouseIdleHandler? MouseIdleEvent;
        public event ShowHandler? ShowEvent;
        public event ActivationChangeHandler? ActivationChangeEvent;
        public event SessionActivateHandler? SessionActivateEvent;
        public event SessionConnectHandler? SessionConnectEvent;
        public event SetFocusHandler? SetFocusEvent;
        public event LoadHandler? LoadEvent;
        public event CloseHandler? CloseEvent;
        public event CloseRequestHandler? CloseRequestEvent;
        public event ForwardMessageHandler? ForwardMessageEvent;

        // Raisers invoked by the engine as it pumps the Silk window.
        internal void RaiseLoad()
        {
            IsLoaded = true;
            LoadEvent?.Invoke();
            ShowEvent?.Invoke(true, true);
        }

        internal void RaiseResize() => SizeChangedEvent?.Invoke();
        internal void RaiseMove() => LocationChangedEvent?.Invoke(Position);
        internal void RaiseClose() => CloseEvent?.Invoke();

        internal void RaiseActivation(bool active)
        {
            ActivationState = active;
            ActivationChangeEvent?.Invoke();
        }
    }
}
