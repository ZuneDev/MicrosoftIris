// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Command
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Session;
using System;
using System.ComponentModel;

namespace Microsoft.Iris
{
    public class Command : ModelItem, ICommand, INotifyPropertyChanged
    {
        private static readonly EventCookie s_invokedEvent = EventCookie.ReserveSlot();
        private bool _availableFlag = true;
        private InvokePolicy _priority = InvokePolicy.AsynchronousNormal;

        public Command(IModelItemOwner owner, string description, EventHandler invokedHandler)
          : base(owner, description)
        {
            if (invokedHandler == null)
                return;
            Invoked += invokedHandler;
        }

        public Command(IModelItemOwner owner, EventHandler invokedHandler)
          : this(owner, null, invokedHandler)
        {
        }

        public Command(IModelItemOwner owner)
          : this(owner, null, null)
        {
        }

        public Command()
          : this(null)
        {
        }

        public virtual bool Available
        {
            get
            {
                using (ThreadValidator)
                    return _availableFlag;
            }
            set
            {
                using (ThreadValidator)
                {
                    if (_availableFlag == value)
                        return;
                    _availableFlag = value;
                    FirePropertyChanged(nameof(Available));
                }
            }
        }

        public InvokePolicy Priority
        {
            get
            {
                using (ThreadValidator)
                    return _priority;
            }
            set
            {
                using (ThreadValidator)
                {
                    if (_priority == value)
                        return;
                    _priority = value;
                    FirePropertyChanged(nameof(Priority));
                }
            }
        }

        public void Invoke() => Invoke(Priority);

        public void Invoke(InvokePolicy invokePolicy)
        {
            using (ThreadValidator)
            {
                if (invokePolicy != InvokePolicy.Synchronous)
                {
                    DispatchPriority priority = DispatchPriority.AppEvent;
                    if (invokePolicy == InvokePolicy.AsynchronousLowPri)
                        priority = DispatchPriority.Idle;
                    DeferredCall.Post(priority, new SimpleCallback(InvokeWorker));
                }
                else
                    InvokeWorker();
            }
        }

        private void InvokeWorker()
        {
            if (IsDisposed)
                return;
            FirePropertyChanged("Invoked");
            if (GetEventHandler(s_invokedEvent) is EventHandler eventHandler)
                eventHandler(this, EventArgs.Empty);
            OnInvoked();
        }

        public event EventHandler Invoked
        {
            add
            {
                using (ThreadValidator)
                    AddEventHandler(s_invokedEvent, value);
            }
            remove
            {
                using (ThreadValidator)
                    RemoveEventHandler(s_invokedEvent, value);
            }
        }

        protected virtual void OnInvoked()
        {
        }
    }
}
