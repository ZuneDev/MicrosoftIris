// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.LocalSoundBufferCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class LocalSoundBufferCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnSoundBufferReady;
        private static ushort[] s_priv_ByteOrder_Msg1_OnSoundBufferLost;

        protected LocalSoundBufferCallback()
        {
        }

        internal static LocalSoundBufferCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalSoundBufferCallback(port, callbackInstance);
        }

        protected LocalSoundBufferCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalSoundBufferCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is ISoundBufferCallback _priv_target))
                return;
            switch (message->nMsg)
            {
                case 0:
                    Dispatch_OnSoundBufferReady(port, _priv_target, (LocalSoundBufferCallback.Msg0_OnSoundBufferReady*)message);
                    break;
                case 1:
                    Dispatch_OnSoundBufferLost(port, _priv_target, (LocalSoundBufferCallback.Msg1_OnSoundBufferLost*)message);
                    break;
            }
        }

        private static unsafe void Dispatch_OnSoundBufferReady(
          RenderPort _priv_port,
          ISoundBufferCallback _priv_target,
          LocalSoundBufferCallback.Msg0_OnSoundBufferReady* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnSoundBufferReady, typeof(LocalSoundBufferCallback.Msg0_OnSoundBufferReady), sizeof(CallbackMessage), 0);
            RENDERHANDLE idTarget = _priv_pmsg->idTarget;
            _priv_target.OnSoundBufferReady(idTarget);
        }

        private static unsafe void Dispatch_OnSoundBufferLost(
          RenderPort _priv_port,
          ISoundBufferCallback _priv_target,
          LocalSoundBufferCallback.Msg1_OnSoundBufferLost* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg1_OnSoundBufferLost, typeof(LocalSoundBufferCallback.Msg1_OnSoundBufferLost), sizeof(CallbackMessage), 0);
            RENDERHANDLE idTarget = _priv_pmsg->idTarget;
            _priv_target.OnSoundBufferLost(idTarget);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnSoundBufferReady
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idTarget;
        }

        [ComVisible(false)]
        private struct Msg1_OnSoundBufferLost
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idTarget;
        }
    }
}
