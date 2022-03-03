// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteAnimation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteAnimation : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg43_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_RemoveAllEvents;
        private static ushort[] s_priv_ByteOrder_Msg1_RemoveEvent;
        private static ushort[] s_priv_ByteOrder_Msg2_AddValueEvent;
        private static ushort[] s_priv_ByteOrder_Msg3_AddProgressEvent;
        private static ushort[] s_priv_ByteOrder_Msg4_AddTimeEvent;
        private static ushort[] s_priv_ByteOrder_Msg5_AddStageEvent;
        private static ushort[] s_priv_ByteOrder_Msg6_RemoveTarget;
        private static ushort[] s_priv_ByteOrder_Msg7_AddTarget;
        private static ushort[] s_priv_ByteOrder_Msg8_SetEaseOut;
        private static ushort[] s_priv_ByteOrder_Msg9_SetEaseIn;
        private static ushort[] s_priv_ByteOrder_Msg10_SetBezier;
        private static ushort[] s_priv_ByteOrder_Msg11_SetCosine;
        private static ushort[] s_priv_ByteOrder_Msg12_SetSine;
        private static ushort[] s_priv_ByteOrder_Msg13_SetSCurve;
        private static ushort[] s_priv_ByteOrder_Msg14_SetLogarithmic;
        private static ushort[] s_priv_ByteOrder_Msg15_SetExponential;
        private static ushort[] s_priv_ByteOrder_Msg16_SetLinear;
        private static ushort[] s_priv_ByteOrder_Msg17_SetBinaryOperation;
        private static ushort[] s_priv_ByteOrder_Msg18_SetContinuous;
        private static ushort[] s_priv_ByteOrder_Msg19_SetCaptured;
        private static ushort[] s_priv_ByteOrder_Msg20_SetQuaternion;
        private static ushort[] s_priv_ByteOrder_Msg21_SetVector4;
        private static ushort[] s_priv_ByteOrder_Msg22_SetVector3;
        private static ushort[] s_priv_ByteOrder_Msg23_SetVector2;
        private static ushort[] s_priv_ByteOrder_Msg24_SetFloat;
        private static ushort[] s_priv_ByteOrder_Msg25_EndAnimationInput;
        private static ushort[] s_priv_ByteOrder_Msg26_BeginAnimationInput;
        private static ushort[] s_priv_ByteOrder_Msg27_SetKeyframeTime;
        private static ushort[] s_priv_ByteOrder_Msg29_SetKeyframeCount;
        private static ushort[] s_priv_ByteOrder_Msg31_AddKeyframe;
        private static ushort[] s_priv_ByteOrder_Msg32_InstantFinish;
        private static ushort[] s_priv_ByteOrder_Msg33_InstantAdvance;
        private static ushort[] s_priv_ByteOrder_Msg34_Reset;
        private static ushort[] s_priv_ByteOrder_Msg35_Pause;
        private static ushort[] s_priv_ByteOrder_Msg36_Play;
        private static ushort[] s_priv_ByteOrder_Msg37_SetResetBehavior;
        private static ushort[] s_priv_ByteOrder_Msg39_SetAutoReset;
        private static ushort[] s_priv_ByteOrder_Msg41_SetRepeatCount;

        protected RemoteAnimation()
        {
        }

        public static unsafe RemoteAnimation Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          AnimationInputType animationType,
          LocalAnimationCallback cb)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE animationClassHandle = _priv_protocolInstance.Animation_ClassHandle;
            RemoteAnimation remoteAnimation = new RemoteAnimation(port, _priv_owner);
            uint num = (uint)sizeof(RemoteAnimation.Msg43_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteAnimation.Msg43_Create* msg43CreatePtr = (RemoteAnimation.Msg43_Create*)pMem;
            msg43CreatePtr->_priv_size = num;
            msg43CreatePtr->_priv_msgid = 43U;
            msg43CreatePtr->animationType = animationType;
            msg43CreatePtr->_priv_objcb = cb.RenderHandle;
            msg43CreatePtr->_priv_ctxcb = port.Session.LocalContext;
            msg43CreatePtr->_priv_idObjectSubject = remoteAnimation.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg43_Create, typeof(RemoteAnimation.Msg43_Create), 0, 0);
            port.CreateRemoteObject(animationClassHandle, remoteAnimation.m_renderHandle, (Message*)msg43CreatePtr);
            return remoteAnimation;
        }

        public unsafe void BuildRemoveAllEvents(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg0_RemoveAllEvents);
            RemoteAnimation.Msg0_RemoveAllEvents* msg0RemoveAllEventsPtr = (RemoteAnimation.Msg0_RemoveAllEvents*)_priv_portUse.AllocMessageBuffer(size);
            msg0RemoveAllEventsPtr->_priv_size = size;
            msg0RemoveAllEventsPtr->_priv_msgid = 0U;
            msg0RemoveAllEventsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0RemoveAllEventsPtr, ref s_priv_ByteOrder_Msg0_RemoveAllEvents, typeof(RemoteAnimation.Msg0_RemoveAllEvents), 0, 0);
            _priv_pmsgUse = (Message*)msg0RemoveAllEventsPtr;
        }

        public unsafe void SendRemoveAllEvents()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRemoveAllEvents(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRemoveEvent(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint idEvent)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg1_RemoveEvent);
            RemoteAnimation.Msg1_RemoveEvent* msg1RemoveEventPtr = (RemoteAnimation.Msg1_RemoveEvent*)_priv_portUse.AllocMessageBuffer(size);
            msg1RemoveEventPtr->_priv_size = size;
            msg1RemoveEventPtr->_priv_msgid = 1U;
            msg1RemoveEventPtr->idEvent = idEvent;
            msg1RemoveEventPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1RemoveEventPtr, ref s_priv_ByteOrder_Msg1_RemoveEvent, typeof(RemoteAnimation.Msg1_RemoveEvent), 0, 0);
            _priv_pmsgUse = (Message*)msg1RemoveEventPtr;
        }

        public unsafe void SendRemoveEvent(uint idEvent)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRemoveEvent(out _priv_portUse, out _priv_pmsgUse, idEvent);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddValueEvent(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint idEvent,
          ValueEventCondition nCondition,
          RENDERHANDLE idTargetObject,
          uint nTargetMethodId,
          uint nTargetMethodArg,
          bool fInitialActivation,
          bool fAllowRepeat)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg2_AddValueEvent);
            RemoteAnimation.Msg2_AddValueEvent* msg2AddValueEventPtr = (RemoteAnimation.Msg2_AddValueEvent*)_priv_portUse.AllocMessageBuffer(size);
            msg2AddValueEventPtr->_priv_size = size;
            msg2AddValueEventPtr->_priv_msgid = 2U;
            msg2AddValueEventPtr->idEvent = idEvent;
            msg2AddValueEventPtr->nCondition = nCondition;
            msg2AddValueEventPtr->idTargetObject = idTargetObject;
            msg2AddValueEventPtr->nTargetMethodId = nTargetMethodId;
            msg2AddValueEventPtr->nTargetMethodArg = nTargetMethodArg;
            msg2AddValueEventPtr->fInitialActivation = fInitialActivation ? uint.MaxValue : 0U;
            msg2AddValueEventPtr->fAllowRepeat = fAllowRepeat ? uint.MaxValue : 0U;
            msg2AddValueEventPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2AddValueEventPtr, ref s_priv_ByteOrder_Msg2_AddValueEvent, typeof(RemoteAnimation.Msg2_AddValueEvent), 0, 0);
            _priv_pmsgUse = (Message*)msg2AddValueEventPtr;
        }

        public unsafe void SendAddValueEvent(
          uint idEvent,
          ValueEventCondition nCondition,
          RENDERHANDLE idTargetObject,
          uint nTargetMethodId,
          uint nTargetMethodArg,
          bool fInitialActivation,
          bool fAllowRepeat)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddValueEvent(out _priv_portUse, out _priv_pmsgUse, idEvent, nCondition, idTargetObject, nTargetMethodId, nTargetMethodArg, fInitialActivation, fAllowRepeat);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddProgressEvent(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint idEvent,
          float flProgress,
          RENDERHANDLE idTargetObject,
          uint nTargetMethodId,
          uint nTargetMethodArg,
          bool fInitialActivation,
          bool fAllowRepeat)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg3_AddProgressEvent);
            RemoteAnimation.Msg3_AddProgressEvent* addProgressEventPtr = (RemoteAnimation.Msg3_AddProgressEvent*)_priv_portUse.AllocMessageBuffer(size);
            addProgressEventPtr->_priv_size = size;
            addProgressEventPtr->_priv_msgid = 3U;
            addProgressEventPtr->idEvent = idEvent;
            addProgressEventPtr->flProgress = flProgress;
            addProgressEventPtr->idTargetObject = idTargetObject;
            addProgressEventPtr->nTargetMethodId = nTargetMethodId;
            addProgressEventPtr->nTargetMethodArg = nTargetMethodArg;
            addProgressEventPtr->fInitialActivation = fInitialActivation ? uint.MaxValue : 0U;
            addProgressEventPtr->fAllowRepeat = fAllowRepeat ? uint.MaxValue : 0U;
            addProgressEventPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)addProgressEventPtr, ref s_priv_ByteOrder_Msg3_AddProgressEvent, typeof(RemoteAnimation.Msg3_AddProgressEvent), 0, 0);
            _priv_pmsgUse = (Message*)addProgressEventPtr;
        }

        public unsafe void SendAddProgressEvent(
          uint idEvent,
          float flProgress,
          RENDERHANDLE idTargetObject,
          uint nTargetMethodId,
          uint nTargetMethodArg,
          bool fInitialActivation,
          bool fAllowRepeat)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddProgressEvent(out _priv_portUse, out _priv_pmsgUse, idEvent, flProgress, idTargetObject, nTargetMethodId, nTargetMethodArg, fInitialActivation, fAllowRepeat);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddTimeEvent(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint idEvent,
          float flEventTime,
          RENDERHANDLE idTargetObject,
          uint nTargetMethodId,
          uint nTargetMethodArg,
          bool fInitialActivation,
          bool fAllowRepeat)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg4_AddTimeEvent);
            RemoteAnimation.Msg4_AddTimeEvent* msg4AddTimeEventPtr = (RemoteAnimation.Msg4_AddTimeEvent*)_priv_portUse.AllocMessageBuffer(size);
            msg4AddTimeEventPtr->_priv_size = size;
            msg4AddTimeEventPtr->_priv_msgid = 4U;
            msg4AddTimeEventPtr->idEvent = idEvent;
            msg4AddTimeEventPtr->flEventTime = flEventTime;
            msg4AddTimeEventPtr->idTargetObject = idTargetObject;
            msg4AddTimeEventPtr->nTargetMethodId = nTargetMethodId;
            msg4AddTimeEventPtr->nTargetMethodArg = nTargetMethodArg;
            msg4AddTimeEventPtr->fInitialActivation = fInitialActivation ? uint.MaxValue : 0U;
            msg4AddTimeEventPtr->fAllowRepeat = fAllowRepeat ? uint.MaxValue : 0U;
            msg4AddTimeEventPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4AddTimeEventPtr, ref s_priv_ByteOrder_Msg4_AddTimeEvent, typeof(RemoteAnimation.Msg4_AddTimeEvent), 0, 0);
            _priv_pmsgUse = (Message*)msg4AddTimeEventPtr;
        }

        public unsafe void SendAddTimeEvent(
          uint idEvent,
          float flEventTime,
          RENDERHANDLE idTargetObject,
          uint nTargetMethodId,
          uint nTargetMethodArg,
          bool fInitialActivation,
          bool fAllowRepeat)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddTimeEvent(out _priv_portUse, out _priv_pmsgUse, idEvent, flEventTime, idTargetObject, nTargetMethodId, nTargetMethodArg, fInitialActivation, fAllowRepeat);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddStageEvent(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint idEvent,
          AnimationStage nStage,
          RENDERHANDLE idTargetObject,
          uint nTargetMethodId,
          uint nTargetMethodArg,
          bool fInitialActivation,
          bool fAllowRepeat)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg5_AddStageEvent);
            RemoteAnimation.Msg5_AddStageEvent* msg5AddStageEventPtr = (RemoteAnimation.Msg5_AddStageEvent*)_priv_portUse.AllocMessageBuffer(size);
            msg5AddStageEventPtr->_priv_size = size;
            msg5AddStageEventPtr->_priv_msgid = 5U;
            msg5AddStageEventPtr->idEvent = idEvent;
            msg5AddStageEventPtr->nStage = nStage;
            msg5AddStageEventPtr->idTargetObject = idTargetObject;
            msg5AddStageEventPtr->nTargetMethodId = nTargetMethodId;
            msg5AddStageEventPtr->nTargetMethodArg = nTargetMethodArg;
            msg5AddStageEventPtr->fInitialActivation = fInitialActivation ? uint.MaxValue : 0U;
            msg5AddStageEventPtr->fAllowRepeat = fAllowRepeat ? uint.MaxValue : 0U;
            msg5AddStageEventPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg5AddStageEventPtr, ref s_priv_ByteOrder_Msg5_AddStageEvent, typeof(RemoteAnimation.Msg5_AddStageEvent), 0, 0);
            _priv_pmsgUse = (Message*)msg5AddStageEventPtr;
        }

        public unsafe void SendAddStageEvent(
          uint idEvent,
          AnimationStage nStage,
          RENDERHANDLE idTargetObject,
          uint nTargetMethodId,
          uint nTargetMethodArg,
          bool fInitialActivation,
          bool fAllowRepeat)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddStageEvent(out _priv_portUse, out _priv_pmsgUse, idEvent, nStage, idTargetObject, nTargetMethodId, nTargetMethodArg, fInitialActivation, fAllowRepeat);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRemoveTarget(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int targetId)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg6_RemoveTarget);
            RemoteAnimation.Msg6_RemoveTarget* msg6RemoveTargetPtr = (RemoteAnimation.Msg6_RemoveTarget*)_priv_portUse.AllocMessageBuffer(size);
            msg6RemoveTargetPtr->_priv_size = size;
            msg6RemoveTargetPtr->_priv_msgid = 6U;
            msg6RemoveTargetPtr->targetId = targetId;
            msg6RemoveTargetPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg6RemoveTargetPtr, ref s_priv_ByteOrder_Msg6_RemoveTarget, typeof(RemoteAnimation.Msg6_RemoveTarget), 0, 0);
            _priv_pmsgUse = (Message*)msg6RemoveTargetPtr;
        }

        public unsafe void SendRemoveTarget(int targetId)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRemoveTarget(out _priv_portUse, out _priv_pmsgUse, targetId);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddTarget(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int targetId,
          RENDERHANDLE objectId,
          uint propertyId,
          uint propertyInfo)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg7_AddTarget);
            RemoteAnimation.Msg7_AddTarget* msg7AddTargetPtr = (RemoteAnimation.Msg7_AddTarget*)_priv_portUse.AllocMessageBuffer(size);
            msg7AddTargetPtr->_priv_size = size;
            msg7AddTargetPtr->_priv_msgid = 7U;
            msg7AddTargetPtr->targetId = targetId;
            msg7AddTargetPtr->objectId = objectId;
            msg7AddTargetPtr->propertyId = propertyId;
            msg7AddTargetPtr->propertyInfo = propertyInfo;
            msg7AddTargetPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg7AddTargetPtr, ref s_priv_ByteOrder_Msg7_AddTarget, typeof(RemoteAnimation.Msg7_AddTarget), 0, 0);
            _priv_pmsgUse = (Message*)msg7AddTargetPtr;
        }

        public unsafe void SendAddTarget(
          int targetId,
          RENDERHANDLE objectId,
          uint propertyId,
          uint propertyInfo)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddTarget(out _priv_portUse, out _priv_pmsgUse, targetId, objectId, propertyId, propertyInfo);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetEaseOut(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical,
          float flWeight,
          float flHandle)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg8_SetEaseOut);
            RemoteAnimation.Msg8_SetEaseOut* msg8SetEaseOutPtr = (RemoteAnimation.Msg8_SetEaseOut*)_priv_portUse.AllocMessageBuffer(size);
            msg8SetEaseOutPtr->_priv_size = size;
            msg8SetEaseOutPtr->_priv_msgid = 8U;
            msg8SetEaseOutPtr->idxKeyframe = idxKeyframe;
            msg8SetEaseOutPtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg8SetEaseOutPtr->flWeight = flWeight;
            msg8SetEaseOutPtr->flHandle = flHandle;
            msg8SetEaseOutPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg8SetEaseOutPtr, ref s_priv_ByteOrder_Msg8_SetEaseOut, typeof(RemoteAnimation.Msg8_SetEaseOut), 0, 0);
            _priv_pmsgUse = (Message*)msg8SetEaseOutPtr;
        }

        public unsafe void SendSetEaseOut(
          int idxKeyframe,
          bool fSpherical,
          float flWeight,
          float flHandle)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEaseOut(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical, flWeight, flHandle);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetEaseIn(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical,
          float flWeight,
          float flHandle)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg9_SetEaseIn);
            RemoteAnimation.Msg9_SetEaseIn* msg9SetEaseInPtr = (RemoteAnimation.Msg9_SetEaseIn*)_priv_portUse.AllocMessageBuffer(size);
            msg9SetEaseInPtr->_priv_size = size;
            msg9SetEaseInPtr->_priv_msgid = 9U;
            msg9SetEaseInPtr->idxKeyframe = idxKeyframe;
            msg9SetEaseInPtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg9SetEaseInPtr->flWeight = flWeight;
            msg9SetEaseInPtr->flHandle = flHandle;
            msg9SetEaseInPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg9SetEaseInPtr, ref s_priv_ByteOrder_Msg9_SetEaseIn, typeof(RemoteAnimation.Msg9_SetEaseIn), 0, 0);
            _priv_pmsgUse = (Message*)msg9SetEaseInPtr;
        }

        public unsafe void SendSetEaseIn(
          int idxKeyframe,
          bool fSpherical,
          float flWeight,
          float flHandle)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEaseIn(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical, flWeight, flHandle);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetBezier(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical,
          float flHandle1,
          float flHandle2)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg10_SetBezier);
            RemoteAnimation.Msg10_SetBezier* msg10SetBezierPtr = (RemoteAnimation.Msg10_SetBezier*)_priv_portUse.AllocMessageBuffer(size);
            msg10SetBezierPtr->_priv_size = size;
            msg10SetBezierPtr->_priv_msgid = 10U;
            msg10SetBezierPtr->idxKeyframe = idxKeyframe;
            msg10SetBezierPtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg10SetBezierPtr->flHandle1 = flHandle1;
            msg10SetBezierPtr->flHandle2 = flHandle2;
            msg10SetBezierPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg10SetBezierPtr, ref s_priv_ByteOrder_Msg10_SetBezier, typeof(RemoteAnimation.Msg10_SetBezier), 0, 0);
            _priv_pmsgUse = (Message*)msg10SetBezierPtr;
        }

        public unsafe void SendSetBezier(
          int idxKeyframe,
          bool fSpherical,
          float flHandle1,
          float flHandle2)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetBezier(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical, flHandle1, flHandle2);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetCosine(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg11_SetCosine);
            RemoteAnimation.Msg11_SetCosine* msg11SetCosinePtr = (RemoteAnimation.Msg11_SetCosine*)_priv_portUse.AllocMessageBuffer(size);
            msg11SetCosinePtr->_priv_size = size;
            msg11SetCosinePtr->_priv_msgid = 11U;
            msg11SetCosinePtr->idxKeyframe = idxKeyframe;
            msg11SetCosinePtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg11SetCosinePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg11SetCosinePtr, ref s_priv_ByteOrder_Msg11_SetCosine, typeof(RemoteAnimation.Msg11_SetCosine), 0, 0);
            _priv_pmsgUse = (Message*)msg11SetCosinePtr;
        }

        public unsafe void SendSetCosine(int idxKeyframe, bool fSpherical)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetCosine(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetSine(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg12_SetSine);
            RemoteAnimation.Msg12_SetSine* msg12SetSinePtr = (RemoteAnimation.Msg12_SetSine*)_priv_portUse.AllocMessageBuffer(size);
            msg12SetSinePtr->_priv_size = size;
            msg12SetSinePtr->_priv_msgid = 12U;
            msg12SetSinePtr->idxKeyframe = idxKeyframe;
            msg12SetSinePtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg12SetSinePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg12SetSinePtr, ref s_priv_ByteOrder_Msg12_SetSine, typeof(RemoteAnimation.Msg12_SetSine), 0, 0);
            _priv_pmsgUse = (Message*)msg12SetSinePtr;
        }

        public unsafe void SendSetSine(int idxKeyframe, bool fSpherical)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetSine(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetSCurve(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical,
          float flWeight)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg13_SetSCurve);
            RemoteAnimation.Msg13_SetSCurve* msg13SetScurvePtr = (RemoteAnimation.Msg13_SetSCurve*)_priv_portUse.AllocMessageBuffer(size);
            msg13SetScurvePtr->_priv_size = size;
            msg13SetScurvePtr->_priv_msgid = 13U;
            msg13SetScurvePtr->idxKeyframe = idxKeyframe;
            msg13SetScurvePtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg13SetScurvePtr->flWeight = flWeight;
            msg13SetScurvePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg13SetScurvePtr, ref s_priv_ByteOrder_Msg13_SetSCurve, typeof(RemoteAnimation.Msg13_SetSCurve), 0, 0);
            _priv_pmsgUse = (Message*)msg13SetScurvePtr;
        }

        public unsafe void SendSetSCurve(int idxKeyframe, bool fSpherical, float flWeight)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetSCurve(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical, flWeight);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetLogarithmic(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical,
          float flWeight)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg14_SetLogarithmic);
            RemoteAnimation.Msg14_SetLogarithmic* msg14SetLogarithmicPtr = (RemoteAnimation.Msg14_SetLogarithmic*)_priv_portUse.AllocMessageBuffer(size);
            msg14SetLogarithmicPtr->_priv_size = size;
            msg14SetLogarithmicPtr->_priv_msgid = 14U;
            msg14SetLogarithmicPtr->idxKeyframe = idxKeyframe;
            msg14SetLogarithmicPtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg14SetLogarithmicPtr->flWeight = flWeight;
            msg14SetLogarithmicPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg14SetLogarithmicPtr, ref s_priv_ByteOrder_Msg14_SetLogarithmic, typeof(RemoteAnimation.Msg14_SetLogarithmic), 0, 0);
            _priv_pmsgUse = (Message*)msg14SetLogarithmicPtr;
        }

        public unsafe void SendSetLogarithmic(int idxKeyframe, bool fSpherical, float flWeight)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetLogarithmic(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical, flWeight);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetExponential(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical,
          float flWeight)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg15_SetExponential);
            RemoteAnimation.Msg15_SetExponential* msg15SetExponentialPtr = (RemoteAnimation.Msg15_SetExponential*)_priv_portUse.AllocMessageBuffer(size);
            msg15SetExponentialPtr->_priv_size = size;
            msg15SetExponentialPtr->_priv_msgid = 15U;
            msg15SetExponentialPtr->idxKeyframe = idxKeyframe;
            msg15SetExponentialPtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg15SetExponentialPtr->flWeight = flWeight;
            msg15SetExponentialPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg15SetExponentialPtr, ref s_priv_ByteOrder_Msg15_SetExponential, typeof(RemoteAnimation.Msg15_SetExponential), 0, 0);
            _priv_pmsgUse = (Message*)msg15SetExponentialPtr;
        }

        public unsafe void SendSetExponential(int idxKeyframe, bool fSpherical, float flWeight)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetExponential(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical, flWeight);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetLinear(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          bool fSpherical)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg16_SetLinear);
            RemoteAnimation.Msg16_SetLinear* msg16SetLinearPtr = (RemoteAnimation.Msg16_SetLinear*)_priv_portUse.AllocMessageBuffer(size);
            msg16SetLinearPtr->_priv_size = size;
            msg16SetLinearPtr->_priv_msgid = 16U;
            msg16SetLinearPtr->idxKeyframe = idxKeyframe;
            msg16SetLinearPtr->fSpherical = fSpherical ? uint.MaxValue : 0U;
            msg16SetLinearPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg16SetLinearPtr, ref s_priv_ByteOrder_Msg16_SetLinear, typeof(RemoteAnimation.Msg16_SetLinear), 0, 0);
            _priv_pmsgUse = (Message*)msg16SetLinearPtr;
        }

        public unsafe void SendSetLinear(int idxKeyframe, bool fSpherical)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetLinear(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, fSpherical);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetBinaryOperation(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          BinaryOpCode opCode)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg17_SetBinaryOperation);
            RemoteAnimation.Msg17_SetBinaryOperation* setBinaryOperationPtr = (RemoteAnimation.Msg17_SetBinaryOperation*)_priv_portUse.AllocMessageBuffer(size);
            setBinaryOperationPtr->_priv_size = size;
            setBinaryOperationPtr->_priv_msgid = 17U;
            setBinaryOperationPtr->idxKeyframe = idxKeyframe;
            setBinaryOperationPtr->opCode = opCode;
            setBinaryOperationPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setBinaryOperationPtr, ref s_priv_ByteOrder_Msg17_SetBinaryOperation, typeof(RemoteAnimation.Msg17_SetBinaryOperation), 0, 0);
            _priv_pmsgUse = (Message*)setBinaryOperationPtr;
        }

        public unsafe void SendSetBinaryOperation(int idxKeyframe, BinaryOpCode opCode)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetBinaryOperation(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, opCode);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetContinuous(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          RENDERHANDLE idObject,
          uint idProperty,
          uint propertyInfo)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg18_SetContinuous);
            RemoteAnimation.Msg18_SetContinuous* msg18SetContinuousPtr = (RemoteAnimation.Msg18_SetContinuous*)_priv_portUse.AllocMessageBuffer(size);
            msg18SetContinuousPtr->_priv_size = size;
            msg18SetContinuousPtr->_priv_msgid = 18U;
            msg18SetContinuousPtr->idxKeyframe = idxKeyframe;
            msg18SetContinuousPtr->idObject = idObject;
            msg18SetContinuousPtr->idProperty = idProperty;
            msg18SetContinuousPtr->propertyInfo = propertyInfo;
            msg18SetContinuousPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg18SetContinuousPtr, ref s_priv_ByteOrder_Msg18_SetContinuous, typeof(RemoteAnimation.Msg18_SetContinuous), 0, 0);
            _priv_pmsgUse = (Message*)msg18SetContinuousPtr;
        }

        public unsafe void SendSetContinuous(
          int idxKeyframe,
          RENDERHANDLE idObject,
          uint idProperty,
          uint propertyInfo)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetContinuous(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, idObject, idProperty, propertyInfo);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetCaptured(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          RENDERHANDLE idObject,
          uint idProperty,
          uint propertyInfo,
          bool fRefreshOnRepeat)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg19_SetCaptured);
            RemoteAnimation.Msg19_SetCaptured* msg19SetCapturedPtr = (RemoteAnimation.Msg19_SetCaptured*)_priv_portUse.AllocMessageBuffer(size);
            msg19SetCapturedPtr->_priv_size = size;
            msg19SetCapturedPtr->_priv_msgid = 19U;
            msg19SetCapturedPtr->idxKeyframe = idxKeyframe;
            msg19SetCapturedPtr->idObject = idObject;
            msg19SetCapturedPtr->idProperty = idProperty;
            msg19SetCapturedPtr->propertyInfo = propertyInfo;
            msg19SetCapturedPtr->fRefreshOnRepeat = fRefreshOnRepeat ? uint.MaxValue : 0U;
            msg19SetCapturedPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg19SetCapturedPtr, ref s_priv_ByteOrder_Msg19_SetCaptured, typeof(RemoteAnimation.Msg19_SetCaptured), 0, 0);
            _priv_pmsgUse = (Message*)msg19SetCapturedPtr;
        }

        public unsafe void SendSetCaptured(
          int idxKeyframe,
          RENDERHANDLE idObject,
          uint idProperty,
          uint propertyInfo,
          bool fRefreshOnRepeat)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetCaptured(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, idObject, idProperty, propertyInfo, fRefreshOnRepeat);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetQuaternion(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          Quaternion qValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg20_SetQuaternion);
            RemoteAnimation.Msg20_SetQuaternion* msg20SetQuaternionPtr = (RemoteAnimation.Msg20_SetQuaternion*)_priv_portUse.AllocMessageBuffer(size);
            msg20SetQuaternionPtr->_priv_size = size;
            msg20SetQuaternionPtr->_priv_msgid = 20U;
            msg20SetQuaternionPtr->idxKeyframe = idxKeyframe;
            msg20SetQuaternionPtr->qValue = qValue;
            msg20SetQuaternionPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg20SetQuaternionPtr, ref s_priv_ByteOrder_Msg20_SetQuaternion, typeof(RemoteAnimation.Msg20_SetQuaternion), 0, 0);
            _priv_pmsgUse = (Message*)msg20SetQuaternionPtr;
        }

        public unsafe void SendSetQuaternion(int idxKeyframe, Quaternion qValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetQuaternion(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, qValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetVector4(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          Vector4 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg21_SetVector4);
            RemoteAnimation.Msg21_SetVector4* msg21SetVector4Ptr = (RemoteAnimation.Msg21_SetVector4*)_priv_portUse.AllocMessageBuffer(size);
            msg21SetVector4Ptr->_priv_size = size;
            msg21SetVector4Ptr->_priv_msgid = 21U;
            msg21SetVector4Ptr->idxKeyframe = idxKeyframe;
            msg21SetVector4Ptr->vValue = vValue;
            msg21SetVector4Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg21SetVector4Ptr, ref s_priv_ByteOrder_Msg21_SetVector4, typeof(RemoteAnimation.Msg21_SetVector4), 0, 0);
            _priv_pmsgUse = (Message*)msg21SetVector4Ptr;
        }

        public unsafe void SendSetVector4(int idxKeyframe, Vector4 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetVector4(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetVector3(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          Vector3 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg22_SetVector3);
            RemoteAnimation.Msg22_SetVector3* msg22SetVector3Ptr = (RemoteAnimation.Msg22_SetVector3*)_priv_portUse.AllocMessageBuffer(size);
            msg22SetVector3Ptr->_priv_size = size;
            msg22SetVector3Ptr->_priv_msgid = 22U;
            msg22SetVector3Ptr->idxKeyframe = idxKeyframe;
            msg22SetVector3Ptr->vValue = vValue;
            msg22SetVector3Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg22SetVector3Ptr, ref s_priv_ByteOrder_Msg22_SetVector3, typeof(RemoteAnimation.Msg22_SetVector3), 0, 0);
            _priv_pmsgUse = (Message*)msg22SetVector3Ptr;
        }

        public unsafe void SendSetVector3(int idxKeyframe, Vector3 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetVector3(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetVector2(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          Vector2 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg23_SetVector2);
            RemoteAnimation.Msg23_SetVector2* msg23SetVector2Ptr = (RemoteAnimation.Msg23_SetVector2*)_priv_portUse.AllocMessageBuffer(size);
            msg23SetVector2Ptr->_priv_size = size;
            msg23SetVector2Ptr->_priv_msgid = 23U;
            msg23SetVector2Ptr->idxKeyframe = idxKeyframe;
            msg23SetVector2Ptr->vValue = vValue;
            msg23SetVector2Ptr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg23SetVector2Ptr, ref s_priv_ByteOrder_Msg23_SetVector2, typeof(RemoteAnimation.Msg23_SetVector2), 0, 0);
            _priv_pmsgUse = (Message*)msg23SetVector2Ptr;
        }

        public unsafe void SendSetVector2(int idxKeyframe, Vector2 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetVector2(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetFloat(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          float flValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg24_SetFloat);
            RemoteAnimation.Msg24_SetFloat* msg24SetFloatPtr = (RemoteAnimation.Msg24_SetFloat*)_priv_portUse.AllocMessageBuffer(size);
            msg24SetFloatPtr->_priv_size = size;
            msg24SetFloatPtr->_priv_msgid = 24U;
            msg24SetFloatPtr->idxKeyframe = idxKeyframe;
            msg24SetFloatPtr->flValue = flValue;
            msg24SetFloatPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg24SetFloatPtr, ref s_priv_ByteOrder_Msg24_SetFloat, typeof(RemoteAnimation.Msg24_SetFloat), 0, 0);
            _priv_pmsgUse = (Message*)msg24SetFloatPtr;
        }

        public unsafe void SendSetFloat(int idxKeyframe, float flValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetFloat(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, flValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildEndAnimationInput(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg25_EndAnimationInput);
            RemoteAnimation.Msg25_EndAnimationInput* endAnimationInputPtr = (RemoteAnimation.Msg25_EndAnimationInput*)_priv_portUse.AllocMessageBuffer(size);
            endAnimationInputPtr->_priv_size = size;
            endAnimationInputPtr->_priv_msgid = 25U;
            endAnimationInputPtr->idxKeyframe = idxKeyframe;
            endAnimationInputPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)endAnimationInputPtr, ref s_priv_ByteOrder_Msg25_EndAnimationInput, typeof(RemoteAnimation.Msg25_EndAnimationInput), 0, 0);
            _priv_pmsgUse = (Message*)endAnimationInputPtr;
        }

        public unsafe void SendEndAnimationInput(int idxKeyframe)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildEndAnimationInput(out _priv_portUse, out _priv_pmsgUse, idxKeyframe);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildBeginAnimationInput(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg26_BeginAnimationInput);
            RemoteAnimation.Msg26_BeginAnimationInput* beginAnimationInputPtr = (RemoteAnimation.Msg26_BeginAnimationInput*)_priv_portUse.AllocMessageBuffer(size);
            beginAnimationInputPtr->_priv_size = size;
            beginAnimationInputPtr->_priv_msgid = 26U;
            beginAnimationInputPtr->idxKeyframe = idxKeyframe;
            beginAnimationInputPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)beginAnimationInputPtr, ref s_priv_ByteOrder_Msg26_BeginAnimationInput, typeof(RemoteAnimation.Msg26_BeginAnimationInput), 0, 0);
            _priv_pmsgUse = (Message*)beginAnimationInputPtr;
        }

        public unsafe void SendBeginAnimationInput(int idxKeyframe)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildBeginAnimationInput(out _priv_portUse, out _priv_pmsgUse, idxKeyframe);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetKeyframeTime(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          float flTimeSec)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg27_SetKeyframeTime);
            RemoteAnimation.Msg27_SetKeyframeTime* msg27SetKeyframeTimePtr = (RemoteAnimation.Msg27_SetKeyframeTime*)_priv_portUse.AllocMessageBuffer(size);
            msg27SetKeyframeTimePtr->_priv_size = size;
            msg27SetKeyframeTimePtr->_priv_msgid = 27U;
            msg27SetKeyframeTimePtr->idxKeyframe = idxKeyframe;
            msg27SetKeyframeTimePtr->flTimeSec = flTimeSec;
            msg27SetKeyframeTimePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg27SetKeyframeTimePtr, ref s_priv_ByteOrder_Msg27_SetKeyframeTime, typeof(RemoteAnimation.Msg27_SetKeyframeTime), 0, 0);
            _priv_pmsgUse = (Message*)msg27SetKeyframeTimePtr;
        }

        public unsafe void SendSetKeyframeTime(int idxKeyframe, float flTimeSec)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetKeyframeTime(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, flTimeSec);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetKeyframeCount(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int cKeyframes)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg29_SetKeyframeCount);
            RemoteAnimation.Msg29_SetKeyframeCount* setKeyframeCountPtr = (RemoteAnimation.Msg29_SetKeyframeCount*)_priv_portUse.AllocMessageBuffer(size);
            setKeyframeCountPtr->_priv_size = size;
            setKeyframeCountPtr->_priv_msgid = 29U;
            setKeyframeCountPtr->cKeyframes = cKeyframes;
            setKeyframeCountPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setKeyframeCountPtr, ref s_priv_ByteOrder_Msg29_SetKeyframeCount, typeof(RemoteAnimation.Msg29_SetKeyframeCount), 0, 0);
            _priv_pmsgUse = (Message*)setKeyframeCountPtr;
        }

        public unsafe void SendSetKeyframeCount(int cKeyframes)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetKeyframeCount(out _priv_portUse, out _priv_pmsgUse, cKeyframes);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddKeyframe(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxKeyframe,
          float flTimeSec)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg31_AddKeyframe);
            RemoteAnimation.Msg31_AddKeyframe* msg31AddKeyframePtr = (RemoteAnimation.Msg31_AddKeyframe*)_priv_portUse.AllocMessageBuffer(size);
            msg31AddKeyframePtr->_priv_size = size;
            msg31AddKeyframePtr->_priv_msgid = 31U;
            msg31AddKeyframePtr->idxKeyframe = idxKeyframe;
            msg31AddKeyframePtr->flTimeSec = flTimeSec;
            msg31AddKeyframePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg31AddKeyframePtr, ref s_priv_ByteOrder_Msg31_AddKeyframe, typeof(RemoteAnimation.Msg31_AddKeyframe), 0, 0);
            _priv_pmsgUse = (Message*)msg31AddKeyframePtr;
        }

        public unsafe void SendAddKeyframe(int idxKeyframe, float flTimeSec)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddKeyframe(out _priv_portUse, out _priv_pmsgUse, idxKeyframe, flTimeSec);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildInstantFinish(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg32_InstantFinish);
            RemoteAnimation.Msg32_InstantFinish* msg32InstantFinishPtr = (RemoteAnimation.Msg32_InstantFinish*)_priv_portUse.AllocMessageBuffer(size);
            msg32InstantFinishPtr->_priv_size = size;
            msg32InstantFinishPtr->_priv_msgid = 32U;
            msg32InstantFinishPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg32InstantFinishPtr, ref s_priv_ByteOrder_Msg32_InstantFinish, typeof(RemoteAnimation.Msg32_InstantFinish), 0, 0);
            _priv_pmsgUse = (Message*)msg32InstantFinishPtr;
        }

        public unsafe void SendInstantFinish()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildInstantFinish(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildInstantAdvance(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float advanceTime)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg33_InstantAdvance);
            RemoteAnimation.Msg33_InstantAdvance* msg33InstantAdvancePtr = (RemoteAnimation.Msg33_InstantAdvance*)_priv_portUse.AllocMessageBuffer(size);
            msg33InstantAdvancePtr->_priv_size = size;
            msg33InstantAdvancePtr->_priv_msgid = 33U;
            msg33InstantAdvancePtr->advanceTime = advanceTime;
            msg33InstantAdvancePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg33InstantAdvancePtr, ref s_priv_ByteOrder_Msg33_InstantAdvance, typeof(RemoteAnimation.Msg33_InstantAdvance), 0, 0);
            _priv_pmsgUse = (Message*)msg33InstantAdvancePtr;
        }

        public unsafe void SendInstantAdvance(float advanceTime)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildInstantAdvance(out _priv_portUse, out _priv_pmsgUse, advanceTime);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildReset(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg34_Reset);
            RemoteAnimation.Msg34_Reset* msg34ResetPtr = (RemoteAnimation.Msg34_Reset*)_priv_portUse.AllocMessageBuffer(size);
            msg34ResetPtr->_priv_size = size;
            msg34ResetPtr->_priv_msgid = 34U;
            msg34ResetPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg34ResetPtr, ref s_priv_ByteOrder_Msg34_Reset, typeof(RemoteAnimation.Msg34_Reset), 0, 0);
            _priv_pmsgUse = (Message*)msg34ResetPtr;
        }

        public unsafe void SendReset()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildReset(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPause(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg35_Pause);
            RemoteAnimation.Msg35_Pause* msg35PausePtr = (RemoteAnimation.Msg35_Pause*)_priv_portUse.AllocMessageBuffer(size);
            msg35PausePtr->_priv_size = size;
            msg35PausePtr->_priv_msgid = 35U;
            msg35PausePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg35PausePtr, ref s_priv_ByteOrder_Msg35_Pause, typeof(RemoteAnimation.Msg35_Pause), 0, 0);
            _priv_pmsgUse = (Message*)msg35PausePtr;
        }

        public unsafe void SendPause()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPause(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPlay(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg36_Play);
            RemoteAnimation.Msg36_Play* msg36PlayPtr = (RemoteAnimation.Msg36_Play*)_priv_portUse.AllocMessageBuffer(size);
            msg36PlayPtr->_priv_size = size;
            msg36PlayPtr->_priv_msgid = 36U;
            msg36PlayPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg36PlayPtr, ref s_priv_ByteOrder_Msg36_Play, typeof(RemoteAnimation.Msg36_Play), 0, 0);
            _priv_pmsgUse = (Message*)msg36PlayPtr;
        }

        public unsafe void SendPlay()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPlay(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetResetBehavior(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          AnimationResetBehavior behavior)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg37_SetResetBehavior);
            RemoteAnimation.Msg37_SetResetBehavior* setResetBehaviorPtr = (RemoteAnimation.Msg37_SetResetBehavior*)_priv_portUse.AllocMessageBuffer(size);
            setResetBehaviorPtr->_priv_size = size;
            setResetBehaviorPtr->_priv_msgid = 37U;
            setResetBehaviorPtr->behavior = behavior;
            setResetBehaviorPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setResetBehaviorPtr, ref s_priv_ByteOrder_Msg37_SetResetBehavior, typeof(RemoteAnimation.Msg37_SetResetBehavior), 0, 0);
            _priv_pmsgUse = (Message*)setResetBehaviorPtr;
        }

        public unsafe void SendSetResetBehavior(AnimationResetBehavior behavior)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetResetBehavior(out _priv_portUse, out _priv_pmsgUse, behavior);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetAutoReset(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fAutoReset)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg39_SetAutoReset);
            RemoteAnimation.Msg39_SetAutoReset* msg39SetAutoResetPtr = (RemoteAnimation.Msg39_SetAutoReset*)_priv_portUse.AllocMessageBuffer(size);
            msg39SetAutoResetPtr->_priv_size = size;
            msg39SetAutoResetPtr->_priv_msgid = 39U;
            msg39SetAutoResetPtr->fAutoReset = fAutoReset ? uint.MaxValue : 0U;
            msg39SetAutoResetPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg39SetAutoResetPtr, ref s_priv_ByteOrder_Msg39_SetAutoReset, typeof(RemoteAnimation.Msg39_SetAutoReset), 0, 0);
            _priv_pmsgUse = (Message*)msg39SetAutoResetPtr;
        }

        public unsafe void SendSetAutoReset(bool fAutoReset)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetAutoReset(out _priv_portUse, out _priv_pmsgUse, fAutoReset);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetRepeatCount(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int cRepeats)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteAnimation.Msg41_SetRepeatCount);
            RemoteAnimation.Msg41_SetRepeatCount* msg41SetRepeatCountPtr = (RemoteAnimation.Msg41_SetRepeatCount*)_priv_portUse.AllocMessageBuffer(size);
            msg41SetRepeatCountPtr->_priv_size = size;
            msg41SetRepeatCountPtr->_priv_msgid = 41U;
            msg41SetRepeatCountPtr->cRepeats = cRepeats;
            msg41SetRepeatCountPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg41SetRepeatCountPtr, ref s_priv_ByteOrder_Msg41_SetRepeatCount, typeof(RemoteAnimation.Msg41_SetRepeatCount), 0, 0);
            _priv_pmsgUse = (Message*)msg41SetRepeatCountPtr;
        }

        public unsafe void SendSetRepeatCount(int cRepeats)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetRepeatCount(out _priv_portUse, out _priv_pmsgUse, cRepeats);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteAnimation CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteAnimation(port, handle, true);
        }

        public static RemoteAnimation CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteAnimation(port, handle, false);
        }

        protected RemoteAnimation(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteAnimation(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteAnimation && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg43_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public AnimationInputType animationType;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
        }

        [ComVisible(false)]
        private struct Msg0_RemoveAllEvents
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg1_RemoveEvent
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint idEvent;
        }

        [ComVisible(false)]
        private struct Msg2_AddValueEvent
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint idEvent;
            public ValueEventCondition nCondition;
            public RENDERHANDLE idTargetObject;
            public uint nTargetMethodId;
            public uint nTargetMethodArg;
            public uint fInitialActivation;
            public uint fAllowRepeat;
        }

        [ComVisible(false)]
        private struct Msg3_AddProgressEvent
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint idEvent;
            public float flProgress;
            public RENDERHANDLE idTargetObject;
            public uint nTargetMethodId;
            public uint nTargetMethodArg;
            public uint fInitialActivation;
            public uint fAllowRepeat;
        }

        [ComVisible(false)]
        private struct Msg4_AddTimeEvent
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint idEvent;
            public float flEventTime;
            public RENDERHANDLE idTargetObject;
            public uint nTargetMethodId;
            public uint nTargetMethodArg;
            public uint fInitialActivation;
            public uint fAllowRepeat;
        }

        [ComVisible(false)]
        private struct Msg5_AddStageEvent
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint idEvent;
            public AnimationStage nStage;
            public RENDERHANDLE idTargetObject;
            public uint nTargetMethodId;
            public uint nTargetMethodArg;
            public uint fInitialActivation;
            public uint fAllowRepeat;
        }

        [ComVisible(false)]
        private struct Msg6_RemoveTarget
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int targetId;
        }

        [ComVisible(false)]
        private struct Msg7_AddTarget
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int targetId;
            public RENDERHANDLE objectId;
            public uint propertyId;
            public uint propertyInfo;
        }

        [ComVisible(false)]
        private struct Msg8_SetEaseOut
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
            public float flWeight;
            public float flHandle;
        }

        [ComVisible(false)]
        private struct Msg9_SetEaseIn
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
            public float flWeight;
            public float flHandle;
        }

        [ComVisible(false)]
        private struct Msg10_SetBezier
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
            public float flHandle1;
            public float flHandle2;
        }

        [ComVisible(false)]
        private struct Msg11_SetCosine
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
        }

        [ComVisible(false)]
        private struct Msg12_SetSine
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
        }

        [ComVisible(false)]
        private struct Msg13_SetSCurve
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
            public float flWeight;
        }

        [ComVisible(false)]
        private struct Msg14_SetLogarithmic
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
            public float flWeight;
        }

        [ComVisible(false)]
        private struct Msg15_SetExponential
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
            public float flWeight;
        }

        [ComVisible(false)]
        private struct Msg16_SetLinear
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public uint fSpherical;
        }

        [ComVisible(false)]
        private struct Msg17_SetBinaryOperation
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public BinaryOpCode opCode;
        }

        [ComVisible(false)]
        private struct Msg18_SetContinuous
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public RENDERHANDLE idObject;
            public uint idProperty;
            public uint propertyInfo;
        }

        [ComVisible(false)]
        private struct Msg19_SetCaptured
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public RENDERHANDLE idObject;
            public uint idProperty;
            public uint propertyInfo;
            public uint fRefreshOnRepeat;
        }

        [ComVisible(false)]
        private struct Msg20_SetQuaternion
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public Quaternion qValue;
        }

        [ComVisible(false)]
        private struct Msg21_SetVector4
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public Vector4 vValue;
        }

        [ComVisible(false)]
        private struct Msg22_SetVector3
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public Vector3 vValue;
        }

        [ComVisible(false)]
        private struct Msg23_SetVector2
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public Vector2 vValue;
        }

        [ComVisible(false)]
        private struct Msg24_SetFloat
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public float flValue;
        }

        [ComVisible(false)]
        private struct Msg25_EndAnimationInput
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
        }

        [ComVisible(false)]
        private struct Msg26_BeginAnimationInput
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
        }

        [ComVisible(false)]
        private struct Msg27_SetKeyframeTime
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public float flTimeSec;
        }

        [ComVisible(false)]
        private struct Msg29_SetKeyframeCount
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int cKeyframes;
        }

        [ComVisible(false)]
        private struct Msg31_AddKeyframe
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxKeyframe;
            public float flTimeSec;
        }

        [ComVisible(false)]
        private struct Msg32_InstantFinish
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg33_InstantAdvance
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float advanceTime;
        }

        [ComVisible(false)]
        private struct Msg34_Reset
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg35_Pause
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg36_Play
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg37_SetResetBehavior
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public AnimationResetBehavior behavior;
        }

        [ComVisible(false)]
        private struct Msg39_SetAutoReset
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fAutoReset;
        }

        [ComVisible(false)]
        private struct Msg41_SetRepeatCount
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int cRepeats;
        }
    }
}
