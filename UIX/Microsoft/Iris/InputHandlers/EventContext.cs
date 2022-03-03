// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.EventContext
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.InputHandlers
{
    internal class EventContext : InputHandler
    {
        private object _value;

        protected override void ConfigureInteractivity() => UI.SetEventContext(this);

        public object Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                FireNotification(NotificationID.Value);
            }
        }
    }
}
