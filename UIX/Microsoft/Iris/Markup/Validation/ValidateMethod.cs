// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateMethod
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateMethod : Microsoft.Iris.Markup.Validation.Validate
    {
        private string _methodName;
        private ValidateTypeIdentifier _returnType;
        private Vector<MethodSpecifier> _specifiers;
        private ValidateParameterDefinitionList _paramList;
        private ValidateCode _body;
        private bool _hasVirtualKeyword;
        private bool _hasOverrideKeyword;
        private MarkupMethodSchema _methodExport;
        private MarkupMethodSchema _foundBaseMethod;

        public ValidateMethod(
          SourceMarkupLoader owner,
          int line,
          int column,
          string methodName,
          ValidateTypeIdentifier returnType,
          Vector<MethodSpecifier> specifiers,
          ValidateParameterDefinitionList paramList,
          ValidateCode body)
          : base(owner, line, column)
        {
            _methodName = methodName;
            _returnType = returnType;
            _specifiers = specifiers;
            _paramList = paramList;
            _body = body;
            _body.MarkAsNotEmbedded();
            _body.DisallowTriggers();
        }

        public void Validate(ValidateClass validateOwner, ValidateContext context)
        {
            if (HasErrors)
                return;
            if (context.CurrentPass == LoadPass.PopulatePublicModel)
            {
                _returnType.Validate();
                if (_paramList != null)
                    _paramList.Validate(context);
                context.NotifyMethodFound(_methodName);
                if (_paramList != null && _paramList.HasErrors || (_returnType.HasErrors || HasErrors))
                {
                    MarkHasErrors();
                }
                else
                {
                    TypeSchema[] parameterTypes = TypeSchema.EmptyList;
                    string[] parameterNames = MarkupMethodSchema.s_emptyStringArray;
                    if (_paramList != null)
                    {
                        int count = _paramList.Parameters.Count;
                        parameterTypes = new TypeSchema[count];
                        parameterNames = new string[count];
                        for (int index = 0; index < count; ++index)
                        {
                            ValidateParameterDefinition parameter = (ValidateParameterDefinition)_paramList.Parameters[index];
                            parameterNames[index] = parameter.Name;
                            parameterTypes[index] = parameter.FoundType;
                        }
                    }
                    _methodExport = MarkupMethodSchema.Build(validateOwner.ObjectType, validateOwner.TypeExport, _methodName, _returnType.FoundType, parameterTypes, parameterNames);
                    for (int index1 = 0; index1 < _specifiers.Count; ++index1)
                    {
                        for (int index2 = index1 + 1; index2 < _specifiers.Count; ++index2)
                        {
                            if (_specifiers[index1] == _specifiers[index2])
                                ReportError("Duplicate modifier '{0}'", _specifiers[index1].ToString());
                        }
                    }
                    _hasVirtualKeyword = _specifiers.Contains(MethodSpecifier.Virtual);
                    _hasOverrideKeyword = _specifiers.Contains(MethodSpecifier.Override);
                    if (!_hasVirtualKeyword || !_hasOverrideKeyword)
                        return;
                    ReportError("Only one of 'virtual' and 'override' is allowed");
                }
            }
            else
            {
                if (context.CurrentPass != LoadPass.Full)
                    return;
                context.NotifyMethodScopeEnter(this);
                MarkupTypeSchema typeSchema = validateOwner.TypeExport.Base as MarkupTypeSchema;
                if (typeSchema != null)
                {
                    MarkupMethodSchema methodExact = FindMethodExact(typeSchema, _methodExport, true);
                    if (methodExact != null)
                    {
                        if (methodExact.IsVirtual && !_hasOverrideKeyword)
                            ReportError("Method '{0}' is virtual in '{1}', must use 'override' to override", _methodName, methodExact.Owner.Name);
                        if (!methodExact.IsVirtual)
                            ReportError("Method '{0}' was already defined in '{1}' with the same signature and was not declared virtual", _methodName, methodExact.Owner.Name);
                        if ((_hasVirtualKeyword || _hasOverrideKeyword) && methodExact.ReturnType != _methodExport.ReturnType)
                            ReportError("Method '{0}' overrides base method from '{1}', but return types do not match: '{2}' on override, '{3}' on base.", _methodName, methodExact.Owner.Name, _methodExport.ReturnType.Name, methodExact.ReturnType.Name);
                        if (methodExact.IsVirtual && _hasOverrideKeyword)
                        {
                            for (MarkupTypeSchema markupTypeSchema = typeSchema; markupTypeSchema != null && _foundBaseMethod == null; markupTypeSchema = markupTypeSchema.Base as MarkupTypeSchema)
                            {
                                foreach (MarkupMethodSchema virtualMethod in markupTypeSchema.VirtualMethods)
                                {
                                    if (virtualMethod.VirtualId == methodExact.VirtualId)
                                    {
                                        _foundBaseMethod = virtualMethod;
                                        break;
                                    }
                                }
                            }
                        }
                        _methodExport.SetVirtualId(methodExact.VirtualId);
                    }
                    else if (_hasOverrideKeyword)
                        ReportError("Method '{0}' declared override, but no base method found to override", _methodName);
                }
                else if (_hasOverrideKeyword)
                    ReportError("Method '{0}' declared override, but no base method found to override", _methodName);
                if (_hasVirtualKeyword || _hasOverrideKeyword)
                    validateOwner.AddVirtualMethod(_methodExport, _foundBaseMethod);
                if (_paramList != null)
                    _paramList.Validate(context);
                _body.Validate(new TypeRestriction(_returnType.FoundType), context);
                context.NotifyMethodScopeExit(this);
            }
        }

        private MarkupMethodSchema FindMethodExact(
          MarkupTypeSchema typeSchema,
          MarkupMethodSchema methodCheck,
          bool deep)
        {
            MarkupTypeSchema markupTypeSchema = typeSchema;
            do
            {
                foreach (MarkupMethodSchema method in markupTypeSchema.Methods)
                {
                    if (IsExactMatch(method, methodCheck))
                        return method;
                }
                markupTypeSchema = markupTypeSchema.Base as MarkupTypeSchema;
            }
            while (deep && markupTypeSchema != null);
            return null;
        }

        public static bool IsExactMatch(MarkupMethodSchema method, MarkupMethodSchema methodCheck)
        {
            if (method == null || methodCheck == null || (!(method.Name == methodCheck.Name) || method.ParameterTypes.Length != methodCheck.ParameterTypes.Length))
                return false;
            bool flag = true;
            for (int index = 0; index < method.ParameterTypes.Length; ++index)
            {
                if (method.ParameterTypes[index] != methodCheck.ParameterTypes[index])
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public MarkupMethodSchema MethodExport => _methodExport;

        public MarkupMethodSchema FoundBaseMethod => _foundBaseMethod;

        public bool HasVirtualKeyword => _hasVirtualKeyword;

        public bool HasOverrideKeyword => _hasOverrideKeyword;

        public ValidateCode Body => _body;

        public override string ToString()
        {
            string str = "";
            if (_paramList != null)
            {
                foreach (ValidateParameterDefinition parameter in _paramList.Parameters)
                {
                    if (str.Length > 0)
                        str += ", ";
                    str += parameter.FoundType.ToString() + " " + parameter.Name;
                }
            }
            return string.Format("{0} {1}({2})", _methodName, _returnType.FoundType, str);
        }
    }
}
