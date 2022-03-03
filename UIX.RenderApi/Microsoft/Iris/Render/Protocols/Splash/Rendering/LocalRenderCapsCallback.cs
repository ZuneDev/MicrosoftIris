// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.LocalRenderCapsCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class LocalRenderCapsCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnSoundCaps;
        private static ushort[] s_priv_ByteOrder_Msg1_OnGraphicsCaps;
        private static ushort[] s_priv_ByteOrder_Msg2_OnEndCapsCheck;
        private static ushort[] s_priv_ByteOrder_Msg3_OnBeginCapsCheck;

        protected LocalRenderCapsCallback()
        {
        }

        internal static LocalRenderCapsCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalRenderCapsCallback(port, callbackInstance);
        }

        protected LocalRenderCapsCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalRenderCapsCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is IRenderCapsCallback _priv_target))
                return;
            switch (message->nMsg)
            {
                case 0:
                    Dispatch_OnSoundCaps(port, _priv_target, (LocalRenderCapsCallback.Msg0_OnSoundCaps*)message);
                    break;
                case 1:
                    Dispatch_OnGraphicsCaps(port, _priv_target, (LocalRenderCapsCallback.Msg1_OnGraphicsCaps*)message);
                    break;
                case 2:
                    Dispatch_OnEndCapsCheck(port, _priv_target, (LocalRenderCapsCallback.Msg2_OnEndCapsCheck*)message);
                    break;
                case 3:
                    Dispatch_OnBeginCapsCheck(port, _priv_target, (LocalRenderCapsCallback.Msg3_OnBeginCapsCheck*)message);
                    break;
            }
        }

        private static unsafe void Dispatch_OnSoundCaps(
          RenderPort _priv_port,
          IRenderCapsCallback _priv_target,
          LocalRenderCapsCallback.Msg0_OnSoundCaps* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnSoundCaps, typeof(LocalRenderCapsCallback.Msg0_OnSoundCaps), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            uint nCookie = _priv_pmsg->nCookie;
            Microsoft.Iris.Render.Internal.SoundCaps capsInfo = _priv_pmsg->capsInfo;
            _priv_target.OnSoundCaps(target, nCookie, capsInfo);
        }

        private static unsafe void Dispatch_OnGraphicsCaps(
          RenderPort _priv_port,
          IRenderCapsCallback _priv_target,
          LocalRenderCapsCallback.Msg1_OnGraphicsCaps* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg1_OnGraphicsCaps, typeof(LocalRenderCapsCallback.Msg1_OnGraphicsCaps), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            uint nCookie = _priv_pmsg->nCookie;
            GraphicsCaps capsInfo = _priv_pmsg->capsInfo;
            _priv_target.OnGraphicsCaps(target, nCookie, capsInfo);
        }

        private static unsafe void Dispatch_OnEndCapsCheck(
          RenderPort _priv_port,
          IRenderCapsCallback _priv_target,
          LocalRenderCapsCallback.Msg2_OnEndCapsCheck* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg2_OnEndCapsCheck, typeof(LocalRenderCapsCallback.Msg2_OnEndCapsCheck), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            uint nCookie = _priv_pmsg->nCookie;
            _priv_target.OnEndCapsCheck(target, nCookie);
        }

        private static unsafe void Dispatch_OnBeginCapsCheck(
          RenderPort _priv_port,
          IRenderCapsCallback _priv_target,
          LocalRenderCapsCallback.Msg3_OnBeginCapsCheck* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg3_OnBeginCapsCheck, typeof(LocalRenderCapsCallback.Msg3_OnBeginCapsCheck), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            uint nCookie = _priv_pmsg->nCookie;
            _priv_target.OnBeginCapsCheck(target, nCookie);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnSoundCaps
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint nCookie;
            public Microsoft.Iris.Render.Internal.SoundCaps capsInfo;
        }

        [ComVisible(false)]
        private struct Msg1_OnGraphicsCaps
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint nCookie;
            public GraphicsCaps capsInfo;
        }

        [ComVisible(false)]
        private struct Msg2_OnEndCapsCheck
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint nCookie;
        }

        [ComVisible(false)]
        private struct Msg3_OnBeginCapsCheck
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint nCookie;
        }
    }
}
