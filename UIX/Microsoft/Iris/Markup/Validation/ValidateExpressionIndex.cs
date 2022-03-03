// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionIndex
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionIndex : ValidateExpression
    {
        private ValidateExpression _indexee;
        private ValidateParameter _index;
        private ValidateExpressionCall _call;
        private ValidateExpression _assignmentValue;

        public ValidateExpressionIndex(
          SourceMarkupLoader owner,
          ValidateExpression indexee,
          ValidateParameter index,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Index)
        {
            _indexee = indexee;
            _index = index;
        }

        public ValidateExpression CallExpression => _call;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (Usage == ExpressionUsage.RValue || Usage == ExpressionUsage.DeclareTrigger)
            {
                _call = new ValidateExpressionCall(Owner, _indexee, "get_Item", _index, Line, Column);
                _call.Validate(TypeRestriction.NotVoid, context);
                if (_call.HasErrors)
                    MarkHasErrors();
                else
                    DeclareEvaluationType(_call.ObjectType, typeRestriction);
            }
            else
            {
                _index.AppendToEnd(new ValidateParameter(Owner, _assignmentValue, _assignmentValue.Line, _assignmentValue.Column));
                _call = new ValidateExpressionCall(Owner, _indexee, "set_Item", _index, Line, Column);
                _call.SetAsIndexAssignment();
                _call.Validate(new TypeRestriction(VoidSchema.Type), context);
                if (_call.HasErrors)
                    MarkHasErrors();
                else
                    DeclareEvaluationType(_assignmentValue.ObjectType, typeRestriction);
            }
        }

        public void SetAssignmentValue(ValidateExpression assignmentValue) => _assignmentValue = assignmentValue;
    }
}
