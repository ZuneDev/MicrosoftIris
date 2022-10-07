// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupListeners
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    public class MarkupListeners : Listeners
    {
        public MarkupListeners(int listenerCount)
          : base(listenerCount)
        {
        }

        public void RefreshListener(
          int index,
          INotifyObject notifier,
          string watch,
          IMarkupTypeBase scriptHost,
          uint scriptOffset,
          uint refreshOffset)
        {
            MarkupListener markupListener = null;
            if (_listenerList[index] is MarkupListener)
            {
                markupListener = (MarkupListener)_listenerList[index];
                markupListener.Dispose();
            }
            if (refreshOffset == uint.MaxValue)
            {
                if (markupListener == null)
                    markupListener = new MarkupListener();
                markupListener.Reset(notifier, watch, scriptHost, scriptOffset);
            }
            else
            {
                if (markupListener == null)
                    markupListener = new DestructiveListener();
                ((DestructiveListener)markupListener).Reset(notifier, watch, scriptHost, scriptOffset, refreshOffset);
            }
            _listenerList[index] = markupListener;
        }
    }
}
