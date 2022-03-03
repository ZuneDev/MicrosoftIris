// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteExternalAnimationInput
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteExternalAnimationInput : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_Create;

        protected RemoteExternalAnimationInput()
        {
        }

        public static unsafe RemoteExternalAnimationInput Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          uint nUniqueId)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE inputClassHandle = _priv_protocolInstance.ExternalAnimationInput_ClassHandle;
            RemoteExternalAnimationInput externalAnimationInput = new RemoteExternalAnimationInput(port, _priv_owner);
            uint num = (uint)sizeof(RemoteExternalAnimationInput.Msg0_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteExternalAnimationInput.Msg0_Create* msg0CreatePtr = (RemoteExternalAnimationInput.Msg0_Create*)pMem;
            msg0CreatePtr->_priv_size = num;
            msg0CreatePtr->_priv_msgid = 0U;
            msg0CreatePtr->nUniqueId = nUniqueId;
            msg0CreatePtr->_priv_idObjectSubject = externalAnimationInput.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg0_Create, typeof(RemoteExternalAnimationInput.Msg0_Create), 0, 0);
            port.CreateRemoteObject(inputClassHandle, externalAnimationInput.m_renderHandle, (Message*)msg0CreatePtr);
            return externalAnimationInput;
        }

        public static RemoteExternalAnimationInput CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteExternalAnimationInput(port, handle, true);
        }

        public static RemoteExternalAnimationInput CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteExternalAnimationInput(port, handle, false);
        }

        protected RemoteExternalAnimationInput(
          RenderPort port,
          RENDERHANDLE handle,
          bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteExternalAnimationInput(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteExternalAnimationInput && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nUniqueId;
        }
    }
}
