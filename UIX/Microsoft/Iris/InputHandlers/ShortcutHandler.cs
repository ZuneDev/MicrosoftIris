// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.ShortcutHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.InputHandlers
{
    internal class ShortcutHandler : InputHandler
    {
        private ShortcutHandlerCommand _shortcut;
        private IUICommand _command;
        private bool _handle;

        public ShortcutHandler() => _handle = true;

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            UI.KeyInteractive = true;
        }

        public ShortcutHandlerCommand Shortcut
        {
            get => _shortcut;
            set
            {
                if (_shortcut == value)
                    return;
                _shortcut = value;
                FireNotification(NotificationID.Shortcut);
            }
        }

        public IUICommand Command
        {
            get => _command;
            set
            {
                if (_command == value)
                    return;
                _command = value;
                FireNotification(NotificationID.Command);
            }
        }

        public bool Handle
        {
            get => _handle;
            set
            {
                if (_handle == value)
                    return;
                _handle = value;
                FireNotification(NotificationID.Handle);
            }
        }

        protected override void OnCommandDown(UIClass ui, KeyCommandInfo info)
        {
            if (info.Command != (CommandCode)_shortcut)
                return;
            InvokeCommand();
            if (!_handle)
                return;
            info.MarkHandled();
        }

        protected override void OnCommandUp(UIClass ui, KeyCommandInfo info)
        {
            if (info.Command != (CommandCode)_shortcut || !_handle)
                return;
            info.MarkHandled();
        }

        private void InvokeCommand()
        {
            FireNotification(NotificationID.Invoked);
            if (_command == null)
                return;
            _command.Invoke();
        }

        public override string ToString() => InvariantString.Format("{0}({1})", GetType().Name, _shortcut);
    }
}
