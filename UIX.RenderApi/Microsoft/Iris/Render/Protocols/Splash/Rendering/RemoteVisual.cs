// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteVisual
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteVisual : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_ClearGradients;
        private static ushort[] s_priv_ByteOrder_Msg1_AddGradient;
        private static ushort[] s_priv_ByteOrder_Msg2_SetRelativeSize;
        private static ushort[] s_priv_ByteOrder_Msg3_Reset;
        private static ushort[] s_priv_ByteOrder_Msg4_ChangeDataBits;
        private static ushort[] s_priv_ByteOrder_Msg5_ChangeParent;
        private static ushort[] s_priv_ByteOrder_Msg6_SetDebugOutlineColor;
        private static ushort[] s_priv_ByteOrder_Msg7_SetDebugID;
        private static ushort[] s_priv_ByteOrder_Msg8_SetAlpha;
        private static ushort[] s_priv_ByteOrder_Msg9_SetLayer;
        private static ushort[] s_priv_ByteOrder_Msg10_SetRotation;
        private static ushort[] s_priv_ByteOrder_Msg11_SetCenterPointScale;
        private static ushort[] s_priv_ByteOrder_Msg12_SetScale;
        private static ushort[] s_priv_ByteOrder_Msg13_SetSize;
        private static ushort[] s_priv_ByteOrder_Msg14_SetPosition;
        private static ushort[] s_priv_ByteOrder_Msg16_SetVisible;

        protected RemoteVisual()
        {
        }

        public unsafe void BuildClearGradients(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg0_ClearGradients);
            RemoteVisual.Msg0_ClearGradients* msg0ClearGradientsPtr = (RemoteVisual.Msg0_ClearGradients*)_priv_portUse.AllocMessageBuffer(size);
            msg0ClearGradientsPtr->_priv_size = size;
            msg0ClearGradientsPtr->_priv_msgid = 0U;
            msg0ClearGradientsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0ClearGradientsPtr, ref s_priv_ByteOrder_Msg0_ClearGradients, typeof(RemoteVisual.Msg0_ClearGradients), 0, 0);
            _priv_pmsgUse = (Message*)msg0ClearGradientsPtr;
        }

        public unsafe void SendClearGradients()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildClearGradients(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildAddGradient(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteGradient grad)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (grad != null)
                _priv_portUse.ValidateHandleOrNull(grad.RenderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg1_AddGradient);
            RemoteVisual.Msg1_AddGradient* msg1AddGradientPtr = (RemoteVisual.Msg1_AddGradient*)_priv_portUse.AllocMessageBuffer(size);
            msg1AddGradientPtr->_priv_size = size;
            msg1AddGradientPtr->_priv_msgid = 1U;
            msg1AddGradientPtr->grad = grad != null ? grad.RenderHandle : RENDERHANDLE.NULL;
            msg1AddGradientPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg1AddGradientPtr, ref s_priv_ByteOrder_Msg1_AddGradient, typeof(RemoteVisual.Msg1_AddGradient), 0, 0);
            _priv_pmsgUse = (Message*)msg1AddGradientPtr;
        }

        public unsafe void SendAddGradient(RemoteGradient grad)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildAddGradient(out _priv_portUse, out _priv_pmsgUse, grad);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetRelativeSize(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fRelative)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg2_SetRelativeSize);
            RemoteVisual.Msg2_SetRelativeSize* msg2SetRelativeSizePtr = (RemoteVisual.Msg2_SetRelativeSize*)_priv_portUse.AllocMessageBuffer(size);
            msg2SetRelativeSizePtr->_priv_size = size;
            msg2SetRelativeSizePtr->_priv_msgid = 2U;
            msg2SetRelativeSizePtr->fRelative = fRelative ? uint.MaxValue : 0U;
            msg2SetRelativeSizePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg2SetRelativeSizePtr, ref s_priv_ByteOrder_Msg2_SetRelativeSize, typeof(RemoteVisual.Msg2_SetRelativeSize), 0, 0);
            _priv_pmsgUse = (Message*)msg2SetRelativeSizePtr;
        }

        public unsafe void SendSetRelativeSize(bool fRelative)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetRelativeSize(out _priv_portUse, out _priv_pmsgUse, fRelative);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildReset(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg3_Reset);
            RemoteVisual.Msg3_Reset* msg3ResetPtr = (RemoteVisual.Msg3_Reset*)_priv_portUse.AllocMessageBuffer(size);
            msg3ResetPtr->_priv_size = size;
            msg3ResetPtr->_priv_msgid = 3U;
            msg3ResetPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg3ResetPtr, ref s_priv_ByteOrder_Msg3_Reset, typeof(RemoteVisual.Msg3_Reset), 0, 0);
            _priv_pmsgUse = (Message*)msg3ResetPtr;
        }

        public unsafe void SendReset()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildReset(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildChangeDataBits(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint nValue,
          uint nMask)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg4_ChangeDataBits);
            RemoteVisual.Msg4_ChangeDataBits* msg4ChangeDataBitsPtr = (RemoteVisual.Msg4_ChangeDataBits*)_priv_portUse.AllocMessageBuffer(size);
            msg4ChangeDataBitsPtr->_priv_size = size;
            msg4ChangeDataBitsPtr->_priv_msgid = 4U;
            msg4ChangeDataBitsPtr->nValue = nValue;
            msg4ChangeDataBitsPtr->nMask = nMask;
            msg4ChangeDataBitsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4ChangeDataBitsPtr, ref s_priv_ByteOrder_Msg4_ChangeDataBits, typeof(RemoteVisual.Msg4_ChangeDataBits), 0, 0);
            _priv_pmsgUse = (Message*)msg4ChangeDataBitsPtr;
        }

        public unsafe void SendChangeDataBits(uint nValue, uint nMask)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildChangeDataBits(out _priv_portUse, out _priv_pmsgUse, nValue, nMask);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildChangeParent(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteVisual visNewParent,
          RemoteVisual visSibling,
          VisualOrder nOrder)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (visNewParent != null)
                _priv_portUse.ValidateHandleOrNull(visNewParent.RenderHandle);
            if (visSibling != null)
                _priv_portUse.ValidateHandleOrNull(visSibling.RenderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg5_ChangeParent);
            RemoteVisual.Msg5_ChangeParent* msg5ChangeParentPtr = (RemoteVisual.Msg5_ChangeParent*)_priv_portUse.AllocMessageBuffer(size);
            msg5ChangeParentPtr->_priv_size = size;
            msg5ChangeParentPtr->_priv_msgid = 5U;
            msg5ChangeParentPtr->visNewParent = visNewParent != null ? visNewParent.RenderHandle : RENDERHANDLE.NULL;
            msg5ChangeParentPtr->visSibling = visSibling != null ? visSibling.RenderHandle : RENDERHANDLE.NULL;
            msg5ChangeParentPtr->nOrder = nOrder;
            msg5ChangeParentPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg5ChangeParentPtr, ref s_priv_ByteOrder_Msg5_ChangeParent, typeof(RemoteVisual.Msg5_ChangeParent), 0, 0);
            _priv_pmsgUse = (Message*)msg5ChangeParentPtr;
        }

        public unsafe void SendChangeParent(
          RemoteVisual visNewParent,
          RemoteVisual visSibling,
          VisualOrder nOrder)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildChangeParent(out _priv_portUse, out _priv_pmsgUse, visNewParent, visSibling, nOrder);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetDebugOutlineColor(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          ColorF clr)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg6_SetDebugOutlineColor);
            RemoteVisual.Msg6_SetDebugOutlineColor* debugOutlineColorPtr = (RemoteVisual.Msg6_SetDebugOutlineColor*)_priv_portUse.AllocMessageBuffer(size);
            debugOutlineColorPtr->_priv_size = size;
            debugOutlineColorPtr->_priv_msgid = 6U;
            debugOutlineColorPtr->clr = clr;
            debugOutlineColorPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)debugOutlineColorPtr, ref s_priv_ByteOrder_Msg6_SetDebugOutlineColor, typeof(RemoteVisual.Msg6_SetDebugOutlineColor), 0, 0);
            _priv_pmsgUse = (Message*)debugOutlineColorPtr;
        }

        public unsafe void SendSetDebugOutlineColor(ColorF clr)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetDebugOutlineColor(out _priv_portUse, out _priv_pmsgUse, clr);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetDebugID(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string stDebugID)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteVisual.Msg7_SetDebugID));
            BLOBREF blobref = blobInfo.Add(stDebugID);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteVisual.Msg7_SetDebugID* msg7SetDebugIdPtr = (RemoteVisual.Msg7_SetDebugID*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg7SetDebugIdPtr->_priv_size = adjustedTotalSize;
            msg7SetDebugIdPtr->_priv_msgid = 7U;
            msg7SetDebugIdPtr->stDebugID = blobref;
            blobInfo.Attach((Message*)msg7SetDebugIdPtr);
            msg7SetDebugIdPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg7SetDebugIdPtr, ref s_priv_ByteOrder_Msg7_SetDebugID, typeof(RemoteVisual.Msg7_SetDebugID), 0, 0);
            _priv_pmsgUse = (Message*)msg7SetDebugIdPtr;
        }

        public unsafe void SendSetDebugID(string stDebugID)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetDebugID(out _priv_portUse, out _priv_pmsgUse, stDebugID);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetAlpha(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float fAlpha)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg8_SetAlpha);
            RemoteVisual.Msg8_SetAlpha* msg8SetAlphaPtr = (RemoteVisual.Msg8_SetAlpha*)_priv_portUse.AllocMessageBuffer(size);
            msg8SetAlphaPtr->_priv_size = size;
            msg8SetAlphaPtr->_priv_msgid = 8U;
            msg8SetAlphaPtr->fAlpha = fAlpha;
            msg8SetAlphaPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg8SetAlphaPtr, ref s_priv_ByteOrder_Msg8_SetAlpha, typeof(RemoteVisual.Msg8_SetAlpha), 0, 0);
            _priv_pmsgUse = (Message*)msg8SetAlphaPtr;
        }

        public unsafe void SendSetAlpha(float fAlpha)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetAlpha(out _priv_portUse, out _priv_pmsgUse, fAlpha);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetLayer(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint layer)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg9_SetLayer);
            RemoteVisual.Msg9_SetLayer* msg9SetLayerPtr = (RemoteVisual.Msg9_SetLayer*)_priv_portUse.AllocMessageBuffer(size);
            msg9SetLayerPtr->_priv_size = size;
            msg9SetLayerPtr->_priv_msgid = 9U;
            msg9SetLayerPtr->layer = layer;
            msg9SetLayerPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg9SetLayerPtr, ref s_priv_ByteOrder_Msg9_SetLayer, typeof(RemoteVisual.Msg9_SetLayer), 0, 0);
            _priv_pmsgUse = (Message*)msg9SetLayerPtr;
        }

        public unsafe void SendSetLayer(uint layer)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetLayer(out _priv_portUse, out _priv_pmsgUse, layer);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetRotation(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          AxisAngle aaRotation)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg10_SetRotation);
            RemoteVisual.Msg10_SetRotation* msg10SetRotationPtr = (RemoteVisual.Msg10_SetRotation*)_priv_portUse.AllocMessageBuffer(size);
            msg10SetRotationPtr->_priv_size = size;
            msg10SetRotationPtr->_priv_msgid = 10U;
            msg10SetRotationPtr->aaRotation = aaRotation;
            msg10SetRotationPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg10SetRotationPtr, ref s_priv_ByteOrder_Msg10_SetRotation, typeof(RemoteVisual.Msg10_SetRotation), 0, 0);
            _priv_pmsgUse = (Message*)msg10SetRotationPtr;
        }

        public unsafe void SendSetRotation(AxisAngle aaRotation)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetRotation(out _priv_portUse, out _priv_pmsgUse, aaRotation);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetCenterPointScale(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Vector3 vCenterPointScale)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg11_SetCenterPointScale);
            RemoteVisual.Msg11_SetCenterPointScale* centerPointScalePtr = (RemoteVisual.Msg11_SetCenterPointScale*)_priv_portUse.AllocMessageBuffer(size);
            centerPointScalePtr->_priv_size = size;
            centerPointScalePtr->_priv_msgid = 11U;
            centerPointScalePtr->vCenterPointScale = vCenterPointScale;
            centerPointScalePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)centerPointScalePtr, ref s_priv_ByteOrder_Msg11_SetCenterPointScale, typeof(RemoteVisual.Msg11_SetCenterPointScale), 0, 0);
            _priv_pmsgUse = (Message*)centerPointScalePtr;
        }

        public unsafe void SendSetCenterPointScale(Vector3 vCenterPointScale)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetCenterPointScale(out _priv_portUse, out _priv_pmsgUse, vCenterPointScale);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetScale(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Vector3 vScale)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg12_SetScale);
            RemoteVisual.Msg12_SetScale* msg12SetScalePtr = (RemoteVisual.Msg12_SetScale*)_priv_portUse.AllocMessageBuffer(size);
            msg12SetScalePtr->_priv_size = size;
            msg12SetScalePtr->_priv_msgid = 12U;
            msg12SetScalePtr->vScale = vScale;
            msg12SetScalePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg12SetScalePtr, ref s_priv_ByteOrder_Msg12_SetScale, typeof(RemoteVisual.Msg12_SetScale), 0, 0);
            _priv_pmsgUse = (Message*)msg12SetScalePtr;
        }

        public unsafe void SendSetScale(Vector3 vScale)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetScale(out _priv_portUse, out _priv_pmsgUse, vScale);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetSize(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Vector2 vSizePxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg13_SetSize);
            RemoteVisual.Msg13_SetSize* msg13SetSizePtr = (RemoteVisual.Msg13_SetSize*)_priv_portUse.AllocMessageBuffer(size);
            msg13SetSizePtr->_priv_size = size;
            msg13SetSizePtr->_priv_msgid = 13U;
            msg13SetSizePtr->vSizePxl = vSizePxl;
            msg13SetSizePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg13SetSizePtr, ref s_priv_ByteOrder_Msg13_SetSize, typeof(RemoteVisual.Msg13_SetSize), 0, 0);
            _priv_pmsgUse = (Message*)msg13SetSizePtr;
        }

        public unsafe void SendSetSize(Vector2 vSizePxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetSize(out _priv_portUse, out _priv_pmsgUse, vSizePxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetPosition(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Vector3 vPositionPxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg14_SetPosition);
            RemoteVisual.Msg14_SetPosition* msg14SetPositionPtr = (RemoteVisual.Msg14_SetPosition*)_priv_portUse.AllocMessageBuffer(size);
            msg14SetPositionPtr->_priv_size = size;
            msg14SetPositionPtr->_priv_msgid = 14U;
            msg14SetPositionPtr->vPositionPxl = vPositionPxl;
            msg14SetPositionPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg14SetPositionPtr, ref s_priv_ByteOrder_Msg14_SetPosition, typeof(RemoteVisual.Msg14_SetPosition), 0, 0);
            _priv_pmsgUse = (Message*)msg14SetPositionPtr;
        }

        public unsafe void SendSetPosition(Vector3 vPositionPxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetPosition(out _priv_portUse, out _priv_pmsgUse, vPositionPxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetVisible(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fVisible)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteVisual.Msg16_SetVisible);
            RemoteVisual.Msg16_SetVisible* msg16SetVisiblePtr = (RemoteVisual.Msg16_SetVisible*)_priv_portUse.AllocMessageBuffer(size);
            msg16SetVisiblePtr->_priv_size = size;
            msg16SetVisiblePtr->_priv_msgid = 16U;
            msg16SetVisiblePtr->fVisible = fVisible ? uint.MaxValue : 0U;
            msg16SetVisiblePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg16SetVisiblePtr, ref s_priv_ByteOrder_Msg16_SetVisible, typeof(RemoteVisual.Msg16_SetVisible), 0, 0);
            _priv_pmsgUse = (Message*)msg16SetVisiblePtr;
        }

        public unsafe void SendSetVisible(bool fVisible)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetVisible(out _priv_portUse, out _priv_pmsgUse, fVisible);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteVisual CreateFromHandle(RenderPort port, RENDERHANDLE handle) => new RemoteVisual(port, handle, true);

        public static RemoteVisual CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteVisual(port, handle, false);
        }

        protected RemoteVisual(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteVisual(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteVisual && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_ClearGradients
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg1_AddGradient
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE grad;
        }

        [ComVisible(false)]
        private struct Msg2_SetRelativeSize
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fRelative;
        }

        [ComVisible(false)]
        private struct Msg3_Reset
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg4_ChangeDataBits
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nValue;
            public uint nMask;
        }

        [ComVisible(false)]
        private struct Msg5_ChangeParent
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE visNewParent;
            public RENDERHANDLE visSibling;
            public VisualOrder nOrder;
        }

        [ComVisible(false)]
        private struct Msg6_SetDebugOutlineColor
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public ColorF clr;
        }

        [ComVisible(false)]
        private struct Msg7_SetDebugID
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF stDebugID;
        }

        [ComVisible(false)]
        private struct Msg8_SetAlpha
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float fAlpha;
        }

        [ComVisible(false)]
        private struct Msg9_SetLayer
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint layer;
        }

        [ComVisible(false)]
        private struct Msg10_SetRotation
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public AxisAngle aaRotation;
        }

        [ComVisible(false)]
        private struct Msg11_SetCenterPointScale
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Vector3 vCenterPointScale;
        }

        [ComVisible(false)]
        private struct Msg12_SetScale
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Vector3 vScale;
        }

        [ComVisible(false)]
        private struct Msg13_SetSize
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Vector2 vSizePxl;
        }

        [ComVisible(false)]
        private struct Msg14_SetPosition
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Vector3 vPositionPxl;
        }

        [ComVisible(false)]
        private struct Msg16_SetVisible
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fVisible;
        }
    }
}
