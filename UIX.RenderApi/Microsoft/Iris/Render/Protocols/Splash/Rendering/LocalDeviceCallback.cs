// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.LocalDeviceCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Graphics;
using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class LocalDeviceCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnBackBufferCaptured;
        private static ushort[] s_priv_ByteOrder_Msg1_OnSurfacePoolAllocation;
        private static ushort[] s_priv_ByteOrder_Msg2_OnVsaBlock;
        private static ushort[] s_priv_ByteOrder_Msg3_OnLostDevice;
        private static ushort[] s_priv_ByteOrder_Msg4_OnCreated;

        protected LocalDeviceCallback()
        {
        }

        internal static LocalDeviceCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalDeviceCallback(port, callbackInstance);
        }

        protected LocalDeviceCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalDeviceCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is IDeviceCallback _priv_target))
                return;
            switch (message->nMsg)
            {
                case 0:
                    Dispatch_OnBackBufferCaptured(port, _priv_target, (LocalDeviceCallback.Msg0_OnBackBufferCaptured*)message);
                    break;
                case 1:
                    Dispatch_OnSurfacePoolAllocation(port, _priv_target, (LocalDeviceCallback.Msg1_OnSurfacePoolAllocation*)message);
                    break;
                case 2:
                    Dispatch_OnVsaBlock(port, _priv_target, (LocalDeviceCallback.Msg2_OnVsaBlock*)message);
                    break;
                case 3:
                    Dispatch_OnLostDevice(port, _priv_target, (LocalDeviceCallback.Msg3_OnLostDevice*)message);
                    break;
                case 4:
                    Dispatch_OnCreated(port, _priv_target, (LocalDeviceCallback.Msg4_OnCreated*)message);
                    break;
            }
        }

        private static unsafe void Dispatch_OnBackBufferCaptured(
          RenderPort _priv_port,
          IDeviceCallback _priv_target,
          LocalDeviceCallback.Msg0_OnBackBufferCaptured* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnBackBufferCaptured, typeof(LocalDeviceCallback.Msg0_OnBackBufferCaptured), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnBackBufferCaptured(target);
        }

        private static unsafe void Dispatch_OnSurfacePoolAllocation(
          RenderPort _priv_port,
          IDeviceCallback _priv_target,
          LocalDeviceCallback.Msg1_OnSurfacePoolAllocation* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg1_OnSurfacePoolAllocation, typeof(LocalDeviceCallback.Msg1_OnSurfacePoolAllocation), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            RENDERHANDLE idSurfacePool = _priv_pmsg->idSurfacePool;
            SurfacePoolAllocationResult nResult = _priv_pmsg->nResult;
            _priv_target.OnSurfacePoolAllocation(target, idSurfacePool, nResult);
        }

        private static unsafe void Dispatch_OnVsaBlock(
          RenderPort _priv_port,
          IDeviceCallback _priv_target,
          LocalDeviceCallback.Msg2_OnVsaBlock* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg2_OnVsaBlock, typeof(LocalDeviceCallback.Msg2_OnVsaBlock), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            bool fInBlock = _priv_pmsg->fInBlock != 0U;
            _priv_target.OnVsaBlock(target, fInBlock);
        }

        private static unsafe void Dispatch_OnLostDevice(
          RenderPort _priv_port,
          IDeviceCallback _priv_target,
          LocalDeviceCallback.Msg3_OnLostDevice* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg3_OnLostDevice, typeof(LocalDeviceCallback.Msg3_OnLostDevice), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            uint renderGeneration = _priv_pmsg->cRenderGeneration;
            bool fLost = _priv_pmsg->fLost != 0U;
            _priv_target.OnLostDevice(target, renderGeneration, fLost);
        }

        private static unsafe void Dispatch_OnCreated(
          RenderPort _priv_port,
          IDeviceCallback _priv_target,
          LocalDeviceCallback.Msg4_OnCreated* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg4_OnCreated, typeof(LocalDeviceCallback.Msg4_OnCreated), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            bool fAllowDynamicPool = _priv_pmsg->fAllowDynamicPool != 0U;
            _priv_target.OnCreated(target, fAllowDynamicPool);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnBackBufferCaptured
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }

        [ComVisible(false)]
        private struct Msg1_OnSurfacePoolAllocation
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public RENDERHANDLE idSurfacePool;
            public SurfacePoolAllocationResult nResult;
        }

        [ComVisible(false)]
        private struct Msg2_OnVsaBlock
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint fInBlock;
        }

        [ComVisible(false)]
        private struct Msg3_OnLostDevice
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint cRenderGeneration;
            public uint fLost;
        }

        [ComVisible(false)]
        private struct Msg4_OnCreated
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint fAllowDynamicPool;
        }
    }
}
