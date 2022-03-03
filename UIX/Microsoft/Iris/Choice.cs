// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Choice
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Iris
{
    public class Choice :
      ModelItem,
      IValueRange,
      IModelItem,
      INotifyPropertyChanged,
      IModelItemOwner,
      IUIChoice,
      IUIValueRange,
      IDisposableObject,
      INotifyObject,
      AssemblyObjectProxyHelper.IFrameworkProxyObject,
      AssemblyObjectProxyHelper.IAssemblyProxyObject
    {
        private NotifyService _notifier = new NotifyService();
        private static readonly EventCookie s_chosenChangedEvent = EventCookie.ReserveSlot();
        private Microsoft.Iris.ModelItems.Choice _choice;
        private CodeListeners _listeners;
        private ModelItem _lastChosen;

        public Choice(IModelItemOwner owner, string description, IList options)
          : base(owner, description)
        {
            Initialize();
            Options = options;
        }

        public Choice(IModelItemOwner owner, string description)
          : base(owner, description)
          => Initialize();

        public Choice(IModelItemOwner owner)
          : this(owner, null)
        {
        }

        public Choice()
          : this(null)
        {
        }

        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);
            if (disposing)
            {
                _notifier.ClearListeners();
                _choice.Dispose(this);
                _listeners.Dispose(this);
            }
            _choice = null;
        }

        object AssemblyObjectProxyHelper.IFrameworkProxyObject.FrameworkObject => this;

        object AssemblyObjectProxyHelper.IAssemblyProxyObject.AssemblyObject => this;

        public IList Options
        {
            get
            {
                using (ThreadValidator)
                    return (IList)AssemblyLoadResult.UnwrapObject(_choice.Options);
            }
            set
            {
                using (ThreadValidator)
                {
                    IList potentialOptionsWrapped = (IList)AssemblyLoadResult.WrapObject(value);
                    ValidateOptionsList(potentialOptionsWrapped, value);
                    _choice.Options = potentialOptionsWrapped;
                }
            }
        }

        public object ChosenValue
        {
            get
            {
                using (ThreadValidator)
                    return AssemblyLoadResult.UnwrapObject(_choice.ChosenValue);
            }
            set
            {
                using (ThreadValidator)
                {
                    int index;
                    string error;
                    if (!_choice.ValidateOption(AssemblyLoadResult.WrapObject(value), out index, out error))
                        throw new ArgumentException(error);
                    _choice.ChosenIndex = index;
                }
            }
        }

        public int ChosenIndex
        {
            get
            {
                using (ThreadValidator)
                    return _choice.ChosenIndex;
            }
            set
            {
                using (ThreadValidator)
                {
                    string error;
                    _choice.ChosenIndex = _choice.ValidateIndex(value, out error) ? value : throw new ArgumentException(error);
                }
            }
        }

        public int DefaultIndex
        {
            get
            {
                using (ThreadValidator)
                    return _choice.DefaultIndex;
            }
            set
            {
                using (ThreadValidator)
                    _choice.DefaultIndex = value;
            }
        }

        object IValueRange.Value
        {
            get
            {
                using (ThreadValidator)
                    return AssemblyLoadResult.UnwrapObject(((IUIValueRange)_choice).ObjectValue);
            }
        }

        object IUIValueRange.ObjectValue => ((IUIValueRange)_choice).ObjectValue;

        IList IUIChoice.Options
        {
            get => _choice.Options;
            set => _choice.Options = value;
        }

        object IUIChoice.ChosenValue => _choice.ChosenValue;

        bool IUIChoice.ValidateIndex(int index, out string error) => _choice.ValidateIndex(index, out error);

        bool IUIChoice.ValidateOption(object value, out int index, out string error) => _choice.ValidateOption(value, out index, out error);

        bool IUIChoice.ValidateOptionsList(IList value, out string error) => _choice.ValidateOptionsList(value, out error);

        public bool HasNextValue
        {
            get
            {
                using (ThreadValidator)
                    return _choice.HasNextValue;
            }
        }

        public bool HasSelection
        {
            get
            {
                using (ThreadValidator)
                    return _choice.HasSelection;
            }
        }

        public bool HasPreviousValue
        {
            get
            {
                using (ThreadValidator)
                    return _choice.HasPreviousValue;
            }
        }

        public bool Wrap
        {
            get
            {
                using (ThreadValidator)
                    return _choice.Wrap;
            }
            set
            {
                using (ThreadValidator)
                    _choice.Wrap = value;
            }
        }

        public void PreviousValue()
        {
            using (ThreadValidator)
                _choice.PreviousValue();
        }

        public void PreviousValue(bool wrap)
        {
            using (ThreadValidator)
                _choice.PreviousValue(wrap);
        }

        public void NextValue()
        {
            using (ThreadValidator)
                _choice.NextValue();
        }

        public void NextValue(bool wrap)
        {
            using (ThreadValidator)
                _choice.NextValue(wrap);
        }

        public void Clear()
        {
            using (ThreadValidator)
                _choice.Clear();
        }

        public void DefaultValue()
        {
            using (ThreadValidator)
                _choice.DefaultValue();
        }

        protected virtual void OnChosenChanged()
        {
        }

        public event EventHandler ChosenChanged
        {
            add
            {
                using (ThreadValidator)
                    AddEventHandler(s_chosenChangedEvent, value);
            }
            remove
            {
                using (ThreadValidator)
                    RemoveEventHandler(s_chosenChangedEvent, value);
            }
        }

        private void Initialize()
        {
            _choice = CreateInternalChoice();
            _choice.DeclareOwner(this);
            Vector<Listener> listeners = new Vector<Listener>(9);
            DelegateListener.OnNotifyCallback callback = new DelegateListener.OnNotifyCallback(OnInternalChoicePropertyChanged);
            listeners.Add(new DelegateListener(_choice, NotificationID.Options, callback));
            listeners.Add(new DelegateListener(_choice, NotificationID.DefaultIndex, callback));
            listeners.Add(new DelegateListener(_choice, NotificationID.ChosenIndex, callback));
            listeners.Add(new DelegateListener(_choice, NotificationID.ChosenValue, new DelegateListener.OnNotifyCallback(OnChosenValueChanged)));
            listeners.Add(new DelegateListener(_choice, NotificationID.Value, callback));
            listeners.Add(new DelegateListener(_choice, NotificationID.HasSelection, callback));
            listeners.Add(new DelegateListener(_choice, NotificationID.Wrap, callback));
            listeners.Add(new DelegateListener(_choice, NotificationID.HasPreviousValue, callback));
            listeners.Add(new DelegateListener(_choice, NotificationID.HasNextValue, callback));
            _listeners = new CodeListeners(listeners);
            _listeners.DeclareOwner(this);
        }

        protected override void OnPropertyChanged(string property)
        {
            base.OnPropertyChanged(property);
            _notifier.FireThreadSafe(property);
        }

        private void OnInternalChoicePropertyChanged(DelegateListener listener) => FirePropertyChanged(listener.Watch);

        private void OnChosenValueChanged(DelegateListener listener)
        {
            if (_lastChosen != null)
                _lastChosen.Selected = false;
            _lastChosen = ChosenValue as ModelItem;
            if (_lastChosen != null)
                _lastChosen.Selected = true;
            FireChangedChosenEvent();
            OnInternalChoicePropertyChanged(listener);
        }

        internal virtual Microsoft.Iris.ModelItems.Choice CreateInternalChoice() => new Microsoft.Iris.ModelItems.Choice();

        private void ValidateOptionsList(IList potentialOptionsWrapped, IList potentialOptions)
        {
            using (ThreadValidator)
            {
                string error;
                if (!_choice.ValidateOptionsList(potentialOptionsWrapped, out error))
                    throw new ArgumentException(error);
                ValidateOptionsListWorker(potentialOptions);
            }
        }

        protected virtual void ValidateOptionsListWorker(IList potentialOptions)
        {
        }

        private void FireChangedChosenEvent()
        {
            if (GetEventHandler(s_chosenChangedEvent) is EventHandler eventHandler)
                eventHandler(this, EventArgs.Empty);
            OnChosenChanged();
        }

        private void SetChosenSelected(bool selectedFlag)
        {
            if (!(ChosenValue is ModelItem chosenValue))
                return;
            chosenValue.Selected = selectedFlag;
        }

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
