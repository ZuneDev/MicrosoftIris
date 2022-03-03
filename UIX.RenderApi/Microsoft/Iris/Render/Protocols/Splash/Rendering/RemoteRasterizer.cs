// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteRasterizer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Messaging;
using Microsoft.Iris.Render.Remote;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteRasterizer : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_LoadRawImage;

        protected RemoteRasterizer()
        {
        }

        public static unsafe void BuildLoadRawImage(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ProtocolSplashRendering _priv_protocolInstance,
          RemoteSurface surContent,
          RemoteDataBuffer buffer,
          ImageHeader info)
        {
            RenderPort port = _priv_protocolInstance.Port;
            _priv_portUse = port;
            RENDERHANDLE rasterizerClassHandle = _priv_protocolInstance.Rasterizer_ClassHandle;
            _priv_portUse.ValidateHandle(rasterizerClassHandle);
            if (surContent != null)
                _priv_portUse.ValidateHandleOrNull(surContent.RenderHandle);
            if (buffer != null)
                _priv_portUse.ValidateHandleOrNull(buffer.RenderHandle);
            uint size = (uint)sizeof(RemoteRasterizer.Msg0_LoadRawImage);
            RemoteRasterizer.Msg0_LoadRawImage* msg0LoadRawImagePtr = (RemoteRasterizer.Msg0_LoadRawImage*)_priv_portUse.AllocMessageBuffer(size);
            msg0LoadRawImagePtr->_priv_size = size;
            msg0LoadRawImagePtr->_priv_msgid = 0U;
            msg0LoadRawImagePtr->surContent = surContent != null ? surContent.RenderHandle : RENDERHANDLE.NULL;
            msg0LoadRawImagePtr->buffer = buffer != null ? buffer.RenderHandle : RENDERHANDLE.NULL;
            msg0LoadRawImagePtr->info = info;
            msg0LoadRawImagePtr->_priv_idObjectSubject = rasterizerClassHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0LoadRawImagePtr, ref s_priv_ByteOrder_Msg0_LoadRawImage, typeof(RemoteRasterizer.Msg0_LoadRawImage), 0, 0);
            _priv_pmsgUse = (Message*)msg0LoadRawImagePtr;
        }

        public static unsafe void SendLoadRawImage(
          ProtocolSplashRendering _priv_protocolInstance,
          RemoteSurface surContent,
          RemoteDataBuffer buffer,
          ImageHeader info)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            BuildLoadRawImage(out _priv_portUse, out _priv_pmsgUse, _priv_protocolInstance, surContent, buffer, info);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteRasterizer CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteRasterizer(port, handle, true);
        }

        public static RemoteRasterizer CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteRasterizer(port, handle, false);
        }

        protected RemoteRasterizer(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteRasterizer(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteRasterizer && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_LoadRawImage
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE surContent;
            public RENDERHANDLE buffer;
            public ImageHeader info;
        }
    }
}
