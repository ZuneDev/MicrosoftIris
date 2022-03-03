// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateObject
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal abstract class ValidateObject : Microsoft.Iris.Markup.Validation.Validate
    {
        private ObjectSourceType _objectSourceType;
        private ValidateObject _next;

        public ValidateObject(
          SourceMarkupLoader owner,
          int line,
          int column,
          ObjectSourceType objectSourceType)
          : base(owner, line, column)
        {
            _objectSourceType = objectSourceType;
        }

        public abstract void Validate(TypeRestriction typeRestriction, ValidateContext context);

        public abstract TypeSchema ObjectType { get; }

        public ObjectSourceType ObjectSourceType => _objectSourceType;

        public ValidateObject Next
        {
            get => _next;
            set => _next = value;
        }

        public void AppendToEnd(ValidateObject item)
        {
            ValidateObject validateObject = this;
            while (validateObject.Next != null)
                validateObject = validateObject.Next;
            validateObject.Next = item;
        }
    }
}
