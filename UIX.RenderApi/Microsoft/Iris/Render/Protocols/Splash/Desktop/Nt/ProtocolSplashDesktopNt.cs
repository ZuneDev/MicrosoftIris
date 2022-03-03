// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt.ProtocolSplashDesktopNt
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt
{
    internal sealed class ProtocolSplashDesktopNt : ProtocolInstance
    {
        private RENDERHANDLE _priv_remoteClass_FormWindow;
        private RENDERHANDLE _priv_remoteClass_HwndHostWindow;
        private RENDERHANDLE _priv_remoteClass_DesktopManager;
        private RENDERHANDLE _priv_callbackInstance_HwndHostWindowCallback;
        private RENDERHANDLE _priv_callbackInstance_DesktopManagerCallback;

        public static ProtocolSplashDesktopNt Bind(RenderPort port)
        {
            Debug2.Validate(port != null, typeof(ArgumentNullException), nameof(port));
            ProtocolInstance protocolInstance = port.LookUpProtocol("Splash::Desktop::Nt");
            if (protocolInstance != null)
            {
                ProtocolSplashDesktopNt protocolSplashDesktopNt = protocolInstance as ProtocolSplashDesktopNt;
                Debug2.Validate(protocolSplashDesktopNt != null, typeof(InvalidOperationException), "protocol name collision");
                return protocolSplashDesktopNt;
            }
            ProtocolSplashDesktopNt protocolSplashDesktopNt1 = new ProtocolSplashDesktopNt(port);
            port.BindProtocol(protocolSplashDesktopNt1);
            return protocolSplashDesktopNt1;
        }

        public LocalHwndHostWindowCallback LocalHwndHostWindowCallbackHandle => LocalHwndHostWindowCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_HwndHostWindowCallback);

        public LocalDesktopManagerCallback LocalDesktopManagerCallbackHandle => LocalDesktopManagerCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_DesktopManagerCallback);

        private ProtocolSplashDesktopNt(RenderPort port)
          : base(port, "Splash::Desktop::Nt")
        {
        }

        protected override void Init()
        {
            RenderPort port = this.Port;
            this._priv_remoteClass_FormWindow = port.InitRemoteClass("Splash::Desktop::Nt::FormWindow");
            this._priv_remoteClass_HwndHostWindow = port.InitRemoteClass("Splash::Desktop::Nt::HwndHostWindow");
            this._priv_remoteClass_DesktopManager = port.InitRemoteClass("Splash::Desktop::Nt::DesktopManager");
            this._priv_callbackInstance_HwndHostWindowCallback = LocalHwndHostWindowCallback.BindCallback(port);
            this._priv_callbackInstance_DesktopManagerCallback = LocalDesktopManagerCallback.BindCallback(port);
        }

        internal RENDERHANDLE FormWindow_ClassHandle => this._priv_remoteClass_FormWindow;

        internal RENDERHANDLE HwndHostWindow_ClassHandle => this._priv_remoteClass_HwndHostWindow;

        internal RENDERHANDLE DesktopManager_ClassHandle => this._priv_remoteClass_DesktopManager;

        internal RENDERHANDLE HwndHostWindowCallback_CallbackInstance => this._priv_callbackInstance_HwndHostWindowCallback;

        internal RENDERHANDLE DesktopManagerCallback_CallbackInstance => this._priv_callbackInstance_DesktopManagerCallback;

        internal RemoteFormWindow BuildRemoteFormWindow(
          IRenderHandleOwner _priv_owner,
          RemoteDesktopManager manager,
          LocalFormWindowCallback cb)
        {
            return RemoteFormWindow.Create(this, _priv_owner, manager, cb);
        }

        internal RemoteHwndHostWindow BuildRemoteHwndHostWindow(
          IRenderHandleOwner _priv_owner,
          RemoteFormWindow winParent,
          LocalHwndHostWindowCallback cb)
        {
            return RemoteHwndHostWindow.Create(this, _priv_owner, winParent, cb);
        }

        internal RemoteDesktopManager BuildRemoteDesktopManager(
          IRenderHandleOwner _priv_owner,
          LocalDesktopManagerCallback cb,
          bool fEnumDisplayModes)
        {
            return RemoteDesktopManager.Create(this, _priv_owner, cb, fEnumDisplayModes);
        }
    }
}
