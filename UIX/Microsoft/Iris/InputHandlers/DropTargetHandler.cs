// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.DropTargetHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.InputHandlers
{
    internal class DropTargetHandler : InputHandler
    {
        private bool _dragging;
        private DropAction _allowedDropActions;
        private WeakReference _eventContext;

        internal DropTargetHandler() => UISession.Default.Form.EnableExternalDragDrop = true;

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            UI.MouseInteractive = true;
        }

        public DropAction AllowedDropActions
        {
            get => _allowedDropActions;
            set
            {
                if (_allowedDropActions == value)
                    return;
                _allowedDropActions = value;
                FireNotification(NotificationID.AllowedDropActions);
                if (!Dragging)
                    return;
                DragDropHelper.OnAllowedDropActionsChanged();
            }
        }

        public bool Dragging => _dragging;

        public object EventContext => CheckEventContext(ref _eventContext);

        protected override void OnDragEnter(UIClass ui, DragDropInfo info)
        {
            _dragging = true;
            DragDropHelper.TargetHandler = this;
            FireNotification(NotificationID.Dragging);
            FireNotification(NotificationID.DragEnter);
            SetEventContext(info.Target, ref _eventContext, NotificationID.EventContext);
            info.MarkHandled();
            base.OnDragEnter(ui, info);
        }

        protected override void OnDragOver(UIClass ui, DragDropInfo info)
        {
            if (Dragging)
            {
                FireNotification(NotificationID.DragOver);
                info.MarkHandled();
            }
            base.OnDragOver(ui, info);
        }

        protected override void OnDragLeave(UIClass ui, DragDropInfo info)
        {
            if (Dragging)
            {
                EndDrag(NotificationID.DragLeave);
                info.MarkHandled();
                SetEventContext(null, ref _eventContext, NotificationID.EventContext);
            }
            base.OnDragLeave(ui, info);
        }

        protected override void OnDropped(UIClass ui, DragDropInfo info)
        {
            if (Dragging)
            {
                EndDrag(NotificationID.Dropped);
                info.MarkHandled();
            }
            base.OnDropped(ui, info);
        }

        private void EndDrag(string eventName)
        {
            _dragging = false;
            FireNotification(NotificationID.Dragging);
            FireNotification(eventName);
            DragDropHelper.TargetHandler = null;
        }

        public object GetValue() => DragDropHelper.GetValue();
    }
}
