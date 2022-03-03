// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.Choice
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using System.Collections;

namespace Microsoft.Iris.ModelItems
{
    internal class Choice :
      DisposableNotifyObjectBase,
      IUIChoice,
      IUIValueRange,
      IDisposableObject,
      INotifyObject
    {
        protected IList _options;
        protected int _chosen;
        private int _default;
        private bool _wrap;
        private static int s_noSelectionSentinal = -1;

        public Choice()
        {
            _chosen = s_noSelectionSentinal;
            _default = 0;
        }

        protected override void OnDispose()
        {
            SetOptions(null, true);
            base.OnDispose();
        }

        public IList Options
        {
            get => _options;
            set => SetOptions(value, false);
        }

        private void SetOptions(IList value, bool disposing)
        {
            if (_options == value)
                return;
            if (_options != null && _options is INotifyList options)
                options.ContentsChanged -= new UIListContentsChangedHandler(OnListContentsChanged);
            _options = value;
            if (value != null && value is INotifyList notifyList)
                notifyList.ContentsChanged += new UIListContentsChangedHandler(OnListContentsChanged);
            if (disposing)
                return;
            int chosen = _chosen;
            Clear();
            if (ValidateIndex(chosen))
                SetChosenIndex(chosen);
            else if (!ListUtility.IsNullOrEmpty(_options))
                SetChosenIndex(0);
            FireNotification(NotificationID.Options);
        }

        private bool ValidateIndex(int index) => ValidateIndex(index, out string _);

        public bool ValidateIndex(int index, out string error)
        {
            bool flag = true;
            error = null;
            if (_options != null && (index < 0 || index >= _options.Count))
            {
                error = string.Format("Selected Index {0} is not a valid index in SourceList of size {1}", index, _options.Count);
                flag = false;
            }
            return flag;
        }

        public bool ValidateOption(object option, out int index, out string error)
        {
            bool flag = false;
            index = -1;
            if (_options != null)
            {
                index = _options.IndexOf(option);
                if (index >= 0)
                    flag = true;
            }
            error = !flag ? string.Format("Script runtime failure: Invalid '{0}' value  for '{1}'", option, "ChosenValue") : null;
            return flag;
        }

        public virtual bool ValidateOptionsList(IList options, out string error)
        {
            error = null;
            return true;
        }

        public object ChosenValue => !HasSelection || OptionsCount == 0 ? null : _options[_chosen];

        public int ChosenIndex
        {
            get => _chosen;
            set => SetChosenIndex(value);
        }

        public int DefaultIndex
        {
            get => _default;
            set
            {
                if (_default == value)
                    return;
                _default = value;
                FireNotification(NotificationID.DefaultIndex);
            }
        }

        private void SetChosenIndex(int index)
        {
            if (_chosen == index)
                return;
            bool hasSelection = HasSelection;
            bool hasPreviousValue;
            bool hasNextValue;
            CapturePrevNextState(out hasPreviousValue, out hasNextValue);
            _chosen = index;
            FireNotification(NotificationID.ChosenIndex);
            FireNotification(NotificationID.ChosenValue);
            FireNotification(NotificationID.Value);
            if (HasSelection != hasSelection)
                FireNotification(NotificationID.HasSelection);
            FirePrevNextNotifications(hasPreviousValue, hasNextValue);
        }

        public bool HasSelection => _chosen != s_noSelectionSentinal && _options != null;

        public bool HasPreviousValue => HasPreviousValueWorker(_wrap);

        public bool HasPreviousValueWorker(bool wrap)
        {
            if (!HasSelection)
                return false;
            return wrap || _chosen > 0;
        }

        public bool HasNextValue => HasNextValueWorker(_wrap);

        public bool HasNextValueWorker(bool wrap)
        {
            if (!HasSelection)
                return false;
            return wrap || _chosen < OptionsCount - 1;
        }

        public bool Wrap
        {
            get => _wrap;
            set
            {
                if (_wrap == value)
                    return;
                bool hasPreviousValue;
                bool hasNextValue;
                CapturePrevNextState(out hasPreviousValue, out hasNextValue);
                _wrap = value;
                FireNotification(NotificationID.Wrap);
                FirePrevNextNotifications(hasPreviousValue, hasNextValue);
            }
        }

        private void CapturePrevNextState(out bool hasPreviousValue, out bool hasNextValue)
        {
            hasPreviousValue = HasPreviousValue;
            hasNextValue = HasNextValue;
        }

        private void FirePrevNextNotifications(bool hadPreviousValue, bool hadNextValue)
        {
            if (hadPreviousValue != HasPreviousValue)
                FireNotification(NotificationID.HasPreviousValue);
            if (hadNextValue == HasNextValue)
                return;
            FireNotification(NotificationID.HasNextValue);
        }

        private int OptionsCount
        {
            get
            {
                int num = 0;
                if (_options != null)
                    num = _options.Count;
                return num;
            }
        }

        public void PreviousValue() => PreviousValue(_wrap);

        public void PreviousValue(bool wrap)
        {
            if (!HasPreviousValueWorker(wrap))
                return;
            int index = _chosen - 1;
            if (index < 0)
                index = OptionsCount - 1;
            SetChosenIndex(index);
        }

        public void NextValue() => NextValue(_wrap);

        public void NextValue(bool wrap)
        {
            if (!HasNextValueWorker(wrap))
                return;
            int index = _chosen + 1;
            if (index >= OptionsCount)
                index = 0;
            SetChosenIndex(index);
        }

        public void DefaultValue()
        {
            if (_default < 0 || _default >= OptionsCount)
                return;
            SetChosenIndex(_default);
        }

        object IUIValueRange.ObjectValue => ChosenValue;

        public void Clear() => SetChosenIndex(s_noSelectionSentinal);

        private void OnListContentsChanged(IList senderList, UIListContentsChangedArgs args)
        {
            UIListContentsChangeType type = args.Type;
            int oldIndex = args.OldIndex;
            int newIndex = args.NewIndex;
            int count = args.Count;
            switch (type)
            {
                case UIListContentsChangeType.Add:
                case UIListContentsChangeType.AddRange:
                case UIListContentsChangeType.Insert:
                case UIListContentsChangeType.InsertRange:
                    if (_chosen < newIndex)
                        break;
                    SetChosenIndex(_chosen + count);
                    break;
                case UIListContentsChangeType.Remove:
                    if (oldIndex == _chosen)
                    {
                        Clear();
                        break;
                    }
                    if (_chosen <= oldIndex)
                        break;
                    SetChosenIndex(_chosen - 1);
                    break;
                case UIListContentsChangeType.Move:
                    if (oldIndex == newIndex || _chosen < oldIndex)
                        break;
                    if (_chosen == oldIndex)
                    {
                        SetChosenIndex(newIndex);
                        break;
                    }
                    if (_chosen >= newIndex)
                        break;
                    SetChosenIndex(newIndex - 1);
                    break;
                case UIListContentsChangeType.Clear:
                case UIListContentsChangeType.Reset:
                    Clear();
                    break;
            }
        }
    }
}
