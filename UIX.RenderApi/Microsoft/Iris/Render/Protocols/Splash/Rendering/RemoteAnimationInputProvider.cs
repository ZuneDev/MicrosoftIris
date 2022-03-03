// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteAnimationInputProvider
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteAnimationInputProvider : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg10_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_RevokeQuaternion;
        private static ushort[] s_priv_ByteOrder_Msg1_RevokeVector4;
        private static ushort[] s_priv_ByteOrder_Msg2_RevokeVector3;
        private static ushort[] s_priv_ByteOrder_Msg3_RevokeVector2;
        private static ushort[] s_priv_ByteOrder_Msg4_RevokeFloat;
        private static ushort[] s_priv_ByteOrder_Msg5_PublishQuaternion;
        private static ushort[] s_priv_ByteOrder_Msg6_PublishVector4;
        private static ushort[] s_priv_ByteOrder_Msg7_PublishVector3;
        private static ushort[] s_priv_ByteOrder_Msg8_PublishVector2;
        private static ushort[] s_priv_ByteOrder_Msg9_PublishFloat;

        protected RemoteAnimationInputProvider()
        {
        }

        public static unsafe RemoteAnimationInputProvider Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          uint nExternalAnimationInputId)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE providerClassHandle = _priv_protocolInstance.AnimationInputProvider_ClassHandle;
            RemoteAnimationInputProvider animationInputProvider = new RemoteAnimationInputProvider(port, _priv_owner);
            uint num = (uint)sizeof(RemoteAnimationInputProvider.Msg10_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteAnimationInputProvider.Msg10_Create* msg10CreatePtr = (RemoteAnimationInputProvider.Msg10_Create*)pMem;
            msg10CreatePtr->_priv_size = num;
            msg10CreatePtr->_priv_msgid = 10U;
            msg10CreatePtr->nExternalAnimationInputId = nExternalAnimationInputId;
            msg10CreatePtr->_priv_idObjectSubject = animationInputProvider.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg10_Create, typeof(RemoteAnimationInputProvider.Msg10_Create), 0, 0);
            port.CreateRemoteObject(providerClassHandle, animationInputProvider.m_renderHandle, (Message*)msg10CreatePtr);
            return animationInputProvider;
        }

        public unsafe void BuildRevokeQuaternion(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg0_RevokeQuaternion);
            RemoteAnimationInputProvider.Msg0_RevokeQuaternion* revokeQuaternionPtr = (RemoteAnimationInputProvider.Msg0_RevokeQuaternion*)_priv_portUse.AllocMessageBuffer(size);
            revokeQuaternionPtr->_priv_size = size;
            revokeQuaternionPtr->_priv_msgid = 0U;
            revokeQuaternionPtr->propertyId = propertyId;
            revokeQuaternionPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)revokeQuaternionPtr, ref s_priv_ByteOrder_Msg0_RevokeQuaternion, typeof(RemoteAnimationInputProvider.Msg0_RevokeQuaternion), 0, 0);
            _priv_pmsgUse = (Message*)revokeQuaternionPtr;
        }

        public unsafe void SendRevokeQuaternion(uint propertyId)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRevokeQuaternion(out _priv_portUse, out _priv_pmsgUse, propertyId);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRevokeVector4(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg1_RevokeVector4);
            RemoteAnimationInputProvider.Msg1_RevokeVector4* msg1RevokeVector4Ptr = (RemoteAnimationInputProvider.Msg1_RevokeVector4*)_priv_portUse.AllocMessageBuffer(size);
            msg1RevokeVector4Ptr->_priv_size = size;
            msg1RevokeVector4Ptr->_priv_msgid = 1U;
            msg1RevokeVector4Ptr->propertyId = propertyId;
            msg1RevokeVector4Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1RevokeVector4Ptr, ref s_priv_ByteOrder_Msg1_RevokeVector4, typeof(RemoteAnimationInputProvider.Msg1_RevokeVector4), 0, 0);
            _priv_pmsgUse = (Message*)msg1RevokeVector4Ptr;
        }

        public unsafe void SendRevokeVector4(uint propertyId)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRevokeVector4(out _priv_portUse, out _priv_pmsgUse, propertyId);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRevokeVector3(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg2_RevokeVector3);
            RemoteAnimationInputProvider.Msg2_RevokeVector3* msg2RevokeVector3Ptr = (RemoteAnimationInputProvider.Msg2_RevokeVector3*)_priv_portUse.AllocMessageBuffer(size);
            msg2RevokeVector3Ptr->_priv_size = size;
            msg2RevokeVector3Ptr->_priv_msgid = 2U;
            msg2RevokeVector3Ptr->propertyId = propertyId;
            msg2RevokeVector3Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2RevokeVector3Ptr, ref s_priv_ByteOrder_Msg2_RevokeVector3, typeof(RemoteAnimationInputProvider.Msg2_RevokeVector3), 0, 0);
            _priv_pmsgUse = (Message*)msg2RevokeVector3Ptr;
        }

        public unsafe void SendRevokeVector3(uint propertyId)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRevokeVector3(out _priv_portUse, out _priv_pmsgUse, propertyId);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRevokeVector2(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg3_RevokeVector2);
            RemoteAnimationInputProvider.Msg3_RevokeVector2* msg3RevokeVector2Ptr = (RemoteAnimationInputProvider.Msg3_RevokeVector2*)_priv_portUse.AllocMessageBuffer(size);
            msg3RevokeVector2Ptr->_priv_size = size;
            msg3RevokeVector2Ptr->_priv_msgid = 3U;
            msg3RevokeVector2Ptr->propertyId = propertyId;
            msg3RevokeVector2Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3RevokeVector2Ptr, ref s_priv_ByteOrder_Msg3_RevokeVector2, typeof(RemoteAnimationInputProvider.Msg3_RevokeVector2), 0, 0);
            _priv_pmsgUse = (Message*)msg3RevokeVector2Ptr;
        }

        public unsafe void SendRevokeVector2(uint propertyId)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRevokeVector2(out _priv_portUse, out _priv_pmsgUse, propertyId);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRevokeFloat(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg4_RevokeFloat);
            RemoteAnimationInputProvider.Msg4_RevokeFloat* msg4RevokeFloatPtr = (RemoteAnimationInputProvider.Msg4_RevokeFloat*)_priv_portUse.AllocMessageBuffer(size);
            msg4RevokeFloatPtr->_priv_size = size;
            msg4RevokeFloatPtr->_priv_msgid = 4U;
            msg4RevokeFloatPtr->propertyId = propertyId;
            msg4RevokeFloatPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4RevokeFloatPtr, ref s_priv_ByteOrder_Msg4_RevokeFloat, typeof(RemoteAnimationInputProvider.Msg4_RevokeFloat), 0, 0);
            _priv_pmsgUse = (Message*)msg4RevokeFloatPtr;
        }

        public unsafe void SendRevokeFloat(uint propertyId)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRevokeFloat(out _priv_portUse, out _priv_pmsgUse, propertyId);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPublishQuaternion(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId,
          Quaternion value)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg5_PublishQuaternion);
            RemoteAnimationInputProvider.Msg5_PublishQuaternion* publishQuaternionPtr = (RemoteAnimationInputProvider.Msg5_PublishQuaternion*)_priv_portUse.AllocMessageBuffer(size);
            publishQuaternionPtr->_priv_size = size;
            publishQuaternionPtr->_priv_msgid = 5U;
            publishQuaternionPtr->propertyId = propertyId;
            publishQuaternionPtr->value = value;
            publishQuaternionPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)publishQuaternionPtr, ref s_priv_ByteOrder_Msg5_PublishQuaternion, typeof(RemoteAnimationInputProvider.Msg5_PublishQuaternion), 0, 0);
            _priv_pmsgUse = (Message*)publishQuaternionPtr;
        }

        public unsafe void SendPublishQuaternion(uint propertyId, Quaternion value)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPublishQuaternion(out _priv_portUse, out _priv_pmsgUse, propertyId, value);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPublishVector4(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId,
          Vector4 value)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg6_PublishVector4);
            RemoteAnimationInputProvider.Msg6_PublishVector4* msg6PublishVector4Ptr = (RemoteAnimationInputProvider.Msg6_PublishVector4*)_priv_portUse.AllocMessageBuffer(size);
            msg6PublishVector4Ptr->_priv_size = size;
            msg6PublishVector4Ptr->_priv_msgid = 6U;
            msg6PublishVector4Ptr->propertyId = propertyId;
            msg6PublishVector4Ptr->value = value;
            msg6PublishVector4Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg6PublishVector4Ptr, ref s_priv_ByteOrder_Msg6_PublishVector4, typeof(RemoteAnimationInputProvider.Msg6_PublishVector4), 0, 0);
            _priv_pmsgUse = (Message*)msg6PublishVector4Ptr;
        }

        public unsafe void SendPublishVector4(uint propertyId, Vector4 value)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPublishVector4(out _priv_portUse, out _priv_pmsgUse, propertyId, value);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPublishVector3(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId,
          Vector3 value)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg7_PublishVector3);
            RemoteAnimationInputProvider.Msg7_PublishVector3* msg7PublishVector3Ptr = (RemoteAnimationInputProvider.Msg7_PublishVector3*)_priv_portUse.AllocMessageBuffer(size);
            msg7PublishVector3Ptr->_priv_size = size;
            msg7PublishVector3Ptr->_priv_msgid = 7U;
            msg7PublishVector3Ptr->propertyId = propertyId;
            msg7PublishVector3Ptr->value = value;
            msg7PublishVector3Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg7PublishVector3Ptr, ref s_priv_ByteOrder_Msg7_PublishVector3, typeof(RemoteAnimationInputProvider.Msg7_PublishVector3), 0, 0);
            _priv_pmsgUse = (Message*)msg7PublishVector3Ptr;
        }

        public unsafe void SendPublishVector3(uint propertyId, Vector3 value)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPublishVector3(out _priv_portUse, out _priv_pmsgUse, propertyId, value);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPublishVector2(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId,
          Vector2 value)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg8_PublishVector2);
            RemoteAnimationInputProvider.Msg8_PublishVector2* msg8PublishVector2Ptr = (RemoteAnimationInputProvider.Msg8_PublishVector2*)_priv_portUse.AllocMessageBuffer(size);
            msg8PublishVector2Ptr->_priv_size = size;
            msg8PublishVector2Ptr->_priv_msgid = 8U;
            msg8PublishVector2Ptr->propertyId = propertyId;
            msg8PublishVector2Ptr->value = value;
            msg8PublishVector2Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg8PublishVector2Ptr, ref s_priv_ByteOrder_Msg8_PublishVector2, typeof(RemoteAnimationInputProvider.Msg8_PublishVector2), 0, 0);
            _priv_pmsgUse = (Message*)msg8PublishVector2Ptr;
        }

        public unsafe void SendPublishVector2(uint propertyId, Vector2 value)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPublishVector2(out _priv_portUse, out _priv_pmsgUse, propertyId, value);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPublishFloat(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint propertyId,
          float value)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationInputProvider.Msg9_PublishFloat);
            RemoteAnimationInputProvider.Msg9_PublishFloat* msg9PublishFloatPtr = (RemoteAnimationInputProvider.Msg9_PublishFloat*)_priv_portUse.AllocMessageBuffer(size);
            msg9PublishFloatPtr->_priv_size = size;
            msg9PublishFloatPtr->_priv_msgid = 9U;
            msg9PublishFloatPtr->propertyId = propertyId;
            msg9PublishFloatPtr->value = value;
            msg9PublishFloatPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg9PublishFloatPtr, ref s_priv_ByteOrder_Msg9_PublishFloat, typeof(RemoteAnimationInputProvider.Msg9_PublishFloat), 0, 0);
            _priv_pmsgUse = (Message*)msg9PublishFloatPtr;
        }

        public unsafe void SendPublishFloat(uint propertyId, float value)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPublishFloat(out _priv_portUse, out _priv_pmsgUse, propertyId, value);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteAnimationInputProvider CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteAnimationInputProvider(port, handle, true);
        }

        public static RemoteAnimationInputProvider CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteAnimationInputProvider(port, handle, false);
        }

        protected RemoteAnimationInputProvider(
          RenderPort port,
          RENDERHANDLE handle,
          bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteAnimationInputProvider(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteAnimationInputProvider && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg10_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nExternalAnimationInputId;
        }

        [ComVisible(false)]
        private struct Msg0_RevokeQuaternion
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
        }

        [ComVisible(false)]
        private struct Msg1_RevokeVector4
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
        }

        [ComVisible(false)]
        private struct Msg2_RevokeVector3
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
        }

        [ComVisible(false)]
        private struct Msg3_RevokeVector2
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
        }

        [ComVisible(false)]
        private struct Msg4_RevokeFloat
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
        }

        [ComVisible(false)]
        private struct Msg5_PublishQuaternion
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
            public Quaternion value;
        }

        [ComVisible(false)]
        private struct Msg6_PublishVector4
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
            public Vector4 value;
        }

        [ComVisible(false)]
        private struct Msg7_PublishVector3
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
            public Vector3 value;
        }

        [ComVisible(false)]
        private struct Msg8_PublishVector2
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
            public Vector2 value;
        }

        [ComVisible(false)]
        private struct Msg9_PublishFloat
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint propertyId;
            public float value;
        }
    }
}
