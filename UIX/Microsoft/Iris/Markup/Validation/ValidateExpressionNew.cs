// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionNew
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionNew : ValidateExpression
    {
        private ValidateTypeIdentifier _constructType;
        private ValidateParameter _parameterList;
        private bool _isParameterizedConstruction;
        private TypeSchema _foundConstructType;
        private int _foundConstructTypeIndex = -1;
        private ConstructorSchema _foundParameterizedConstructor;
        private int _foundParameterizedConstructorIndex = -1;

        public ValidateExpressionNew(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier constructType,
          ValidateParameter parameterList,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.New)
        {
            if (parameterList == ValidateParameter.EmptyList)
                parameterList = null;
            _constructType = constructType;
            _parameterList = parameterList;
        }

        public ValidateTypeIdentifier ConstructType => _constructType;

        public ValidateParameter ParameterList => _parameterList;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            _constructType.Validate();
            if (_constructType.HasErrors)
            {
                MarkHasErrors();
            }
            else
            {
                _foundConstructType = _constructType.FoundType;
                _foundConstructTypeIndex = _constructType.FoundTypeIndex;
                DeclareEvaluationType(_foundConstructType, typeRestriction);
                if (Usage == ExpressionUsage.LValue)
                    ReportError("Expression cannot be used as the target an assignment (related symbol: '{0}')", _foundConstructType.Name);
                int length = 0;
                for (ValidateParameter validateParameter = _parameterList; validateParameter != null; validateParameter = validateParameter.Next)
                {
                    validateParameter.Validate(context);
                    if (validateParameter.HasErrors)
                        MarkHasErrors();
                    ++length;
                }
                if (HasErrors)
                    return;
                if (length == 0)
                {
                    if (_foundConstructType.HasDefaultConstructor)
                        return;
                    ReportError("A matching constructor could not be found on '{0}'", _foundConstructType.Name);
                }
                else
                {
                    TypeSchema[] parameters = new TypeSchema[length];
                    int index = 0;
                    for (ValidateParameter validateParameter = _parameterList; validateParameter != null; validateParameter = validateParameter.Next)
                    {
                        parameters[index] = validateParameter.FoundParameterType;
                        ++index;
                    }
                    _foundParameterizedConstructor = _foundConstructType.FindConstructor(parameters);
                    if (_foundParameterizedConstructor != null)
                    {
                        _isParameterizedConstruction = true;
                        _foundParameterizedConstructorIndex = Owner.TrackImportedConstructor(_foundParameterizedConstructor);
                    }
                    else
                    {
                        string empty = string.Empty;
                        bool flag = true;
                        foreach (TypeSchema typeSchema in parameters)
                        {
                            if (!flag)
                                empty += ", ";
                            empty += typeSchema.Name;
                            flag = false;
                        }
                        ReportError("A matching constructor could not be found on '{0}' that accepts parameters '{1}'", _foundConstructType.Name, empty);
                    }
                }
            }
        }

        public bool IsParameterizedConstruction => _isParameterizedConstruction;

        public TypeSchema FoundConstructType => _foundConstructType;

        public int FoundConstructTypeIndex => _foundConstructTypeIndex;

        public ConstructorSchema FoundParameterizedConstructor => _foundParameterizedConstructor;

        public int FoundParameterizedConstructorIndex => _foundParameterizedConstructorIndex;
    }
}
