// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.DispatcherTimer
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Debug;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Queues;
using System;

namespace Microsoft.Iris.Session
{
    internal class DispatcherTimer
    {
        private TimeSpan _interval;
        private bool _autoRepeat;
        private object _userData;
        private DispatcherTimer.TimerCallback _callback;
        private long _timeBase;
        private TimeoutManager _timeoutManager;
        private ITimerOwner _owner;

        public DispatcherTimer(ITimerOwner owner)
        {
            _owner = owner;
            _interval = TimeSpan.FromMilliseconds(100.0);
            _autoRepeat = true;
            _timeBase = SystemTickCount.Milliseconds;
            _timeoutManager = UIDispatcher.CurrentDispatcher.TimeoutManager;
        }

        public DispatcherTimer()
          : this(null)
        {
        }

        public TimeSpan TimeSpanInterval
        {
            get => _interval;
            set
            {
                if (!(_interval != value))
                    return;
                _interval = value;
                if (Enabled)
                    Start();
                FireNotification(NotificationID.Interval);
            }
        }

        public int Interval
        {
            get => (int)TimeSpanInterval.TotalMilliseconds;
            set => TimeSpanInterval = TimeSpan.FromMilliseconds(value);
        }

        public bool Enabled
        {
            get => _callback != null;
            set
            {
                if (Enabled == value)
                    return;
                if (value)
                    Start();
                else
                    Stop();
            }
        }

        public bool AutoRepeat
        {
            get => _autoRepeat;
            set
            {
                if (_autoRepeat == value)
                    return;
                _autoRepeat = value;
                FireNotification(NotificationID.AutoRepeat);
            }
        }

        internal object UserData
        {
            get => _userData;
            set
            {
                if (_userData == value)
                    return;
                _userData = value;
            }
        }

        public void Start()
        {
            bool enabled = Enabled;
            StopWorker();
            _timeBase = SystemTickCount.Milliseconds;
            _callback = new DispatcherTimer.TimerCallback(this);
            ScheduleCallback(_timeBase);
            FireEnabledChange(enabled);
        }

        public void Stop()
        {
            bool enabled = Enabled;
            StopWorker();
            FireEnabledChange(enabled);
        }

        private void StopWorker()
        {
            if (_callback == null)
                return;
            _timeoutManager.CancelTimeout(_callback);
            _callback = null;
        }

        private void FireEnabledChange(bool wasEnabled)
        {
            if (Enabled == wasEnabled)
                return;
            FireNotification(NotificationID.Enabled);
        }

        public override string ToString()
        {
            string str = " one-shot";
            if (_autoRepeat)
                str = " repeating";
            return "[" + Interval + str + "] -> " + DebugHelpers.DEBUG_ObjectToString(Tick);
        }

        public event EventHandler Tick;

        private void ScheduleCallback(long currentTimeInMilliseconds)
        {
            if (_callback == null)
                return;
            TimeSpan timeSpan1 = _interval;
            TimeSpan timeSpan2 = TimeSpan.FromMilliseconds(currentTimeInMilliseconds - _timeBase);
            if (timeSpan2 >= timeSpan1)
            {
                long ticks = timeSpan1.Ticks;
                if (ticks > 0L)
                    timeSpan1 = TimeSpan.FromTicks(ticks * ((timeSpan2.Ticks + ticks / 2L) / ticks));
            }
            _timeBase += (long)timeSpan1.TotalMilliseconds;
            _timeoutManager.SetTimeoutRelative(_callback, TimeSpan.FromMilliseconds(_timeBase - currentTimeInMilliseconds));
        }

        private void CallTickHandlers(DispatcherTimer.TimerCallback callback)
        {
            if (!CallbackValid(callback))
                return;
            if (_autoRepeat)
            {
                ScheduleCallback(SystemTickCount.Milliseconds);
            }
            else
            {
                _callback = null;
                FireEnabledChange(true);
            }
            if (Tick != null)
                Tick(_owner != null ? _owner : (object)this, EventArgs.Empty);
            FireNotification(NotificationID.Tick);
        }

        private bool CallbackValid(DispatcherTimer.TimerCallback callback) => callback == _callback;

        private void FireNotification(string id)
        {
            if (_owner == null)
                return;
            _owner.OnTimerPropertyChanged(id);
        }

        private class TimerCallback : QueueItem
        {
            private DispatcherTimer _timer;

            public TimerCallback(DispatcherTimer timer) => _timer = timer;

            public override void Dispatch() => _timer.CallTickHandlers(this);

            public override string ToString()
            {
                string str = "";
                if (!_timer.CallbackValid(this))
                    str = "CANCELED ";
                return str + GetType().Name + " -> " + _timer;
            }
        }

        internal static class SystemTickCount
        {
            private static long s_tickCount;
            private static int s_lastTickCount;

            public static long Milliseconds
            {
                get
                {
                    Refresh();
                    return s_tickCount;
                }
            }

            private static void Refresh()
            {
                int tickCount = Environment.TickCount;
                long num = tickCount < s_lastTickCount ? int.MaxValue - s_lastTickCount + tickCount : tickCount - s_lastTickCount;
                s_tickCount += num;
                s_lastTickCount = tickCount;
            }
        }
    }
}
