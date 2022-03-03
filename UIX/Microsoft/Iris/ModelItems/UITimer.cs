// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.UITimer
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.ModelItems
{
    internal sealed class UITimer : DisposableNotifyObjectBase, ITimerOwner
    {
        private DispatcherTimer _dispatcherTimer;

        public UITimer() => _dispatcherTimer = new DispatcherTimer(this);

        protected override void OnDispose()
        {
            Enabled = false;
            base.OnDispose();
        }

        void ITimerOwner.OnTimerPropertyChanged(string id) => FireNotification(id);

        public int Interval
        {
            get => _dispatcherTimer.Interval;
            set => _dispatcherTimer.Interval = value;
        }

        public bool Enabled
        {
            get => _dispatcherTimer.Enabled;
            set => _dispatcherTimer.Enabled = value;
        }

        public bool AutoRepeat
        {
            get => _dispatcherTimer.AutoRepeat;
            set => _dispatcherTimer.AutoRepeat = value;
        }

        internal object UserData
        {
            get => _dispatcherTimer.UserData;
            set => _dispatcherTimer.UserData = value;
        }

        public void Start() => _dispatcherTimer.Start();

        public void Stop() => _dispatcherTimer.Stop();

        public override string ToString() => GetType().Name + _dispatcherTimer.ToString();
    }
}
