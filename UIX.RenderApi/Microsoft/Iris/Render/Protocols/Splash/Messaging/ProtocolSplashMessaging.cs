// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Messaging.ProtocolSplashMessaging
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Protocols.Splash.Messaging
{
    internal sealed class ProtocolSplashMessaging : ProtocolInstance
    {
        private RENDERHANDLE _priv_remoteClass_Broker;
        private RENDERHANDLE _priv_remoteClass_Context;
        private RENDERHANDLE _priv_remoteClass_DataBuffer;
        private RENDERHANDLE _priv_remoteClass_ContextRelay;
        private RENDERHANDLE _priv_callbackInstance_RenderPortCallback;
        private RENDERHANDLE _priv_callbackInstance_DataBufferCallback;

        public static ProtocolSplashMessaging Bind(RenderPort port)
        {
            Debug2.Validate(port != null, typeof(ArgumentNullException), nameof(port));
            ProtocolInstance protocolInstance = port.LookUpProtocol("Splash::Messaging");
            if (protocolInstance != null)
            {
                ProtocolSplashMessaging protocolSplashMessaging = protocolInstance as ProtocolSplashMessaging;
                Debug2.Validate(protocolSplashMessaging != null, typeof(InvalidOperationException), "protocol name collision");
                return protocolSplashMessaging;
            }
            ProtocolSplashMessaging protocolSplashMessaging1 = new ProtocolSplashMessaging(port);
            port.BindProtocol(protocolSplashMessaging1);
            return protocolSplashMessaging1;
        }

        public LocalRenderPortCallback LocalRenderPortCallbackHandle => LocalRenderPortCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_RenderPortCallback);

        public LocalDataBufferCallback LocalDataBufferCallbackHandle => LocalDataBufferCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_DataBufferCallback);

        private ProtocolSplashMessaging(RenderPort port)
          : base(port, "Splash::Messaging")
        {
        }

        protected override void Init()
        {
            RenderPort port = this.Port;
            this._priv_remoteClass_Broker = port.InitRemoteClass("Splash::Messaging::Broker");
            this._priv_remoteClass_Context = port.InitRemoteClass("Splash::Messaging::Context");
            this._priv_remoteClass_DataBuffer = port.InitRemoteClass("Splash::Messaging::DataBuffer");
            this._priv_remoteClass_ContextRelay = port.InitRemoteClass("Splash::Messaging::ContextRelay");
            this._priv_callbackInstance_RenderPortCallback = LocalRenderPortCallback.BindCallback(port);
            this._priv_callbackInstance_DataBufferCallback = LocalDataBufferCallback.BindCallback(port);
        }

        internal RENDERHANDLE Broker_ClassHandle => this._priv_remoteClass_Broker;

        internal RENDERHANDLE Context_ClassHandle => this._priv_remoteClass_Context;

        internal RENDERHANDLE DataBuffer_ClassHandle => this._priv_remoteClass_DataBuffer;

        internal RENDERHANDLE ContextRelay_ClassHandle => this._priv_remoteClass_ContextRelay;

        internal RENDERHANDLE RenderPortCallback_CallbackInstance => this._priv_callbackInstance_RenderPortCallback;

        internal RENDERHANDLE DataBufferCallback_CallbackInstance => this._priv_callbackInstance_DataBufferCallback;

        internal RemoteContextRelay BuildRemoteContextRelay(
          IRenderHandleOwner _priv_owner,
          TransportProtocol protocol,
          string stServer,
          string stSession)
        {
            return RemoteContextRelay.Create(this, _priv_owner, protocol, stServer, stSession);
        }
    }
}
