// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt.RemoteNtDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt
{
    internal class RemoteNtDevice : RemoteDx9Device
    {
        private static ushort[] s_priv_ByteOrder_Msg19_Create;
        private static ushort[] s_priv_ByteOrder_Msg11_RenderNowIfPossible;
        private static ushort[] s_priv_ByteOrder_Msg12_CreateTransferBuffer;
        private static ushort[] s_priv_ByteOrder_Msg13_SetMaxOverscan;
        private static ushort[] s_priv_ByteOrder_Msg15_SetDisplayOverscan;
        private static ushort[] s_priv_ByteOrder_Msg17_SetEnablePageFlip;

        protected RemoteNtDevice()
        {
        }

        public static unsafe RemoteNtDevice Create(
          ProtocolSplashRenderingNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          LocalDeviceCallback cb)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE deviceClassHandle = _priv_protocolInstance.NtDevice_ClassHandle;
            RemoteNtDevice remoteNtDevice = new RemoteNtDevice(port, _priv_owner);
            uint num = (uint)sizeof(RemoteNtDevice.Msg19_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteNtDevice.Msg19_Create* msg19CreatePtr = (RemoteNtDevice.Msg19_Create*)pMem;
            msg19CreatePtr->_priv_size = num;
            msg19CreatePtr->_priv_msgid = 19U;
            msg19CreatePtr->_priv_objcb = cb.RenderHandle;
            msg19CreatePtr->_priv_ctxcb = port.Session.LocalContext;
            msg19CreatePtr->_priv_idObjectSubject = remoteNtDevice.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg19_Create, typeof(RemoteNtDevice.Msg19_Create), 0, 0);
            port.CreateRemoteObject(deviceClassHandle, remoteNtDevice.m_renderHandle, (Message*)msg19CreatePtr);
            return remoteNtDevice;
        }

        public unsafe void BuildRenderNowIfPossible(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteNtDevice.Msg11_RenderNowIfPossible);
            RemoteNtDevice.Msg11_RenderNowIfPossible* renderNowIfPossiblePtr = (RemoteNtDevice.Msg11_RenderNowIfPossible*)_priv_portUse.AllocMessageBuffer(size);
            renderNowIfPossiblePtr->_priv_size = size;
            renderNowIfPossiblePtr->_priv_msgid = 11U;
            renderNowIfPossiblePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)renderNowIfPossiblePtr, ref s_priv_ByteOrder_Msg11_RenderNowIfPossible, typeof(RemoteNtDevice.Msg11_RenderNowIfPossible), 0, 0);
            _priv_pmsgUse = (Message*)renderNowIfPossiblePtr;
        }

        public unsafe void SendRenderNowIfPossible()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRenderNowIfPossible(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildCreateTransferBuffer(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizeTransferPxl,
          SurfaceFormat nFormat)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteNtDevice.Msg12_CreateTransferBuffer);
            RemoteNtDevice.Msg12_CreateTransferBuffer* createTransferBufferPtr = (RemoteNtDevice.Msg12_CreateTransferBuffer*)_priv_portUse.AllocMessageBuffer(size);
            createTransferBufferPtr->_priv_size = size;
            createTransferBufferPtr->_priv_msgid = 12U;
            createTransferBufferPtr->sizeTransferPxl = sizeTransferPxl;
            createTransferBufferPtr->nFormat = nFormat;
            createTransferBufferPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)createTransferBufferPtr, ref s_priv_ByteOrder_Msg12_CreateTransferBuffer, typeof(RemoteNtDevice.Msg12_CreateTransferBuffer), 0, 0);
            _priv_pmsgUse = (Message*)createTransferBufferPtr;
        }

        public unsafe void SendCreateTransferBuffer(Size sizeTransferPxl, SurfaceFormat nFormat)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateTransferBuffer(out _priv_portUse, out _priv_pmsgUse, sizeTransferPxl, nFormat);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetMaxOverscan(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float flMaxOverscan)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteNtDevice.Msg13_SetMaxOverscan);
            RemoteNtDevice.Msg13_SetMaxOverscan* msg13SetMaxOverscanPtr = (RemoteNtDevice.Msg13_SetMaxOverscan*)_priv_portUse.AllocMessageBuffer(size);
            msg13SetMaxOverscanPtr->_priv_size = size;
            msg13SetMaxOverscanPtr->_priv_msgid = 13U;
            msg13SetMaxOverscanPtr->flMaxOverscan = flMaxOverscan;
            msg13SetMaxOverscanPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg13SetMaxOverscanPtr, ref s_priv_ByteOrder_Msg13_SetMaxOverscan, typeof(RemoteNtDevice.Msg13_SetMaxOverscan), 0, 0);
            _priv_pmsgUse = (Message*)msg13SetMaxOverscanPtr;
        }

        public unsafe void SendSetMaxOverscan(float flMaxOverscan)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetMaxOverscan(out _priv_portUse, out _priv_pmsgUse, flMaxOverscan);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetDisplayOverscan(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float flDisplayOverscan)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteNtDevice.Msg15_SetDisplayOverscan);
            RemoteNtDevice.Msg15_SetDisplayOverscan* setDisplayOverscanPtr = (RemoteNtDevice.Msg15_SetDisplayOverscan*)_priv_portUse.AllocMessageBuffer(size);
            setDisplayOverscanPtr->_priv_size = size;
            setDisplayOverscanPtr->_priv_msgid = 15U;
            setDisplayOverscanPtr->flDisplayOverscan = flDisplayOverscan;
            setDisplayOverscanPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setDisplayOverscanPtr, ref s_priv_ByteOrder_Msg15_SetDisplayOverscan, typeof(RemoteNtDevice.Msg15_SetDisplayOverscan), 0, 0);
            _priv_pmsgUse = (Message*)setDisplayOverscanPtr;
        }

        public unsafe void SendSetDisplayOverscan(float flDisplayOverscan)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetDisplayOverscan(out _priv_portUse, out _priv_pmsgUse, flDisplayOverscan);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetEnablePageFlip(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fPageFlip)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteNtDevice.Msg17_SetEnablePageFlip);
            RemoteNtDevice.Msg17_SetEnablePageFlip* setEnablePageFlipPtr = (RemoteNtDevice.Msg17_SetEnablePageFlip*)_priv_portUse.AllocMessageBuffer(size);
            setEnablePageFlipPtr->_priv_size = size;
            setEnablePageFlipPtr->_priv_msgid = 17U;
            setEnablePageFlipPtr->fPageFlip = fPageFlip ? uint.MaxValue : 0U;
            setEnablePageFlipPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setEnablePageFlipPtr, ref s_priv_ByteOrder_Msg17_SetEnablePageFlip, typeof(RemoteNtDevice.Msg17_SetEnablePageFlip), 0, 0);
            _priv_pmsgUse = (Message*)setEnablePageFlipPtr;
        }

        public unsafe void SendSetEnablePageFlip(bool fPageFlip)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEnablePageFlip(out _priv_portUse, out _priv_pmsgUse, fPageFlip);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteNtDevice CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteNtDevice(port, handle, true);
        }

        public static RemoteNtDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteNtDevice(port, handle, false);
        }

        protected RemoteNtDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteNtDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteNtDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg19_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
        }

        [ComVisible(false)]
        private struct Msg11_RenderNowIfPossible
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg12_CreateTransferBuffer
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizeTransferPxl;
            public SurfaceFormat nFormat;
        }

        [ComVisible(false)]
        private struct Msg13_SetMaxOverscan
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float flMaxOverscan;
        }

        [ComVisible(false)]
        private struct Msg15_SetDisplayOverscan
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float flDisplayOverscan;
        }

        [ComVisible(false)]
        private struct Msg17_SetEnablePageFlip
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fPageFlip;
        }
    }
}
