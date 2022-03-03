// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Queues.Interconnect
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Threading;

namespace Microsoft.Iris.Queues
{
    internal class Interconnect
    {
        private Map<Thread, Feeder> _feeders;

        public Interconnect() => _feeders = new Map<Thread, Feeder>();

        private Feeder GetFeeder(Thread thread, bool lazyCreate)
        {
            if (thread == null)
                thread = Thread.CurrentThread;
            lock (this)
            {
                Feeder feeder;
                if (!_feeders.TryGetValue(thread, out feeder) && lazyCreate)
                    _feeders[thread] = feeder = new Feeder();
                return feeder;
            }
        }

        public Feeder EnterDispatch(Dispatcher dispatcher, bool isRoot)
        {
            Feeder feeder = GetFeeder(Thread.CurrentThread, isRoot);
            if (isRoot)
                feeder.EnterDispatch(dispatcher);
            return feeder;
        }

        public void LeaveDispatch(Dispatcher dispatcher, bool isRoot)
        {
            Thread currentThread = Thread.CurrentThread;
            Feeder feeder = GetFeeder(currentThread, false);
            if (!isRoot)
                return;
            feeder.LeaveDispatch(dispatcher);
            lock (this)
                _feeders.Remove(currentThread);
            if (!feeder.HasItems)
                return;
            dispatcher.DrainFeeder();
        }

        public void PostItem(Thread thread, QueueItem item, int priority) => GetFeeder(thread, false)?.PostItem(item, priority);
    }
}
