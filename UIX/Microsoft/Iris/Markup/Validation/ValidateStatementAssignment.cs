// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatementAssignment
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateStatementAssignment : ValidateStatement
    {
        private ValidateStatementScopedLocal _declaredScopedLocal;
        private ValidateExpression _lvalue;
        private ValidateExpression _rvalue;

        public ValidateStatementAssignment(
          SourceMarkupLoader owner,
          ValidateStatementScopedLocal declaredScopedLocal,
          ValidateExpression lvalue,
          ValidateExpression rvalue,
          int line,
          int column)
          : base(owner, line, column, StatementType.Assignment)
        {
            _declaredScopedLocal = declaredScopedLocal;
            _lvalue = lvalue;
            _rvalue = rvalue;
        }

        public ValidateStatementScopedLocal DeclaredScopedLocal => _declaredScopedLocal;

        public ValidateExpression LValue => _lvalue;

        public ValidateExpression RValue => _rvalue;

        public override void Validate(ValidateCode container, ValidateContext context)
        {
            if (_declaredScopedLocal != null)
            {
                _declaredScopedLocal.Validate(container, context);
                if (_declaredScopedLocal.HasErrors)
                {
                    MarkHasErrors();
                    return;
                }
                _declaredScopedLocal.HasInitialAssignment = true;
            }
            _lvalue.MakeLValueUsage();
            _lvalue.Validate(TypeRestriction.NotVoid, context);
            if (_lvalue.HasErrors)
                MarkHasErrors();
            _rvalue.Validate(TypeRestriction.NotVoid, context);
            if (_rvalue.HasErrors)
                MarkHasErrors();
            if (HasErrors || _lvalue.ObjectType.IsAssignableFrom(_rvalue.ObjectType))
                return;
            ReportError("Invalid assignment: Type '{0}' cannot be assigned to type '{1}'", _rvalue.ObjectType.Name, _lvalue.ObjectType.Name);
        }
    }
}
