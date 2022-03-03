// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteDx9Sprite
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteDx9Sprite : RemoteSprite
    {
        private static ushort[] s_priv_ByteOrder_Msg22_Create;

        protected RemoteDx9Sprite()
        {
        }

        public static unsafe RemoteDx9Sprite Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE spriteClassHandle = _priv_protocolInstance.Dx9Sprite_ClassHandle;
            RemoteDx9Sprite remoteDx9Sprite = new RemoteDx9Sprite(port, _priv_owner);
            uint num = (uint)sizeof(RemoteDx9Sprite.Msg22_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteDx9Sprite.Msg22_Create* msg22CreatePtr = (RemoteDx9Sprite.Msg22_Create*)pMem;
            msg22CreatePtr->_priv_size = num;
            msg22CreatePtr->_priv_msgid = 22U;
            msg22CreatePtr->_priv_idObjectSubject = remoteDx9Sprite.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg22_Create, typeof(RemoteDx9Sprite.Msg22_Create), 0, 0);
            port.CreateRemoteObject(spriteClassHandle, remoteDx9Sprite.m_renderHandle, (Message*)msg22CreatePtr);
            return remoteDx9Sprite;
        }

        public static RemoteDx9Sprite CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteDx9Sprite(port, handle, true);
        }

        public static RemoteDx9Sprite CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDx9Sprite(port, handle, false);
        }

        protected RemoteDx9Sprite(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDx9Sprite(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDx9Sprite && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg22_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }
    }
}
