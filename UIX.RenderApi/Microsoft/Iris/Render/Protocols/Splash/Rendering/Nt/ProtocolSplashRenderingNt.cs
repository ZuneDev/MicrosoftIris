// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt.ProtocolSplashRenderingNt
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt
{
    internal sealed class ProtocolSplashRenderingNt : ProtocolInstance
    {
        private RENDERHANDLE _priv_remoteClass_GdiDevice;
        private RENDERHANDLE _priv_remoteClass_NtDevice;
        private RENDERHANDLE _priv_remoteClass_GdiEffect;
        private RENDERHANDLE _priv_remoteClass_GdiSprite;
        private RENDERHANDLE _priv_remoteClass_WinSoundDevice;
        private RENDERHANDLE _priv_remoteClass_Ds8SoundDevice;

        public static ProtocolSplashRenderingNt Bind(RenderPort port)
        {
            Debug2.Validate(port != null, typeof(ArgumentNullException), nameof(port));
            ProtocolInstance protocolInstance = port.LookUpProtocol("Splash::Rendering::Nt");
            if (protocolInstance != null)
            {
                ProtocolSplashRenderingNt splashRenderingNt = protocolInstance as ProtocolSplashRenderingNt;
                Debug2.Validate(splashRenderingNt != null, typeof(InvalidOperationException), "protocol name collision");
                return splashRenderingNt;
            }
            ProtocolSplashRenderingNt splashRenderingNt1 = new ProtocolSplashRenderingNt(port);
            port.BindProtocol(splashRenderingNt1);
            return splashRenderingNt1;
        }

        private ProtocolSplashRenderingNt(RenderPort port)
          : base(port, "Splash::Rendering::Nt")
        {
        }

        protected override void Init()
        {
            RenderPort port = this.Port;
            this._priv_remoteClass_GdiDevice = port.InitRemoteClass("Splash::Rendering::Nt::GdiDevice");
            this._priv_remoteClass_NtDevice = port.InitRemoteClass("Splash::Rendering::Nt::NtDevice");
            this._priv_remoteClass_GdiEffect = port.InitRemoteClass("Splash::Rendering::Nt::GdiEffect");
            this._priv_remoteClass_GdiSprite = port.InitRemoteClass("Splash::Rendering::Nt::GdiSprite");
            this._priv_remoteClass_WinSoundDevice = port.InitRemoteClass("Splash::Rendering::Nt::WinSoundDevice");
            this._priv_remoteClass_Ds8SoundDevice = port.InitRemoteClass("Splash::Rendering::Nt::Ds8SoundDevice");
        }

        internal RENDERHANDLE GdiDevice_ClassHandle => this._priv_remoteClass_GdiDevice;

        internal RENDERHANDLE NtDevice_ClassHandle => this._priv_remoteClass_NtDevice;

        internal RENDERHANDLE GdiEffect_ClassHandle => this._priv_remoteClass_GdiEffect;

        internal RENDERHANDLE GdiSprite_ClassHandle => this._priv_remoteClass_GdiSprite;

        internal RENDERHANDLE WinSoundDevice_ClassHandle => this._priv_remoteClass_WinSoundDevice;

        internal RENDERHANDLE Ds8SoundDevice_ClassHandle => this._priv_remoteClass_Ds8SoundDevice;

        internal RemoteGdiDevice BuildRemoteGdiDevice(
          IRenderHandleOwner _priv_owner,
          LocalDeviceCallback cb)
        {
            return RemoteGdiDevice.Create(this, _priv_owner, cb);
        }

        internal RemoteNtDevice BuildRemoteNtDevice(
          IRenderHandleOwner _priv_owner,
          LocalDeviceCallback cb)
        {
            return RemoteNtDevice.Create(this, _priv_owner, cb);
        }

        internal RemoteGdiSprite BuildRemoteGdiSprite(IRenderHandleOwner _priv_owner) => RemoteGdiSprite.Create(this, _priv_owner);

        internal RemoteWinSoundDevice BuildRemoteWinSoundDevice(
          IRenderHandleOwner _priv_owner)
        {
            return RemoteWinSoundDevice.Create(this, _priv_owner);
        }

        internal RemoteDs8SoundDevice INPROC_BuildRemoteDs8SoundDevice(
          IRenderHandleOwner _priv_owner,
          HWND hwnd)
        {
            return RemoteDs8SoundDevice.INPROC_Create(this, _priv_owner, hwnd);
        }
    }
}
