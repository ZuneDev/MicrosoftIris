// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteVisualContainer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteVisualContainer : RemoteVisual
    {
        private static ushort[] s_priv_ByteOrder_Msg19_Create;
        private static ushort[] s_priv_ByteOrder_Msg18_SetCamera;

        protected RemoteVisualContainer()
        {
        }

        public static unsafe RemoteVisualContainer Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE containerClassHandle = _priv_protocolInstance.VisualContainer_ClassHandle;
            RemoteVisualContainer remoteVisualContainer = new RemoteVisualContainer(port, _priv_owner);
            uint num = (uint)sizeof(RemoteVisualContainer.Msg19_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteVisualContainer.Msg19_Create* msg19CreatePtr = (RemoteVisualContainer.Msg19_Create*)pMem;
            msg19CreatePtr->_priv_size = num;
            msg19CreatePtr->_priv_msgid = 19U;
            msg19CreatePtr->_priv_idObjectSubject = remoteVisualContainer.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg19_Create, typeof(RemoteVisualContainer.Msg19_Create), 0, 0);
            port.CreateRemoteObject(containerClassHandle, remoteVisualContainer.m_renderHandle, (Message*)msg19CreatePtr);
            return remoteVisualContainer;
        }

        public unsafe void BuildSetCamera(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteCamera camera)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (camera != null)
                _priv_portUse.ValidateHandleOrNull(camera.RenderHandle);
            uint size = (uint)sizeof(RemoteVisualContainer.Msg18_SetCamera);
            RemoteVisualContainer.Msg18_SetCamera* msg18SetCameraPtr = (RemoteVisualContainer.Msg18_SetCamera*)_priv_portUse.AllocMessageBuffer(size);
            msg18SetCameraPtr->_priv_size = size;
            msg18SetCameraPtr->_priv_msgid = 18U;
            msg18SetCameraPtr->camera = camera != null ? camera.RenderHandle : RENDERHANDLE.NULL;
            msg18SetCameraPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg18SetCameraPtr, ref s_priv_ByteOrder_Msg18_SetCamera, typeof(RemoteVisualContainer.Msg18_SetCamera), 0, 0);
            _priv_pmsgUse = (Message*)msg18SetCameraPtr;
        }

        public unsafe void SendSetCamera(RemoteCamera camera)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetCamera(out _priv_portUse, out _priv_pmsgUse, camera);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteVisualContainer CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteVisualContainer(port, handle, true);
        }

        public static RemoteVisualContainer CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteVisualContainer(port, handle, false);
        }

        protected RemoteVisualContainer(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteVisualContainer(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteVisualContainer && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg19_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg18_SetCamera
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE camera;
        }
    }
}
