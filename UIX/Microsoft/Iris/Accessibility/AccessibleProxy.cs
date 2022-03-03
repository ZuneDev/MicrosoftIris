// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Accessibility.AccessibleProxy
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Accessibility;
using Microsoft.Iris.Navigation;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace Microsoft.Iris.Accessibility
{
    [ComVisible(true)]
    [SuppressUnmanagedCodeSecurity]
    public class AccessibleProxy : Microsoft.Iris.OS.CLR.StandardOleMarshalObject, IAccessible, IEnumVARIANT
    {
        internal static readonly Guid IID_IAccessible = new Guid("618736E0-3C3D-11CF-810C-00AA00389B71");
        private UIClass _ui;
        private Accessible _data;
        private IEnumVARIANT _children;
        private int _proxyID = -1;
        private static int s_proxyIDAllocator = 0;
        private static Map<int, AccessibleProxy> s_proxyFromID = new Map<int, AccessibleProxy>();
        private static DeferredHandler s_notifyEventHandler = new DeferredHandler(NotifyEvent);
        private static bool s_accessibilityActive;

        internal AccessibleProxy(UIClass ui, Accessible data)
        {
            _ui = ui;
            _data = data;
            _data.Attach(this);
            _children = new AccessibleChildren(this);
        }

        internal UIClass UI => _ui;

        internal void Detach()
        {
            _data.Detach();
            _ui = null;
            if (_proxyID == -1)
                return;
            s_proxyFromID.Remove(_proxyID);
        }

        internal static bool AccessibilityActive
        {
            get => s_accessibilityActive;
            set => s_accessibilityActive = true;
        }

        internal virtual IAccessible Parent => _ui.Parent != null ? _ui.Parent.AccessibleProxy : null;

        internal int ChildCount => _ui.Children.Count;

        internal virtual string Name
        {
            get => _data.Name;
            set => _data.Name = value;
        }

        internal string Value
        {
            get => _data.Value;
            set => _data.Value = value;
        }

        internal string Description => _data.Description;

        internal AccRole Role => _data.Role;

        internal AccStates State
        {
            get
            {
                AccStates accStates = AccStates.None;
                if (_ui.DirectKeyFocus)
                    accStates |= AccStates.Focused;
                if (_ui.IsKeyFocusable())
                    accStates |= AccStates.Focusable;
                if (_ui.DirectMouseFocus)
                    accStates |= AccStates.HotTracked;
                if (!_ui.Visible)
                    accStates |= AccStates.Invisible;
                if (_ui.Host != null && _ui.Host.IsOffscreen)
                    accStates |= AccStates.OffScreen;
                if (_data.HasPopup)
                    accStates |= AccStates.HasPopup;
                if (_data.IsAnimated)
                    accStates |= AccStates.Animated;
                if (_data.IsBusy)
                    accStates |= AccStates.Busy;
                if (_data.IsChecked)
                    accStates |= AccStates.Checked;
                if (_data.IsCollapsed)
                    accStates |= AccStates.Collapsed;
                if (_data.IsDefault)
                    accStates |= AccStates.Default;
                if (_data.IsExpanded)
                    accStates |= AccStates.Expanded;
                if (_data.IsMarquee)
                    accStates |= AccStates.Marquee;
                if (_data.IsMixed)
                    accStates |= AccStates.Mixed;
                if (_data.IsMultiSelectable)
                    accStates |= AccStates.MultiSelectable;
                if (_data.IsPressed)
                    accStates |= AccStates.Pressed;
                if (_data.IsProtected)
                    accStates |= AccStates.Protected;
                if (_data.IsSelectable)
                    accStates |= AccStates.Selectable;
                if (_data.IsSelected)
                    accStates |= AccStates.Selected;
                if (_data.IsTraversed)
                    accStates |= AccStates.Traversed;
                if (_data.IsUnavailable)
                    accStates |= AccStates.Unavailable;
                return accStates;
            }
        }

        internal string Help => _data.Help;

        internal int HelpTopic => _data.HelpTopic;

        internal string KeyboardShortcut => _data.KeyboardShortcut;

        internal bool HasFocus => _ui.DirectKeyFocus;

        internal string DefaultAction => _data.DefaultAction;

        internal Rectangle Location
        {
            get
            {
                Rectangle rectangle = Rectangle.Zero;
                Vector3 positionPxlVector;
                Vector3 sizePxlVector;
                if (_ui.RootItem != null && ((INavigationSite)_ui.RootItem).ComputeBounds(out positionPxlVector, out sizePxlVector))
                {
                    rectangle = new Rectangle((int)positionPxlVector.X, (int)positionPxlVector.Y, (int)sizePxlVector.X, (int)sizePxlVector.Y);
                    Point location = rectangle.Location;
                    UISession.Default.Form.ClientToScreen(ref location);
                    rectangle.Location = location;
                }
                return rectangle;
            }
        }

        internal virtual IAccessible Navigate(AccNavDirs navDir)
        {
            UIClass resultUI = null;
            switch (navDir)
            {
                case AccNavDirs.Up:
                    _ui.FindNextFocusablePeer(Direction.North, RectangleF.Zero, out resultUI);
                    break;
                case AccNavDirs.Down:
                    _ui.FindNextFocusablePeer(Direction.South, RectangleF.Zero, out resultUI);
                    break;
                case AccNavDirs.Left:
                    _ui.FindNextFocusablePeer(Direction.West, RectangleF.Zero, out resultUI);
                    break;
                case AccNavDirs.Right:
                    _ui.FindNextFocusablePeer(Direction.East, RectangleF.Zero, out resultUI);
                    break;
                case AccNavDirs.Next:
                    resultUI = (UIClass)_ui.NextSibling;
                    break;
                case AccNavDirs.Previous:
                    resultUI = (UIClass)_ui.PreviousSibling;
                    break;
                case AccNavDirs.FirstChild:
                    resultUI = (UIClass)_ui.FirstChild;
                    break;
                case AccNavDirs.LastChild:
                    resultUI = (UIClass)_ui.LastChild;
                    break;
            }
            return resultUI != null ? resultUI.AccessibleProxy : null;
        }

        internal void DoDefaultAction()
        {
            if (_data.DefaultActionCommand == null)
                return;
            _data.DefaultActionCommand.Invoke();
        }

        internal static void NotifyCreated(UIClass ui)
        {
            if (!AccessibilityActive)
                return;
            ui.AccessibleProxy.QueueNotifyEvent(AccEvents.ObjectCreate);
        }

        internal static void NotifyDestroyed(UIClass ui)
        {
            if (!AccessibilityActive)
                return;
            ui.AccessibleProxy.QueueNotifyEvent(AccEvents.ObjectDestroy);
        }

        internal static void NotifyTreeChanged(UIClass ui)
        {
            if (!AccessibilityActive || !ui.Initialized)
                return;
            ui.AccessibleProxy.QueueNotifyEvent(AccEvents.ObjectReorder);
        }

        internal static void NotifyVisibilityChange(UIClass ui, bool visible)
        {
            if (!AccessibilityActive || !ui.Initialized)
                return;
            ui.AccessibleProxy.QueueNotifyEvent(visible ? AccEvents.ObjectShow : AccEvents.ObjectHide);
        }

        internal static void NotifyFocus(UIClass ui)
        {
            if (!AccessibilityActive || !ui.Initialized)
                return;
            ui.AccessibleProxy.QueueNotifyEvent(AccEvents.ObjectFocus);
        }

        internal void NotifyAccessiblePropertyChanged(AccessibleProperty property)
        {
            if (!_ui.Initialized)
                return;
            AccEvents eventType = AccEvents.None;
            switch (property)
            {
                case AccessibleProperty.DefaultAction:
                    eventType = AccEvents.ObjectDefaultActionChange;
                    break;
                case AccessibleProperty.Description:
                    eventType = AccEvents.ObjectDescriptionChange;
                    break;
                case AccessibleProperty.HasPopup:
                case AccessibleProperty.IsAnimated:
                case AccessibleProperty.IsBusy:
                case AccessibleProperty.IsChecked:
                case AccessibleProperty.IsCollapsed:
                case AccessibleProperty.IsDefault:
                case AccessibleProperty.IsExpanded:
                case AccessibleProperty.IsMarquee:
                case AccessibleProperty.IsMixed:
                case AccessibleProperty.IsMultiSelectable:
                case AccessibleProperty.IsPressed:
                case AccessibleProperty.IsProtected:
                case AccessibleProperty.IsSelectable:
                case AccessibleProperty.IsTraversed:
                case AccessibleProperty.IsUnavailable:
                    eventType = AccEvents.ObjectStateChange;
                    break;
                case AccessibleProperty.Help:
                    eventType = AccEvents.ObjectHelpChange;
                    break;
                case AccessibleProperty.HelpTopic:
                    eventType = AccEvents.ObjectHelpChange;
                    break;
                case AccessibleProperty.IsSelected:
                    if (_data.IsSelected)
                    {
                        eventType = AccEvents.ObjectSelection;
                        break;
                    }
                    break;
                case AccessibleProperty.KeyboardShortcut:
                    eventType = AccEvents.ObjectAcceleratorChange;
                    break;
                case AccessibleProperty.Name:
                    eventType = AccEvents.ObjectNameChange;
                    break;
                case AccessibleProperty.Value:
                    eventType = AccEvents.ObjectValueChange;
                    break;
            }
            if (eventType == AccEvents.None)
                return;
            QueueNotifyEvent(eventType);
        }

        private void QueueNotifyEvent(AccEvents eventType)
        {
            object obj = new object[2]
            {
         this,
         (int) eventType
            };
            DeferredCall.Post(DispatchPriority.AppEvent, s_notifyEventHandler, obj);
        }

        private static void NotifyEvent(object payload)
        {
            object[] objArray = (object[])payload;
            AccessibleProxy accessibleProxy = (AccessibleProxy)objArray[0];
            AccEvents accEvents = (AccEvents)objArray[1];
            int proxyId = accessibleProxy.ProxyID;
            UISession.Default.Form.NotifyWinEvent((int)accEvents, proxyId, 0);
            if (accEvents != AccEvents.ObjectDestroy)
                return;
            accessibleProxy.Detach();
        }

        object IAccessible.accParent
        {
            get
            {
                VerifyProxyAccess();
                return Parent;
            }
        }

        object IAccessible.get_accChild(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return this;
        }

        int IAccessible.accChildCount
        {
            get
            {
                VerifyProxyAccess();
                return ChildCount;
            }
        }

        string IAccessible.get_accName(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return Name;
        }

        string IAccessible.get_accValue(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return Value;
        }

        string IAccessible.get_accDescription(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return Description;
        }

        object IAccessible.get_accRole(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return (int)Role;
        }

        object IAccessible.get_accState(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return (int)State;
        }

        string IAccessible.get_accHelp(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return Help;
        }

        int IAccessible.get_accHelpTopic(out string pszHelpFile, object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            pszHelpFile = null;
            return HelpTopic;
        }

        string IAccessible.get_accKeyboardShortcut(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return KeyboardShortcut;
        }

        object IAccessible.accFocus
        {
            get
            {
                VerifyProxyAccess();
                return HasFocus ? 0 : (object)null;
            }
        }

        object IAccessible.accSelection
        {
            get
            {
                VerifyProxyAccess();
                Marshal.ThrowExceptionForHR(-2147467263);
                return null;
            }
        }

        string IAccessible.get_accDefaultAction(object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            return DefaultAction;
        }

        void IAccessible.accSelect(int flagsSelect, object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            Marshal.ThrowExceptionForHR(-2147467263);
        }

        void IAccessible.accLocation(
          out int pxLeft,
          out int pyTop,
          out int pcxWidth,
          out int pcyHeight,
          object varChild)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            Rectangle location = Location;
            pxLeft = location.X;
            pyTop = location.Y;
            pcxWidth = location.Width;
            pcyHeight = location.Height;
        }

        object IAccessible.accNavigate(int navDir, object varStart)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varStart);
            return Navigate((AccNavDirs)navDir);
        }

        object IAccessible.accHitTest(int xLeft, int yTop)
        {
            VerifyProxyAccess();
            Marshal.ThrowExceptionForHR(-2147467263);
            return null;
        }

        void IAccessible.accDoDefaultAction(object varChild)
        {
            VerifyProxyAccess();
            DoDefaultAction();
        }

        void IAccessible.set_accName(object varChild, string pszName)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            Name = pszName;
        }

        void IAccessible.set_accValue(object varChild, string pszValue)
        {
            VerifyProxyAccess();
            VerifySelfChildID(varChild);
            Value = pszValue;
        }

        IEnumVARIANT IEnumVARIANT.Clone()
        {
            VerifyProxyAccess();
            return _children.Clone();
        }

        int IEnumVARIANT.Next(int celt, object[] rgVar, IntPtr pceltFetched)
        {
            VerifyProxyAccess();
            return _children.Next(celt, rgVar, pceltFetched);
        }

        int IEnumVARIANT.Reset()
        {
            VerifyProxyAccess();
            return _children.Reset();
        }

        int IEnumVARIANT.Skip(int celt)
        {
            VerifyProxyAccess();
            return _children.Skip(celt);
        }

        private void VerifyProxyAccess()
        {
            if (_ui != null && UIDispatcher.IsUIThread)
                return;
            Marshal.ThrowExceptionForHR(-2147467259);
        }

        private void VerifySelfChildID(object varChild)
        {
            if (varChild is int && (AccChildID)varChild == AccChildID.Self)
                return;
            Marshal.ThrowExceptionForHR(-2147024809);
        }

        private int ProxyID
        {
            get
            {
                if (_proxyID == -1)
                {
                    _proxyID = ++s_proxyIDAllocator;
                    s_proxyFromID[_proxyID] = this;
                }
                return _proxyID;
            }
        }

        internal static AccessibleProxy AccessibleProxyFromID(int proxyID)
        {
            AccessibleProxy accessibleProxy;
            s_proxyFromID.TryGetValue(proxyID, out accessibleProxy);
            return accessibleProxy;
        }

        [DllImport("oleacc.dll")]
        internal static extern IntPtr LresultFromObject(
          ref Guid riid,
          IntPtr wParam,
          [MarshalAs(UnmanagedType.Interface)] object accPtr);

        [DllImport("oleacc.dll")]
        internal static extern int CreateStdAccessibleObject(
          IntPtr hwnd,
          int objectID,
          [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
          [MarshalAs(UnmanagedType.Interface)] out object accPtr);
    }
}
