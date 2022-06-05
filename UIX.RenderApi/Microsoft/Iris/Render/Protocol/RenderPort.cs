// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.RenderPort
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocols.Splash.Messaging;
using System;
using System.Collections;

namespace Microsoft.Iris.Render.Protocol
{
    internal class RenderPort : IRenderPortCallback, IRenderHandleOwner
    {
        private const uint c_maxCachedBatchSendInfos = 25;
        private MessagingSession _session;
        private IChannel _channel;
        private ContextID _idRemoteContext;
        private ProtocolSplashMessaging _protocolMessaging;
        private ArrayList _protocols = new ArrayList();
        private HandleTable _handleTable;
        private Map<RENDERGROUP, WeakReference> _groupProxies;
        private MessageCookieLayout _layoutOfRenderHandles;
        private RENDERGROUP _defaultGroupId;
        private RENDERHANDLE _rootHandle;
        private ArrayList _callbackHandlers = new ArrayList();
        private MessageHeap _currentMessageHeap;
        private ObjectCache _spareHeapCache;
        private Map<MessageHeap, MessageHeap> _activeHeaps;
        private uint _nextBatchId;
        private RenderPort.BatchSendInfo _sentBatchesHead;
        private RenderPort.BatchSendInfo _sentBatchesTail;
        private RenderPort.BatchSendInfo _freeBatchSendInfos;
        private uint _numFreeBatchSendInfos;
        private unsafe MessageBatchEntry* _lastBatchEntry;
        private uint _lastBatchBlockId;
        private RENDERHANDLE[] _pendingBatchDeletes;
        private uint _pendingBatchDeleteSlot;
        private RENDERGROUP[] _pendingBatchGroupDeletes;
        private uint _pendingBatchGroupDeleteSlot;
        private BatchCallback _batchPopulateCallback;
        private bool _individualMessageMode;
        private uint _uBatchedMessages;
        private DeferredHandler _deferredObjectReleaseWorker;
        private DeferredHandler _deferredGroupReleaseWorker;
        private DeferredHandler _deferredHeapReleaseWorker;
        private DeferredHandler _deferredTrafficUpdate;
        private bool _foreignByteOrder;
        private static object s_SyncLock = new object();
        private static ushort[] s_ByteOrder_CallbackMessage;
        private static ushort[] s_ByteOrder_MessageBatchHeader;
        private static ushort[] s_ByteOrder_MessageBatchEntry;
        private ulong _ulOverallControlTraffic;
        private uint _uRecentControlTraffic;
        private ulong _ulOverallDataTraffic;
        private uint _uRecentDataTraffic;
        private ulong _ulOverallTotalTraffic;
        private DateTime _dtRecentTrafficTime;
        private bool _fPendingTrafficSummary;
        private static TimeSpan s_timeTrafficWindow = TimeSpan.FromMilliseconds(500.0);

        internal RenderPort(
          MessagingSession session,
          IChannel channel,
          ContextID idRemoteContext,
          RENDERGROUP idGroup,
          MessageCookieLayout layout,
          bool foreignByteOrder)
        {
            Debug2.Validate(session != null, typeof(ArgumentNullException), nameof(session));
            Debug2.Validate(channel != null, typeof(ArgumentNullException), nameof(channel));
            Debug2.Validate(idRemoteContext != ContextID.NULL, typeof(ArgumentNullException), nameof(idRemoteContext));
            this._deferredObjectReleaseWorker = new DeferredHandler(this.DeferredObjectReleaseWorker);
            this._deferredGroupReleaseWorker = new DeferredHandler(this.DeferredGroupReleaseWorker);
            this._deferredHeapReleaseWorker = new DeferredHandler(this.DeferredHeapReleaseWorker);
            this._deferredTrafficUpdate = new DeferredHandler(this.DeferredTrafficUpdate);
            this._activeHeaps = new Map<MessageHeap, MessageHeap>();
            this._spareHeapCache = new ObjectCache(new ObjectCache.Callback(this.HeapCacheCallback), 10, false);
            this._individualMessageMode = true;
            this._layoutOfRenderHandles = layout;
            this._handleTable = new HandleTable(this, this._layoutOfRenderHandles);
            this._defaultGroupId = this.AllocHandleGroup(false);
            this._rootHandle = this.AllocHandle(this);
            if (idGroup != RENDERGROUP.NULL)
            {
                this.AllocHandleGroupWithId(idGroup);
                this._defaultGroupId = idGroup;
            }
            this._channel = channel;
            this._session = session;
            this._foreignByteOrder = foreignByteOrder;
            this._idRemoteContext = idRemoteContext;
            this._protocolMessaging = ProtocolSplashMessaging.Bind(this);
        }

