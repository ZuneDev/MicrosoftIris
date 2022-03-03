// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.UICommand
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.ModelItems
{
    internal class UICommand : NotifyObjectBase, IUICommand
    {
        private bool _availableFlag = true;
        private InvokePriority _priority;

        public virtual bool Available
        {
            get => _availableFlag;
            set
            {
                if (_availableFlag == value)
                    return;
                _availableFlag = value;
                FireNotification(NotificationID.Available);
            }
        }

        public InvokePriority Priority
        {
            get => _priority;
            set
            {
                if (_priority == value)
                    return;
                _priority = value;
                FireNotification(NotificationID.Priority);
            }
        }

        public void Invoke()
        {
            if (Priority == InvokePriority.Normal)
                InvokeWorker();
            else
                DeferredCall.Post(DispatchPriority.Idle, new SimpleCallback(InvokeWorker));
        }

        public void InvokeWorker()
        {
            FireNotification(NotificationID.Invoked);
            OnInvoked();
        }

        protected virtual void OnInvoked()
        {
        }
    }
}
