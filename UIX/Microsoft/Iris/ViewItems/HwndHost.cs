// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.HwndHost
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.ViewItems
{
    internal class HwndHost : ContentViewItem
    {
        private IHwndHostWindow _window;
        private Color _backgroundColor;
        private long _childWindowHwnd;
        private IntPtr _childHwndIntPtr;

        public HwndHost()
        {
            ((ITrackableUIElementEvents)this).UIChange += new EventHandler(OnUIChange);
            IRenderWindow renderWindow = UISession.Default.GetRenderWindow();
            _window = renderWindow.CreateHwndHostWindow();
            _window.OnHandleChanged += new EventHandler(OnHandleChanged);
            renderWindow.ForwardMessageEvent += new ForwardMessageHandler(OnForwardMessage);
            _backgroundColor = Color.White;
            _window.BackgroundColor = new ColorF(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        }

        protected override void OnDispose()
        {
            IRenderWindow renderWindow = UISession.Default.GetRenderWindow();
            if (renderWindow != null)
                renderWindow.ForwardMessageEvent -= new ForwardMessageHandler(OnForwardMessage);
            if (_window != null)
            {
                _window.OnHandleChanged -= new EventHandler(OnHandleChanged);
                _window.Dispose();
                _window = null;
            }
          ((ITrackableUIElementEvents)this).UIChange -= new EventHandler(OnUIChange);
            base.OnDispose();
        }

        public long Handle => _window.Hwnd.ToInt64();

        public long ChildHandle
        {
            get => _childWindowHwnd;
            set
            {
                if (value == _childWindowHwnd)
                    return;
                _childWindowHwnd = value;
                _childHwndIntPtr = (IntPtr)ChildHandle;
            }
        }

        public override Color Background
        {
            get => _backgroundColor;
            set
            {
                if (!(_backgroundColor != value))
                    return;
                if (value.A != byte.MaxValue)
                {
                    ErrorManager.ReportError("HwndHost.Background must be a solid color");
                }
                else
                {
                    _backgroundColor = value;
                    _window.BackgroundColor = value.RenderConvert();
                    FireNotification(NotificationID.Background);
                }
            }
        }

        protected override bool HasContent() => true;

        private void OnHandleChanged(object sender, EventArgs args) => FireNotification(NotificationID.Handle);

        private unsafe void OnForwardMessage(uint message, IntPtr wParam, IntPtr lParam)
        {
            if (!(_childHwndIntPtr != IntPtr.Zero))
                return;
            Win32Api.MSG msg;
            msg.message = message;
            msg.wParam = wParam;
            msg.lParam = lParam;
            Win32Api.MSG* msgPtr = &msg;
            Win32Api.SendMessage(_childHwndIntPtr, 895U, IntPtr.Zero, (IntPtr)msgPtr);
        }

        private void OnUIChange(object sender, EventArgs args)
        {
            if (_window == null || !IsZoned || !HasVisual)
                return;
            Vector3 parentOffsetPxlVector;
            Vector3 scaleVector;
            GetAccumulatedOffsetAndScale(this, null, out parentOffsetPxlVector, out scaleVector);
            Vector2 visualSize = VisualSize;
            _window.ClientPosition = new Point((int)Math.Round(parentOffsetPxlVector.X), (int)Math.Round(parentOffsetPxlVector.Y));
            _window.WindowSize = new Size(Math2.RoundUp(scaleVector.X * visualSize.X), Math2.RoundUp(scaleVector.Y * visualSize.Y));
            _window.Visible = FullyVisible;
        }
    }
}
