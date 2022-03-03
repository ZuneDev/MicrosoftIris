// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteGradient
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteGradient : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_Clear;
        private static ushort[] s_priv_ByteOrder_Msg1_AddValue;
        private static ushort[] s_priv_ByteOrder_Msg2_SetOffset;
        private static ushort[] s_priv_ByteOrder_Msg3_SetColorMask;
        private static ushort[] s_priv_ByteOrder_Msg4_SetOrientation;

        protected RemoteGradient()
        {
        }

        public unsafe void BuildClear(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGradient.Msg0_Clear);
            RemoteGradient.Msg0_Clear* msg0ClearPtr = (RemoteGradient.Msg0_Clear*)_priv_portUse.AllocMessageBuffer(size);
            msg0ClearPtr->_priv_size = size;
            msg0ClearPtr->_priv_msgid = 0U;
            msg0ClearPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0ClearPtr, ref s_priv_ByteOrder_Msg0_Clear, typeof(RemoteGradient.Msg0_Clear), 0, 0);
            _priv_pmsgUse = (Message*)msg0ClearPtr;
        }

        public unsafe void SendClear()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildClear(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddValue(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float flValue,
          float flPosition,
          RelativeSpace relative)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGradient.Msg1_AddValue);
            RemoteGradient.Msg1_AddValue* msg1AddValuePtr = (RemoteGradient.Msg1_AddValue*)_priv_portUse.AllocMessageBuffer(size);
            msg1AddValuePtr->_priv_size = size;
            msg1AddValuePtr->_priv_msgid = 1U;
            msg1AddValuePtr->flValue = flValue;
            msg1AddValuePtr->flPosition = flPosition;
            msg1AddValuePtr->relative = relative;
            msg1AddValuePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1AddValuePtr, ref s_priv_ByteOrder_Msg1_AddValue, typeof(RemoteGradient.Msg1_AddValue), 0, 0);
            _priv_pmsgUse = (Message*)msg1AddValuePtr;
        }

        public unsafe void SendAddValue(float flValue, float flPosition, RelativeSpace relative)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddValue(out _priv_portUse, out _priv_pmsgUse, flValue, flPosition, relative);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetOffset(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float flOffset)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGradient.Msg2_SetOffset);
            RemoteGradient.Msg2_SetOffset* msg2SetOffsetPtr = (RemoteGradient.Msg2_SetOffset*)_priv_portUse.AllocMessageBuffer(size);
            msg2SetOffsetPtr->_priv_size = size;
            msg2SetOffsetPtr->_priv_msgid = 2U;
            msg2SetOffsetPtr->flOffset = flOffset;
            msg2SetOffsetPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2SetOffsetPtr, ref s_priv_ByteOrder_Msg2_SetOffset, typeof(RemoteGradient.Msg2_SetOffset), 0, 0);
            _priv_pmsgUse = (Message*)msg2SetOffsetPtr;
        }

        public unsafe void SendSetOffset(float flOffset)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetOffset(out _priv_portUse, out _priv_pmsgUse, flOffset);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetColorMask(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ColorF clrMask)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGradient.Msg3_SetColorMask);
            RemoteGradient.Msg3_SetColorMask* msg3SetColorMaskPtr = (RemoteGradient.Msg3_SetColorMask*)_priv_portUse.AllocMessageBuffer(size);
            msg3SetColorMaskPtr->_priv_size = size;
            msg3SetColorMaskPtr->_priv_msgid = 3U;
            msg3SetColorMaskPtr->clrMask = clrMask;
            msg3SetColorMaskPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3SetColorMaskPtr, ref s_priv_ByteOrder_Msg3_SetColorMask, typeof(RemoteGradient.Msg3_SetColorMask), 0, 0);
            _priv_pmsgUse = (Message*)msg3SetColorMaskPtr;
        }

        public unsafe void SendSetColorMask(ColorF clrMask)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetColorMask(out _priv_portUse, out _priv_pmsgUse, clrMask);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetOrientation(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Orientation dir)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGradient.Msg4_SetOrientation);
            RemoteGradient.Msg4_SetOrientation* msg4SetOrientationPtr = (RemoteGradient.Msg4_SetOrientation*)_priv_portUse.AllocMessageBuffer(size);
            msg4SetOrientationPtr->_priv_size = size;
            msg4SetOrientationPtr->_priv_msgid = 4U;
            msg4SetOrientationPtr->dir = dir;
            msg4SetOrientationPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4SetOrientationPtr, ref s_priv_ByteOrder_Msg4_SetOrientation, typeof(RemoteGradient.Msg4_SetOrientation), 0, 0);
            _priv_pmsgUse = (Message*)msg4SetOrientationPtr;
        }

        public unsafe void SendSetOrientation(Orientation dir)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetOrientation(out _priv_portUse, out _priv_pmsgUse, dir);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteGradient CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteGradient(port, handle, true);
        }

        public static RemoteGradient CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteGradient(port, handle, false);
        }

        protected RemoteGradient(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteGradient(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteGradient && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_Clear
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg1_AddValue
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float flValue;
            public float flPosition;
            public RelativeSpace relative;
        }

        [ComVisible(false)]
        private struct Msg2_SetOffset
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float flOffset;
        }

        [ComVisible(false)]
        private struct Msg3_SetColorMask
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ColorF clrMask;
        }

        [ComVisible(false)]
        private struct Msg4_SetOrientation
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Orientation dir;
        }
    }
}
