// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateExpressionCall
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateExpressionCall : ValidateExpression
    {
        private ValidateExpression _target;
        private ValidateTypeIdentifier _typeIdentifier;
        private string _memberName;
        private ValidateParameter _parameterList;
        private TypeSchema[] _foundParameterTypes;
        private SchemaType _foundMemberType;
        private int _foundMemberIndex = -1;
        private bool _foundTargetIsStatic;
        private object _foundCanonicalInstance;
        private bool _isIndexAssignment;

        public ValidateExpressionCall(
          SourceMarkupLoader owner,
          ValidateExpression target,
          string memberName,
          ValidateParameter parameterList,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Call)
        {
            _target = target;
            _memberName = memberName;
            _parameterList = parameterList;
        }

        public ValidateExpressionCall(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeIdentifier,
          string memberName,
          ValidateParameter parameterList,
          int line,
          int column)
          : base(owner, line, column, ExpressionType.Call)
        {
            _typeIdentifier = typeIdentifier;
            _memberName = memberName;
            _parameterList = parameterList;
        }

        public ValidateExpression Target => !_foundTargetIsStatic ? _target : null;

        public string MemberName => _memberName;

        public ValidateParameter ParameterList => _parameterList;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            int foundTargetTypeIndex = -1;
            TypeSchema targetType;
            if (_target != null)
            {
                if (_target.ExpressionType == ExpressionType.Symbol)
                {
                    ValidateExpressionSymbol target = (ValidateExpressionSymbol)_target;
                    target.Validate(TypeRestriction.NotVoid, context, true);
                    foundTargetTypeIndex = target.FoundSymbolIndex;
                    _foundTargetIsStatic = target.FoundSymbolIsType;
                }
                else
                    _target.Validate(TypeRestriction.NotVoid, context);
                if (_target.HasErrors)
                {
                    MarkHasErrors();
                    return;
                }
                targetType = _target.ObjectType;
            }
            else
            {
                _typeIdentifier.Validate();
                if (_typeIdentifier.HasErrors)
                {
                    MarkHasErrors();
                    return;
                }
                targetType = _typeIdentifier.FoundType;
                foundTargetTypeIndex = _typeIdentifier.FoundTypeIndex;
                _foundTargetIsStatic = true;
            }
            ExpressionRestriction expressionRestriction = ExpressionRestriction.None;
            bool canRead = true;
            bool canWrite = false;
            _foundParameterTypes = BuildParameterTypeList(context);
            if (_foundParameterTypes == null)
                return;
            if (_target != null && _target.ExpressionType == ExpressionType.BaseClass)
                ValidateBaseCall(typeRestriction, context);
            else if (_parameterList == null)
            {
                if (!ValidatePropertyOrEvent(targetType, typeRestriction, context, foundTargetTypeIndex, ref expressionRestriction, ref canRead, ref canWrite))
                    return;
            }
            else if (!ValidateMethodCall(targetType, typeRestriction, context, ref expressionRestriction))
                return;
            if (expressionRestriction == ExpressionRestriction.NoAccess)
            {
                ReportError("Expression access to '{0}' is not available", _memberName);
            }
            else
            {
                if (Usage == ExpressionUsage.LValue && (!canWrite || expressionRestriction == ExpressionRestriction.ReadOnly))
                    ReportError("Expression access to '{0}' only supports read operations", _memberName);
                if (Usage == ExpressionUsage.RValue && !canRead)
                    ReportError("Expression access to '{0}' only supports write operations", _memberName);
                if (HasErrors)
                    return;
                if (_foundMemberType == SchemaType.Property && Usage == ExpressionUsage.LValue)
                    context.Owner.NotifyFoundPropertyAssignment(this);
                if (_foundMemberType != SchemaType.Method)
                    return;
                context.Owner.NotifyFoundMethodCall(this);
            }
        }

        private bool ValidatePropertyOrEvent(
          TypeSchema targetType,
          TypeRestriction typeRestriction,
          ValidateContext context,
          int foundTargetTypeIndex,
          ref ExpressionRestriction expressionRestriction,
          ref bool canRead,
          ref bool canWrite)
        {
            PropertySchema propertyDeep = targetType.FindPropertyDeep(_memberName);
            if (propertyDeep != null && (propertyDeep.IsStatic || !_foundTargetIsStatic))
            {
                _foundMemberType = SchemaType.Property;
                _foundMemberIndex = Owner.TrackImportedProperty(propertyDeep);
                expressionRestriction = propertyDeep.ExpressionRestriction;
                canRead = propertyDeep.CanRead;
                canWrite = propertyDeep.CanWrite && !propertyDeep.Owner.IsRuntimeImmutable;
                DeclareEvaluationType(propertyDeep.PropertyType, typeRestriction);
                if (!_foundTargetIsStatic && propertyDeep.NotifiesOnChange)
                    DeclareNotifies(context);
            }
            if (_foundMemberType == SchemaType.None && !_foundTargetIsStatic)
            {
                EventSchema eventDeep = targetType.FindEventDeep(_memberName);
                if (eventDeep != null)
                {
                    _foundMemberType = SchemaType.Event;
                    _foundMemberIndex = Owner.TrackImportedEvent(eventDeep);
                    expressionRestriction = ExpressionRestriction.ReadOnly;
                    DeclareEvaluationType(VoidSchema.Type, typeRestriction);
                    DeclareNotifies(context);
                }
            }
            if (_foundMemberType == SchemaType.None && _foundTargetIsStatic)
            {
                object canonicalInstance = targetType.FindCanonicalInstance(_memberName);
                if (canonicalInstance != null)
                {
                    _foundMemberType = SchemaType.CanonicalInstance;
                    _foundMemberIndex = foundTargetTypeIndex;
                    _foundCanonicalInstance = canonicalInstance;
                    expressionRestriction = ExpressionRestriction.ReadOnly;
                    DeclareEvaluationType(targetType, typeRestriction);
                }
            }
            if (_foundMemberType != SchemaType.None)
                return true;
            ReportError("Unable to find a Property, Event, or Method called \"{0}\" on '{1}'", _memberName, targetType.Name);
            return false;
        }

        private bool ValidateMethodCall(
          TypeSchema targetType,
          TypeRestriction typeRestriction,
          ValidateContext context,
          ref ExpressionRestriction expressionRestriction)
        {
            expressionRestriction = ExpressionRestriction.ReadOnly;
            TypeSchema[] foundParameterTypes = _foundParameterTypes;
            MethodSchema methodDeep = targetType.FindMethodDeep(_memberName, foundParameterTypes);
            if (methodDeep != null && (methodDeep.IsStatic || !_foundTargetIsStatic))
            {
                _foundMemberType = SchemaType.Method;
                _foundMemberIndex = Owner.TrackImportedMethod(methodDeep);
                DeclareEvaluationType(methodDeep.ReturnType, typeRestriction);
            }
            if (_foundMemberType != SchemaType.None)
                return true;
            if (foundParameterTypes.Length == 0)
            {
                ReportError("Unable to find a Property, Event, or Method called \"{0}\" on '{1}'", _memberName, targetType.Name);
                return false;
            }
            string empty = string.Empty;
            bool flag = true;
            foreach (TypeSchema typeSchema in foundParameterTypes)
            {
                if (!flag)
                    empty += ", ";
                empty += typeSchema.Name;
                flag = false;
            }
            ReportError("Unable to find a Method \"{0}\" that accepts parameters '{1}' on '{2}'", _memberName, empty, targetType.Name);
            return false;
        }

        private void ValidateBaseCall(TypeRestriction typeRestriction, ValidateContext context)
        {
            MarkupMethodSchema markupMethodSchema = context.CurrentMethod != null ? context.CurrentMethod.FoundBaseMethod : null;
            if (markupMethodSchema != null)
            {
                if (_parameterList == null || _memberName != markupMethodSchema.Name)
                    ReportError("'base' keyword can only be used to call the base virtual method inside an override");
                else if (new MethodSignatureKey(markupMethodSchema.Name, markupMethodSchema.ParameterTypes).Equals(new MethodSignatureKey(_memberName, _foundParameterTypes)))
                {
                    _foundMemberType = SchemaType.Method;
                    _foundMemberIndex = Owner.TrackImportedMethod(markupMethodSchema);
                    DeclareEvaluationType(markupMethodSchema.ReturnType, typeRestriction);
                }
                else
                    ReportError("'base' keyword can only be used to call the base virtual method inside an override");
            }
            else
                MarkHasErrors();
        }

        private TypeSchema[] BuildParameterTypeList(ValidateContext context)
        {
            TypeSchema[] typeSchemaArray = TypeSchema.EmptyList;
            if (_parameterList != ValidateParameter.EmptyList)
            {
                int length = 0;
                for (ValidateParameter validateParameter = _parameterList; validateParameter != null; validateParameter = validateParameter.Next)
                {
                    validateParameter.Validate(context);
                    if (validateParameter.HasErrors)
                        MarkHasErrors();
                    ++length;
                }
                if (HasErrors)
                    return null;
                typeSchemaArray = new TypeSchema[length];
                int index = 0;
                for (ValidateParameter validateParameter = _parameterList; validateParameter != null; validateParameter = validateParameter.Next)
                {
                    typeSchemaArray[index] = validateParameter.FoundParameterType;
                    ++index;
                }
            }
            return typeSchemaArray;
        }

        public void SetAsIndexAssignment() => _isIndexAssignment = true;

        public SchemaType FoundMemberType => _foundMemberType;

        public int FoundMemberIndex => _foundMemberIndex;

        public object FoundCanonicalInstance => _foundCanonicalInstance;

        public bool IsIndexAssignment => _isIndexAssignment;
    }
}
