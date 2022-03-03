// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateCode
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateCode : ValidateObject
    {
        private ValidateStatementCompound _statementCompound;
        private Vector<ValidateStatementReturn> _returnStatements = new Vector<ValidateStatementReturn>();
        private Vector<ValidateExpression> _declaredTriggerExpressions;
        private TypeSchema _returnType;
        private uint _encodeStartOffset;
        private bool _embedded;
        private bool _allowTriggers = true;
        private bool _hasDeclaredTriggerStatements;
        private bool _hasInitialEvaluateStatement;
        private bool _initialEvaluate;
        private bool _finalEvaluate;

        public ValidateCode(
          SourceMarkupLoader owner,
          ValidateStatementCompound statementCompound,
          int line,
          int offset)
          : base(owner, line, offset, ObjectSourceType.Code)
        {
            _statementCompound = statementCompound;
            _embedded = true;
        }

        public ValidateStatementCompound StatementCompound => _statementCompound;

        public bool Embedded => _embedded;

        public override TypeSchema ObjectType => _returnType;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (_allowTriggers)
                context.StartDeclaredTriggerTracking();
            _statementCompound.Validate(this, context);
            if (_allowTriggers)
                _declaredTriggerExpressions = context.StopDeclaredTriggerTracking();
            if (_statementCompound.HasErrors)
                MarkHasErrors();
            if (_returnStatements.Count > 0)
            {
                for (int index = 0; index < _returnStatements.Count; ++index)
                {
                    ValidateStatementReturn returnStatement = _returnStatements[index];
                    if (returnStatement.HasErrors)
                        return;
                    TypeSchema type = VoidSchema.Type;
                    ValidateExpression expression = returnStatement.Expression;
                    if (expression != null)
                        type = expression.ObjectType;
                    ValidateReturnType(type);
                    if (HasErrors)
                        return;
                }
            }
            else
                _returnType = VoidSchema.Type;
            string errorMessage = "'{0}' cannot be used in this context (expecting types compatible with '{1}')";
            if (typeRestriction.Primary == VoidSchema.Type && typeRestriction.Secondary == null)
                errorMessage = "Return values are not supported for this code block (currently returning '{0}')";
            else if (_returnStatements.Count == 0)
                errorMessage = "Code block must have at least one return statement of type '{0}'";
            if (!typeRestriction.Check(this, errorMessage, _returnType))
                return;
            ValidateStatementReturn validateStatementReturn = null;
            ValidateStatement validateStatement = _statementCompound.StatementList;
            if (validateStatement != null)
            {
                while (validateStatement.Next != null)
                    validateStatement = validateStatement.Next;
                if (validateStatement.StatementType == StatementType.Return)
                {
                    validateStatementReturn = (ValidateStatementReturn)validateStatement;
                    validateStatementReturn.MarkAsTrailingReturn();
                }
            }
            if (_returnType == VoidSchema.Type || validateStatementReturn != null)
                return;
            ReportError("Code block must end with a return of type '{0}'", _returnType.Name);
        }

        private void ValidateReturnType(TypeSchema type)
        {
            if (_returnType == null)
            {
                _returnType = type;
            }
            else
            {
                if (_returnType == type)
                    return;
                if (type.IsAssignableFrom(_returnType))
                {
                    _returnType = type;
                }
                else
                {
                    if (_returnType.IsAssignableFrom(type))
                        return;
                    ReportError("Return type cannot be determined due to inconsistant return type statements (mismatched types are: '{0}' and '{1}')", _returnType.Name, type.Name);
                }
            }
        }

        public void MarkAsNotEmbedded() => _embedded = false;

        public void DisallowTriggers() => _allowTriggers = false;

        public uint EncodeStartOffset => _encodeStartOffset;

        public Vector<ValidateStatementReturn> ReturnStatements => _returnStatements;

        public Vector<ValidateExpression> DeclaredTriggerExpressions => _declaredTriggerExpressions;

        public bool InitialEvaluate
        {
            get
            {
                if (_hasInitialEvaluateStatement)
                    return _initialEvaluate;
                return !_hasDeclaredTriggerStatements && !_finalEvaluate && !_embedded;
            }
        }

        public bool FinalEvaluate => _finalEvaluate;

        public void TrackReturnStatement(ValidateStatementReturn returnStatement) => _returnStatements.Add(returnStatement);

        public void TrackEncodingOffset(uint startOffset) => _encodeStartOffset = startOffset;

        public void MarkInitialEvaluate(bool initialEvaluate)
        {
            _hasInitialEvaluateStatement = true;
            _initialEvaluate = initialEvaluate;
        }

        public void MarkFinalEvaluate(bool finalEvaluate) => _finalEvaluate = finalEvaluate;

        public void MarkDeclaredTriggerStatements() => _hasDeclaredTriggerStatements = true;

        public ValidateCode Next
        {
            get => (ValidateCode)base.Next;
            set => base.Next = value;
        }
    }
}