        public void Connect()
        {
            Debug2.Throw(this._handleTable != null && this._channel != null, "must be valid to connect");
            Debug2.Throw(!this._channel.IsConnected, "already connected");
            this._channel.Connect(this._idRemoteContext, this._rootHandle, this._layoutOfRenderHandles);
            foreach (ProtocolInstance protocol in this._protocols)
                protocol.OnConnect();
        }

        internal void PingConnection() => LocalRenderPortCallback.AutoSendOnPingReply(this._protocolMessaging, RENDERHANDLE.NULL);

        internal event EventHandler PingReply;

        public void Dispose()
        {
            this.PingReply = null;
            this.DisposeObjectMaps();
            if (this._channel != null)
            {
                this._channel.Dispose();
                this._channel = null;
            }
            if (this._spareHeapCache == null)
                return;
            if (this._currentMessageHeap != null)
                this.ReleaseHeap(this._currentMessageHeap);
            this._currentMessageHeap = null;
            this._spareHeapCache.Dispose();
            this._spareHeapCache = null;
        }

        public void DisposeObjectMaps()
        {
            if (this._handleTable == null)
                return;
            this._handleTable.Dispose();
            this._handleTable = null;
        }

        public MessagingSession Session => this._session;

        public ContextID RemoteContext => this._idRemoteContext;

        public bool ForeignByteOrder => this._foreignByteOrder;

        public bool AlwaysBigEndian => this._foreignByteOrder;

        internal uint DefaultGroupID => RENDERGROUP.ToUInt32(this._defaultGroupId);

        RENDERHANDLE IRenderHandleOwner.RenderHandle => RENDERHANDLE.NULL;

        void IRenderHandleOwner.OnDisconnect()
        {
        }

        internal ProtocolSplashMessaging MessagingProtocol => this._protocolMessaging;

        public ProtocolInstance LookUpProtocol(string name)
        {
            ProtocolInstance protocolInstance = null;
            foreach (ProtocolInstance protocol in this._protocols)
            {
                if (protocol.Name == name)
                    protocolInstance = protocol;
            }
            return protocolInstance;
        }

        internal void BindProtocol(ProtocolInstance protocol)
        {
            Debug2.Validate(this.LookUpProtocol(protocol.Name) == null, typeof(InvalidOperationException), nameof(protocol), "protocol already bound");
            this._protocols.Add(protocol);
            if (this._channel == null || !this._channel.IsConnected)
                return;
            protocol.OnConnect();
        }

        internal RENDERHANDLE InitRemoteClass(string className)
        {
            if (className.Equals("Splash::Messaging::Broker"))
                return this._rootHandle;
            RENDERHANDLE idObjectClass = this.AllocHandle(this);
            RemoteBroker.SendCreateClass(this._protocolMessaging, className, idObjectClass);
            return idObjectClass;
        }

        internal unsafe void CreateRemoteObject(
          RENDERHANDLE classCreate,
          RENDERHANDLE hNewObject,
          Message* pmsgConstructor)
        {
            RemoteBroker.SendCreateObject(this._protocolMessaging, classCreate, hNewObject, pmsgConstructor);
        }

