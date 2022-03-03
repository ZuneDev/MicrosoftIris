// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatement
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal abstract class ValidateStatement : Microsoft.Iris.Markup.Validation.Validate
    {
        private StatementType _statementType;
        private ValidateStatement _next;

        public ValidateStatement(
          SourceMarkupLoader owner,
          int line,
          int column,
          StatementType statementType)
          : base(owner, line, column)
        {
            _statementType = statementType;
        }

        public StatementType StatementType => _statementType;

        public ValidateStatement Next
        {
            get => _next;
            set => _next = value;
        }

        public void AppendToEnd(ValidateStatement item)
        {
            ValidateStatement validateStatement = this;
            while (validateStatement.Next != null)
                validateStatement = validateStatement.Next;
            validateStatement.Next = item;
        }

        public abstract void Validate(ValidateCode container, ValidateContext context);
    }
}
