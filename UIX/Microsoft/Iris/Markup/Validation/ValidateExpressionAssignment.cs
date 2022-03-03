// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionAssignment
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionAssignment : ValidateExpression
    {
        private ValidateExpression _lvalue;
        private ValidateExpression _rvalue;
        private bool _isIndexAssignment;

        public ValidateExpressionAssignment(
          SourceMarkupLoader owner,
          ValidateExpression lvalue,
          ValidateExpression rvalue,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Assignment)
        {
            _lvalue = lvalue;
            _rvalue = rvalue;
        }

        public ValidateExpression LValue => _lvalue;

        public ValidateExpression RValue => _rvalue;

        public bool IsIndexAssignment => _isIndexAssignment;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (_lvalue.ExpressionType == ExpressionType.Index)
            {
                _isIndexAssignment = true;
                ((ValidateExpressionIndex)_lvalue).SetAssignmentValue(_rvalue);
            }
            _lvalue.MakeLValueUsage();
            _lvalue.Validate(TypeRestriction.NotVoid, context);
            if (_lvalue.HasErrors)
                MarkHasErrors();
            if (_lvalue.ObjectType != null)
                DeclareEvaluationType(_lvalue.ObjectType, typeRestriction);
            if (!_rvalue.HasErrors && _rvalue.ObjectType == null)
                _rvalue.Validate(TypeRestriction.NotVoid, context);
            if (_rvalue.HasErrors)
                MarkHasErrors();
            if (HasErrors || _lvalue.ObjectType.IsAssignableFrom(_rvalue.ObjectType))
                return;
            ReportError("Invalid assignment: Type '{0}' cannot be assigned to type '{1}'", _rvalue.ObjectType.Name, _lvalue.ObjectType.Name);
        }
    }
}
