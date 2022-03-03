// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.RemoteFormWindowBase
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop
{
    internal class RemoteFormWindowBase : RemoteWindow
    {
        private static ushort[] s_priv_ByteOrder_Msg7_SetEdgeImageParts;
        private static ushort[] s_priv_ByteOrder_Msg8_SetInitialPlacement;
        private static ushort[] s_priv_ByteOrder_Msg9_RefreshHitTarget;
        private static ushort[] s_priv_ByteOrder_Msg10_ExitInternalDrag;
        private static ushort[] s_priv_ByteOrder_Msg11_EnterInternalDrag;
        private static ushort[] s_priv_ByteOrder_Msg12_SetDragDropResult;
        private static ushort[] s_priv_ByteOrder_Msg13_EnableExternalDragDrop;
        private static ushort[] s_priv_ByteOrder_Msg14_SetIcon;
        private static ushort[] s_priv_ByteOrder_Msg15_SetMaxResizeWidth;
        private static ushort[] s_priv_ByteOrder_Msg16_SetMinResizeWidth;
        private static ushort[] s_priv_ByteOrder_Msg17_BringToTop;
        private static ushort[] s_priv_ByteOrder_Msg18_EnableShellShutdownHook;
        private static ushort[] s_priv_ByteOrder_Msg19_UpdateForegroundLockState;
        private static ushort[] s_priv_ByteOrder_Msg21_SetHitMasks;
        private static ushort[] s_priv_ByteOrder_Msg22_SetCapture;
        private static ushort[] s_priv_ByteOrder_Msg23_SetText;
        private static ushort[] s_priv_ByteOrder_Msg24_Destroy;
        private static ushort[] s_priv_ByteOrder_Msg25_SetForeground;
        private static ushort[] s_priv_ByteOrder_Msg26_TakeFocus;
        private static ushort[] s_priv_ByteOrder_Msg27_ForceMouseIdle;
        private static ushort[] s_priv_ByteOrder_Msg28_TemporarilyExitExclusiveMode;
        private static ushort[] s_priv_ByteOrder_Msg29_Restore;
        private static ushort[] s_priv_ByteOrder_Msg30_SetMode;
        private static ushort[] s_priv_ByteOrder_Msg31_SetVisible;
        private static ushort[] s_priv_ByteOrder_Msg32_SetPosition;
        private static ushort[] s_priv_ByteOrder_Msg33_SetSize;
        private static ushort[] s_priv_ByteOrder_Msg34_SetCursors;
        private static ushort[] s_priv_ByteOrder_Msg35_SetStyles;
        private static ushort[] s_priv_ByteOrder_Msg36_SetMouseIdleOptions;
        private static ushort[] s_priv_ByteOrder_Msg37_SetOptions;
        private static ushort[] s_priv_ByteOrder_Msg38_CreateRootContainer;

        protected RemoteFormWindowBase()
        {
        }

        public unsafe void BuildSetEdgeImageParts(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fActiveEdges,
          string sModuleName,
          string sResourceIdLeft,
          string sResourceIdTop,
          string sResourceIdRight,
          string sResourceIdBottom,
          Inset insetSplits)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteFormWindowBase.Msg7_SetEdgeImageParts));
            BLOBREF blobref1 = blobInfo.Add(sModuleName);
            BLOBREF blobref2 = blobInfo.Add(sResourceIdLeft);
            BLOBREF blobref3 = blobInfo.Add(sResourceIdTop);
            BLOBREF blobref4 = blobInfo.Add(sResourceIdRight);
            BLOBREF blobref5 = blobInfo.Add(sResourceIdBottom);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteFormWindowBase.Msg7_SetEdgeImageParts* setEdgeImagePartsPtr = (RemoteFormWindowBase.Msg7_SetEdgeImageParts*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            setEdgeImagePartsPtr->_priv_size = adjustedTotalSize;
            setEdgeImagePartsPtr->_priv_msgid = 7U;
            setEdgeImagePartsPtr->fActiveEdges = fActiveEdges ? uint.MaxValue : 0U;
            setEdgeImagePartsPtr->sModuleName = blobref1;
            setEdgeImagePartsPtr->sResourceIdLeft = blobref2;
            setEdgeImagePartsPtr->sResourceIdTop = blobref3;
            setEdgeImagePartsPtr->sResourceIdRight = blobref4;
            setEdgeImagePartsPtr->sResourceIdBottom = blobref5;
            setEdgeImagePartsPtr->insetSplits = insetSplits;
            blobInfo.Attach((Message*)setEdgeImagePartsPtr);
            setEdgeImagePartsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setEdgeImagePartsPtr, ref s_priv_ByteOrder_Msg7_SetEdgeImageParts, typeof(RemoteFormWindowBase.Msg7_SetEdgeImageParts), 0, 0);
            _priv_pmsgUse = (Message*)setEdgeImagePartsPtr;
        }

        public unsafe void SendSetEdgeImageParts(
          bool fActiveEdges,
          string sModuleName,
          string sResourceIdLeft,
          string sResourceIdTop,
          string sResourceIdRight,
          string sResourceIdBottom,
          Inset insetSplits)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetEdgeImageParts(out _priv_portUse, out _priv_pmsgUse, fActiveEdges, sModuleName, sResourceIdLeft, sResourceIdTop, sResourceIdRight, sResourceIdBottom, insetSplits);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetInitialPlacement(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint nShowState,
          Rectangle rcNormalPosition,
          Point ptMaxPosition)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg8_SetInitialPlacement);
            RemoteFormWindowBase.Msg8_SetInitialPlacement* initialPlacementPtr = (RemoteFormWindowBase.Msg8_SetInitialPlacement*)_priv_portUse.AllocMessageBuffer(size);
            initialPlacementPtr->_priv_size = size;
            initialPlacementPtr->_priv_msgid = 8U;
            initialPlacementPtr->nShowState = nShowState;
            initialPlacementPtr->rcNormalPosition = rcNormalPosition;
            initialPlacementPtr->ptMaxPosition = ptMaxPosition;
            initialPlacementPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)initialPlacementPtr, ref s_priv_ByteOrder_Msg8_SetInitialPlacement, typeof(RemoteFormWindowBase.Msg8_SetInitialPlacement), 0, 0);
            _priv_pmsgUse = (Message*)initialPlacementPtr;
        }

        public unsafe void SendSetInitialPlacement(
          uint nShowState,
          Rectangle rcNormalPosition,
          Point ptMaxPosition)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetInitialPlacement(out _priv_portUse, out _priv_pmsgUse, nShowState, rcNormalPosition, ptMaxPosition);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRefreshHitTarget(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg9_RefreshHitTarget);
            RemoteFormWindowBase.Msg9_RefreshHitTarget* refreshHitTargetPtr = (RemoteFormWindowBase.Msg9_RefreshHitTarget*)_priv_portUse.AllocMessageBuffer(size);
            refreshHitTargetPtr->_priv_size = size;
            refreshHitTargetPtr->_priv_msgid = 9U;
            refreshHitTargetPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)refreshHitTargetPtr, ref s_priv_ByteOrder_Msg9_RefreshHitTarget, typeof(RemoteFormWindowBase.Msg9_RefreshHitTarget), 0, 0);
            _priv_pmsgUse = (Message*)refreshHitTargetPtr;
        }

        public unsafe void SendRefreshHitTarget()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRefreshHitTarget(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildExitInternalDrag(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg10_ExitInternalDrag);
            RemoteFormWindowBase.Msg10_ExitInternalDrag* exitInternalDragPtr = (RemoteFormWindowBase.Msg10_ExitInternalDrag*)_priv_portUse.AllocMessageBuffer(size);
            exitInternalDragPtr->_priv_size = size;
            exitInternalDragPtr->_priv_msgid = 10U;
            exitInternalDragPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)exitInternalDragPtr, ref s_priv_ByteOrder_Msg10_ExitInternalDrag, typeof(RemoteFormWindowBase.Msg10_ExitInternalDrag), 0, 0);
            _priv_pmsgUse = (Message*)exitInternalDragPtr;
        }

        public unsafe void SendExitInternalDrag()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildExitInternalDrag(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildEnterInternalDrag(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg11_EnterInternalDrag);
            RemoteFormWindowBase.Msg11_EnterInternalDrag* enterInternalDragPtr = (RemoteFormWindowBase.Msg11_EnterInternalDrag*)_priv_portUse.AllocMessageBuffer(size);
            enterInternalDragPtr->_priv_size = size;
            enterInternalDragPtr->_priv_msgid = 11U;
            enterInternalDragPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)enterInternalDragPtr, ref s_priv_ByteOrder_Msg11_EnterInternalDrag, typeof(RemoteFormWindowBase.Msg11_EnterInternalDrag), 0, 0);
            _priv_pmsgUse = (Message*)enterInternalDragPtr;
        }

        public unsafe void SendEnterInternalDrag()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildEnterInternalDrag(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetDragDropResult(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint nDragOverResult,
          uint nDragDropResult)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg12_SetDragDropResult);
            RemoteFormWindowBase.Msg12_SetDragDropResult* setDragDropResultPtr = (RemoteFormWindowBase.Msg12_SetDragDropResult*)_priv_portUse.AllocMessageBuffer(size);
            setDragDropResultPtr->_priv_size = size;
            setDragDropResultPtr->_priv_msgid = 12U;
            setDragDropResultPtr->nDragOverResult = nDragOverResult;
            setDragDropResultPtr->nDragDropResult = nDragDropResult;
            setDragDropResultPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setDragDropResultPtr, ref s_priv_ByteOrder_Msg12_SetDragDropResult, typeof(RemoteFormWindowBase.Msg12_SetDragDropResult), 0, 0);
            _priv_pmsgUse = (Message*)setDragDropResultPtr;
        }

        public unsafe void SendSetDragDropResult(uint nDragOverResult, uint nDragDropResult)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetDragDropResult(out _priv_portUse, out _priv_pmsgUse, nDragOverResult, nDragDropResult);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildEnableExternalDragDrop(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fEnable)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg13_EnableExternalDragDrop);
            RemoteFormWindowBase.Msg13_EnableExternalDragDrop* externalDragDropPtr = (RemoteFormWindowBase.Msg13_EnableExternalDragDrop*)_priv_portUse.AllocMessageBuffer(size);
            externalDragDropPtr->_priv_size = size;
            externalDragDropPtr->_priv_msgid = 13U;
            externalDragDropPtr->fEnable = fEnable ? uint.MaxValue : 0U;
            externalDragDropPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)externalDragDropPtr, ref s_priv_ByteOrder_Msg13_EnableExternalDragDrop, typeof(RemoteFormWindowBase.Msg13_EnableExternalDragDrop), 0, 0);
            _priv_pmsgUse = (Message*)externalDragDropPtr;
        }

        public unsafe void SendEnableExternalDragDrop(bool fEnable)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildEnableExternalDragDrop(out _priv_portUse, out _priv_pmsgUse, fEnable);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetIcon(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string sModule,
          uint nResourceID,
          uint nOptions)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteFormWindowBase.Msg14_SetIcon));
            BLOBREF blobref = blobInfo.Add(sModule);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteFormWindowBase.Msg14_SetIcon* msg14SetIconPtr = (RemoteFormWindowBase.Msg14_SetIcon*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg14SetIconPtr->_priv_size = adjustedTotalSize;
            msg14SetIconPtr->_priv_msgid = 14U;
            msg14SetIconPtr->sModule = blobref;
            msg14SetIconPtr->nResourceID = nResourceID;
            msg14SetIconPtr->nOptions = nOptions;
            blobInfo.Attach((Message*)msg14SetIconPtr);
            msg14SetIconPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg14SetIconPtr, ref s_priv_ByteOrder_Msg14_SetIcon, typeof(RemoteFormWindowBase.Msg14_SetIcon), 0, 0);
            _priv_pmsgUse = (Message*)msg14SetIconPtr;
        }

        public unsafe void SendSetIcon(string sModule, uint nResourceID, uint nOptions)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetIcon(out _priv_portUse, out _priv_pmsgUse, sModule, nResourceID, nOptions);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetMaxResizeWidth(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nMaxResizeWidth)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg15_SetMaxResizeWidth);
            RemoteFormWindowBase.Msg15_SetMaxResizeWidth* setMaxResizeWidthPtr = (RemoteFormWindowBase.Msg15_SetMaxResizeWidth*)_priv_portUse.AllocMessageBuffer(size);
            setMaxResizeWidthPtr->_priv_size = size;
            setMaxResizeWidthPtr->_priv_msgid = 15U;
            setMaxResizeWidthPtr->nMaxResizeWidth = nMaxResizeWidth;
            setMaxResizeWidthPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setMaxResizeWidthPtr, ref s_priv_ByteOrder_Msg15_SetMaxResizeWidth, typeof(RemoteFormWindowBase.Msg15_SetMaxResizeWidth), 0, 0);
            _priv_pmsgUse = (Message*)setMaxResizeWidthPtr;
        }

        public unsafe void SendSetMaxResizeWidth(int nMaxResizeWidth)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetMaxResizeWidth(out _priv_portUse, out _priv_pmsgUse, nMaxResizeWidth);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetMinResizeWidth(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int nMinResizeWidth)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg16_SetMinResizeWidth);
            RemoteFormWindowBase.Msg16_SetMinResizeWidth* setMinResizeWidthPtr = (RemoteFormWindowBase.Msg16_SetMinResizeWidth*)_priv_portUse.AllocMessageBuffer(size);
            setMinResizeWidthPtr->_priv_size = size;
            setMinResizeWidthPtr->_priv_msgid = 16U;
            setMinResizeWidthPtr->nMinResizeWidth = nMinResizeWidth;
            setMinResizeWidthPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)setMinResizeWidthPtr, ref s_priv_ByteOrder_Msg16_SetMinResizeWidth, typeof(RemoteFormWindowBase.Msg16_SetMinResizeWidth), 0, 0);
            _priv_pmsgUse = (Message*)setMinResizeWidthPtr;
        }

        public unsafe void SendSetMinResizeWidth(int nMinResizeWidth)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetMinResizeWidth(out _priv_portUse, out _priv_pmsgUse, nMinResizeWidth);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildBringToTop(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg17_BringToTop);
            RemoteFormWindowBase.Msg17_BringToTop* msg17BringToTopPtr = (RemoteFormWindowBase.Msg17_BringToTop*)_priv_portUse.AllocMessageBuffer(size);
            msg17BringToTopPtr->_priv_size = size;
            msg17BringToTopPtr->_priv_msgid = 17U;
            msg17BringToTopPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg17BringToTopPtr, ref s_priv_ByteOrder_Msg17_BringToTop, typeof(RemoteFormWindowBase.Msg17_BringToTop), 0, 0);
            _priv_pmsgUse = (Message*)msg17BringToTopPtr;
        }

        public unsafe void SendBringToTop()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildBringToTop(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildEnableShellShutdownHook(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string messageToHook,
          ushort uIdMsg)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteFormWindowBase.Msg18_EnableShellShutdownHook));
            BLOBREF blobref = blobInfo.Add(messageToHook);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteFormWindowBase.Msg18_EnableShellShutdownHook* shellShutdownHookPtr = (RemoteFormWindowBase.Msg18_EnableShellShutdownHook*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            shellShutdownHookPtr->_priv_size = adjustedTotalSize;
            shellShutdownHookPtr->_priv_msgid = 18U;
            shellShutdownHookPtr->messageToHook = blobref;
            shellShutdownHookPtr->uIdMsg = uIdMsg;
            blobInfo.Attach((Message*)shellShutdownHookPtr);
            shellShutdownHookPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)shellShutdownHookPtr, ref s_priv_ByteOrder_Msg18_EnableShellShutdownHook, typeof(RemoteFormWindowBase.Msg18_EnableShellShutdownHook), 0, 0);
            _priv_pmsgUse = (Message*)shellShutdownHookPtr;
        }

        public unsafe void SendEnableShellShutdownHook(string messageToHook, ushort uIdMsg)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildEnableShellShutdownHook(out _priv_portUse, out _priv_pmsgUse, messageToHook, uIdMsg);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildUpdateForegroundLockState(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg19_UpdateForegroundLockState);
            RemoteFormWindowBase.Msg19_UpdateForegroundLockState* foregroundLockStatePtr = (RemoteFormWindowBase.Msg19_UpdateForegroundLockState*)_priv_portUse.AllocMessageBuffer(size);
            foregroundLockStatePtr->_priv_size = size;
            foregroundLockStatePtr->_priv_msgid = 19U;
            foregroundLockStatePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)foregroundLockStatePtr, ref s_priv_ByteOrder_Msg19_UpdateForegroundLockState, typeof(RemoteFormWindowBase.Msg19_UpdateForegroundLockState), 0, 0);
            _priv_pmsgUse = (Message*)foregroundLockStatePtr;
        }

        public unsafe void SendUpdateForegroundLockState()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildUpdateForegroundLockState(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void INPROC_BuildSetAppNotifyWindow(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          HWND hwndAppNotify)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg20_SetAppNotifyWindow);
            RemoteFormWindowBase.Msg20_SetAppNotifyWindow* setAppNotifyWindowPtr = (RemoteFormWindowBase.Msg20_SetAppNotifyWindow*)_priv_portUse.AllocMessageBuffer(size);
            setAppNotifyWindowPtr->_priv_size = size;
            setAppNotifyWindowPtr->_priv_msgid = 20U;
            setAppNotifyWindowPtr->hwndAppNotify = hwndAppNotify;
            setAppNotifyWindowPtr->_priv_idObjectSubject = this.m_renderHandle;
            _priv_pmsgUse = (Message*)setAppNotifyWindowPtr;
        }

        public unsafe void INPROC_SendSetAppNotifyWindow(HWND hwndAppNotify)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.INPROC_BuildSetAppNotifyWindow(out _priv_portUse, out _priv_pmsgUse, hwndAppNotify);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetHitMasks(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint uMaskTraverse,
          uint uMaskHit,
          uint uMaskResult,
          uint uMaskClip)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg21_SetHitMasks);
            RemoteFormWindowBase.Msg21_SetHitMasks* msg21SetHitMasksPtr = (RemoteFormWindowBase.Msg21_SetHitMasks*)_priv_portUse.AllocMessageBuffer(size);
            msg21SetHitMasksPtr->_priv_size = size;
            msg21SetHitMasksPtr->_priv_msgid = 21U;
            msg21SetHitMasksPtr->uMaskTraverse = uMaskTraverse;
            msg21SetHitMasksPtr->uMaskHit = uMaskHit;
            msg21SetHitMasksPtr->uMaskResult = uMaskResult;
            msg21SetHitMasksPtr->uMaskClip = uMaskClip;
            msg21SetHitMasksPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg21SetHitMasksPtr, ref s_priv_ByteOrder_Msg21_SetHitMasks, typeof(RemoteFormWindowBase.Msg21_SetHitMasks), 0, 0);
            _priv_pmsgUse = (Message*)msg21SetHitMasksPtr;
        }

        public unsafe void SendSetHitMasks(
          uint uMaskTraverse,
          uint uMaskHit,
          uint uMaskResult,
          uint uMaskClip)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetHitMasks(out _priv_portUse, out _priv_pmsgUse, uMaskTraverse, uMaskHit, uMaskResult, uMaskClip);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetCapture(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RemoteVisual hVisual,
          bool fState)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (hVisual != null)
                _priv_portUse.ValidateHandleOrNull(hVisual.RenderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg22_SetCapture);
            RemoteFormWindowBase.Msg22_SetCapture* msg22SetCapturePtr = (RemoteFormWindowBase.Msg22_SetCapture*)_priv_portUse.AllocMessageBuffer(size);
            msg22SetCapturePtr->_priv_size = size;
            msg22SetCapturePtr->_priv_msgid = 22U;
            msg22SetCapturePtr->hVisual = hVisual != null ? hVisual.RenderHandle : RENDERHANDLE.NULL;
            msg22SetCapturePtr->fState = fState ? uint.MaxValue : 0U;
            msg22SetCapturePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg22SetCapturePtr, ref s_priv_ByteOrder_Msg22_SetCapture, typeof(RemoteFormWindowBase.Msg22_SetCapture), 0, 0);
            _priv_pmsgUse = (Message*)msg22SetCapturePtr;
        }

        public unsafe void SendSetCapture(RemoteVisual hVisual, bool fState)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetCapture(out _priv_portUse, out _priv_pmsgUse, hVisual, fState);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetText(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          string text)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            BlobInfo blobInfo = new BlobInfo(_priv_portUse, (uint)sizeof(RemoteFormWindowBase.Msg23_SetText));
            BLOBREF blobref = blobInfo.Add(text);
            uint adjustedTotalSize = blobInfo.AdjustedTotalSize;
            RemoteFormWindowBase.Msg23_SetText* msg23SetTextPtr = (RemoteFormWindowBase.Msg23_SetText*)_priv_portUse.AllocMessageBuffer(adjustedTotalSize);
            msg23SetTextPtr->_priv_size = adjustedTotalSize;
            msg23SetTextPtr->_priv_msgid = 23U;
            msg23SetTextPtr->text = blobref;
            blobInfo.Attach((Message*)msg23SetTextPtr);
            msg23SetTextPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg23SetTextPtr, ref s_priv_ByteOrder_Msg23_SetText, typeof(RemoteFormWindowBase.Msg23_SetText), 0, 0);
            _priv_pmsgUse = (Message*)msg23SetTextPtr;
        }

        public unsafe void SendSetText(string text)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetText(out _priv_portUse, out _priv_pmsgUse, text);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildDestroy(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg24_Destroy);
            RemoteFormWindowBase.Msg24_Destroy* msg24DestroyPtr = (RemoteFormWindowBase.Msg24_Destroy*)_priv_portUse.AllocMessageBuffer(size);
            msg24DestroyPtr->_priv_size = size;
            msg24DestroyPtr->_priv_msgid = 24U;
            msg24DestroyPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg24DestroyPtr, ref s_priv_ByteOrder_Msg24_Destroy, typeof(RemoteFormWindowBase.Msg24_Destroy), 0, 0);
            _priv_pmsgUse = (Message*)msg24DestroyPtr;
        }

        public unsafe void SendDestroy()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildDestroy(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetForeground(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fForce)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg25_SetForeground);
            RemoteFormWindowBase.Msg25_SetForeground* msg25SetForegroundPtr = (RemoteFormWindowBase.Msg25_SetForeground*)_priv_portUse.AllocMessageBuffer(size);
            msg25SetForegroundPtr->_priv_size = size;
            msg25SetForegroundPtr->_priv_msgid = 25U;
            msg25SetForegroundPtr->fForce = fForce ? uint.MaxValue : 0U;
            msg25SetForegroundPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg25SetForegroundPtr, ref s_priv_ByteOrder_Msg25_SetForeground, typeof(RemoteFormWindowBase.Msg25_SetForeground), 0, 0);
            _priv_pmsgUse = (Message*)msg25SetForegroundPtr;
        }

        public unsafe void SendSetForeground(bool fForce)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetForeground(out _priv_portUse, out _priv_pmsgUse, fForce);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildTakeFocus(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg26_TakeFocus);
            RemoteFormWindowBase.Msg26_TakeFocus* msg26TakeFocusPtr = (RemoteFormWindowBase.Msg26_TakeFocus*)_priv_portUse.AllocMessageBuffer(size);
            msg26TakeFocusPtr->_priv_size = size;
            msg26TakeFocusPtr->_priv_msgid = 26U;
            msg26TakeFocusPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg26TakeFocusPtr, ref s_priv_ByteOrder_Msg26_TakeFocus, typeof(RemoteFormWindowBase.Msg26_TakeFocus), 0, 0);
            _priv_pmsgUse = (Message*)msg26TakeFocusPtr;
        }

        public unsafe void SendTakeFocus()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildTakeFocus(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildForceMouseIdle(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fIdle)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg27_ForceMouseIdle);
            RemoteFormWindowBase.Msg27_ForceMouseIdle* msg27ForceMouseIdlePtr = (RemoteFormWindowBase.Msg27_ForceMouseIdle*)_priv_portUse.AllocMessageBuffer(size);
            msg27ForceMouseIdlePtr->_priv_size = size;
            msg27ForceMouseIdlePtr->_priv_msgid = 27U;
            msg27ForceMouseIdlePtr->fIdle = fIdle ? uint.MaxValue : 0U;
            msg27ForceMouseIdlePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg27ForceMouseIdlePtr, ref s_priv_ByteOrder_Msg27_ForceMouseIdle, typeof(RemoteFormWindowBase.Msg27_ForceMouseIdle), 0, 0);
            _priv_pmsgUse = (Message*)msg27ForceMouseIdlePtr;
        }

        public unsafe void SendForceMouseIdle(bool fIdle)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildForceMouseIdle(out _priv_portUse, out _priv_pmsgUse, fIdle);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildTemporarilyExitExclusiveMode(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg28_TemporarilyExitExclusiveMode);
            RemoteFormWindowBase.Msg28_TemporarilyExitExclusiveMode* exitExclusiveModePtr = (RemoteFormWindowBase.Msg28_TemporarilyExitExclusiveMode*)_priv_portUse.AllocMessageBuffer(size);
            exitExclusiveModePtr->_priv_size = size;
            exitExclusiveModePtr->_priv_msgid = 28U;
            exitExclusiveModePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)exitExclusiveModePtr, ref s_priv_ByteOrder_Msg28_TemporarilyExitExclusiveMode, typeof(RemoteFormWindowBase.Msg28_TemporarilyExitExclusiveMode), 0, 0);
            _priv_pmsgUse = (Message*)exitExclusiveModePtr;
        }

        public unsafe void SendTemporarilyExitExclusiveMode()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildTemporarilyExitExclusiveMode(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildRestore(out RenderPort _priv_portUse, out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg29_Restore);
            RemoteFormWindowBase.Msg29_Restore* msg29RestorePtr = (RemoteFormWindowBase.Msg29_Restore*)_priv_portUse.AllocMessageBuffer(size);
            msg29RestorePtr->_priv_size = size;
            msg29RestorePtr->_priv_msgid = 29U;
            msg29RestorePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg29RestorePtr, ref s_priv_ByteOrder_Msg29_Restore, typeof(RemoteFormWindowBase.Msg29_Restore), 0, 0);
            _priv_pmsgUse = (Message*)msg29RestorePtr;
        }

        public unsafe void SendRestore()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildRestore(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetMode(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint uMode)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg30_SetMode);
            RemoteFormWindowBase.Msg30_SetMode* msg30SetModePtr = (RemoteFormWindowBase.Msg30_SetMode*)_priv_portUse.AllocMessageBuffer(size);
            msg30SetModePtr->_priv_size = size;
            msg30SetModePtr->_priv_msgid = 30U;
            msg30SetModePtr->uMode = uMode;
            msg30SetModePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg30SetModePtr, ref s_priv_ByteOrder_Msg30_SetMode, typeof(RemoteFormWindowBase.Msg30_SetMode), 0, 0);
            _priv_pmsgUse = (Message*)msg30SetModePtr;
        }

        public unsafe void SendSetMode(uint uMode)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetMode(out _priv_portUse, out _priv_pmsgUse, uMode);
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
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg31_SetVisible);
            RemoteFormWindowBase.Msg31_SetVisible* msg31SetVisiblePtr = (RemoteFormWindowBase.Msg31_SetVisible*)_priv_portUse.AllocMessageBuffer(size);
            msg31SetVisiblePtr->_priv_size = size;
            msg31SetVisiblePtr->_priv_msgid = 31U;
            msg31SetVisiblePtr->fVisible = fVisible ? uint.MaxValue : 0U;
            msg31SetVisiblePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg31SetVisiblePtr, ref s_priv_ByteOrder_Msg31_SetVisible, typeof(RemoteFormWindowBase.Msg31_SetVisible), 0, 0);
            _priv_pmsgUse = (Message*)msg31SetVisiblePtr;
        }

        public unsafe void SendSetVisible(bool fVisible)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetVisible(out _priv_portUse, out _priv_pmsgUse, fVisible);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetPosition(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Point ptScreenPxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg32_SetPosition);
            RemoteFormWindowBase.Msg32_SetPosition* msg32SetPositionPtr = (RemoteFormWindowBase.Msg32_SetPosition*)_priv_portUse.AllocMessageBuffer(size);
            msg32SetPositionPtr->_priv_size = size;
            msg32SetPositionPtr->_priv_msgid = 32U;
            msg32SetPositionPtr->ptScreenPxl = ptScreenPxl;
            msg32SetPositionPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg32SetPositionPtr, ref s_priv_ByteOrder_Msg32_SetPosition, typeof(RemoteFormWindowBase.Msg32_SetPosition), 0, 0);
            _priv_pmsgUse = (Message*)msg32SetPositionPtr;
        }

        public unsafe void SendSetPosition(Point ptScreenPxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetPosition(out _priv_portUse, out _priv_pmsgUse, ptScreenPxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetSize(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size sizeClientPxl)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg33_SetSize);
            RemoteFormWindowBase.Msg33_SetSize* msg33SetSizePtr = (RemoteFormWindowBase.Msg33_SetSize*)_priv_portUse.AllocMessageBuffer(size);
            msg33SetSizePtr->_priv_size = size;
            msg33SetSizePtr->_priv_msgid = 33U;
            msg33SetSizePtr->sizeClientPxl = sizeClientPxl;
            msg33SetSizePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg33SetSizePtr, ref s_priv_ByteOrder_Msg33_SetSize, typeof(RemoteFormWindowBase.Msg33_SetSize), 0, 0);
            _priv_pmsgUse = (Message*)msg33SetSizePtr;
        }

        public unsafe void SendSetSize(Size sizeClientPxl)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetSize(out _priv_portUse, out _priv_pmsgUse, sizeClientPxl);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetCursors(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          int idCursorNormal,
          int idCursorIdle)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg34_SetCursors);
            RemoteFormWindowBase.Msg34_SetCursors* msg34SetCursorsPtr = (RemoteFormWindowBase.Msg34_SetCursors*)_priv_portUse.AllocMessageBuffer(size);
            msg34SetCursorsPtr->_priv_size = size;
            msg34SetCursorsPtr->_priv_msgid = 34U;
            msg34SetCursorsPtr->idCursorNormal = idCursorNormal;
            msg34SetCursorsPtr->idCursorIdle = idCursorIdle;
            msg34SetCursorsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg34SetCursorsPtr, ref s_priv_ByteOrder_Msg34_SetCursors, typeof(RemoteFormWindowBase.Msg34_SetCursors), 0, 0);
            _priv_pmsgUse = (Message*)msg34SetCursorsPtr;
        }

        public unsafe void SendSetCursors(int idCursorNormal, int idCursorIdle)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetCursors(out _priv_portUse, out _priv_pmsgUse, idCursorNormal, idCursorIdle);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetStyles(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint uRst,
          uint uRstEx,
          uint uMin,
          uint uMinEx,
          uint uMax,
          uint uMaxEx,
          uint uFul,
          uint uFulEx)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg35_SetStyles);
            RemoteFormWindowBase.Msg35_SetStyles* msg35SetStylesPtr = (RemoteFormWindowBase.Msg35_SetStyles*)_priv_portUse.AllocMessageBuffer(size);
            msg35SetStylesPtr->_priv_size = size;
            msg35SetStylesPtr->_priv_msgid = 35U;
            msg35SetStylesPtr->uRst = uRst;
            msg35SetStylesPtr->uRstEx = uRstEx;
            msg35SetStylesPtr->uMin = uMin;
            msg35SetStylesPtr->uMinEx = uMinEx;
            msg35SetStylesPtr->uMax = uMax;
            msg35SetStylesPtr->uMaxEx = uMaxEx;
            msg35SetStylesPtr->uFul = uFul;
            msg35SetStylesPtr->uFulEx = uFulEx;
            msg35SetStylesPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg35SetStylesPtr, ref s_priv_ByteOrder_Msg35_SetStyles, typeof(RemoteFormWindowBase.Msg35_SetStyles), 0, 0);
            _priv_pmsgUse = (Message*)msg35SetStylesPtr;
        }

        public unsafe void SendSetStyles(
          uint uRst,
          uint uRstEx,
          uint uMin,
          uint uMinEx,
          uint uMax,
          uint uMaxEx,
          uint uFul,
          uint uFulEx)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetStyles(out _priv_portUse, out _priv_pmsgUse, uRst, uRstEx, uMin, uMinEx, uMax, uMaxEx, uFul, uFulEx);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetMouseIdleOptions(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          Size szMouseIdleTolerance,
          uint dwMouseIdleTimeout)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg36_SetMouseIdleOptions);
            RemoteFormWindowBase.Msg36_SetMouseIdleOptions* mouseIdleOptionsPtr = (RemoteFormWindowBase.Msg36_SetMouseIdleOptions*)_priv_portUse.AllocMessageBuffer(size);
            mouseIdleOptionsPtr->_priv_size = size;
            mouseIdleOptionsPtr->_priv_msgid = 36U;
            mouseIdleOptionsPtr->szMouseIdleTolerance = szMouseIdleTolerance;
            mouseIdleOptionsPtr->dwMouseIdleTimeout = dwMouseIdleTimeout;
            mouseIdleOptionsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)mouseIdleOptionsPtr, ref s_priv_ByteOrder_Msg36_SetMouseIdleOptions, typeof(RemoteFormWindowBase.Msg36_SetMouseIdleOptions), 0, 0);
            _priv_pmsgUse = (Message*)mouseIdleOptionsPtr;
        }

        public unsafe void SendSetMouseIdleOptions(Size szMouseIdleTolerance, uint dwMouseIdleTimeout)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetMouseIdleOptions(out _priv_portUse, out _priv_pmsgUse, szMouseIdleTolerance, dwMouseIdleTimeout);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetOptions(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          uint uOptionsMask,
          uint uOptionsValue)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg37_SetOptions);
            RemoteFormWindowBase.Msg37_SetOptions* msg37SetOptionsPtr = (RemoteFormWindowBase.Msg37_SetOptions*)_priv_portUse.AllocMessageBuffer(size);
            msg37SetOptionsPtr->_priv_size = size;
            msg37SetOptionsPtr->_priv_msgid = 37U;
            msg37SetOptionsPtr->uOptionsMask = uOptionsMask;
            msg37SetOptionsPtr->uOptionsValue = uOptionsValue;
            msg37SetOptionsPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg37SetOptionsPtr, ref s_priv_ByteOrder_Msg37_SetOptions, typeof(RemoteFormWindowBase.Msg37_SetOptions), 0, 0);
            _priv_pmsgUse = (Message*)msg37SetOptionsPtr;
        }

        public unsafe void SendSetOptions(uint uOptionsMask, uint uOptionsValue)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetOptions(out _priv_portUse, out _priv_pmsgUse, uOptionsMask, uOptionsValue);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildCreateRootContainer(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteFormWindowBase.Msg38_CreateRootContainer);
            RemoteFormWindowBase.Msg38_CreateRootContainer* createRootContainerPtr = (RemoteFormWindowBase.Msg38_CreateRootContainer*)_priv_portUse.AllocMessageBuffer(size);
            createRootContainerPtr->_priv_size = size;
            createRootContainerPtr->_priv_msgid = 38U;
            createRootContainerPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)createRootContainerPtr, ref s_priv_ByteOrder_Msg38_CreateRootContainer, typeof(RemoteFormWindowBase.Msg38_CreateRootContainer), 0, 0);
            _priv_pmsgUse = (Message*)createRootContainerPtr;
        }

        public unsafe void SendCreateRootContainer()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateRootContainer(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteFormWindowBase CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteFormWindowBase(port, handle, true);
        }

        public static RemoteFormWindowBase CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteFormWindowBase(port, handle, false);
        }

        protected RemoteFormWindowBase(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteFormWindowBase(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteFormWindowBase && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg7_SetEdgeImageParts
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fActiveEdges;
            public BLOBREF sModuleName;
            public BLOBREF sResourceIdLeft;
            public BLOBREF sResourceIdTop;
            public BLOBREF sResourceIdRight;
            public BLOBREF sResourceIdBottom;
            public Inset insetSplits;
        }

        [ComVisible(false)]
        private struct Msg8_SetInitialPlacement
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nShowState;
            public Rectangle rcNormalPosition;
            public Point ptMaxPosition;
        }

        [ComVisible(false)]
        private struct Msg9_RefreshHitTarget
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg10_ExitInternalDrag
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg11_EnterInternalDrag
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg12_SetDragDropResult
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint nDragOverResult;
            public uint nDragDropResult;
        }

        [ComVisible(false)]
        private struct Msg13_EnableExternalDragDrop
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fEnable;
        }

        [ComVisible(false)]
        private struct Msg14_SetIcon
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF sModule;
            public uint nResourceID;
            public uint nOptions;
        }

        [ComVisible(false)]
        private struct Msg15_SetMaxResizeWidth
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nMaxResizeWidth;
        }

        [ComVisible(false)]
        private struct Msg16_SetMinResizeWidth
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int nMinResizeWidth;
        }

        [ComVisible(false)]
        private struct Msg17_BringToTop
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg18_EnableShellShutdownHook
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF messageToHook;
            public ushort uIdMsg;
        }

        [ComVisible(false)]
        private struct Msg19_UpdateForegroundLockState
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg20_SetAppNotifyWindow
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public HWND hwndAppNotify;
        }

        [ComVisible(false)]
        private struct Msg21_SetHitMasks
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint uMaskTraverse;
            public uint uMaskHit;
            public uint uMaskResult;
            public uint uMaskClip;
        }

        [ComVisible(false)]
        private struct Msg22_SetCapture
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE hVisual;
            public uint fState;
        }

        [ComVisible(false)]
        private struct Msg23_SetText
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public BLOBREF text;
        }

        [ComVisible(false)]
        private struct Msg24_Destroy
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg25_SetForeground
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fForce;
        }

        [ComVisible(false)]
        private struct Msg26_TakeFocus
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg27_ForceMouseIdle
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fIdle;
        }

        [ComVisible(false)]
        private struct Msg28_TemporarilyExitExclusiveMode
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg29_Restore
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg30_SetMode
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint uMode;
        }

        [ComVisible(false)]
        private struct Msg31_SetVisible
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fVisible;
        }

        [ComVisible(false)]
        private struct Msg32_SetPosition
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Point ptScreenPxl;
        }

        [ComVisible(false)]
        private struct Msg33_SetSize
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size sizeClientPxl;
        }

        [ComVisible(false)]
        private struct Msg34_SetCursors
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public int idCursorNormal;
            public int idCursorIdle;
        }

        [ComVisible(false)]
        private struct Msg35_SetStyles
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint uRst;
            public uint uRstEx;
            public uint uMin;
            public uint uMinEx;
            public uint uMax;
            public uint uMaxEx;
            public uint uFul;
            public uint uFulEx;
        }

        [ComVisible(false)]
        private struct Msg36_SetMouseIdleOptions
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public Size szMouseIdleTolerance;
            public uint dwMouseIdleTimeout;
        }

        [ComVisible(false)]
        private struct Msg37_SetOptions
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint uOptionsMask;
            public uint uOptionsValue;
        }

        [ComVisible(false)]
        private struct Msg38_CreateRootContainer
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }
    }
}
