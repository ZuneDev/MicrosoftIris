// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionIsCheck
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionIsCheck : ValidateExpression
    {
        private ValidateExpression _expression;
        private ValidateTypeIdentifier _typeIdentifier;

        public ValidateExpressionIsCheck(
          SourceMarkupLoader owner,
          ValidateExpression expression,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.IsCheck)
        {
            _expression = expression;
            _typeIdentifier = typeIdentifier;
        }

        public ValidateExpression Expression => _expression;

        public ValidateTypeIdentifier TypeIdentifier => _typeIdentifier;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (Usage == ExpressionUsage.LValue)
                ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", "Is");
            _expression.Validate(TypeRestriction.NotVoid, context);
            if (_expression.HasErrors)
                MarkHasErrors();
            _typeIdentifier.Validate();
            if (_typeIdentifier.HasErrors)
                MarkHasErrors();
            DeclareEvaluationType(BooleanSchema.Type, typeRestriction);
        }
    }
}
