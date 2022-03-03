// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.RemoteInputRouter
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop
{
    internal class RemoteInputRouter : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg2_Create;
        private static ushort[] s_priv_ByteOrder_Msg0_UnRegisterWithInputSource;
        private static ushort[] s_priv_ByteOrder_Msg1_RegisterWithInputSource;

        protected RemoteInputRouter()
        {
        }

        public static unsafe RemoteInputRouter Create(
          ProtocolSplashDesktop _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          LocalInputCallback ic)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE routerClassHandle = _priv_protocolInstance.InputRouter_ClassHandle;
            RemoteInputRouter remoteInputRouter = new RemoteInputRouter(port, _priv_owner);
            uint num = (uint)sizeof(RemoteInputRouter.Msg2_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteInputRouter.Msg2_Create* msg2CreatePtr = (RemoteInputRouter.Msg2_Create*)pMem;
            msg2CreatePtr->_priv_size = num;
            msg2CreatePtr->_priv_msgid = 2U;
            msg2CreatePtr->_priv_objic = ic.RenderHandle;
            msg2CreatePtr->_priv_ctxic = port.Session.LocalContext;
            msg2CreatePtr->_priv_idObjectSubject = remoteInputRouter.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg2_Create, typeof(RemoteInputRouter.Msg2_Create), 0, 0);
            port.CreateRemoteObject(routerClassHandle, remoteInputRouter.m_renderHandle, (Message*)msg2CreatePtr);
            return remoteInputRouter;
        }

        public unsafe void BuildUnRegisterWithInputSource(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteInputRouter.Msg0_UnRegisterWithInputSource);
            RemoteInputRouter.Msg0_UnRegisterWithInputSource* registerWithInputSourcePtr = (RemoteInputRouter.Msg0_UnRegisterWithInputSource*)_priv_portUse.AllocMessageBuffer(size);
            registerWithInputSourcePtr->_priv_size = size;
            registerWithInputSourcePtr->_priv_msgid = 0U;
            registerWithInputSourcePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)registerWithInputSourcePtr, ref s_priv_ByteOrder_Msg0_UnRegisterWithInputSource, typeof(RemoteInputRouter.Msg0_UnRegisterWithInputSource), 0, 0);
            _priv_pmsgUse = (Message*)registerWithInputSourcePtr;
        }

        public unsafe void SendUnRegisterWithInputSource()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildUnRegisterWithInputSource(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRegisterWithInputSource(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint idGroup,
          RENDERHANDLE idSource)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteInputRouter.Msg1_RegisterWithInputSource);
            RemoteInputRouter.Msg1_RegisterWithInputSource* registerWithInputSourcePtr = (RemoteInputRouter.Msg1_RegisterWithInputSource*)_priv_portUse.AllocMessageBuffer(size);
            registerWithInputSourcePtr->_priv_size = size;
            registerWithInputSourcePtr->_priv_msgid = 1U;
            registerWithInputSourcePtr->idGroup = idGroup;
            registerWithInputSourcePtr->idSource = idSource;
            registerWithInputSourcePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)registerWithInputSourcePtr, ref s_priv_ByteOrder_Msg1_RegisterWithInputSource, typeof(RemoteInputRouter.Msg1_RegisterWithInputSource), 0, 0);
            _priv_pmsgUse = (Message*)registerWithInputSourcePtr;
        }

        public unsafe void SendRegisterWithInputSource(uint idGroup, RENDERHANDLE idSource)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRegisterWithInputSource(out _priv_portUse, out _priv_pmsgUse, idGroup, idSource);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteInputRouter CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteInputRouter(port, handle, true);
        }

        public static RemoteInputRouter CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteInputRouter(port, handle, false);
        }

        protected RemoteInputRouter(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteInputRouter(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteInputRouter && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg2_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objic;
            public ContextID _priv_ctxic;
        }

        [ComVisible(false)]
        private struct Msg0_UnRegisterWithInputSource
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg1_RegisterWithInputSource
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint idGroup;
            public RENDERHANDLE idSource;
        }
    }
}
