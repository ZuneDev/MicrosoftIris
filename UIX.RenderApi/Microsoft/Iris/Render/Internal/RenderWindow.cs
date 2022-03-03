// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.RenderWindow
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Desktop;
using Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Iris.Render.Internal
{
    [SuppressUnmanagedCodeSecurity]
    internal sealed class RenderWindow :
      IRenderWindow,
      ITreeOwner,
      IFormWindowCallback,
      IRenderHandleOwner
    {
        private const uint ACT_INACTIVE = 0;
        private const uint ACT_ACTIVE = 1;
        private const uint ACT_POPUP = 2;
        private const uint ACT_SYSUI = 3;
        private const uint STCH_MODE = 1;
        private const uint STCH_ACTIVATION = 2;
        private const uint STCH_VISIBILITY = 4;
        private const uint STCH_LOCATION = 8;
        private const uint STCH_SIZE = 16;
        private const uint STCH_UNPLANNED = 256;
        private const uint WM_KEYFIRST = 256;
        private const uint WM_KEYLAST = 265;
        private const uint WM_MOUSEFIRST = 512;
        private const uint WM_MOUSELAST = 526;
        private static ushort[] s_ByteOrder_FormStateCallbackMsg;
        private RenderSession m_session;
        private DisplayManager m_displayManager;
        private GraphicsDevice m_device;
        private GraphicsDeviceType m_graphicsDeviceType;
        private VisualContainer m_rootVisual;
        private int m_nX;
        private int m_nY;
        private int m_nWidth;
        private int m_nHeight;
        private int m_nMinResizeWidth;
        private int m_nMaxResizeWidth;
        private Size m_szDefault;
        private string m_stText;
        private Display m_currentDisplay;
        private bool m_fSpanningMonitors;
        private bool m_fOnSecondaryMonitor;
        private bool m_fRightToLeft;
        private bool m_fActivation;
        private bool m_fFocused;
        private bool m_fVisible;
        private bool m_fExclusive;
        private bool m_fShownBefore;
        private bool m_fSessionHasDisplay;
        private bool m_fSessionLocked;
        private bool m_fExplicitlyLocked;
        private bool m_fClosing;
        private bool m_fLoadComplete;
        private bool m_fLoadEventFired;
        private WindowState m_nWindowState;
        private FormStyleInfo m_windowStyles;
        private ColorF m_clrBackground;
        private ColorF m_clrOutlineAllColor;
        private ColorF m_clrOutlineMarkedColor;
        private Cursor m_cursor;
        private Cursor m_cursorIdle;
        private Stack m_stkWaitCursors;
        private HWND m_hwnd;
        private RemoteFormWindow m_remoteWindow;
        private RenderWindow.RenderFlags m_renderFlags;
        private bool m_fPreProcessedInput;
        private int m_nMouseLockCount;
        private ArrayList m_partialDropData;
        private bool m_fEnableExternalDragDrop;
        private uint m_nDragDropResult;
        private uint m_nDragOverResult;
        private bool m_fIsDragInProgress;
        private FormPlacement m_finalPlacement;
        private SmartMap<RenderWindow.ShutdownHookInfo> m_mapShutdownHooks;
        private ushort m_nextShutdownHookId;

        internal RenderWindow(
          RenderSession session,
          DisplayManager displayManager,
          GraphicsDeviceType graphicsDeviceType,
          GraphicsRenderingQuality renderingQuality)
        {
            Debug2.Validate(session != null, typeof(ArgumentNullException), "must pass a valid session");
            session.AssertOwningThread();
            Debug2.Validate(displayManager != null, typeof(ArgumentNullException), "must pass a valid DisplayManager");
            this.m_session = session;
            this.m_displayManager = displayManager;
            this.CreateGraphicsDevice(session, graphicsDeviceType, renderingQuality);
            this.m_device.RegisterWindow(this);
            this.m_clrBackground = new ColorF(0.0f, 0.0f, 0.0f);
            this.m_remoteWindow = this.m_session.BuildRemoteFormWindow(this, this.m_displayManager);
            this.m_fPreProcessedInput = false;
            this.m_cursor = Cursor.Default;
            this.m_cursorIdle = Cursor.Default;
            this.m_nWindowState = WindowState.Normal;
            this.m_fRightToLeft = false;
            this.m_fSessionHasDisplay = true;
            this.m_fSessionLocked = false;
            this.m_szDefault = new Size(640, 480);
            this.m_clrOutlineAllColor = new ColorF(byte.MaxValue, 0, byte.MaxValue, 0);
            this.m_clrOutlineMarkedColor = new ColorF(byte.MaxValue, byte.MaxValue, 0, 0);
            this.m_mapShutdownHooks = new SmartMap<RenderWindow.ShutdownHookInfo>();
            this.m_nextShutdownHookId = 1;
            this.m_nMouseLockCount = 0;
            this.SetFullScreenExclusive(false);
        }

        public void Initialize() => this.BuildRootContainer();

        private GraphicsDevice CreateGraphicsDevice(
          RenderSession session,
          GraphicsDeviceType graphicsDeviceType,
          GraphicsRenderingQuality renderingQuality)
        {
            GraphicsDevice graphicsDevice = null;
            EngineApi.IFC(FormApi.SpGdiplusInit());
            this.m_graphicsDeviceType = graphicsDeviceType;
            switch (this.m_graphicsDeviceType)
            {
                case GraphicsDeviceType.Gdi:
                    this.m_device = new GdiGraphicsDevice(session);
                    break;
                case GraphicsDeviceType.Direct3D9:
                    this.m_device = new NtGraphicsDevice(session, renderingQuality);
                    break;
            }
            return graphicsDevice;
        }

        internal void Dispose()
        {
            this.Visible = false;
            this.m_remoteWindow.SendSetRoot(null);
            this.m_rootVisual.UnregisterUsage(this);
            this.StopRendering();
        }

        internal void StopRendering()
        {
            if (this.m_device != null)
            {
                this.m_device.Dispose();
                this.m_device = null;
            }
            if (this.m_remoteWindow != null)
            {
                this.m_remoteWindow.Dispose();
                this.m_remoteWindow = null;
            }
            EngineApi.IFC(FormApi.SpGdiplusUninit());
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteWindow.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteWindow = null;

        internal RemoteFormWindow RemoteStub => this.m_remoteWindow;

        public int Left => this.m_nX;

        public int Top => this.m_nY;

        public int Right => this.m_nX + this.m_nWidth;

        public int Bottom => this.m_nY + this.m_nHeight;

        public int Width => this.m_nWidth;

        public int Height => this.m_nHeight;

        public HWND WindowHandle => this.m_hwnd;

        public Size ClientSize
        {
            get => new Size(this.m_nWidth, this.m_nHeight);
            set
            {
                if (this.m_nWidth == value.Width && this.m_nHeight == value.Height || !this.m_session.IsValid)
                    return;
                this.m_remoteWindow.SendSetSize(value);
            }
        }

        public Size InitialClientSize
        {
            get => this.m_szDefault;
            set => this.m_szDefault = value;
        }

        public FormPlacement InitialPlacement
        {
            set
            {
                if (!this.m_session.IsValid || this.m_remoteWindow == null || !(this.m_hwnd == HWND.NULL))
                    return;
                this.m_remoteWindow.SendSetInitialPlacement(value.ShowState, value.NormalPosition, value.MaximizedLocation);
            }
        }

        public FormPlacement FinalPlacement => this.m_finalPlacement;

        public int MinResizeWidth
        {
            get => this.m_nMinResizeWidth;
            set
            {
                if (this.m_nMinResizeWidth == value)
                    return;
                this.m_nMinResizeWidth = value;
                if (!this.m_session.IsValid)
                    return;
                this.m_remoteWindow.SendSetMinResizeWidth(this.m_nMinResizeWidth);
            }
        }

        public int MaxResizeWidth
        {
            get => this.m_nMaxResizeWidth;
            set
            {
                if (this.m_nMaxResizeWidth == value)
                    return;
                this.m_nMaxResizeWidth = value;
                if (!this.m_session.IsValid)
                    return;
                this.m_remoteWindow.SendSetMaxResizeWidth(this.m_nMaxResizeWidth);
            }
        }

        public Point Position
        {
            get => new Point(this.m_nX, this.m_nY);
            set
            {
                if (this.m_nX == value.X && this.m_nY == value.Y || !this.m_session.IsValid)
                    return;
                this.m_remoteWindow.SendSetPosition(value);
            }
        }

        public string Text
        {
            get => this.m_stText == null ? "" : this.m_stText;
            set
            {
                this.m_stText = value;
                this.UpdateText(true);
            }
        }

        public Cursor Cursor
        {
            get => this.m_cursor;
            set
            {
                if (this.m_cursor == value)
                    return;
                this.m_cursor = value;
                this.UpdateCursors();
            }
        }

        public Cursor IdleCursor
        {
            get => this.m_cursorIdle;
            set
            {
                if (this.m_cursorIdle == value)
                    return;
                this.m_cursorIdle = value;
                this.UpdateCursors();
            }
        }

        public bool Visible
        {
            get => this.m_fVisible;
            set
            {
                if (this.m_fVisible == value)
                    return;
                this.m_fVisible = value;
                if (!this.m_session.IsValid)
                    return;
                this.m_remoteWindow.SendSetVisible(this.m_fVisible);
            }
        }

        public bool IsLoaded => this.m_fLoadEventFired;

        public ColorF BackgroundColor
        {
            get => this.m_clrBackground;
            set => this.SetBackgroundColor(value);
        }

        IDisplay IRenderWindow.CurrentDisplay
        {
            get => this.m_currentDisplay == null ? this.m_displayManager.PrimaryDisplay : m_currentDisplay;
            set => this.SetCurrentDisplay(value);
        }

        private void SetCurrentDisplay(IDisplay inputIDisplay)
        {
            Debug2.Validate(inputIDisplay is Display, null, "CurrentDisplay.set param MUST be a valid Display object");
            Display display = this.m_displayManager.DisplayFromUniqueId((inputIDisplay as Display).UniqueId);
            if (display == null)
                return;
            this.m_currentDisplay = display;
        }

        public bool FullScreenExclusive
        {
            get => this.m_fExclusive;
            set => this.SetFullScreenExclusive(value);
        }

        private void SetFullScreenExclusive(bool fNewValue)
        {
            switch (this.m_graphicsDeviceType)
            {
                case GraphicsDeviceType.Direct3D9:
                    this.m_fExclusive = fNewValue;
                    break;
                case GraphicsDeviceType.XeDirectX9:
                    this.m_fExclusive = true;
                    break;
                default:
                    this.m_fExclusive = false;
                    break;
            }
        }

        public bool ActivationState => this.m_fActivation;

        public WindowState WindowState
        {
            get => this.m_nWindowState;
            set
            {
                if (this.m_nWindowState == value)
                    return;
                this.m_nWindowState = value;
                if (!this.m_session.IsValid)
                    return;
                this.m_remoteWindow.SendSetMode((uint)this.m_nWindowState);
            }
        }

        public FormStyleInfo Styles
        {
            get => this.m_windowStyles;
            set
            {
                this.m_windowStyles = value;
                if (!this.m_session.IsValid)
                    return;
                this.m_remoteWindow.SendSetStyles(value.uStyleRestored, value.uExStyleRestored, value.uStyleMinimized, value.uExStyleMinimized, value.uStyleMaximized, value.uExStyleMaximized, value.uStyleFullscreen, value.uExStyleFullscreen);
            }
        }

        HWND IRenderWindow.AppNotifyWindow
        {
            set => this.m_remoteWindow.INPROC_SendSetAppNotifyWindow(value);
        }

        IVisualContainer IRenderWindow.VisualRoot => m_rootVisual;

        TreeNode ITreeOwner.Root => m_rootVisual;

        internal bool IsClosing => this.m_fClosing;

        internal bool IsSessionActive => this.m_fSessionHasDisplay && !this.m_fSessionLocked;

        internal bool IsSessionRemote => false;

        internal bool IsSpanningMonitors => this.m_fSpanningMonitors;

        internal bool IsOnSecondaryMonitor => this.m_fOnSecondaryMonitor;

        internal ColorF OutlineAllColor
        {
            get => this.m_clrOutlineAllColor;
            set
            {
                if (!(this.m_clrOutlineAllColor != value))
                    return;
                this.m_clrOutlineAllColor = value;
                this.m_remoteWindow.SendSetOutlineAllColor(value);
            }
        }

        internal ColorF OutlineMarkedColor
        {
            get => this.m_clrOutlineMarkedColor;
            set
            {
                if (!(this.m_clrOutlineMarkedColor != value))
                    return;
                this.m_clrOutlineMarkedColor = value;
                this.m_remoteWindow.SendSetOutlineMarkedColor(value);
            }
        }

        internal GraphicsDevice GraphicsDevice => this.m_device;

        internal RenderSession Session => this.m_session;

        internal GraphicsDeviceType GraphicsDeviceType => this.m_graphicsDeviceType;

        internal bool IsRightToLeft => this.m_fRightToLeft;

        public event LocationChangedHandler LocationChangedEvent;

        public event SizeChangedHandler SizeChangedEvent;

        public event MonitorChangedHandler MonitorChangedEvent;

        public event WindowStateChangedHandler WindowStateChangedEvent;

        public event CloseHandler CloseEvent;

        public event CloseRequestHandler CloseRequestEvent;

        public event SysCommandHandler SysCommandEvent;

        public event MouseIdleHandler MouseIdleEvent;

        public event ShowHandler ShowEvent;

        public event ActivationChangeHandler ActivationChangeEvent;

        public event SessionActivateHandler SessionActivateEvent;

        public event SessionConnectHandler SessionConnectEvent;

        public event SetFocusHandler SetFocusEvent;

        public event LoadHandler LoadEvent;

        public event ForwardMessageHandler ForwardMessageEvent;

        public event RenderWindow.RendererSuspendedHandler RendererSuspendedEvent;

        internal event EventHandler WindowCreatedEvent;

        private void OnCreated()
        {
            if (this.WindowCreatedEvent == null)
                return;
            this.WindowCreatedEvent(this, EventArgs.Empty);
        }

        private void OnForwardWndMsg(uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (this.ForwardMessageEvent == null)
                return;
            this.ForwardMessageEvent(msg, wParam, lParam);
        }

        private void FireLoadEvent()
        {
            if (this.m_fLoadEventFired || !this.m_fLoadComplete || this.m_nWidth == 0 && this.m_nHeight == 0)
                return;
            if (this.LoadEvent != null)
                this.LoadEvent();
            this.m_fLoadEventFired = true;
        }

        private void OnLocationChanged()
        {
            if (this.LocationChangedEvent == null)
                return;
            this.LocationChangedEvent(this.Position);
        }

        internal void NotifyDisplayReconfigured() => this.OnSizeChanged();

        private void OnSizeChanged()
        {
            this.FireLoadEvent();
            if (this.SizeChangedEvent == null)
                return;
            this.SizeChangedEvent();
        }

        private void OnMonitorChanged()
        {
            if (this.MonitorChangedEvent == null)
                return;
            this.MonitorChangedEvent();
        }

        private void OnWindowStateChanged(bool fUnplanned)
        {
            if (this.WindowStateChangedEvent == null)
                return;
            this.WindowStateChangedEvent(fUnplanned);
        }

        private void OnDestroyed()
        {
        }

        private void OnSysCommand(IntPtr uParam1, IntPtr uParam2)
        {
            if (this.SysCommandEvent == null)
                return;
            this.SysCommandEvent(uParam1, uParam2);
        }

        private void OnMouseIdle(bool fIdle)
        {
            if (this.MouseIdleEvent == null)
                return;
            this.MouseIdleEvent(fIdle);
        }

        private void OnShow(bool fShow, bool fFirstShow)
        {
            if (this.ShowEvent == null)
                return;
            this.ShowEvent(fShow, fFirstShow);
        }

        private void OnActivationChange()
        {
            if (this.ActivationChangeEvent == null)
                return;
            this.ActivationChangeEvent();
        }

        private void OnTermSessionChange(uint uParam)
        {
            bool flag = false;
            bool isSessionActive = this.IsSessionActive;
            switch (uParam)
            {
                case 1:
                case 3:
                    this.m_fSessionHasDisplay = true;
                    break;
                case 2:
                case 4:
                    this.m_fSessionHasDisplay = false;
                    break;
                case 7:
                    this.m_fSessionLocked = true;
                    this.m_fExplicitlyLocked = true;
                    break;
                case 8:
                    this.m_fSessionLocked = false;
                    flag = flag || !this.m_fExplicitlyLocked;
                    break;
            }
            if (this.IsSessionActive != isSessionActive || flag)
                this.FireSessionActivate(this.IsSessionActive);
            if (uParam != 2U && uParam != 1U && (uParam != 4U && uParam != 3U))
                return;
            this.FireSessionConnect(uParam == 1U || uParam == 3U);
        }

        public void FireSessionActivate(bool fIsActive) => this.OnSessionActivate(fIsActive);

        private void OnSessionActivate(bool fIsActive)
        {
            if (fIsActive && this.Visible && this.m_fExclusive)
            {
                this.Focus();
                this.m_remoteWindow.SendSetForeground(false);
            }
            if (this.SessionActivateEvent == null)
                return;
            this.SessionActivateEvent(fIsActive);
        }

        private void FireSessionConnect(bool fIsConnected)
        {
            if (this.SessionConnectEvent == null)
                return;
            this.SessionConnectEvent(fIsConnected);
        }

        private void OnNativeScreensave(bool fStart)
        {
        }

        private void OnDroppedFiles(IEnumerable files)
        {
        }

        public void EnableShellShutdownHook(string hookName, EventHandler handler) => this.GetShutdownHookInfo(hookName, true).Handler += handler;

        private RenderWindow.ShutdownHookInfo GetShutdownHookInfo(
          string hookName,
          bool fCanAdd)
        {
            RenderWindow.ShutdownHookInfo desired = new RenderWindow.ShutdownHookInfo(hookName);
            if (this.m_mapShutdownHooks.Lookup(desired, out uint _))
                return desired;
            if (fCanAdd)
            {
                ushort uIdMsg = this.m_nextShutdownHookId++;
                this.m_mapShutdownHooks.SetValue(uIdMsg, desired);
                this.m_remoteWindow.SendEnableShellShutdownHook(hookName, uIdMsg);
            }
            else
                desired = null;
            return desired;
        }

        void IFormWindowCallback.OnTerminalSessionChange(
          RENDERHANDLE target,
          IntPtr wParam,
          IntPtr lParam)
        {
            this.OnTermSessionChange((uint)wParam.ToInt32());
        }

        void IFormWindowCallback.OnPrivateSysCommand(
          RENDERHANDLE target,
          IntPtr wParam,
          IntPtr lParam)
        {
            this.OnSysCommand(wParam, lParam);
        }

        void IFormWindowCallback.OnMouseIdle(RENDERHANDLE target, bool fNewIdle) => this.OnMouseIdle(fNewIdle);

        void IFormWindowCallback.OnCloseRequested(RENDERHANDLE target) => this.EngineCloseRequest();

        void IFormWindowCallback.OnLoad(RENDERHANDLE target)
        {
            this.m_fLoadComplete = true;
            if (this.m_fClosing)
                return;
            this.FireLoadEvent();
        }

        void IFormWindowCallback.OnWindowDestroyed(
          RENDERHANDLE target,
          uint nFinalShowState,
          Rectangle rcFinalPosition,
          Point ptFinalMaximizedLocation)
        {
            this.m_finalPlacement.NormalPosition = rcFinalPosition;
            this.m_finalPlacement.MaximizedLocation = ptFinalMaximizedLocation;
            this.m_finalPlacement.ShowState = nFinalShowState;
            this.m_hwnd = HWND.NULL;
            this.m_fClosing = true;
            if (this.CloseEvent == null)
                return;
            this.CloseEvent();
        }

        void IFormWindowCallback.OnWindowCreated(RENDERHANDLE target, HWND hWnd)
        {
            this.m_hwnd = hWnd;
            this.m_device.PostCreate();
            this.UpdateText(false);
            this.OnCreated();
        }

        unsafe void IFormWindowCallback.OnStateChange(
          RENDERHANDLE target,
          Message* pmsgRaw)
        {
            RenderWindow.FormStateCallbackMsg* stateCallbackMsgPtr = (RenderWindow.FormStateCallbackMsg*)pmsgRaw;
            if (this.m_session.IsForeignByteOrderOnWindowing)
                MarshalHelper.SwapByteOrder((byte*)stateCallbackMsgPtr, ref s_ByteOrder_FormStateCallbackMsg, typeof(RenderWindow.FormStateCallbackMsg), 0, 0);
            uint num = 0;
            if (this.m_currentDisplay != null)
                num = this.m_currentDisplay.UniqueId;
            bool flag1 = stateCallbackMsgPtr->fOnSecondaryMonitor != 0;
            bool flag2 = stateCallbackMsgPtr->cSpanningMonitors > 1U;
            bool flag3 = false;
            if (stateCallbackMsgPtr->idDisplay != uint.MaxValue)
                flag3 = (int)num != (int)stateCallbackMsgPtr->idDisplay || this.m_fOnSecondaryMonitor != flag1 || this.m_fSpanningMonitors != flag2;
            bool flag4 = this.m_nX != stateCallbackMsgPtr->rcWindowGlobal_left || this.m_nY != stateCallbackMsgPtr->rcWindowGlobal_top;
            bool flag5 = this.m_nWidth != stateCallbackMsgPtr->szClientDims_cx || this.m_nHeight != stateCallbackMsgPtr->szClientDims_cy;
            this.m_nX = stateCallbackMsgPtr->rcWindowGlobal_left;
            this.m_nY = stateCallbackMsgPtr->rcWindowGlobal_top;
            this.m_nWidth = stateCallbackMsgPtr->szClientDims_cx;
            this.m_nHeight = stateCallbackMsgPtr->szClientDims_cy;
            this.m_nWindowState = (WindowState)stateCallbackMsgPtr->uCurrentMode;
            this.m_fVisible = stateCallbackMsgPtr->fVisible != 0;
            this.m_fSpanningMonitors = flag2;
            this.m_fOnSecondaryMonitor = flag1;
            this.SetFullScreenExclusive(stateCallbackMsgPtr->fExclusive != 0);
            bool fActivation = this.m_fActivation;
            this.m_fActivation = stateCallbackMsgPtr->uActivation == 1U;
            if (flag3)
                this.m_currentDisplay = this.m_displayManager.DisplayFromUniqueId(stateCallbackMsgPtr->idDisplay);
            if (flag3)
                this.OnMonitorChanged();
            if (flag4)
                this.OnLocationChanged();
            if (flag5)
            {
                this.m_rootVisual.Size = new Vector2(m_nWidth, m_nHeight);
                this.OnSizeChanged();
            }
            if (((int)stateCallbackMsgPtr->uRecentlyChanged & 1) != 0)
                this.OnWindowStateChanged(((int)stateCallbackMsgPtr->uRecentlyChanged & 256) != 0);
            if (((int)stateCallbackMsgPtr->uRecentlyChanged & 4) != 0)
            {
                bool fFirstShow = this.m_fVisible && !this.m_fShownBefore;
                if (this.m_fVisible)
                    this.m_fShownBefore = true;
                this.OnShow(this.m_fVisible, fFirstShow);
            }
            if (((int)stateCallbackMsgPtr->uRecentlyChanged & 2) == 0 || this.m_fActivation == fActivation)
                return;
            this.OnActivationChange();
        }

        void IFormWindowCallback.OnPartialDrop(RENDERHANDLE target, string file)
        {
            if (this.m_partialDropData == null)
                this.m_partialDropData = new ArrayList();
            this.m_partialDropData.Add(file);
        }

        void IFormWindowCallback.OnDropComplete(RENDERHANDLE target)
        {
            if (this.m_partialDropData == null || this.m_partialDropData.Count <= 0)
                return;
            IEnumerable partialDropData = m_partialDropData;
            this.m_partialDropData = null;
            this.OnDroppedFiles(partialDropData);
        }

        void IFormWindowCallback.OnSetFocus(
          RENDERHANDLE target,
          bool focused,
          HWND hwndFocusChange)
        {
            this.m_fFocused = focused;
            int num = focused ? 1 : 0;
            if (this.SetFocusEvent == null)
                return;
            this.SetFocusEvent(focused);
        }

        void IFormWindowCallback.OnShellShutdownHook(
          RENDERHANDLE target,
          ushort hookId)
        {
            RenderWindow.ShutdownHookInfo shutdownHookInfo;
            if (!this.m_mapShutdownHooks.TryGetValue(hookId, out shutdownHookInfo))
                return;
            shutdownHookInfo.OnHook(this, EventArgs.Empty);
        }

        void IFormWindowCallback.OnNativeScreensave(
          RENDERHANDLE target,
          bool fStartScreensave)
        {
            this.m_session.DeferredInvoke(new DeferredHandler(this.DoNativeScreensave), fStartScreensave, DeferredInvokePriority.Idle);
        }

        private void DoNativeScreensave(object objParam) => this.OnNativeScreensave((bool)objParam);

        private void DoPopAnimations(object objParam)
        {
        }

        void IFormWindowCallback.OnRendererSuspended(
          RENDERHANDLE target,
          bool fSuspended)
        {
            this.FireRendererSuspended(fSuspended);
        }

        private void FireRendererSuspended(bool fSuspended)
        {
            if (this.RendererSuspendedEvent == null)
                return;
            this.RendererSuspendedEvent(m_device, new RenderWindow.RendererSuspendedArgs(fSuspended));
        }

        internal RenderWindow.RenderFlags GlobalRenderFlags
        {
            get => this.m_renderFlags;
            set => this.SetGlobalRenderFlags(value, RenderFlags.All);
        }

        internal bool SetGlobalRenderFlags(
          RenderWindow.RenderFlags flags,
          RenderWindow.RenderFlags mask)
        {
            bool flag = false;
            RenderWindow.RenderFlags renderFlags = this.m_renderFlags & ~mask | flags & mask;
            if (this.m_renderFlags != renderFlags)
            {
                this.m_renderFlags = renderFlags;
                flag = true;
                this.m_remoteWindow.SendChangeDataBits((uint)flags, (uint)mask);
            }
            return flag;
        }

        public void SetWindowOptions(WindowOptions optionMask, bool enable)
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendSetOptions((uint)optionMask, enable ? (uint)optionMask : 0U);
        }

        public void SetMouseIdleOptions(Size mouseIdleTolerance, uint mouseIdleDelay)
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendSetMouseIdleOptions(mouseIdleTolerance, mouseIdleDelay);
        }

        public void ForwardWindowMessage(ref Win32Api.MSG msg)
        {
            if (msg.message < 256U || msg.message > 265U)
                return;
            this.OnForwardWndMsg(msg.message, msg.wParam, msg.lParam);
        }

        private void BuildRootContainer()
        {
            this.m_remoteWindow.SendSetSize(this.m_szDefault);
            this.m_remoteWindow.SendCreateRootContainer();
            this.m_remoteWindow.SendSetHitMasks(1U, 2U, 4U, 8U);
            RemoteVisual remoteVisual;
            this.m_rootVisual = new VisualContainer(true, this.m_session, this, null, out remoteVisual);
            this.m_rootVisual.RegisterUsage(this);
            this.m_remoteWindow.SendSetRoot(remoteVisual);
            this.m_rootVisual.Size = new Vector2(m_nWidth, m_nHeight);
        }

        void IRenderWindow.ClientToScreen(ref Point point)
        {
            Win32Api.POINT pt;
            pt.x = !this.m_fRightToLeft ? point.X : this.ClientSize.Width - point.X;
            pt.y = point.Y;
            Win32Api.ClientToScreen(this.m_hwnd, ref pt);
            point.X = pt.x;
            point.Y = pt.y;
        }

        void IRenderWindow.ScreenToClient(ref Point point)
        {
            Win32Api.POINT pt;
            pt.x = point.X;
            pt.y = point.Y;
            Win32Api.ScreenToClient(this.m_hwnd, ref pt);
            point.X = !this.m_fRightToLeft ? pt.x : this.ClientSize.Width - pt.x;
            point.Y = pt.y;
        }

        void IRenderWindow.TakeFocus() => this.Focus();

        private void Focus()
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendTakeFocus();
        }

        void IRenderWindow.TakeForeground(bool fForce)
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendSetForeground(fForce);
        }

        void IRenderWindow.BringToTop()
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendBringToTop();
        }

        void IRenderWindow.RefreshHitTarget()
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendRefreshHitTarget();
        }

        private void EngineCloseRequest()
        {
            if (this.CloseRequestEvent != null)
                this.CloseRequestEvent();
            else
                this.Close(FormCloseReason.RendererRequest);
        }

        public void Close(FormCloseReason nReason)
        {
            if (this.m_fClosing)
                return;
            this.ForceCloseWorker(nReason);
        }

        private void ForceClose() => this.ForceCloseWorker(FormCloseReason.ForcedClose);

        private void ForceCloseWorker(FormCloseReason nReason)
        {
            if (this.m_fClosing)
                return;
            this.m_fClosing = true;
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendDestroy();
        }

        public void PushWaitCursor(Cursor cursor)
        {
            if (this.m_stkWaitCursors == null)
                this.m_stkWaitCursors = new Stack();
            this.m_stkWaitCursors.Push(cursor);
            this.UpdateCursors();
        }

        public void PopWaitCursor()
        {
            this.m_stkWaitCursors.Pop();
            this.UpdateCursors();
        }

        public void ForceMouseIdle(bool fIdle)
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendForceMouseIdle(fIdle);
        }

        public void SetCapture(IRawInputSite captureSite, bool state)
        {
            if (captureSite == null)
                return;
            this.m_remoteWindow.SendSetCapture((captureSite as Visual).RemoteStub, state);
        }

        public void SetBackgroundColor(ColorF color)
        {
            if (!(this.m_clrBackground != color))
                return;
            this.m_clrBackground = color;
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendSetBackgroundColor(color);
        }

        public void SetIcon(string sModuleName, uint nResourceID, IconFlags nOptions)
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendSetIcon(sModuleName, nResourceID, (uint)nOptions);
        }

        public void SetEdgeImages(bool fActiveEdges, ShadowEdgePart[] edges)
        {
            if (!this.m_session.IsValid)
                return;
            Debug2.Validate(edges != null, null, "Must pass non-null edges set");
            Debug2.Validate(edges.Length == 4, null, "Must pass exactly 4 edges, LTRB");
            for (int index = 0; index < 4; ++index)
            {
                Debug2.Validate(edges[index].ModuleName != null, null, "Must pass non-null ModuleName:" + index.ToString());
                Debug2.Validate(edges[index].ResourceName != null, null, "Must pass non-null ResourceName:" + index.ToString());
            }
            Debug2.Validate(string.Equals(edges[0].ModuleName, edges[1].ModuleName), null, "Module names differ - not supported");
            Debug2.Validate(string.Equals(edges[0].ModuleName, edges[2].ModuleName), null, "Module names differ - not supported");
            Debug2.Validate(string.Equals(edges[0].ModuleName, edges[3].ModuleName), null, "Module names differ - not supported");
            Debug2.Validate(edges[0].SplitPoints == edges[2].SplitPoints, null, "L+R splits differ - not supported");
            Debug2.Validate(edges[1].SplitPoints == edges[3].SplitPoints, null, "T+B splits differ - not supported");
            Inset insetSplits = new Inset(edges[1].SplitPoints.Left, edges[0].SplitPoints.Top, edges[1].SplitPoints.Right, edges[0].SplitPoints.Bottom);
            this.m_remoteWindow.SendSetEdgeImageParts(fActiveEdges, edges[0].ModuleName, edges[0].ResourceName, edges[1].ResourceName, edges[2].ResourceName, edges[3].ResourceName, insetSplits);
        }

        public bool EnableExternalDragDrop
        {
            get => this.m_fEnableExternalDragDrop;
            set
            {
                if (this.m_fEnableExternalDragDrop == value)
                    return;
                this.m_fEnableExternalDragDrop = value;
                if (!this.m_session.IsValid)
                    return;
                this.m_remoteWindow.SendEnableExternalDragDrop(value);
            }
        }

        public void SetDragDropResult(uint nDragOverResult, uint nDragDropResult)
        {
            if ((int)this.m_nDragOverResult == (int)nDragOverResult && (int)this.m_nDragDropResult == (int)nDragDropResult)
                return;
            this.m_nDragDropResult = nDragDropResult;
            this.m_nDragOverResult = nDragOverResult;
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendSetDragDropResult(nDragOverResult, nDragDropResult);
        }

        public bool IsDragInProgress
        {
            get => this.m_fIsDragInProgress;
            set
            {
                if (this.m_fIsDragInProgress == value)
                    return;
                this.m_fIsDragInProgress = value;
                if (!this.m_session.IsValid)
                    return;
                if (this.m_fIsDragInProgress)
                    this.m_remoteWindow.SendEnterInternalDrag();
                else
                    this.m_remoteWindow.SendExitInternalDrag();
            }
        }

        public void Restore()
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendRestore();
        }

        public void TemporarilyExitExclusiveMode()
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendTemporarilyExitExclusiveMode();
        }

        public IHwndHostWindow CreateHwndHostWindow() => new HwndHostWindow(this);

        public void UnlockForegroundWindow()
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendUpdateForegroundLockState();
        }

        public void BringToTop()
        {
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendBringToTop();
        }

        public void LockMouseActive(bool fLock)
        {
            if (fLock)
            {
                ++this.m_nMouseLockCount;
                if (this.m_nMouseLockCount != 1)
                    return;
                this.SetWindowOptions(WindowOptions.LockMouseActive, true);
            }
            else
            {
                this.m_nMouseLockCount = Math.Max(this.m_nMouseLockCount - 1, 0);
                if (this.m_nMouseLockCount != 0)
                    return;
                this.SetWindowOptions(WindowOptions.LockMouseActive, false);
            }
        }

        private void UpdateText(bool fChanged)
        {
            if (!(this.m_hwnd != HWND.NULL) || this.m_stText == null && !fChanged || !this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendSetText(this.Text);
        }

        private void UpdateCursors()
        {
            Cursor cursor1 = Cursor.NullCursor;
            Cursor nullCursor = Cursor.NullCursor;
            Cursor cursor2 = null;
            if (this.m_stkWaitCursors != null && this.m_stkWaitCursors.Count > 0)
                cursor2 = this.m_stkWaitCursors.Peek() as Cursor;
            Cursor cursor3;
            if (cursor2 != null)
            {
                cursor1 = cursor3 = cursor2;
            }
            else
            {
                if (this.m_cursor != null)
                    cursor1 = this.m_cursor;
                cursor3 = this.m_cursorIdle == null ? cursor1 : this.m_cursorIdle;
            }
            if (!this.m_session.IsValid)
                return;
            this.m_remoteWindow.SendSetCursors(cursor1.ResourceId, cursor3.ResourceId);
        }

        public bool IsPreProcessedInput => this.m_fPreProcessedInput;

        public delegate void RendererSuspendedHandler(
          object sender,
          RenderWindow.RendererSuspendedArgs args);

        internal class ShutdownHookInfo
        {
            private string m_stHookId;

            public ShutdownHookInfo(string stHookId) => this.m_stHookId = stHookId;

            public event EventHandler Handler;

            public void OnHook(object sender, EventArgs args)
            {
                if (this.Handler == null)
                    return;
                this.Handler(sender, args);
            }

            public override bool Equals(object obj) => obj is RenderWindow.ShutdownHookInfo shutdownHookInfo && this.m_stHookId == shutdownHookInfo.m_stHookId;

            public override int GetHashCode() => this.m_stHookId.GetHashCode();
        }

        internal class RendererSuspendedArgs : EventArgs
        {
            private bool m_fSuspended;

            public RendererSuspendedArgs(bool fSuspended) => this.m_fSuspended = fSuspended;

            public bool Suspended => this.m_fSuspended;
        }

        [System.Flags]
        internal enum RenderFlags
        {
            None = 0,
            DebugPainting = 1,
            OutlineMarked = 2,
            OutlineAll = 4,
            RenderDump = 8,
            Wireframe = 16, // 0x00000010
            InvalidationDebug = 32, // 0x00000020
            NoLighting = 64, // 0x00000040
            NoVertexAlpha = 128, // 0x00000080
            NoTextures = 256, // 0x00000100
            NoAlpha = 512, // 0x00000200
            RemoteDebugMode = 1024, // 0x00000400
            DumpRopsOneTime = 2048, // 0x00000800
            All = DumpRopsOneTime | RemoteDebugMode | NoAlpha | NoTextures | NoVertexAlpha | NoLighting | InvalidationDebug | Wireframe | RenderDump | OutlineAll | OutlineMarked | DebugPainting, // 0x00000FFF
        }

        [ComVisible(false)]
        private struct FormStateCallbackMsg
        {
            public uint cbSize;
            public int nMsg;
            public RENDERHANDLE idObjectSubject;
            public uint uRecentlyChanged;
            public uint uCurrentMode;
            public uint uActivation;
            public int fVisible;
            public int szClientDims_cx;
            public int szClientDims_cy;
            public int rcWindowGlobal_left;
            public int rcWindowGlobal_top;
            public int rcWindowGlobal_right;
            public int rcWindowGlobal_bottom;
            public uint idDisplay;
            public uint cSpanningMonitors;
            public int fExclusive;
            public int fOnSecondaryMonitor;
        }
    }
}
