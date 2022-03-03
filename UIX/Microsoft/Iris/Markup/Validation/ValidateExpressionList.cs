// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;
using System.Collections;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionList : ValidateExpression
    {
        private ArrayList _expressionList;
        private int _foundTypeIndex;

        public ValidateExpressionList(SourceMarkupLoader owner, int line, int column)
          : base(owner, line, column, ExpressionType.List)
          => _expressionList = new ArrayList();

        public void AppendToEnd(ValidateExpression expression) => _expressionList.Add(expression);

        public ArrayList Expressions => _expressionList;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (Usage == ExpressionUsage.LValue)
                ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", "Expression List");
            if (_expressionList.Count > 0)
            {
                for (int index = 0; index < _expressionList.Count; ++index)
                {
                    ValidateExpression expression = (ValidateExpression)_expressionList[index];
                    if (index < _expressionList.Count - 1)
                    {
                        expression.Validate(TypeRestriction.None, context);
                    }
                    else
                    {
                        expression.Validate(typeRestriction, context);
                        if (expression.ObjectType != null)
                            DeclareEvaluationType(expression.ObjectType, typeRestriction);
                    }
                }
            }
            else
                DeclareEvaluationType(VoidSchema.Type, typeRestriction);
            if (ObjectType == null)
                MarkHasErrors();
            else
                _foundTypeIndex = Owner.TrackImportedType(ObjectType);
        }

        public int FoundTypeIndex => _foundTypeIndex;
    }
}
