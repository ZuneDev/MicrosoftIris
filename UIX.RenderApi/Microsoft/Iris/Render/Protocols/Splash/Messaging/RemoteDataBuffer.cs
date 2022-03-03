// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Messaging.RemoteDataBuffer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Messaging
{
    internal class RemoteDataBuffer : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_RegisterOwner;

        protected RemoteDataBuffer()
        {
        }

        public unsafe void BuildRegisterOwner(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          LocalDataBufferCallback cb)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDataBuffer.Msg0_RegisterOwner);
            RemoteDataBuffer.Msg0_RegisterOwner* msg0RegisterOwnerPtr = (RemoteDataBuffer.Msg0_RegisterOwner*)_priv_portUse.AllocMessageBuffer(size);
            msg0RegisterOwnerPtr->_priv_size = size;
            msg0RegisterOwnerPtr->_priv_msgid = 0U;
            msg0RegisterOwnerPtr->_priv_objcb = cb.RenderHandle;
            msg0RegisterOwnerPtr->_priv_ctxcb = _priv_portUse.Session.LocalContext;
            msg0RegisterOwnerPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0RegisterOwnerPtr, ref s_priv_ByteOrder_Msg0_RegisterOwner, typeof(RemoteDataBuffer.Msg0_RegisterOwner), 0, 0);
            _priv_pmsgUse = (Message*)msg0RegisterOwnerPtr;
        }

        public unsafe void SendRegisterOwner(LocalDataBufferCallback cb)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRegisterOwner(out _priv_portUse, out _priv_pmsgUse, cb);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteDataBuffer CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteDataBuffer(port, handle, true);
        }

        public static RemoteDataBuffer CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDataBuffer(port, handle, false);
        }

        protected RemoteDataBuffer(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDataBuffer(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDataBuffer && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_RegisterOwner
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
        }
    }
}
