// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.DelegateListener
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal class DelegateListener : Listener
    {
        private DelegateListener.OnNotifyCallback _onNotify;

        public DelegateListener(
          INotifyObject notifier,
          string watch,
          DelegateListener.OnNotifyCallback callback)
        {
            _watch = watch;
            _onNotify = callback;
            notifier.AddListener(this);
        }

        public override void Dispose()
        {
            base.Dispose();
            _onNotify = null;
        }

        public override void OnNotify()
        {
            base.OnNotify();
            _onNotify(this);
        }

        public delegate void OnNotifyCallback(DelegateListener listener);
    }
}
