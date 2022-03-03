// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.Index
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.ViewItems
{
    internal class Index : NotifyObjectBase
    {
        private int _virtualIndex;
        private int _dataIndex;
        private Repeater _repeater;

        public Index(int virtualIndex, int dataIndex, Repeater repeater)
        {
            _virtualIndex = virtualIndex;
            _dataIndex = dataIndex;
            _repeater = repeater;
        }

        public int Value => _virtualIndex;

        public int SourceValue => _dataIndex;

        public void SetValue(int virtualIndex, int dataIndex)
        {
            if (_virtualIndex != virtualIndex)
            {
                _virtualIndex = virtualIndex;
                FireNotification(NotificationID.Value);
            }
            if (_dataIndex == dataIndex)
                return;
            _dataIndex = dataIndex;
            FireNotification(NotificationID.SourceValue);
        }

        public Index GetContainerIndex()
        {
            ViewItem viewItem = _repeater;
            while (!(viewItem.Parent is Repeater))
            {
                viewItem = viewItem.Parent;
                if (viewItem == null)
                    return null;
            }
            return ((IndexLayoutInput)viewItem.GetLayoutInput(IndexLayoutInput.Data)).Index;
        }
    }
}
