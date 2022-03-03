// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RangedValue
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using System;
using System.ComponentModel;

namespace Microsoft.Iris
{
    public class RangedValue :
      ModelItem,
      IValueRange,
      IModelItem,
      INotifyPropertyChanged,
      IModelItemOwner,
      IUIRangedValue,
      IUIValueRange,
      INotifyObject,
      AssemblyObjectProxyHelper.IFrameworkProxyObject,
      AssemblyObjectProxyHelper.IAssemblyProxyObject,
      IDisposableObject
    {
        private Microsoft.Iris.ModelItems.RangedValue _rangedValue;
        private NotifyService _notifier = new NotifyService();
        private CodeListeners _listeners;

        public RangedValue(IModelItemOwner owner, string description)
          : base(owner, description)
          => Initialize();

        public RangedValue(IModelItemOwner owner)
          : this(owner, null)
        {
        }

        public RangedValue()
          : this(null)
        {
        }

        public float Value
        {
            get
            {
                using (ThreadValidator)
                    return _rangedValue.Value;
            }
            set
            {
                using (ThreadValidator)
                    _rangedValue.Value = value;
            }
        }

        object IValueRange.Value
        {
            get
            {
                using (ThreadValidator)
                    return ((IUIValueRange)_rangedValue).ObjectValue;
            }
        }

        object IUIValueRange.ObjectValue => ((IUIValueRange)_rangedValue).ObjectValue;

        public float MinValue
        {
            get
            {
                using (ThreadValidator)
                    return _rangedValue.MinValue;
            }
            set
            {
                using (ThreadValidator)
                    _rangedValue.MinValue = value <= (double)MaxValue ? value : throw new ArgumentException(InvariantString.Format("MinValue must be less than or equal to MaxValue.  Value Supplied was {0}, MaxValue is {1}", value, MaxValue));
            }
        }

        public float MaxValue
        {
            get
            {
                using (ThreadValidator)
                    return _rangedValue.MaxValue;
            }
            set
            {
                using (ThreadValidator)
                    _rangedValue.MaxValue = value >= (double)MinValue ? value : throw new ArgumentException(InvariantString.Format("MaxValue must be greater than or equal to MinValue.  Value Supplied was {0}, MinValue is {1}", value, MinValue));
            }
        }

        public float Step
        {
            get
            {
                using (ThreadValidator)
                    return _rangedValue.Step;
            }
            set
            {
                using (ThreadValidator)
                    _rangedValue.Step = value;
            }
        }

        public float Range
        {
            get
            {
                using (ThreadValidator)
                    return _rangedValue.Range;
            }
        }

        public bool HasNextValue
        {
            get
            {
                using (ThreadValidator)
                    return _rangedValue.HasNextValue;
            }
        }

        public bool HasPreviousValue
        {
            get
            {
                using (ThreadValidator)
                    return _rangedValue.HasPreviousValue;
            }
        }

        public void NextValue()
        {
            using (ThreadValidator)
                _rangedValue.NextValue();
        }

        public void PreviousValue()
        {
            using (ThreadValidator)
                _rangedValue.PreviousValue();
        }

        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);
            if (disposing)
            {
                _notifier.ClearListeners();
                _listeners.Dispose(this);
            }
            _rangedValue = null;
        }

        object AssemblyObjectProxyHelper.IFrameworkProxyObject.FrameworkObject => this;

        object AssemblyObjectProxyHelper.IAssemblyProxyObject.AssemblyObject => this;

        private void Initialize()
        {
            _rangedValue = CreateInternalRangedValue();
            Vector<Listener> listeners = new Vector<Listener>(7);
            DelegateListener.OnNotifyCallback callback = new DelegateListener.OnNotifyCallback(OnInternalPropertyChanged);
            listeners.Add(new DelegateListener(_rangedValue, NotificationID.MinValue, callback));
            listeners.Add(new DelegateListener(_rangedValue, NotificationID.MaxValue, callback));
            listeners.Add(new DelegateListener(_rangedValue, NotificationID.Step, callback));
            listeners.Add(new DelegateListener(_rangedValue, NotificationID.Range, callback));
            listeners.Add(new DelegateListener(_rangedValue, NotificationID.Value, callback));
            listeners.Add(new DelegateListener(_rangedValue, NotificationID.HasPreviousValue, callback));
            listeners.Add(new DelegateListener(_rangedValue, NotificationID.HasNextValue, callback));
            _listeners = new CodeListeners(listeners);
            _listeners.DeclareOwner(this);
        }

        internal virtual Microsoft.Iris.ModelItems.RangedValue CreateInternalRangedValue() => new Microsoft.Iris.ModelItems.RangedValue();

        protected override void OnPropertyChanged(string property)
        {
            base.OnPropertyChanged(property);
            _notifier.FireThreadSafe(property);
        }

        private void OnInternalPropertyChanged(DelegateListener listener) => FirePropertyChanged(listener.Watch);

        void IDisposableObject.DeclareOwner(object owner)
        {
        }

        void IDisposableObject.TransferOwnership(object owner)
        {
        }

        void IDisposableObject.Dispose(object owner) => Dispose();

        void INotifyObject.AddListener(Listener listener) => _notifier.AddListener(listener);
    }
}
