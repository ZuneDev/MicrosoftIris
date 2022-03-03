// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.BooleanChoice
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using System.Collections;

namespace Microsoft.Iris
{
    public class BooleanChoice :
      Choice,
      IUIBooleanChoice,
      IUIChoice,
      IUIValueRange,
      IDisposableObject,
      INotifyObject
    {
        public BooleanChoice(IModelItemOwner owner, string description, IList options)
          : base(owner, description)
        {
            if (options == null)
                return;
            Options = options;
        }

        public BooleanChoice(IModelItemOwner owner, string description)
          : this(owner, description, null)
        {
        }

        public BooleanChoice(IModelItemOwner owner)
          : this(owner, null, null)
        {
        }

        public BooleanChoice()
          : this(null)
        {
        }

        internal override Microsoft.Iris.ModelItems.Choice CreateInternalChoice() => new Microsoft.Iris.ModelItems.BooleanChoice();

        public bool Value
        {
            get
            {
                using (ThreadValidator)
                    return ChosenIndex == 1;
            }
            set
            {
                using (ThreadValidator)
                {
                    if (value == Value)
                        return;
                    if (value)
                        ChosenIndex = 1;
                    else
                        ChosenIndex = 0;
                }
            }
        }
    }
}
