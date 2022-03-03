// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt.RemoteGdiDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt
{
    internal class RemoteGdiDevice : RemoteDevice
    {
        private static ushort[] s_priv_ByteOrder_Msg23_Create;
        private static ushort[] s_priv_ByteOrder_Msg8_SecureDesktopMode;
        private static ushort[] s_priv_ByteOrder_Msg9_SetZoomMode;
        private static ushort[] s_priv_ByteOrder_Msg11_SetEnableAlphaTracking;
        private static ushort[] s_priv_ByteOrder_Msg13_SetBackgroundColor;
        private static ushort[] s_priv_ByteOrder_Msg15_SetUseSystemMemoryBitmaps;
        private static ushort[] s_priv_ByteOrder_Msg17_SetEnableBackBuffer;
        private static ushort[] s_priv_ByteOrder_Msg19_SetAllowAnimations;
        private static ushort[] s_priv_ByteOrder_Msg21_SetAllowAlpha;

        protected RemoteGdiDevice()
        {
        }

        public static unsafe RemoteGdiDevice Create(
          ProtocolSplashRenderingNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          LocalDeviceCallback cb)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE deviceClassHandle = _priv_protocolInstance.GdiDevice_ClassHandle;
            RemoteGdiDevice remoteGdiDevice = new RemoteGdiDevice(port, _priv_owner);
            uint num = (uint)sizeof(RemoteGdiDevice.Msg23_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteGdiDevice.Msg23_Create* msg23CreatePtr = (RemoteGdiDevice.Msg23_Create*)pMem;
            msg23CreatePtr->_priv_size = num;
            msg23CreatePtr->_priv_msgid = 23U;
            msg23CreatePtr->_priv_objcb = cb.RenderHandle;
            msg23CreatePtr->_priv_ctxcb = port.Session.LocalContext;
            msg23CreatePtr->_priv_idObjectSubject = remoteGdiDevice.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg23_Create, typeof(RemoteGdiDevice.Msg23_Create), 0, 0);
            port.CreateRemoteObject(deviceClassHandle, remoteGdiDevice.m_renderHandle, (Message*)msg23CreatePtr);
            return remoteGdiDevice;
        }

        public unsafe void BuildSecureDesktopMode(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fEnabled)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiDevice.Msg8_SecureDesktopMode);
            RemoteGdiDevice.Msg8_SecureDesktopMode* secureDesktopModePtr = (RemoteGdiDevice.Msg8_SecureDesktopMode*)_priv_portUse.AllocMessageBuffer(size);
            secureDesktopModePtr->_priv_size = size;
            secureDesktopModePtr->_priv_msgid = 8U;
            secureDesktopModePtr->fEnabled = fEnabled ? uint.MaxValue : 0U;
            secureDesktopModePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)secureDesktopModePtr, ref s_priv_ByteOrder_Msg8_SecureDesktopMode, typeof(RemoteGdiDevice.Msg8_SecureDesktopMode), 0, 0);
            _priv_pmsgUse = (Message*)secureDesktopModePtr;
        }

        public unsafe void SendSecureDesktopMode(bool fEnabled)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSecureDesktopMode(out _priv_portUse, out _priv_pmsgUse, fEnabled);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetZoomMode(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          VideoZoomMode vzmZoomMode)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiDevice.Msg9_SetZoomMode);
            RemoteGdiDevice.Msg9_SetZoomMode* msg9SetZoomModePtr = (RemoteGdiDevice.Msg9_SetZoomMode*)_priv_portUse.AllocMessageBuffer(size);
            msg9SetZoomModePtr->_priv_size = size;
            msg9SetZoomModePtr->_priv_msgid = 9U;
            msg9SetZoomModePtr->vzmZoomMode = vzmZoomMode;
            msg9SetZoomModePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg9SetZoomModePtr, ref s_priv_ByteOrder_Msg9_SetZoomMode, typeof(RemoteGdiDevice.Msg9_SetZoomMode), 0, 0);
            _priv_pmsgUse = (Message*)msg9SetZoomModePtr;
        }

        public unsafe void SendSetZoomMode(VideoZoomMode vzmZoomMode)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetZoomMode(out _priv_portUse, out _priv_pmsgUse, vzmZoomMode);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetEnableAlphaTracking(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fEnableAlphaTracking)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiDevice.Msg11_SetEnableAlphaTracking);
            RemoteGdiDevice.Msg11_SetEnableAlphaTracking* enableAlphaTrackingPtr = (RemoteGdiDevice.Msg11_SetEnableAlphaTracking*)_priv_portUse.AllocMessageBuffer(size);
            enableAlphaTrackingPtr->_priv_size = size;
            enableAlphaTrackingPtr->_priv_msgid = 11U;
            enableAlphaTrackingPtr->fEnableAlphaTracking = fEnableAlphaTracking ? uint.MaxValue : 0U;
            enableAlphaTrackingPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)enableAlphaTrackingPtr, ref s_priv_ByteOrder_Msg11_SetEnableAlphaTracking, typeof(RemoteGdiDevice.Msg11_SetEnableAlphaTracking), 0, 0);
            _priv_pmsgUse = (Message*)enableAlphaTrackingPtr;
        }

        public unsafe void SendSetEnableAlphaTracking(bool fEnableAlphaTracking)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEnableAlphaTracking(out _priv_portUse, out _priv_pmsgUse, fEnableAlphaTracking);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetBackgroundColor(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ColorF clrBackground)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiDevice.Msg13_SetBackgroundColor);
            RemoteGdiDevice.Msg13_SetBackgroundColor* setBackgroundColorPtr = (RemoteGdiDevice.Msg13_SetBackgroundColor*)_priv_portUse.AllocMessageBuffer(size);
            setBackgroundColorPtr->_priv_size = size;
            setBackgroundColorPtr->_priv_msgid = 13U;
            setBackgroundColorPtr->clrBackground = clrBackground;
            setBackgroundColorPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setBackgroundColorPtr, ref s_priv_ByteOrder_Msg13_SetBackgroundColor, typeof(RemoteGdiDevice.Msg13_SetBackgroundColor), 0, 0);
            _priv_pmsgUse = (Message*)setBackgroundColorPtr;
        }

        public unsafe void SendSetBackgroundColor(ColorF clrBackground)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetBackgroundColor(out _priv_portUse, out _priv_pmsgUse, clrBackground);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetUseSystemMemoryBitmaps(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fSystemMemoryBitmaps)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiDevice.Msg15_SetUseSystemMemoryBitmaps);
            RemoteGdiDevice.Msg15_SetUseSystemMemoryBitmaps* systemMemoryBitmapsPtr = (RemoteGdiDevice.Msg15_SetUseSystemMemoryBitmaps*)_priv_portUse.AllocMessageBuffer(size);
            systemMemoryBitmapsPtr->_priv_size = size;
            systemMemoryBitmapsPtr->_priv_msgid = 15U;
            systemMemoryBitmapsPtr->fSystemMemoryBitmaps = fSystemMemoryBitmaps ? uint.MaxValue : 0U;
            systemMemoryBitmapsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)systemMemoryBitmapsPtr, ref s_priv_ByteOrder_Msg15_SetUseSystemMemoryBitmaps, typeof(RemoteGdiDevice.Msg15_SetUseSystemMemoryBitmaps), 0, 0);
            _priv_pmsgUse = (Message*)systemMemoryBitmapsPtr;
        }

        public unsafe void SendSetUseSystemMemoryBitmaps(bool fSystemMemoryBitmaps)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetUseSystemMemoryBitmaps(out _priv_portUse, out _priv_pmsgUse, fSystemMemoryBitmaps);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetEnableBackBuffer(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fEnableBackBuffer)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiDevice.Msg17_SetEnableBackBuffer);
            RemoteGdiDevice.Msg17_SetEnableBackBuffer* enableBackBufferPtr = (RemoteGdiDevice.Msg17_SetEnableBackBuffer*)_priv_portUse.AllocMessageBuffer(size);
            enableBackBufferPtr->_priv_size = size;
            enableBackBufferPtr->_priv_msgid = 17U;
            enableBackBufferPtr->fEnableBackBuffer = fEnableBackBuffer ? uint.MaxValue : 0U;
            enableBackBufferPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)enableBackBufferPtr, ref s_priv_ByteOrder_Msg17_SetEnableBackBuffer, typeof(RemoteGdiDevice.Msg17_SetEnableBackBuffer), 0, 0);
            _priv_pmsgUse = (Message*)enableBackBufferPtr;
        }

        public unsafe void SendSetEnableBackBuffer(bool fEnableBackBuffer)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEnableBackBuffer(out _priv_portUse, out _priv_pmsgUse, fEnableBackBuffer);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetAllowAnimations(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fAllowAnimations)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiDevice.Msg19_SetAllowAnimations);
            RemoteGdiDevice.Msg19_SetAllowAnimations* setAllowAnimationsPtr = (RemoteGdiDevice.Msg19_SetAllowAnimations*)_priv_portUse.AllocMessageBuffer(size);
            setAllowAnimationsPtr->_priv_size = size;
            setAllowAnimationsPtr->_priv_msgid = 19U;
            setAllowAnimationsPtr->fAllowAnimations = fAllowAnimations ? uint.MaxValue : 0U;
            setAllowAnimationsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setAllowAnimationsPtr, ref s_priv_ByteOrder_Msg19_SetAllowAnimations, typeof(RemoteGdiDevice.Msg19_SetAllowAnimations), 0, 0);
            _priv_pmsgUse = (Message*)setAllowAnimationsPtr;
        }

        public unsafe void SendSetAllowAnimations(bool fAllowAnimations)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetAllowAnimations(out _priv_portUse, out _priv_pmsgUse, fAllowAnimations);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetAllowAlpha(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fAllowAlpha)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiDevice.Msg21_SetAllowAlpha);
            RemoteGdiDevice.Msg21_SetAllowAlpha* msg21SetAllowAlphaPtr = (RemoteGdiDevice.Msg21_SetAllowAlpha*)_priv_portUse.AllocMessageBuffer(size);
            msg21SetAllowAlphaPtr->_priv_size = size;
            msg21SetAllowAlphaPtr->_priv_msgid = 21U;
            msg21SetAllowAlphaPtr->fAllowAlpha = fAllowAlpha ? uint.MaxValue : 0U;
            msg21SetAllowAlphaPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg21SetAllowAlphaPtr, ref s_priv_ByteOrder_Msg21_SetAllowAlpha, typeof(RemoteGdiDevice.Msg21_SetAllowAlpha), 0, 0);
            _priv_pmsgUse = (Message*)msg21SetAllowAlphaPtr;
        }

        public unsafe void SendSetAllowAlpha(bool fAllowAlpha)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetAllowAlpha(out _priv_portUse, out _priv_pmsgUse, fAllowAlpha);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteGdiDevice CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteGdiDevice(port, handle, true);
        }

        public static RemoteGdiDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteGdiDevice(port, handle, false);
        }

        protected RemoteGdiDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteGdiDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteGdiDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg23_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
        }

        [ComVisible(false)]
        private struct Msg8_SecureDesktopMode
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fEnabled;
        }

        [ComVisible(false)]
        private struct Msg9_SetZoomMode
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public VideoZoomMode vzmZoomMode;
        }

        [ComVisible(false)]
        private struct Msg11_SetEnableAlphaTracking
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fEnableAlphaTracking;
        }

        [ComVisible(false)]
        private struct Msg13_SetBackgroundColor
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ColorF clrBackground;
        }

        [ComVisible(false)]
        private struct Msg15_SetUseSystemMemoryBitmaps
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fSystemMemoryBitmaps;
        }

        [ComVisible(false)]
        private struct Msg17_SetEnableBackBuffer
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fEnableBackBuffer;
        }

        [ComVisible(false)]
        private struct Msg19_SetAllowAnimations
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fAllowAnimations;
        }

        [ComVisible(false)]
        private struct Msg21_SetAllowAlpha
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fAllowAlpha;
        }
    }
}
