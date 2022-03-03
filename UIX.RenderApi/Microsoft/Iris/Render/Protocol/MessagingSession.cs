// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.MessagingSession
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;
using System.Threading;

namespace Microsoft.Iris.Render.Protocol
{
    internal sealed class MessagingSession : RenderObject
    {
        private static Map<Thread, MessagingSession> s_sessions = new Map<Thread, MessagingSession>();
        private readonly ContextID m_localContextId;
        private bool m_isConnected;
        private int m_pingRequestCount;
        private IRenderHost m_renderHost;
        private Thread m_owningThread;
        private Vector m_ports;
        private Vector m_activeTrackers;
        private RenderPort m_portRendering;
        private RenderPort m_portWindowing;
        private bool m_isBatchingMessages;
        private BatchCallback m_batchFillCallback;
        private BatchCallback m_batchBeginCallbacks;
        private BatchCallback m_batchEndCallbacks;
        private BatchCallback m_batchDeliveryCallbacks;
        private bool m_fBatchFlushRequested;
        private DeferredInvokeItem m_batchFlushInvokeItem;
        private EngineApi.MessageBufferEventHandler m_messageBufferHandler;
        private TimeoutHandler m_externalTimeoutHandler;
        private EngineApi.TimeoutEventHandler m_internalTimeoutHandler;

        internal MessagingSession(IRenderHost renderHost, RenderToken token)
          : this(renderHost, token, null, 0U)
        {
        }

        internal unsafe MessagingSession(
          IRenderHost renderHost,
          RenderToken token,
          TimeoutHandler handlerTimeout,
          uint nTimeoutSec)
        {
            Debug2.Validate(renderHost != null, typeof(ArgumentNullException), nameof(renderHost));
            Debug2.Validate(token != null, typeof(ArgumentNullException), nameof(token));
            Debug2.Validate(token.LocalContextId != ContextID.NULL, typeof(ArgumentNullException), "token.LocalContextId");
            Debug2.Validate(token.EngineInfo is IrisEngineInfo, typeof(ArgumentException), "token.EngineInfo");
            this.m_pingRequestCount = 0;
            this.m_renderHost = renderHost;
            this.m_ports = new Vector(2);
            this.m_localContextId = token.LocalContextId;
            this.m_batchFlushInvokeItem = new DeferredInvokeItem(null, new DeferredHandler(this.ProcessDeferredFlushBatch), null);
            this.m_batchFillCallback = new BatchCallback(this.OnBatchFirstMessage);
            this.InitializeLocalMessaging(this.m_localContextId, new EngineApi.MessageBufferEventHandler(this.OnIncomingMessageBuffer), nTimeoutSec, handlerTimeout);
            this.m_portRendering = this.CreatePort((token.EngineInfo as IrisEngineInfo).ConnectionInfo, token.DestinationContextId, token.RenderGroupId);
            this.m_portWindowing = null;
            this.m_activeTrackers = new Vector();
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    if (this.IsConnected)
                        this.Disconnect();
                    this.DisposeRemainingDataBufferTrackers();
                    foreach (RenderPort port in this.m_ports)
                        port.Dispose();
                    this.m_ports.Clear();
                    this.ShutdownLocalMessaging();
                }
                this.m_activeTrackers = null;
                this.m_portWindowing = null;
                this.m_portRendering = null;
                this.m_ports = null;
                this.m_renderHost = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal static MessagingSession Current
        {
            get
            {
                lock (s_sessions)
                    return s_sessions.ContainsKey(Thread.CurrentThread) ? s_sessions[Thread.CurrentThread] : null;
            }
        }

        internal Thread OwningThread => this.m_owningThread;

        internal bool IsConnected => this.m_isConnected;

        internal ContextID LocalContext => this.m_localContextId;

        internal RenderPort RenderingPort => this.m_portRendering;

        internal RenderPort WindowingPort => this.m_portWindowing == null ? this.m_portRendering : this.m_portWindowing;

        private void InitializeLocalMessaging(
          ContextID localContextId,
          EngineApi.MessageBufferEventHandler messageHandler,
          uint timeoutInSeconds,
          TimeoutHandler timeoutHandler)
        {
            this.m_messageBufferHandler = messageHandler;
            this.m_externalTimeoutHandler = timeoutHandler;
            this.m_internalTimeoutHandler = new EngineApi.TimeoutEventHandler(this.OnTimedOut);
            EngineApi.InitArgs args = new EngineApi.InitArgs(MessageCookieLayout.Default, localContextId, messageHandler);
            if (timeoutInSeconds > 0U && timeoutHandler != null)
            {
                args.pfnTimeout = this.m_internalTimeoutHandler;
                args.nTimeOutSec = timeoutInSeconds;
            }
            EngineApi.IFC(EngineApi.SpInit(ref args));
            lock (s_sessions)
            {
                this.m_owningThread = Thread.CurrentThread;
                s_sessions[this.m_owningThread] = this;
            }
        }

