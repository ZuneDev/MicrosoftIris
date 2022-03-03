// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteDevice : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_TriggerDeviceReset;
        private static ushort[] s_priv_ByteOrder_Msg1_EndCaptureBackBuffer;
        private static ushort[] s_priv_ByteOrder_Msg2_BeginCaptureBackBuffer;
        private static ushort[] s_priv_ByteOrder_Msg3_CreateGradient;
        private static ushort[] s_priv_ByteOrder_Msg4_Stop;
        private static ushort[] s_priv_ByteOrder_Msg5_Restart;
        private static ushort[] s_priv_ByteOrder_Msg6_CreateSurfacePool;

        protected RemoteDevice()
        {
        }

        public unsafe void BuildTriggerDeviceReset(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDevice.Msg0_TriggerDeviceReset);
            RemoteDevice.Msg0_TriggerDeviceReset* triggerDeviceResetPtr = (RemoteDevice.Msg0_TriggerDeviceReset*)_priv_portUse.AllocMessageBuffer(size);
            triggerDeviceResetPtr->_priv_size = size;
            triggerDeviceResetPtr->_priv_msgid = 0U;
            triggerDeviceResetPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)triggerDeviceResetPtr, ref s_priv_ByteOrder_Msg0_TriggerDeviceReset, typeof(RemoteDevice.Msg0_TriggerDeviceReset), 0, 0);
            _priv_pmsgUse = (Message*)triggerDeviceResetPtr;
        }

        public unsafe void SendTriggerDeviceReset()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildTriggerDeviceReset(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildEndCaptureBackBuffer(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDevice.Msg1_EndCaptureBackBuffer);
            RemoteDevice.Msg1_EndCaptureBackBuffer* captureBackBufferPtr = (RemoteDevice.Msg1_EndCaptureBackBuffer*)_priv_portUse.AllocMessageBuffer(size);
            captureBackBufferPtr->_priv_size = size;
            captureBackBufferPtr->_priv_msgid = 1U;
            captureBackBufferPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)captureBackBufferPtr, ref s_priv_ByteOrder_Msg1_EndCaptureBackBuffer, typeof(RemoteDevice.Msg1_EndCaptureBackBuffer), 0, 0);
            _priv_pmsgUse = (Message*)captureBackBufferPtr;
        }

        public unsafe void SendEndCaptureBackBuffer()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildEndCaptureBackBuffer(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildBeginCaptureBackBuffer(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stFileName)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteDevice.Msg2_BeginCaptureBackBuffer));
            BLOBREF blobref = blobInfo.Add(stFileName);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteDevice.Msg2_BeginCaptureBackBuffer* captureBackBufferPtr = (RemoteDevice.Msg2_BeginCaptureBackBuffer*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            captureBackBufferPtr->_priv_size = adjustedTotalSize;
            captureBackBufferPtr->_priv_msgid = 2U;
            captureBackBufferPtr->stFileName = blobref;
            blobInfo.Attach((Message*)captureBackBufferPtr);
            captureBackBufferPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)captureBackBufferPtr, ref s_priv_ByteOrder_Msg2_BeginCaptureBackBuffer, typeof(RemoteDevice.Msg2_BeginCaptureBackBuffer), 0, 0);
            _priv_pmsgUse = (Message*)captureBackBufferPtr;
        }

        public unsafe void SendBeginCaptureBackBuffer(string stFileName)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildBeginCaptureBackBuffer(out _priv_portUse, out _priv_pmsgUse, stFileName);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildCreateGradient(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RENDERHANDLE idNewGradient)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDevice.Msg3_CreateGradient);
            RemoteDevice.Msg3_CreateGradient* msg3CreateGradientPtr = (RemoteDevice.Msg3_CreateGradient*)_priv_portUse.AllocMessageBuffer(size);
            msg3CreateGradientPtr->_priv_size = size;
            msg3CreateGradientPtr->_priv_msgid = 3U;
            msg3CreateGradientPtr->idNewGradient = idNewGradient;
            msg3CreateGradientPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3CreateGradientPtr, ref s_priv_ByteOrder_Msg3_CreateGradient, typeof(RemoteDevice.Msg3_CreateGradient), 0, 0);
            _priv_pmsgUse = (Message*)msg3CreateGradientPtr;
        }

        public unsafe void SendCreateGradient(RENDERHANDLE idNewGradient)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateGradient(out _priv_portUse, out _priv_pmsgUse, idNewGradient);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildStop(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDevice.Msg4_Stop);
            RemoteDevice.Msg4_Stop* msg4StopPtr = (RemoteDevice.Msg4_Stop*)_priv_portUse.AllocMessageBuffer(size);
            msg4StopPtr->_priv_size = size;
            msg4StopPtr->_priv_msgid = 4U;
            msg4StopPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4StopPtr, ref s_priv_ByteOrder_Msg4_Stop, typeof(RemoteDevice.Msg4_Stop), 0, 0);
            _priv_pmsgUse = (Message*)msg4StopPtr;
        }

        public unsafe void SendStop()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildStop(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRestart(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint nRenderGeneration)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDevice.Msg5_Restart);
            RemoteDevice.Msg5_Restart* msg5RestartPtr = (RemoteDevice.Msg5_Restart*)_priv_portUse.AllocMessageBuffer(size);
            msg5RestartPtr->_priv_size = size;
            msg5RestartPtr->_priv_msgid = 5U;
            msg5RestartPtr->nRenderGeneration = nRenderGeneration;
            msg5RestartPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg5RestartPtr, ref s_priv_ByteOrder_Msg5_Restart, typeof(RemoteDevice.Msg5_Restart), 0, 0);
            _priv_pmsgUse = (Message*)msg5RestartPtr;
        }

        public unsafe void SendRestart(uint nRenderGeneration)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRestart(out _priv_portUse, out _priv_pmsgUse, nRenderGeneration);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildCreateSurfacePool(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RENDERHANDLE idNewSurface,
          SurfaceFormat nFormat)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDevice.Msg6_CreateSurfacePool);
            RemoteDevice.Msg6_CreateSurfacePool* createSurfacePoolPtr = (RemoteDevice.Msg6_CreateSurfacePool*)_priv_portUse.AllocMessageBuffer(size);
            createSurfacePoolPtr->_priv_size = size;
            createSurfacePoolPtr->_priv_msgid = 6U;
            createSurfacePoolPtr->idNewSurface = idNewSurface;
            createSurfacePoolPtr->nFormat = nFormat;
            createSurfacePoolPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)createSurfacePoolPtr, ref s_priv_ByteOrder_Msg6_CreateSurfacePool, typeof(RemoteDevice.Msg6_CreateSurfacePool), 0, 0);
            _priv_pmsgUse = (Message*)createSurfacePoolPtr;
        }

        public unsafe void SendCreateSurfacePool(RENDERHANDLE idNewSurface, SurfaceFormat nFormat)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateSurfacePool(out _priv_portUse, out _priv_pmsgUse, idNewSurface, nFormat);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteDevice CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteDevice(port, handle, true);

        public static RemoteDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDevice(port, handle, false);
        }

        protected RemoteDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_TriggerDeviceReset
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg1_EndCaptureBackBuffer
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg2_BeginCaptureBackBuffer
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stFileName;
        }

        [ComVisible(false)]
        private struct Msg3_CreateGradient
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idNewGradient;
        }

        [ComVisible(false)]
        private struct Msg4_Stop
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg5_Restart
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nRenderGeneration;
        }

        [ComVisible(false)]
        private struct Msg6_CreateSurfacePool
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idNewSurface;
            public SurfaceFormat nFormat;
        }
    }
}
