// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.TimeoutManager
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Queues;
using System;
using System.Threading;

namespace Microsoft.Iris.Session
{
    public class TimeoutManager
    {
        private TimeoutManager.PendingList _pending;
        private DateTime _lastSystemTime;
        private long _lastSystemMilliseconds;
        private static readonly DeferredHandler _cancelTimeoutInterthread = new DeferredHandler(CancelTimeoutInterthread);

        public TimeoutManager()
        {
            _pending = new TimeoutManager.PendingList();
            _lastSystemTime = TimeNow;
            _lastSystemMilliseconds = DispatcherTimer.SystemTickCount.Milliseconds;
        }

        public void Dispose() => _pending.Dispose();

        public static DateTime TimeNow => DateTime.UtcNow;

        public uint NextTimeoutMillis
        {
            get
            {
                SynchronizeSystemTime();
                uint num1 = uint.MaxValue;
                if (!_pending.IsEmpty)
                {
                    TimeSpan timeSpan = _pending.NextExpirationTime - TimeNow;
                    if (timeSpan > TimeSpan.Zero)
                    {
                        long ticks = timeSpan.Ticks;
                        long num2 = ticks / 10000L;
                        if (ticks % 10000L > 0L)
                            ++num2;
                        if (num2 < uint.MaxValue)
                            num1 = (uint)num2;
                    }
                    else
                        num1 = 0U;
                }
                return num1;
            }
        }

        public void SetTimeoutAbsolute(QueueItem item, DateTime when) => SetTimeoutWorker(TimeNow, null, item, when, false);

        public static void SetTimeoutAbsolute(Thread thread, QueueItem item, DateTime when)
        {
            DateTime timeNow = TimeNow;
            SetTimeoutOnThread(thread, timeNow, item, when, false);
        }

        public void SetTimeoutRelative(QueueItem item, TimeSpan delay)
        {
            DateTime timeNow = TimeNow;
            SetTimeoutWorker(timeNow, null, item, timeNow + delay, true);
        }

        public static void SetTimeoutRelative(Thread thread, QueueItem item, TimeSpan delay)
        {
            DateTime timeNow = TimeNow;
            SetTimeoutOnThread(thread, timeNow, item, timeNow + delay, true);
        }

        public void CancelTimeout(QueueItem item)
        {
            if (!UIDispatcher.IsUIThread)
                DeferredCall.Post(DispatchPriority.Normal, _cancelTimeoutInterthread, item);
            else
                _pending.RemoveItem(item);
        }

        public bool ProcessPendingTimeouts()
        {
            bool flag = false;
            SynchronizeSystemTime();
            DateTime timeNow = TimeNow;
            while (true)
            {
                QueueItem queueItem = _pending.RemoveNextExpired(timeNow);
                if (queueItem != null)
                {
                    UIDispatcher.Post(Thread.CurrentThread, DispatchPriority.Normal, queueItem);
                    flag = true;
                }
                else
                    break;
            }
            return flag;
        }

        private void SetTimeoutWorker(
          DateTime currentTime,
          QueueItem preWrapped,
          QueueItem item,
          DateTime when,
          bool isRelative)
        {
            if (!UIDispatcher.IsUIThread)
            {
                UIDispatcher.Post(UIDispatcher.MainUIThread, DispatchPriority.Normal, PendingList.GetInterthreadItem(item, when, isRelative));
            }
            else
            {
                SynchronizeSystemTime();
                if (preWrapped != null)
                    _pending.AddItemInternal(preWrapped);
                else
                    _pending.AddItem(item, when, isRelative);
            }
        }

        private static void SetTimeoutOnThread(
          Thread thread,
          DateTime currentTime,
          QueueItem item,
          DateTime when,
          bool isRelative)
        {
            if (thread == Thread.CurrentThread)
                DeliverToCurrentThread(currentTime, null, item, when, isRelative);
            else
                UIDispatcher.Post(thread, DispatchPriority.Normal, PendingList.GetInterthreadItem(item, when, isRelative));
        }

        private static void DeliverToCurrentThread(
          DateTime currentTime,
          QueueItem preWrapped,
          QueueItem item,
          DateTime when,
          bool isRelative)
        {
            TimeoutManagerForCurrentThread?.SetTimeoutWorker(currentTime, preWrapped, item, when, isRelative);
        }

        private static void CancelTimeoutInterthread(object param) => TimeoutManagerForCurrentThread?.CancelTimeout((QueueItem)param);

        private void SynchronizeSystemTime()
        {
            DateTime timeNow = TimeNow;
            long milliseconds = DispatcherTimer.SystemTickCount.Milliseconds;
            DateTime dateTime = _lastSystemTime + TimeSpan.FromMilliseconds(milliseconds - _lastSystemMilliseconds);
            _lastSystemTime = timeNow;
            _lastSystemMilliseconds = milliseconds;
            if (Math.Abs((timeNow - dateTime).TotalSeconds) <= 30.0)
                return;
            _pending.ShiftRelativeTimeouts(timeNow - dateTime);
        }