        private void ShutdownLocalMessaging()
        {
            lock (s_sessions)
            {
                s_sessions.Remove(this.m_owningThread);
                this.m_owningThread = null;
            }
            EngineApi.IFC(EngineApi.SpUninit());
            this.m_messageBufferHandler = null;
            this.m_externalTimeoutHandler = null;
            this.m_internalTimeoutHandler = null;
        }

        internal static void DoEvents(uint nTimeoutInMsecs)
        {
            EngineApi.SpWaitMessage(nTimeoutInMsecs, IntPtr.Zero);
            EngineApi.SpPeekMessage(out Win32Api.MSG _, HWND.NULL, 0U, 0U, 1U, out EngineApi.WorkResult _);
        }

        private RenderPort CreatePort(
          ConnectionInfo connectionInfo,
          ContextID destinationContextId,
          RENDERGROUP grpObjects)
        {
            foreach (RenderPort port in this.m_ports)
                Debug2.Validate(port.RemoteContext != destinationContextId, typeof(ArgumentException), "There already is a RenderPort allocated for this destination context");
            bool foreignByteOrder = false;
            IChannel channel;
            switch (connectionInfo)
            {
                case LocalConnectionInfo _:
                    channel = new LocalChannel((LocalConnectionInfo)connectionInfo);
                    break;
                case RemoteConnectionInfo _:
                    RemoteConnectionInfo connectionInfo1 = (RemoteConnectionInfo)connectionInfo;
                    channel = new RemoteChannel(connectionInfo1);
                    foreignByteOrder = connectionInfo1.SwapByteOrder;
                    break;
                default:
                    return null;
            }
            RenderPort renderPort = new RenderPort(this, channel, destinationContextId, grpObjects, MessageCookieLayout.Default, foreignByteOrder);
            if (this.m_isConnected)
                renderPort.Connect();
            this.m_ports.Add(renderPort);
            return renderPort;
        }

        internal DataBufferTracker CreateDataBufferTracker(object objUser) => new DataBufferTracker(objUser);

        internal void ReturnDataBufferTracker(DataBufferTracker tracker)
        {
            if (tracker.Count != 0)
            {
                tracker.TrackerEmpty += new EventHandler(this.OnDataBufferTrackerEmpty);
                this.m_activeTrackers.Add(tracker);
            }
            else
                tracker.Dispose();
        }

        private void OnDataBufferTrackerEmpty(object objSender, EventArgs args)
        {
            DataBufferTracker dataBufferTracker = (DataBufferTracker)objSender;
            this.m_activeTrackers.Remove(dataBufferTracker);
            dataBufferTracker.TrackerEmpty -= new EventHandler(this.OnDataBufferTrackerEmpty);
            dataBufferTracker.Dispose();
        }

        private void DisposeRemainingDataBufferTrackers()
        {
            if (this.m_activeTrackers.Count <= 0)
                return;
            foreach (DataBufferTracker activeTracker in this.m_activeTrackers)
                activeTracker.Dispose();
            this.m_activeTrackers.Clear();
        }

        internal void Connect()
        {
            Debug2.Throw(!this.m_isConnected, "MessagingSession is already connected");
            foreach (RenderPort port in this.m_ports)
                port.Connect();
            this.Synchronize();
            this.StartNewBatch();
            this.m_isConnected = true;
        }

        internal void Disconnect()
        {
            Debug2.Throw(this.m_isConnected, "MessagingSession is not connected");
            this.FlushCurrentBatch(true);
            this.Synchronize();
            foreach (RenderPort port in this.m_ports)
                port.DisposeObjectMaps();
            foreach (RenderPort port in this.m_ports)
                port.Dispose();
            this.m_ports.Clear();
            this.m_isConnected = false;
        }

        internal void Synchronize()
        {
            foreach (RenderPort port in this.m_ports)
            {
                port.PingReply += new EventHandler(this.OnPingReply);
                ++this.m_pingRequestCount;
                port.PingConnection();
            }
            if (this.m_isBatchingMessages)
                this.FlushBatch();
            while (this.m_pingRequestCount > 0)
                DoEvents(uint.MaxValue);
        }

        private void OnPingReply(object sender, EventArgs args)
        {
            (sender as RenderPort).PingReply -= new EventHandler(this.OnPingReply);
            --this.m_pingRequestCount;
        }

        private void StartNewBatch()
        {
            if (this.m_isBatchingMessages || this.m_ports.Count <= 0)
                return;
            foreach (RenderPort port in this.m_ports)
                port.BeginMessageBatch(this.m_batchFillCallback);
            this.m_isBatchingMessages = true;
            if (this.m_batchBeginCallbacks == null)
                return;
            this.m_batchBeginCallbacks();
            this.m_batchBeginCallbacks = null;
        }

