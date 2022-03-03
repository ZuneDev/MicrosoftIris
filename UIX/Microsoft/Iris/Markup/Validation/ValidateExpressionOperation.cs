// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionOperation
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionOperation : ValidateExpression
    {
        private ValidateExpression _leftSide;
        private ValidateExpression _rightSide;
        private OperationType _op;
        private TypeSchema _foundOperationTargetType;
        private int _foundOperationTargetTypeIndex = -1;

        public ValidateExpressionOperation(
          SourceMarkupLoader owner,
          ValidateExpression leftSide,
          OperationType op,
          ValidateExpression rightSide,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Operation)
        {
            _leftSide = leftSide;
            _rightSide = rightSide;
            _op = op;
        }

        public ValidateExpression LeftSide => _leftSide;

        public ValidateExpression RightSide => _rightSide;

        public OperationType Op => _op;

        private static string GetOperationToken(OperationType op)
        {
            string str = null;
            switch (op)
            {
                case OperationType.MathAdd:
                    str = "+";
                    break;
                case OperationType.MathSubtract:
                    str = "-";
                    break;
                case OperationType.MathMultiply:
                    str = "*";
                    break;
                case OperationType.MathDivide:
                    str = "\\";
                    break;
                case OperationType.MathModulus:
                    str = "%";
                    break;
                case OperationType.LogicalAnd:
                    str = "&&";
                    break;
                case OperationType.LogicalOr:
                    str = "||";
                    break;
                case OperationType.RelationalEquals:
                    str = "==";
                    break;
                case OperationType.RelationalNotEquals:
                    str = "!=";
                    break;
                case OperationType.RelationalLessThan:
                    str = "<";
                    break;
                case OperationType.RelationalGreaterThan:
                    str = ">";
                    break;
                case OperationType.RelationalLessThanEquals:
                    str = "<=";
                    break;
                case OperationType.RelationalGreaterThanEquals:
                    str = ">=";
                    break;
                case OperationType.RelationalIs:
                    str = "is";
                    break;
                case OperationType.LogicalNot:
                    str = "!";
                    break;
            }
            return str;
        }

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (_op == OperationType.PostIncrement || _op == OperationType.PostDecrement)
                ReportError("Post increment/decrement operators are not currently supported");
            if (Usage == ExpressionUsage.LValue)
                ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", "Operation");
            _leftSide.Validate(TypeRestriction.NotVoid, context);
            if (_leftSide.HasErrors)
                MarkHasErrors();
            if (_rightSide != null)
            {
                _rightSide.Validate(TypeRestriction.NotVoid, context);
                if (_rightSide.HasErrors)
                    MarkHasErrors();
            }
            if (HasErrors)
                return;
            if (_rightSide != null && !_leftSide.ObjectType.IsAssignableFrom(_rightSide.ObjectType))
            {
                if (_leftSide.ObjectType == NullSchema.Type && _rightSide.ObjectType.IsNullAssignable || _rightSide.ObjectType == NullSchema.Type && _leftSide.ObjectType.IsNullAssignable)
                {
                    _foundOperationTargetType = NullSchema.Type;
                }
                else
                {
                    ReportError("Operator '{0}' cannot be applied to operands of dissimilar types '{1}' and '{2}'", GetOperationToken(_op), _leftSide.ObjectType.Name, _rightSide.ObjectType.Name);
                    return;
                }
            }
            else
                _foundOperationTargetType = _leftSide.ObjectType;
            if (!_foundOperationTargetType.SupportsOperationDeep(_op))
            {
                ReportError("Operator '{0}' cannot be applied to operand of type '{1}'", GetOperationToken(_op), _foundOperationTargetType.Name);
            }
            else
            {
                _foundOperationTargetTypeIndex = Owner.TrackImportedType(_foundOperationTargetType);
                switch (_op)
                {
                    case OperationType.MathAdd:
                    case OperationType.MathSubtract:
                    case OperationType.MathMultiply:
                    case OperationType.MathDivide:
                    case OperationType.MathModulus:
                    case OperationType.MathNegate:
                    case OperationType.PostIncrement:
                    case OperationType.PostDecrement:
                        DeclareEvaluationType(_foundOperationTargetType, typeRestriction);
                        break;
                    case OperationType.LogicalAnd:
                    case OperationType.LogicalOr:
                    case OperationType.RelationalEquals:
                    case OperationType.RelationalNotEquals:
                    case OperationType.RelationalLessThan:
                    case OperationType.RelationalGreaterThan:
                    case OperationType.RelationalLessThanEquals:
                    case OperationType.RelationalGreaterThanEquals:
                    case OperationType.RelationalIs:
                    case OperationType.LogicalNot:
                        DeclareEvaluationType(BooleanSchema.Type, typeRestriction);
                        break;
                }
            }
        }

        public TypeSchema FoundOperationTargetType => _foundOperationTargetType;

        public int FoundOperationTargetTypeIndex => _foundOperationTargetTypeIndex;
    }
}
