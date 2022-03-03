// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionThis
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionThis : ValidateExpression
    {
        public ValidateExpressionThis(SourceMarkupLoader owner, int line, int column)
          : base(owner, line, column, ExpressionType.This)
        {
        }

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (Usage == ExpressionUsage.LValue)
                ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", "this");
            DeclareEvaluationType(context.Owner.TypeExport, TypeRestriction.None);
        }
    }
}
