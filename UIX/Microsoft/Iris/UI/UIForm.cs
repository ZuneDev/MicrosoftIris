// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.UIForm
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Accessibility;
using Microsoft.Iris.Accessibility;
using Microsoft.Iris.Input;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.UI
{
    internal class UIForm : Form, INotifyObject
    {
        private DeferredHandler _initialLoadComplete;
        private string _initialSource;
        private Vector<UIPropertyRecord> _initialProperties;
        private NotifyService _notifier = new NotifyService();
        private NativeApi.NotifyWindowCallback _notificationCallback;
        private bool _showWindowFrame;
        private bool _showShadow;
        private bool _startCentered;
        private bool _startInWorkArea;
        private bool _respectStartupSettings;
        private bool _alwaysOnTop;
        private bool _showInTaskbar;
        private bool _preventInterruption;
        private MaximizeMode _maximizeMode;
        private bool _mouseIsIdle;
        private int _mouseIdleTimeout;
        private bool _hideMouseOnIdle;

        public UIForm(UISession session)
          : base(session)
        {
            session.InputManager.KeyFocusCanBeNull = true;
            session.InputManager.InvalidKeyFocus += new InvalidKeyFocusHandler(OnInvalidKeyFocus);
            _showWindowFrame = true;
            _alwaysOnTop = false;
            _showInTaskbar = true;
            _showShadow = false;
            _startCentered = false;
            _startInWorkArea = false;
            _preventInterruption = false;
            UpdateStyles();
            SetWindowOptions(WindowOptions.FreeformResize, true);
            _notificationCallback = new NativeApi.NotifyWindowCallback(OnNotifyCallback);
            IntPtr handle;
            RendererApi.IFC(NativeApi.SpCreateNotifyWindow(out handle, _notificationCallback));
            AppNotifyWindow = handle;
        }

        private void OnInitialize()
        {
            UIZone newZone = new UIZone(this);
            newZone.DeclareOwner(this);
            AttachChildZone(newZone);
            newZone.RootViewItem.RequestSource(_initialSource, _initialProperties);
        }

        public string Caption
        {
            get => Text;
            set
            {
                if (!(value != Text))
                    return;
                Text = value;
                FireNotification(NotificationID.Caption);
            }
        }

        public bool ShowWindowFrame
        {
            get => _showWindowFrame;
            set
            {
                if (_showWindowFrame == value)
                    return;
                _showWindowFrame = value;
                UpdateStyles();
                FireNotification(NotificationID.ShowWindowFrame);
            }
        }

        public bool ShowShadow
        {
            get => _showShadow;
            set
            {
                if (_showShadow == value)
                    return;
                _showShadow = value;
                SetWindowOptions(WindowOptions.ShowFormShadow, value);
            }
        }

        public bool PreventInterruption
        {
            get => _preventInterruption;
            set
            {
                if (_preventInterruption == value)
                    return;
                _preventInterruption = value;
                SetWindowOptions(WindowOptions.PreventInterruption, value);
            }
        }

        public MaximizeMode MaximizeMode
        {
            get => _maximizeMode;
            set
            {
                if (_maximizeMode == value)
                    return;
                _maximizeMode = value;
                if (value == MaximizeMode.FullScreen)
                    SetWindowOptions(WindowOptions.MaximizeFullScreen, true);
                else
                    SetWindowOptions(WindowOptions.MaximizeFullScreen, false);
                FireNotification(NotificationID.MaximizeMode);
            }
        }

        public bool StartCentered
        {
            get => _startCentered;
            set
            {
                if (_startCentered == value)
                    return;
                _startCentered = value;
                SetWindowOptions(WindowOptions.StartCentered, value);
            }
        }

        public bool StartInWorkArea
        {
            get => _startInWorkArea;
            set
            {
                if (_startInWorkArea == value)
                    return;
                _startInWorkArea = value;
                SetWindowOptions(WindowOptions.StartInWorkArea, value);
            }
        }

        public bool RespectsStartupSettings
        {
            get => _respectStartupSettings;
            set
            {
                if (_respectStartupSettings == value)
                    return;
                _respectStartupSettings = value;
                SetWindowOptions(WindowOptions.RespectStartupSettings, value);
            }
        }

        public bool AlwaysOnTop
        {
            get => _alwaysOnTop;
            set
            {
                if (_alwaysOnTop == value)
                    return;
                _alwaysOnTop = value;
                UpdateStyles();
                FireNotification(NotificationID.AlwaysOnTop);
            }
        }

        public bool ShowInTaskbar
        {
            get => _showInTaskbar;
            set
            {
                if (_showInTaskbar == value)
                    return;
                _showInTaskbar = value;
                UpdateStyles();
                FireNotification(NotificationID.ShowInTaskbar);
            }
        }

        public int MouseIdleTimeout
        {
            get => _mouseIdleTimeout;
            set
            {
                _mouseIdleTimeout = value;
                if (value != 0)
                {
                    SetMouseIdleOptions(new Size(2, 2), (uint)value);
                    SetWindowOptions(WindowOptions.TrackMouseIdle, true);
                }
                else
                    SetWindowOptions(WindowOptions.TrackMouseIdle, false);
                FireNotification(NotificationID.MouseIdleTimeout);
            }
        }

        public bool HideMouseOnIdle
        {
            get => _hideMouseOnIdle;
            set
            {
                if (_hideMouseOnIdle == value)
                    return;
                _hideMouseOnIdle = value;
                SetWindowOptions(WindowOptions.MouseleaveOnIdle, value);
                if (value)
                    IdleCursor = CursorID.None;
                else
                    IdleCursor = CursorID.NotSpecified;
                FireNotification(NotificationID.HideMouseOnIdle);
            }
        }

        public void RequestLoad(string source, Vector<UIPropertyRecord> properties)
        {
            if (Zone == null)
            {
                _initialSource = source;
                _initialProperties = properties;
            }
            else
                Zone.RootViewItem.RequestSource(source, properties);
        }

        public SavedKeyFocus SaveKeyFocus() => Zone != null && Zone.RootUI != null ? new SavedKeyFocus(Zone.RootUI.SaveKeyFocus()) : null;

        public void RestoreKeyFocus(SavedKeyFocus state)
        {
            if (state == null)
                return;
            DeferredCall.Post(DispatchPriority.LayoutSync, new DeferredHandler(DeferredRestoreKeyFocus), state.Payload);
        }

        private void DeferredRestoreKeyFocus(object cookie)
        {
            if (Zone == null || Zone.RootUI == null)
                return;
            Zone.RootUI.RestoreKeyFocus(cookie);
        }

        private void UpdateStyles()
        {
            FormStyleInfo formStyleInfo = new FormStyleInfo();
            uint num1 = 100663296;
            uint num2 = !ShowInTaskbar ? 128U : 262144U;
            if (AlwaysOnTop)
                num2 |= 8U;
            formStyleInfo.uStyleFullscreen = num1;
            formStyleInfo.uExStyleFullscreen = num2;
            uint num3 = num1 | 720896U;
            uint num4 = !ShowWindowFrame ? num3 | 2147483648U : num3 | 12845056U;
            formStyleInfo.uStyleRestored = num4;
            formStyleInfo.uExStyleRestored = num2;
            formStyleInfo.uStyleMinimized = num4;
            formStyleInfo.uExStyleMinimized = num2;
            formStyleInfo.uStyleMaximized = num4;
            formStyleInfo.uExStyleMaximized = num2;
            Styles = formStyleInfo;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Graphic.EnsureFallbackImages();
            OnInitialize();
            Visible = true;
        }

        internal override void OnShow(bool fShow, bool fFirstShow)
        {
            base.OnShow(fShow, fFirstShow);
            if (!fShow || !fFirstShow)
                return;
            Session.InputManager.KeyFocusCanBeNull = false;
            if (_initialLoadComplete == null)
                return;
            DeferredCall.Post(DispatchPriority.Idle, new SimpleCallback(DeliverIntialLoadCompleteCallback));
        }

        protected override void OnActivationChange()
        {
            FireNotification(NotificationID.Active);
            base.OnActivationChange();
        }

        protected override void OnWindowStateChanged(bool fUnplanned) => FireNotification(NotificationID.WindowState);

        protected override void OnLocationChanged(Point position) => FireNotification(NotificationID.Position);

        protected override void OnSizeChanged() => FireNotification(NotificationID.ClientSize);

        public bool MouseIsIdle => _mouseIsIdle;

        protected override void OnMouseIdle(bool value)
        {
            if (_mouseIsIdle != value)
            {
                _mouseIsIdle = value;
                FireNotification(NotificationID.MouseActive);
            }
            base.OnMouseIdle(value);
        }

        private void DeliverIntialLoadCompleteCallback()
        {
            DeferredCall.Post(DispatchPriority.Idle, _initialLoadComplete, null);
            _initialLoadComplete = null;
        }

        public void SetInitialLoadCompleteCallback(DeferredHandler callback) => _initialLoadComplete = callback;

        public void Close() => RequestClose(FormCloseReason.UserRequest);

        protected override void OnCloseRequest(FormCloseReason nReason)
        {
            bool block = false;
            if (CloseRequested != null)
                CloseRequested(nReason, ref block);
            if (block)
                return;
            base.OnCloseRequest(nReason);
        }

        public event FormCloseRequestedHandler CloseRequested;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Zone.Dispose(this);
            Session.InputManager.InvalidKeyFocus -= new InvalidKeyFocusHandler(OnInvalidKeyFocus);
            Session.Dispatcher.StopCurrentMessageLoop();
            NativeApi.SpDestroyNotifyWindow();
        }

        private void OnInvalidKeyFocus(ICookedInputSite lastKnownFocused)
        {
            if (lastKnownFocused is UIClass uiClass)
            {
                UIClass focusableAncestor = uiClass.FindKeyFocusableAncestor();
                if (focusableAncestor != null)
                {
                    focusableAncestor.RequestKeyFocus();
                    return;
                }
            }
            SetDefaultKeyFocus();
        }

        protected override IntPtr OnAccGetObject(int wparam, int lparam)
        {
            AccessibleProxy.AccessibilityActive = true;
            AccObjectID accObjectId = (AccObjectID)lparam;
            object accPtr1 = null;
            if (accObjectId == AccObjectID.Client)
            {
                accPtr1 = Zone.RootUI.AccessibleProxy;
                RootAccessibleProxy rootAccessibleProxy = (RootAccessibleProxy)accPtr1;
                if (rootAccessibleProxy.ClientBridge == null)
                {
                    object accPtr2;
                    AccessibleProxy.CreateStdAccessibleObject(__WindowHandle, -4, AccessibleProxy.IID_IAccessible, out accPtr2);
                    rootAccessibleProxy.AttachClientBridge((IAccessible)accPtr2);
                }
            }
            else if (accObjectId > AccObjectID.Window)
                accPtr1 = AccessibleProxy.AccessibleProxyFromID((int)accObjectId);
            if (accPtr1 == null)
                return IntPtr.Zero;
            Guid iidIaccessible = AccessibleProxy.IID_IAccessible;
            return AccessibleProxy.LresultFromObject(ref iidIaccessible, (IntPtr)wparam, accPtr1);
        }

        private IntPtr OnNotifyCallback(
          NativeApi.NotificationType notification,
          int param1,
          int param2)
        {
            return notification == NativeApi.NotificationType.GetObject ? OnAccGetObject(param1, param2) : new IntPtr(0);
        }

        private void FireNotification(string id)
        {
            if (PropertyChanged != null)
                PropertyChanged(id);
            _notifier.Fire(id);
        }

        public event FormPropertyChangedHandler PropertyChanged;

        void INotifyObject.AddListener(Listener listener) => _notifier.AddListener(listener);
    }
}
