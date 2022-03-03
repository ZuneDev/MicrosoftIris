// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllProxyList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.ViewItems;
using System;
using System.Collections;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllProxyList :
      DllInterfaceProxy,
      IVirtualList,
      INotifyList,
      IList,
      ICollection,
      IEnumerable,
      IUIXListCallbacks
    {
        private UpdateHelper _updater;
        private Repeater _repeater;
        private bool _wantSlowDataRequests;
        private bool _callbacksRegistered;
        private SlowDataAcquireCompleteHandler _slowDataAcquireCompleteHandler;
        private UIListContentsChangedHandler _contentsChanged;

        protected override void LoadWorker(IntPtr nativeObject, IntPtr nativeMarshalAs)
        {
            base.LoadWorker(nativeObject, nativeMarshalAs);
            NativeApi.SUCCEEDED(NativeApi.SpUIXListWantSlowDataRequests(_interface, out _wantSlowDataRequests));
            if (!_wantSlowDataRequests)
                return;
            _updater = new UpdateHelper(this);
        }

        protected override void OnDispose()
        {
            UpdateCallbackRegistration(true);
            base.OnDispose();
        }

        public unsafe int Add(object value)
        {
            int count = 0;
            UIXVariant uixVariant;
            UIXVariant.MarshalObject(value, &uixVariant);
            NativeApi.SUCCEEDED(NativeApi.SpUIXListAdd(_interface, &uixVariant, out count));
            UIXVariant.CleanupMarshalledObject(&uixVariant);
            return count;
        }

        public bool Contains(object value) => IndexOf(value) >= 0;

        public unsafe int IndexOf(object value)
        {
            int index = -1;
            UIXVariant uixVariant;
            UIXVariant.MarshalObject(value, &uixVariant);
            NativeApi.SUCCEEDED(NativeApi.SpUIXListIndexOf(_interface, &uixVariant, out index));
            UIXVariant.CleanupMarshalledObject(&uixVariant);
            return index;
        }

        public void Clear() => NativeApi.SUCCEEDED(NativeApi.SpUIXListClear(_interface));

        public unsafe void Insert(int index, object value)
        {
            UIXVariant uixVariant;
            UIXVariant.MarshalObject(value, &uixVariant);
            NativeApi.SUCCEEDED(NativeApi.SpUIXListInsert(_interface, index, &uixVariant));
            UIXVariant.CleanupMarshalledObject(&uixVariant);
        }

        public unsafe void Remove(object value)
        {
            UIXVariant uixVariant;
            UIXVariant.MarshalObject(value, &uixVariant);
            NativeApi.SUCCEEDED(NativeApi.SpUIXListRemove(_interface, &uixVariant));
            UIXVariant.CleanupMarshalledObject(&uixVariant);
        }

        public void RemoveAt(int index) => NativeApi.SUCCEEDED(NativeApi.SpUIXListRemoveAt(_interface, index));

        public void CopyTo(Array array, int index) => throw new NotImplementedException();

        public int Count
        {
            get
            {
                int count = 0;
                NativeApi.SUCCEEDED(NativeApi.SpUIXListGetCount(_interface, out count));
                return count;
            }
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        bool IVirtualList.IsItemAvailable(int index)
        {
            bool isAvailable;
            NativeApi.SUCCEEDED(NativeApi.SpUIXListIsItemAvailable(_interface, index, out isAvailable));
            return isAvailable;
        }

        public void RequestItem(int index, ItemRequestCallback callback)
        {
            object obj = this[index];
            callback(this, index, obj);
        }

        public unsafe object this[int index]
        {
            get
            {
                object obj = null;
                UIXVariant inboundObject;
                if (NativeApi.SUCCEEDED(NativeApi.SpUIXListGetItem(_interface, index, out inboundObject)))
                    obj = UIXVariant.GetValue(inboundObject, OwningLoadResult);
                return obj;
            }
            set
            {
                UIXVariant uixVariant;
                UIXVariant.MarshalObject(value, &uixVariant);
                NativeApi.SUCCEEDED(NativeApi.SpUIXListSetItem(_interface, index, &uixVariant));
                UIXVariant.CleanupMarshalledObject(&uixVariant);
            }
        }

        public object SyncRoot => (object)null;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public StackIListEnumerator GetEnumerator() => new StackIListEnumerator(this);

        public event UIListContentsChangedHandler ContentsChanged
        {
            add
            {
                if (_contentsChanged == null)
                    UpdateCallbackRegistration();
                _contentsChanged += value;
            }
            remove
            {
                _contentsChanged -= value;
                if (_contentsChanged != null)
                    return;
                UpdateCallbackRegistration();
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            int num = (int)NativeApi.SpUIXListMove(_interface, oldIndex, newIndex);
        }

        public void ListChanged(int nativeType, int oldIndex, int newIndex, int count)
        {
            bool flag1 = true;
            bool flag2 = true;
            UIListContentsChangeType type = (UIListContentsChangeType)nativeType;
            switch (type)
            {
                case UIListContentsChangeType.Add:
                case UIListContentsChangeType.AddRange:
                case UIListContentsChangeType.Remove:
                case UIListContentsChangeType.Insert:
                case UIListContentsChangeType.InsertRange:
                case UIListContentsChangeType.Clear:
                case UIListContentsChangeType.Reset:
                    if (!flag1)
                        break;
                    if (_contentsChanged != null)
                        _contentsChanged(this, new UIListContentsChangedArgs(type, oldIndex, newIndex, count));
                    if (flag2)
                        FireNotification(NotificationID.Count);
                    if (_updater == null)
                        break;
                    ForwardListChangeToUpdater(type, oldIndex, newIndex, count);
                    break;
                case UIListContentsChangeType.Move:
                case UIListContentsChangeType.Modified:
                    flag2 = false;
                    goto case UIListContentsChangeType.Add;
                default:
                    flag1 = false;
                    goto case UIListContentsChangeType.Add;
            }
        }

        private void ForwardListChangeToUpdater(
          UIListContentsChangeType type,
          int oldIndex,
          int newIndex,
          int count)
        {
            switch (type)
            {
                case UIListContentsChangeType.Remove:
                    _updater.RemoveIndex(oldIndex);
                    _updater.AdjustIndices(oldIndex, -1);
                    break;
                case UIListContentsChangeType.Move:
                    int lowThreshold;
                    int highThreshold;
                    int amt;
                    if (oldIndex < newIndex)
                    {
                        lowThreshold = oldIndex;
                        highThreshold = newIndex;
                        amt = -1;
                    }
                    else
                    {
                        lowThreshold = newIndex;
                        highThreshold = oldIndex;
                        amt = 1;
                    }
                    _updater.RemoveIndex(oldIndex);
                    _updater.AdjustIndices(lowThreshold, highThreshold, amt);
                    _updater.AddIndex(newIndex);
                    break;
                case UIListContentsChangeType.Insert:
                    _updater.AdjustIndices(newIndex, 1);
                    break;
                case UIListContentsChangeType.InsertRange:
                    _updater.AdjustIndices(newIndex, count);
                    break;
                case UIListContentsChangeType.Clear:
                case UIListContentsChangeType.Reset:
                    _updater.Clear();
                    break;
            }
        }

        public bool SlowDataRequestsEnabled => _wantSlowDataRequests;

        public void NotifyRequestSlowData(int index) => NativeApi.SUCCEEDED(NativeApi.SpUIXListFetchSlowData(_interface, index));

        public void SlowDataAcquireComplete(int index)
        {
            bool flag = false;
            if (_slowDataAcquireCompleteHandler != null)
                flag = _slowDataAcquireCompleteHandler(this, index);
            if (flag)
                return;
            _updater.NotifySlowDataAcquireComplete(index);
        }

        public SlowDataAcquireCompleteHandler SlowDataAcquireCompleteHandler
        {
            get => _slowDataAcquireCompleteHandler;
            set
            {
                if (!(value != _slowDataAcquireCompleteHandler))
                    return;
                _slowDataAcquireCompleteHandler = value;
                UpdateCallbackRegistration();
            }
        }

        public void NotifyVisualsCreated(int index)
        {
            if (_updater != null)
                _updater.AddIndex(index);
            NativeApi.SUCCEEDED(NativeApi.SpUIXListNotifyVisualsCreated(_interface, index));
        }

        public void NotifyVisualsReleased(int index)
        {
            if (_updater != null)
                _updater.RemoveIndex(index);
            NativeApi.SUCCEEDED(NativeApi.SpUIXListNotifyVisualsReleased(_interface, index));
        }

        public Repeater RepeaterHost
        {
            get => _repeater;
            set
            {
                if (value == _repeater)
                    return;
                _repeater = value;
                UpdateCallbackRegistration();
            }
        }

        private void UpdateCallbackRegistration() => UpdateCallbackRegistration(false);

        private void UpdateCallbackRegistration(bool inDispose)
        {
            bool flag = (_slowDataAcquireCompleteHandler != null || _contentsChanged != null || _repeater != null) & !inDispose;
            if (flag == _callbacksRegistered)
                return;
            if (flag)
            {
                int num1 = (int)NativeApi.SpUIXListRegisterCallbacks(_interface, this);
            }
            else
            {
                int num2 = (int)NativeApi.SpUIXListUnregisterCallbacks(_interface, this);
                if (_updater != null)
                    _updater.Clear();
            }
            _callbacksRegistered = flag;
        }
    }
}
