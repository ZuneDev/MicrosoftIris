// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.ProtocolSplashDesktop
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop
{
    internal sealed class ProtocolSplashDesktop : ProtocolInstance
    {
        private RENDERHANDLE _priv_remoteClass_InputRouter;
        private RENDERHANDLE _priv_remoteClass_FormWindowBase;
        private RENDERHANDLE _priv_callbackInstance_InputCallback;
        private RENDERHANDLE _priv_callbackInstance_FormWindowCallback;

        public static ProtocolSplashDesktop Bind(RenderPort port)
        {
            Debug2.Validate(port != null, typeof(ArgumentNullException), nameof(port));
            ProtocolInstance protocolInstance = port.LookUpProtocol("Splash::Desktop");
            if (protocolInstance != null)
            {
                ProtocolSplashDesktop protocolSplashDesktop = protocolInstance as ProtocolSplashDesktop;
                Debug2.Validate(protocolSplashDesktop != null, typeof(InvalidOperationException), "protocol name collision");
                return protocolSplashDesktop;
            }
            ProtocolSplashDesktop protocolSplashDesktop1 = new ProtocolSplashDesktop(port);
            port.BindProtocol(protocolSplashDesktop1);
            return protocolSplashDesktop1;
        }

        public LocalInputCallback LocalInputCallbackHandle => LocalInputCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_InputCallback);

        public LocalFormWindowCallback LocalFormWindowCallbackHandle => LocalFormWindowCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_FormWindowCallback);

        private ProtocolSplashDesktop(RenderPort port)
          : base(port, "Splash::Desktop")
        {
        }

        protected override void Init()
        {
            RenderPort port = this.Port;
            this._priv_remoteClass_InputRouter = port.InitRemoteClass("Splash::Desktop::InputRouter");
            this._priv_remoteClass_FormWindowBase = port.InitRemoteClass("Splash::Desktop::FormWindowBase");
            this._priv_callbackInstance_InputCallback = LocalInputCallback.BindCallback(port);
            this._priv_callbackInstance_FormWindowCallback = LocalFormWindowCallback.BindCallback(port);
        }

        internal RENDERHANDLE InputRouter_ClassHandle => this._priv_remoteClass_InputRouter;

        internal RENDERHANDLE FormWindowBase_ClassHandle => this._priv_remoteClass_FormWindowBase;

        internal RENDERHANDLE InputCallback_CallbackInstance => this._priv_callbackInstance_InputCallback;

        internal RENDERHANDLE FormWindowCallback_CallbackInstance => this._priv_callbackInstance_FormWindowCallback;

        internal RemoteInputRouter BuildRemoteInputRouter(
          IRenderHandleOwner _priv_owner,
          LocalInputCallback ic)
        {
            return RemoteInputRouter.Create(this, _priv_owner, ic);
        }
    }
}
