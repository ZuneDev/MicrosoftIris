// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.Form
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Input;
using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.UI;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Iris.Session
{
    [SuppressUnmanagedCodeSecurity]
    internal class Form : IModalSession
    {
        private UISession m_session;
        private UIZone m_zone;
        private IRenderWindow m_renderWindow;
        private bool m_fCloseRequested;
        private bool m_fClosing;
        private Size m_sizeWindow;
        private bool _refreshFocusRequestedFlag;
        private readonly SimpleCallback _processRefreshFocus;
        private SmartMap m_mapShutdownHooks;
        private ushort m_nextShutdownHookId;

        public Form(UISession session)
        {
            m_session = session;
            m_renderWindow = session.GetRenderWindow();
            InternalWindow.LoadEvent += new LoadHandler(OnRenderWindowLoad);
            InternalWindow.ShowEvent += new ShowHandler(OnShow);
            InternalWindow.CloseRequestEvent += new CloseRequestHandler(OnRenderWindowCloseRequested);
            InternalWindow.CloseEvent += new CloseHandler(OnRenderWindowClose);
            InternalWindow.MouseIdleEvent += new MouseIdleHandler(OnRenderWindowMouseIdle);
            InternalWindow.SizeChangedEvent += new SizeChangedHandler(OnRenderWindowSizeChanged);
            InternalWindow.SysCommandEvent += new SysCommandHandler(OnRenderWindowSysCommand);
            InternalWindow.SetFocusEvent += new SetFocusHandler(OnRenderWindowSetFocus);
            InternalWindow.WindowStateChangedEvent += new WindowStateChangedHandler(OnWindowStateChanged);
            InternalWindow.LocationChangedEvent += new Microsoft.Iris.Render.LocationChangedHandler(OnLocationChanged);
            InternalWindow.MonitorChangedEvent += new MonitorChangedHandler(OnMonitorChanged);
            InternalWindow.ActivationChangeEvent += new ActivationChangeHandler(OnActivationChange);
            InternalWindow.SessionConnectEvent += new SessionConnectHandler(OnSessionConnect);
            InternalWindow.BackgroundColor = new ColorF(byte.MaxValue, 0, 0, 0);
            session.RegisterHost(this);
            m_mapShutdownHooks = new SmartMap();
            m_nextShutdownHookId = 1;
            _processRefreshFocus = new SimpleCallback(ProcessRefreshFocus);
            DeferredCall.Post(DispatchPriority.Housekeeping, new SimpleCallback(InitializeWindow));
        }

        private void InitializeWindow() => InternalWindow.Initialize();

        private IRenderWindow InternalWindow => m_renderWindow;

        public FormPlacement FinalPlacement => InternalWindow.FinalPlacement;

        public FormPlacement InitialPlacement
        {
            set => InternalWindow.InitialPlacement = value;
        }

        public IntPtr __WindowHandle => InternalWindow.WindowHandle.h;

        public bool InvokeRequired => !UIDispatcher.IsUIThread;

        public UISession Session => m_session;

        public UIZone Zone => m_zone;

        internal void ClientToScreen(ref Point point)
        {
            Point point1 = new Point(point.X, point.Y);
            InternalWindow.ClientToScreen(ref point1);
            point.X = point1.X;
            point.Y = point1.Y;
        }

        internal IVisualContainer RootVisual => InternalWindow.VisualRoot;

        public Size ClientSize
        {
            get => new Size(InternalWindow.Width, InternalWindow.Height);
            set => InternalWindow.ClientSize = value;
        }

        public Size InitialClientSize
        {
            get
            {
                Size initialClientSize = InternalWindow.InitialClientSize;
                return new Size(initialClientSize.Width, initialClientSize.Height);
            }
            set => InternalWindow.InitialClientSize = value;
        }

        public Point Position
        {
            get => new Point(InternalWindow.Left, InternalWindow.Top);
            set => InternalWindow.Position = value;
        }

        public string Text
        {
            get => InternalWindow.Text;
            set => InternalWindow.Text = value;
        }

        public CursorID Cursor
        {
            get => InternalWindow.Cursor.CursorID;
            set => InternalWindow.Cursor = Input.Cursor.GetCursor(value);
        }

        public CursorID IdleCursor
        {
            get => InternalWindow.IdleCursor.CursorID;
            set => InternalWindow.IdleCursor = Input.Cursor.GetCursor(value);
        }

        public bool Visible
        {
            get => InternalWindow.Visible;
            set
            {
                if (m_zone != null)
                    m_zone.SetPhysicalVisible(value);
                InternalWindow.Visible = value;
            }
        }

        public bool ActivationState => InternalWindow.ActivationState;

        public void TakeFocus() => InternalWindow.TakeFocus();

        public Microsoft.Iris.WindowState WindowState
        {
            get => (Microsoft.Iris.WindowState)InternalWindow.WindowState;
            set => InternalWindow.WindowState = (Microsoft.Iris.Render.WindowState)value;
        }

        public FormStyleInfo Styles
        {
            get => InternalWindow.Styles;
            set => InternalWindow.Styles = value;
        }

        public bool IsClosing => m_fClosing;

        public IntPtr AppNotifyWindow
        {
            set => InternalWindow.AppNotifyWindow = new HWND(value);
        }

        public void BeginDynamicResize(Size sizeLargestExpectedPxl)
        {
        }

        public void EndDynamicResize()
        {
        }

        protected void SetWindowOptions(WindowOptions options, bool enable) => InternalWindow.SetWindowOptions(options, enable);

        protected void SetMouseIdleOptions(Size mouseIdleTolerance, uint mouseIdleDelay) => InternalWindow.SetMouseIdleOptions(mouseIdleTolerance, mouseIdleDelay);

        public bool IsDragInProgress
        {
            get => InternalWindow.IsDragInProgress;
            set => InternalWindow.IsDragInProgress = value;
        }

        internal void RefreshHitTarget() => InternalWindow.RefreshHitTarget();

        public event EventHandler NativeSetFocus;

        protected virtual void OnNativeSetFocus(object sender, Form.NativeSetFocusEventArgs args)
        {
            if (NativeSetFocus == null)
                return;
            NativeSetFocus(sender, args);
        }

        public bool SetDefaultKeyFocus()
        {
            bool flag = false;
            if (m_zone != null)
                flag = m_zone.OnInboundKeyNavigation(Direction.Next, RectangleF.Zero, true);
            return flag;
        }

        public void RequestClose(FormCloseReason nReason)
        {
            if (m_fClosing)
                return;
            if (m_fCloseRequested)
                return;
            try
            {
                m_fCloseRequested = true;
                OnCloseRequest(nReason);
            }
            finally
            {
                m_fCloseRequested = false;
            }
        }

        public void ForceClose() => ForceCloseWorker(FormCloseReason.ForcedClose);

        private void ForceCloseWorker(FormCloseReason nReason)
        {
            if (m_fClosing)
                return;
            m_fClosing = true;
            InternalWindow.Close(FormCloseReason.UserRequest);
        }

        private void OnRenderWindowClose()
        {
            m_fClosing = true;
            OnDestroy();
        }

        public void ForceMouseIdle(bool fIdle) => InternalWindow.ForceMouseIdle(fIdle);

        internal void SetCapture(IRawInputSite captureSite, bool state) => InternalWindow.SetCapture(captureSite, state);

        public void SetBackgroundColor(Color color) => InternalWindow.BackgroundColor = color.RenderConvert();

        public void SetIcon(string sModuleName, uint nResourceID, IconFlags nOptions) => InternalWindow.SetIcon(sModuleName, nResourceID, nOptions);

        public void SetShadowImages(bool fActiveEdges, UIImage[] images)
        {
            ShadowEdgePart[] edges = new ShadowEdgePart[4];
            for (int index = 0; index < 4; ++index)
            {
                string host;
                string identifier;
                Inset nineGrid;
                ShadowEdgeResourceFromImage(images[index], out host, out identifier, out nineGrid);
                edges[index].ModuleName = host;
                edges[index].ResourceName = identifier;
                edges[index].SplitPoints = nineGrid;
            }
            InternalWindow.SetEdgeImages(fActiveEdges, edges);
        }

        internal void ShadowEdgeResourceFromImage(
          UIImage uiimage,
          out string host,
          out string identifier,
          out Inset nineGrid)
        {
            host = null;
            identifier = null;
            nineGrid = new Inset();
            if (!(uiimage is UriImage uriImage))
                return;
            nineGrid = uriImage.NineGrid;
            string source = uriImage.Source;
            if (source == null)
                return;
            string hierarchicalPart;
            ResourceManager.ParseUri(source, out string _, out hierarchicalPart);
            DllResources.ParseResource(hierarchicalPart, out host, out identifier);
        }

        public bool EnableExternalDragDrop
        {
            get => InternalWindow.EnableExternalDragDrop;
            set => InternalWindow.EnableExternalDragDrop = value;
        }

        public void SetDragDropResult(uint nDragOverResult, uint nDragDropResult) => InternalWindow.SetDragDropResult(nDragOverResult, nDragDropResult);

        public void TemporarilyExitExclusiveMode() => InternalWindow.TemporarilyExitExclusiveMode();

        public void EnableShellShutdownHook(string hookName, EventHandler handler) => GetShutdownHookInfo(hookName, true).Handler += handler;

        protected virtual void OnLocationChanged(Point position)
        {
            Form.LocationChangedHandler locationChangedEvent = OnLocationChangedEvent;
            if (locationChangedEvent == null)
                return;
            Form.LocationChangedArgs args = new Form.LocationChangedArgs(position);
            locationChangedEvent(args);
        }

        public event Form.LocationChangedHandler OnLocationChangedEvent;

        protected virtual void OnSizeChanged()
        {
        }

        protected virtual void OnMonitorChanged()
        {
        }

        protected virtual void OnWindowStateChanged(bool fUnplanned)
        {
        }

        protected virtual void OnCloseRequest(FormCloseReason nReason) => ForceCloseWorker(nReason);

        protected virtual void OnDestroy()
        {
            if (m_renderWindow == null)
                return;
            InternalWindow.LoadEvent -= new LoadHandler(OnRenderWindowLoad);
            InternalWindow.ShowEvent -= new ShowHandler(OnShow);
            InternalWindow.CloseRequestEvent -= new CloseRequestHandler(OnRenderWindowCloseRequested);
            InternalWindow.CloseEvent -= new CloseHandler(OnRenderWindowClose);
            InternalWindow.MouseIdleEvent -= new MouseIdleHandler(OnRenderWindowMouseIdle);
            InternalWindow.SizeChangedEvent -= new SizeChangedHandler(OnRenderWindowSizeChanged);
            InternalWindow.SysCommandEvent -= new SysCommandHandler(OnRenderWindowSysCommand);
            InternalWindow.SetFocusEvent -= new SetFocusHandler(OnRenderWindowSetFocus);
            InternalWindow.WindowStateChangedEvent -= new WindowStateChangedHandler(OnWindowStateChanged);
            InternalWindow.LocationChangedEvent -= new Microsoft.Iris.Render.LocationChangedHandler(OnLocationChanged);
            InternalWindow.MonitorChangedEvent -= new MonitorChangedHandler(OnMonitorChanged);
            InternalWindow.ActivationChangeEvent -= new ActivationChangeHandler(OnActivationChange);
            InternalWindow.SessionConnectEvent -= new SessionConnectHandler(OnSessionConnect);
        }

        protected virtual void OnSysCommand(IntPtr uParam1, IntPtr uParam2)
        {
        }

        protected virtual void OnMouseIdle(bool fIdle)
        {
            if (m_session == null)
                return;
            UI.Environment.Instance.SetIsMouseActive(!fIdle);
        }

        internal virtual void OnShow(bool fShow, bool fFirstShow)
        {
        }

        public event EventHandler ActivationChange;

        protected virtual void OnActivationChange()
        {
            if (ActivationChange == null)
                return;
            ActivationChange(this, EventArgs.Empty);
        }

        protected virtual void OnLoad()
        {
        }

        private void OnSessionConnect(bool fIsConnected)
        {
            if (SessionConnect == null)
                return;
            SessionConnect(this, fIsConnected);
        }

        public event FormSessionConnectHandler SessionConnect;

        public IntPtr FireAccGetObject(int iFlags, int iObjectID) => OnAccGetObject(iFlags, iObjectID);

        protected virtual IntPtr OnAccGetObject(int iFlags, int iObjectID) => IntPtr.Zero;

        public void BringToTop() => InternalWindow.BringToTop();

        public void LockMouseActive(bool fLock) => InternalWindow.LockMouseActive(fLock);

        internal void ScreenToClient(ref Point point)
        {
            Point point1 = new Point(point.X, point.Y);
            InternalWindow.ScreenToClient(ref point1);
            point.X = point1.X;
            point.Y = point1.Y;
        }

        internal void NotifyWinEvent(int idEvent, int idObject, int idChild) => NotifyWinEvent(idEvent, __WindowHandle, idObject, idChild);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern void NotifyWinEvent(int idEvent, IntPtr hwnd, int idObject, int idChild);

        internal void AttachChildZone(UIZone newZone)
        {
            m_zone = newZone;
            m_zone.ResizeRootContainer(InitialClientSize);
            m_zone.SetPhysicalVisible(Visible);
        }

        private void OnRenderWindowSysCommand(IntPtr wParam, IntPtr lParam) => OnSysCommand(wParam, lParam);

        private void OnRenderWindowMouseIdle(bool fNewIdle) => OnMouseIdle(fNewIdle);

        private void OnRenderWindowCloseRequested() => RequestClose(FormCloseReason.RendererRequest);

        private void OnRenderWindowLoad()
        {
            if (m_fClosing)
                return;
            OnLoad();
        }

        private void OnRenderWindowSizeChanged()
        {
            Size clientSize = m_renderWindow.ClientSize;
            if (!(clientSize != m_sizeWindow))
                return;
            m_sizeWindow = clientSize;
            if (m_renderWindow.WindowState == Render.WindowState.Minimized)
                return;
            if (m_zone != null)
                m_zone.ResizeRootContainer(clientSize);
            OnSizeChanged();
            m_session.RequestUpdateView(true);
        }

        private void OnRenderWindowSetFocus(bool focused)
        {
            if (focused && !_refreshFocusRequestedFlag)
            {
                DeferredCall.Post(DispatchPriority.Idle, _processRefreshFocus);
                _refreshFocusRequestedFlag = true;
            }
            m_session.InputManager.Keyboard.Reset();
        }

        private void ProcessRefreshFocus()
        {
            if (!_refreshFocusRequestedFlag)
                return;
            _refreshFocusRequestedFlag = false;
            if (!(m_session.InputManager.Queue.InstantaneousKeyFocus is IInputCustomFocus instantaneousKeyFocus))
                return;
            instantaneousKeyFocus.OverrideHostFocus();
        }

        bool IModalSession.IsModalAllowed => !IsClosing;

        private Form.ShutdownHookInfo GetShutdownHookInfo(string hookName, bool fCanAdd)
        {
            Form.ShutdownHookInfo shutdownHookInfo = new Form.ShutdownHookInfo(hookName);
            uint key;
            if (m_mapShutdownHooks.Lookup(shutdownHookInfo, out key))
                shutdownHookInfo = (Form.ShutdownHookInfo)m_mapShutdownHooks[key];
            else if (fCanAdd)
                m_mapShutdownHooks[m_nextShutdownHookId++] = shutdownHookInfo;
            else
                shutdownHookInfo = null;
            return shutdownHookInfo;
        }

        public bool IsPreProcessedInput => false;

        public class LocationChangedArgs : EventArgs
        {
            private Point m_position;

            public LocationChangedArgs(Point position) => m_position = position;

            public Point Position => m_position;
        }

        public delegate void LocationChangedHandler(Form.LocationChangedArgs args);

        public class NativeSetFocusEventArgs : EventArgs
        {
            private bool _focused;
            private IntPtr _hwndFocusChange;

            public NativeSetFocusEventArgs(bool focused, IntPtr hwndFocusChange)
            {
                _focused = focused;
                _hwndFocusChange = hwndFocusChange;
            }

            public bool Focused => _focused;

            public IntPtr HwndFocusChange => _hwndFocusChange;
        }

        private class ShutdownHookInfo
        {
            private string m_stHookId;

            public ShutdownHookInfo(string stHookId) => m_stHookId = stHookId;

            public event EventHandler Handler;

            public void OnHook(object sender, EventArgs args)
            {
                if (Handler == null)
                    return;
                Handler(sender, args);
            }

            public override bool Equals(object obj) => obj is Form.ShutdownHookInfo shutdownHookInfo && m_stHookId == shutdownHookInfo.m_stHookId;

            public override int GetHashCode() => m_stHookId.GetHashCode();
        }
    }
}
