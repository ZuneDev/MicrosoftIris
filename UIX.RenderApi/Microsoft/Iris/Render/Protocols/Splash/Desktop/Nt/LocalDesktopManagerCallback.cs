// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt.LocalDesktopManagerCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt
{
    internal class LocalDesktopManagerCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnEndDisplayModes;
        private static ushort[] s_priv_ByteOrder_Msg1_OnDisplayMode;
        private static ushort[] s_priv_ByteOrder_Msg2_OnBeginDisplayModes;
        private static ushort[] s_priv_ByteOrder_Msg3_OnEndEnumMonitorInfo;
        private static ushort[] s_priv_ByteOrder_Msg4_OnMonitorInfo;
        private static ushort[] s_priv_ByteOrder_Msg5_OnBeginEnumMonitorInfo;

        protected LocalDesktopManagerCallback()
        {
        }

        internal static LocalDesktopManagerCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalDesktopManagerCallback(port, callbackInstance);
        }

        protected LocalDesktopManagerCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalDesktopManagerCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is IDesktopManagerCallback _priv_target))
                return;
            switch (message->nMsg)
            {
                case 0:
                    Dispatch_OnEndDisplayModes(port, _priv_target, (LocalDesktopManagerCallback.Msg0_OnEndDisplayModes*)message);
                    break;
                case 1:
                    Dispatch_OnDisplayMode(port, _priv_target, (LocalDesktopManagerCallback.Msg1_OnDisplayMode*)message);
                    break;
                case 2:
                    Dispatch_OnBeginDisplayModes(port, _priv_target, (LocalDesktopManagerCallback.Msg2_OnBeginDisplayModes*)message);
                    break;
                case 3:
                    Dispatch_OnEndEnumMonitorInfo(port, _priv_target, (LocalDesktopManagerCallback.Msg3_OnEndEnumMonitorInfo*)message);
                    break;
                case 4:
                    Dispatch_OnMonitorInfo(port, _priv_target, (LocalDesktopManagerCallback.Msg4_OnMonitorInfo*)message);
                    break;
                case 5:
                    Dispatch_OnBeginEnumMonitorInfo(port, _priv_target, (LocalDesktopManagerCallback.Msg5_OnBeginEnumMonitorInfo*)message);
                    break;
            }
        }

        private static unsafe void Dispatch_OnEndDisplayModes(
          RenderPort _priv_port,
          IDesktopManagerCallback _priv_target,
          LocalDesktopManagerCallback.Msg0_OnEndDisplayModes* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnEndDisplayModes, typeof(LocalDesktopManagerCallback.Msg0_OnEndDisplayModes), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnEndDisplayModes(target);
        }

        private static unsafe void Dispatch_OnDisplayMode(
          RenderPort _priv_port,
          IDesktopManagerCallback _priv_target,
          LocalDesktopManagerCallback.Msg1_OnDisplayMode* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg1_OnDisplayMode, typeof(LocalDesktopManagerCallback.Msg1_OnDisplayMode), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            RenderDisplayMode mode = _priv_pmsg->mode;
            bool fSupported = _priv_pmsg->fSupported != 0U;
            _priv_target.OnDisplayMode(target, mode, fSupported);
        }

        private static unsafe void Dispatch_OnBeginDisplayModes(
          RenderPort _priv_port,
          IDesktopManagerCallback _priv_target,
          LocalDesktopManagerCallback.Msg2_OnBeginDisplayModes* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg2_OnBeginDisplayModes, typeof(LocalDesktopManagerCallback.Msg2_OnBeginDisplayModes), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnBeginDisplayModes(target);
        }

        private static unsafe void Dispatch_OnEndEnumMonitorInfo(
          RenderPort _priv_port,
          IDesktopManagerCallback _priv_target,
          LocalDesktopManagerCallback.Msg3_OnEndEnumMonitorInfo* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg3_OnEndEnumMonitorInfo, typeof(LocalDesktopManagerCallback.Msg3_OnEndEnumMonitorInfo), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnEndEnumMonitorInfo(target);
        }

        private static unsafe void Dispatch_OnMonitorInfo(
          RenderPort _priv_port,
          IDesktopManagerCallback _priv_target,
          LocalDesktopManagerCallback.Msg4_OnMonitorInfo* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg4_OnMonitorInfo, typeof(LocalDesktopManagerCallback.Msg4_OnMonitorInfo), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            Message* pmsgInner;
            MarshalHelper.Decode(_priv_port, (Message*)_priv_pmsg, _priv_pmsg->info, out pmsgInner);
            _priv_target.OnMonitorInfo(target, pmsgInner);
        }

        private static unsafe void Dispatch_OnBeginEnumMonitorInfo(
          RenderPort _priv_port,
          IDesktopManagerCallback _priv_target,
          LocalDesktopManagerCallback.Msg5_OnBeginEnumMonitorInfo* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg5_OnBeginEnumMonitorInfo, typeof(LocalDesktopManagerCallback.Msg5_OnBeginEnumMonitorInfo), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnBeginEnumMonitorInfo(target);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnEndDisplayModes
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }

        [ComVisible(false)]
        private struct Msg1_OnDisplayMode
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public RenderDisplayMode mode;
            public uint fSupported;
        }

        [ComVisible(false)]
        private struct Msg2_OnBeginDisplayModes
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }

        [ComVisible(false)]
        private struct Msg3_OnEndEnumMonitorInfo
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }

        [ComVisible(false)]
        private struct Msg4_OnMonitorInfo
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public BLOBREF info;
        }

        [ComVisible(false)]
        private struct Msg5_OnBeginEnumMonitorInfo
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }
    }
}
