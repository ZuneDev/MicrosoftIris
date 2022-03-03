// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteDx9Effect
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteDx9Effect : RemoteEffect
    {
        private static ushort[] s_priv_ByteOrder_Msg8_LoadEffectResource;

        protected RemoteDx9Effect()
        {
        }

        public static unsafe RemoteDx9Effect Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE effectClassHandle = _priv_protocolInstance.Dx9Effect_ClassHandle;
            RemoteDx9Effect remoteDx9Effect = new RemoteDx9Effect(port, _priv_owner);
            port.CreateRemoteObject(effectClassHandle, remoteDx9Effect.m_renderHandle, null);
            return remoteDx9Effect;
        }

        public unsafe void BuildLoadEffectResource(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteDx9EffectResource effectResource)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (effectResource != null)
                _priv_portUse.ValidateHandleOrNull(effectResource.RenderHandle);
            uint size = (uint)sizeof(RemoteDx9Effect.Msg8_LoadEffectResource);
            RemoteDx9Effect.Msg8_LoadEffectResource* loadEffectResourcePtr = (RemoteDx9Effect.Msg8_LoadEffectResource*)_priv_portUse.AllocMessageBuffer(size);
            loadEffectResourcePtr->_priv_size = size;
            loadEffectResourcePtr->_priv_msgid = 8U;
            loadEffectResourcePtr->effectResource = effectResource != null ? effectResource.RenderHandle : RENDERHANDLE.NULL;
            loadEffectResourcePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)loadEffectResourcePtr, ref s_priv_ByteOrder_Msg8_LoadEffectResource, typeof(RemoteDx9Effect.Msg8_LoadEffectResource), 0, 0);
            _priv_pmsgUse = (Message*)loadEffectResourcePtr;
        }

        public unsafe void SendLoadEffectResource(RemoteDx9EffectResource effectResource)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildLoadEffectResource(out _priv_portUse, out _priv_pmsgUse, effectResource);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteDx9Effect CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteDx9Effect(port, handle, true);
        }

        public static RemoteDx9Effect CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDx9Effect(port, handle, false);
        }

        protected RemoteDx9Effect(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDx9Effect(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDx9Effect && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg8_LoadEffectResource
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE effectResource;
        }
    }
}
