// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteAnimationManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteAnimationManager : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg3_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_SetAnimationRate;
        private static ushort[] s_priv_ByteOrder_Msg1_PulseTimeAdvance;
        private static ushort[] s_priv_ByteOrder_Msg2_SetGlobalSpeedAdjustment;

        protected RemoteAnimationManager()
        {
        }

        public static unsafe RemoteAnimationManager Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE managerClassHandle = _priv_protocolInstance.AnimationManager_ClassHandle;
            RemoteAnimationManager animationManager = new RemoteAnimationManager(port, _priv_owner);
            uint num = (uint)sizeof(RemoteAnimationManager.Msg3_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteAnimationManager.Msg3_Create* msg3CreatePtr = (RemoteAnimationManager.Msg3_Create*)pMem;
            msg3CreatePtr->_priv_size = num;
            msg3CreatePtr->_priv_msgid = 3U;
            msg3CreatePtr->_priv_idObjectSubject = animationManager.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg3_Create, typeof(RemoteAnimationManager.Msg3_Create), 0, 0);
            port.CreateRemoteObject(managerClassHandle, animationManager.m_renderHandle, (Message*)msg3CreatePtr);
            return animationManager;
        }

        public unsafe void BuildSetAnimationRate(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nFramesPerSecond)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationManager.Msg0_SetAnimationRate);
            RemoteAnimationManager.Msg0_SetAnimationRate* setAnimationRatePtr = (RemoteAnimationManager.Msg0_SetAnimationRate*)_priv_portUse.AllocMessageBuffer(size);
            setAnimationRatePtr->_priv_size = size;
            setAnimationRatePtr->_priv_msgid = 0U;
            setAnimationRatePtr->nFramesPerSecond = nFramesPerSecond;
            setAnimationRatePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setAnimationRatePtr, ref s_priv_ByteOrder_Msg0_SetAnimationRate, typeof(RemoteAnimationManager.Msg0_SetAnimationRate), 0, 0);
            _priv_pmsgUse = (Message*)setAnimationRatePtr;
        }

        public unsafe void SendSetAnimationRate(int nFramesPerSecond)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetAnimationRate(out _priv_portUse, out _priv_pmsgUse, nFramesPerSecond);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPulseTimeAdvance(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPulseMs)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationManager.Msg1_PulseTimeAdvance);
            RemoteAnimationManager.Msg1_PulseTimeAdvance* pulseTimeAdvancePtr = (RemoteAnimationManager.Msg1_PulseTimeAdvance*)_priv_portUse.AllocMessageBuffer(size);
            pulseTimeAdvancePtr->_priv_size = size;
            pulseTimeAdvancePtr->_priv_msgid = 1U;
            pulseTimeAdvancePtr->nPulseMs = nPulseMs;
            pulseTimeAdvancePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)pulseTimeAdvancePtr, ref s_priv_ByteOrder_Msg1_PulseTimeAdvance, typeof(RemoteAnimationManager.Msg1_PulseTimeAdvance), 0, 0);
            _priv_pmsgUse = (Message*)pulseTimeAdvancePtr;
        }

        public unsafe void SendPulseTimeAdvance(int nPulseMs)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPulseTimeAdvance(out _priv_portUse, out _priv_pmsgUse, nPulseMs);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetGlobalSpeedAdjustment(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float flFactor)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimationManager.Msg2_SetGlobalSpeedAdjustment);
            RemoteAnimationManager.Msg2_SetGlobalSpeedAdjustment* globalSpeedAdjustmentPtr = (RemoteAnimationManager.Msg2_SetGlobalSpeedAdjustment*)_priv_portUse.AllocMessageBuffer(size);
            globalSpeedAdjustmentPtr->_priv_size = size;
            globalSpeedAdjustmentPtr->_priv_msgid = 2U;
            globalSpeedAdjustmentPtr->flFactor = flFactor;
            globalSpeedAdjustmentPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)globalSpeedAdjustmentPtr, ref s_priv_ByteOrder_Msg2_SetGlobalSpeedAdjustment, typeof(RemoteAnimationManager.Msg2_SetGlobalSpeedAdjustment), 0, 0);
            _priv_pmsgUse = (Message*)globalSpeedAdjustmentPtr;
        }

        public unsafe void SendSetGlobalSpeedAdjustment(float flFactor)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetGlobalSpeedAdjustment(out _priv_portUse, out _priv_pmsgUse, flFactor);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteAnimationManager CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteAnimationManager(port, handle, true);
        }

        public static RemoteAnimationManager CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteAnimationManager(port, handle, false);
        }

        protected RemoteAnimationManager(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteAnimationManager(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteAnimationManager && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg3_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg0_SetAnimationRate
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nFramesPerSecond;
        }

        [ComVisible(false)]
        private struct Msg1_PulseTimeAdvance
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPulseMs;
        }

        [ComVisible(false)]
        private struct Msg2_SetGlobalSpeedAdjustment
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float flFactor;
        }
    }
}
