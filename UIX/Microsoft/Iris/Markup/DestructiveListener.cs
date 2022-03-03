// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.DestructiveListener
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal class DestructiveListener : MarkupListener
    {
        private uint _refreshId;

        public void Reset(
          INotifyObject notifier,
          string watch,
          IMarkupTypeBase scriptHost,
          uint scriptId,
          uint refreshId)
        {
            Reset(notifier, watch, scriptHost, scriptId);
            _refreshId = refreshId;
        }

        public override void OnNotify()
        {
            base.OnNotify();
            ScriptHost.ScheduleScriptRun(_refreshId, true);
        }
    }
}
