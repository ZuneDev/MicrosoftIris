// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpression
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal abstract class ValidateExpression : ValidateObject
    {
        private TypeSchema _evaluationType;
        private ExpressionType _expressionType;
        private ExpressionUsage _usage;
        private int _notifyIndex = -1;
        private bool _isNotifierRoot;
        private uint _encodeStartOffset;

        public ValidateExpression(
          SourceMarkupLoader owner,
          int line,
          int column,
          ExpressionType expressionType)
          : base(owner, line, column, ObjectSourceType.Expression)
        {
            _expressionType = expressionType;
            _usage = ExpressionUsage.RValue;
        }

        public override TypeSchema ObjectType => _evaluationType;

        public ExpressionType ExpressionType => _expressionType;

        public void MakeLValueUsage() => _usage = ExpressionUsage.LValue;

        public void MakeDeclareTriggerUsage() => _usage = ExpressionUsage.DeclareTrigger;

        public uint EncodeStartOffset => _encodeStartOffset;

        protected void DeclareEvaluationType(TypeSchema evaluationType, TypeRestriction typeRestriction)
        {
            if (!typeRestriction.Check(this, evaluationType))
                return;
            _evaluationType = evaluationType;
        }

        public ExpressionUsage Usage => _usage;

        protected void DeclareNotifies(ValidateContext context) => _notifyIndex = context.TrackDeclareNotifies(this);

        public int NotifyIndex => _notifyIndex;

        public bool IsNotifierRoot => _isNotifierRoot;

        public void MarkNotifierRoot() => _isNotifierRoot = true;

        public void TrackEncodingOffset(uint startOffset) => _encodeStartOffset = startOffset;
    }
}
