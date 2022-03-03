// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Messaging.LocalRenderPortCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Messaging
{
    internal class LocalRenderPortCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnBatchProcessed;
        private static ushort[] s_priv_ByteOrder_Msg1_OnPingReply;

        protected LocalRenderPortCallback()
        {
        }

        public static unsafe void AutoSendOnBatchProcessed(
          ProtocolSplashMessaging _priv_protocolInstance,
          RENDERHANDLE target,
          uint uBatchCompleted)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE callbackInstance = _priv_protocolInstance.RenderPortCallback_CallbackInstance;
            uint num = (uint)sizeof(LocalRenderPortCallback.Msg0_OnBatchProcessed);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            LocalRenderPortCallback.Msg0_OnBatchProcessed* onBatchProcessedPtr = (LocalRenderPortCallback.Msg0_OnBatchProcessed*)pMem;
            onBatchProcessedPtr->_priv_size = num;
            onBatchProcessedPtr->_priv_msgid = 0U;
            onBatchProcessedPtr->target = target;
            onBatchProcessedPtr->uBatchCompleted = uBatchCompleted;
            onBatchProcessedPtr->_priv_idObjectSubject = callbackInstance;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg0_OnBatchProcessed, typeof(LocalRenderPortCallback.Msg0_OnBatchProcessed), 0, 0);
            port.SendLocalCallbackMessage((Message*)onBatchProcessedPtr);
        }

        public static unsafe void AutoSendOnPingReply(
          ProtocolSplashMessaging _priv_protocolInstance,
          RENDERHANDLE target)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE callbackInstance = _priv_protocolInstance.RenderPortCallback_CallbackInstance;
            uint num = (uint)sizeof(LocalRenderPortCallback.Msg1_OnPingReply);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            LocalRenderPortCallback.Msg1_OnPingReply* msg1OnPingReplyPtr = (LocalRenderPortCallback.Msg1_OnPingReply*)pMem;
            msg1OnPingReplyPtr->_priv_size = num;
            msg1OnPingReplyPtr->_priv_msgid = 1U;
            msg1OnPingReplyPtr->target = target;
            msg1OnPingReplyPtr->_priv_idObjectSubject = callbackInstance;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg1_OnPingReply, typeof(LocalRenderPortCallback.Msg1_OnPingReply), 0, 0);
            port.SendLocalCallbackMessage((Message*)msg1OnPingReplyPtr);
        }

        internal static LocalRenderPortCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalRenderPortCallback(port, callbackInstance);
        }

        protected LocalRenderPortCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalRenderPortCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is IRenderPortCallback _priv_target))
                return;
            switch (message->nMsg)
            {
                case 0:
                    Dispatch_OnBatchProcessed(port, _priv_target, (LocalRenderPortCallback.Msg0_OnBatchProcessed*)message);
                    break;
                case 1:
                    Dispatch_OnPingReply(port, _priv_target, (LocalRenderPortCallback.Msg1_OnPingReply*)message);
                    break;
            }
        }

        private static unsafe void Dispatch_OnBatchProcessed(
          RenderPort _priv_port,
          IRenderPortCallback _priv_target,
          LocalRenderPortCallback.Msg0_OnBatchProcessed* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnBatchProcessed, typeof(LocalRenderPortCallback.Msg0_OnBatchProcessed), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            uint uBatchCompleted = _priv_pmsg->uBatchCompleted;
            _priv_target.OnBatchProcessed(target, uBatchCompleted);
        }

        private static unsafe void Dispatch_OnPingReply(
          RenderPort _priv_port,
          IRenderPortCallback _priv_target,
          LocalRenderPortCallback.Msg1_OnPingReply* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg1_OnPingReply, typeof(LocalRenderPortCallback.Msg1_OnPingReply), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnPingReply(target);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnBatchProcessed
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint uBatchCompleted;
        }

        [ComVisible(false)]
        private struct Msg1_OnPingReply
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }
    }
}
