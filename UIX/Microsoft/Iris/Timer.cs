// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Timer
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris
{
    public sealed class Timer : ModelItem, ITimerOwner
    {
        private DispatcherTimer _dispatcherTimer;

        public Timer(IModelItemOwner owner, string description)
          : base(owner, description)
        {
            UIDispatcher.VerifyOnApplicationThread();
            _dispatcherTimer = new DispatcherTimer(this);
        }

        public Timer(IModelItemOwner owner)
          : this(owner, null)
        {
        }

        public Timer()
          : this(null)
        {
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                UIDispatcher.VerifyOnApplicationThread();
                Stop();
            }
            base.OnDispose(disposing);
        }

        void ITimerOwner.OnTimerPropertyChanged(string id)
        {
            if (ReferenceEquals(id, NotificationID.Interval))
                FirePropertyChanged("TimeSpanInterval");
            FirePropertyChanged(id);
        }

        public TimeSpan TimeSpanInterval
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _dispatcherTimer.TimeSpanInterval;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _dispatcherTimer.TimeSpanInterval = !(value < TimeSpan.Zero) ? value : throw new ArgumentException("Must have non-negative time interval");
            }
        }

        public int Interval
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _dispatcherTimer.Interval;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _dispatcherTimer.Interval = value >= 0 ? value : throw new ArgumentException("Must have non-negative time interval");
            }
        }

        public bool Enabled
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _dispatcherTimer.Enabled;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _dispatcherTimer.Enabled = value;
            }
        }

        public bool AutoRepeat
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _dispatcherTimer.AutoRepeat;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _dispatcherTimer.AutoRepeat = value;
            }
        }

        public event EventHandler Tick
        {
            add
            {
                UIDispatcher.VerifyOnApplicationThread();
                _dispatcherTimer.Tick += value;
            }
            remove
            {
                UIDispatcher.VerifyOnApplicationThread();
                _dispatcherTimer.Tick -= value;
            }
        }

        public void Start()
        {
            UIDispatcher.VerifyOnApplicationThread();
            _dispatcherTimer.Start();
        }

        public void Stop()
        {
            UIDispatcher.VerifyOnApplicationThread();
            _dispatcherTimer.Stop();
        }

        public override string ToString() => GetType().Name + _dispatcherTimer.ToString();
    }
}
