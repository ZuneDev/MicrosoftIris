// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.UIDispatcher
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Queues;
using Microsoft.Iris.Render;
using System;
using System.Threading;

namespace Microsoft.Iris.Session
{
    internal class UIDispatcher : Dispatcher, IDisposable, IRenderHost
    {
        private static Thread s_mainUIThread;
        private static bool s_exiting;
        private UISession _parentSession;
        private Thread _thread;
        private PriorityQueue _masterQueue;
        private TimeoutManager _timeoutManager;
        private Queue _rpcYieldQueue;
        private Queue _cleanupQueue;
        private UIDispatcher.MessageLoop _messageLoop;
        private bool _respectNativeQuitRequests;
        private PriorityQueue.HookProc _doBatchFlush;
        private bool _shutdown;

        internal UIDispatcher(bool isMainUIThread)
          : this(null, null, 0U, isMainUIThread)
        {
        }

        internal UIDispatcher(
          UISession parentSession,
          TimeoutHandler handlerTimeout,
          uint nTimeoutSec,
          bool isMainUIThread)
        {
            _thread = Thread.CurrentThread;
            _parentSession = parentSession;
            _timeoutManager = new TimeoutManager();
            Queue[] queues = new Queue[17];
            if (parentSession != null)
                queues[6] = parentSession.InputManager.Queue;
            _masterQueue = new PriorityQueue(queues);
            _masterQueue.LoopHook = new PriorityQueue.HookProc(CheckInterthreadItems);
            SetQueueDrainHook(DispatchPriority.Normal, new PriorityQueue.HookProc(CheckLoopCondition));
            SetQueueDrainHook(DispatchPriority.RPC, new PriorityQueue.HookProc(ProcessNativeEvents));
            SetQueueDrainHook(DispatchPriority.Idle, new PriorityQueue.HookProc(ProcessTimeouts));
            SetQueueDrainHook(DispatchPriority.Sleep, new PriorityQueue.HookProc(WaitForWork));
            _rpcYieldQueue = _masterQueue.BuildSubsetQueue(new int[2]
            {
        5,
        16
            }, true);
            _cleanupQueue = _masterQueue.BuildSubsetQueue(new int[1], true);
            _doBatchFlush = new PriorityQueue.HookProc(DoBatchFlush);
            if (!isMainUIThread)
                return;
            s_mainUIThread = Thread.CurrentThread;
        }

        public void ShutDown(bool flushRefs)
        {
            DoHousekeeping();
            if (flushRefs)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                DoHousekeeping();
            }
            if (_shutdown)
                return;
            _shutdown = true;
            FinalStopDispatch();
        }

        public new void Dispose()
        {
            ShutDown(false);
            if (s_mainUIThread == Thread.CurrentThread)
            {
                s_mainUIThread = null;
                s_exiting = true;
            }
            if (_masterQueue != null)
            {
                _masterQueue.Dispose();
                _rpcYieldQueue.Dispose();
                _cleanupQueue.Dispose();
                _timeoutManager.Dispose();
            }
            base.Dispose();
        }

        public static UIDispatcher CurrentDispatcher => Dispatcher.CurrentDispatcher as UIDispatcher;

        public static bool IsUIThread => Thread.CurrentThread == s_mainUIThread;

        public static Thread MainUIThread => s_mainUIThread;

        public static bool Exiting => s_exiting;

        public UISession UISession => _parentSession;

        public TimeoutManager TimeoutManager => _timeoutManager;

        public bool RespectNativeQuitRequests
        {
            get => _respectNativeQuitRequests;
            set => _respectNativeQuitRequests = value;
        }

        void IRenderHost.DeferredInvoke(
          Microsoft.Iris.Render.DeferredInvokePriority priority,
          IDeferredInvokeItem item,
          TimeSpan delay)
        {
            if (delay == TimeSpan.Zero)
            {
                DispatchPriority priority1 = DispatchPriority.Idle;
                switch (priority)
                {
                    case Render.DeferredInvokePriority.High:
                        priority1 = DispatchPriority.High;
                        break;
                    case Render.DeferredInvokePriority.Normal:
                        priority1 = DispatchPriority.Normal;
                        break;
                    case Render.DeferredInvokePriority.VisualUpdate:
                        priority1 = DispatchPriority.Render;
                        break;
                    case Render.DeferredInvokePriority.Low:
                        priority1 = DispatchPriority.Idle;
                        break;
                    case Render.DeferredInvokePriority.Idle:
                        priority1 = DispatchPriority.Idle;
                        break;
                }
                Post(priority1, DeferredCall.Create(item));
            }
            else
                Post(delay, DeferredCall.Create(item));
        }

        public static void Post(DateTime when, QueueItem item)
        {
            Thread mainUiThread = MainUIThread;
            if (mainUiThread == null)
                return;
            Post(mainUiThread, when, item);
        }

        public static void Post(TimeSpan delay, QueueItem item)
        {
            Thread mainUiThread = MainUIThread;
            if (mainUiThread == null)
                return;
            Post(mainUiThread, delay, item);
        }

        public static void Post(DispatchPriority priority, QueueItem item)
        {
            Thread mainUiThread = MainUIThread;
            if (mainUiThread == null)
                return;
            Post(mainUiThread, priority, item);
        }

        public static void Post(Thread thread, DateTime when, QueueItem item) => TimeoutManager.SetTimeoutAbsolute(thread, item, when);

