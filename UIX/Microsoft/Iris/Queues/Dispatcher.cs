// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Queues.Dispatcher
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Microsoft.Iris.Queues
{
    internal abstract class Dispatcher
    {
        private static Interconnect s_interconnect = new Interconnect();
        [ThreadStatic]
        private static Dispatcher s_threadDispatcher;
        private static NamedPipeClientStream _debugPipe;
        private static BinaryWriter _debugPipeWriter;
        private static BinaryReader _debugPipeReader;
        private Thread _owningThread;
        private uint _enterCount;
        private Feeder _feeder;
        private int _feederReadStamp;
        private int _feederWriteStamp;

        public Dispatcher()
        {
            _owningThread = Thread.CurrentThread;
            _feeder = EnterDispatch();
        }

        public void FinalStopDispatch()
        {
            LeaveDispatch();
            _feeder = null;
        }

        public void Dispose()
        {
            if (_enterCount != 1U)
                return;
            LeaveDispatch();
        }

        public static Dispatcher CurrentDispatcher => s_threadDispatcher;

        public static void PostItem_AnyThread(Thread thread, QueueItem item, int priority)
        {
            if (thread == null || thread == Thread.CurrentThread)
            {
                Dispatcher currentDispatcher = CurrentDispatcher;
                if (currentDispatcher != null)
                {
                    currentDispatcher.PostItem_SameThread(item, priority);
                    return;
                }
            }
            s_interconnect.PostItem(thread, item, priority);
        }

        public Thread DispatchThread => _owningThread;

        public void MainLoop(Queue queue)
        {
            EnterDispatch();
            try
            {
                while (true)
                {
                    QueueItem nextItem = queue.GetNextItem();
                    if (nextItem != null)
                    {
                        SendDebugMessage(nextItem.ToDebugPacketString());
                        nextItem.Dispatch();
                    }
                    else
                        break;
                }
            }
            finally
            {
                LeaveDispatch();
            }
        }

        protected abstract void PostItem_SameThread(QueueItem item, int priority);

        protected abstract void PostItems_SameThread(QueueItem.FIFO items, int priority);

        protected abstract void WakeDispatchThread();

        private Feeder EnterDispatch()
        {
            bool isRoot = _enterCount == 0U;
            ++_enterCount;
            if (isRoot)
                s_threadDispatcher = this;
            return s_interconnect.EnterDispatch(this, isRoot);
        }

        private void LeaveDispatch()
        {
            --_enterCount;
            bool isRoot = _enterCount == 0U;
            if (isRoot)
                s_threadDispatcher = null;
            s_interconnect.LeaveDispatch(this, isRoot);
        }

        public void NotifyFeederItems()
        {
            if (Thread.CurrentThread == _owningThread)
                return;
            Interlocked.Increment(ref _feederWriteStamp);
            WakeDispatchThread();
        }

        internal bool DrainFeeder()
        {
            int feederWriteStamp = _feederWriteStamp;
            if (feederWriteStamp == _feederReadStamp)
                return false;
            bool flag = false;
            _feederReadStamp = feederWriteStamp;
            QueueItem.FIFO[] recycled = _feeder.HandoffFIFOs();
            if (recycled != null)
            {
                for (int priority = 0; priority < recycled.Length; ++priority)
                {
                    QueueItem.FIFO items = recycled[priority];
                    if (items != null)
                    {
                        PostItems_SameThread(items, priority);
                        flag = true;
                    }
                }
                _feeder.RecycleFIFOs(recycled);
            }
            return flag;
        }

        private static NamedPipeClientStream DebugPipe
        {
            get
            {
                if (_debugPipe == null && Application.IsDebug)
                {
                    _debugPipe = new NamedPipeClientStream(System.Reflection.Assembly.GetExecutingAssembly().FullName);
                    _debugPipe.Connect();
                }
                return _debugPipe;
            }
        }

        private static BinaryWriter DebugPipeWriter
        {
            get
            {
                if (_debugPipe != null && _debugPipeWriter == null)
                    _debugPipeWriter = new BinaryWriter(DebugPipe);
                return _debugPipeWriter;
            }
        }

        private static BinaryReader DebugPipeReader
        {
            get
            {
                if (_debugPipe != null && _debugPipeReader == null)
                    _debugPipeReader = new BinaryReader(DebugPipe);
                return _debugPipeReader;
            }
        }

        /// <summary>
        /// Sends a message via <see cref="debugPipe"/>
        /// </summary>
        /// <param name="message"></param>
        public static void SendDebugMessage(string message)
        {
            if (Application.IsDebug && DebugPipe.IsConnected && DebugPipe.CanWrite)
            {
                DebugPipe.WriteByte(0x01);
                byte[] buffer = System.Text.Encoding.Unicode.GetBytes(message);
                DebugPipe.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
                DebugPipe.Write(buffer, 0, buffer.Length);
                DebugPipe.Flush();
            }
        }
    }
}
