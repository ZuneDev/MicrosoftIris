// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteSoundBuffer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Messaging;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteSoundBuffer : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_LoadSoundData;

        protected RemoteSoundBuffer()
        {
        }

        public unsafe void BuildLoadSoundData(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteDataBuffer dataBuffer)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (dataBuffer != null)
                _priv_portUse.ValidateHandleOrNull(dataBuffer.RenderHandle);
            uint size = (uint)sizeof(RemoteSoundBuffer.Msg0_LoadSoundData);
            RemoteSoundBuffer.Msg0_LoadSoundData* msg0LoadSoundDataPtr = (RemoteSoundBuffer.Msg0_LoadSoundData*)_priv_portUse.AllocMessageBuffer(size);
            msg0LoadSoundDataPtr->_priv_size = size;
            msg0LoadSoundDataPtr->_priv_msgid = 0U;
            msg0LoadSoundDataPtr->dataBuffer = dataBuffer != null ? dataBuffer.RenderHandle : RENDERHANDLE.NULL;
            msg0LoadSoundDataPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0LoadSoundDataPtr, ref s_priv_ByteOrder_Msg0_LoadSoundData, typeof(RemoteSoundBuffer.Msg0_LoadSoundData), 0, 0);
            _priv_pmsgUse = (Message*)msg0LoadSoundDataPtr;
        }

        public unsafe void SendLoadSoundData(RemoteDataBuffer dataBuffer)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildLoadSoundData(out _priv_portUse, out _priv_pmsgUse, dataBuffer);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteSoundBuffer CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteSoundBuffer(port, handle, true);
        }

        public static RemoteSoundBuffer CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteSoundBuffer(port, handle, false);
        }

        protected RemoteSoundBuffer(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteSoundBuffer(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteSoundBuffer && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_LoadSoundData
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE dataBuffer;
        }
    }
}
