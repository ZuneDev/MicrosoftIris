// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteRenderCaps
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteRenderCaps : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg1_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_CheckCaps;

        protected RemoteRenderCaps()
        {
        }

        public static unsafe RemoteRenderCaps Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          LocalRenderCapsCallback callback)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE renderCapsClassHandle = _priv_protocolInstance.RenderCaps_ClassHandle;
            RemoteRenderCaps remoteRenderCaps = new RemoteRenderCaps(port, _priv_owner);
            uint num = (uint)sizeof(RemoteRenderCaps.Msg1_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteRenderCaps.Msg1_Create* msg1CreatePtr = (RemoteRenderCaps.Msg1_Create*)pMem;
            msg1CreatePtr->_priv_size = num;
            msg1CreatePtr->_priv_msgid = 1U;
            msg1CreatePtr->_priv_objcallback = callback.RenderHandle;
            msg1CreatePtr->_priv_ctxcallback = port.Session.LocalContext;
            msg1CreatePtr->_priv_idObjectSubject = remoteRenderCaps.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg1_Create, typeof(RemoteRenderCaps.Msg1_Create), 0, 0);
            port.CreateRemoteObject(renderCapsClassHandle, remoteRenderCaps.m_renderHandle, (Message*)msg1CreatePtr);
            return remoteRenderCaps;
        }

        public unsafe void BuildCheckCaps(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint nCookie)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteRenderCaps.Msg0_CheckCaps);
            RemoteRenderCaps.Msg0_CheckCaps* msg0CheckCapsPtr = (RemoteRenderCaps.Msg0_CheckCaps*)_priv_portUse.AllocMessageBuffer(size);
            msg0CheckCapsPtr->_priv_size = size;
            msg0CheckCapsPtr->_priv_msgid = 0U;
            msg0CheckCapsPtr->nCookie = nCookie;
            msg0CheckCapsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0CheckCapsPtr, ref s_priv_ByteOrder_Msg0_CheckCaps, typeof(RemoteRenderCaps.Msg0_CheckCaps), 0, 0);
            _priv_pmsgUse = (Message*)msg0CheckCapsPtr;
        }

        public unsafe void SendCheckCaps(uint nCookie)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCheckCaps(out _priv_portUse, out _priv_pmsgUse, nCookie);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteRenderCaps CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteRenderCaps(port, handle, true);
        }

        public static RemoteRenderCaps CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteRenderCaps(port, handle, false);
        }

        protected RemoteRenderCaps(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteRenderCaps(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteRenderCaps && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg1_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objcallback;
            public ContextID _priv_ctxcallback;
        }

        [ComVisible(false)]
        private struct Msg0_CheckCaps
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nCookie;
        }
    }
}
