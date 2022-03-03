// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt.RemoteHwndHostWindow
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt
{
    internal class RemoteHwndHostWindow : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg4_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_SetVisible;
        private static ushort[] s_priv_ByteOrder_Msg1_SetSize;
        private static ushort[] s_priv_ByteOrder_Msg2_SetPosition;
        private static ushort[] s_priv_ByteOrder_Msg3_SetBackgroundColor;

        protected RemoteHwndHostWindow()
        {
        }

        public static unsafe RemoteHwndHostWindow Create(
          ProtocolSplashDesktopNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          RemoteFormWindow winParent,
          LocalHwndHostWindowCallback cb)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE windowClassHandle = _priv_protocolInstance.HwndHostWindow_ClassHandle;
            RemoteHwndHostWindow remoteHwndHostWindow = new RemoteHwndHostWindow(port, _priv_owner);
            uint num = (uint)sizeof(RemoteHwndHostWindow.Msg4_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteHwndHostWindow.Msg4_Create* msg4CreatePtr = (RemoteHwndHostWindow.Msg4_Create*)pMem;
            msg4CreatePtr->_priv_size = num;
            msg4CreatePtr->_priv_msgid = 4U;
            msg4CreatePtr->winParent = winParent != null ? winParent.RenderHandle : RENDERHANDLE.NULL;
            msg4CreatePtr->_priv_objcb = cb.RenderHandle;
            msg4CreatePtr->_priv_ctxcb = port.Session.LocalContext;
            msg4CreatePtr->_priv_idObjectSubject = remoteHwndHostWindow.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg4_Create, typeof(RemoteHwndHostWindow.Msg4_Create), 0, 0);
            port.CreateRemoteObject(windowClassHandle, remoteHwndHostWindow.m_renderHandle, (Message*)msg4CreatePtr);
            return remoteHwndHostWindow;
        }

        public unsafe void BuildSetVisible(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fVisible)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteHwndHostWindow.Msg0_SetVisible);
            RemoteHwndHostWindow.Msg0_SetVisible* msg0SetVisiblePtr = (RemoteHwndHostWindow.Msg0_SetVisible*)_priv_portUse.AllocMessageBuffer(size);
            msg0SetVisiblePtr->_priv_size = size;
            msg0SetVisiblePtr->_priv_msgid = 0U;
            msg0SetVisiblePtr->fVisible = fVisible ? uint.MaxValue : 0U;
            msg0SetVisiblePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0SetVisiblePtr, ref s_priv_ByteOrder_Msg0_SetVisible, typeof(RemoteHwndHostWindow.Msg0_SetVisible), 0, 0);
            _priv_pmsgUse = (Message*)msg0SetVisiblePtr;
        }

        public unsafe void SendSetVisible(bool fVisible)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetVisible(out _priv_portUse, out _priv_pmsgUse, fVisible);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetSize(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizeWindowPxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteHwndHostWindow.Msg1_SetSize);
            RemoteHwndHostWindow.Msg1_SetSize* msg1SetSizePtr = (RemoteHwndHostWindow.Msg1_SetSize*)_priv_portUse.AllocMessageBuffer(size);
            msg1SetSizePtr->_priv_size = size;
            msg1SetSizePtr->_priv_msgid = 1U;
            msg1SetSizePtr->sizeWindowPxl = sizeWindowPxl;
            msg1SetSizePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1SetSizePtr, ref s_priv_ByteOrder_Msg1_SetSize, typeof(RemoteHwndHostWindow.Msg1_SetSize), 0, 0);
            _priv_pmsgUse = (Message*)msg1SetSizePtr;
        }

        public unsafe void SendSetSize(Size sizeWindowPxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetSize(out _priv_portUse, out _priv_pmsgUse, sizeWindowPxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetPosition(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Point ptClientPxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteHwndHostWindow.Msg2_SetPosition);
            RemoteHwndHostWindow.Msg2_SetPosition* msg2SetPositionPtr = (RemoteHwndHostWindow.Msg2_SetPosition*)_priv_portUse.AllocMessageBuffer(size);
            msg2SetPositionPtr->_priv_size = size;
            msg2SetPositionPtr->_priv_msgid = 2U;
            msg2SetPositionPtr->ptClientPxl = ptClientPxl;
            msg2SetPositionPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2SetPositionPtr, ref s_priv_ByteOrder_Msg2_SetPosition, typeof(RemoteHwndHostWindow.Msg2_SetPosition), 0, 0);
            _priv_pmsgUse = (Message*)msg2SetPositionPtr;
        }

        public unsafe void SendSetPosition(Point ptClientPxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetPosition(out _priv_portUse, out _priv_pmsgUse, ptClientPxl);
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
            uint size = (uint)sizeof(RemoteHwndHostWindow.Msg3_SetBackgroundColor);
            RemoteHwndHostWindow.Msg3_SetBackgroundColor* setBackgroundColorPtr = (RemoteHwndHostWindow.Msg3_SetBackgroundColor*)_priv_portUse.AllocMessageBuffer(size);
            setBackgroundColorPtr->_priv_size = size;
            setBackgroundColorPtr->_priv_msgid = 3U;
            setBackgroundColorPtr->clrBackground = clrBackground;
            setBackgroundColorPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setBackgroundColorPtr, ref s_priv_ByteOrder_Msg3_SetBackgroundColor, typeof(RemoteHwndHostWindow.Msg3_SetBackgroundColor), 0, 0);
            _priv_pmsgUse = (Message*)setBackgroundColorPtr;
        }

        public unsafe void SendSetBackgroundColor(ColorF clrBackground)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetBackgroundColor(out _priv_portUse, out _priv_pmsgUse, clrBackground);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteHwndHostWindow CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteHwndHostWindow(port, handle, true);
        }

        public static RemoteHwndHostWindow CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteHwndHostWindow(port, handle, false);
        }

        protected RemoteHwndHostWindow(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteHwndHostWindow(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteHwndHostWindow && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg4_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE winParent;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
        }

        [ComVisible(false)]
        private struct Msg0_SetVisible
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fVisible;
        }

        [ComVisible(false)]
        private struct Msg1_SetSize
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizeWindowPxl;
        }

        [ComVisible(false)]
        private struct Msg2_SetPosition
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Point ptClientPxl;
        }

        [ComVisible(false)]
        private struct Msg3_SetBackgroundColor
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ColorF clrBackground;
        }
    }
}
