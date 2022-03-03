// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt.RemoteFormWindow
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt
{
    internal class RemoteFormWindow : RemoteFormWindowBase
    {
        private static ushort[] s_priv_ByteOrder_Msg41_Create;

        protected RemoteFormWindow()
        {
        }

        public static unsafe RemoteFormWindow Create(
          ProtocolSplashDesktopNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          RemoteDesktopManager manager,
          LocalFormWindowCallback cb)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE windowClassHandle = _priv_protocolInstance.FormWindow_ClassHandle;
            RemoteFormWindow remoteFormWindow = new RemoteFormWindow(port, _priv_owner);
            uint num = (uint)sizeof(RemoteFormWindow.Msg41_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteFormWindow.Msg41_Create* msg41CreatePtr = (RemoteFormWindow.Msg41_Create*)pMem;
            msg41CreatePtr->_priv_size = num;
            msg41CreatePtr->_priv_msgid = 41U;
            msg41CreatePtr->manager = manager != null ? manager.RenderHandle : RENDERHANDLE.NULL;
            msg41CreatePtr->_priv_objcb = cb.RenderHandle;
            msg41CreatePtr->_priv_ctxcb = port.Session.LocalContext;
            msg41CreatePtr->_priv_idObjectSubject = remoteFormWindow.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg41_Create, typeof(RemoteFormWindow.Msg41_Create), 0, 0);
            port.CreateRemoteObject(windowClassHandle, remoteFormWindow.m_renderHandle, (Message*)msg41CreatePtr);
            return remoteFormWindow;
        }

        public static RemoteFormWindow CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteFormWindow(port, handle, true);
        }

        public static RemoteFormWindow CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteFormWindow(port, handle, false);
        }

        protected RemoteFormWindow(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteFormWindow(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteFormWindow && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg41_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE manager;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
        }
    }
}
