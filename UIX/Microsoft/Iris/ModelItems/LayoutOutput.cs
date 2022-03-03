// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.LayoutOutput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.ModelItems
{
    internal class LayoutOutput : NotifyObjectBase
    {
        private Size _size;

        public LayoutOutput(Size size) => _size = size;

        public Size Size
        {
            get => _size;
            private set
            {
                if (!(_size != value))
                    return;
                _size = value;
                FireNotification(NotificationID.Size);
            }
        }

        public void OnLayoutComplete(Size size) => Size = size;
    }
}