        internal unsafe void SendLocalCallbackMessage(Message* pmsgCallback) => Protocols.Splash.Messaging.RemoteContext.SendForwardMessage(this._protocolMessaging, this._session.LocalContext, pmsgCallback);

        internal void LinkContext(ContextID idContextExisting, ContextID idContextAlias) => RemoteContextRelay.SendLinkContext(this._protocolMessaging, idContextExisting, idContextAlias);

        internal void UnlinkContext(ContextID idContextExisting, ContextID idContextAlias) => RemoteContextRelay.SendUnlinkContext(this._protocolMessaging, idContextExisting, idContextAlias);

        public void BeginMessageBatch(BatchCallback populateCallback)
        {
            this.Session.AssertOwningThread();
            Debug2.Throw(this._individualMessageMode, "Already in batch mode");
            this._batchPopulateCallback = populateCallback;
            this._individualMessageMode = false;
        }

        public void EndMessageBatch(BatchCallback deliveryCallback)
        {
            this.Session.AssertOwningThread();
            Debug2.Throw(!this._individualMessageMode, "Must be collecting a batch to end");
            this.SendCurrentBatch(deliveryCallback);
            this._individualMessageMode = true;
        }

        public unsafe void SendDataBuffer(void* pvData, uint cbSize, RENDERHANDLE idRemoteBuffer)
        {
            Debug2.Validate(idRemoteBuffer != RENDERHANDLE.NULL, typeof(ArgumentNullException), nameof(idRemoteBuffer));
            this.UpdateTraffic(0U, cbSize);
            var info = new EngineApi.BufferInfo
            {
                idContextSrc = this._session.LocalContext,
                idContextDest = this._idRemoteContext,
                idBuffer = idRemoteBuffer,
                nFlags = 0,
                cbSizeBuffer = cbSize
            };
            EngineApi.IFC(EngineApi.SpBufferOpen(&info, pvData));
        }

        public unsafe void SendBatchBuffer(void* pvData, uint cbSize, RENDERHANDLE idRemoteBuffer)
        {
            int num = idRemoteBuffer != RENDERHANDLE.NULL ? 1 : 0;
            this.UpdateTraffic(cbSize, 0U);
            var info = new EngineApi.BufferInfo()
            {
                idContextSrc = this._session.LocalContext,
                idContextDest = this._idRemoteContext,
                idBuffer = idRemoteBuffer,
                nFlags = EngineApi.BufferFlags.IsBatch,
                cbSizeBuffer = cbSize
            };
            EngineApi.IFC(EngineApi.SpBufferOpen(&info, pvData));
        }

        public unsafe void SendMessageBuffer(Message* pmsg)
        {
            this.UpdateTraffic(pmsg->cbSize, 0U);
            uint src = pmsg->cbSize;
            if (this.ForeignByteOrder)
                src = Memory.ConvertEndian(src);
            var info = new EngineApi.BufferInfo()
            {
                idContextSrc = this._session.LocalContext,
                idContextDest = this._idRemoteContext,
                idBuffer = RENDERHANDLE.NULL,
                nFlags = EngineApi.BufferFlags.CopyData,
                cbSizeBuffer = src
            };
            EngineApi.IFC(EngineApi.SpBufferOpen(&info, pmsg));
        }

        private void UpdateTraffic(uint uControl, uint uData)
        {
            this._uRecentControlTraffic += uControl;
            this._uRecentDataTraffic += uData;
            this._dtRecentTrafficTime = DateTime.UtcNow;
            if (Debug2.IsCategoryEnabled(DebugCategory.Protocol, 1))
            {
                if (this._fPendingTrafficSummary || !this.Session.IsConnected)
                    return;
                this.Session.DeferredInvoke(_deferredTrafficUpdate, null, DeferredInvokePriority.Idle, s_timeTrafficWindow);
                this._fPendingTrafficSummary = true;
            }
            else
                this.TrafficUpdateWorker();
        }

