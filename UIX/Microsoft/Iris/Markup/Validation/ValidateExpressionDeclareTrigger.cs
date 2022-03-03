// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionDeclareTrigger
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionDeclareTrigger : ValidateExpression
    {
        private ValidateExpression _expression;

        public ValidateExpressionDeclareTrigger(
          SourceMarkupLoader owner,
          ValidateExpression expression,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.DeclareTrigger)
        {
            _expression = expression;
        }

        public ValidateExpression Expression => _expression;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (Usage == ExpressionUsage.LValue)
                ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", "Trigger Declaration");
            if (context.IsNotifierTracking)
            {
                ReportError("Declared triggers may not exist within other declared triggers");
            }
            else
            {
                StartNotifierTracking(context, _expression);
                try
                {
                    _expression.Validate(TypeRestriction.None, context);
                }
                finally
                {
                    StopNotifierTracking(this, context, _expression);
                }
                if (_expression.HasErrors)
                    MarkHasErrors();
                if (HasErrors)
                    return;
                DeclareEvaluationType(_expression.ObjectType, typeRestriction);
                if (context.IsTrackingDeclaredTriggers)
                    context.TrackDeclaredTrigger(_expression);
                else
                    ReportError("Expressions can only be used as triggers if they exist within Script blocks");
            }
        }

        public static void StartNotifierTracking(ValidateContext context, ValidateExpression expression) => context.StartNotifierTracking(expression);

        public static void StopNotifierTracking(
          Microsoft.Iris.Markup.Validation.Validate validate,
          ValidateContext context,
          ValidateExpression expression)
        {
            if (context.StopNotifierTracking())
            {
                if (expression.ExpressionType != ExpressionType.Symbol)
                    return;
                ValidateExpressionSymbol expressionSymbol = (ValidateExpressionSymbol)expression;
                if (expressionSymbol.FoundSymbolOrigin != SymbolOrigin.ScopedLocal)
                    return;
                validate.ReportError("Expression cannot be declared as a trigger since '{0}' is a temporary (scoped) variable", expressionSymbol.Symbol);
            }
            else
                validate.ReportError("Expression cannot be declared as a trigger since it does not have an outermost property or event that fires notifications");
        }
    }
}
