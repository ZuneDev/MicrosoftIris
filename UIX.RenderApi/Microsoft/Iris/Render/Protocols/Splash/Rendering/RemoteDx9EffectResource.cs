// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteDx9EffectResource
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Messaging;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteDx9EffectResource : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg10_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_SetDebugID;
        private static ushort[] s_priv_ByteOrder_Msg1_SetGutterSize;
        private static ushort[] s_priv_ByteOrder_Msg2_PostPropertiesAdded;
        private static ushort[] s_priv_ByteOrder_Msg3_AddTextureRequirements;
        private static ushort[] s_priv_ByteOrder_Msg4_AddProperty;
        private static ushort[] s_priv_ByteOrder_Msg5_AddProperty;
        private static ushort[] s_priv_ByteOrder_Msg6_AddProperty;
        private static ushort[] s_priv_ByteOrder_Msg7_AddProperty;
        private static ushort[] s_priv_ByteOrder_Msg8_AddProperty;
        private static ushort[] s_priv_ByteOrder_Msg9_AddProperty;

        protected RemoteDx9EffectResource()
        {
        }

        public static unsafe RemoteDx9EffectResource Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          RemoteDevice devOwner,
          RemoteDataBuffer dataBuffer)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE resourceClassHandle = _priv_protocolInstance.Dx9EffectResource_ClassHandle;
            RemoteDx9EffectResource dx9EffectResource = new RemoteDx9EffectResource(port, _priv_owner);
            uint num = (uint)sizeof(RemoteDx9EffectResource.Msg10_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteDx9EffectResource.Msg10_Create* msg10CreatePtr = (RemoteDx9EffectResource.Msg10_Create*)pMem;
            msg10CreatePtr->_priv_size = num;
            msg10CreatePtr->_priv_msgid = 10U;
            msg10CreatePtr->devOwner = devOwner != null ? devOwner.RenderHandle : RENDERHANDLE.NULL;
            msg10CreatePtr->dataBuffer = dataBuffer != null ? dataBuffer.RenderHandle : RENDERHANDLE.NULL;
            msg10CreatePtr->_priv_idObjectSubject = dx9EffectResource.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg10_Create, typeof(RemoteDx9EffectResource.Msg10_Create), 0, 0);
            port.CreateRemoteObject(resourceClassHandle, dx9EffectResource.m_renderHandle, (Message*)msg10CreatePtr);
            return dx9EffectResource;
        }

        public unsafe void BuildSetDebugID(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stDebugID)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDx9EffectResource.Msg0_SetDebugID));
            BLOBREF blobref = blobInfo.Add(stDebugID);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDx9EffectResource.Msg0_SetDebugID* msg0SetDebugIdPtr = (RemoteDx9EffectResource.Msg0_SetDebugID*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg0SetDebugIdPtr->_priv_size = adjustedTotalSize;
            msg0SetDebugIdPtr->_priv_msgid = 0U;
            msg0SetDebugIdPtr->stDebugID = blobref;
            blobInfo.Attach((Message*)msg0SetDebugIdPtr);
            msg0SetDebugIdPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0SetDebugIdPtr, ref s_priv_ByteOrder_Msg0_SetDebugID, typeof(RemoteDx9EffectResource.Msg0_SetDebugID), 0, 0);
            _priv_pmsgUse = (Message*)msg0SetDebugIdPtr;
        }

        public unsafe void SendSetDebugID(string stDebugID)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetDebugID(out _priv_portUse, out _priv_pmsgUse, stDebugID);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetGutterSize(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizeGutterPxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDx9EffectResource.Msg1_SetGutterSize);
            RemoteDx9EffectResource.Msg1_SetGutterSize* msg1SetGutterSizePtr = (RemoteDx9EffectResource.Msg1_SetGutterSize*)_priv_portUse.AllocMessageBuffer(size);
            msg1SetGutterSizePtr->_priv_size = size;
            msg1SetGutterSizePtr->_priv_msgid = 1U;
            msg1SetGutterSizePtr->sizeGutterPxl = sizeGutterPxl;
            msg1SetGutterSizePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1SetGutterSizePtr, ref s_priv_ByteOrder_Msg1_SetGutterSize, typeof(RemoteDx9EffectResource.Msg1_SetGutterSize), 0, 0);
            _priv_pmsgUse = (Message*)msg1SetGutterSizePtr;
        }

        public unsafe void SendSetGutterSize(Size sizeGutterPxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetGutterSize(out _priv_portUse, out _priv_pmsgUse, sizeGutterPxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPostPropertiesAdded(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDx9EffectResource.Msg2_PostPropertiesAdded);
            RemoteDx9EffectResource.Msg2_PostPropertiesAdded* postPropertiesAddedPtr = (RemoteDx9EffectResource.Msg2_PostPropertiesAdded*)_priv_portUse.AllocMessageBuffer(size);
            postPropertiesAddedPtr->_priv_size = size;
            postPropertiesAddedPtr->_priv_msgid = 2U;
            postPropertiesAddedPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)postPropertiesAddedPtr, ref s_priv_ByteOrder_Msg2_PostPropertiesAdded, typeof(RemoteDx9EffectResource.Msg2_PostPropertiesAdded), 0, 0);
            _priv_pmsgUse = (Message*)postPropertiesAddedPtr;
        }

        public unsafe void SendPostPropertiesAdded()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPostPropertiesAdded(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddTextureRequirements(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxImage,
          int nRequirements,
          string stTexelSize,
          string stTexUVSize,
          string stTexUVRefPoint,
          int nDownsamplePropertyID)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDx9EffectResource.Msg3_AddTextureRequirements));
            BLOBREF blobref1 = blobInfo.Add(stTexelSize);
            BLOBREF blobref2 = blobInfo.Add(stTexUVSize);
            BLOBREF blobref3 = blobInfo.Add(stTexUVRefPoint);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDx9EffectResource.Msg3_AddTextureRequirements* textureRequirementsPtr = (RemoteDx9EffectResource.Msg3_AddTextureRequirements*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            textureRequirementsPtr->_priv_size = adjustedTotalSize;
            textureRequirementsPtr->_priv_msgid = 3U;
            textureRequirementsPtr->idxImage = idxImage;
            textureRequirementsPtr->nRequirements = nRequirements;
            textureRequirementsPtr->stTexelSize = blobref1;
            textureRequirementsPtr->stTexUVSize = blobref2;
            textureRequirementsPtr->stTexUVRefPoint = blobref3;
            textureRequirementsPtr->nDownsamplePropertyID = nDownsamplePropertyID;
            blobInfo.Attach((Message*)textureRequirementsPtr);
            textureRequirementsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)textureRequirementsPtr, ref s_priv_ByteOrder_Msg3_AddTextureRequirements, typeof(RemoteDx9EffectResource.Msg3_AddTextureRequirements), 0, 0);
            _priv_pmsgUse = (Message*)textureRequirementsPtr;
        }

        public unsafe void SendAddTextureRequirements(
          int idxImage,
          int nRequirements,
          string stTexelSize,
          string stTexUVSize,
          string stTexUVRefPoint,
          int nDownsamplePropertyID)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddTextureRequirements(out _priv_portUse, out _priv_pmsgUse, idxImage, nRequirements, stTexelSize, stTexUVSize, stTexUVRefPoint, nDownsamplePropertyID);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stName,
          int nCoordinateMapID,
          int nImageIndexID)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDx9EffectResource.Msg4_AddProperty));
            BLOBREF blobref = blobInfo.Add(stName);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDx9EffectResource.Msg4_AddProperty* msg4AddPropertyPtr = (RemoteDx9EffectResource.Msg4_AddProperty*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg4AddPropertyPtr->_priv_size = adjustedTotalSize;
            msg4AddPropertyPtr->_priv_msgid = 4U;
            msg4AddPropertyPtr->stName = blobref;
            msg4AddPropertyPtr->nCoordinateMapID = nCoordinateMapID;
            msg4AddPropertyPtr->nImageIndexID = nImageIndexID;
            blobInfo.Attach((Message*)msg4AddPropertyPtr);
            msg4AddPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4AddPropertyPtr, ref s_priv_ByteOrder_Msg4_AddProperty, typeof(RemoteDx9EffectResource.Msg4_AddProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg4AddPropertyPtr;
        }

        public unsafe void SendAddProperty(string stName, int nCoordinateMapID, int nImageIndexID)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddProperty(out _priv_portUse, out _priv_pmsgUse, stName, nCoordinateMapID, nImageIndexID);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stName,
          Vector4 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDx9EffectResource.Msg5_AddProperty));
            BLOBREF blobref = blobInfo.Add(stName);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDx9EffectResource.Msg5_AddProperty* msg5AddPropertyPtr = (RemoteDx9EffectResource.Msg5_AddProperty*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg5AddPropertyPtr->_priv_size = adjustedTotalSize;
            msg5AddPropertyPtr->_priv_msgid = 5U;
            msg5AddPropertyPtr->stName = blobref;
            msg5AddPropertyPtr->vValue = vValue;
            blobInfo.Attach((Message*)msg5AddPropertyPtr);
            msg5AddPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg5AddPropertyPtr, ref s_priv_ByteOrder_Msg5_AddProperty, typeof(RemoteDx9EffectResource.Msg5_AddProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg5AddPropertyPtr;
        }

        public unsafe void SendAddProperty(string stName, Vector4 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddProperty(out _priv_portUse, out _priv_pmsgUse, stName, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stName,
          Vector3 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDx9EffectResource.Msg6_AddProperty));
            BLOBREF blobref = blobInfo.Add(stName);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDx9EffectResource.Msg6_AddProperty* msg6AddPropertyPtr = (RemoteDx9EffectResource.Msg6_AddProperty*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg6AddPropertyPtr->_priv_size = adjustedTotalSize;
            msg6AddPropertyPtr->_priv_msgid = 6U;
            msg6AddPropertyPtr->stName = blobref;
            msg6AddPropertyPtr->vValue = vValue;
            blobInfo.Attach((Message*)msg6AddPropertyPtr);
            msg6AddPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg6AddPropertyPtr, ref s_priv_ByteOrder_Msg6_AddProperty, typeof(RemoteDx9EffectResource.Msg6_AddProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg6AddPropertyPtr;
        }

        public unsafe void SendAddProperty(string stName, Vector3 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddProperty(out _priv_portUse, out _priv_pmsgUse, stName, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stName,
          Vector2 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDx9EffectResource.Msg7_AddProperty));
            BLOBREF blobref = blobInfo.Add(stName);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDx9EffectResource.Msg7_AddProperty* msg7AddPropertyPtr = (RemoteDx9EffectResource.Msg7_AddProperty*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg7AddPropertyPtr->_priv_size = adjustedTotalSize;
            msg7AddPropertyPtr->_priv_msgid = 7U;
            msg7AddPropertyPtr->stName = blobref;
            msg7AddPropertyPtr->vValue = vValue;
            blobInfo.Attach((Message*)msg7AddPropertyPtr);
            msg7AddPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg7AddPropertyPtr, ref s_priv_ByteOrder_Msg7_AddProperty, typeof(RemoteDx9EffectResource.Msg7_AddProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg7AddPropertyPtr;
        }

        public unsafe void SendAddProperty(string stName, Vector2 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddProperty(out _priv_portUse, out _priv_pmsgUse, stName, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stName,
          float flValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDx9EffectResource.Msg8_AddProperty));
            BLOBREF blobref = blobInfo.Add(stName);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDx9EffectResource.Msg8_AddProperty* msg8AddPropertyPtr = (RemoteDx9EffectResource.Msg8_AddProperty*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg8AddPropertyPtr->_priv_size = adjustedTotalSize;
            msg8AddPropertyPtr->_priv_msgid = 8U;
            msg8AddPropertyPtr->stName = blobref;
            msg8AddPropertyPtr->flValue = flValue;
            blobInfo.Attach((Message*)msg8AddPropertyPtr);
            msg8AddPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg8AddPropertyPtr, ref s_priv_ByteOrder_Msg8_AddProperty, typeof(RemoteDx9EffectResource.Msg8_AddProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg8AddPropertyPtr;
        }

        public unsafe void SendAddProperty(string stName, float flValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddProperty(out _priv_portUse, out _priv_pmsgUse, stName, flValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stName,
          int nValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDx9EffectResource.Msg9_AddProperty));
            BLOBREF blobref = blobInfo.Add(stName);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDx9EffectResource.Msg9_AddProperty* msg9AddPropertyPtr = (RemoteDx9EffectResource.Msg9_AddProperty*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg9AddPropertyPtr->_priv_size = adjustedTotalSize;
            msg9AddPropertyPtr->_priv_msgid = 9U;
            msg9AddPropertyPtr->stName = blobref;
            msg9AddPropertyPtr->nValue = nValue;
            blobInfo.Attach((Message*)msg9AddPropertyPtr);
            msg9AddPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg9AddPropertyPtr, ref s_priv_ByteOrder_Msg9_AddProperty, typeof(RemoteDx9EffectResource.Msg9_AddProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg9AddPropertyPtr;
        }

        public unsafe void SendAddProperty(string stName, int nValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddProperty(out _priv_portUse, out _priv_pmsgUse, stName, nValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteDx9EffectResource CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteDx9EffectResource(port, handle, true);
        }

        public static RemoteDx9EffectResource CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDx9EffectResource(port, handle, false);
        }

        protected RemoteDx9EffectResource(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDx9EffectResource(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDx9EffectResource && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg10_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE devOwner;
            public RENDERHANDLE dataBuffer;
        }

        [ComVisible(false)]
        private struct Msg0_SetDebugID
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stDebugID;
        }

        [ComVisible(false)]
        private struct Msg1_SetGutterSize
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizeGutterPxl;
        }

        [ComVisible(false)]
        private struct Msg2_PostPropertiesAdded
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg3_AddTextureRequirements
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxImage;
            public int nRequirements;
            public BLOBREF stTexelSize;
            public BLOBREF stTexUVSize;
            public BLOBREF stTexUVRefPoint;
            public int nDownsamplePropertyID;
        }

        [ComVisible(false)]
        private struct Msg4_AddProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stName;
            public int nCoordinateMapID;
            public int nImageIndexID;
        }

        [ComVisible(false)]
        private struct Msg5_AddProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stName;
            public Vector4 vValue;
        }

        [ComVisible(false)]
        private struct Msg6_AddProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stName;
            public Vector3 vValue;
        }

        [ComVisible(false)]
        private struct Msg7_AddProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stName;
            public Vector2 vValue;
        }

        [ComVisible(false)]
        private struct Msg8_AddProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stName;
            public float flValue;
        }

        [ComVisible(false)]
        private struct Msg9_AddProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stName;
            public int nValue;
        }
    }
}
