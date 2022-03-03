// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.PropertySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup
{
    internal abstract class PropertySchema : DisposableObject
    {
        public static PropertySchema[] EmptyList = new PropertySchema[0];
        private TypeSchema _owner;

        public PropertySchema(TypeSchema owner)
        {
            _owner = owner;
            DeclareOwner(owner);
        }

        public TypeSchema Owner => _owner;

        public abstract string Name { get; }

        public abstract TypeSchema PropertyType { get; }

        public abstract TypeSchema AlternateType { get; }

        public abstract bool CanRead { get; }

        public abstract bool CanWrite { get; }

        public abstract bool IsStatic { get; }

        public abstract ExpressionRestriction ExpressionRestriction { get; }

        public abstract bool RequiredForCreation { get; }

        public abstract RangeValidator RangeValidator { get; }

        public abstract bool NotifiesOnChange { get; }

        public abstract object GetValue(object instance);

        public abstract void SetValue(ref object instance, object value);
    }
}