        private void DeferredTrafficUpdate(object obj)
        {
            if (!this._fPendingTrafficSummary)
                return;
            if (DateTime.UtcNow - this._dtRecentTrafficTime < s_timeTrafficWindow)
            {
                this.Session.DeferredInvoke(_deferredTrafficUpdate, null, DeferredInvokePriority.Idle, this._dtRecentTrafficTime + s_timeTrafficWindow - DateTime.UtcNow);
            }
            else
            {
                this._fPendingTrafficSummary = false;
                this.TrafficUpdateWorker();
            }
        }

        private void TrafficUpdateWorker()
        {
            this._ulOverallControlTraffic += _uRecentControlTraffic;
            this._ulOverallDataTraffic += _uRecentDataTraffic;
            this._ulOverallTotalTraffic += this._uRecentControlTraffic + this._uRecentDataTraffic;
            Debug2.IsCategoryEnabled(DebugCategory.Protocol, 1);
            this._uRecentControlTraffic = 0U;
            this._uRecentDataTraffic = 0U;
        }

        public unsafe void* AllocMessageBuffer(uint size)
        {
            this.Session.AssertOwningThread();
            if (this._currentMessageHeap == null)
                this._currentMessageHeap = this.AllocHeap();
            return this._currentMessageHeap.Alloc(size);
        }

        public unsafe void SendRemoteMessage(Message* pmsg)
        {
            this.Session.AssertOwningThread();
            Debug2.Throw(this._channel != null, "Not connected to renderer");
            if (this._individualMessageMode)
            {
                this.SendMessageBuffer(pmsg);
                if (this._currentMessageHeap == null)
                    return;
                this._currentMessageHeap.Reset();
            }
            else
                this.StoreMessageEntry(pmsg);
        }

        public RENDERHANDLE AllocHandle(IRenderHandleOwner owner)
        {
            Debug2.Throw(this._handleTable != null, "Render port has already shut down");
            Debug2.Validate(owner != null, typeof(ArgumentNullException), nameof(owner));
            return this._handleTable.Alloc(this._defaultGroupId, owner);
        }

        public void RegisterKnownHandle(IRenderHandleOwner owner, RENDERHANDLE handle)
        {
            Debug2.Throw(this._handleTable != null, "Render port has already shut down");
            Debug2.Validate(owner != null, typeof(ArgumentNullException), nameof(owner));
            this._handleTable.AllocWithId(owner, handle, true);
        }

        public void FreeHandle(RENDERHANDLE handle)
        {
            if (this.Session.IsOwningThread())
            {
                if (this._handleTable == null)
                    return;
                if (this._channel != null && this._handleTable.GetGroup(handle) == this._defaultGroupId)
                {
                    this._handleTable.SetOwner(handle, null);
                    this.AddPendingDelete(handle);
                    RemoteBroker.SendDestroyObject(this._protocolMessaging, handle);
                }
                else
                    this._handleTable.Free(handle);
            }
            else
                this.Session.DeferredInvoke(_deferredObjectReleaseWorker, handle, DeferredInvokePriority.High);
        }

        public void CancelAllocHandle(RENDERHANDLE handle)
        {
            if (this._handleTable == null)
                return;
            this._handleTable.Free(handle);
        }

        public IRenderHandleOwner TryGetHandleOwner(RENDERHANDLE handle) => this._handleTable.GetOwner(handle, true);

        public IRenderHandleOwner TryGetProxyOwner(RENDERHANDLE handle)
        {
            IRenderHandleOwner renderHandleOwner = null;
            if (this._groupProxies != null)
            {
                RENDERGROUP group = this._handleTable.GetGroup(handle);
                if (group != RENDERGROUP.NULL)
                    renderHandleOwner = this.GetGroupProxy(group);
            }
            return renderHandleOwner;
        }

        public IRenderHandleOwner GetHandleOwner(RENDERHANDLE handle) => this._handleTable.GetOwner(handle);

        public void ValidateHandle(RENDERHANDLE handle) => this._handleTable.Validate(handle);

