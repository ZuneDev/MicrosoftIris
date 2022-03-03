// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatementBreak
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateStatementBreak : ValidateStatement
    {
        private ValidateStatementLoop _loopStatement;
        private Vector<int> _scopedLocalsToClear;
        private uint _jumpFixupOffset;
        private bool _isContinue;

        public ValidateStatementBreak(SourceMarkupLoader owner, bool isContinue, int line, int column)
          : base(owner, line, column, StatementType.Break)
          => _isContinue = isContinue;

        protected override void OnDispose()
        {
            _loopStatement = null;
            base.OnDispose();
        }

        public override void Validate(ValidateCode container, ValidateContext context)
        {
            _loopStatement = context.EnclosingLoop;
            if (_loopStatement == null)
            {
                ReportError("No enclosing loop out of which to break or continue");
            }
            else
            {
                _loopStatement.TrackBreakStatement(this);
                _scopedLocalsToClear = context.GetLoopUnwindList(Owner);
            }
        }

        public bool IsContinue => _isContinue;

        public Vector<int> ScopedLocalsToClear => _scopedLocalsToClear;

        public uint JumpFixupOffset => _jumpFixupOffset;

        public void TrackJumpFixupOffset(uint jumpFixupOffset) => _jumpFixupOffset = jumpFixupOffset;
    }
}
