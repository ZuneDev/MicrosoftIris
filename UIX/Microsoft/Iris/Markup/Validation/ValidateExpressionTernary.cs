// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionTernary
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionTernary : ValidateExpression
    {
        private ValidateExpression _condition;
        private ValidateExpression _trueClause;
        private ValidateExpression _falseClause;

        public ValidateExpressionTernary(
          SourceMarkupLoader owner,
          ValidateExpression condition,
          ValidateExpression trueClause,
          ValidateExpression falseClause,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Ternary)
        {
            _condition = condition;
            _trueClause = trueClause;
            _falseClause = falseClause;
        }

        public ValidateExpression Condition => _condition;

        public ValidateExpression TrueClause => _trueClause;

        public ValidateExpression FalseClause => _falseClause;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (Usage == ExpressionUsage.LValue)
                ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", "Ternary");
            _condition.Validate(new TypeRestriction(BooleanSchema.Type), context);
            if (_condition.HasErrors)
                MarkHasErrors();
            _trueClause.Validate(typeRestriction, context);
            if (_trueClause.HasErrors)
                MarkHasErrors();
            _falseClause.Validate(typeRestriction, context);
            if (_falseClause.HasErrors)
                MarkHasErrors();
            if (_trueClause.ObjectType == null || _falseClause.ObjectType == null)
                return;
            if (_trueClause.ObjectType.IsAssignableFrom(_falseClause.ObjectType))
                DeclareEvaluationType(_trueClause.ObjectType, typeRestriction);
            else if (_falseClause.ObjectType.IsAssignableFrom(_trueClause.ObjectType))
                DeclareEvaluationType(_falseClause.ObjectType, typeRestriction);
            else
                ReportError("Both expressions for the {0} operator must match: '{1}' and '{2}' are not compatible", "?:", _trueClause.ObjectType.Name, _falseClause.ObjectType.Name);
        }
    }
}
