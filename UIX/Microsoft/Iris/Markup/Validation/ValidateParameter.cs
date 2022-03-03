// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateParameter
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateParameter : Microsoft.Iris.Markup.Validation.Validate
    {
        private ValidateExpression _expression;
        private TypeSchema _foundParameterType;
        private ValidateParameter _next;
        public static ValidateParameter EmptyList;

        public static void InitializeStatics() => EmptyList = new ValidateParameter();

        public ValidateParameter(
          SourceMarkupLoader owner,
          ValidateExpression expression,
          int line,
          int column)
          : base(owner, line, column)
        {
            _expression = expression;
        }

        protected ValidateParameter()
        {
        }

        public ValidateExpression Expression => _expression;

        public ValidateParameter Next
        {
            get => _next;
            set => _next = value;
        }

        public void Validate(ValidateContext context) => Validate(context, false);

        public void Validate(ValidateContext context, bool voidAcceptable)
        {
            _expression.Validate(!voidAcceptable ? new TypeRestriction(ObjectSchema.Type) : new TypeRestriction(ObjectSchema.Type, VoidSchema.Type), context);
            if (_expression.HasErrors)
                MarkHasErrors();
            else
                _foundParameterType = _expression.ObjectType;
        }

        public TypeSchema FoundParameterType => _foundParameterType;

        public void AppendToEnd(ValidateParameter item)
        {
            ValidateParameter validateParameter = this;
            while (validateParameter.Next != null)
                validateParameter = validateParameter.Next;
            validateParameter.Next = item;
        }
    }
}
