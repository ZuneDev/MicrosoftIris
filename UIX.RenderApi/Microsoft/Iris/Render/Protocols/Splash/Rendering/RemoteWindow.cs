// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteWindow
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteWindow : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_SetBackgroundColor;
        private static ushort[] s_priv_ByteOrder_Msg1_SetOutlineMarkedColor;
        private static ushort[] s_priv_ByteOrder_Msg2_SetOutlineAllColor;
        private static ushort[] s_priv_ByteOrder_Msg3_ChangeDataBits;
        private static ushort[] s_priv_ByteOrder_Msg5_SetRoot;

        protected RemoteWindow()
        {
        }

        public unsafe void BuildSetBackgroundColor(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ColorF clrBack)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteWindow.Msg0_SetBackgroundColor);
            RemoteWindow.Msg0_SetBackgroundColor* setBackgroundColorPtr = (RemoteWindow.Msg0_SetBackgroundColor*)_priv_portUse.AllocMessageBuffer(size);
            setBackgroundColorPtr->_priv_size = size;
            setBackgroundColorPtr->_priv_msgid = 0U;
            setBackgroundColorPtr->clrBack = clrBack;
            setBackgroundColorPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setBackgroundColorPtr, ref s_priv_ByteOrder_Msg0_SetBackgroundColor, typeof(RemoteWindow.Msg0_SetBackgroundColor), 0, 0);
            _priv_pmsgUse = (Message*)setBackgroundColorPtr;
        }

        public unsafe void SendSetBackgroundColor(ColorF clrBack)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetBackgroundColor(out _priv_portUse, out _priv_pmsgUse, clrBack);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetOutlineMarkedColor(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ColorF clr)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteWindow.Msg1_SetOutlineMarkedColor);
            RemoteWindow.Msg1_SetOutlineMarkedColor* outlineMarkedColorPtr = (RemoteWindow.Msg1_SetOutlineMarkedColor*)_priv_portUse.AllocMessageBuffer(size);
            outlineMarkedColorPtr->_priv_size = size;
            outlineMarkedColorPtr->_priv_msgid = 1U;
            outlineMarkedColorPtr->clr = clr;
            outlineMarkedColorPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)outlineMarkedColorPtr, ref s_priv_ByteOrder_Msg1_SetOutlineMarkedColor, typeof(RemoteWindow.Msg1_SetOutlineMarkedColor), 0, 0);
            _priv_pmsgUse = (Message*)outlineMarkedColorPtr;
        }

        public unsafe void SendSetOutlineMarkedColor(ColorF clr)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetOutlineMarkedColor(out _priv_portUse, out _priv_pmsgUse, clr);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetOutlineAllColor(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ColorF clr)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteWindow.Msg2_SetOutlineAllColor);
            RemoteWindow.Msg2_SetOutlineAllColor* setOutlineAllColorPtr = (RemoteWindow.Msg2_SetOutlineAllColor*)_priv_portUse.AllocMessageBuffer(size);
            setOutlineAllColorPtr->_priv_size = size;
            setOutlineAllColorPtr->_priv_msgid = 2U;
            setOutlineAllColorPtr->clr = clr;
            setOutlineAllColorPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setOutlineAllColorPtr, ref s_priv_ByteOrder_Msg2_SetOutlineAllColor, typeof(RemoteWindow.Msg2_SetOutlineAllColor), 0, 0);
            _priv_pmsgUse = (Message*)setOutlineAllColorPtr;
        }

        public unsafe void SendSetOutlineAllColor(ColorF clr)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetOutlineAllColor(out _priv_portUse, out _priv_pmsgUse, clr);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildChangeDataBits(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint nValue,
          uint nMask)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteWindow.Msg3_ChangeDataBits);
            RemoteWindow.Msg3_ChangeDataBits* msg3ChangeDataBitsPtr = (RemoteWindow.Msg3_ChangeDataBits*)_priv_portUse.AllocMessageBuffer(size);
            msg3ChangeDataBitsPtr->_priv_size = size;
            msg3ChangeDataBitsPtr->_priv_msgid = 3U;
            msg3ChangeDataBitsPtr->nValue = nValue;
            msg3ChangeDataBitsPtr->nMask = nMask;
            msg3ChangeDataBitsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3ChangeDataBitsPtr, ref s_priv_ByteOrder_Msg3_ChangeDataBits, typeof(RemoteWindow.Msg3_ChangeDataBits), 0, 0);
            _priv_pmsgUse = (Message*)msg3ChangeDataBitsPtr;
        }

        public unsafe void SendChangeDataBits(uint nValue, uint nMask)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildChangeDataBits(out _priv_portUse, out _priv_pmsgUse, nValue, nMask);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetRoot(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteVisual visRoot)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (visRoot != null)
                _priv_portUse.ValidateHandleOrNull(visRoot.RenderHandle);
            uint size = (uint)sizeof(RemoteWindow.Msg5_SetRoot);
            RemoteWindow.Msg5_SetRoot* msg5SetRootPtr = (RemoteWindow.Msg5_SetRoot*)_priv_portUse.AllocMessageBuffer(size);
            msg5SetRootPtr->_priv_size = size;
            msg5SetRootPtr->_priv_msgid = 5U;
            msg5SetRootPtr->visRoot = visRoot != null ? visRoot.RenderHandle : RENDERHANDLE.NULL;
            msg5SetRootPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg5SetRootPtr, ref s_priv_ByteOrder_Msg5_SetRoot, typeof(RemoteWindow.Msg5_SetRoot), 0, 0);
            _priv_pmsgUse = (Message*)msg5SetRootPtr;
        }

        public unsafe void SendSetRoot(RemoteVisual visRoot)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetRoot(out _priv_portUse, out _priv_pmsgUse, visRoot);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteWindow CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteWindow(port, handle, true);

        public static RemoteWindow CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteWindow(port, handle, false);
        }

        protected RemoteWindow(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteWindow(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteWindow && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_SetBackgroundColor
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ColorF clrBack;
        }

        [ComVisible(false)]
        private struct Msg1_SetOutlineMarkedColor
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ColorF clr;
        }

        [ComVisible(false)]
        private struct Msg2_SetOutlineAllColor
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ColorF clr;
        }

        [ComVisible(false)]
        private struct Msg3_ChangeDataBits
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nValue;
            public uint nMask;
        }

        [ComVisible(false)]
        private struct Msg5_SetRoot
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE visRoot;
        }
    }
}
