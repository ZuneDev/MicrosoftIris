// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteVideoPool
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteVideoPool : RemoteSurfacePool
    {
        private static ushort[] s_priv_ByteOrder_Msg6_NotifyVideoInputChanged;
        private static ushort[] s_priv_ByteOrder_Msg7_SetContentOverscan;
        private static ushort[] s_priv_ByteOrder_Msg8_NotifyVideoSizeChanged;
        private static ushort[] s_priv_ByteOrder_Msg10_OnFrameReady;
        private static ushort[] s_priv_ByteOrder_Msg11_OnClockStop;
        private static ushort[] s_priv_ByteOrder_Msg12_OnClockStart;
        private static ushort[] s_priv_ByteOrder_Msg13_OnDisconnect;
        private static ushort[] s_priv_ByteOrder_Msg14_OnConnect;

        protected RemoteVideoPool()
        {
        }

        public unsafe void BuildNotifyVideoInputChanged(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizeContentPxl,
          Size sizeAspectRatio)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVideoPool.Msg6_NotifyVideoInputChanged);
            RemoteVideoPool.Msg6_NotifyVideoInputChanged* videoInputChangedPtr = (RemoteVideoPool.Msg6_NotifyVideoInputChanged*)_priv_portUse.AllocMessageBuffer(size);
            videoInputChangedPtr->_priv_size = size;
            videoInputChangedPtr->_priv_msgid = 6U;
            videoInputChangedPtr->sizeContentPxl = sizeContentPxl;
            videoInputChangedPtr->sizeAspectRatio = sizeAspectRatio;
            videoInputChangedPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)videoInputChangedPtr, ref s_priv_ByteOrder_Msg6_NotifyVideoInputChanged, typeof(RemoteVideoPool.Msg6_NotifyVideoInputChanged), 0, 0);
            _priv_pmsgUse = (Message*)videoInputChangedPtr;
        }

        public unsafe void SendNotifyVideoInputChanged(Size sizeContentPxl, Size sizeAspectRatio)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildNotifyVideoInputChanged(out _priv_portUse, out _priv_pmsgUse, sizeContentPxl, sizeAspectRatio);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetContentOverscan(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float flContentOverscan)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVideoPool.Msg7_SetContentOverscan);
            RemoteVideoPool.Msg7_SetContentOverscan* setContentOverscanPtr = (RemoteVideoPool.Msg7_SetContentOverscan*)_priv_portUse.AllocMessageBuffer(size);
            setContentOverscanPtr->_priv_size = size;
            setContentOverscanPtr->_priv_msgid = 7U;
            setContentOverscanPtr->flContentOverscan = flContentOverscan;
            setContentOverscanPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setContentOverscanPtr, ref s_priv_ByteOrder_Msg7_SetContentOverscan, typeof(RemoteVideoPool.Msg7_SetContentOverscan), 0, 0);
            _priv_pmsgUse = (Message*)setContentOverscanPtr;
        }

        public unsafe void SendSetContentOverscan(float flContentOverscan)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetContentOverscan(out _priv_portUse, out _priv_pmsgUse, flContentOverscan);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildNotifyVideoSizeChanged(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizeTargetPxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVideoPool.Msg8_NotifyVideoSizeChanged);
            RemoteVideoPool.Msg8_NotifyVideoSizeChanged* videoSizeChangedPtr = (RemoteVideoPool.Msg8_NotifyVideoSizeChanged*)_priv_portUse.AllocMessageBuffer(size);
            videoSizeChangedPtr->_priv_size = size;
            videoSizeChangedPtr->_priv_msgid = 8U;
            videoSizeChangedPtr->sizeTargetPxl = sizeTargetPxl;
            videoSizeChangedPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)videoSizeChangedPtr, ref s_priv_ByteOrder_Msg8_NotifyVideoSizeChanged, typeof(RemoteVideoPool.Msg8_NotifyVideoSizeChanged), 0, 0);
            _priv_pmsgUse = (Message*)videoSizeChangedPtr;
        }

        public unsafe void SendNotifyVideoSizeChanged(Size sizeTargetPxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildNotifyVideoSizeChanged(out _priv_portUse, out _priv_pmsgUse, sizeTargetPxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildOnFrameReady(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxFrame,
          long nTimeStampQPC,
          long nFrameCount,
          Rectangle rcContent,
          uint nOptions)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVideoPool.Msg10_OnFrameReady);
            RemoteVideoPool.Msg10_OnFrameReady* msg10OnFrameReadyPtr = (RemoteVideoPool.Msg10_OnFrameReady*)_priv_portUse.AllocMessageBuffer(size);
            msg10OnFrameReadyPtr->_priv_size = size;
            msg10OnFrameReadyPtr->_priv_msgid = 10U;
            msg10OnFrameReadyPtr->idxFrame = idxFrame;
            msg10OnFrameReadyPtr->nTimeStampQPC = nTimeStampQPC;
            msg10OnFrameReadyPtr->nFrameCount = nFrameCount;
            msg10OnFrameReadyPtr->rcContent = rcContent;
            msg10OnFrameReadyPtr->nOptions = nOptions;
            msg10OnFrameReadyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg10OnFrameReadyPtr, ref s_priv_ByteOrder_Msg10_OnFrameReady, typeof(RemoteVideoPool.Msg10_OnFrameReady), 0, 0);
            _priv_pmsgUse = (Message*)msg10OnFrameReadyPtr;
        }

        public unsafe void SendOnFrameReady(
          int idxFrame,
          long nTimeStampQPC,
          long nFrameCount,
          Rectangle rcContent,
          uint nOptions)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildOnFrameReady(out _priv_portUse, out _priv_pmsgUse, idxFrame, nTimeStampQPC, nFrameCount, rcContent, nOptions);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildOnClockStop(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVideoPool.Msg11_OnClockStop);
            RemoteVideoPool.Msg11_OnClockStop* msg11OnClockStopPtr = (RemoteVideoPool.Msg11_OnClockStop*)_priv_portUse.AllocMessageBuffer(size);
            msg11OnClockStopPtr->_priv_size = size;
            msg11OnClockStopPtr->_priv_msgid = 11U;
            msg11OnClockStopPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg11OnClockStopPtr, ref s_priv_ByteOrder_Msg11_OnClockStop, typeof(RemoteVideoPool.Msg11_OnClockStop), 0, 0);
            _priv_pmsgUse = (Message*)msg11OnClockStopPtr;
        }

        public unsafe void SendOnClockStop()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildOnClockStop(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildOnClockStart(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint nClockGeneration,
          uint nFrameRateNumerator,
          uint nFrameRateDenominator,
          Rectangle rcBasisPxl,
          Size sizeAspectRatio)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVideoPool.Msg12_OnClockStart);
            RemoteVideoPool.Msg12_OnClockStart* msg12OnClockStartPtr = (RemoteVideoPool.Msg12_OnClockStart*)_priv_portUse.AllocMessageBuffer(size);
            msg12OnClockStartPtr->_priv_size = size;
            msg12OnClockStartPtr->_priv_msgid = 12U;
            msg12OnClockStartPtr->nClockGeneration = nClockGeneration;
            msg12OnClockStartPtr->nFrameRateNumerator = nFrameRateNumerator;
            msg12OnClockStartPtr->nFrameRateDenominator = nFrameRateDenominator;
            msg12OnClockStartPtr->rcBasisPxl = rcBasisPxl;
            msg12OnClockStartPtr->sizeAspectRatio = sizeAspectRatio;
            msg12OnClockStartPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg12OnClockStartPtr, ref s_priv_ByteOrder_Msg12_OnClockStart, typeof(RemoteVideoPool.Msg12_OnClockStart), 0, 0);
            _priv_pmsgUse = (Message*)msg12OnClockStartPtr;
        }

        public unsafe void SendOnClockStart(
          uint nClockGeneration,
          uint nFrameRateNumerator,
          uint nFrameRateDenominator,
          Rectangle rcBasisPxl,
          Size sizeAspectRatio)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildOnClockStart(out _priv_portUse, out _priv_pmsgUse, nClockGeneration, nFrameRateNumerator, nFrameRateDenominator, rcBasisPxl, sizeAspectRatio);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildOnDisconnect(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVideoPool.Msg13_OnDisconnect);
            RemoteVideoPool.Msg13_OnDisconnect* msg13OnDisconnectPtr = (RemoteVideoPool.Msg13_OnDisconnect*)_priv_portUse.AllocMessageBuffer(size);
            msg13OnDisconnectPtr->_priv_size = size;
            msg13OnDisconnectPtr->_priv_msgid = 13U;
            msg13OnDisconnectPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg13OnDisconnectPtr, ref s_priv_ByteOrder_Msg13_OnDisconnect, typeof(RemoteVideoPool.Msg13_OnDisconnect), 0, 0);
            _priv_pmsgUse = (Message*)msg13OnDisconnectPtr;
        }

        public unsafe void SendOnDisconnect()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildOnDisconnect(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildOnConnect(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVideoPool.Msg14_OnConnect);
            RemoteVideoPool.Msg14_OnConnect* msg14OnConnectPtr = (RemoteVideoPool.Msg14_OnConnect*)_priv_portUse.AllocMessageBuffer(size);
            msg14OnConnectPtr->_priv_size = size;
            msg14OnConnectPtr->_priv_msgid = 14U;
            msg14OnConnectPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg14OnConnectPtr, ref s_priv_ByteOrder_Msg14_OnConnect, typeof(RemoteVideoPool.Msg14_OnConnect), 0, 0);
            _priv_pmsgUse = (Message*)msg14OnConnectPtr;
        }

        public unsafe void SendOnConnect()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildOnConnect(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteVideoPool CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteVideoPool(port, handle, true);
        }

        public static RemoteVideoPool CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteVideoPool(port, handle, false);
        }

        protected RemoteVideoPool(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteVideoPool(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteVideoPool && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg6_NotifyVideoInputChanged
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizeContentPxl;
            public Size sizeAspectRatio;
        }

        [ComVisible(false)]
        private struct Msg7_SetContentOverscan
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float flContentOverscan;
        }

        [ComVisible(false)]
        private struct Msg8_NotifyVideoSizeChanged
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizeTargetPxl;
        }

        [ComVisible(false)]
        private struct Msg10_OnFrameReady
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxFrame;
            public long nTimeStampQPC;
            public long nFrameCount;
            public Rectangle rcContent;
            public uint nOptions;
        }

        [ComVisible(false)]
        private struct Msg11_OnClockStop
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg12_OnClockStart
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nClockGeneration;
            public uint nFrameRateNumerator;
            public uint nFrameRateDenominator;
            public Rectangle rcBasisPxl;
            public Size sizeAspectRatio;
        }

        [ComVisible(false)]
        private struct Msg13_OnDisconnect
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg14_OnConnect
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }
    }
}
