// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionSymbol
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionSymbol : ValidateExpression
    {
        private string _symbol;
        private int _foundSymbolIndex = -1;
        private SymbolOrigin _foundSymbolOrigin;
        private bool _foundSymbolIsType;

        public ValidateExpressionSymbol(SourceMarkupLoader owner, string symbol, int line, int column)
          : base(owner, line, column, ExpressionType.Symbol)
          => _symbol = symbol;

        public string Symbol => _symbol;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context) => Validate(typeRestriction, context, false);

        public void Validate(
          TypeRestriction typeRestriction,
          ValidateContext context,
          bool allowTypeSymbols)
        {
            ExpressionRestriction expressionRestriction;
            TypeSchema evaluationType = context.ResolveSymbol(_symbol, out _foundSymbolOrigin, out expressionRestriction);
            if (evaluationType == null)
            {
                if (allowTypeSymbols)
                {
                    ValidateTypeIdentifier validateTypeIdentifier = new ValidateTypeIdentifier(Owner, null, _symbol, Line, Column);
                    validateTypeIdentifier.Validate();
                    if (validateTypeIdentifier.HasErrors)
                    {
                        MarkHasErrors();
                        return;
                    }
                    evaluationType = validateTypeIdentifier.FoundType;
                    _foundSymbolIndex = validateTypeIdentifier.FoundTypeIndex;
                    _foundSymbolIsType = true;
                }
                else
                {
                    ReportError("Unable to locate symbol \"{0}\" within Properties, Locals, Input, or Content", _symbol);
                    return;
                }
            }
            if (_foundSymbolOrigin != SymbolOrigin.None)
                _foundSymbolIndex = context.TrackSymbolUsage(_symbol, _foundSymbolOrigin);
            DeclareEvaluationType(evaluationType, typeRestriction);
            if (expressionRestriction == ExpressionRestriction.NoAccess)
            {
                ReportError("Expression access to '{0}' is not available", _symbol);
            }
            else
            {
                if (Usage == ExpressionUsage.LValue && expressionRestriction != ExpressionRestriction.None)
                    ReportError("Expression access to '{0}' only supports read operations", _symbol);
                if (expressionRestriction != ExpressionRestriction.None)
                    return;
                DeclareNotifies(context);
            }
        }

        public int FoundSymbolIndex => _foundSymbolIndex;

        public SymbolOrigin FoundSymbolOrigin => _foundSymbolOrigin;

        public bool FoundSymbolIsType => _foundSymbolIsType;
    }
}
