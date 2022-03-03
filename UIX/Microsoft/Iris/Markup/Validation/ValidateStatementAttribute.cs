// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatementAttribute
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateStatementAttribute : ValidateStatement
    {
        private const string DeclareTriggerAttributeName = "DeclareTrigger";
        private const string InitialEvaluateAttributeName = "InitialEvaluate";
        private const string FinalEvaluateAttributeName = "FinalEvaluate";
        private string _attributeName;
        private ValidateParameter _parameterList;

        public ValidateStatementAttribute(
          SourceMarkupLoader owner,
          string attributeName,
          ValidateParameter parameterList,
          int line,
          int column)
          : base(owner, line, column, StatementType.Attribute)
        {
            if (parameterList == ValidateParameter.EmptyList)
                parameterList = null;
            _attributeName = attributeName;
            _parameterList = parameterList;
        }

        public string AttributeName => _attributeName;

        public ValidateParameter ParameterList => _parameterList;

        public override void Validate(ValidateCode container, ValidateContext context)
        {
            if (_attributeName == "DeclareTrigger")
                ValidateDeclareTrigger(container, context);
            else if (_attributeName == "InitialEvaluate")
                ValidateInitialOrFinalEvaluate(container, context, true);
            else if (_attributeName == "FinalEvaluate")
                ValidateInitialOrFinalEvaluate(container, context, false);
            else
                ReportError("Script attribute '{0}' is unknown", _attributeName);
        }

        private void ValidateDeclareTrigger(ValidateCode container, ValidateContext context)
        {
            ValidateParameter parameterList = _parameterList;
            if (_parameterList == null || parameterList.Next != null)
            {
                ReportError("Script attribute '{0}' invalid number of parameters (expecting: {1})", _attributeName, "1");
            }
            else
            {
                ValidateExpressionDeclareTrigger.StartNotifierTracking(context, parameterList.Expression);
                parameterList.Expression.MakeDeclareTriggerUsage();
                parameterList.Validate(context, true);
                ValidateExpressionDeclareTrigger.StopNotifierTracking(this, context, parameterList.Expression);
                if (parameterList.HasErrors)
                    MarkHasErrors();
                else if (context.IsTrackingDeclaredTriggers)
                {
                    ValidateExpression expression = _parameterList.Expression;
                    context.TrackDeclaredTrigger(expression);
                    container.MarkDeclaredTriggerStatements();
                }
                else
                    ReportError("Expressions can only be used as triggers if they exist within Script blocks");
            }
        }

        private void ValidateInitialOrFinalEvaluate(
          ValidateCode container,
          ValidateContext context,
          bool isInitialEvaluate)
        {
            if (_parameterList == null || _parameterList.Next != null)
            {
                ReportError("Script attribute '{0}' invalid number of parameters (expecting: {1})", _attributeName, "1");
            }
            else
            {
                _parameterList.Validate(context, false);
                if (!(_parameterList.Expression is ValidateExpressionConstant expression) || expression.ConstantType != ConstantType.Boolean)
                {
                    ReportError("Script attribute parameter must be Boolean 'true' or 'false'");
                }
                else
                {
                    bool foundConstant = (bool)expression.FoundConstant;
                    if (isInitialEvaluate)
                        container.MarkInitialEvaluate(foundConstant);
                    else
                        container.MarkFinalEvaluate(foundConstant);
                }
            }
        }
    }
}