        public void ValidateHandleOrNull(RENDERHANDLE handle)
        {
            if (!(handle != RENDERHANDLE.NULL))
                return;
            this._handleTable.Validate(handle);
        }

        public RENDERGROUP AllocHandleGroup() => this.AllocHandleGroup(true);

        public void AllocHandleGroupWithId(RENDERGROUP idGroup) => this._handleTable.AllocGroupWithId(idGroup);

        private RENDERGROUP AllocHandleGroup(bool failIfNotConnected)
        {
            RENDERGROUP handle = this._handleTable.AllocGroup();
            if (this._channel != null)
                Protocols.Splash.Messaging.RemoteContext.SendCreateGroup(this._protocolMessaging, (int)RENDERGROUP.ToUInt32(handle), this._session.LocalContext);
            else if (failIfNotConnected)
                Debug2.Throw(false, "Not connected to renderer");
            return handle;
        }

        public void FreeHandleGroup(RENDERGROUP group)
        {
            if (this.Session.IsOwningThread())
            {
                if (this._handleTable == null)
                    return;
                if (this._channel != null)
                {
                    this.AddPendingGroupDelete(group);
                    Protocols.Splash.Messaging.RemoteContext.SendDestroyGroup(this._protocolMessaging, (int)RENDERGROUP.ToUInt32(group));
                }
                else
                    this._handleTable.FreeGroup(group);
            }
            else
                this.Session.DeferredInvoke(_deferredGroupReleaseWorker, group, DeferredInvokePriority.High);
        }

        public void RegisterGroupProxy(RENDERGROUP group, IRenderHandleOwner proxy)
        {
            if (this._groupProxies == null)
                this._groupProxies = new Map<RENDERGROUP, WeakReference>();
            this._groupProxies[group] = new WeakReference(proxy);
        }

        public void RevokeGroupProxy(RENDERGROUP group)
        {
            if (this._groupProxies == null)
                return;
            this._groupProxies[group] = null;
        }

        public IRenderHandleOwner GetGroupProxy(RENDERGROUP group)
        {
            IRenderHandleOwner renderHandleOwner = null;
            if (this._groupProxies != null)
            {
                WeakReference groupProxy = this._groupProxies[group];
                if (groupProxy != null)
                    renderHandleOwner = groupProxy.Target as IRenderHandleOwner;
            }
            return renderHandleOwner;
        }

        public MessageHeap AllocHeap()
        {
            this.Session.AssertOwningThread();
            return (MessageHeap)this._spareHeapCache.Pop();
        }

        public void ReleaseHeap(MessageHeap heap)
        {
            if (this.Session.IsOwningThread())
                this.DeferredHeapReleaseWorker(heap);
            else if (this.Session.OwningThread != null)
                this.Session.DeferredInvoke(_deferredHeapReleaseWorker, heap, DeferredInvokePriority.High);
            else
                heap.Dispose();
        }

        private unsafe void StoreMessageEntry(Message* pMethodMsg)
        {
            MessageHeap.Block block = this._currentMessageHeap.LookupBlock(pMethodMsg);
            Debug2.Throw((IntPtr)block.data != IntPtr.Zero, "message should be in current message heap");
            uint num = (uint)((ulong)((sbyte*)pMethodMsg - (sbyte*)block.data) - (ulong)sizeof(MessageBatchEntry));
            if ((IntPtr)_lastBatchEntry != IntPtr.Zero && _lastBatchBlockId == block.id)
                this._lastBatchEntry->uOffsetNextEntry = num;
            else
                ((MessageBatchHeader*)block.data)->uOffsetFirstEntry = num;
            this._lastBatchEntry = (MessageBatchEntry*)block.data + num;
            this._lastBatchBlockId = block.id;
            ++this._uBatchedMessages;
            BatchCallback populateCallback = this._batchPopulateCallback;
            if (populateCallback == null)
                return;
            this._batchPopulateCallback = null;
            populateCallback();
        }

