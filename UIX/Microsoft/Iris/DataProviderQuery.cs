// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.DataProviderQuery
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Iris
{
    public abstract class DataProviderQuery :
      IDataProviderQuery,
      IDataProviderBaseObject,
      AssemblyObjectProxyHelper.IFrameworkProxyObject,
      IDisposableObject,
      INotifyPropertyChanged
    {
        private MarkupDataQuerySchema _typeSchema;
        private MarkupDataQuery _internalQuery;
        private Dictionary<string, object> _propertyValues;
        private object _result;
        private DataProviderQueryStatus _status;
        private bool _isDisposed;
        private bool _isInvalid;
        private bool _enabled = true;
        private bool _initialized;

        protected abstract void BeginExecute();

        public object ResultTypeCookie => _typeSchema.ResultType;

        public object Result
        {
            get => _result;
            set
            {
                if (Equals(_result, value))
                    return;
                _result = value;
                FirePropertyChanged(nameof(Result));
            }
        }

        public DataProviderQueryStatus Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;
                _status = value;
                FirePropertyChanged(nameof(Status));
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
                FirePropertyChanged(nameof(Enabled));
                if (!_enabled || !_isInvalid)
                    return;
                DeferredBeginExecute(null);
            }
        }

        protected bool Initialized => _initialized;

        public void Refresh() => BeginExecute();

        public virtual object GetProperty(string propertyName)
        {
            lock (SynchronizedPropertyStorage)
            {
                if (_propertyValues != null)
                {
                    object obj;
                    if (_propertyValues.TryGetValue(propertyName, out obj))
                        return obj;
                }
            }
            return null;
        }

        public virtual bool TryGetProperty<T>(string propertyName, out T value)
        {
            try
            {
                value = (T)GetProperty(propertyName);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            bool flag1 = false;
            lock (SynchronizedPropertyStorage)
            {
                bool flag2 = false;
                if (_propertyValues == null)
                {
                    _propertyValues = new Dictionary<string, object>();
                    flag2 = true;
                }
                if (!flag2 && _propertyValues.ContainsKey(propertyName))
                {
                    if (Equals(_propertyValues[propertyName], value))
                        goto label_8;
                }
                _propertyValues[propertyName] = value;
                flag1 = true;
            }
        label_8:
            if (!flag1)
                return;
            FirePropertyChanged(propertyName);
        }

        protected void FirePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
            if (_internalQuery != null)
                _internalQuery.FireNotificationThreadSafe(propertyName);
            bool invalidatesQuery = _typeSchema.InvalidatesQuery(propertyName);
            if (PropertyChanged != null)
                PropertyChanged(this, new DataProviderPropertyChangedEventArgs(propertyName, invalidatesQuery));
            if (_isInvalid || !invalidatesQuery)
                return;
            _isInvalid = true;
            if (!_enabled)
                return;
            Application.DeferredInvoke(new DeferredInvokeHandler(DeferredBeginExecute), DeferredInvokePriority.Low);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void DeferredBeginExecute(object args)
        {
            if (!_isInvalid)
                return;
            _isInvalid = false;
            BeginExecute();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
        }

        protected virtual void OnDispose()
        {
        }

        public bool IsDisposed => _isDisposed;

        void IDisposableObject.DeclareOwner(object owner) => DeclareOwnerWorker(owner);

        void IDataProviderQuery.DeclareOwner(object owner) => DeclareOwnerWorker(owner);

        private void DeclareOwnerWorker(object owner)
        {
        }

        void IDisposableObject.TransferOwnership(object owner)
        {
        }

        void IDisposableObject.Dispose(object owner) => DisposeWorker(owner);

        void IDataProviderQuery.Dispose(object owner) => DisposeWorker(owner);

        private void DisposeWorker(object owner)
        {
            if (_isDisposed)
                return;
            OnDispose();
            _isDisposed = true;
        }

        protected DataProviderQuery(object typeCookie)
        {
            _typeSchema = typeCookie as MarkupDataQuerySchema;
            if (_typeSchema == null)
                throw new ArgumentException(nameof(typeCookie), "typeCookie must be the queryTypeCookie passed to ConstructQuery");
            _isInvalid = true;
        }

        object AssemblyObjectProxyHelper.IFrameworkProxyObject.FrameworkObject => _internalQuery;

        internal string ProviderName => _typeSchema.ProviderName;

        void IDataProviderQuery.SetInternalObject(MarkupDataQuery internalQuery) => _internalQuery = internalQuery;

        void IDataProviderQuery.OnInitialize()
        {
            _initialized = true;
            if (!_enabled)
                return;
            DeferredBeginExecute(null);
        }

        public override string ToString() => _typeSchema.Name;

        private object SynchronizedPropertyStorage => _internalQuery;
    }
}
