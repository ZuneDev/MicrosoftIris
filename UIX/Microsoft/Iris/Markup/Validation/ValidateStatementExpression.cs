// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatementExpression
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateStatementExpression : ValidateStatement
    {
        private ValidateExpression _expression;

        public ValidateStatementExpression(
          SourceMarkupLoader owner,
          ValidateExpression expression,
          int line,
          int column)
          : base(owner, line, column, StatementType.Expression)
        {
            _expression = expression;
        }

        public ValidateExpression Expression => _expression;

        public override void Validate(ValidateCode container, ValidateContext context)
        {
            _expression.Validate(TypeRestriction.None, context);
            if (!_expression.HasErrors)
                return;
            MarkHasErrors();
        }
    }
}
