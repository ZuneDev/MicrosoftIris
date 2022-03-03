// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.LocalFormWindowCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop
{
    internal class LocalFormWindowCallback : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_OnRendererSuspended;
        private static ushort[] s_priv_ByteOrder_Msg1_OnNativeScreensave;
        private static ushort[] s_priv_ByteOrder_Msg2_OnShellShutdownHook;
        private static ushort[] s_priv_ByteOrder_Msg4_OnDropComplete;
        private static ushort[] s_priv_ByteOrder_Msg5_OnPartialDrop;
        private static ushort[] s_priv_ByteOrder_Msg6_OnStateChange;
        private static ushort[] s_priv_ByteOrder_Msg7_OnTerminalSessionChange;
        private static ushort[] s_priv_ByteOrder_Msg8_OnPrivateSysCommand;
        private static ushort[] s_priv_ByteOrder_Msg9_OnMouseIdle;
        private static ushort[] s_priv_ByteOrder_Msg10_OnCloseRequested;
        private static ushort[] s_priv_ByteOrder_Msg11_OnLoad;
        private static ushort[] s_priv_ByteOrder_Msg12_OnWindowDestroyed;

        protected LocalFormWindowCallback()
        {
        }

        internal static LocalFormWindowCallback CreateCallbackImpl(
          RenderPort port,
          RENDERHANDLE callbackInstance)
        {
            return new LocalFormWindowCallback(port, callbackInstance);
        }

        protected LocalFormWindowCallback(RenderPort port, RENDERHANDLE handle)
          : base(port, handle, false)
        {
        }

        public override bool Equals(object other) => other is LocalFormWindowCallback && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        internal static unsafe RENDERHANDLE BindCallback(RenderPort port) => port.RegisterCallback(new PortCallback(DispatchCallback), out uint _);

        private static unsafe void DispatchCallback(
          RenderPort port,
          IRenderHandleOwner owner,
          CallbackMessage* message)
        {
            if (!(owner is IFormWindowCallback _priv_target))
                return;
            switch (message->nMsg)
            {
                case 0:
                    Dispatch_OnRendererSuspended(port, _priv_target, (LocalFormWindowCallback.Msg0_OnRendererSuspended*)message);
                    break;
                case 1:
                    Dispatch_OnNativeScreensave(port, _priv_target, (LocalFormWindowCallback.Msg1_OnNativeScreensave*)message);
                    break;
                case 2:
                    Dispatch_OnShellShutdownHook(port, _priv_target, (LocalFormWindowCallback.Msg2_OnShellShutdownHook*)message);
                    break;
                case 3:
                    Dispatch_OnSetFocus(port, _priv_target, (LocalFormWindowCallback.Msg3_OnSetFocus*)message);
                    break;
                case 4:
                    Dispatch_OnDropComplete(port, _priv_target, (LocalFormWindowCallback.Msg4_OnDropComplete*)message);
                    break;
                case 5:
                    Dispatch_OnPartialDrop(port, _priv_target, (LocalFormWindowCallback.Msg5_OnPartialDrop*)message);
                    break;
                case 6:
                    Dispatch_OnStateChange(port, _priv_target, (LocalFormWindowCallback.Msg6_OnStateChange*)message);
                    break;
                case 7:
                    Dispatch_OnTerminalSessionChange(port, _priv_target, (LocalFormWindowCallback.Msg7_OnTerminalSessionChange*)message);
                    break;
                case 8:
                    Dispatch_OnPrivateSysCommand(port, _priv_target, (LocalFormWindowCallback.Msg8_OnPrivateSysCommand*)message);
                    break;
                case 9:
                    Dispatch_OnMouseIdle(port, _priv_target, (LocalFormWindowCallback.Msg9_OnMouseIdle*)message);
                    break;
                case 10:
                    Dispatch_OnCloseRequested(port, _priv_target, (LocalFormWindowCallback.Msg10_OnCloseRequested*)message);
                    break;
                case 11:
                    Dispatch_OnLoad(port, _priv_target, (LocalFormWindowCallback.Msg11_OnLoad*)message);
                    break;
                case 12:
                    Dispatch_OnWindowDestroyed(port, _priv_target, (LocalFormWindowCallback.Msg12_OnWindowDestroyed*)message);
                    break;
                case 13:
                    Dispatch_OnWindowCreated(port, _priv_target, (LocalFormWindowCallback.Msg13_OnWindowCreated*)message);
                    break;
            }
        }

        private static unsafe void Dispatch_OnRendererSuspended(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg0_OnRendererSuspended* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg0_OnRendererSuspended, typeof(LocalFormWindowCallback.Msg0_OnRendererSuspended), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            bool fEnabled = _priv_pmsg->fEnabled != 0U;
            _priv_target.OnRendererSuspended(target, fEnabled);
        }

        private static unsafe void Dispatch_OnNativeScreensave(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg1_OnNativeScreensave* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg1_OnNativeScreensave, typeof(LocalFormWindowCallback.Msg1_OnNativeScreensave), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            bool fStartScreensave = _priv_pmsg->fStartScreensave != 0U;
            _priv_target.OnNativeScreensave(target, fStartScreensave);
        }

        private static unsafe void Dispatch_OnShellShutdownHook(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg2_OnShellShutdownHook* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg2_OnShellShutdownHook, typeof(LocalFormWindowCallback.Msg2_OnShellShutdownHook), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            ushort uIdMsg = _priv_pmsg->uIdMsg;
            _priv_target.OnShellShutdownHook(target, uIdMsg);
        }

        private static unsafe void Dispatch_OnSetFocus(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg3_OnSetFocus* _priv_pmsg)
        {
            RENDERHANDLE target = _priv_pmsg->target;
            bool focused = _priv_pmsg->focused != 0U;
            HWND hwndFocusChange = _priv_pmsg->hwndFocusChange;
            _priv_target.OnSetFocus(target, focused, hwndFocusChange);
        }

        private static unsafe void Dispatch_OnDropComplete(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg4_OnDropComplete* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg4_OnDropComplete, typeof(LocalFormWindowCallback.Msg4_OnDropComplete), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnDropComplete(target);
        }

        private static unsafe void Dispatch_OnPartialDrop(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg5_OnPartialDrop* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg5_OnPartialDrop, typeof(LocalFormWindowCallback.Msg5_OnPartialDrop), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            string file;
            MarshalHelper.Decode(_priv_port, (Message*)_priv_pmsg, _priv_pmsg->file, out file);
            _priv_target.OnPartialDrop(target, file);
        }

        private static unsafe void Dispatch_OnStateChange(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg6_OnStateChange* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg6_OnStateChange, typeof(LocalFormWindowCallback.Msg6_OnStateChange), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            Message* pmsgInner;
            MarshalHelper.Decode(_priv_port, (Message*)_priv_pmsg, _priv_pmsg->stateInfo, out pmsgInner);
            _priv_target.OnStateChange(target, pmsgInner);
        }

        private static unsafe void Dispatch_OnTerminalSessionChange(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg7_OnTerminalSessionChange* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg7_OnTerminalSessionChange, typeof(LocalFormWindowCallback.Msg7_OnTerminalSessionChange), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            IntPtr wParam = _priv_pmsg->wParam;
            IntPtr lParam = _priv_pmsg->lParam;
            _priv_target.OnTerminalSessionChange(target, wParam, lParam);
        }

        private static unsafe void Dispatch_OnPrivateSysCommand(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg8_OnPrivateSysCommand* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg8_OnPrivateSysCommand, typeof(LocalFormWindowCallback.Msg8_OnPrivateSysCommand), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            IntPtr wParam = _priv_pmsg->wParam;
            IntPtr lParam = _priv_pmsg->lParam;
            _priv_target.OnPrivateSysCommand(target, wParam, lParam);
        }

        private static unsafe void Dispatch_OnMouseIdle(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg9_OnMouseIdle* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg9_OnMouseIdle, typeof(LocalFormWindowCallback.Msg9_OnMouseIdle), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            bool fNewIdle = _priv_pmsg->fNewIdle != 0U;
            _priv_target.OnMouseIdle(target, fNewIdle);
        }

        private static unsafe void Dispatch_OnCloseRequested(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg10_OnCloseRequested* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg10_OnCloseRequested, typeof(LocalFormWindowCallback.Msg10_OnCloseRequested), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnCloseRequested(target);
        }

        private static unsafe void Dispatch_OnLoad(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg11_OnLoad* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg11_OnLoad, typeof(LocalFormWindowCallback.Msg11_OnLoad), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            _priv_target.OnLoad(target);
        }

        private static unsafe void Dispatch_OnWindowDestroyed(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg12_OnWindowDestroyed* _priv_pmsg)
        {
            if (_priv_port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)_priv_pmsg, ref s_priv_ByteOrder_Msg12_OnWindowDestroyed, typeof(LocalFormWindowCallback.Msg12_OnWindowDestroyed), sizeof(CallbackMessage), 0);
            RENDERHANDLE target = _priv_pmsg->target;
            uint nFinalShow = _priv_pmsg->nFinalShow;
            Rectangle rcLastPosition = _priv_pmsg->rcLastPosition;
            Point finalMaxPosition = _priv_pmsg->ptFinalMaxPosition;
            _priv_target.OnWindowDestroyed(target, nFinalShow, rcLastPosition, finalMaxPosition);
        }

        private static unsafe void Dispatch_OnWindowCreated(
          RenderPort _priv_port,
          IFormWindowCallback _priv_target,
          LocalFormWindowCallback.Msg13_OnWindowCreated* _priv_pmsg)
        {
            RENDERHANDLE target = _priv_pmsg->target;
            HWND hWnd = _priv_pmsg->m_hWnd;
            _priv_target.OnWindowCreated(target, hWnd);
        }

        [Conditional("DEBUG")]
        private static unsafe void OnInvalidCallback(CallbackMessage* message, string category)
        {
        }

        [ComVisible(false)]
        private struct Msg0_OnRendererSuspended
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint fEnabled;
        }

        [ComVisible(false)]
        private struct Msg1_OnNativeScreensave
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint fStartScreensave;
        }

        [ComVisible(false)]
        private struct Msg2_OnShellShutdownHook
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public ushort uIdMsg;
        }

        [ComVisible(false)]
        private struct Msg3_OnSetFocus
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint focused;
            public HWND hwndFocusChange;
        }

        [ComVisible(false)]
        private struct Msg4_OnDropComplete
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }

        [ComVisible(false)]
        private struct Msg5_OnPartialDrop
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public BLOBREF file;
        }

        [ComVisible(false)]
        private struct Msg6_OnStateChange
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public BLOBREF stateInfo;
        }

        [ComVisible(false)]
        private struct Msg7_OnTerminalSessionChange
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public IntPtr wParam;
            public IntPtr lParam;
        }

        [ComVisible(false)]
        private struct Msg8_OnPrivateSysCommand
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public IntPtr wParam;
            public IntPtr lParam;
        }

        [ComVisible(false)]
        private struct Msg9_OnMouseIdle
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint fNewIdle;
        }

        [ComVisible(false)]
        private struct Msg10_OnCloseRequested
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }

        [ComVisible(false)]
        private struct Msg11_OnLoad
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
        }

        [ComVisible(false)]
        private struct Msg12_OnWindowDestroyed
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public uint nFinalShow;
            public Rectangle rcLastPosition;
            public Point ptFinalMaxPosition;
        }

        [ComVisible(false)]
        private struct Msg13_OnWindowCreated
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE target;
            public HWND m_hWnd;
        }
    }
}
