// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt.RemoteGdiEffect
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt
{
    internal class RemoteGdiEffect : RemoteEffect
    {
        private static ushort[] s_priv_ByteOrder_Msg8_SetType;

        protected RemoteGdiEffect()
        {
        }

        public static unsafe RemoteGdiEffect Create(
          ProtocolSplashRenderingNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE effectClassHandle = _priv_protocolInstance.GdiEffect_ClassHandle;
            RemoteGdiEffect remoteGdiEffect = new RemoteGdiEffect(port, _priv_owner);
            port.CreateRemoteObject(effectClassHandle, remoteGdiEffect.m_renderHandle, null);
            return remoteGdiEffect;
        }

        public unsafe void BuildSetType(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nType)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteGdiEffect.Msg8_SetType);
            RemoteGdiEffect.Msg8_SetType* msg8SetTypePtr = (RemoteGdiEffect.Msg8_SetType*)_priv_portUse.AllocMessageBuffer(size);
            msg8SetTypePtr->_priv_size = size;
            msg8SetTypePtr->_priv_msgid = 8U;
            msg8SetTypePtr->nType = nType;
            msg8SetTypePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg8SetTypePtr, ref s_priv_ByteOrder_Msg8_SetType, typeof(RemoteGdiEffect.Msg8_SetType), 0, 0);
            _priv_pmsgUse = (Message*)msg8SetTypePtr;
        }

        public unsafe void SendSetType(int nType)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetType(out _priv_portUse, out _priv_pmsgUse, nType);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteGdiEffect CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteGdiEffect(port, handle, true);
        }

        public static RemoteGdiEffect CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteGdiEffect(port, handle, false);
        }

        protected RemoteGdiEffect(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteGdiEffect(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteGdiEffect && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg8_SetType
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nType;
        }
    }
}
