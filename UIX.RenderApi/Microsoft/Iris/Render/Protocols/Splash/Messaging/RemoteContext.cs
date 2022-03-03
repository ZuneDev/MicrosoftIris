// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Messaging.RemoteContext
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Messaging
{
    internal class RemoteContext : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_ReleaseObject;
        private static ushort[] s_priv_ByteOrder_Msg2_ForwardMessage;
        private static ushort[] s_priv_ByteOrder_Msg3_DestroyGroup;
        private static ushort[] s_priv_ByteOrder_Msg4_CreateGroup;

        protected RemoteContext()
        {
        }

        public static unsafe void BuildReleaseObject(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          IntPtr punkObj)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE contextClassHandle = _priv_protocolInstance.Context_ClassHandle;
            _priv_portUse.ValidateHandle(contextClassHandle);
            uint size = (uint)sizeof(RemoteContext.Msg0_ReleaseObject);
            RemoteContext.Msg0_ReleaseObject* msg0ReleaseObjectPtr = (RemoteContext.Msg0_ReleaseObject*)_priv_portUse.AllocMessageBuffer(size);
            msg0ReleaseObjectPtr->_priv_size = size;
            msg0ReleaseObjectPtr->_priv_msgid = 0U;
            msg0ReleaseObjectPtr->punkObj = punkObj;
            msg0ReleaseObjectPtr->_priv_idObjectSubject = contextClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0ReleaseObjectPtr, ref s_priv_ByteOrder_Msg0_ReleaseObject, typeof(RemoteContext.Msg0_ReleaseObject), 0, 0);
            _priv_pmsgUse = (Message*)msg0ReleaseObjectPtr;
        }

        public static unsafe void SendReleaseObject(
          ProtocolSplashMessaging _priv_protocolInstance,
          IntPtr punkObj)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildReleaseObject(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, punkObj);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static unsafe void BuildForwardMessage(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          ContextID idContextDest,
          Message* msgReturn)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE contextClassHandle = _priv_protocolInstance.Context_ClassHandle;
            _priv_portUse.ValidateHandle(contextClassHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteContext.Msg2_ForwardMessage));
            BLOBREF blobref = blobInfo.Add(msgReturn);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteContext.Msg2_ForwardMessage* msg2ForwardMessagePtr = (RemoteContext.Msg2_ForwardMessage*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg2ForwardMessagePtr->_priv_size = adjustedTotalSize;
            msg2ForwardMessagePtr->_priv_msgid = 2U;
            msg2ForwardMessagePtr->idContextDest = idContextDest;
            msg2ForwardMessagePtr->msgReturn = blobref;
            blobInfo.Attach((Message*)msg2ForwardMessagePtr);
            msg2ForwardMessagePtr->_priv_idObjectSubject = contextClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2ForwardMessagePtr, ref s_priv_ByteOrder_Msg2_ForwardMessage, typeof(RemoteContext.Msg2_ForwardMessage), 0, 0);
            _priv_pmsgUse = (Message*)msg2ForwardMessagePtr;
        }

        public static unsafe void SendForwardMessage(
          ProtocolSplashMessaging _priv_protocolInstance,
          ContextID idContextDest,
          Message* msgReturn)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildForwardMessage(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, idContextDest, msgReturn);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static unsafe void BuildDestroyGroup(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          int idxGroup)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE contextClassHandle = _priv_protocolInstance.Context_ClassHandle;
            _priv_portUse.ValidateHandle(contextClassHandle);
            uint size = (uint)sizeof(RemoteContext.Msg3_DestroyGroup);
            RemoteContext.Msg3_DestroyGroup* msg3DestroyGroupPtr = (RemoteContext.Msg3_DestroyGroup*)_priv_portUse.AllocMessageBuffer(size);
            msg3DestroyGroupPtr->_priv_size = size;
            msg3DestroyGroupPtr->_priv_msgid = 3U;
            msg3DestroyGroupPtr->idxGroup = idxGroup;
            msg3DestroyGroupPtr->_priv_idObjectSubject = contextClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3DestroyGroupPtr, ref s_priv_ByteOrder_Msg3_DestroyGroup, typeof(RemoteContext.Msg3_DestroyGroup), 0, 0);
            _priv_pmsgUse = (Message*)msg3DestroyGroupPtr;
        }

        public static unsafe void SendDestroyGroup(
          ProtocolSplashMessaging _priv_protocolInstance,
          int idxGroup)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildDestroyGroup(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, idxGroup);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static unsafe void BuildCreateGroup(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          int idxGroup,
          ContextID idContextOwner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE contextClassHandle = _priv_protocolInstance.Context_ClassHandle;
            _priv_portUse.ValidateHandle(contextClassHandle);
            uint size = (uint)sizeof(RemoteContext.Msg4_CreateGroup);
            RemoteContext.Msg4_CreateGroup* msg4CreateGroupPtr = (RemoteContext.Msg4_CreateGroup*)_priv_portUse.AllocMessageBuffer(size);
            msg4CreateGroupPtr->_priv_size = size;
            msg4CreateGroupPtr->_priv_msgid = 4U;
            msg4CreateGroupPtr->idxGroup = idxGroup;
            msg4CreateGroupPtr->idContextOwner = idContextOwner;
            msg4CreateGroupPtr->_priv_idObjectSubject = contextClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4CreateGroupPtr, ref s_priv_ByteOrder_Msg4_CreateGroup, typeof(RemoteContext.Msg4_CreateGroup), 0, 0);
            _priv_pmsgUse = (Message*)msg4CreateGroupPtr;
        }

        public static unsafe void SendCreateGroup(
          ProtocolSplashMessaging _priv_protocolInstance,
          int idxGroup,
          ContextID idContextOwner)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildCreateGroup(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, idxGroup, idContextOwner);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteContext CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteContext(port, handle, true);

        public static RemoteContext CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteContext(port, handle, false);
        }

        protected RemoteContext(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteContext(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteContext && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_ReleaseObject
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public IntPtr punkObj;
        }

        [ComVisible(false)]
        private struct Msg2_ForwardMessage
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ContextID idContextDest;
            public BLOBREF msgReturn;
        }

        [ComVisible(false)]
        private struct Msg3_DestroyGroup
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxGroup;
        }

        [ComVisible(false)]
        private struct Msg4_CreateGroup
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxGroup;
            public ContextID idContextOwner;
        }
    }
}
