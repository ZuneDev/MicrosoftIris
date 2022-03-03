// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.HwndHostWindow
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt;
using System;

namespace Microsoft.Iris.Render.Internal
{
    internal sealed class HwndHostWindow :
      RenderObject,
      IHwndHostWindow,
      IDisposable,
      IHwndHostWindowCallback,
      IRenderHandleOwner
    {
        private RemoteHwndHostWindow m_remoteWindow;
        private ColorF m_clrBackground;
        private HWND m_hwnd;
        private Point m_ptClientPosition;
        private Size m_sizeWindow;
        private bool m_fVisible;
        private EventHandler m_eventHandleChanged;

        public HwndHostWindow(RenderWindow winParent)
        {
            ProtocolSplashDesktopNt ntDesktopProtocol = winParent.Session.NtDesktopProtocol;
            this.m_remoteWindow = ntDesktopProtocol.BuildRemoteHwndHostWindow(this, winParent.RemoteStub, ntDesktopProtocol.LocalHwndHostWindowCallbackHandle);
            this.m_clrBackground = new ColorF(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        }

        protected override void Dispose(bool fInDispose)
        {
            if (fInDispose && this.m_remoteWindow != null)
            {
                this.m_remoteWindow.Dispose();
                this.m_remoteWindow = null;
            }
            base.Dispose(fInDispose);
        }

        ColorF IHwndHostWindow.BackgroundColor
        {
            get => this.m_clrBackground;
            set
            {
                if (!(this.m_clrBackground != value))
                    return;
                this.m_clrBackground = value;
                this.m_remoteWindow.SendSetBackgroundColor(value);
            }
        }

        Point IHwndHostWindow.ClientPosition
        {
            get => this.m_ptClientPosition;
            set
            {
                if (!(this.m_ptClientPosition != value))
                    return;
                this.m_ptClientPosition = value;
                this.m_remoteWindow.SendSetPosition(value);
            }
        }

        IntPtr IHwndHostWindow.Hwnd => this.m_hwnd.h;

        bool IHwndHostWindow.Visible
        {
            get => this.m_fVisible;
            set
            {
                if (this.m_fVisible == value)
                    return;
                this.m_fVisible = value;
                this.m_remoteWindow.SendSetVisible(value);
            }
        }

        Size IHwndHostWindow.WindowSize
        {
            get => this.m_sizeWindow;
            set
            {
                if (!(this.m_sizeWindow != value))
                    return;
                this.m_sizeWindow = value;
                this.m_remoteWindow.SendSetSize(value);
            }
        }

        event EventHandler IHwndHostWindow.OnHandleChanged
        {
            add => this.m_eventHandleChanged += value;
            remove => this.m_eventHandleChanged -= value;
        }

        void IHwndHostWindowCallback.OnHandleChanged(
          RENDERHANDLE target,
          HWND hWnd)
        {
            this.m_hwnd = hWnd;
            if (this.m_eventHandleChanged == null)
                return;
            this.m_eventHandleChanged(this, EventArgs.Empty);
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteWindow.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteWindow = null;
    }
}
