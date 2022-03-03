// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Xenon.ProtocolSplashRenderingXenon
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Xenon
{
    internal sealed class ProtocolSplashRenderingXenon : ProtocolInstance
    {
        private RENDERHANDLE _priv_remoteClass_XeDevice;
        private RENDERHANDLE _priv_remoteClass_XAudSoundDevice;

        public static ProtocolSplashRenderingXenon Bind(RenderPort port)
        {
            Debug2.Validate(port != null, typeof(ArgumentNullException), nameof(port));
            ProtocolInstance protocolInstance = port.LookUpProtocol("Splash::Rendering::Xenon");
            if (protocolInstance != null)
            {
                ProtocolSplashRenderingXenon splashRenderingXenon = protocolInstance as ProtocolSplashRenderingXenon;
                Debug2.Validate(splashRenderingXenon != null, typeof(InvalidOperationException), "protocol name collision");
                return splashRenderingXenon;
            }
            ProtocolSplashRenderingXenon splashRenderingXenon1 = new ProtocolSplashRenderingXenon(port);
            port.BindProtocol(splashRenderingXenon1);
            return splashRenderingXenon1;
        }

        private ProtocolSplashRenderingXenon(RenderPort port)
          : base(port, "Splash::Rendering::Xenon")
        {
        }

        protected override void Init()
        {
            RenderPort port = this.Port;
            this._priv_remoteClass_XeDevice = port.InitRemoteClass("Splash::Rendering::Xenon::XeDevice");
            this._priv_remoteClass_XAudSoundDevice = port.InitRemoteClass("Splash::Rendering::Xenon::XAudSoundDevice");
        }

        internal RENDERHANDLE XeDevice_ClassHandle => this._priv_remoteClass_XeDevice;

        internal RENDERHANDLE XAudSoundDevice_ClassHandle => this._priv_remoteClass_XAudSoundDevice;

        internal RemoteXeDevice BuildRemoteXeDevice(
          IRenderHandleOwner _priv_owner,
          LocalDeviceCallback cb,
          Size sizeScreenPxl)
        {
            return RemoteXeDevice.Create(this, _priv_owner, cb, sizeScreenPxl);
        }

        internal RemoteXAudSoundDevice BuildRemoteXAudSoundDevice(
          IRenderHandleOwner _priv_owner)
        {
            return RemoteXAudSoundDevice.Create(this, _priv_owner);
        }
    }
}
