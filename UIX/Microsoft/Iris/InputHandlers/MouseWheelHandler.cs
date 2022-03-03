﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.MouseWheelHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Markup;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.InputHandlers
{
    internal class MouseWheelHandler : InputHandler
    {
        private bool _handle;

        public MouseWheelHandler() => _handle = true;

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            UI.MouseInteractive = true;
            UI.KeyInteractive = true;
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

        protected override void OnMouseWheel(UIClass ui, MouseWheelInfo info)
        {
            if (info.WheelDelta > 0)
                FireNotification(NotificationID.UpInvoked);
            else
                FireNotification(NotificationID.DownInvoked);
            if (!_handle)
                return;
            info.MarkHandled();
        }
    }
}
