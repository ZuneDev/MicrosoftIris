// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionNullCoalescing
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionNullCoalescing : ValidateExpression
    {
        private ValidateExpression _condition;
        private ValidateExpression _nullClause;

        public ValidateExpressionNullCoalescing(
          SourceMarkupLoader owner,
          ValidateExpression condition,
          ValidateExpression nullClause,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.NullCoalescing)
        {
            _condition = condition;
            _nullClause = nullClause;
        }

        public ValidateExpression Condition => _condition;

        public ValidateExpression NullClause => _nullClause;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (Usage == ExpressionUsage.LValue)
                ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", "Null coalescing");
            _condition.Validate(typeRestriction, context);
            if (_condition.HasErrors)
                MarkHasErrors();
            if (_condition.ObjectType != null)
                DeclareEvaluationType(_condition.ObjectType, typeRestriction);
            _nullClause.Validate(typeRestriction, context);
            if (_condition.HasErrors)
                MarkHasErrors();
            if (_condition.ObjectType == null || _nullClause.ObjectType == null)
                return;
            if (_condition.ObjectType == NullSchema.Type)
            {
                ReportError("Null constant may not be used as the condition for ?? operator");
            }
            else
            {
                if (!_condition.ObjectType.IsNullAssignable)
                    ReportError("The {0} operator must be used with a reference type ('{1}' is not null assignable)", "??", _condition.ObjectType.Name);
                if (_condition.ObjectType.IsAssignableFrom(_nullClause.ObjectType))
                    return;
                ReportError("Both expressions for the {0} operator must match: '{1}' and '{2}' are not compatible", "??", _condition.ObjectType.Name, _nullClause.ObjectType.Name);
            }
        }
    }
}
