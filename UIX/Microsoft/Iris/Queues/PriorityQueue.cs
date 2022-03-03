// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Queues.PriorityQueue
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Queues
{
    internal class PriorityQueue : Queue
    {
        public const int MAX_PRIORITIES = 32;
        private static readonly int[] s_lowestBitInNibble = new int[16]
        {
      4,
      0,
      1,
      0,
      2,
      0,
      1,
      0,
      3,
      0,
      1,
      0,
      2,
      0,
      1,
      0
        };
        private int _readyMask;
        private int _wakeMask;
        private int _hookMask;
        private int _lockMask;
        private int _allQueues;
        private Queue[] _queues;
        private PriorityQueue.HookProc _loopHook;
        private PriorityQueue.HookProc[] _drainHooks;

        public PriorityQueue(uint count) : this(new Queue[count])
        {
        }

        public PriorityQueue(Queue[] queues)
        {
            _allQueues = InitChildQueues(queues);
            _queues = queues;
            _drainHooks = new PriorityQueue.HookProc[queues.Length];
            _wakeMask = _allQueues;
            UpdateReadyMask();
        }

        public override void Dispose()
        {
            Queue[] queues = _queues;
            base.Dispose();
            if (queues == null)
                return;
            foreach (Queue queue in queues)
                queue?.Dispose();
        }

        private int InitChildQueues(Queue[] queues)
        {
            int num = 0;
            for (int priority = 0; priority < queues.Length; ++priority)
            {
                Queue queue = queues[priority];
                if (queue == null)
                {
                    queue = new SimpleQueue();
                    queues[priority] = queue;
                }
                PriorityQueue.WakeProxy wakeProxy = new PriorityQueue.WakeProxy(this, priority, queue);
                num |= 1 << priority;
            }
            return num;
        }

        public Queue this[int priority] => _queues[priority];

        public PriorityQueue.HookProc GetDrainHook(int priority) => _drainHooks[priority];

        public void SetDrainHook(int priority, PriorityQueue.HookProc hook)
        {
            _drainHooks[priority] = hook;
            if (hook != null)
                _hookMask |= 1 << priority;
            else
                _hookMask &= ~(1 << priority);
            UpdateReadyMask();
        }

        public PriorityQueue.HookProc LoopHook
        {
            get => _loopHook;
            set => _loopHook = value;
        }

        public bool IsLocked(int priority) => (_lockMask & 1 << priority) != 0;

        public void SetLock(int priority, bool value)
        {
            if (value)
                _lockMask |= 1 << priority;
            else
                _lockMask &= ~(1 << priority);
            UpdateReadyMask();
        }

        public void LockAll(bool value)
        {
            if (value)
                _lockMask |= _allQueues;
            else
                _lockMask &= ~_allQueues;
            UpdateReadyMask();
        }

        public Queue BuildSubsetQueue(int[] priorities, bool ignoreLocks)
        {
            int subsetMask = 0;
            for (int index = 0; index < priorities.Length; ++index)
                subsetMask |= 1 << priorities[index];
            return new PriorityQueue.SubsetQueue(this, subsetMask, ignoreLocks);
        }

        public override QueueItem GetNextItem() => GetNextItemWorker(_allQueues, false);

        private QueueItem GetNextItemWorker(int subsetMask, bool ignoreLocks)
        {
            int mask = BeginReadLoop(subsetMask, ignoreLocks);
            QueueItem queueItem = null;
            while (mask != 0)
            {
                int lowestBit = FindLowestBit(mask);
                queueItem = _queues[lowestBit].GetNextItem();
                if (queueItem == null)
                {
                    SetWake(lowestBit, false);
                    PriorityQueue.HookProc drainHook = _drainHooks[lowestBit];
                    if (drainHook != null)
                    {
                        bool didWork;
                        bool abort;
                        drainHook(out didWork, out abort);
                        if (!abort)
                        {
                            if (didWork)
                            {
                                mask = BeginReadLoop(subsetMask, ignoreLocks);
                                continue;
                            }
                        }
                        else
                            break;
                    }
                    mask &= ~(1 << lowestBit);
                }
                else
                    break;
            }
            return queueItem;
        }

        private int BeginReadLoop(int subsetMask, bool ignoreLocks)
        {
            if (!ignoreLocks)
                subsetMask &= ~_lockMask;
            subsetMask &= _wakeMask | _hookMask;
            if (subsetMask != 0 && _loopHook != null)
            {
                bool didWork;
                bool abort;
                _loopHook(out didWork, out abort);
                if (abort)
                    subsetMask = 0;
            }
            return subsetMask;
        }

        private void SetWake(int priority, bool value)
        {
            if (value)
                _wakeMask |= 1 << priority;
            else
                _wakeMask &= ~(1 << priority);
            UpdateReadyMask();
        }

        private void UpdateReadyMask()
        {
            bool flag = _readyMask == 0;
            _readyMask = (_wakeMask | _hookMask) & ~_lockMask;
            if (!flag || _readyMask == 0)
                return;
            OnWake();
        }

        private static int FindLowestBit(int mask)
        {
            int num = 0;
            if ((mask & ushort.MaxValue) == 0)
            {
                num += 16;
                mask >>= 16;
            }
            if ((mask & byte.MaxValue) == 0)
            {
                num += 8;
                mask >>= 8;
            }
            if ((mask & 15) == 0)
            {
                num += 4;
                mask >>= 4;
            }
            return num + s_lowestBitInNibble[mask & 15];
        }

        public delegate void HookProc(out bool didWork, out bool abort);

        private class SubsetQueue : Queue
        {
            private PriorityQueue _owner;
            private int _subsetMask;
            private bool _ignoreLocks;

            public SubsetQueue(PriorityQueue owner, int subsetMask, bool ignoreLocks)
            {
                _owner = owner;
                _subsetMask = subsetMask;
                _ignoreLocks = ignoreLocks;
            }

            public override QueueItem GetNextItem() => _owner.GetNextItemWorker(_subsetMask, _ignoreLocks);
        }

        private class WakeProxy
        {
            private PriorityQueue _owner;
            private int _priority;

            public WakeProxy(PriorityQueue owner, int priority, Queue queue)
            {
                _owner = owner;
                _priority = priority;
                queue.Wake += new EventHandler(OnChildWake);
            }

            private void OnChildWake(object sender, EventArgs args) => _owner.SetWake(_priority, true);
        }
    }
}
