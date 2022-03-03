// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Queues.Feeder
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Queues
{
    internal class Feeder
    {
        private Dispatcher _dispatcher;
        private bool _hasItems;
        private QueueItem.FIFO[] _fifos;

        public bool HasItems => _hasItems;

        public void EnterDispatch(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            if (!_hasItems)
                return;
            _dispatcher.NotifyFeederItems();
        }

        public void LeaveDispatch(Dispatcher dispatcher) => _dispatcher = null;

        public void PostItem(QueueItem item, int priority)
        {
            bool flag = false;
            lock (this)
            {
                if (_fifos == null)
                    _fifos = new QueueItem.FIFO[32];
                (_fifos[priority] ?? (_fifos[priority] = new QueueItem.FIFO())).Append(item);
                if (!_hasItems)
                {
                    _hasItems = true;
                    if (_dispatcher != null)
                        flag = true;
                }
            }
            if (!flag)
                return;
            _dispatcher.NotifyFeederItems();
        }

        public QueueItem.FIFO[] HandoffFIFOs()
        {
            lock (this)
            {
                QueueItem.FIFO[] fifos = _fifos;
                _fifos = null;
                _hasItems = false;
                return fifos;
            }
        }

        public void RecycleFIFOs(QueueItem.FIFO[] recycled)
        {
            if (_fifos != null)
                return;
            lock (this)
            {
                if (_fifos != null)
                    return;
                _fifos = recycled;
            }
        }
    }
}
