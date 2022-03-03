// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionConstant
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionConstant : ValidateExpression
    {
        private string _constantInline;
        private ConstantType _constantType;
        private object _foundConstant;
        private int _foundTypeIndex;

        public ValidateExpressionConstant(
          SourceMarkupLoader owner,
          string constantInline,
          ConstantType constantType,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Constant)
        {
            _constantInline = constantInline;
            _constantType = constantType;
        }

        public ValidateExpressionConstant(
          SourceMarkupLoader owner,
          bool constantValue,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Constant)
        {
            _constantInline = null;
            _constantType = ConstantType.Boolean;
            _foundConstant = BooleanBoxes.Box(constantValue);
        }

        public ConstantType ConstantType => _constantType;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (Usage == ExpressionUsage.LValue)
                ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", _constantInline);
            TypeSchema evaluationType;
            switch (_constantType)
            {
                case ConstantType.String:
                    int errorIndex;
                    string invalidSequence;
                    _foundConstant = StringUtility.Unescape(_constantInline, out errorIndex, out invalidSequence);
                    if (_foundConstant == null)
                        ReportErrorWithAdjustedPosition(string.Format("Invalid escape sequence '{0}' in string literal", invalidSequence), 0, errorIndex + 1);
                    evaluationType = StringSchema.Type;
                    break;
                case ConstantType.StringLiteral:
                    _foundConstant = _constantInline;
                    _constantType = ConstantType.String;
                    evaluationType = StringSchema.Type;
                    break;
                case ConstantType.Integer:
                    Result result1 = Int32Schema.Type.TypeConverter(_constantInline, StringSchema.Type, out _foundConstant);
                    if (result1.Failed)
                        ReportError(result1.Error);
                    evaluationType = Int32Schema.Type;
                    break;
                case ConstantType.LongInteger:
                    Result result2 = Int64Schema.Type.TypeConverter(_constantInline, StringSchema.Type, out _foundConstant);
                    if (result2.Failed)
                        ReportError(result2.Error);
                    evaluationType = Int64Schema.Type;
                    break;
                case ConstantType.Float:
                    Result result3 = SingleSchema.Type.TypeConverter(_constantInline, StringSchema.Type, out _foundConstant);
                    if (result3.Failed)
                        ReportError(result3.Error);
                    evaluationType = SingleSchema.Type;
                    break;
                case ConstantType.Boolean:
                    evaluationType = BooleanSchema.Type;
                    break;
                case ConstantType.Null:
                    _foundConstant = null;
                    evaluationType = NullSchema.Type;
                    break;
                default:
                    evaluationType = null;
                    break;
            }
            DeclareEvaluationType(evaluationType, typeRestriction);
            if (ObjectType == null)
                MarkHasErrors();
            else
                _foundTypeIndex = Owner.TrackImportedType(ObjectType);
        }

        public object FoundConstant => _foundConstant;

        public int FoundTypeIndex => _foundTypeIndex;
    }
}
