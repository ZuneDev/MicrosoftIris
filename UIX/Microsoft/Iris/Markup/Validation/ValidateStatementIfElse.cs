// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatementIfElse
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateStatementIfElse : ValidateStatement
    {
        private ValidateExpression _condition;
        private ValidateStatementCompound _statementCompoundTrue;
        private ValidateStatementCompound _statementCompoundFalse;

        public ValidateStatementIfElse(
          SourceMarkupLoader owner,
          ValidateExpression condition,
          ValidateStatementCompound statementCompoundTrue,
          ValidateStatementCompound statementCompoundFalse,
          int line,
          int column)
          : base(owner, line, column, StatementType.IfElse)
        {
            _condition = condition;
            _statementCompoundTrue = statementCompoundTrue;
            _statementCompoundFalse = statementCompoundFalse;
        }

        public ValidateExpression Condition => _condition;

        public ValidateStatement StatementCompoundTrue => _statementCompoundTrue;

        public ValidateStatement StatementCompoundFalse => _statementCompoundFalse;

        public override void Validate(ValidateCode container, ValidateContext context)
        {
            _condition.Validate(new TypeRestriction(BooleanSchema.Type), context);
            if (_condition.HasErrors)
                MarkHasErrors();
            _statementCompoundTrue.Validate(container, context);
            if (_statementCompoundTrue.HasErrors)
                MarkHasErrors();
            _statementCompoundFalse.Validate(container, context);
            if (!_statementCompoundFalse.HasErrors)
                return;
            MarkHasErrors();
        }
    }
}
