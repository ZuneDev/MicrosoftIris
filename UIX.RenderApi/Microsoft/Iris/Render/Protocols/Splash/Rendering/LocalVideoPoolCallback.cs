// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.LocalVideoPoolCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class LocalVideoPoolCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnInvalidate;
        private static ushort[] s_priv_ByteOrder_Msg1_OnInputChanged;

        protected LocalVideoPoolCallback()
        {
        }

        internal static LocalVideoPoolCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalVideoPoolCallback(port, callbackInstance);
        }

        protected LocalVideoPoolCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalVideoPoolCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (owner is not IVideoPoolCallback _priv_target)
                return;
            switch (message->nMsg)
            {
                case 0:
                    Dispatch_OnInvalidate(port, _priv_target, (LocalVideoPoolCallback.Msg0_OnInvalidate*)message);
                    break;
                case 1:
                    Dispatch_OnInputChanged(port, _priv_target, (LocalVideoPoolCallback.Msg1_OnInputChanged*)message);
                    break;
            }
        }

        private static unsafe void Dispatch_OnInvalidate(
          RenderPort _priv_port,
          IVideoPoolCallback _priv_target,
          LocalVideoPoolCallback.Msg0_OnInvalidate* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnInvalidate, typeof(LocalVideoPoolCallback.Msg0_OnInvalidate), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnInvalidate(target);
        }

        private static unsafe void Dispatch_OnInputChanged(
          RenderPort _priv_port,
          IVideoPoolCallback _priv_target,
          LocalVideoPoolCallback.Msg1_OnInputChanged* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg1_OnInputChanged, typeof(LocalVideoPoolCallback.Msg1_OnInputChanged), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            Size sizeTargetPxl = _priv_pmsg->sizeTargetPxl;
            Size sizeAspectRatioPxl = _priv_pmsg->sizeAspectRatioPxl;
            uint frameRateNumerator = _priv_pmsg->nFrameRateNumerator;
            uint frameRateDenominator = _priv_pmsg->nFrameRateDenominator;
            SurfaceFormat nFormat = _priv_pmsg->nFormat;
            _priv_target.OnInputChanged(target, sizeTargetPxl, sizeAspectRatioPxl, frameRateNumerator, frameRateDenominator, nFormat);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnInvalidate
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }

        [ComVisible(false)]
        private struct Msg1_OnInputChanged
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public Size sizeTargetPxl;
            public Size sizeAspectRatioPxl;
            public uint nFrameRateNumerator;
            public uint nFrameRateDenominator;
            public SurfaceFormat nFormat;
        }
    }
}
