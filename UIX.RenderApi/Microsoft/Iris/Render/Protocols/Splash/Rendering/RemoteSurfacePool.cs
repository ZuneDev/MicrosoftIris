// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteSurfacePool
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteSurfacePool : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_SetEffect;
        private static ushort[] s_priv_ByteOrder_Msg1_CreateSurface;
        private static ushort[] s_priv_ByteOrder_Msg2_Free;
        private static ushort[] s_priv_ByteOrder_Msg3_Allocate;
        private static ushort[] s_priv_ByteOrder_Msg4_SetPriority;

        protected RemoteSurfacePool()
        {
        }

        public unsafe void BuildSetEffect(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteEffect effect)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (effect != null)
                _priv_portUse.ValidateHandleOrNull(effect.RenderHandle);
            uint size = (uint)sizeof(RemoteSurfacePool.Msg0_SetEffect);
            RemoteSurfacePool.Msg0_SetEffect* msg0SetEffectPtr = (RemoteSurfacePool.Msg0_SetEffect*)_priv_portUse.AllocMessageBuffer(size);
            msg0SetEffectPtr->_priv_size = size;
            msg0SetEffectPtr->_priv_msgid = 0U;
            msg0SetEffectPtr->effect = effect != null ? effect.RenderHandle : RENDERHANDLE.NULL;
            msg0SetEffectPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0SetEffectPtr, ref s_priv_ByteOrder_Msg0_SetEffect, typeof(RemoteSurfacePool.Msg0_SetEffect), 0, 0);
            _priv_pmsgUse = (Message*)msg0SetEffectPtr;
        }

        public unsafe void SendSetEffect(RemoteEffect effect)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEffect(out _priv_portUse, out _priv_pmsgUse, effect);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildCreateSurface(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RENDERHANDLE idNewSurface)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurfacePool.Msg1_CreateSurface);
            RemoteSurfacePool.Msg1_CreateSurface* msg1CreateSurfacePtr = (RemoteSurfacePool.Msg1_CreateSurface*)_priv_portUse.AllocMessageBuffer(size);
            msg1CreateSurfacePtr->_priv_size = size;
            msg1CreateSurfacePtr->_priv_msgid = 1U;
            msg1CreateSurfacePtr->idNewSurface = idNewSurface;
            msg1CreateSurfacePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1CreateSurfacePtr, ref s_priv_ByteOrder_Msg1_CreateSurface, typeof(RemoteSurfacePool.Msg1_CreateSurface), 0, 0);
            _priv_pmsgUse = (Message*)msg1CreateSurfacePtr;
        }

        public unsafe void SendCreateSurface(RENDERHANDLE idNewSurface)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateSurface(out _priv_portUse, out _priv_pmsgUse, idNewSurface);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildFree(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurfacePool.Msg2_Free);
            RemoteSurfacePool.Msg2_Free* msg2FreePtr = (RemoteSurfacePool.Msg2_Free*)_priv_portUse.AllocMessageBuffer(size);
            msg2FreePtr->_priv_size = size;
            msg2FreePtr->_priv_msgid = 2U;
            msg2FreePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2FreePtr, ref s_priv_ByteOrder_Msg2_Free, typeof(RemoteSurfacePool.Msg2_Free), 0, 0);
            _priv_pmsgUse = (Message*)msg2FreePtr;
        }

        public unsafe void SendFree()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildFree(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAllocate(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizePxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurfacePool.Msg3_Allocate);
            RemoteSurfacePool.Msg3_Allocate* msg3AllocatePtr = (RemoteSurfacePool.Msg3_Allocate*)_priv_portUse.AllocMessageBuffer(size);
            msg3AllocatePtr->_priv_size = size;
            msg3AllocatePtr->_priv_msgid = 3U;
            msg3AllocatePtr->sizePxl = sizePxl;
            msg3AllocatePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3AllocatePtr, ref s_priv_ByteOrder_Msg3_Allocate, typeof(RemoteSurfacePool.Msg3_Allocate), 0, 0);
            _priv_pmsgUse = (Message*)msg3AllocatePtr;
        }

        public unsafe void SendAllocate(Size sizePxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAllocate(out _priv_portUse, out _priv_pmsgUse, sizePxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetPriority(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPriority)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurfacePool.Msg4_SetPriority);
            RemoteSurfacePool.Msg4_SetPriority* msg4SetPriorityPtr = (RemoteSurfacePool.Msg4_SetPriority*)_priv_portUse.AllocMessageBuffer(size);
            msg4SetPriorityPtr->_priv_size = size;
            msg4SetPriorityPtr->_priv_msgid = 4U;
            msg4SetPriorityPtr->nPriority = nPriority;
            msg4SetPriorityPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4SetPriorityPtr, ref s_priv_ByteOrder_Msg4_SetPriority, typeof(RemoteSurfacePool.Msg4_SetPriority), 0, 0);
            _priv_pmsgUse = (Message*)msg4SetPriorityPtr;
        }

        public unsafe void SendSetPriority(int nPriority)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetPriority(out _priv_portUse, out _priv_pmsgUse, nPriority);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteSurfacePool CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteSurfacePool(port, handle, true);
        }

        public static RemoteSurfacePool CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteSurfacePool(port, handle, false);
        }

        protected RemoteSurfacePool(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteSurfacePool(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteSurfacePool && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_SetEffect
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE effect;
        }

        [ComVisible(false)]
        private struct Msg1_CreateSurface
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idNewSurface;
        }

        [ComVisible(false)]
        private struct Msg2_Free
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg3_Allocate
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizePxl;
        }

        [ComVisible(false)]
        private struct Msg4_SetPriority
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPriority;
        }
    }
}
