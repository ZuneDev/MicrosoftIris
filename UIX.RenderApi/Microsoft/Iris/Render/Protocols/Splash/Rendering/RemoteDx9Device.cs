// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteDx9Device
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteDx9Device : RemoteDevice
    {
        private static ushort[] s_priv_ByteOrder_Msg8_CreateVideoPool;
        private static ushort[] s_priv_ByteOrder_Msg9_EndVideoSurfaceAllocation;
        private static ushort[] s_priv_ByteOrder_Msg10_BeginVideoSurfaceAllocation;

        protected RemoteDx9Device()
        {
        }

        public unsafe void BuildCreateVideoPool(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          LocalVideoPoolCallback cbOwner,
          RENDERHANDLE idNewSurface)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDx9Device.Msg8_CreateVideoPool);
            RemoteDx9Device.Msg8_CreateVideoPool* msg8CreateVideoPoolPtr = (RemoteDx9Device.Msg8_CreateVideoPool*)_priv_portUse.AllocMessageBuffer(size);
            msg8CreateVideoPoolPtr->_priv_size = size;
            msg8CreateVideoPoolPtr->_priv_msgid = 8U;
            msg8CreateVideoPoolPtr->_priv_objcbOwner = cbOwner.RenderHandle;
            msg8CreateVideoPoolPtr->_priv_ctxcbOwner = _priv_portUse.Session.LocalContext;
            msg8CreateVideoPoolPtr->idNewSurface = idNewSurface;
            msg8CreateVideoPoolPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg8CreateVideoPoolPtr, ref s_priv_ByteOrder_Msg8_CreateVideoPool, typeof(RemoteDx9Device.Msg8_CreateVideoPool), 0, 0);
            _priv_pmsgUse = (Message*)msg8CreateVideoPoolPtr;
        }

        public unsafe void SendCreateVideoPool(
          LocalVideoPoolCallback cbOwner,
          RENDERHANDLE idNewSurface)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateVideoPool(out _priv_portUse, out _priv_pmsgUse, cbOwner, idNewSurface);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildEndVideoSurfaceAllocation(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDx9Device.Msg9_EndVideoSurfaceAllocation);
            RemoteDx9Device.Msg9_EndVideoSurfaceAllocation* surfaceAllocationPtr = (RemoteDx9Device.Msg9_EndVideoSurfaceAllocation*)_priv_portUse.AllocMessageBuffer(size);
            surfaceAllocationPtr->_priv_size = size;
            surfaceAllocationPtr->_priv_msgid = 9U;
            surfaceAllocationPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)surfaceAllocationPtr, ref s_priv_ByteOrder_Msg9_EndVideoSurfaceAllocation, typeof(RemoteDx9Device.Msg9_EndVideoSurfaceAllocation), 0, 0);
            _priv_pmsgUse = (Message*)surfaceAllocationPtr;
        }

        public unsafe void SendEndVideoSurfaceAllocation()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildEndVideoSurfaceAllocation(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildBeginVideoSurfaceAllocation(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDx9Device.Msg10_BeginVideoSurfaceAllocation);
            RemoteDx9Device.Msg10_BeginVideoSurfaceAllocation* surfaceAllocationPtr = (RemoteDx9Device.Msg10_BeginVideoSurfaceAllocation*)_priv_portUse.AllocMessageBuffer(size);
            surfaceAllocationPtr->_priv_size = size;
            surfaceAllocationPtr->_priv_msgid = 10U;
            surfaceAllocationPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)surfaceAllocationPtr, ref s_priv_ByteOrder_Msg10_BeginVideoSurfaceAllocation, typeof(RemoteDx9Device.Msg10_BeginVideoSurfaceAllocation), 0, 0);
            _priv_pmsgUse = (Message*)surfaceAllocationPtr;
        }

        public unsafe void SendBeginVideoSurfaceAllocation()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildBeginVideoSurfaceAllocation(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteDx9Device CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteDx9Device(port, handle, true);
        }

        public static RemoteDx9Device CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDx9Device(port, handle, false);
        }

        protected RemoteDx9Device(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDx9Device(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDx9Device && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg8_CreateVideoPool
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objcbOwner;
            public ContextID _priv_ctxcbOwner;
            public RENDERHANDLE idNewSurface;
        }

        [ComVisible(false)]
        private struct Msg9_EndVideoSurfaceAllocation
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg10_BeginVideoSurfaceAllocation
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }
    }
}
