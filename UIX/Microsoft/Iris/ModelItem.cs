// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.Iris
{
    public class ModelItem :
      IModelItem,
      INotifyPropertyChanged,
      IModelItemOwner,
      IDisposable,
      IThreadSafeObject
    {
        private static readonly EventCookie s_propertyChangedEvent = EventCookie.ReserveSlot();
        private static readonly EventCookie s_focusRequestedEvent = EventCookie.ReserveSlot();
        private static readonly DataCookie s_descriptionProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_uniqueIdProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_selectedProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_selectionPolicyProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_ownedObjectsProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_extraDataProperty = DataCookie.ReserveSlot();
        private IModelItemOwner _owner;
        private DynamicData _dataMap;
        private bool _isDisposed;
        private Thread _affinity;

        public ModelItem(IModelItemOwner owner, string description)
        {
            ThreadSafety.InitializeObject(this);
            _dataMap = new DynamicData();
            _dataMap.Create();
            SetData(s_descriptionProperty, description);
            Owner = owner;
        }

        public ModelItem(IModelItemOwner owner)
          : this(owner, null)
        {
        }

        public ModelItem()
          : this(null)
        {
        }

        ~ModelItem()
        {
            string name = GetType().Name;
            string data = (string)GetData(s_descriptionProperty);
            OnDispose(false);
        }

        public void Dispose() => Dispose(ModelItemDisposeMode.RemoveOwnerReference);

        public void Dispose(ModelItemDisposeMode disposeMode)
        {
            using (ThreadValidator)
            {
                if (_isDisposed)
                    return;
                if (_owner != null)
                {
                    if (disposeMode == ModelItemDisposeMode.RemoveOwnerReference)
                        _owner.UnregisterObject(this);
                    _owner = null;
                }
                try
                {
                    GC.SuppressFinalize(this);
                    OnDispose(true);
                }
                finally
                {
                    try
                    {
                        DisposeOwnedObjects();
                    }
                    finally
                    {
                        _isDisposed = true;
                    }
                }
            }
        }

        protected virtual void OnDispose(bool disposing)
        {
        }

        protected ThreadSafetyBlock ThreadValidator => new ThreadSafetyBlock(this);

        Thread IThreadSafeObject.Affinity
        {
            get => _affinity;
            set => _affinity = value;
        }

        public IModelItemOwner Owner
        {
            get
            {
                using (ThreadValidator)
                    return _owner;
            }
            set
            {
                using (ThreadValidator)
                {
                    if (_owner == value)
                        return;
                    IModelItemOwner owner = _owner;
                    owner?.UnregisterObject(this);
                    _owner = value;
                    if (_owner != null)
                        _owner.RegisterObject(this);
                    OnOwnerChanged(_owner, owner);
                }
            }
        }

        public bool IsDisposed
        {
            get
            {
                using (ThreadValidator)
                    return _isDisposed;
            }
        }

        public string Description
        {
            get
            {
                using (ThreadValidator)
                    return (string)GetData(s_descriptionProperty);
            }
            set
            {
                using (ThreadValidator)
                {
                    if (!(Description != value))
                        return;
                    SetData(s_descriptionProperty, value);
                    FirePropertyChanged(nameof(Description));
                }
            }
        }

        public Guid UniqueId
        {
            get
            {
                using (ThreadValidator)
                {
                    object data = GetData(s_uniqueIdProperty);
                    return data == null ? Guid.Empty : (Guid)data;
                }
            }
            set
            {
                using (ThreadValidator)
                {
                    if (!(UniqueId != value))
                        return;
                    SetData(s_uniqueIdProperty, value);
                    FirePropertyChanged(nameof(UniqueId));
                }
            }
        }

        public IDictionary Data
        {
            get
            {
                using (ThreadValidator)
                {
                    IDictionary dictionary = (IDictionary)GetData(s_extraDataProperty);
                    if (dictionary == null)
                    {
                        dictionary = new HybridDictionary();
                        SetData(s_extraDataProperty, dictionary);
                    }
                    return dictionary;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                using (ThreadValidator)
                    AddEventHandler(s_propertyChangedEvent, value);
            }
            remove
            {
                using (ThreadValidator)
                    RemoveEventHandler(s_propertyChangedEvent, value);
            }
        }

        protected void FirePropertyChanged(string property)
        {
            using (ThreadValidator)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));
                OnPropertyChanged(property);
                if (!(GetEventHandler(s_propertyChangedEvent) is PropertyChangedEventHandler eventHandler))
                    return;
                eventHandler(this, new PropertyChangedEventArgs(property));
            }
        }

        protected virtual void OnPropertyChanged(string property)
        {
        }

        protected virtual void OnOwnerChanged(IModelItemOwner newOwner, IModelItemOwner oldOwner)
        {
        }

        void IModelItemOwner.RegisterObject(ModelItem item)
        {
            using (ThreadValidator)
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item));
                if (item == this)
                    throw new ArgumentException("Cannot make a ModelItem the owner of itself");
                GetOwnedObjects(true).Add(item);
            }
        }

        void IModelItemOwner.UnregisterObject(ModelItem item)
        {
            using (ThreadValidator)
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item));
                Vector<ModelItem> ownedObjects = GetOwnedObjects(false);
                if (ownedObjects == null || !ownedObjects.Contains(item))
                    throw new ArgumentException(InvariantString.Format("Cannot unregister an object that was never registered.  Owner \"{0}\" was unable to identify \"{1}\".", this, item));
                ownedObjects.Remove(item);
            }
        }

        private void DisposeOwnedObjects()
        {
            Vector<ModelItem> ownedObjects = GetOwnedObjects(false);
            if (ownedObjects == null)
                return;
            foreach (ModelItem modelItem in ownedObjects)
                modelItem.Dispose(ModelItemDisposeMode.KeepOwnerReference);
            SetData(s_ownedObjectsProperty, null);
        }

        private Vector<ModelItem> GetOwnedObjects(bool createIfNoneFlag)
        {
            Vector<ModelItem> vector = (Vector<ModelItem>)GetData(s_ownedObjectsProperty);
            if (vector == null && createIfNoneFlag)
            {
                vector = new Vector<ModelItem>();
                SetData(s_ownedObjectsProperty, vector);
            }
            return vector;
        }

        public bool Selected
        {
            get
            {
                using (ThreadValidator)
                {
                    object data = GetData(s_selectedProperty);
                    return data != null && (bool)data;
                }
            }
            set
            {
                using (ThreadValidator)
                {
                    if (Selected == value)
                        return;
                    SetData(s_selectedProperty, value);
                    FirePropertyChanged(nameof(Selected));
                }
            }
        }

        public override string ToString()
        {
            using (ThreadValidator)
            {
                string name = GetType().Name;
                string description = Description;
                return description != null ? InvariantString.Format("{0}:\"{1}\"", name, description) : name;
            }
        }

        internal object GetData(DataCookie cookie) => _dataMap.GetData(cookie);

        internal void SetData(DataCookie cookie, object value) => _dataMap.SetData(cookie, value);

        internal Delegate GetEventHandler(EventCookie cookie) => _dataMap.GetEventHandler(cookie);

        internal void AddEventHandler(EventCookie cookie, Delegate handlerToAdd) => _dataMap.AddEventHandler(cookie, handlerToAdd);

        internal void RemoveEventHandler(EventCookie cookie, Delegate handlerToRemove) => _dataMap.RemoveEventHandler(cookie, handlerToRemove);

        internal void RemoveEventHandlers(EventCookie cookie) => _dataMap.RemoveEventHandlers(cookie);

        private static uint GetKey(EventCookie cookie)
        {
            uint uint32 = EventCookie.ToUInt32(cookie);
            return uint32 != 0U ? uint32 : throw new ArgumentException("invalid event key");
        }
    }
}
