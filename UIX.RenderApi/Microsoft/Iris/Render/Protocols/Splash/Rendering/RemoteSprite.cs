// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteSprite
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteSprite : RemoteVisual
    {
        private static ushort[] s_priv_ByteOrder_Msg18_SetNineGrid;
        private static ushort[] s_priv_ByteOrder_Msg19_AddCoordMapEntry;
        private static ushort[] s_priv_ByteOrder_Msg20_ClearCoordMaps;
        private static ushort[] s_priv_ByteOrder_Msg21_SetEffect;

        protected RemoteSprite()
        {
        }

        public unsafe void BuildSetNineGrid(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nLeft,
          int nTop,
          int nRight,
          int nBottom)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSprite.Msg18_SetNineGrid);
            RemoteSprite.Msg18_SetNineGrid* msg18SetNineGridPtr = (RemoteSprite.Msg18_SetNineGrid*)_priv_portUse.AllocMessageBuffer(size);
            msg18SetNineGridPtr->_priv_size = size;
            msg18SetNineGridPtr->_priv_msgid = 18U;
            msg18SetNineGridPtr->nLeft = nLeft;
            msg18SetNineGridPtr->nTop = nTop;
            msg18SetNineGridPtr->nRight = nRight;
            msg18SetNineGridPtr->nBottom = nBottom;
            msg18SetNineGridPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg18SetNineGridPtr, ref s_priv_ByteOrder_Msg18_SetNineGrid, typeof(RemoteSprite.Msg18_SetNineGrid), 0, 0);
            _priv_pmsgUse = (Message*)msg18SetNineGridPtr;
        }

        public unsafe void SendSetNineGrid(int nLeft, int nTop, int nRight, int nBottom)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetNineGrid(out _priv_portUse, out _priv_pmsgUse, nLeft, nTop, nRight, nBottom);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddCoordMapEntry(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idxLayer,
          Orientation orientation,
          float flPosition,
          float flValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSprite.Msg19_AddCoordMapEntry);
            RemoteSprite.Msg19_AddCoordMapEntry* addCoordMapEntryPtr = (RemoteSprite.Msg19_AddCoordMapEntry*)_priv_portUse.AllocMessageBuffer(size);
            addCoordMapEntryPtr->_priv_size = size;
            addCoordMapEntryPtr->_priv_msgid = 19U;
            addCoordMapEntryPtr->idxLayer = idxLayer;
            addCoordMapEntryPtr->orientation = orientation;
            addCoordMapEntryPtr->flPosition = flPosition;
            addCoordMapEntryPtr->flValue = flValue;
            addCoordMapEntryPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)addCoordMapEntryPtr, ref s_priv_ByteOrder_Msg19_AddCoordMapEntry, typeof(RemoteSprite.Msg19_AddCoordMapEntry), 0, 0);
            _priv_pmsgUse = (Message*)addCoordMapEntryPtr;
        }

        public unsafe void SendAddCoordMapEntry(
          int idxLayer,
          Orientation orientation,
          float flPosition,
          float flValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddCoordMapEntry(out _priv_portUse, out _priv_pmsgUse, idxLayer, orientation, flPosition, flValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildClearCoordMaps(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSprite.Msg20_ClearCoordMaps);
            RemoteSprite.Msg20_ClearCoordMaps* msg20ClearCoordMapsPtr = (RemoteSprite.Msg20_ClearCoordMaps*)_priv_portUse.AllocMessageBuffer(size);
            msg20ClearCoordMapsPtr->_priv_size = size;
            msg20ClearCoordMapsPtr->_priv_msgid = 20U;
            msg20ClearCoordMapsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg20ClearCoordMapsPtr, ref s_priv_ByteOrder_Msg20_ClearCoordMaps, typeof(RemoteSprite.Msg20_ClearCoordMaps), 0, 0);
            _priv_pmsgUse = (Message*)msg20ClearCoordMapsPtr;
        }

        public unsafe void SendClearCoordMaps()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildClearCoordMaps(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetEffect(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteEffect effect)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (effect != null)
                _priv_portUse.ValidateHandleOrNull(effect.RenderHandle);
            uint size = (uint)sizeof(RemoteSprite.Msg21_SetEffect);
            RemoteSprite.Msg21_SetEffect* msg21SetEffectPtr = (RemoteSprite.Msg21_SetEffect*)_priv_portUse.AllocMessageBuffer(size);
            msg21SetEffectPtr->_priv_size = size;
            msg21SetEffectPtr->_priv_msgid = 21U;
            msg21SetEffectPtr->effect = effect != null ? effect.RenderHandle : RENDERHANDLE.NULL;
            msg21SetEffectPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg21SetEffectPtr, ref s_priv_ByteOrder_Msg21_SetEffect, typeof(RemoteSprite.Msg21_SetEffect), 0, 0);
            _priv_pmsgUse = (Message*)msg21SetEffectPtr;
        }

        public unsafe void SendSetEffect(RemoteEffect effect)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEffect(out _priv_portUse, out _priv_pmsgUse, effect);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteSprite CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteSprite(port, handle, true);

        public static RemoteSprite CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteSprite(port, handle, false);
        }

        protected RemoteSprite(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteSprite(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteSprite && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg18_SetNineGrid
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nLeft;
            public int nTop;
            public int nRight;
            public int nBottom;
        }

        [ComVisible(false)]
        private struct Msg19_AddCoordMapEntry
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idxLayer;
            public Orientation orientation;
            public float flPosition;
            public float flValue;
        }

        [ComVisible(false)]
        private struct Msg20_ClearCoordMaps
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg21_SetEffect
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE effect;
        }
    }
}
