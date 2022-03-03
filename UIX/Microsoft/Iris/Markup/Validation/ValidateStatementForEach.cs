// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatementForEach
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateStatementForEach : ValidateStatementLoop
    {
        private ValidateStatementScopedLocal _scopedLocal;
        private ValidateExpression _expression;
        private ValidateStatementCompound _statementCompound;
        private Vector<int> _scopedLocalsToClear;
        private int _foundGetEnumeratorIndex = -1;
        private int _foundCurrentIndex = -1;
        private int _foundMoveNextIndex = -1;
        private static PropertySchema s_currentProperty;
        private static MethodSchema s_moveNextMethod;

        public ValidateStatementForEach(
          SourceMarkupLoader owner,
          ValidateStatementScopedLocal scopedLocal,
          ValidateExpression expression,
          ValidateStatementCompound statementCompound,
          int line,
          int column)
          : base(owner, line, column, StatementType.ForEach)
        {
            _scopedLocal = scopedLocal;
            _expression = expression;
            _statementCompound = statementCompound;
        }

        public static void InitializeStatics()
        {
            s_currentProperty = EnumeratorSchema.Type.FindProperty("Current");
            s_moveNextMethod = EnumeratorSchema.Type.FindMethod("MoveNext", TypeSchema.EmptyList);
        }

        public ValidateStatementScopedLocal ScopedLocal => _scopedLocal;

        public ValidateExpression Expression => _expression;

        public ValidateStatementCompound StatementCompound => _statementCompound;

        public override void Validate(ValidateCode container, ValidateContext context)
        {
            context.NotifyScopedLocalFrameEnter(this);
            try
            {
                _scopedLocal.Validate(container, context);
                if (_scopedLocal.HasErrors)
                    MarkHasErrors();
                _scopedLocal.HasInitialAssignment = true;
                _expression.Validate(new TypeRestriction(ListSchema.Type), context);
                if (_expression.HasErrors)
                {
                    MarkHasErrors();
                }
                else
                {
                    MethodSchema methodDeep = ListSchema.Type.FindMethodDeep("GetEnumerator", TypeSchema.EmptyList);
                    if (methodDeep == null)
                        ReportError("Type '{0}' is not enumerable", _expression.ObjectType.Name);
                    if (!EnumeratorSchema.Type.IsAssignableFrom(methodDeep.ReturnType))
                        ReportError("While searching for an enumerator, a 'GetEnumerator' method was found on '{0}', but, it is not of type 'Enumerator' (it is '{1}')", _expression.ObjectType.Name, methodDeep.ReturnType.Name);
                    _foundGetEnumeratorIndex = Owner.TrackImportedMethod(methodDeep);
                    _statementCompound.Validate(container, context);
                    if (_statementCompound.HasErrors)
                        MarkHasErrors();
                    _foundCurrentIndex = Owner.TrackImportedProperty(s_currentProperty);
                    _foundMoveNextIndex = Owner.TrackImportedMethod(s_moveNextMethod);
                }
            }
            finally
            {
                _scopedLocalsToClear = context.NotifyScopedLocalFrameExit();
            }
        }

        public int FoundGetEnumeratorIndex => _foundGetEnumeratorIndex;

        public int FoundCurrentIndex => _foundCurrentIndex;

        public int FoundMoveNextIndex => _foundMoveNextIndex;

        public Vector<int> ScopedLocalsToClear => _scopedLocalsToClear;
    }
}
