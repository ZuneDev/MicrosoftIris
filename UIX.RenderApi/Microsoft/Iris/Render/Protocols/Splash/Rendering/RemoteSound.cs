// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteSound
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteSound : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_Stop;
        private static ushort[] s_priv_ByteOrder_Msg1_Play;

        protected RemoteSound()
        {
        }

        public unsafe void BuildStop(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSound.Msg0_Stop);
            RemoteSound.Msg0_Stop* msg0StopPtr = (RemoteSound.Msg0_Stop*)_priv_portUse.AllocMessageBuffer(size);
            msg0StopPtr->_priv_size = size;
            msg0StopPtr->_priv_msgid = 0U;
            msg0StopPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0StopPtr, ref s_priv_ByteOrder_Msg0_Stop, typeof(RemoteSound.Msg0_Stop), 0, 0);
            _priv_pmsgUse = (Message*)msg0StopPtr;
        }

        public unsafe void SendStop()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildStop(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildPlay(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSound.Msg1_Play);
            RemoteSound.Msg1_Play* msg1PlayPtr = (RemoteSound.Msg1_Play*)_priv_portUse.AllocMessageBuffer(size);
            msg1PlayPtr->_priv_size = size;
            msg1PlayPtr->_priv_msgid = 1U;
            msg1PlayPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1PlayPtr, ref s_priv_ByteOrder_Msg1_Play, typeof(RemoteSound.Msg1_Play), 0, 0);
            _priv_pmsgUse = (Message*)msg1PlayPtr;
        }

        public unsafe void SendPlay()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildPlay(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteSound CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteSound(port, handle, true);

        public static RemoteSound CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteSound(port, handle, false);
        }

        protected RemoteSound(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteSound(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteSound && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_Stop
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg1_Play
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }
    }
}
