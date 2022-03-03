// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.AssemblyObjectProxyHelper
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.ViewItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Iris.Markup
{
    internal static class AssemblyObjectProxyHelper
    {
        private static AssemblyObjectProxyHelper.ProxyTypeInfo[] s_proxyTypeInfoTable;
        private static Type s_typeofString;

        public static void InitializeStatics()
        {
            s_typeofString = typeof(string);
            s_proxyTypeInfoTable = new AssemblyObjectProxyHelper.ProxyTypeInfo[7]
            {
        new AssemblyObjectProxyHelper.ProxyTypeInfo(typeof (ICommand), typeof (AssemblyObjectProxyHelper.ProxyCommand),  CommandSchema.Type),
        new AssemblyObjectProxyHelper.ProxyTypeInfo(typeof (IValueRange), typeof (AssemblyObjectProxyHelper.ProxyValueRange),  ValueRangeSchema.Type),
        new AssemblyObjectProxyHelper.ProxyTypeInfo(typeof (IList), typeof (AssemblyObjectProxyHelper.ProxyList),  ListSchema.Type),
        new AssemblyObjectProxyHelper.ProxyTypeInfo(typeof (IDictionary), typeof (AssemblyObjectProxyHelper.ProxyDictionary),  DictionarySchema.Type),
        new AssemblyObjectProxyHelper.ProxyTypeInfo(typeof (IEnumerator), typeof (AssemblyObjectProxyHelper.ProxyListEnumerator),  EnumeratorSchema.Type),
        new AssemblyObjectProxyHelper.ProxyTypeInfo(typeof (IDisposable), typeof (AssemblyObjectProxyHelper.ProxyObject),  null),
        new AssemblyObjectProxyHelper.ProxyTypeInfo(typeof (INotifyPropertyChanged), typeof (AssemblyObjectProxyHelper.ProxyObject),  null)
            };
        }

        public static AssemblyTypeSchema CreateProxySchema(Type assemblyType)
        {
            foreach (AssemblyObjectProxyHelper.ProxyTypeInfo proxyTypeInfo in s_proxyTypeInfoTable)
            {
                if (proxyTypeInfo.type.IsAssignableFrom(assemblyType))
                {
                    AssemblyTypeSchema assemblyTypeSchema = new FrameworkCompatibleAssemblyTypeSchema(assemblyType, proxyTypeInfo.proxyType);
                    if (proxyTypeInfo.equivalents != null)
                        assemblyTypeSchema.ShareEquivalents(proxyTypeInfo.equivalents);
                    return assemblyTypeSchema;
                }
            }
            return new StandardAssemblyTypeSchema(assemblyType);
        }

        internal static Type ProxyListType => typeof(AssemblyObjectProxyHelper.ProxyList);

        internal static Type ProxyDictionaryType => typeof(AssemblyObjectProxyHelper.ProxyDictionary);

        internal static Type ProxyCommandType => typeof(AssemblyObjectProxyHelper.ProxyCommand);

        internal static Type ProxyValueRangeType => typeof(AssemblyObjectProxyHelper.ProxyValueRange);

        internal static object WrapObject(TypeSchema typeSchema, object instance)
        {
            if (instance == null)
                return null;
            if (instance.GetType().IsPrimitive)
                return instance;
            switch (instance)
            {
                case Type type:
                    return AssemblyLoadResult.MapType(type);
                case AssemblyObjectProxyHelper.IFrameworkProxyObject frameworkProxyObject:
                    return frameworkProxyObject.FrameworkObject;
                default:
                    AssemblyObjectProxyHelper.ProxyObject proxyObject = null;
                    bool isDisposable = instance is IDisposable;
                    bool notifiesOnChange = instance is INotifyPropertyChanged;
                    switch (instance)
                    {
                        case ICommand _:
                            proxyObject = new AssemblyObjectProxyHelper.ProxyCommand(instance);
                            break;
                        case IValueRange _:
                            proxyObject = new AssemblyObjectProxyHelper.ProxyValueRange(instance);
                            break;
                        case IDictionary _:
                            proxyObject = new AssemblyObjectProxyHelper.ProxyDictionary(instance);
                            break;
                        case IList _:
                            proxyObject = !(instance is Group) ? (!(instance is IVirtualList) ? (!(instance is INotifyList) ? new AssemblyObjectProxyHelper.ProxyList(instance) : new AssemblyObjectProxyHelper.ProxyNotifyList(instance)) : new AssemblyObjectProxyHelper.ProxyVirtualNotifyList(instance, instance is INotifyList)) : new AssemblyObjectProxyHelper.ProxyGroup(instance, instance is INotifyList);
                            break;
                        default:
                            if (isDisposable || notifiesOnChange)
                            {
                                proxyObject = new AssemblyObjectProxyHelper.ProxyObject(instance);
                                break;
                            }
                            break;
                    }
                    if (proxyObject == null)
                        return instance;
                    proxyObject.SetIntrinsicState(isDisposable, notifiesOnChange);
                    return proxyObject;
            }
        }

        internal static object UnwrapObject(object instance)
        {
            if (instance == null)
                return null;
            Type type = instance.GetType();
            if (type.IsPrimitive || type == s_typeofString)
                return instance;
            switch (instance)
            {
                case AssemblyObjectProxyHelper.IAssemblyProxyObject assemblyProxyObject:
                    return assemblyProxyObject.AssemblyObject;
                case IList uixList:
                    return new AssemblyObjectProxyHelper.ReverseProxyList(uixList);
                case IDictionary _:
                case IDisposable _:
                    return new AssemblyObjectProxyHelper.WrappedFrameworkObject(instance);
                default:
                    return instance;
            }
        }

        private struct ProxyTypeInfo
        {
            public Type type;
            public Type proxyType;
            public Vector<TypeSchema> equivalents;

            public ProxyTypeInfo(Type type, Type proxyType, TypeSchema equivalence)
            {
                this.type = type;
                this.proxyType = proxyType;
                equivalents = null;
                if (equivalence == null)
                    return;
                equivalents = new Vector<TypeSchema>(1);
                equivalents.Add(equivalence);
            }
        }

        public interface IFrameworkProxyObject
        {
            object FrameworkObject { get; }
        }

        private class WrappedFrameworkObject : AssemblyObjectProxyHelper.IFrameworkProxyObject
        {
            private object _frameworkObject;

            public WrappedFrameworkObject(object frameworkObject) => _frameworkObject = frameworkObject;

            public object FrameworkObject => _frameworkObject;
        }

        public interface IAssemblyProxyObject
        {
            object AssemblyObject { get; }
        }

        private class ProxyObject :
          DisposableObject,
          INotifyObject,
          AssemblyObjectProxyHelper.IAssemblyProxyObject,
          IModelItemOwner,
          ISchemaInfo
        {
            protected object _assemblyObject;
            private bool _isDisposable;
            private bool _isModelItemOwner;
            private bool _hasPropertyChangedHandlerAttached;
            private NotifyService _notifier = new NotifyService();

            public ProxyObject(object assemblyObject) => _assemblyObject = assemblyObject;

            public void SetIntrinsicState(bool isDisposable, bool notifiesOnChange) => _isDisposable = isDisposable;

            protected override void OnDispose()
            {
                if (_hasPropertyChangedHandlerAttached)
                {
                    ((INotifyPropertyChanged)_assemblyObject).PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
                    _hasPropertyChangedHandlerAttached = false;
                }
                _notifier.ClearListeners();
                if (_isDisposable && (!(_assemblyObject is ModelItem) || _isModelItemOwner))
                    ((IDisposable)_assemblyObject).Dispose();
                base.OnDispose();
            }

            protected override void OnOwnerDeclared(object owner)
            {
                base.OnOwnerDeclared(owner);
                if (!(_assemblyObject is ModelItem assemblyObject))
                    return;
                assemblyObject.Owner = this;
            }

            public void AddListener(Listener listener)
            {
                if (!_hasPropertyChangedHandlerAttached)
                {
                    ((INotifyPropertyChanged)_assemblyObject).PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
                    _hasPropertyChangedHandlerAttached = true;
                }
                _notifier.AddListener(listener);
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                _notifier.FireThreadSafe(args.PropertyName);
                if (_notifier.HasListeners)
                    return;
                ((INotifyPropertyChanged)_assemblyObject).PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
                _hasPropertyChangedHandlerAttached = false;
            }

            public void RegisterObject(ModelItem modelItem)
            {
                if (modelItem != _assemblyObject)
                    throw new ArgumentException("Unexpected model item provided for ownership registration");
                _isModelItemOwner = true;
            }

            public void UnregisterObject(ModelItem modelItem)
            {
                if (!_isModelItemOwner || modelItem != _assemblyObject)
                    throw new ArgumentException("Unexpected model item provided for ownership unregistration");
                _isModelItemOwner = false;
            }

            public object AssemblyObject => _assemblyObject;

            public TypeSchema TypeSchema => AssemblyLoadResult.MapType(_assemblyObject.GetType());

            public override bool Equals(object rhs)
            {
                if (rhs is AssemblyObjectProxyHelper.IAssemblyProxyObject)
                    rhs = ((AssemblyObjectProxyHelper.IAssemblyProxyObject)rhs).AssemblyObject;
                return _assemblyObject.Equals(rhs);
            }

            public override int GetHashCode() => _assemblyObject.GetHashCode();

            public override string ToString() => _assemblyObject.ToString();
        }

        private class ProxyCommand : AssemblyObjectProxyHelper.ProxyObject, IUICommand
        {
            public ProxyCommand(object assemblyObject)
              : base(assemblyObject)
            {
            }

            protected ICommand ExternalCommand => (ICommand)_assemblyObject;

            public bool Available
            {
                get => ExternalCommand.Available;
                set => ExternalCommand.Available = value;
            }

            public InvokePriority Priority
            {
                get
                {
                    switch (ExternalCommand.Priority)
                    {
                        case InvokePolicy.Synchronous:
                        case InvokePolicy.AsynchronousNormal:
                            return InvokePriority.Normal;
                        default:
                            return InvokePriority.Low;
                    }
                }
                set
                {
                    if (value == InvokePriority.Normal)
                        ExternalCommand.Priority = InvokePolicy.AsynchronousNormal;
                    else
                        ExternalCommand.Priority = InvokePolicy.AsynchronousLowPri;
                }
            }

            public void Invoke() => ExternalCommand.Invoke();
        }

        private class ProxyValueRange : AssemblyObjectProxyHelper.ProxyObject, IUIValueRange
        {
            public ProxyValueRange(object assemblyObject)
              : base(assemblyObject)
            {
            }

            protected IValueRange ExternalRange => (IValueRange)_assemblyObject;

            public object ObjectValue => AssemblyLoadResult.WrapObject(ExternalRange.Value);

            public bool HasPreviousValue => ExternalRange.HasPreviousValue;

            public bool HasNextValue => ExternalRange.HasNextValue;

            public void PreviousValue() => ExternalRange.PreviousValue();

            public void NextValue() => ExternalRange.NextValue();
        }

        private class ProxyList :
          AssemblyObjectProxyHelper.ProxyObject,
          IUIList,
          IList,
          ICollection,
          IEnumerable
        {
            private bool _canSearch;

            public ProxyList(object assemblyObject)
              : base(assemblyObject)
              => _canSearch = assemblyObject is ISearchableList;

            protected IList ExternalList => (IList)_assemblyObject;

            public int Add(object value) => ExternalList.Add(AssemblyLoadResult.UnwrapObject(value));

            public void Clear() => ExternalList.Clear();

            public bool Contains(object value) => ExternalList.Contains(AssemblyLoadResult.UnwrapObject(value));

            public int IndexOf(object value) => ExternalList.IndexOf(AssemblyLoadResult.UnwrapObject(value));

            public void Insert(int index, object value) => ExternalList.Insert(index, AssemblyLoadResult.UnwrapObject(value));

            public bool IsFixedSize => ExternalList.IsFixedSize;

            public bool IsReadOnly => ExternalList.IsReadOnly;

            public void Remove(object value) => ExternalList.Remove(AssemblyLoadResult.UnwrapObject(value));

            public void RemoveAt(int index) => ExternalList.RemoveAt(index);

            public object this[int index]
            {
                get => AssemblyLoadResult.WrapObject(ExternalList[index]);
                set => ExternalList[index] = AssemblyLoadResult.UnwrapObject(value);
            }

            public void CopyTo(Array array, int index)
            {
                int index1 = index;
                for (int index2 = 0; index2 < ExternalList.Count; ++index2)
                {
                    array.SetValue(AssemblyLoadResult.WrapObject(ExternalList[index2]), index1);
                    ++index1;
                }
            }

            public int Count => ExternalList.Count;

            public bool IsSynchronized => ExternalList.IsSynchronized;

            public object SyncRoot => ExternalList.SyncRoot;

            public IEnumerator GetEnumerator() => new AssemblyObjectProxyHelper.ProxyListEnumerator(ExternalList.GetEnumerator());

            public bool CanSearch => _canSearch;

            public int SearchForString(string searchString) => _canSearch ? ((ISearchableList)ExternalList).SearchForString(searchString) : -1;

            public virtual void Move(int oldIndex, int newIndex)
            {
                object external = ExternalList[oldIndex];
                RemoveAt(oldIndex);
                Insert(newIndex, external);
            }
        }

        private class ProxyListEnumerator : IEnumerator, AssemblyObjectProxyHelper.IAssemblyProxyObject
        {
            private IEnumerator _assemblyEnumerator;

            public ProxyListEnumerator(object assemblyObject) => _assemblyEnumerator = (IEnumerator)assemblyObject;

            public bool MoveNext() => _assemblyEnumerator.MoveNext();

            public void Reset() => _assemblyEnumerator.Reset();

            public object Current => AssemblyLoadResult.WrapObject(_assemblyEnumerator.Current);

            public object AssemblyObject => _assemblyEnumerator;
        }

        private class ProxyNotifyList :
          AssemblyObjectProxyHelper.ProxyList,
          INotifyList,
          IList,
          ICollection,
          IEnumerable
        {
            private bool _isNotifyList;
            private UIListContentsChangedHandler _listContentsChangedHandler;
            private Delegate _handlersAttachedToMe;

            public ProxyNotifyList(object assemblyObject, bool isNotifyList)
              : base(assemblyObject)
              => _isNotifyList = isNotifyList;

            public ProxyNotifyList(object assemblyObject)
              : this(assemblyObject, true)
            {
            }

            protected INotifyList ExternalNotifyList => (INotifyList)_assemblyObject;

            public override void Move(int oldIndex, int newIndex) => ExternalNotifyList.Move(oldIndex, newIndex);

            public event UIListContentsChangedHandler ContentsChanged
            {
                add
                {
                    if (!_isNotifyList)
                        return;
                    if (_listContentsChangedHandler == null)
                    {
                        _listContentsChangedHandler = new UIListContentsChangedHandler(OnListContentsChanged);
                        ExternalNotifyList.ContentsChanged += _listContentsChangedHandler;
                    }
                    _handlersAttachedToMe = Delegate.Combine(_handlersAttachedToMe, value);
                }
                remove
                {
                    if (!_isNotifyList)
                        return;
                    _handlersAttachedToMe = Delegate.Remove(_handlersAttachedToMe, value);
                    if ((object)_handlersAttachedToMe != null)
                        return;
                    ExternalNotifyList.ContentsChanged -= _listContentsChangedHandler;
                    _listContentsChangedHandler = null;
                }
            }

            private void OnListContentsChanged(IList senderList, UIListContentsChangedArgs args) => ((UIListContentsChangedHandler)_handlersAttachedToMe)(this, args);
        }

        private class ProxyVirtualNotifyList :
          AssemblyObjectProxyHelper.ProxyNotifyList,
          IVirtualList,
          INotifyList,
          IList,
          ICollection,
          IEnumerable
        {
            private ItemRequestCallback _onItemGeneratedCallback;
            private Dictionary<object, object> _itemCallbacks;

            public ProxyVirtualNotifyList(object assemblyObject, bool isNotifyList)
              : base(assemblyObject, isNotifyList)
            {
            }

            protected IVirtualList ExternalVirtualList => (IVirtualList)_assemblyObject;

            public void RequestItem(int index, ItemRequestCallback callback)
            {
                if (_itemCallbacks == null)
                    _itemCallbacks = new Dictionary<object, object>();
                _itemCallbacks[index] = callback;
                if (_onItemGeneratedCallback == null)
                    _onItemGeneratedCallback = new ItemRequestCallback(OnItemGenerated);
                ExternalVirtualList.RequestItem(index, _onItemGeneratedCallback);
            }

            public bool IsItemAvailable(int index) => ExternalVirtualList.IsItemAvailable(index);

            public void NotifyVisualsCreated(int index) => ExternalVirtualList.NotifyVisualsCreated(index);

            public void NotifyVisualsReleased(int index) => ExternalVirtualList.NotifyVisualsReleased(index);

            public bool SlowDataRequestsEnabled => ExternalVirtualList.SlowDataRequestsEnabled;

            public void NotifyRequestSlowData(int index) => ExternalVirtualList.NotifyRequestSlowData(index);

            public Repeater RepeaterHost
            {
                get => ExternalVirtualList.RepeaterHost;
                set => ExternalVirtualList.RepeaterHost = value;
            }

            public SlowDataAcquireCompleteHandler SlowDataAcquireCompleteHandler
            {
                get => ExternalVirtualList.SlowDataAcquireCompleteHandler;
                set => ExternalVirtualList.SlowDataAcquireCompleteHandler = value;
            }

            private void OnItemGenerated(object sender, int index, object item)
            {
                ItemRequestCallback itemCallback = (ItemRequestCallback)_itemCallbacks[index];
                _itemCallbacks.Remove(index);
                itemCallback(this, index, AssemblyLoadResult.WrapObject(item));
            }
        }

        private class ProxyGroup :
          AssemblyObjectProxyHelper.ProxyVirtualNotifyList,
          IUIGroup,
          IList,
          ICollection,
          IEnumerable
        {
            public ProxyGroup(object assemblyObject, bool isNotifyList)
              : base(assemblyObject, isNotifyList)
            {
            }

            protected Group ExternalGroup => (Group)_assemblyObject;

            public int StartIndex => ExternalGroup.StartIndex;

            public int EndIndex => ExternalGroup.EndIndex;
        }

        private class ProxyDictionary :
          AssemblyObjectProxyHelper.ProxyObject,
          IDictionary,
          ICollection,
          IEnumerable
        {
            public ProxyDictionary(object assemblyObject)
              : base(assemblyObject)
            {
            }

            protected IDictionary ExternalDictionary => (IDictionary)_assemblyObject;

            public bool IsReadOnly => ExternalDictionary.IsReadOnly;

            public bool Contains(object key) => ExternalDictionary.Contains(AssemblyLoadResult.WrapObject(key));

            public bool IsFixedSize => false;

            public void Remove(object key)
            {
            }

            public void Clear() => ExternalDictionary.Clear();

            public void Add(object key, object value) => ExternalDictionary.Add(AssemblyLoadResult.WrapObject(key), AssemblyLoadResult.WrapObject(value));

            public ICollection Keys => (ICollection)null;

            public ICollection Values => (ICollection)null;

            public object this[object key]
            {
                get => AssemblyLoadResult.UnwrapObject(ExternalDictionary[AssemblyLoadResult.WrapObject(key)]);
                set => ExternalDictionary[AssemblyLoadResult.WrapObject(key)] = AssemblyLoadResult.WrapObject(value);
            }

            public void CopyTo(Array array, int index)
            {
            }

            public int Count => ExternalDictionary.Count;

            public bool IsSynchronized => false;

            public object SyncRoot => (object)null;

            public IEnumerator GetEnumerator() => (IEnumerator)null;

            IDictionaryEnumerator IDictionary.GetEnumerator() => (IDictionaryEnumerator)null;
        }

        private class ReverseProxyList :
          IList,
          ICollection,
          IEnumerable,
          AssemblyObjectProxyHelper.IFrameworkProxyObject
        {
            private IList _uixList;

            public ReverseProxyList(IList uixList) => _uixList = uixList;

            public object FrameworkObject => _uixList;

            public int Add(object value) => _uixList.Add(AssemblyLoadResult.WrapObject(value));

            public void Clear() => _uixList.Clear();

            public bool Contains(object value) => _uixList.Contains(AssemblyLoadResult.WrapObject(value));

            public int IndexOf(object value) => _uixList.IndexOf(AssemblyLoadResult.WrapObject(value));

            public void Insert(int index, object value) => _uixList.Insert(index, AssemblyLoadResult.WrapObject(value));

            public bool IsFixedSize => _uixList.IsFixedSize;

            public bool IsReadOnly => _uixList.IsReadOnly;

            public void Remove(object value) => _uixList.Remove(AssemblyLoadResult.WrapObject(value));

            public void RemoveAt(int index) => _uixList.RemoveAt(index);

            public object this[int index]
            {
                get => AssemblyLoadResult.UnwrapObject(_uixList[index]);
                set => _uixList[index] = AssemblyLoadResult.WrapObject(value);
            }

            public void CopyTo(Array array, int index)
            {
                int index1 = index;
                for (int index2 = 0; index2 < _uixList.Count; ++index2)
                {
                    array.SetValue(AssemblyLoadResult.UnwrapObject(_uixList[index2]), index1);
                    ++index1;
                }
            }

            public int Count => _uixList.Count;

            public bool IsSynchronized => _uixList.IsSynchronized;

            public object SyncRoot => _uixList.SyncRoot;

            public IEnumerator GetEnumerator() => new AssemblyObjectProxyHelper.ReverseProxyListEnumerator(_uixList.GetEnumerator());
        }

        private class ReverseProxyListEnumerator :
          IEnumerator,
          AssemblyObjectProxyHelper.IFrameworkProxyObject
        {
            private IEnumerator _uixEnumerator;

            public ReverseProxyListEnumerator(IEnumerator uixEnumerator) => _uixEnumerator = uixEnumerator;

            public bool MoveNext() => _uixEnumerator.MoveNext();

            public void Reset() => _uixEnumerator.Reset();

            public object Current => AssemblyLoadResult.UnwrapObject(_uixEnumerator.Current);

            public object FrameworkObject => _uixEnumerator;
        }
    }
}