        private unsafe void SendCurrentBatch(BatchCallback deliveryCallback)
        {
            try
            {
                if ((IntPtr)this._lastBatchEntry != IntPtr.Zero)
                {
                    uint nextBatchId = this._nextBatchId;
                    ++this._nextBatchId;
                    try
                    {
                        LocalRenderPortCallback.AutoSendOnBatchProcessed(this._protocolMessaging, RENDERHANDLE.NULL, nextBatchId);
                        RenderPort.BatchSendInfo info = this.AllocBatchSendInfo();
                        info.batchID = nextBatchId;
                        info.heap = this._currentMessageHeap;
                        info.pendingDeletes = this._pendingBatchDeletes;
                        info.pendingGroupDeletes = this._pendingBatchGroupDeletes;
                        info.deliveryCallback = deliveryCallback;
                        this.EnqueuePendingBatch(info);
                        RENDERHANDLE renderhandle = RENDERHANDLE.NULL;
                        int blockCount = this._currentMessageHeap.BlockCount;
                        uint num = 0;
                        MessageHeap.Enumerator enumerator = this._currentMessageHeap.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            MessageHeap.Block current = enumerator.Current;
                            MessageBatchHeader* data = (MessageBatchHeader*)current.data;
                            RENDERHANDLE idRemoteBuffer = RENDERHANDLE.NULL;
                            --blockCount;
                            if (blockCount > 0)
                                idRemoteBuffer = this.AllocHandle(this);
                            data->idPredicateBuffer = renderhandle;
                            if (this.ForeignByteOrder)
                                SwapBatchListByteOrder(data);
                            num += current.size;
                            this.SendBatchBuffer(current.data, current.size, idRemoteBuffer);
                            renderhandle = idRemoteBuffer;
                        }
                        this._currentMessageHeap = null;
                        this._pendingBatchDeletes = null;
                        this._pendingBatchDeleteSlot = 0U;
                        this._pendingBatchGroupDeletes = null;
                        this._pendingBatchGroupDeleteSlot = 0U;
                    }
                    finally
                    {
                        if (this._currentMessageHeap != null)
                        {
                            this.DequeuePendingBatch(nextBatchId, true);
                            this.ReleaseHeap(this._currentMessageHeap);
                            this._currentMessageHeap = null;
                        }
                    }
                }
                else
                {
                    if (this._currentMessageHeap != null)
                    {
                        this.ReleaseHeap(this._currentMessageHeap);
                        this._currentMessageHeap = null;
                    }
                    if (deliveryCallback == null)
                        return;
                    deliveryCallback();
                }
            }
            finally
            {
                this._currentMessageHeap = null;
                this._batchPopulateCallback = null;
                this._lastBatchEntry = null;
                this._lastBatchBlockId = 0U;
                this._uBatchedMessages = 0U;
            }
        }

        private static unsafe void SwapBatchListByteOrder(MessageBatchHeader* pHeader)
        {
            byte* pMem1 = (byte*)pHeader;
            uint num = pHeader->uOffsetFirstEntry;
            MarshalHelper.SwapByteOrder(pMem1, ref s_ByteOrder_MessageBatchHeader, typeof(MessageBatchHeader), 0, 0);
            while (num != 0U)
            {
                byte* pMem2 = pMem1 + (int)num;
                num = ((MessageBatchEntry*)pMem2)->uOffsetNextEntry;
                MarshalHelper.SwapByteOrder(pMem2, ref s_ByteOrder_MessageBatchEntry, typeof(MessageBatchEntry), 0, 0);
            }
        }

        private void DeferredObjectReleaseWorker(object args) => this.FreeHandle((RENDERHANDLE)args);

        private void DeferredGroupReleaseWorker(object args) => this.FreeHandleGroup((RENDERGROUP)args);

        private void DeferredHeapReleaseWorker(object args) => this._spareHeapCache.Push((MessageHeap)args);

