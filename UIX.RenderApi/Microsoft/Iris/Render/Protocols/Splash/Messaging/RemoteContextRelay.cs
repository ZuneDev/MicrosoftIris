// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Messaging.RemoteContextRelay
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Messaging
{
    internal class RemoteContextRelay : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg2_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_UnlinkContext;
        private static ushort[] s_priv_ByteOrder_Msg1_LinkContext;

        protected RemoteContextRelay()
        {
        }

        public static unsafe RemoteContextRelay Create(
          ProtocolSplashMessaging _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          TransportProtocol protocol,
          string stServer,
          string stSession)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE relayClassHandle = _priv_protocolInstance.ContextRelay_ClassHandle;
            RemoteContextRelay remoteContextRelay = new RemoteContextRelay(port, _priv_owner);
            BlobInfo blobInfo = new BlobInfo(port, (uint)sizeof(RemoteContextRelay.Msg2_Create));
            BLOBREF blobref1 = blobInfo.Add(stServer);
            BLOBREF blobref2 = blobInfo.Add(stSession);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)adjustedTotalSize];
            RemoteContextRelay.Msg2_Create* msg2CreatePtr = (RemoteContextRelay.Msg2_Create*)pMem;
            msg2CreatePtr->_priv_size = adjustedTotalSize;
            msg2CreatePtr->_priv_msgid = 2U;
            msg2CreatePtr->protocol = protocol;
            msg2CreatePtr->stServer = blobref1;
            msg2CreatePtr->stSession = blobref2;
            blobInfo.Attach((Message*)msg2CreatePtr);
            msg2CreatePtr->_priv_idObjectSubject = remoteContextRelay.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg2_Create, typeof(RemoteContextRelay.Msg2_Create), 0, 0);
            port.CreateRemoteObject(relayClassHandle, remoteContextRelay.m_renderHandle, (Message*)msg2CreatePtr);
            return remoteContextRelay;
        }

        public static unsafe void BuildUnlinkContext(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          ContextID idContextExisting,
          ContextID idContextAlias)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE relayClassHandle = _priv_protocolInstance.ContextRelay_ClassHandle;
            _priv_portUse.ValidateHandle(relayClassHandle);
            uint size = (uint)sizeof(RemoteContextRelay.Msg0_UnlinkContext);
            RemoteContextRelay.Msg0_UnlinkContext* msg0UnlinkContextPtr = (RemoteContextRelay.Msg0_UnlinkContext*)_priv_portUse.AllocMessageBuffer(size);
            msg0UnlinkContextPtr->_priv_size = size;
            msg0UnlinkContextPtr->_priv_msgid = 0U;
            msg0UnlinkContextPtr->idContextExisting = idContextExisting;
            msg0UnlinkContextPtr->idContextAlias = idContextAlias;
            msg0UnlinkContextPtr->_priv_idObjectSubject = relayClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0UnlinkContextPtr, ref s_priv_ByteOrder_Msg0_UnlinkContext, typeof(RemoteContextRelay.Msg0_UnlinkContext), 0, 0);
            _priv_pmsgUse = (Message*)msg0UnlinkContextPtr;
        }

        public static unsafe void SendUnlinkContext(
          ProtocolSplashMessaging _priv_protocolInstance,
          ContextID idContextExisting,
          ContextID idContextAlias)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildUnlinkContext(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, idContextExisting, idContextAlias);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static unsafe void BuildLinkContext(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          ContextID idContextExisting,
          ContextID idContextAlias)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE relayClassHandle = _priv_protocolInstance.ContextRelay_ClassHandle;
            _priv_portUse.ValidateHandle(relayClassHandle);
            uint size = (uint)sizeof(RemoteContextRelay.Msg1_LinkContext);
            RemoteContextRelay.Msg1_LinkContext* msg1LinkContextPtr = (RemoteContextRelay.Msg1_LinkContext*)_priv_portUse.AllocMessageBuffer(size);
            msg1LinkContextPtr->_priv_size = size;
            msg1LinkContextPtr->_priv_msgid = 1U;
            msg1LinkContextPtr->idContextExisting = idContextExisting;
            msg1LinkContextPtr->idContextAlias = idContextAlias;
            msg1LinkContextPtr->_priv_idObjectSubject = relayClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1LinkContextPtr, ref s_priv_ByteOrder_Msg1_LinkContext, typeof(RemoteContextRelay.Msg1_LinkContext), 0, 0);
            _priv_pmsgUse = (Message*)msg1LinkContextPtr;
        }

        public static unsafe void SendLinkContext(
          ProtocolSplashMessaging _priv_protocolInstance,
          ContextID idContextExisting,
          ContextID idContextAlias)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildLinkContext(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, idContextExisting, idContextAlias);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteContextRelay CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteContextRelay(port, handle, true);
        }

        public static RemoteContextRelay CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteContextRelay(port, handle, false);
        }

        protected RemoteContextRelay(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteContextRelay(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteContextRelay && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg2_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public TransportProtocol protocol;
            public BLOBREF stServer;
            public BLOBREF stSession;
        }

        [ComVisible(false)]
        private struct Msg0_UnlinkContext
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ContextID idContextExisting;
            public ContextID idContextAlias;
        }

        [ComVisible(false)]
        private struct Msg1_LinkContext
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ContextID idContextExisting;
            public ContextID idContextAlias;
        }
    }
}
