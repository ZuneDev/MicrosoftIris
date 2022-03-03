// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.IntRangedValue
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris
{
    public class IntRangedValue : RangedValue, IUIIntRangedValue, IUIRangedValue, IUIValueRange, INotifyObject
    {
        public IntRangedValue(IModelItemOwner owner, string description) : base(owner, description)
        {
        }

        public IntRangedValue(IModelItemOwner owner) : this(owner, null)
        {
        }

        public IntRangedValue() : this(null)
        {
        }

        public int Value
        {
            get => (int)base.Value;
            set => base.Value = value;
        }

        public int MinValue
        {
            get => (int)base.MinValue;
            set => base.MinValue = value;
        }

        public int MaxValue
        {
            get => (int)base.MaxValue;
            set => base.MaxValue = value;
        }

        public int Step
        {
            get => (int)base.Step;
            set => base.Step = value;
        }

        internal override ModelItems.RangedValue CreateInternalRangedValue() => new ModelItems.IntRangedValue();
    }
}
