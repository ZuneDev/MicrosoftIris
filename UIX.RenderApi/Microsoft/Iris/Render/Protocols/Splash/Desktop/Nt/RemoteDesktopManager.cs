// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt.RemoteDesktopManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt
{
    internal class RemoteDesktopManager : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg2_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_RebuildMonitorCache;
        private static ushort[] s_priv_ByteOrder_Msg1_ChangeFullScreenMode;

        protected RemoteDesktopManager()
        {
        }

        public static unsafe RemoteDesktopManager Create(
          ProtocolSplashDesktopNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          LocalDesktopManagerCallback cb,
          bool fEnumDisplayModes)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE managerClassHandle = _priv_protocolInstance.DesktopManager_ClassHandle;
            RemoteDesktopManager remoteDesktopManager = new RemoteDesktopManager(port, _priv_owner);
            uint num = (uint)sizeof(RemoteDesktopManager.Msg2_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteDesktopManager.Msg2_Create* msg2CreatePtr = (RemoteDesktopManager.Msg2_Create*)pMem;
            msg2CreatePtr->_priv_size = num;
            msg2CreatePtr->_priv_msgid = 2U;
            msg2CreatePtr->_priv_objcb = cb.RenderHandle;
            msg2CreatePtr->_priv_ctxcb = port.Session.LocalContext;
            msg2CreatePtr->fEnumDisplayModes = fEnumDisplayModes ? uint.MaxValue : 0U;
            msg2CreatePtr->_priv_idObjectSubject = remoteDesktopManager.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg2_Create, typeof(RemoteDesktopManager.Msg2_Create), 0, 0);
            port.CreateRemoteObject(managerClassHandle, remoteDesktopManager.m_renderHandle, (Message*)msg2CreatePtr);
            return remoteDesktopManager;
        }

        public unsafe void BuildRebuildMonitorCache(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDesktopManager.Msg0_RebuildMonitorCache);
            RemoteDesktopManager.Msg0_RebuildMonitorCache* rebuildMonitorCachePtr = (RemoteDesktopManager.Msg0_RebuildMonitorCache*)_priv_portUse.AllocMessageBuffer(size);
            rebuildMonitorCachePtr->_priv_size = size;
            rebuildMonitorCachePtr->_priv_msgid = 0U;
            rebuildMonitorCachePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)rebuildMonitorCachePtr, ref s_priv_ByteOrder_Msg0_RebuildMonitorCache, typeof(RemoteDesktopManager.Msg0_RebuildMonitorCache), 0, 0);
            _priv_pmsgUse = (Message*)rebuildMonitorCachePtr;
        }

        public unsafe void SendRebuildMonitorCache()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRebuildMonitorCache(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildChangeFullScreenMode(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint idDisplay,
          RenderDisplayMode mode,
          bool fTvMode)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteDesktopManager.Msg1_ChangeFullScreenMode);
            RemoteDesktopManager.Msg1_ChangeFullScreenMode* changeFullScreenModePtr = (RemoteDesktopManager.Msg1_ChangeFullScreenMode*)_priv_portUse.AllocMessageBuffer(size);
            changeFullScreenModePtr->_priv_size = size;
            changeFullScreenModePtr->_priv_msgid = 1U;
            changeFullScreenModePtr->idDisplay = idDisplay;
            changeFullScreenModePtr->mode = mode;
            changeFullScreenModePtr->fTvMode = fTvMode ? uint.MaxValue : 0U;
            changeFullScreenModePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)changeFullScreenModePtr, ref s_priv_ByteOrder_Msg1_ChangeFullScreenMode, typeof(RemoteDesktopManager.Msg1_ChangeFullScreenMode), 0, 0);
            _priv_pmsgUse = (Message*)changeFullScreenModePtr;
        }

        public unsafe void SendChangeFullScreenMode(
          uint idDisplay,
          RenderDisplayMode mode,
          bool fTvMode)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildChangeFullScreenMode(out _priv_portUse, out _priv_pmsgUse, idDisplay, mode, fTvMode);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteDesktopManager CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteDesktopManager(port, handle, true);
        }

        public static RemoteDesktopManager CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDesktopManager(port, handle, false);
        }

        protected RemoteDesktopManager(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDesktopManager(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDesktopManager && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg2_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
            public uint fEnumDisplayModes;
        }

        [ComVisible(false)]
        private struct Msg0_RebuildMonitorCache
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg1_ChangeFullScreenMode
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint idDisplay;
            public RenderDisplayMode mode;
            public uint fTvMode;
        }
    }
}
