// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteSurface
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteSurface : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_RemapContainer;
        private static ushort[] s_priv_ByteOrder_Msg1_RemapLocation;
        private static ushort[] s_priv_ByteOrder_Msg2_MarkContentValid;
        private static ushort[] s_priv_ByteOrder_Msg3_Clear;
        private static ushort[] s_priv_ByteOrder_Msg5_SetGutter;
        private static ushort[] s_priv_ByteOrder_Msg6_SetDescription;
        private static ushort[] s_priv_ByteOrder_Msg7_SetRotation;
        private static ushort[] s_priv_ByteOrder_Msg8_SetStorageSize;

        protected RemoteSurface()
        {
        }

        public unsafe void BuildRemapContainer(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteSurfacePool poolNewContainer)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (poolNewContainer != null)
                _priv_portUse.ValidateHandleOrNull(poolNewContainer.RenderHandle);
            uint size = (uint)sizeof(RemoteSurface.Msg0_RemapContainer);
            RemoteSurface.Msg0_RemapContainer* msg0RemapContainerPtr = (RemoteSurface.Msg0_RemapContainer*)_priv_portUse.AllocMessageBuffer(size);
            msg0RemapContainerPtr->_priv_size = size;
            msg0RemapContainerPtr->_priv_msgid = 0U;
            msg0RemapContainerPtr->poolNewContainer = poolNewContainer != null ? poolNewContainer.RenderHandle : RENDERHANDLE.NULL;
            msg0RemapContainerPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0RemapContainerPtr, ref s_priv_ByteOrder_Msg0_RemapContainer, typeof(RemoteSurface.Msg0_RemapContainer), 0, 0);
            _priv_pmsgUse = (Message*)msg0RemapContainerPtr;
        }

        public unsafe void SendRemapContainer(RemoteSurfacePool poolNewContainer)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRemapContainer(out _priv_portUse, out _priv_pmsgUse, poolNewContainer);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRemapLocation(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Rectangle rcContentPxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurface.Msg1_RemapLocation);
            RemoteSurface.Msg1_RemapLocation* msg1RemapLocationPtr = (RemoteSurface.Msg1_RemapLocation*)_priv_portUse.AllocMessageBuffer(size);
            msg1RemapLocationPtr->_priv_size = size;
            msg1RemapLocationPtr->_priv_msgid = 1U;
            msg1RemapLocationPtr->rcContentPxl = rcContentPxl;
            msg1RemapLocationPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1RemapLocationPtr, ref s_priv_ByteOrder_Msg1_RemapLocation, typeof(RemoteSurface.Msg1_RemapLocation), 0, 0);
            _priv_pmsgUse = (Message*)msg1RemapLocationPtr;
        }

        public unsafe void SendRemapLocation(Rectangle rcContentPxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRemapLocation(out _priv_portUse, out _priv_pmsgUse, rcContentPxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildMarkContentValid(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurface.Msg2_MarkContentValid);
            RemoteSurface.Msg2_MarkContentValid* markContentValidPtr = (RemoteSurface.Msg2_MarkContentValid*)_priv_portUse.AllocMessageBuffer(size);
            markContentValidPtr->_priv_size = size;
            markContentValidPtr->_priv_msgid = 2U;
            markContentValidPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)markContentValidPtr, ref s_priv_ByteOrder_Msg2_MarkContentValid, typeof(RemoteSurface.Msg2_MarkContentValid), 0, 0);
            _priv_pmsgUse = (Message*)markContentValidPtr;
        }

        public unsafe void SendMarkContentValid()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildMarkContentValid(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildClear(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurface.Msg3_Clear);
            RemoteSurface.Msg3_Clear* msg3ClearPtr = (RemoteSurface.Msg3_Clear*)_priv_portUse.AllocMessageBuffer(size);
            msg3ClearPtr->_priv_size = size;
            msg3ClearPtr->_priv_msgid = 3U;
            msg3ClearPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3ClearPtr, ref s_priv_ByteOrder_Msg3_Clear, typeof(RemoteSurface.Msg3_Clear), 0, 0);
            _priv_pmsgUse = (Message*)msg3ClearPtr;
        }

        public unsafe void SendClear()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildClear(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetGutter(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizeGutter)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurface.Msg5_SetGutter);
            RemoteSurface.Msg5_SetGutter* msg5SetGutterPtr = (RemoteSurface.Msg5_SetGutter*)_priv_portUse.AllocMessageBuffer(size);
            msg5SetGutterPtr->_priv_size = size;
            msg5SetGutterPtr->_priv_msgid = 5U;
            msg5SetGutterPtr->sizeGutter = sizeGutter;
            msg5SetGutterPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg5SetGutterPtr, ref s_priv_ByteOrder_Msg5_SetGutter, typeof(RemoteSurface.Msg5_SetGutter), 0, 0);
            _priv_pmsgUse = (Message*)msg5SetGutterPtr;
        }

        public unsafe void SendSetGutter(Size sizeGutter)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetGutter(out _priv_portUse, out _priv_pmsgUse, sizeGutter);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetDescription(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stDescription)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteSurface.Msg6_SetDescription));
            BLOBREF blobref = blobInfo.Add(stDescription);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteSurface.Msg6_SetDescription* msg6SetDescriptionPtr = (RemoteSurface.Msg6_SetDescription*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg6SetDescriptionPtr->_priv_size = adjustedTotalSize;
            msg6SetDescriptionPtr->_priv_msgid = 6U;
            msg6SetDescriptionPtr->stDescription = blobref;
            blobInfo.Attach((Message*)msg6SetDescriptionPtr);
            msg6SetDescriptionPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg6SetDescriptionPtr, ref s_priv_ByteOrder_Msg6_SetDescription, typeof(RemoteSurface.Msg6_SetDescription), 0, 0);
            _priv_pmsgUse = (Message*)msg6SetDescriptionPtr;
        }

        public unsafe void SendSetDescription(string stDescription)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetDescription(out _priv_portUse, out _priv_pmsgUse, stDescription);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetRotation(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fRotated)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurface.Msg7_SetRotation);
            RemoteSurface.Msg7_SetRotation* msg7SetRotationPtr = (RemoteSurface.Msg7_SetRotation*)_priv_portUse.AllocMessageBuffer(size);
            msg7SetRotationPtr->_priv_size = size;
            msg7SetRotationPtr->_priv_msgid = 7U;
            msg7SetRotationPtr->fRotated = fRotated ? uint.MaxValue : 0U;
            msg7SetRotationPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg7SetRotationPtr, ref s_priv_ByteOrder_Msg7_SetRotation, typeof(RemoteSurface.Msg7_SetRotation), 0, 0);
            _priv_pmsgUse = (Message*)msg7SetRotationPtr;
        }

        public unsafe void SendSetRotation(bool fRotated)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetRotation(out _priv_portUse, out _priv_pmsgUse, fRotated);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetStorageSize(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizeStoragePxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSurface.Msg8_SetStorageSize);
            RemoteSurface.Msg8_SetStorageSize* msg8SetStorageSizePtr = (RemoteSurface.Msg8_SetStorageSize*)_priv_portUse.AllocMessageBuffer(size);
            msg8SetStorageSizePtr->_priv_size = size;
            msg8SetStorageSizePtr->_priv_msgid = 8U;
            msg8SetStorageSizePtr->sizeStoragePxl = sizeStoragePxl;
            msg8SetStorageSizePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg8SetStorageSizePtr, ref s_priv_ByteOrder_Msg8_SetStorageSize, typeof(RemoteSurface.Msg8_SetStorageSize), 0, 0);
            _priv_pmsgUse = (Message*)msg8SetStorageSizePtr;
        }

        public unsafe void SendSetStorageSize(Size sizeStoragePxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetStorageSize(out _priv_portUse, out _priv_pmsgUse, sizeStoragePxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteSurface CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteSurface(port, handle, true);

        public static RemoteSurface CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteSurface(port, handle, false);
        }

        protected RemoteSurface(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteSurface(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteSurface && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_RemapContainer
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE poolNewContainer;
        }

        [ComVisible(false)]
        private struct Msg1_RemapLocation
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Rectangle rcContentPxl;
        }

        [ComVisible(false)]
        private struct Msg2_MarkContentValid
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg3_Clear
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg5_SetGutter
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizeGutter;
        }

        [ComVisible(false)]
        private struct Msg6_SetDescription
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stDescription;
        }

        [ComVisible(false)]
        private struct Msg7_SetRotation
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fRotated;
        }

        [ComVisible(false)]
        private struct Msg8_SetStorageSize
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizeStoragePxl;
        }
    }
}
