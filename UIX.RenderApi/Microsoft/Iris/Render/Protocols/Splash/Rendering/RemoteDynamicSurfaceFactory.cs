// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteDynamicSurfaceFactory
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteDynamicSurfaceFactory : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_CloseInstance;
        private static ushort[] s_priv_ByteOrder_Msg1_CreateVideoInstance;
        private static ushort[] s_priv_ByteOrder_Msg2_CreateSurfaceInstance;

        protected RemoteDynamicSurfaceFactory()
        {
        }

        public static unsafe void BuildCloseInstance(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashRendering _priv_protocolInstance,
          int nUniqueID)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE factoryClassHandle = _priv_protocolInstance.DynamicSurfaceFactory_ClassHandle;
            _priv_portUse.ValidateHandle(factoryClassHandle);
            uint size = (uint)sizeof(RemoteDynamicSurfaceFactory.Msg0_CloseInstance);
            RemoteDynamicSurfaceFactory.Msg0_CloseInstance* msg0CloseInstancePtr = (RemoteDynamicSurfaceFactory.Msg0_CloseInstance*)_priv_portUse.AllocMessageBuffer(size);
            msg0CloseInstancePtr->_priv_size = size;
            msg0CloseInstancePtr->_priv_msgid = 0U;
            msg0CloseInstancePtr->nUniqueID = nUniqueID;
            msg0CloseInstancePtr->_priv_idObjectSubject = factoryClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0CloseInstancePtr, ref s_priv_ByteOrder_Msg0_CloseInstance, typeof(RemoteDynamicSurfaceFactory.Msg0_CloseInstance), 0, 0);
            _priv_pmsgUse = (Message*)msg0CloseInstancePtr;
        }

        public static unsafe void SendCloseInstance(
          ProtocolSplashRendering _priv_protocolInstance,
          int nUniqueID)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildCloseInstance(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, nUniqueID);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static unsafe void BuildCreateVideoInstance(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashRendering _priv_protocolInstance,
          int nUniqueID,
          RENDERHANDLE idClassContext,
          RemoteDevice devOwner,
          RemoteSurface surScene,
          RemoteVideoPool poolScene)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE factoryClassHandle = _priv_protocolInstance.DynamicSurfaceFactory_ClassHandle;
            _priv_portUse.ValidateHandle(factoryClassHandle);
            if (devOwner != null)
                _priv_portUse.ValidateHandleOrNull(devOwner.RenderHandle);
            if (surScene != null)
                _priv_portUse.ValidateHandleOrNull(surScene.RenderHandle);
            if (poolScene != null)
                _priv_portUse.ValidateHandleOrNull(poolScene.RenderHandle);
            uint size = (uint)sizeof(RemoteDynamicSurfaceFactory.Msg1_CreateVideoInstance);
            RemoteDynamicSurfaceFactory.Msg1_CreateVideoInstance* createVideoInstancePtr = (RemoteDynamicSurfaceFactory.Msg1_CreateVideoInstance*)_priv_portUse.AllocMessageBuffer(size);
            createVideoInstancePtr->_priv_size = size;
            createVideoInstancePtr->_priv_msgid = 1U;
            createVideoInstancePtr->nUniqueID = nUniqueID;
            createVideoInstancePtr->idClassContext = idClassContext;
            createVideoInstancePtr->devOwner = devOwner != null ? devOwner.RenderHandle : RENDERHANDLE.NULL;
            createVideoInstancePtr->surScene = surScene != null ? surScene.RenderHandle : RENDERHANDLE.NULL;
            createVideoInstancePtr->poolScene = poolScene != null ? poolScene.RenderHandle : RENDERHANDLE.NULL;
            createVideoInstancePtr->_priv_idObjectSubject = factoryClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)createVideoInstancePtr, ref s_priv_ByteOrder_Msg1_CreateVideoInstance, typeof(RemoteDynamicSurfaceFactory.Msg1_CreateVideoInstance), 0, 0);
            _priv_pmsgUse = (Message*)createVideoInstancePtr;
        }

        public static unsafe void SendCreateVideoInstance(
          ProtocolSplashRendering _priv_protocolInstance,
          int nUniqueID,
          RENDERHANDLE idClassContext,
          RemoteDevice devOwner,
          RemoteSurface surScene,
          RemoteVideoPool poolScene)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildCreateVideoInstance(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, nUniqueID, idClassContext, devOwner, surScene, poolScene);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static unsafe void BuildCreateSurfaceInstance(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashRendering _priv_protocolInstance,
          int nUniqueID,
          RENDERHANDLE idClassContext,
          RemoteDevice devOwner,
          RemoteSurface surScene,
          RemoteVideoPool poolScene)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE factoryClassHandle = _priv_protocolInstance.DynamicSurfaceFactory_ClassHandle;
            _priv_portUse.ValidateHandle(factoryClassHandle);
            if (devOwner != null)
                _priv_portUse.ValidateHandleOrNull(devOwner.RenderHandle);
            if (surScene != null)
                _priv_portUse.ValidateHandleOrNull(surScene.RenderHandle);
            if (poolScene != null)
                _priv_portUse.ValidateHandleOrNull(poolScene.RenderHandle);
            uint size = (uint)sizeof(RemoteDynamicSurfaceFactory.Msg2_CreateSurfaceInstance);
            RemoteDynamicSurfaceFactory.Msg2_CreateSurfaceInstance* createSurfaceInstancePtr = (RemoteDynamicSurfaceFactory.Msg2_CreateSurfaceInstance*)_priv_portUse.AllocMessageBuffer(size);
            createSurfaceInstancePtr->_priv_size = size;
            createSurfaceInstancePtr->_priv_msgid = 2U;
            createSurfaceInstancePtr->nUniqueID = nUniqueID;
            createSurfaceInstancePtr->idClassContext = idClassContext;
            createSurfaceInstancePtr->devOwner = devOwner != null ? devOwner.RenderHandle : RENDERHANDLE.NULL;
            createSurfaceInstancePtr->surScene = surScene != null ? surScene.RenderHandle : RENDERHANDLE.NULL;
            createSurfaceInstancePtr->poolScene = poolScene != null ? poolScene.RenderHandle : RENDERHANDLE.NULL;
            createSurfaceInstancePtr->_priv_idObjectSubject = factoryClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)createSurfaceInstancePtr, ref s_priv_ByteOrder_Msg2_CreateSurfaceInstance, typeof(RemoteDynamicSurfaceFactory.Msg2_CreateSurfaceInstance), 0, 0);
            _priv_pmsgUse = (Message*)createSurfaceInstancePtr;
        }

        public static unsafe void SendCreateSurfaceInstance(
          ProtocolSplashRendering _priv_protocolInstance,
          int nUniqueID,
          RENDERHANDLE idClassContext,
          RemoteDevice devOwner,
          RemoteSurface surScene,
          RemoteVideoPool poolScene)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildCreateSurfaceInstance(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, nUniqueID, idClassContext, devOwner, surScene, poolScene);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteDynamicSurfaceFactory CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteDynamicSurfaceFactory(port, handle, true);
        }

        public static RemoteDynamicSurfaceFactory CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDynamicSurfaceFactory(port, handle, false);
        }

        protected RemoteDynamicSurfaceFactory(
          RenderPort port,
          RENDERHANDLE handle,
          bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDynamicSurfaceFactory(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDynamicSurfaceFactory && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_CloseInstance
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nUniqueID;
        }

        [ComVisible(false)]
        private struct Msg1_CreateVideoInstance
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nUniqueID;
            public RENDERHANDLE idClassContext;
            public RENDERHANDLE devOwner;
            public RENDERHANDLE surScene;
            public RENDERHANDLE poolScene;
        }

        [ComVisible(false)]
        private struct Msg2_CreateSurfaceInstance
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nUniqueID;
            public RENDERHANDLE idClassContext;
            public RENDERHANDLE devOwner;
            public RENDERHANDLE surScene;
            public RENDERHANDLE poolScene;
        }
    }
}
