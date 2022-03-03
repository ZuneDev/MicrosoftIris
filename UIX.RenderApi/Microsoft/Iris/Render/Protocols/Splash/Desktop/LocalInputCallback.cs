// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.LocalInputCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop
{
    internal class LocalInputCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnInput;

        protected LocalInputCallback()
        {
        }

        internal static LocalInputCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalInputCallback(port, callbackInstance);
        }

        protected LocalInputCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalInputCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is IInputCallback _priv_target) || message->nMsg != 0U)
                return;
            Dispatch_OnInput(port, _priv_target, (LocalInputCallback.Msg0_OnInput*)message);
        }

        private static unsafe void Dispatch_OnInput(
          RenderPort _priv_port,
          IInputCallback _priv_target,
          LocalInputCallback.Msg0_OnInput* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnInput, typeof(LocalInputCallback.Msg0_OnInput), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            Message* pmsgInner;
            MarshalHelper.Decode(_priv_port, (Message*)_priv_pmsg, _priv_pmsg->inputInfo, out pmsgInner);
            _priv_target.OnInput(target, pmsgInner);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnInput
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public BLOBREF inputInfo;
        }
    }
}