        private static TimeoutManager TimeoutManagerForCurrentThread
        {
            get
            {
                TimeoutManager timeoutManager = null;
                UIDispatcher currentDispatcher = UIDispatcher.CurrentDispatcher;
                if (currentDispatcher != null)
                    timeoutManager = currentDispatcher.TimeoutManager;
                return timeoutManager;
            }
        }

        protected class PendingList : QueueItem.Chain
        {
            private TimeoutManager.PendingList.PendingItem _head;

            public override void Dispose() => base.Dispose();

            public bool IsEmpty => _head == null;

            public DateTime NextExpirationTime => _head.expireTime;

            public bool NextItemIs(QueueItem innerItem)
            {
                QueueItem queueItem = null;
                if (_head != null)
                    queueItem = _head.innerItem;
                return queueItem == innerItem;
            }

            public void AddItem(QueueItem innerItem, DateTime expireTime, bool isRelative) => AddWorker(null, innerItem, expireTime, isRelative);

            public void AddItemInternal(QueueItem outerItem)
            {
                TimeoutManager.PendingList.PendingItem outerItem1 = outerItem as TimeoutManager.PendingList.PendingItem;
                AddWorker(outerItem1, outerItem1.innerItem, outerItem1.expireTime, outerItem1.isRelative);
            }

            public static QueueItem GetInterthreadItem(
              QueueItem innerItem,
              DateTime expireTime,
              bool isRelative)
            {
                return new TimeoutManager.PendingList.PendingItem(innerItem, expireTime, isRelative);
            }

            public void ShiftRelativeTimeouts(TimeSpan spanTime)
            {
                if (IsEmpty)
                    return;
                Vector<TimeoutManager.PendingList.PendingItem> vector = new Vector<TimeoutManager.PendingList.PendingItem>();
                foreach (TimeoutManager.PendingList.PendingItem pendingItem in this)
                {
                    if (pendingItem.isRelative)
                        vector.Add(pendingItem);
                }
                foreach (TimeoutManager.PendingList.PendingItem outerItem in vector)
                    RemoveWorker(outerItem);
                foreach (TimeoutManager.PendingList.PendingItem pendingItem in vector)
                    AddItem(pendingItem.innerItem, pendingItem.expireTime + spanTime, true);
            }

            public bool RemoveItem(QueueItem innerItem)
            {
                foreach (TimeoutManager.PendingList.PendingItem outerItem in this)
                {
                    if (outerItem.innerItem == innerItem)
                    {
                        RemoveWorker(outerItem);
                        return true;
                    }
                }
                return false;
            }

            public QueueItem RemoveNextExpired(DateTime threshold)
            {
                QueueItem queueItem = null;
                TimeoutManager.PendingList.PendingItem head = _head;
                if (head != null && head.expireTime <= threshold)
                {
                    RemoveWorker(head);
                    queueItem = head.innerItem;
                }
                return queueItem;
            }

            public QueueItem.Chain.ChainEnumerator GetEnumerator()
            {
                QueueItem tail = _head;
                if (tail != null)
                    tail = PrevItem(tail);
                return new QueueItem.Chain.ChainEnumerator(tail);
            }

            private void AddWorker(
              TimeoutManager.PendingList.PendingItem outerItem,
              QueueItem innerItem,
              DateTime expireTime,
              bool isRelative)
            {
                ValidateAdd(innerItem);
                TimeoutManager.PendingList.PendingItem pendingItem1 = null;
                TimeoutManager.PendingList.PendingItem pendingItem2 = null;
                if (_head != null)
                {
                    foreach (TimeoutManager.PendingList.PendingItem pendingItem3 in this)
                    {
                        if (expireTime < pendingItem3.expireTime)
                        {
                            pendingItem1 = pendingItem3;
                            break;
                        }
                    }
                    pendingItem2 = pendingItem1 ?? _head;
                }
                if (outerItem == null)
                    outerItem = new TimeoutManager.PendingList.PendingItem(innerItem, expireTime, isRelative);
                Link(innerItem, null, false);
                Link(outerItem, pendingItem2, true);
                if (_head != pendingItem1)
                    return;
                _head = outerItem;
            }

            private void RemoveWorker(TimeoutManager.PendingList.PendingItem outerItem)
            {
                if (_head == outerItem)
                    _head = IsOnlyChild(_head) ? null : NextItem(_head) as TimeoutManager.PendingList.PendingItem;
                Unlink(outerItem);
                Unlink(outerItem.innerItem);
            }

            internal class PendingItem : QueueItem
            {
                public QueueItem innerItem;
                public DateTime expireTime;
                public bool isRelative;

                public PendingItem(QueueItem innerItem, DateTime expireTime, bool isRelative)
                {
                    this.innerItem = innerItem;
                    this.expireTime = expireTime;
                    this.isRelative = isRelative;
                }

                public override void Dispatch() => DeliverToCurrentThread(TimeNow, this, innerItem, expireTime, isRelative);
            }
        }
    }
}
