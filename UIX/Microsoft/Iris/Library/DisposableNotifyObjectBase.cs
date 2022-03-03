// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.DisposableNotifyObjectBase
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;

namespace Microsoft.Iris.Library
{
    internal class DisposableNotifyObjectBase : DisposableObject, INotifyObject
    {
        private NotifyService _notifier = new NotifyService();

        protected DisposableNotifyObjectBase()
        {
        }

        protected void FireNotification(string id) => _notifier.Fire(id);

        protected void FireThreadSafeNotification(string id) => _notifier.FireThreadSafe(id);

        public void AddListener(Listener listener) => _notifier.AddListener(listener);
    }
}
