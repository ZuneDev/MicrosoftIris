// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionCast
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionCast : ValidateExpression
    {
        private ValidateTypeIdentifier _typeCast;
        private ValidateExpression _castee;
        private CastMethod _foundCastMethod;
        private TypeSchema _foundCasteeType;
        private int _foundCasteeTypeIndex = -1;
        private TypeSchema _foundTypeCast;
        private int _foundTypeCastIndex = -1;

        public ValidateExpressionCast(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeCast,
          ValidateExpression castee,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Cast)
        {
            _typeCast = typeCast;
            _castee = castee;
        }

        public ValidateExpressionCast(
          SourceMarkupLoader owner,
          ValidateExpression typeCastExpression,
          ValidateExpression castee,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Cast)
        {
            if (typeCastExpression.ExpressionType != ExpressionType.Symbol)
            {
                ReportError("Type cast was expecting symbol, found '{0}'", typeCastExpression.ExpressionType.ToString());
            }
            else
            {
                ValidateExpressionSymbol expressionSymbol = (ValidateExpressionSymbol)typeCastExpression;
                _typeCast = new ValidateTypeIdentifier(owner, null, expressionSymbol.Symbol, expressionSymbol.Line, expressionSymbol.Column);
            }
            _castee = castee;
        }

        public ValidateTypeIdentifier TypeCast => _typeCast;

        public ValidateExpression Castee => _castee;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (_typeCast == null)
                return;
            _typeCast.Validate();
            if (_typeCast.HasErrors)
            {
                MarkHasErrors();
            }
            else
            {
                _foundTypeCast = _typeCast.FoundType;
                _foundTypeCastIndex = _typeCast.FoundTypeIndex;
                DeclareEvaluationType(_foundTypeCast, typeRestriction);
                if (Usage == ExpressionUsage.LValue)
                    ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", _foundTypeCast.Name);
                _castee.Validate(TypeRestriction.NotVoid, context);
                if (_castee.HasErrors)
                {
                    MarkHasErrors();
                }
                else
                {
                    _foundCasteeType = _castee.ObjectType;
                    if (_foundTypeCast.Contractual || _foundCasteeType.Contractual)
                    {
                        _foundCastMethod = CastMethod.Cast;
                    }
                    else
                    {
                        if (_foundCasteeType == _foundTypeCast)
                            return;
                        bool flag1 = _foundCasteeType.IsAssignableFrom(_foundTypeCast);
                        bool flag2 = _foundTypeCast.IsAssignableFrom(_foundCasteeType);
                        if (!flag1 && !flag2)
                        {
                            if (_foundTypeCast.SupportsTypeConversion(_foundCasteeType))
                            {
                                _foundCastMethod = CastMethod.Conversion;
                                _foundCasteeTypeIndex = Owner.TrackImportedType(_foundCasteeType);
                            }
                            else
                                ReportError("Cannot cast '{0}' to '{1}'", _foundCasteeType.Name, _foundTypeCast.Name);
                        }
                        else
                        {
                            if (!flag1)
                                return;
                            _foundCastMethod = CastMethod.Cast;
                        }
                    }
                }
            }
        }

        public CastMethod FoundCastMethod => _foundCastMethod;

        public TypeSchema FoundCasteeType => _foundCasteeType;

        public int FoundCasteeTypeIndex => _foundCasteeTypeIndex;

        public TypeSchema FoundTypeCast => _foundTypeCast;

        public int FoundTypeCastIndex => _foundTypeCastIndex;
    }
}
