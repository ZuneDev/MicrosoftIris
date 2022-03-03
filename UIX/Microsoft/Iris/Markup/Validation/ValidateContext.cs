// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateContext
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using System.Collections.Generic;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateContext
    {
        private ValidateClass _owner;
        private MarkupTypeSchema _baseType;
        private LoadPass _currentPass;
        private SymbolOrigin _currentScope;
        private ValidateMethod _currentMethod;
        private SymbolResolutionDirective _resolutionDirective;
        private Vector<SymbolRecord> _symbolTable = new Vector<SymbolRecord>();
        private Vector<SymbolReference> _usedSymbols = new Vector<SymbolReference>();
        private Vector<Vector<SymbolRecord>> _scopedLocalFrameStack = new Vector<Vector<SymbolRecord>>();
        private Stack<int> _loopFramesStack = new Stack<int>();
        private Stack<ValidateStatementLoop> _loopStatementStack = new Stack<ValidateStatementLoop>();
        private Vector<SymbolRecord> _methodParameterRecords;
        private Stack<ValidateContext.PropertyScopeRecord> _propertyScopeStack = new Stack<ValidateContext.PropertyScopeRecord>();
        private Vector<ValidateExpression> _declaredTriggerTrackingList;
        private ValidateExpression _notifierTrackingRoot;
        private uint _notifierCount;
        private Vector<TriggerRecord> _triggerList = new Vector<TriggerRecord>();
        private Vector<ValidateCode> _actionList = new Vector<ValidateCode>();
        private Map<string, TypeSchema> _reservedSymbols;
        public static PropertySchema ClassPropertiesProperty;
        public static PropertySchema UIPropertiesProperty;
        private static PropertySchema ClassLocalsProperty;
        private static PropertySchema UILocalsProperty;
        private static PropertySchema UIInputProperty;
        private static PropertySchema UIContentProperty;
        private static PropertySchema EffectTechniquesProperty;
        private static string[] ReservedNameList = new string[4]
        {
      "Name",
      "RepeatedItem",
      "RepeatedItemIndex",
      "this"
        };

        public ValidateContext(ValidateClass owner, MarkupTypeSchema baseType, LoadPass currentPass)
        {
            _owner = owner;
            _currentScope = SymbolOrigin.None;
            _baseType = baseType;
            _currentPass = currentPass;
        }

        public static void InitializeStatics()
        {
            ClassPropertiesProperty = ClassSchema.Type.FindProperty("Properties");
            UIPropertiesProperty = UISchema.Type.FindProperty("Properties");
            ClassLocalsProperty = ClassSchema.Type.FindProperty("Locals");
            UILocalsProperty = UISchema.Type.FindProperty("Locals");
            UIInputProperty = UISchema.Type.FindProperty("Input");
            UIContentProperty = UISchema.Type.FindProperty("Content");
            EffectTechniquesProperty = EffectSchema.Type.FindProperty("Techniques");
        }

        public TypeSchema ResolveSymbol(
          string name,
          out SymbolOrigin origin,
          out ExpressionRestriction expressionRestriction)
        {
            return ResolveSymbol(name, true, _resolutionDirective, out origin, out expressionRestriction, out TypeSchema _);
        }

        public TypeSchema ResolveSymbolNoBaseTypes(string name, out SymbolOrigin origin) => ResolveSymbol(name, false, _resolutionDirective, out origin, out ExpressionRestriction _, out TypeSchema _);

        private TypeSchema IsSymbolAlreadyDefined(
          string name,
          out SymbolOrigin origin,
          out ExpressionRestriction expressionRestriction,
          out TypeSchema baseTypeOrigin)
        {
            return ResolveSymbol(name, true, SymbolResolutionDirective.None, out origin, out expressionRestriction, out baseTypeOrigin);
        }

        private TypeSchema ResolveSymbol(
          string name,
          bool searchBaseTypes,
          SymbolResolutionDirective resolutionDirective,
          out SymbolOrigin origin,
          out ExpressionRestriction expressionRestriction,
          out TypeSchema baseTypeOrigin)
        {
            baseTypeOrigin = null;
            TypeSchema type = ResolveSymbol(_symbolTable, name, out origin, out expressionRestriction);
            if (type == null && searchBaseTypes)
            {
                if (resolutionDirective != SymbolResolutionDirective.PropertyResolution)
                {
                    for (MarkupTypeSchema markupTypeSchema = _baseType; markupTypeSchema != null; markupTypeSchema = markupTypeSchema.MarkupTypeBase)
                    {
                        if (markupTypeSchema.InheritableSymbolsTable != null)
                            type = ResolveSymbol(markupTypeSchema.InheritableSymbolsTable, name, out origin, out expressionRestriction);
                        if (type != null)
                        {
                            baseTypeOrigin = markupTypeSchema;
                            break;
                        }
                    }
                }
                else if (_baseType != null && _baseType.FindPropertyDeep(name) is MarkupPropertySchema propertyDeep && propertyDeep.RequiredForCreation)
                {
                    type = propertyDeep.PropertyType;
                    origin = SymbolOrigin.Properties;
                    expressionRestriction = propertyDeep.ExpressionRestriction;
                    baseTypeOrigin = propertyDeep.Owner;
                }
            }
            if (type == null && name == "this" && _owner.ObjectType == ClassSchema.Type)
            {
                type = _owner.TypeExport;
                origin = SymbolOrigin.Reserved;
                expressionRestriction = ExpressionRestriction.ReadOnly;
            }
            if (type != null)
                _owner.Owner.TrackImportedType(type);
            return type;
        }

        private TypeSchema ResolveSymbol(
          Vector<SymbolRecord> symbolTable,
          string name,
          out SymbolOrigin origin,
          out ExpressionRestriction expressionRestriction)
        {
            foreach (SymbolRecord symbolRecord in symbolTable)
            {
                if (symbolRecord.Name == name)
                {
                    origin = symbolRecord.SymbolOrigin;
                    expressionRestriction = ExpressionRestriction.None;
                    if (symbolRecord.SymbolOrigin == SymbolOrigin.Content || symbolRecord.SymbolOrigin == SymbolOrigin.Input)
                        expressionRestriction = ExpressionRestriction.ReadOnly;
                    return symbolRecord.Type;
                }
            }
            TypeSchema type;
            if (_reservedSymbols != null && _reservedSymbols.TryGetValue(name, out type))
            {
                origin = SymbolOrigin.Reserved;
                expressionRestriction = ExpressionRestriction.ReadOnly;
                _owner.Owner.TrackImportedType(type);
                return type;
            }
            origin = SymbolOrigin.None;
            expressionRestriction = ExpressionRestriction.None;
            return null;
        }

        private TypeSchema ResolveSymbol(
          SymbolRecord[] symbolTable,
          string name,
          out SymbolOrigin origin,
          out ExpressionRestriction expressionRestriction)
        {
            foreach (SymbolRecord symbolRecord in symbolTable)
            {
                if (symbolRecord.Name == name)
                {
                    origin = symbolRecord.SymbolOrigin;
                    expressionRestriction = ExpressionRestriction.None;
                    if (symbolRecord.SymbolOrigin == SymbolOrigin.Content || symbolRecord.SymbolOrigin == SymbolOrigin.Input)
                        expressionRestriction = ExpressionRestriction.ReadOnly;
                    return symbolRecord.Type;
                }
            }
            TypeSchema type;
            if (_reservedSymbols != null && _reservedSymbols.TryGetValue(name, out type))
            {
                origin = SymbolOrigin.Reserved;
                expressionRestriction = ExpressionRestriction.ReadOnly;
                _owner.Owner.TrackImportedType(type);
                return type;
            }
            origin = SymbolOrigin.None;
            expressionRestriction = ExpressionRestriction.None;
            return null;
        }

        public void DeclareReservedSymbols(Map<string, TypeSchema> reservedSymbols)
        {
            if (_reservedSymbols != null)
                return;
            _reservedSymbols = reservedSymbols;
        }

        public void NotifyObjectTagScopeEnter(ValidateObjectTag objectTag)
        {
            if (_owner == null)
                return;
            _owner.NotifyDiscoveredObjectTag(objectTag);
        }

        public Result NotifyObjectTagScopeExit(ValidateObjectTag objectTag)
        {
            TypeSchema typeSchema1 = objectTag.ObjectType;
            NameUsage activeNameUsage = GetActiveNameUsage();
            string name1 = objectTag.Name;
            Result result = Result.Success;
            if (name1 != null && typeSchema1 != ClassSchema.Type && (typeSchema1 != UISchema.Type && typeSchema1 != EffectSchema.Type) && (typeSchema1 != DataTypeSchema.Type && typeSchema1 != DataQuerySchema.Type && (activeNameUsage != NameUsage.DictionaryKeys && activeNameUsage != NameUsage.NamedContent)))
            {
                if (_currentScope == SymbolOrigin.Techniques && EffectElementSchema.Type.IsAssignableFrom(typeSchema1))
                {
                    string name2 = typeSchema1.Name;
                    string name3 = name2 + "Instance";
                    typeSchema1 = MarkupSystem.UIXGlobal.FindType(name3);
                    if (typeSchema1 == null)
                        result = Result.Fail(string.Format("Element '{0}' has no properties that can be changed dynamically and therefore cannot be named", name2));
                    else
                        _owner.Owner.TrackImportedType(typeSchema1);
                }
                SymbolOrigin origin;
                TypeSchema baseTypeOrigin;
                TypeSchema typeSchema2 = IsSymbolAlreadyDefined(name1, out origin, out ExpressionRestriction _, out baseTypeOrigin);
                if (!result.Failed && typeSchema2 != null)
                {
                    if (baseTypeOrigin != null)
                    {
                        if (_currentScope == SymbolOrigin.Properties && origin == SymbolOrigin.Properties)
                        {
                            if (!typeSchema2.IsAssignableFrom(typeSchema1))
                                result = Result.Fail(string.Format("Property '{0}' exists in base class '{1}' with type '{2}' and type override '{3}' does not match", name1, baseTypeOrigin.Name, typeSchema2.Name, typeSchema1.Name));
                            else if (objectTag.PropertyOverrideCriteria != null)
                            {
                                MarkupPropertySchema propertyDeep = (MarkupPropertySchema)_baseType.FindPropertyDeep(objectTag.Name);
                                result = objectTag.PropertyOverrideCriteria.Verify(propertyDeep.OverrideCriteria);
                            }
                        }
                        else if (_currentScope != SymbolOrigin.Content || origin != SymbolOrigin.Content)
                        {
                            if (_currentScope == origin)
                                result = Result.Fail("Name \"{0}\" (also defined in base class '{1}') cannot be overridden (tried to override {2}, but can only override Properties and Content)", name1, baseTypeOrigin.Name, origin.ToString());
                            else
                                result = Result.Fail(string.Format("Name \"{0}\" (located in {1}) cannot override the same name within base class '{2}' (located in {3}) since overrides cannot cross section types", name1, _currentScope.ToString(), baseTypeOrigin.Name, origin.ToString()));
                        }
                    }
                    else if (origin != SymbolOrigin.Reserved)
                        result = Result.Fail("Name \"{0}\" is already in use (type '{1}') located in '{2}'", name1, typeSchema2.Name, origin.ToString());
                }
                if (!result.Failed)
                {
                    if (IsNameReserved(name1))
                        result = Result.Fail("Name \"{0}\" is reserved and cannot be used.", name1);
                    else if (!IsValidSymbolName(name1))
                        result = Result.Fail("Invalid name \"{0}\".  Valid names must begin with either an alphabetic character or an underscore and can otherwise contain only alphabetic, numeric, or underscore characters", name1);
                }
                if (!result.Failed && typeSchema1 != null)
                    _symbolTable.Add(new SymbolRecord()
                    {
                        Name = name1,
                        Type = typeSchema1,
                        SymbolOrigin = _currentScope
                    });
            }
            return result;
        }

        public void NotifyPropertyScopeEnter(ValidateProperty property)
        {
            PropertySchema foundProperty = property.FoundProperty;
            NameUsage nameUsage;
            if (foundProperty == ClassPropertiesProperty || foundProperty == UIPropertiesProperty)
            {
                _currentScope = SymbolOrigin.Properties;
                nameUsage = NameUsage.Symbols;
                _resolutionDirective = SymbolResolutionDirective.PropertyResolution;
            }
            else if (foundProperty == ClassLocalsProperty || foundProperty == UILocalsProperty)
            {
                _currentScope = SymbolOrigin.Locals;
                nameUsage = NameUsage.Symbols;
            }
            else if (foundProperty == UIInputProperty)
            {
                _currentScope = SymbolOrigin.Input;
                nameUsage = NameUsage.Symbols;
            }
            else if (foundProperty == UIContentProperty)
            {
                if (property.PropertyAttributeList == null)
                {
                    _currentScope = SymbolOrigin.Content;
                    nameUsage = NameUsage.Symbols;
                }
                else
                    nameUsage = NameUsage.NamedContent;
            }
            else if (foundProperty == EffectTechniquesProperty)
            {
                _currentScope = SymbolOrigin.Techniques;
                nameUsage = NameUsage.Symbols;
            }
            else
                nameUsage = !DictionarySchema.Type.IsAssignableFrom(foundProperty.PropertyType) ? NameUsage.Default : NameUsage.DictionaryKeys;
            _propertyScopeStack.Push(new ValidateContext.PropertyScopeRecord()
            {
                nameUsage = nameUsage,
                property = foundProperty
            });
        }

        public void NotifyPropertyScopeExit(ValidateProperty property)
        {
            PropertySchema foundProperty = property.FoundProperty;
            if (foundProperty == ClassPropertiesProperty || foundProperty == UIPropertiesProperty)
            {
                _resolutionDirective = SymbolResolutionDirective.None;
                _currentScope = SymbolOrigin.None;
            }
            else if (foundProperty == ClassLocalsProperty || foundProperty == UILocalsProperty)
                _currentScope = SymbolOrigin.None;
            else if (foundProperty == UIInputProperty)
                _currentScope = SymbolOrigin.None;
            else if (foundProperty == UIContentProperty && property.PropertyAttributeList == null)
                _currentScope = SymbolOrigin.None;
            else if (foundProperty == EffectTechniquesProperty)
                _currentScope = SymbolOrigin.None;
            _propertyScopeStack.Pop();
        }

        public void NotifyMethodScopeEnter(ValidateMethod method)
        {
            _currentMethod = method;
            if (_methodParameterRecords == null)
                _methodParameterRecords = new Vector<SymbolRecord>();
            MarkupMethodSchema methodExport = method.MethodExport;
            for (int index = 0; index < methodExport.ParameterNames.Length; ++index)
            {
                string parameterName = methodExport.ParameterNames[index];
                Result result = IsNameInUse(parameterName, false);
                if (result.Failed)
                {
                    method.ReportError(result.ToString());
                }
                else
                {
                    SymbolRecord symbolRecord = new SymbolRecord();
                    symbolRecord.Name = parameterName;
                    symbolRecord.Type = methodExport.ParameterTypes[index];
                    symbolRecord.SymbolOrigin = SymbolOrigin.Parameter;
                    _symbolTable.Add(symbolRecord);
                    _methodParameterRecords.Add(symbolRecord);
                }
            }
        }

        public void NotifyMethodScopeExit(ValidateMethod method)
        {
            foreach (SymbolRecord methodParameterRecord in _methodParameterRecords)
                _symbolTable.Remove(methodParameterRecord);
            _methodParameterRecords.Clear();
            _currentMethod = null;
        }

        public Result NotifyMethodFound(string name)
        {
            Result result = IsNameInUse(name, false);
            if (result.Failed)
                return result;
            _symbolTable.Add(new SymbolRecord()
            {
                Name = name,
                Type = null,
                SymbolOrigin = SymbolOrigin.Methods
            });
            return Result.Success;
        }

        public bool IsNameReserved(string name)
        {
            foreach (string reservedName in ReservedNameList)
            {
                if (name == reservedName)
                    return true;
            }
            return _reservedSymbols != null && _reservedSymbols.ContainsKey(name);
        }

        public static bool IsValidSymbolName(string name)
        {
            bool flag = true;
            for (int index = 0; index < name.Length; ++index)
            {
                char ch = name[index];
                if (ch != '_' && (ch < 'a' || ch > 'z') && (ch < 'A' || ch > 'Z') && (ch < '0' || ch > '9' || index == 0))
                {
                    flag = false;
                    break;
                }
            }
            if (name.Length == 0)
                flag = false;
            return flag;
        }

        public Result IsNameInUse(string name, bool bypassReservedNameCheck)
        {
            SymbolOrigin origin;
            TypeSchema typeSchema = ResolveSymbol(name, out origin, out ExpressionRestriction _);
            if (typeSchema != null)
                return Result.Fail("Name \"{0}\" is already in use (type '{1}') located in '{2}'", name, typeSchema.Name, origin.ToString());
            if (!bypassReservedNameCheck)
            {
                if (IsNameReserved(name))
                    return Result.Fail("Name \"{0}\" is reserved and cannot be used.", name);
                if (!IsValidSymbolName(name))
                    return Result.Fail("Invalid name \"{0}\".  Valid names must begin with either an alphabetic character or an underscore and can otherwise contain only alphabetic, numeric, or underscore characters", name);
            }
            return Result.Success;
        }

        public void NotifyScopedLocalFrameEnter(ValidateStatementLoop statementLoop)
        {
            _scopedLocalFrameStack.Add(null);
            if (statementLoop == null)
                return;
            _loopFramesStack.Push(_scopedLocalFrameStack.Count);
            _loopStatementStack.Push(statementLoop);
        }

        public void NotifyScopedLocalFrameEnter() => NotifyScopedLocalFrameEnter(null);

        public Result NotifyScopedLocal(string name, TypeSchema type) => NotifyScopedLocal(name, type, false, SymbolOrigin.ScopedLocal);

        public Result NotifyScopedLocal(
          string name,
          TypeSchema type,
          bool bypassReservedNameCheck,
          SymbolOrigin symbolOrigin)
        {
            Result result = IsNameInUse(name, bypassReservedNameCheck);
            if (result.Failed)
                return result;
            SymbolRecord symbolRecord = new SymbolRecord();
            symbolRecord.Name = name;
            symbolRecord.Type = type;
            symbolRecord.SymbolOrigin = symbolOrigin;
            _symbolTable.Add(symbolRecord);
            Vector<SymbolRecord> vector = _scopedLocalFrameStack[_scopedLocalFrameStack.Count - 1];
            if (vector == null)
            {
                vector = new Vector<SymbolRecord>();
                _scopedLocalFrameStack.RemoveAt(_scopedLocalFrameStack.Count - 1);
                _scopedLocalFrameStack.Add(vector);
            }
            vector.Add(symbolRecord);
            return Result.Success;
        }

        public Vector<int> NotifyScopedLocalFrameExit()
        {
            if (_loopFramesStack.Count > 0 && _scopedLocalFrameStack.Count == _loopFramesStack.Peek())
            {
                _loopFramesStack.Pop();
                _loopStatementStack.Pop();
            }
            Vector<SymbolRecord> scopedLocalFrame = _scopedLocalFrameStack[_scopedLocalFrameStack.Count - 1];
            _scopedLocalFrameStack.RemoveAt(_scopedLocalFrameStack.Count - 1);
            Vector<int> vector = null;
            if (scopedLocalFrame != null)
            {
                vector = new Vector<int>();
                foreach (SymbolRecord symbolRecord in scopedLocalFrame)
                {
                    _symbolTable.Remove(symbolRecord);
                    int num = TrackSymbolUsage(symbolRecord.Name, symbolRecord.SymbolOrigin);
                    if (symbolRecord.SymbolOrigin == SymbolOrigin.ScopedLocal)
                        vector.Add(num);
                }
            }
            return vector == null || vector.Count <= 0 ? null : vector;
        }

        private Vector<int> GetImmediateFrameUnwindList(SourceMarkupLoader owner, bool stopAtLoop)
        {
            Vector<int> vector = null;
            int num1 = stopAtLoop ? _loopFramesStack.Peek() : 0;
            for (int index = _scopedLocalFrameStack.Count - 1; index >= num1; --index)
            {
                Vector<SymbolRecord> scopedLocalFrame = _scopedLocalFrameStack[index];
                if (scopedLocalFrame != null)
                {
                    if (vector == null)
                        vector = new Vector<int>();
                    foreach (SymbolRecord symbolRecord in scopedLocalFrame)
                    {
                        int num2 = TrackSymbolUsage(symbolRecord.Name, symbolRecord.SymbolOrigin);
                        if (symbolRecord.SymbolOrigin == SymbolOrigin.ScopedLocal)
                            vector.Add(num2);
                    }
                }
            }
            return vector == null || vector.Count <= 0 ? null : vector;
        }

        public Vector<int> GetImmediateFrameUnwindList(SourceMarkupLoader owner) => GetImmediateFrameUnwindList(owner, false);

        public Vector<int> GetLoopUnwindList(SourceMarkupLoader owner) => GetImmediateFrameUnwindList(owner, true);

        public ValidateStatementLoop EnclosingLoop => _loopStatementStack.Count <= 0 ? null : _loopStatementStack.Peek();

        public int TrackSymbolUsage(string symbol, SymbolOrigin origin)
        {
            for (int index = 0; index < _usedSymbols.Count; ++index)
            {
                SymbolReference usedSymbol = _usedSymbols[index];
                if (usedSymbol.Symbol == symbol && usedSymbol.Origin == origin)
                    return index;
            }
            _usedSymbols.Add(new SymbolReference(symbol, origin));
            return _usedSymbols.Count - 1;
        }

        public SymbolReference[] SymbolReferenceTable
        {
            get
            {
                SymbolReference[] symbolReferenceArray = new SymbolReference[_usedSymbols.Count];
                for (int index = 0; index < _usedSymbols.Count; ++index)
                    symbolReferenceArray[index] = _usedSymbols[index];
                return symbolReferenceArray;
            }
        }

        public SymbolRecord[] InheritableSymbolsTable
        {
            get
            {
                SymbolRecord[] symbolRecordArray = new SymbolRecord[_symbolTable.Count];
                for (int index = 0; index < _symbolTable.Count; ++index)
                    symbolRecordArray[index] = _symbolTable[index];
                return symbolRecordArray;
            }
        }

        public void StartDeclaredTriggerTracking() => _declaredTriggerTrackingList = new Vector<ValidateExpression>();

        public void TrackDeclaredTrigger(ValidateExpression expression) => _declaredTriggerTrackingList.Add(expression);

        public bool IsTrackingDeclaredTriggers => _declaredTriggerTrackingList != null;

        public Vector<ValidateExpression> StopDeclaredTriggerTracking()
        {
            Vector<ValidateExpression> triggerTrackingList = _declaredTriggerTrackingList;
            _declaredTriggerTrackingList = null;
            return triggerTrackingList;
        }

        public void StartNotifierTracking(ValidateExpression root) => _notifierTrackingRoot = root;

        public bool IsNotifierTracking => _notifierTrackingRoot != null;

        public int TrackDeclareNotifies(ValidateExpression expression) => IsNotifierTracking ? (int)_notifierCount++ : -1;

        public bool StopNotifierTracking()
        {
            bool flag;
            if (_notifierTrackingRoot.NotifyIndex != -1)
            {
                _notifierTrackingRoot.MarkNotifierRoot();
                flag = true;
            }
            else
                flag = false;
            _notifierTrackingRoot = null;
            return flag;
        }

        public uint NotifierCount => _notifierCount;

        public void RegisterAction(ValidateCode actionCode) => _actionList.Add(actionCode);

        public void RegisterTrigger(ValidateExpression sourceExpression, ValidateCode actionCode) => _triggerList.Add(new TriggerRecord()
        {
            SourceExpression = sourceExpression,
            ActionCode = actionCode
        });

        public Vector<TriggerRecord> TriggerList => _triggerList;

        public PropertySchema GetActivePropertyScope() => _propertyScopeStack.Count == 0 ? null : _propertyScopeStack.Peek().property;

        public NameUsage GetActiveNameUsage()
        {
            NameUsage nameUsage = NameUsage.Default;
            if (_propertyScopeStack.Count == 0)
            {
                nameUsage = NameUsage.Unknown;
            }
            else
            {
                foreach (ValidateContext.PropertyScopeRecord propertyScope in _propertyScopeStack)
                {
                    nameUsage = propertyScope.nameUsage;
                    if (nameUsage != NameUsage.Default)
                        break;
                }
            }
            return nameUsage;
        }

        public SymbolOrigin ActiveSymbolScope => _currentScope;

        public Vector<ValidateCode> ActionList => _actionList;

        public LoadPass CurrentPass => _currentPass;

        public ValidateClass Owner => _owner;

        public ValidateMethod CurrentMethod => _currentMethod;

        internal struct PropertyScopeRecord
        {
            public NameUsage nameUsage;
            public PropertySchema property;
        }
    }
}