        public static void Post(Thread thread, TimeSpan delay, QueueItem item) => TimeoutManager.SetTimeoutRelative(thread, item, delay);

        public static void Post(Thread thread, DispatchPriority priority, QueueItem item) => PostItem_AnyThread(thread, item, (int)priority);

        public void Run(LoopCondition condition) => MainLoop(_masterQueue, condition);

        public void StopCurrentMessageLoop() => _messageLoop.QuitPending = true;

        public static void StopCurrentMessageLoop(Thread thread)
        {
            if (thread != null && thread != Thread.CurrentThread)
                DeferredCall.Post(thread, DispatchPriority.Normal, new SimpleCallback(StopMessageLoopHandler));
            else
                CurrentDispatcher?.StopCurrentMessageLoop();
        }

        public void RPCYield(LoopCondition condition) => MainLoop(_rpcYieldQueue, condition);

        public void DoHousekeeping() => MainLoop(_cleanupQueue, null);

        private void SetQueueLock(DispatchPriority priority, bool value) => _masterQueue.SetLock((int)priority, value);

        internal void TemporarilyBlockRPCs()
        {
            SetQueueLock(DispatchPriority.RPC, true);
            RequestBatchFlush();
        }

        public void BlockInputQueue(bool value) => SetQueueLock(DispatchPriority.Input, value);

        public bool IsQueueLocked(DispatchPriority priority) => _masterQueue.IsLocked((int)priority);

        private void SetQueueDrainHook(DispatchPriority priority, PriorityQueue.HookProc hook) => _masterQueue.SetDrainHook((int)priority, hook);

        private PriorityQueue.HookProc GetQueueDrainHook(DispatchPriority priority) => _masterQueue.GetDrainHook((int)priority);

        private void MainLoop(Queue queue, LoopCondition condition)
        {
            using (new UIDispatcher.MessageLoop(this, condition))
                MainLoop(queue);
        }

        private static void StopMessageLoopHandler() => CurrentDispatcher?.StopCurrentMessageLoop();

        internal void RequestBatchFlush()
        {
            if (GetQueueDrainHook(DispatchPriority.RenderSync) != null)
                return;
            SetQueueDrainHook(DispatchPriority.RenderSync, _doBatchFlush);
        }

        private void DoBatchFlush(out bool didWork, out bool abort)
        {
            SetQueueDrainHook(DispatchPriority.RenderSync, null);
            UISession.FlushBatch();
            didWork = true;
            abort = false;
            if (!IsQueueLocked(DispatchPriority.RPC))
                return;
            SetQueueLock(DispatchPriority.RPC, false);
            didWork = true;
        }

        protected override void PostItem_SameThread(QueueItem item, int priority)
        {
            if (!(_masterQueue[priority] is SimpleQueue master))
                return;
            master.PostItem(item);
        }

        protected override void PostItems_SameThread(QueueItem.FIFO items, int priority)
        {
            if (!(_masterQueue[priority] is SimpleQueue master))
                return;
            master.PostItems(items);
        }

        protected override void WakeDispatchThread() => UISession.InterThreadWake();

        private void CheckInterthreadItems(out bool didWork, out bool abort)
        {
            didWork = DrainFeeder();
            abort = false;
        }

        private void CheckLoopCondition(out bool didWork, out bool abort)
        {
            didWork = false;
            abort = _messageLoop.QuitPending;
        }

        private void ProcessNativeEvents(out bool didWork, out bool abort)
        {
            didWork = UISession.ProcessNativeEvents();
            abort = false;
            if (didWork)
                return;
            SetQueueLock(DispatchPriority.RPC, true);
        }

        private void ProcessTimeouts(out bool didWork, out bool abort)
        {
            didWork = _timeoutManager.ProcessPendingTimeouts();
            abort = false;
        }

        private void WaitForWork(out bool didWork, out bool abort)
        {
            didWork = true;
            abort = false;
            if (_messageLoop.QuitPending)
            {
                didWork = false;
                abort = true;
            }
            else
            {
                uint nextTimeoutMillis = _timeoutManager.NextTimeoutMillis;
                if (nextTimeoutMillis == 0U)
                    _timeoutManager.ProcessPendingTimeouts();
                else
                    UISession.WaitForWork(nextTimeoutMillis);
            }
            SetQueueLock(DispatchPriority.RPC, false);
        }

        internal static void VerifyOnApplicationThread()
        {
            if (!IsUIThread)
                throw new InvalidOperationException("Operation must be performed on the application thread");
        }

        private class MessageLoop : IDisposable
        {
            private UIDispatcher _dispatcher;
            private LoopCondition _condition;
            private UIDispatcher.MessageLoop _parent;
            private bool _quitPending;

            public MessageLoop(UIDispatcher dispatcher, LoopCondition condition)
            {
                _dispatcher = dispatcher;
                _condition = condition;
                _parent = _dispatcher._messageLoop;
                _dispatcher._messageLoop = this;
            }

            public void Dispose() => _dispatcher._messageLoop = _parent;

            public UIDispatcher.MessageLoop Parent => _parent;

            public bool QuitPending
            {
                get
                {
                    if (!_quitPending && _condition != null)
                        _quitPending = !_condition();
                    return _quitPending;
                }
                set => _quitPending = value;
            }
        }
    }
}