        internal void FlushBatch() => this.FlushCurrentBatch(false);

        private void FlushCurrentBatch(bool stopBatching)
        {
            if (!this.m_isBatchingMessages)
                return;
            if (this.m_batchEndCallbacks != null)
            {
                this.m_batchEndCallbacks();
                this.m_batchEndCallbacks = null;
            }
            this.m_isBatchingMessages = false;
            BatchCallback deliveryCallbacks = this.m_batchDeliveryCallbacks;
            this.m_batchDeliveryCallbacks = null;
            foreach (RenderPort port in this.m_ports)
                port.EndMessageBatch(deliveryCallbacks);
            if (stopBatching)
                return;
            this.StartNewBatch();
        }

        internal void ScheduleBatchFlush()
        {
            if (this.m_fBatchFlushRequested)
                return;
            this.AssertOwningThread();
            this.DeferredInvoke(this.m_batchFlushInvokeItem, DeferredInvokePriority.Low);
            this.m_fBatchFlushRequested = true;
        }

        internal void ProcessDeferredFlushBatch(object args)
        {
            this.AssertOwningThread();
            if (!this.m_fBatchFlushRequested)
                return;
            this.FlushBatch();
            this.m_fBatchFlushRequested = false;
        }

        public void NotifyOnBeginBatch(BatchCallback newCallback)
        {
            if (!this.m_isBatchingMessages)
                return;
            this.m_batchBeginCallbacks += newCallback;
        }

        public void NotifyOnEndBatch(BatchCallback newCallback, bool notifyEmptyBatches)
        {
            if (!this.m_isBatchingMessages)
                return;
            if (notifyEmptyBatches)
                this.ScheduleBatchFlush();
            this.m_batchEndCallbacks += newCallback;
        }

        public void NotifyOnCurrentBatchDelivered(BatchCallback newCallback)
        {
            if (!this.m_isBatchingMessages)
                return;
            this.m_batchDeliveryCallbacks += newCallback;
        }

        private void OnBatchFirstMessage() => this.ScheduleBatchFlush();

        private unsafe int OnIncomingMessageBuffer(
          IntPtr pData,
          uint hContext,
          EngineApi.BufferInfo* pBufferInfo,
          void* pvBufferData)
        {
            RenderPort renderPort = null;
            for (int index = 0; index < this.m_ports.Count; ++index)
            {
                RenderPort port = (RenderPort)this.m_ports[index];
                if (port.RemoteContext == pBufferInfo->idContextSrc)
                {
                    renderPort = port;
                    break;
                }
            }
            Debug2.Throw(renderPort != null, "Expected a render port if we are receiving buffers");
            renderPort.ProcessMessageBuffer(pBufferInfo, pvBufferData);
            return 0;
        }

        private void OnTimedOut(IntPtr pData)
        {
            if (this.m_externalTimeoutHandler == null)
                return;
            this.m_externalTimeoutHandler();
        }

        internal void AssertOwningThread()
        {
        }

        internal bool IsOwningThread() => this.m_owningThread == Thread.CurrentThread;

        internal void DeferredInvoke(Delegate method, object args, DeferredInvokePriority priority) => this.DeferredInvoke(method, args, priority, TimeSpan.FromMilliseconds(0.0));

        internal void DeferredInvoke(
          SharedRenderObject owner,
          Delegate method,
          object args,
          DeferredInvokePriority priority)
        {
            this.DeferredInvoke(owner, method, args, priority, TimeSpan.FromMilliseconds(0.0));
        }

        internal void DeferredInvoke(
          Delegate method,
          object args,
          DeferredInvokePriority priority,
          TimeSpan delay)
        {
            this.DeferredInvoke(null, method, args, priority, delay);
        }

        internal void DeferredInvoke(
          SharedRenderObject instanceOwner,
          Delegate method,
          object args,
          DeferredInvokePriority priority,
          TimeSpan delay)
        {
            if (this.m_owningThread == null)
                return;
            DeferredInvokeItem deferredInvokeItem = new DeferredInvokeItem(instanceOwner, method, args);
            this.m_renderHost.DeferredInvoke(priority, deferredInvokeItem, delay);
        }

        private void DeferredInvoke(DeferredInvokeItem item, DeferredInvokePriority priority)
        {
            if (this.m_owningThread == null)
                return;
            this.m_renderHost.DeferredInvoke(priority, item, TimeSpan.FromMilliseconds(0.0));
        }

        protected override void Invariant()
        {
            Debug2.Validate(this.m_owningThread != null, typeof(InvalidOperationException), this.GetType().ToString());
            Debug2.Validate(this.m_renderHost != null, typeof(InvalidOperationException), this.GetType().ToString());
            Debug2.Validate(this.m_isConnected, typeof(InvalidOperationException), this.GetType().ToString());
        }
    }
}
