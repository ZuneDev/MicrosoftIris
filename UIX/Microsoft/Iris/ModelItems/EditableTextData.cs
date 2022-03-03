// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.EditableTextData
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.ModelItems
{
    internal class EditableTextData : NotifyObjectBase
    {
        private int _maxLength;
        private bool _readOnly;
        private string _value;

        public EditableTextData()
        {
            _value = string.Empty;
            _maxLength = int.MaxValue;
        }

        public string Value
        {
            get => _value;
            set
            {
                if (ReadOnly || !(_value != value))
                    return;
                _value = value;
                FireCodeEvent(ValueChanged);
                FireThreadSafeNotification(NotificationID.Value);
            }
        }

        public int MaxLength
        {
            get => _maxLength;
            set
            {
                if (_maxLength == value)
                    return;
                _maxLength = value;
                FireCodeEvent(MaxLengthChanged);
                FireThreadSafeNotification(NotificationID.MaxLength);
            }
        }

        private void FireCodeEvent(EventHandler eventToFire)
        {
            if (eventToFire == null)
                return;
            eventToFire(this, EventArgs.Empty);
        }

        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                if (_readOnly == value)
                    return;
                _readOnly = value;
                FireCodeEvent(ReadOnlyChanged);
                FireThreadSafeNotification(NotificationID.ReadOnly);
            }
        }

        public event EventHandler Submitted;

        public event EventHandler MaxLengthChanged;

        public event EventHandler ReadOnlyChanged;

        public event EventHandler ValueChanged;

        public void Submit() => DeferredCall.Post(DispatchPriority.AppEvent, new SimpleCallback(AsyncInvoke));

        private void AsyncInvoke()
        {
            FireCodeEvent(Submitted);
            FireThreadSafeNotification(NotificationID.Submitted);
            OnSubmitted();
        }

        protected virtual void OnSubmitted()
        {
        }
    }
}
