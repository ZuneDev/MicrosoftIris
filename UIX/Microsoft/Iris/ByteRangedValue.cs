// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ByteRangedValue
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris
{
    public class ByteRangedValue : RangedValue, IUIByteRangedValue, IUIRangedValue, IUIValueRange, INotifyObject
    {
        public ByteRangedValue(IModelItemOwner owner, string description) : base(owner, description)
        {
        }

        public ByteRangedValue(IModelItemOwner owner) : this(owner, null)
        {
        }

        public ByteRangedValue() : this(null)
        {
        }

        public byte Value
        {
            get => (byte)base.Value;
            set => base.Value = value;
        }

        public byte MinValue
        {
            get => (byte)base.MinValue;
            set => base.MinValue = value;
        }

        public byte MaxValue
        {
            get => (byte)base.MaxValue;
            set => base.MaxValue = value;
        }

        public byte Step
        {
            get => (byte)base.Step;
            set => base.Step = value;
        }

        internal override ModelItems.RangedValue CreateInternalRangedValue() => new ModelItems.ByteRangedValue();
    }
}
