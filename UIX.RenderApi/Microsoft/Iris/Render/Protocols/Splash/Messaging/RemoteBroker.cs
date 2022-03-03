// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Messaging.RemoteBroker
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Messaging
{
    internal class RemoteBroker : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_DestroyObject;
        private static ushort[] s_priv_ByteOrder_Msg1_CreateObject;
        private static ushort[] s_priv_ByteOrder_Msg2_CreateClass;

        protected RemoteBroker()
        {
        }

        public static unsafe void BuildDestroyObject(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          RENDERHANDLE idObject)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE brokerClassHandle = _priv_protocolInstance.Broker_ClassHandle;
            _priv_portUse.ValidateHandle(brokerClassHandle);
            uint size = (uint)sizeof(RemoteBroker.Msg0_DestroyObject);
            RemoteBroker.Msg0_DestroyObject* msg0DestroyObjectPtr = (RemoteBroker.Msg0_DestroyObject*)_priv_portUse.AllocMessageBuffer(size);
            msg0DestroyObjectPtr->_priv_size = size;
            msg0DestroyObjectPtr->_priv_msgid = 0U;
            msg0DestroyObjectPtr->idObject = idObject;
            msg0DestroyObjectPtr->_priv_idObjectSubject = brokerClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0DestroyObjectPtr, ref s_priv_ByteOrder_Msg0_DestroyObject, typeof(RemoteBroker.Msg0_DestroyObject), 0, 0);
            _priv_pmsgUse = (Message*)msg0DestroyObjectPtr;
        }

        public static unsafe void SendDestroyObject(
          ProtocolSplashMessaging _priv_protocolInstance,
          RENDERHANDLE idObject)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildDestroyObject(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, idObject);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static unsafe void BuildCreateObject(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          RENDERHANDLE idObjectClass,
          RENDERHANDLE idObjectNew,
          Message* msgConstruction)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE brokerClassHandle = _priv_protocolInstance.Broker_ClassHandle;
            _priv_portUse.ValidateHandle(brokerClassHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteBroker.Msg1_CreateObject));
            BLOBREF blobref = blobInfo.Add(msgConstruction);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteBroker.Msg1_CreateObject* msg1CreateObjectPtr = (RemoteBroker.Msg1_CreateObject*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg1CreateObjectPtr->_priv_size = adjustedTotalSize;
            msg1CreateObjectPtr->_priv_msgid = 1U;
            msg1CreateObjectPtr->idObjectClass = idObjectClass;
            msg1CreateObjectPtr->idObjectNew = idObjectNew;
            msg1CreateObjectPtr->msgConstruction = blobref;
            blobInfo.Attach((Message*)msg1CreateObjectPtr);
            msg1CreateObjectPtr->_priv_idObjectSubject = brokerClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1CreateObjectPtr, ref s_priv_ByteOrder_Msg1_CreateObject, typeof(RemoteBroker.Msg1_CreateObject), 0, 0);
            _priv_pmsgUse = (Message*)msg1CreateObjectPtr;
        }

        public static unsafe void SendCreateObject(
          ProtocolSplashMessaging _priv_protocolInstance,
          RENDERHANDLE idObjectClass,
          RENDERHANDLE idObjectNew,
          Message* msgConstruction)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildCreateObject(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, idObjectClass, idObjectNew, msgConstruction);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static unsafe void BuildCreateClass(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashMessaging _priv_protocolInstance,
          string stClassName,
          RENDERHANDLE idObjectClass)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE brokerClassHandle = _priv_protocolInstance.Broker_ClassHandle;
            _priv_portUse.ValidateHandle(brokerClassHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteBroker.Msg2_CreateClass));
            BLOBREF blobref = blobInfo.Add(stClassName);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteBroker.Msg2_CreateClass* msg2CreateClassPtr = (RemoteBroker.Msg2_CreateClass*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg2CreateClassPtr->_priv_size = adjustedTotalSize;
            msg2CreateClassPtr->_priv_msgid = 2U;
            msg2CreateClassPtr->stClassName = blobref;
            msg2CreateClassPtr->idObjectClass = idObjectClass;
            blobInfo.Attach((Message*)msg2CreateClassPtr);
            msg2CreateClassPtr->_priv_idObjectSubject = brokerClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2CreateClassPtr, ref s_priv_ByteOrder_Msg2_CreateClass, typeof(RemoteBroker.Msg2_CreateClass), 0, 0);
            _priv_pmsgUse = (Message*)msg2CreateClassPtr;
        }

        public static unsafe void SendCreateClass(
          ProtocolSplashMessaging _priv_protocolInstance,
          string stClassName,
          RENDERHANDLE idObjectClass)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildCreateClass(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, stClassName, idObjectClass);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteBroker CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteBroker(port, handle, true);

        public static RemoteBroker CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteBroker(port, handle, false);
        }

        protected RemoteBroker(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteBroker(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteBroker && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_DestroyObject
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idObject;
        }

        [ComVisible(false)]
        private struct Msg1_CreateObject
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idObjectClass;
            public RENDERHANDLE idObjectNew;
            public BLOBREF msgConstruction;
        }

        [ComVisible(false)]
        private struct Msg2_CreateClass
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stClassName;
            public RENDERHANDLE idObjectClass;
        }
    }
}
