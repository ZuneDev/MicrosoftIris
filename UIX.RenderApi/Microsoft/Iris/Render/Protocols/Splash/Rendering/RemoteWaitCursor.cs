// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteWaitCursor
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteWaitCursor : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg5_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_Show;
        private static ushort[] s_priv_ByteOrder_Msg1_Hide;
        private static ushort[] s_priv_ByteOrder_Msg2_SetVisuals;
        private static ushort[] s_priv_ByteOrder_Msg3_SetShowAnimations;
        private static ushort[] s_priv_ByteOrder_Msg4_SetHideAnimations;

        protected RemoteWaitCursor()
        {
        }

        public static unsafe RemoteWaitCursor Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE cursorClassHandle = _priv_protocolInstance.WaitCursor_ClassHandle;
            RemoteWaitCursor remoteWaitCursor = new RemoteWaitCursor(port, _priv_owner);
            uint num = (uint)sizeof(RemoteWaitCursor.Msg5_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteWaitCursor.Msg5_Create* msg5CreatePtr = (RemoteWaitCursor.Msg5_Create*)pMem;
            msg5CreatePtr->_priv_size = num;
            msg5CreatePtr->_priv_msgid = 5U;
            msg5CreatePtr->_priv_idObjectSubject = remoteWaitCursor.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg5_Create, typeof(RemoteWaitCursor.Msg5_Create), 0, 0);
            port.CreateRemoteObject(cursorClassHandle, remoteWaitCursor.m_renderHandle, (Message*)msg5CreatePtr);
            return remoteWaitCursor;
        }

        public unsafe void BuildShow(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteWaitCursor.Msg0_Show);
            RemoteWaitCursor.Msg0_Show* msg0ShowPtr = (RemoteWaitCursor.Msg0_Show*)_priv_portUse.AllocMessageBuffer(size);
            msg0ShowPtr->_priv_size = size;
            msg0ShowPtr->_priv_msgid = 0U;
            msg0ShowPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0ShowPtr, ref s_priv_ByteOrder_Msg0_Show, typeof(RemoteWaitCursor.Msg0_Show), 0, 0);
            _priv_pmsgUse = (Message*)msg0ShowPtr;
        }

        public unsafe void SendShow()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildShow(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildHide(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteWaitCursor.Msg1_Hide);
            RemoteWaitCursor.Msg1_Hide* msg1HidePtr = (RemoteWaitCursor.Msg1_Hide*)_priv_portUse.AllocMessageBuffer(size);
            msg1HidePtr->_priv_size = size;
            msg1HidePtr->_priv_msgid = 1U;
            msg1HidePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1HidePtr, ref s_priv_ByteOrder_Msg1_Hide, typeof(RemoteWaitCursor.Msg1_Hide), 0, 0);
            _priv_pmsgUse = (Message*)msg1HidePtr;
        }

        public unsafe void SendHide()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildHide(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetVisuals(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteVisual[] arVisuals)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteWaitCursor.Msg2_SetVisuals));
            RENDERHANDLE[] renderHandleArray = MarshalHelper.CreateRenderHandleArray(arVisuals.Length);
            for (int index = 0; index < arVisuals.Length; ++index)
                renderHandleArray[index] = arVisuals[index].RenderHandle;
            BLOBREF blobref = blobInfo.Add(renderHandleArray);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteWaitCursor.Msg2_SetVisuals* msg2SetVisualsPtr = (RemoteWaitCursor.Msg2_SetVisuals*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg2SetVisualsPtr->_priv_size = adjustedTotalSize;
            msg2SetVisualsPtr->_priv_msgid = 2U;
            msg2SetVisualsPtr->arVisuals = blobref;
            blobInfo.Attach((Message*)msg2SetVisualsPtr);
            msg2SetVisualsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2SetVisualsPtr, ref s_priv_ByteOrder_Msg2_SetVisuals, typeof(RemoteWaitCursor.Msg2_SetVisuals), 0, 0);
            _priv_pmsgUse = (Message*)msg2SetVisualsPtr;
        }

        public unsafe void SendSetVisuals(RemoteVisual[] arVisuals)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetVisuals(out _priv_portUse, out _priv_pmsgUse, arVisuals);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetShowAnimations(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteAnimation[] arAnimations)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteWaitCursor.Msg3_SetShowAnimations));
            RENDERHANDLE[] renderHandleArray = MarshalHelper.CreateRenderHandleArray(arAnimations.Length);
            for (int index = 0; index < arAnimations.Length; ++index)
                renderHandleArray[index] = arAnimations[index].RenderHandle;
            BLOBREF blobref = blobInfo.Add(renderHandleArray);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteWaitCursor.Msg3_SetShowAnimations* setShowAnimationsPtr = (RemoteWaitCursor.Msg3_SetShowAnimations*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            setShowAnimationsPtr->_priv_size = adjustedTotalSize;
            setShowAnimationsPtr->_priv_msgid = 3U;
            setShowAnimationsPtr->arAnimations = blobref;
            blobInfo.Attach((Message*)setShowAnimationsPtr);
            setShowAnimationsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setShowAnimationsPtr, ref s_priv_ByteOrder_Msg3_SetShowAnimations, typeof(RemoteWaitCursor.Msg3_SetShowAnimations), 0, 0);
            _priv_pmsgUse = (Message*)setShowAnimationsPtr;
        }

        public unsafe void SendSetShowAnimations(RemoteAnimation[] arAnimations)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetShowAnimations(out _priv_portUse, out _priv_pmsgUse, arAnimations);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetHideAnimations(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteAnimation[] arAnimations)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteWaitCursor.Msg4_SetHideAnimations));
            RENDERHANDLE[] renderHandleArray = MarshalHelper.CreateRenderHandleArray(arAnimations.Length);
            for (int index = 0; index < arAnimations.Length; ++index)
                renderHandleArray[index] = arAnimations[index].RenderHandle;
            BLOBREF blobref = blobInfo.Add(renderHandleArray);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteWaitCursor.Msg4_SetHideAnimations* setHideAnimationsPtr = (RemoteWaitCursor.Msg4_SetHideAnimations*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            setHideAnimationsPtr->_priv_size = adjustedTotalSize;
            setHideAnimationsPtr->_priv_msgid = 4U;
            setHideAnimationsPtr->arAnimations = blobref;
            blobInfo.Attach((Message*)setHideAnimationsPtr);
            setHideAnimationsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setHideAnimationsPtr, ref s_priv_ByteOrder_Msg4_SetHideAnimations, typeof(RemoteWaitCursor.Msg4_SetHideAnimations), 0, 0);
            _priv_pmsgUse = (Message*)setHideAnimationsPtr;
        }

        public unsafe void SendSetHideAnimations(RemoteAnimation[] arAnimations)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetHideAnimations(out _priv_portUse, out _priv_pmsgUse, arAnimations);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteWaitCursor CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteWaitCursor(port, handle, true);
        }

        public static RemoteWaitCursor CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteWaitCursor(port, handle, false);
        }

        protected RemoteWaitCursor(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteWaitCursor(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteWaitCursor && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg5_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg0_Show
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg1_Hide
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg2_SetVisuals
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF arVisuals;
        }

        [ComVisible(false)]
        private struct Msg3_SetShowAnimations
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF arAnimations;
        }

        [ComVisible(false)]
        private struct Msg4_SetHideAnimations
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF arAnimations;
        }
    }
}
