// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatementCompound
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateStatementCompound : ValidateStatement
    {
        private ValidateStatement _statementList;
        private Vector<int> _scopedLocalsToClear;

        public ValidateStatementCompound(
          SourceMarkupLoader owner,
          ValidateStatement statementList,
          int line,
          int column)
          : base(owner, line, column, StatementType.Compound)
        {
            _statementList = statementList;
        }

        public ValidateStatement StatementList => _statementList;

        public override void Validate(ValidateCode container, ValidateContext context)
        {
            context.NotifyScopedLocalFrameEnter();
            try
            {
                for (ValidateStatement validateStatement = _statementList; validateStatement != null; validateStatement = validateStatement.Next)
                {
                    validateStatement.Validate(container, context);
                    if (validateStatement.HasErrors)
                        MarkHasErrors();
                }
            }
            finally
            {
                _scopedLocalsToClear = context.NotifyScopedLocalFrameExit();
            }
        }

        public Vector<int> ScopedLocalsToClear => _scopedLocalsToClear;

        public static ValidateStatementCompound Encapsulate(
          ValidateStatement statement)
        {
            return statement.StatementType != StatementType.Compound ? new ValidateStatementCompound(statement.Owner, statement, statement.Line, statement.Column) : (ValidateStatementCompound)statement;
        }
    }
}
