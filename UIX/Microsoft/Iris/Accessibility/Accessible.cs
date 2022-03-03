// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Accessibility.Accessible
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Accessibility
{
    internal class Accessible : NotifyObjectBase
    {
        private DynamicData _dataMap;
        private AccessibleProxy _proxy;
        private static readonly DataCookie s_descriptionSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_defaultActionSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_defaultActionCommandSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_helpSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_helpTopicSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_keyboardShortcutSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_nameSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_roleSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_valueSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_animatedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_unavailableStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_selectedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_busyStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_pressedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_checkedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_collapsedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_defaultStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_marqueeStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_mixedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_expandedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_traversedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_selectableStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_protectedStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_popupStateSlot = DataCookie.ReserveSlot();
        private static readonly DataCookie s_multiSelectableStateSlot = DataCookie.ReserveSlot();

        public Accessible() => SetData(s_roleSlot, AccRole.Client);

        public void Attach(AccessibleProxy proxy)
        {
            _proxy = proxy;
            FireNotification(NotificationID.Enabled);
        }

        public void Detach()
        {
            SetData(s_defaultActionCommandSlot, null);
            _proxy = null;
        }

        public bool Enabled => _proxy != null;

        public string Description
        {
            get => (string)GetData(s_descriptionSlot);
            set
            {
                string description = Description;
                if (!(value != description))
                    return;
                SetData(s_descriptionSlot, value);
                FireAccessiblePropertyChanged(NotificationID.Description, AccessibleProperty.Description);
            }
        }

        public string DefaultAction
        {
            get => (string)GetData(s_defaultActionSlot);
            set
            {
                string defaultAction = DefaultAction;
                if (!(value != defaultAction))
                    return;
                SetData(s_defaultActionSlot, value);
                FireAccessiblePropertyChanged(NotificationID.DefaultAction, AccessibleProperty.DefaultAction);
            }
        }

        public IUICommand DefaultActionCommand
        {
            get => (IUICommand)GetData(s_defaultActionCommandSlot);
            set
            {
                IUICommand defaultActionCommand = DefaultActionCommand;
                if (value == defaultActionCommand)
                    return;
                SetData(s_defaultActionCommandSlot, value);
                FireAccessiblePropertyChanged(NotificationID.DefaultActionCommand, AccessibleProperty.DefaultActionCommand);
            }
        }

        public string Help
        {
            get => (string)GetData(s_helpSlot);
            set
            {
                string help = Help;
                if (!(value != help))
                    return;
                SetData(s_helpSlot, value);
                FireAccessiblePropertyChanged(NotificationID.Help, AccessibleProperty.Help);
            }
        }

        public int HelpTopic
        {
            get
            {
                object data = GetData(s_helpTopicSlot);
                return data != null ? (int)data : -1;
            }
            set
            {
                int helpTopic = HelpTopic;
                if (value == helpTopic)
                    return;
                SetData(s_helpTopicSlot, value);
                FireAccessiblePropertyChanged(NotificationID.HelpTopic, AccessibleProperty.HelpTopic);
            }
        }

        public string KeyboardShortcut
        {
            get => (string)GetData(s_keyboardShortcutSlot);
            set
            {
                string keyboardShortcut = KeyboardShortcut;
                if (!(value != keyboardShortcut))
                    return;
                SetData(s_keyboardShortcutSlot, value);
                FireAccessiblePropertyChanged(NotificationID.KeyboardShortcut, AccessibleProperty.KeyboardShortcut);
            }
        }

        public string Name
        {
            get => (string)GetData(s_nameSlot);
            set
            {
                string name = Name;
                if (!(value != name))
                    return;
                SetData(s_nameSlot, value);
                FireAccessiblePropertyChanged(NotificationID.Name, AccessibleProperty.Name);
            }
        }

        public AccRole Role
        {
            get
            {
                object data = GetData(s_roleSlot);
                return data != null ? (AccRole)data : AccRole.None;
            }
            set
            {
                AccRole role = Role;
                if (value == role)
                    return;
                SetData(s_roleSlot, value);
                FireAccessiblePropertyChanged(NotificationID.Role, AccessibleProperty.Role);
            }
        }

        public string Value
        {
            get => (string)GetData(s_valueSlot);
            set
            {
                string str = Value;
                if (!(value != str))
                    return;
                SetData(s_valueSlot, value);
                FireAccessiblePropertyChanged(NotificationID.Value, AccessibleProperty.Value);
            }
        }

        public bool IsAnimated
        {
            get
            {
                object data = GetData(s_animatedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isAnimated = IsAnimated;
                if (value == isAnimated)
                    return;
                SetData(s_animatedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsAnimated, AccessibleProperty.IsAnimated);
            }
        }

        public bool IsUnavailable
        {
            get
            {
                object data = GetData(s_unavailableStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isUnavailable = IsUnavailable;
                if (value == isUnavailable)
                    return;
                SetData(s_unavailableStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsUnavailable, AccessibleProperty.IsUnavailable);
            }
        }

        public bool IsSelected
        {
            get
            {
                object data = GetData(s_selectedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isSelected = IsSelected;
                if (value == isSelected)
                    return;
                SetData(s_selectedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsSelected, AccessibleProperty.IsSelected);
            }
        }

        public bool IsBusy
        {
            get
            {
                object data = GetData(s_busyStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isBusy = IsBusy;
                if (value == isBusy)
                    return;
                SetData(s_busyStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsBusy, AccessibleProperty.IsBusy);
            }
        }

        public bool IsPressed
        {
            get
            {
                object data = GetData(s_pressedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isPressed = IsPressed;
                if (value == isPressed)
                    return;
                SetData(s_pressedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsPressed, AccessibleProperty.IsPressed);
            }
        }

        public bool IsChecked
        {
            get
            {
                object data = GetData(s_checkedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isChecked = IsChecked;
                if (value == isChecked)
                    return;
                SetData(s_checkedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsChecked, AccessibleProperty.IsChecked);
            }
        }

        public bool IsCollapsed
        {
            get
            {
                object data = GetData(s_collapsedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isCollapsed = IsCollapsed;
                if (value == isCollapsed)
                    return;
                SetData(s_collapsedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsCollapsed, AccessibleProperty.IsCollapsed);
            }
        }

        public bool IsDefault
        {
            get
            {
                object data = GetData(s_defaultStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isDefault = IsDefault;
                if (value == isDefault)
                    return;
                SetData(s_defaultStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsDefault, AccessibleProperty.IsDefault);
            }
        }

        public bool IsMarquee
        {
            get
            {
                object data = GetData(s_marqueeStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isMarquee = IsMarquee;
                if (value == isMarquee)
                    return;
                SetData(s_marqueeStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsMarquee, AccessibleProperty.IsMarquee);
            }
        }

        public bool IsMixed
        {
            get
            {
                object data = GetData(s_mixedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isMixed = IsMixed;
                if (value == isMixed)
                    return;
                SetData(s_mixedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsMixed, AccessibleProperty.IsMixed);
            }
        }

        public bool IsExpanded
        {
            get
            {
                object data = GetData(s_expandedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isExpanded = IsExpanded;
                if (value == isExpanded)
                    return;
                SetData(s_expandedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsExpanded, AccessibleProperty.IsExpanded);
            }
        }

        public bool IsTraversed
        {
            get
            {
                object data = GetData(s_traversedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isTraversed = IsTraversed;
                if (value == isTraversed)
                    return;
                SetData(s_traversedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsTraversed, AccessibleProperty.IsTraversed);
            }
        }

        public bool IsSelectable
        {
            get
            {
                object data = GetData(s_selectableStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isSelectable = IsSelectable;
                if (value == isSelectable)
                    return;
                SetData(s_selectableStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsSelectable, AccessibleProperty.IsSelectable);
            }
        }

        public bool IsMultiSelectable
        {
            get
            {
                object data = GetData(s_multiSelectableStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isMultiSelectable = IsMultiSelectable;
                if (value == isMultiSelectable)
                    return;
                SetData(s_multiSelectableStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsMultiSelectable, AccessibleProperty.IsMultiSelectable);
            }
        }

        public bool IsProtected
        {
            get
            {
                object data = GetData(s_protectedStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool isProtected = IsProtected;
                if (value == isProtected)
                    return;
                SetData(s_protectedStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.IsProtected, AccessibleProperty.IsProtected);
            }
        }

        public bool HasPopup
        {
            get
            {
                object data = GetData(s_popupStateSlot);
                return data != null && (bool)data;
            }
            set
            {
                bool hasPopup = HasPopup;
                if (value == hasPopup)
                    return;
                SetData(s_popupStateSlot, value);
                FireAccessiblePropertyChanged(NotificationID.HasPopup, AccessibleProperty.HasPopup);
            }
        }

        protected void FireAccessiblePropertyChanged(
          string propertyName,
          AccessibleProperty accessibleProperty)
        {
            FireNotification(propertyName);
            if (_proxy != null)
            {
                _proxy.NotifyAccessiblePropertyChanged(accessibleProperty);
            }
            else
            {
                if (accessibleProperty == AccessibleProperty.Name)
                    return;
                ErrorManager.ReportWarning("Accessibility: Script modifications to the 'Accessible' object ('{0}' property) detected even though an Accessibility client is not is use. Use 'if (Accessible.Enabled) {{ ... }}' to bypass Accessible property access in this case", propertyName);
            }
        }

        protected object GetData(DataCookie cookie) => _dataMap.GetData(cookie);

        protected void SetData(DataCookie cookie, object value) => _dataMap.SetData(cookie, value);
    }
}
