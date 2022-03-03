// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Xenon.ProtocolSplashDesktopXenon
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Xenon
{
    internal sealed class ProtocolSplashDesktopXenon : ProtocolInstance
    {
        private RENDERHANDLE _priv_remoteClass_FormWindow;

        public static ProtocolSplashDesktopXenon Bind(RenderPort port)
        {
            Debug2.Validate(port != null, typeof(ArgumentNullException), nameof(port));
            ProtocolInstance protocolInstance = port.LookUpProtocol("Splash::Desktop::Xenon");
            if (protocolInstance != null)
            {
                ProtocolSplashDesktopXenon splashDesktopXenon = protocolInstance as ProtocolSplashDesktopXenon;
                Debug2.Validate(splashDesktopXenon != null, typeof(InvalidOperationException), "protocol name collision");
                return splashDesktopXenon;
            }
            ProtocolSplashDesktopXenon splashDesktopXenon1 = new ProtocolSplashDesktopXenon(port);
            port.BindProtocol(splashDesktopXenon1);
            return splashDesktopXenon1;
        }

        private ProtocolSplashDesktopXenon(RenderPort port)
          : base(port, "Splash::Desktop::Xenon")
        {
        }

        protected override void Init() => this._priv_remoteClass_FormWindow = this.Port.InitRemoteClass("Splash::Desktop::Xenon::FormWindow");

        internal RENDERHANDLE FormWindow_ClassHandle => this._priv_remoteClass_FormWindow;

        internal RemoteFormWindow BuildRemoteFormWindow(
          IRenderHandleOwner _priv_owner,
          LocalFormWindowCallback cb)
        {
            return RemoteFormWindow.Create(this, _priv_owner, cb);
        }
    }
}
