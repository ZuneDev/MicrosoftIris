// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteCamera
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteCamera : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg5_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_SetPerspective;
        private static ushort[] s_priv_ByteOrder_Msg1_SetZn;
        private static ushort[] s_priv_ByteOrder_Msg2_SetUp;
        private static ushort[] s_priv_ByteOrder_Msg3_SetAt;
        private static ushort[] s_priv_ByteOrder_Msg4_SetEye;

        protected RemoteCamera()
        {
        }

        public static unsafe RemoteCamera Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE cameraClassHandle = _priv_protocolInstance.Camera_ClassHandle;
            RemoteCamera remoteCamera = new RemoteCamera(port, _priv_owner);
            uint num = (uint)sizeof(RemoteCamera.Msg5_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteCamera.Msg5_Create* msg5CreatePtr = (RemoteCamera.Msg5_Create*)pMem;
            msg5CreatePtr->_priv_size = num;
            msg5CreatePtr->_priv_msgid = 5U;
            msg5CreatePtr->_priv_idObjectSubject = remoteCamera.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg5_Create, typeof(RemoteCamera.Msg5_Create), 0, 0);
            port.CreateRemoteObject(cameraClassHandle, remoteCamera.m_renderHandle, (Message*)msg5CreatePtr);
            return remoteCamera;
        }

        public unsafe void BuildSetPerspective(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fPerspective)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteCamera.Msg0_SetPerspective);
            RemoteCamera.Msg0_SetPerspective* msg0SetPerspectivePtr = (RemoteCamera.Msg0_SetPerspective*)_priv_portUse.AllocMessageBuffer(size);
            msg0SetPerspectivePtr->_priv_size = size;
            msg0SetPerspectivePtr->_priv_msgid = 0U;
            msg0SetPerspectivePtr->fPerspective = fPerspective ? uint.MaxValue : 0U;
            msg0SetPerspectivePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0SetPerspectivePtr, ref s_priv_ByteOrder_Msg0_SetPerspective, typeof(RemoteCamera.Msg0_SetPerspective), 0, 0);
            _priv_pmsgUse = (Message*)msg0SetPerspectivePtr;
        }

        public unsafe void SendSetPerspective(bool fPerspective)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetPerspective(out _priv_portUse, out _priv_pmsgUse, fPerspective);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetZn(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float flZn)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteCamera.Msg1_SetZn);
            RemoteCamera.Msg1_SetZn* msg1SetZnPtr = (RemoteCamera.Msg1_SetZn*)_priv_portUse.AllocMessageBuffer(size);
            msg1SetZnPtr->_priv_size = size;
            msg1SetZnPtr->_priv_msgid = 1U;
            msg1SetZnPtr->flZn = flZn;
            msg1SetZnPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1SetZnPtr, ref s_priv_ByteOrder_Msg1_SetZn, typeof(RemoteCamera.Msg1_SetZn), 0, 0);
            _priv_pmsgUse = (Message*)msg1SetZnPtr;
        }

        public unsafe void SendSetZn(float flZn)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetZn(out _priv_portUse, out _priv_pmsgUse, flZn);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetUp(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Vector3 vUp)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteCamera.Msg2_SetUp);
            RemoteCamera.Msg2_SetUp* msg2SetUpPtr = (RemoteCamera.Msg2_SetUp*)_priv_portUse.AllocMessageBuffer(size);
            msg2SetUpPtr->_priv_size = size;
            msg2SetUpPtr->_priv_msgid = 2U;
            msg2SetUpPtr->vUp = vUp;
            msg2SetUpPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2SetUpPtr, ref s_priv_ByteOrder_Msg2_SetUp, typeof(RemoteCamera.Msg2_SetUp), 0, 0);
            _priv_pmsgUse = (Message*)msg2SetUpPtr;
        }

        public unsafe void SendSetUp(Vector3 vUp)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetUp(out _priv_portUse, out _priv_pmsgUse, vUp);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetAt(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Vector3 vAt)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteCamera.Msg3_SetAt);
            RemoteCamera.Msg3_SetAt* msg3SetAtPtr = (RemoteCamera.Msg3_SetAt*)_priv_portUse.AllocMessageBuffer(size);
            msg3SetAtPtr->_priv_size = size;
            msg3SetAtPtr->_priv_msgid = 3U;
            msg3SetAtPtr->vAt = vAt;
            msg3SetAtPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3SetAtPtr, ref s_priv_ByteOrder_Msg3_SetAt, typeof(RemoteCamera.Msg3_SetAt), 0, 0);
            _priv_pmsgUse = (Message*)msg3SetAtPtr;
        }

        public unsafe void SendSetAt(Vector3 vAt)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetAt(out _priv_portUse, out _priv_pmsgUse, vAt);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetEye(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Vector3 vEye)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteCamera.Msg4_SetEye);
            RemoteCamera.Msg4_SetEye* msg4SetEyePtr = (RemoteCamera.Msg4_SetEye*)_priv_portUse.AllocMessageBuffer(size);
            msg4SetEyePtr->_priv_size = size;
            msg4SetEyePtr->_priv_msgid = 4U;
            msg4SetEyePtr->vEye = vEye;
            msg4SetEyePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4SetEyePtr, ref s_priv_ByteOrder_Msg4_SetEye, typeof(RemoteCamera.Msg4_SetEye), 0, 0);
            _priv_pmsgUse = (Message*)msg4SetEyePtr;
        }

        public unsafe void SendSetEye(Vector3 vEye)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEye(out _priv_portUse, out _priv_pmsgUse, vEye);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteCamera CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteCamera(port, handle, true);

        public static RemoteCamera CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteCamera(port, handle, false);
        }

        protected RemoteCamera(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteCamera(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteCamera && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg5_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg0_SetPerspective
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fPerspective;
        }

        [ComVisible(false)]
        private struct Msg1_SetZn
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float flZn;
        }

        [ComVisible(false)]
        private struct Msg2_SetUp
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Vector3 vUp;
        }

        [ComVisible(false)]
        private struct Msg3_SetAt
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Vector3 vAt;
        }

        [ComVisible(false)]
        private struct Msg4_SetEye
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Vector3 vEye;
        }
    }
}