        private unsafe object HeapCacheCallback(ObjectCache.Operation operation, object data)
        {
            this.Session.AssertOwningThread();
            MessageHeap key = data as MessageHeap;
            switch (operation)
            {
                case ObjectCache.Operation.Alloc:
                    key = new MessageHeap(8U * Win32Api.GetSystemPageSize(), (uint)sizeof(MessageBatchHeader), (uint)sizeof(MessageBatchEntry));
                    this._activeHeaps[key] = key;
                    break;
                case ObjectCache.Operation.Free:
                    this._activeHeaps.Remove(key);
                    key.Dispose();
                    key = null;
                    break;
                case ObjectCache.Operation.Freeze:
                    Debug2.Validate(key != null, typeof(ArgumentNullException), nameof(data));
                    key.Reset();
                    break;
            }
            return key;
        }

        internal unsafe void ProcessMessageBuffer(EngineApi.BufferInfo* pBufferInfo, void* pvBufferData)
        {
            if (this.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)pvBufferData, ref s_ByteOrder_CallbackMessage, typeof(CallbackMessage), 0, 0);
            CallbackMessage* message = (CallbackMessage*)pvBufferData;
            this.DispatchCallbackMessage(RENDERHANDLE.ToUInt32(message->idObjectSubject), message);
        }

        internal RENDERHANDLE RegisterCallback(PortCallback callback, out uint idCallback)
        {
            idCallback = (uint)this._callbackHandlers.Add(callback);
            return RENDERHANDLE.FromUInt32(idCallback + 2U);
        }

        private unsafe void DispatchCallbackMessage(uint idCallback, CallbackMessage* message)
        {
            idCallback -= 2U;
            if (idCallback >= (uint)this._callbackHandlers.Count)
                return;
            PortCallback callbackHandler = (PortCallback)this._callbackHandlers[(int)idCallback];
            if (callbackHandler == null)
                return;
            IRenderHandleOwner owner = this;
            if (message->hTarget != RENDERHANDLE.NULL)
            {
                if (this._handleTable == null)
                    return;
                owner = this.TryGetHandleOwner(message->hTarget);
                if (owner == null)
                    return;
            }
            callbackHandler(this, owner, message);
        }

        void IRenderPortCallback.OnPingReply(RENDERHANDLE target)
        {
            if (this.PingReply == null)
                return;
            this.PingReply(this, EventArgs.Empty);
        }

        void IRenderPortCallback.OnBatchProcessed(
          RENDERHANDLE target,
          uint uBatchCompleted)
        {
            RenderPort.BatchSendInfo info = this.DequeuePendingBatch(uBatchCompleted, false);
            if (info == null)
                return;
            this.ProcessPendingDeletes(info.pendingDeletes);
            this.ProcessPendingGroupDeletes(info.pendingGroupDeletes);
            this.ReleaseHeap(info.heap);
            if (info.deliveryCallback != null)
                info.deliveryCallback();
            this.FreeBatchSendInfo(info);
        }

        private void AddPendingDelete(RENDERHANDLE handle)
        {
            uint num1 = 0;
            if (this._pendingBatchDeletes != null)
                num1 = (uint)this._pendingBatchDeletes.Length;
            if (this._pendingBatchDeleteSlot >= num1)
            {
                uint num2 = 2U * num1;
                if (num2 == 0U)
                    num2 = 16U;
                RENDERHANDLE[] renderhandleArray = new RENDERHANDLE[num2];
                if (num1 > 0U)
                    Array.Copy(_pendingBatchDeletes, renderhandleArray, this._pendingBatchDeletes.Length);
                this._pendingBatchDeletes = renderhandleArray;
            }
            this._pendingBatchDeletes[this._pendingBatchDeleteSlot] = handle;
            ++this._pendingBatchDeleteSlot;
        }

        private void ProcessPendingDeletes(RENDERHANDLE[] deleteList)
        {
            if (deleteList == null || this._handleTable == null)
                return;
            foreach (RENDERHANDLE delete in deleteList)
            {
                if (delete != RENDERHANDLE.NULL)
                    this._handleTable.Free(delete);
            }
        }

        private void AddPendingGroupDelete(RENDERGROUP group)
        {
            uint num1 = 0;
            if (this._pendingBatchGroupDeletes != null)
                num1 = (uint)this._pendingBatchGroupDeletes.Length;
            if (this._pendingBatchGroupDeleteSlot >= num1)
            {
                uint num2 = 2U * num1;
                if (num2 == 0U)
                    num2 = 16U;
                RENDERGROUP[] rendergroupArray = new RENDERGROUP[num2];
                if (num1 > 0U)
                    Array.Copy(_pendingBatchGroupDeletes, rendergroupArray, this._pendingBatchGroupDeletes.Length);
                this._pendingBatchGroupDeletes = rendergroupArray;
            }
            this._pendingBatchGroupDeletes[this._pendingBatchGroupDeleteSlot] = group;
            ++this._pendingBatchGroupDeleteSlot;
        }

        private void ProcessPendingGroupDeletes(RENDERGROUP[] deleteList)
        {
            if (deleteList == null || this._handleTable == null)
                return;
            foreach (RENDERGROUP delete in deleteList)
            {
                if (delete != RENDERGROUP.NULL)
                    this._handleTable.FreeGroup(delete);
            }
        }

        private RenderPort.BatchSendInfo AllocBatchSendInfo()
        {
            RenderPort.BatchSendInfo batchSendInfo;
            if (this._freeBatchSendInfos != null)
            {
                batchSendInfo = this._freeBatchSendInfos;
                this._freeBatchSendInfos = batchSendInfo.next;
                batchSendInfo.next = null;
                --this._numFreeBatchSendInfos;
            }
            else
                batchSendInfo = new RenderPort.BatchSendInfo();
            return batchSendInfo;
        }

        private void FreeBatchSendInfo(RenderPort.BatchSendInfo info)
        {
            info.Reset();
            if (this._numFreeBatchSendInfos >= 25U)
                return;
            info.next = this._freeBatchSendInfos;
            this._freeBatchSendInfos = info;
            ++this._numFreeBatchSendInfos;
        }

        private void EnqueuePendingBatch(RenderPort.BatchSendInfo info)
        {
            if (this._sentBatchesTail == null)
            {
                this._sentBatchesHead = info;
                this._sentBatchesTail = info;
            }
            else
            {
                this._sentBatchesTail.next = info;
                this._sentBatchesTail = info;
            }
        }

        private RenderPort.BatchSendInfo DequeuePendingBatch(uint batchID, bool fFreeInfo)
        {
            RenderPort.BatchSendInfo batchSendInfo = null;
            RenderPort.BatchSendInfo info;
            for (info = this._sentBatchesHead; info != null && (int)info.batchID != (int)batchID; info = info.next)
                batchSendInfo = info;
            if (info != null)
            {
                if (this._sentBatchesTail == info)
                    this._sentBatchesTail = batchSendInfo;
                if (batchSendInfo == null)
                    this._sentBatchesHead = info.next;
                else
                    batchSendInfo.next = info.next;
                info.next = null;
                if (fFreeInfo)
                {
                    this.FreeBatchSendInfo(info);
                    info = null;
                }
            }
            return info;
        }

        public static object SyncLock => s_SyncLock;

        private class BatchSendInfo
        {
            public uint batchID;
            public MessageHeap heap;
            public RENDERHANDLE[] pendingDeletes;
            public RENDERGROUP[] pendingGroupDeletes;
            public BatchCallback deliveryCallback;
            public RenderPort.BatchSendInfo next;

            public void Reset()
            {
                this.batchID = 0U;
                this.heap = null;
                this.pendingDeletes = null;
                this.pendingGroupDeletes = null;
                this.deliveryCallback = null;
                this.next = null;
            }
        }
    }
}
