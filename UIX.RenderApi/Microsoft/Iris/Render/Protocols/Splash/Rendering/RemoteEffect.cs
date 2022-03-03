// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteEffect
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteEffect : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_SetDebugID;
        private static ushort[] s_priv_ByteOrder_Msg1_SetProperty;
        private static ushort[] s_priv_ByteOrder_Msg2_SetProperty;
        private static ushort[] s_priv_ByteOrder_Msg3_SetProperty;
        private static ushort[] s_priv_ByteOrder_Msg4_SetProperty;
        private static ushort[] s_priv_ByteOrder_Msg5_SetProperty;
        private static ushort[] s_priv_ByteOrder_Msg6_SetProperty;
        private static ushort[] s_priv_ByteOrder_Msg7_SetProperty;

        protected RemoteEffect()
        {
        }

        public unsafe void BuildSetDebugID(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stDebugID)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteEffect.Msg0_SetDebugID));
            BLOBREF blobref = blobInfo.Add(stDebugID);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteEffect.Msg0_SetDebugID* msg0SetDebugIdPtr = (RemoteEffect.Msg0_SetDebugID*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg0SetDebugIdPtr->_priv_size = adjustedTotalSize;
            msg0SetDebugIdPtr->_priv_msgid = 0U;
            msg0SetDebugIdPtr->stDebugID = blobref;
            blobInfo.Attach((Message*)msg0SetDebugIdPtr);
            msg0SetDebugIdPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0SetDebugIdPtr, ref s_priv_ByteOrder_Msg0_SetDebugID, typeof(RemoteEffect.Msg0_SetDebugID), 0, 0);
            _priv_pmsgUse = (Message*)msg0SetDebugIdPtr;
        }

        public unsafe void SendSetDebugID(string stDebugID)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetDebugID(out _priv_portUse, out _priv_pmsgUse, stDebugID);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPropertyID,
          RemoteSurface[] surfaceArray)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteEffect.Msg1_SetProperty));
            RENDERHANDLE[] renderHandleArray = MarshalHelper.CreateRenderHandleArray(surfaceArray.Length);
            for (int index = 0; index < surfaceArray.Length; ++index)
                renderHandleArray[index] = surfaceArray[index].RenderHandle;
            BLOBREF blobref = blobInfo.Add(renderHandleArray);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteEffect.Msg1_SetProperty* msg1SetPropertyPtr = (RemoteEffect.Msg1_SetProperty*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg1SetPropertyPtr->_priv_size = adjustedTotalSize;
            msg1SetPropertyPtr->_priv_msgid = 1U;
            msg1SetPropertyPtr->nPropertyID = nPropertyID;
            msg1SetPropertyPtr->surfaceArray = blobref;
            blobInfo.Attach((Message*)msg1SetPropertyPtr);
            msg1SetPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1SetPropertyPtr, ref s_priv_ByteOrder_Msg1_SetProperty, typeof(RemoteEffect.Msg1_SetProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg1SetPropertyPtr;
        }

        public unsafe void SendSetProperty(int nPropertyID, RemoteSurface[] surfaceArray)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetProperty(out _priv_portUse, out _priv_pmsgUse, nPropertyID, surfaceArray);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPropertyID,
          RemoteSurface surface)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (surface != null)
                _priv_portUse.ValidateHandleOrNull(surface.RenderHandle);
            uint size = (uint)sizeof(RemoteEffect.Msg2_SetProperty);
            RemoteEffect.Msg2_SetProperty* msg2SetPropertyPtr = (RemoteEffect.Msg2_SetProperty*)_priv_portUse.AllocMessageBuffer(size);
            msg2SetPropertyPtr->_priv_size = size;
            msg2SetPropertyPtr->_priv_msgid = 2U;
            msg2SetPropertyPtr->nPropertyID = nPropertyID;
            msg2SetPropertyPtr->surface = surface != null ? surface.RenderHandle : RENDERHANDLE.NULL;
            msg2SetPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2SetPropertyPtr, ref s_priv_ByteOrder_Msg2_SetProperty, typeof(RemoteEffect.Msg2_SetProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg2SetPropertyPtr;
        }

        public unsafe void SendSetProperty(int nPropertyID, RemoteSurface surface)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetProperty(out _priv_portUse, out _priv_pmsgUse, nPropertyID, surface);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPropertyID,
          Vector4 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteEffect.Msg3_SetProperty);
            RemoteEffect.Msg3_SetProperty* msg3SetPropertyPtr = (RemoteEffect.Msg3_SetProperty*)_priv_portUse.AllocMessageBuffer(size);
            msg3SetPropertyPtr->_priv_size = size;
            msg3SetPropertyPtr->_priv_msgid = 3U;
            msg3SetPropertyPtr->nPropertyID = nPropertyID;
            msg3SetPropertyPtr->vValue = vValue;
            msg3SetPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3SetPropertyPtr, ref s_priv_ByteOrder_Msg3_SetProperty, typeof(RemoteEffect.Msg3_SetProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg3SetPropertyPtr;
        }

        public unsafe void SendSetProperty(int nPropertyID, Vector4 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetProperty(out _priv_portUse, out _priv_pmsgUse, nPropertyID, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPropertyID,
          Vector3 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteEffect.Msg4_SetProperty);
            RemoteEffect.Msg4_SetProperty* msg4SetPropertyPtr = (RemoteEffect.Msg4_SetProperty*)_priv_portUse.AllocMessageBuffer(size);
            msg4SetPropertyPtr->_priv_size = size;
            msg4SetPropertyPtr->_priv_msgid = 4U;
            msg4SetPropertyPtr->nPropertyID = nPropertyID;
            msg4SetPropertyPtr->vValue = vValue;
            msg4SetPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4SetPropertyPtr, ref s_priv_ByteOrder_Msg4_SetProperty, typeof(RemoteEffect.Msg4_SetProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg4SetPropertyPtr;
        }

        public unsafe void SendSetProperty(int nPropertyID, Vector3 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetProperty(out _priv_portUse, out _priv_pmsgUse, nPropertyID, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPropertyID,
          Vector2 vValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteEffect.Msg5_SetProperty);
            RemoteEffect.Msg5_SetProperty* msg5SetPropertyPtr = (RemoteEffect.Msg5_SetProperty*)_priv_portUse.AllocMessageBuffer(size);
            msg5SetPropertyPtr->_priv_size = size;
            msg5SetPropertyPtr->_priv_msgid = 5U;
            msg5SetPropertyPtr->nPropertyID = nPropertyID;
            msg5SetPropertyPtr->vValue = vValue;
            msg5SetPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg5SetPropertyPtr, ref s_priv_ByteOrder_Msg5_SetProperty, typeof(RemoteEffect.Msg5_SetProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg5SetPropertyPtr;
        }

        public unsafe void SendSetProperty(int nPropertyID, Vector2 vValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetProperty(out _priv_portUse, out _priv_pmsgUse, nPropertyID, vValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPropertyID,
          float flValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteEffect.Msg6_SetProperty);
            RemoteEffect.Msg6_SetProperty* msg6SetPropertyPtr = (RemoteEffect.Msg6_SetProperty*)_priv_portUse.AllocMessageBuffer(size);
            msg6SetPropertyPtr->_priv_size = size;
            msg6SetPropertyPtr->_priv_msgid = 6U;
            msg6SetPropertyPtr->nPropertyID = nPropertyID;
            msg6SetPropertyPtr->flValue = flValue;
            msg6SetPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg6SetPropertyPtr, ref s_priv_ByteOrder_Msg6_SetProperty, typeof(RemoteEffect.Msg6_SetProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg6SetPropertyPtr;
        }

        public unsafe void SendSetProperty(int nPropertyID, float flValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetProperty(out _priv_portUse, out _priv_pmsgUse, nPropertyID, flValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetProperty(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nPropertyID,
          int nValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteEffect.Msg7_SetProperty);
            RemoteEffect.Msg7_SetProperty* msg7SetPropertyPtr = (RemoteEffect.Msg7_SetProperty*)_priv_portUse.AllocMessageBuffer(size);
            msg7SetPropertyPtr->_priv_size = size;
            msg7SetPropertyPtr->_priv_msgid = 7U;
            msg7SetPropertyPtr->nPropertyID = nPropertyID;
            msg7SetPropertyPtr->nValue = nValue;
            msg7SetPropertyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg7SetPropertyPtr, ref s_priv_ByteOrder_Msg7_SetProperty, typeof(RemoteEffect.Msg7_SetProperty), 0, 0);
            _priv_pmsgUse = (Message*)msg7SetPropertyPtr;
        }

        public unsafe void SendSetProperty(int nPropertyID, int nValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetProperty(out _priv_portUse, out _priv_pmsgUse, nPropertyID, nValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteEffect CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteEffect(port, handle, true);

        public static RemoteEffect CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteEffect(port, handle, false);
        }

        protected RemoteEffect(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteEffect(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteEffect && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_SetDebugID
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stDebugID;
        }

        [ComVisible(false)]
        private struct Msg1_SetProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPropertyID;
            public BLOBREF surfaceArray;
        }

        [ComVisible(false)]
        private struct Msg2_SetProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPropertyID;
            public RENDERHANDLE surface;
        }

        [ComVisible(false)]
        private struct Msg3_SetProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPropertyID;
            public Vector4 vValue;
        }

        [ComVisible(false)]
        private struct Msg4_SetProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPropertyID;
            public Vector3 vValue;
        }

        [ComVisible(false)]
        private struct Msg5_SetProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPropertyID;
            public Vector2 vValue;
        }

        [ComVisible(false)]
        private struct Msg6_SetProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPropertyID;
            public float flValue;
        }

        [ComVisible(false)]
        private struct Msg7_SetProperty
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nPropertyID;
            public int nValue;
        }
    }
}
