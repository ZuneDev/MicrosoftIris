// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Messaging.LocalDataBufferCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Messaging
{
    internal class LocalDataBufferCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnComplete;

        protected LocalDataBufferCallback()
        {
        }

        internal static LocalDataBufferCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalDataBufferCallback(port, callbackInstance);
        }

        protected LocalDataBufferCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalDataBufferCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is IDataBufferCallback _priv_target) || message->nMsg != 0U)
                return;
            Dispatch_OnComplete(port, _priv_target, (LocalDataBufferCallback.Msg0_OnComplete*)message);
        }

        private static unsafe void Dispatch_OnComplete(
          RenderPort _priv_port,
          IDataBufferCallback _priv_target,
          LocalDataBufferCallback.Msg0_OnComplete* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnComplete, typeof(LocalDataBufferCallback.Msg0_OnComplete), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnComplete(target);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnComplete
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }
    }
}
