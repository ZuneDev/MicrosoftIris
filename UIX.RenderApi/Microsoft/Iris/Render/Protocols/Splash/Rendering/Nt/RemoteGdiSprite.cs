// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt.RemoteGdiSprite
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt
{
    internal class RemoteGdiSprite : RemoteSprite
    {
        private static ushort[] s_priv_ByteOrder_Msg22_Create;

        protected RemoteGdiSprite()
        {
        }

        public static unsafe RemoteGdiSprite Create(
          ProtocolSplashRenderingNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE spriteClassHandle = _priv_protocolInstance.GdiSprite_ClassHandle;
            RemoteGdiSprite remoteGdiSprite = new RemoteGdiSprite(port, _priv_owner);
            uint num = (uint)sizeof(RemoteGdiSprite.Msg22_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteGdiSprite.Msg22_Create* msg22CreatePtr = (RemoteGdiSprite.Msg22_Create*)pMem;
            msg22CreatePtr->_priv_size = num;
            msg22CreatePtr->_priv_msgid = 22U;
            msg22CreatePtr->_priv_idObjectSubject = remoteGdiSprite.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg22_Create, typeof(RemoteGdiSprite.Msg22_Create), 0, 0);
            port.CreateRemoteObject(spriteClassHandle, remoteGdiSprite.m_renderHandle, (Message*)msg22CreatePtr);
            return remoteGdiSprite;
        }

        public static RemoteGdiSprite CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteGdiSprite(port, handle, true);
        }

        public static RemoteGdiSprite CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteGdiSprite(port, handle, false);
        }

        protected RemoteGdiSprite(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteGdiSprite(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteGdiSprite && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

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
