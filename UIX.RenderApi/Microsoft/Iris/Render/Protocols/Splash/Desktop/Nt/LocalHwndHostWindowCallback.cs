// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt.LocalHwndHostWindowCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt
{
    internal class LocalHwndHostWindowCallback : RemoteObject
    {
        protected LocalHwndHostWindowCallback()
        {
        }

        internal static LocalHwndHostWindowCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalHwndHostWindowCallback(port, callbackInstance);
        }

        protected LocalHwndHostWindowCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalHwndHostWindowCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is IHwndHostWindowCallback _priv_target) || message->nMsg != 0U)
                return;
            Dispatch_OnHandleChanged(port, _priv_target, (LocalHwndHostWindowCallback.Msg0_OnHandleChanged*)message);
        }

        private static unsafe void Dispatch_OnHandleChanged(
          RenderPort _priv_port,
          IHwndHostWindowCallback _priv_target,
          LocalHwndHostWindowCallback.Msg0_OnHandleChanged* _priv_pmsg)
        {
            RENDERHANDLE target = _priv_pmsg->target;
            HWND hWnd = _priv_pmsg->m_hWnd;
            _priv_target.OnHandleChanged(target, hWnd);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnHandleChanged
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public HWND m_hWnd;
        }
    }
}
