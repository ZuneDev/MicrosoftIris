// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupEncoder
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Debug;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.Markup.Validation;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Markup
{
    internal class MarkupEncoder
    {
        private ByteCodeWriter _writer;
        private MarkupConstantsTable _constantsTable;
        private MarkupLineNumberTable _lineNumberTable;
        private string _sourceFilePathBestGuess;

        public MarkupEncoder(
          MarkupImportTables importTables,
          MarkupConstantsTable constantsTable,
          MarkupLineNumberTable lineNumberTable)
        {
            _constantsTable = constantsTable;
            _lineNumberTable = lineNumberTable;
        }

        public ByteCodeReader EncodeOBJECTSection(
          ParseResult parseResult,
          string uri,
          string sourceFilePathBestGuess)
        {
            _sourceFilePathBestGuess = sourceFilePathBestGuess;
            _writer = new ByteCodeWriter();
            if (parseResult.ClassList.Count > 0)
            {
                for (int index = 0; index < parseResult.ClassList.Count; ++index)
                    EncodeClass(parseResult.ClassList[index]);
            }
            ByteCodeReader reader = _writer.CreateReader();
            _writer = null;
            return reader;
        }

        private void EncodeClass(ValidateClass cls)
        {
            ValidateUI ui = null;
            if (cls.ObjectType == UISchema.Type)
                ui = (ValidateUI)cls;
            ValidateEffect validateEffect = null;
            if (cls.ObjectType == EffectSchema.Type)
                validateEffect = (ValidateEffect)cls;
            if (cls.IndirectedObject == null)
            {
                int totalInitialEvaluates = 0;
                int totalFinalEvaluates = 0;
                if (cls.ActionList.Count > 0)
                    EncodeScript(cls.ActionList, out totalInitialEvaluates, out totalFinalEvaluates);
                if (cls.TriggerList.Count > 0)
                    EncodeListenerGroupRefresh(cls);
                if (cls.MethodList != null && cls.MethodList.Count > 0)
                    EncodeMethods(cls.TypeExport, cls.MethodList);
                if (cls.FoundPropertiesValidateProperty != null)
                {
                    uint offset = GetOffset();
                    cls.TypeExport.SetInitializePropertiesOffset(offset);
                    EncodeInitializeProperty(cls.FoundPropertiesValidateProperty);
                    cls.RemoveProperty(cls.FoundPropertiesValidateProperty);
                    _writer.WriteByte(OpCode.ReturnVoid);
                }
                if (ui != null && ui.FoundContentValidateProperty != null)
                {
                    uint offset = GetOffset();
                    cls.TypeExport.SetInitializeContentOffset(offset);
                    EncodeInitializeProperty(ui.FoundContentValidateProperty);
                    cls.RemoveProperty(ui.FoundContentValidateProperty);
                    _writer.WriteByte(OpCode.ReturnVoid);
                }
                if (validateEffect != null)
                    EncodeTechniquesProperty((EffectClassTypeSchema)cls.TypeExport, validateEffect);
                uint offset1 = GetOffset();
                cls.TypeExport.SetInitializeLocalsInputOffset(offset1);
                for (ValidateProperty property = cls.PropertyList; property != null; property = property.Next)
                    EncodeInitializeProperty(property);
                if (cls.TypeExport.ListenerCount > 0U)
                    EncodeListenerInitialize(cls);
                if (totalInitialEvaluates > 0)
                    EstablishInitialOrFinalEvaluateOffsets(cls, totalInitialEvaluates, true);
                if (totalFinalEvaluates > 0)
                    EstablishInitialOrFinalEvaluateOffsets(cls, totalFinalEvaluates, false);
                _writer.WriteByte(OpCode.ReturnVoid);
                if (ui == null)
                    return;
                EncodeNamedContent(ui);
            }
            else
            {
                EncodeObjectBySource(cls.IndirectedObject);
                _writer.WriteByte(OpCode.ReturnValue);
            }
        }

        private void EncodeConstructObject(ValidateObjectTag objectTag)
        {
            if (objectTag.IndirectedObject == null)
            {
                if (objectTag.DynamicConstructionType != null)
                {
                    EncodeObjectBySource(objectTag.DynamicConstructionType);
                    _writer.WriteByte(OpCode.ConstructObjectIndirect);
                    _writer.WriteUInt16(objectTag.FoundTypeIndex);
                    bool forceVerifyValues = objectTag.FoundType is MarkupTypeSchema;
                    EncodeInitializeProperties(objectTag, forceVerifyValues);
                    EncodeObjectBySource(objectTag.DynamicConstructionType);
                    if (!objectTag.FoundType.HasInitializer)
                        return;
                    _writer.WriteByte(OpCode.InitializeInstanceIndirect);
                }
                else
                {
                    _writer.WriteByte(OpCode.ConstructObject);
                    _writer.WriteUInt16(objectTag.FoundTypeIndex);
                    EncodeInitializeProperties(objectTag);
                    EncodeInitializeInstance(objectTag.FoundType, objectTag.FoundTypeIndex);
                }
            }
            else
                EncodeObjectBySource(objectTag.IndirectedObject);
        }

        private void EncodeInitializeProperties(ValidateObjectTag objectTag) => EncodeInitializeProperties(objectTag, false);

        private void EncodeInitializeProperties(ValidateObjectTag objectTag, bool forceVerifyValues)
        {
            if (objectTag.PropertyCount <= 0)
                return;
            for (ValidateProperty property = objectTag.PropertyList; property != null; property = property.Next)
                EncodeInitializeProperty(property, objectTag.DynamicConstructionType);
        }

        private void EncodeInitializeInstance(TypeSchema type, int typeIndex)
        {
            if (!type.HasInitializer)
                return;
            _writer.WriteByte(OpCode.InitializeInstance);
            _writer.WriteUInt16(typeIndex);
        }

        private void EncodeInitializeProperty(ValidateProperty property) => EncodeInitializeProperty(property, null);

        private void EncodeInitializeProperty(
          ValidateProperty property,
          ValidateObject dynamicConstructionType)
        {
            if (property.IsPseudo)
                return;
            if (property.ValueApplyMode == ValueApplyMode.SingleValueSet)
            {
                EncodeObjectBySource(property.Value);
                RecordLineNumber(property);
                if (dynamicConstructionType == null)
                {
                    _writer.WriteByte(OpCode.PropertyInitialize);
                    _writer.WriteUInt16(property.FoundPropertyIndex);
                }
                else
                {
                    EncodeObjectBySource(dynamicConstructionType);
                    _writer.WriteByte(OpCode.PropertyInitializeIndirect);
                    _writer.WriteUInt16(property.FoundPropertyIndex);
                }
            }
            else
            {
                if ((property.ValueApplyMode & ValueApplyMode.CollectionPopulateAndSet) != ValueApplyMode.SingleValueSet)
                {
                    _writer.WriteByte(OpCode.ConstructObject);
                    _writer.WriteUInt16(property.FoundPropertyTypeIndex);
                }
                for (ValidateObjectTag next = (ValidateObjectTag)property.Value; next != null; next = next.Next)
                {
                    if (!next.PropertyIsRequiredForCreation)
                    {
                        uint fixUpLocation = uint.MaxValue;
                        if (property.ShouldSkipDictionaryAddIfContains)
                        {
                            _writer.WriteByte(OpCode.JumpIfDictionaryContains);
                            _writer.WriteUInt16((property.ValueApplyMode & ValueApplyMode.CollectionAdd) != ValueApplyMode.SingleValueSet ? property.FoundPropertyIndex : -1);
                            _writer.WriteUInt16(_constantsTable.Add(StringSchema.Type, next.Name, MarkupConstantPersistMode.Binary));
                            fixUpLocation = GetOffset();
                            _writer.WriteUInt32(uint.MaxValue);
                        }
                        EncodeConstructObject(next);
                        if ((property.ValueApplyMode & ValueApplyMode.MultiValueDictionary) != ValueApplyMode.SingleValueSet)
                        {
                            _writer.WriteByte(OpCode.PropertyDictionaryAdd);
                            _writer.WriteUInt16((property.ValueApplyMode & ValueApplyMode.CollectionAdd) != ValueApplyMode.SingleValueSet ? property.FoundPropertyIndex : -1);
                            _writer.WriteUInt16(_constantsTable.Add(StringSchema.Type, next.Name, MarkupConstantPersistMode.Binary));
                        }
                        else
                        {
                            _writer.WriteByte(OpCode.PropertyListAdd);
                            _writer.WriteUInt16((property.ValueApplyMode & ValueApplyMode.CollectionAdd) != ValueApplyMode.SingleValueSet ? property.FoundPropertyIndex : -1);
                        }
                        if (fixUpLocation != uint.MaxValue)
                            FixUpJumpOffset(fixUpLocation);
                    }
                }
                if ((property.ValueApplyMode & ValueApplyMode.CollectionPopulateAndSet) == ValueApplyMode.SingleValueSet)
                    return;
                if (dynamicConstructionType == null)
                {
                    _writer.WriteByte(OpCode.PropertyInitialize);
                    _writer.WriteUInt16(property.FoundPropertyIndex);
                }
                else
                {
                    EncodeObjectBySource(dynamicConstructionType);
                    RecordLineNumber(property);
                    _writer.WriteByte(OpCode.PropertyInitializeIndirect);
                    _writer.WriteUInt16(property.FoundPropertyIndex);
                }
            }
        }

        private void EncodeObjectBySource(ValidateObject obj)
        {
            switch (obj.ObjectSourceType)
            {
                case ObjectSourceType.ObjectTag:
                    EncodeConstructObject((ValidateObjectTag)obj);
                    break;
                case ObjectSourceType.FromString:
                    EncodeFromString((ValidateFromString)obj);
                    break;
                case ObjectSourceType.Code:
                    EncodeCode((ValidateCode)obj);
                    break;
                case ObjectSourceType.Expression:
                    EncodeExpression((ValidateExpression)obj, null);
                    break;
            }
        }

        private void EncodeFromString(ValidateFromString fromString)
        {
            if (fromString.ObjectType.IsRuntimeImmutable)
            {
                MarkupConstantPersistMode persistMode;
                object persistData;
                if (fromString.ObjectType.SupportsBinaryEncoding)
                {
                    persistMode = MarkupConstantPersistMode.Binary;
                    persistData = fromString.FromStringInstance;
                }
                else
                {
                    persistMode = MarkupConstantPersistMode.FromString;
                    persistData = fromString.FromString;
                }
                int rawValue = _constantsTable.Add(fromString.ObjectType, fromString.FromStringInstance, persistMode, persistData);
                _writer.WriteByte(OpCode.PushConstant);
                _writer.WriteUInt16(rawValue);
            }
            else if (fromString.ObjectType.SupportsBinaryEncoding)
            {
                _writer.WriteByte(OpCode.ConstructFromBinary);
                _writer.WriteUInt16(fromString.TypeHintIndex);
                fromString.ObjectType.EncodeBinary(_writer, fromString.FromStringInstance);
            }
            else
            {
                int rawValue = _constantsTable.Add(StringSchema.Type, fromString.FromString, MarkupConstantPersistMode.FromString);
                _writer.WriteByte(OpCode.ConstructFromString);
                _writer.WriteUInt16(fromString.TypeHintIndex);
                _writer.WriteUInt16(rawValue);
            }
        }

        private void EncodeCanonicalInstance(object instance, TypeSchema type, string memberName)
        {
            int rawValue = _constantsTable.Add(type, instance, MarkupConstantPersistMode.Canonical, memberName);
            _writer.WriteByte(OpCode.PushConstant);
            _writer.WriteUInt16(rawValue);
        }

        private void EncodeNamedContent(ValidateUI ui)
        {
            if (ui.FoundNamedContentProperties == null)
                return;
            NamedContentRecord[] namedContentTable = ((UIClassTypeSchema)ui.TypeExport).NamedContentTable;
            int index = 0;
            foreach (ValidateProperty namedContentProperty in ui.FoundNamedContentProperties)
            {
                uint offset = GetOffset();
                namedContentTable[index].SetOffset(offset);
                EncodeObjectBySource(namedContentProperty.Value);
                _writer.WriteByte(OpCode.ReturnValue);
                ++index;
            }
        }

        private void EncodeCode(ValidateCode code)
        {
            uint offset = GetOffset();
            code.TrackEncodingOffset(offset);
            EncodeStatement(code.StatementCompound);
            if (code.ReturnStatements != null)
            {
                foreach (ValidateStatementReturn returnStatement in code.ReturnStatements)
                {
                    if (!returnStatement.IsTrailingReturn)
                        FixUpJumpOffset(returnStatement.JumpFixupOffset);
                }
            }
            if (code.Embedded)
                return;
            if (code.ObjectType != VoidSchema.Type)
                _writer.WriteByte(OpCode.ReturnValue);
            else
                _writer.WriteByte(OpCode.ReturnVoid);
        }

        private void EncodeStatement(ValidateStatement statement)
        {
            RecordLineNumber(statement);
            if (statement.StatementType != StatementType.Compound)
                DeclareDebugPoint(statement.Line, statement.Column);
            switch (statement.StatementType)
            {
                case StatementType.Assignment:
                    ValidateStatementAssignment statementAssignment = (ValidateStatementAssignment)statement;
                    if (statementAssignment.DeclaredScopedLocal != null)
                        EncodeStatement(statementAssignment.DeclaredScopedLocal);
                    EncodeExpression(statementAssignment.RValue);
                    EncodeExpression(statementAssignment.LValue);
                    _writer.WriteByte(OpCode.DiscardValue);
                    break;
                case StatementType.Expression:
                    ValidateStatementExpression statementExpression = (ValidateStatementExpression)statement;
                    EncodeExpression(statementExpression.Expression);
                    if (statementExpression.Expression.ObjectType == VoidSchema.Type)
                        break;
                    _writer.WriteByte(OpCode.DiscardValue);
                    break;
                case StatementType.ForEach:
                    ValidateStatementForEach statementForEach = (ValidateStatementForEach)statement;
                    EncodeScopedLocal(statementForEach.ScopedLocal);
                    EncodeExpression(statementForEach.Expression);
                    _writer.WriteByte(OpCode.MethodInvoke);
                    _writer.WriteUInt16(statementForEach.FoundGetEnumeratorIndex);
                    uint offset1 = GetOffset();
                    _writer.WriteByte(OpCode.MethodInvokePeek);
                    _writer.WriteUInt16(statementForEach.FoundMoveNextIndex);
                    _writer.WriteByte(OpCode.JumpIfFalse);
                    uint offset2 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    _writer.WriteByte(OpCode.PropertyGetPeek);
                    _writer.WriteUInt16(statementForEach.FoundCurrentIndex);
                    _writer.WriteByte(OpCode.VerifyTypeCast);
                    _writer.WriteUInt16(statementForEach.ScopedLocal.FoundTypeIndex);
                    _writer.WriteByte(OpCode.WriteSymbol);
                    _writer.WriteUInt16(statementForEach.ScopedLocal.FoundSymbolIndex);
                    EncodeStatement(statementForEach.StatementCompound);
                    _writer.WriteByte(OpCode.Jump);
                    _writer.WriteUInt32(offset1);
                    FixUpJumpOffset(offset2);
                    uint offset3 = GetOffset();
                    _writer.WriteByte(OpCode.DiscardValue);
                    EncodeScopedLocalsWipe(statementForEach.ScopedLocalsToClear);
                    foreach (ValidateStatementBreak breakStatement in statementForEach.BreakStatements)
                    {
                        if (breakStatement.IsContinue)
                            FixUpJumpOffset(breakStatement.JumpFixupOffset, offset1);
                        else
                            FixUpJumpOffset(breakStatement.JumpFixupOffset, offset3);
                    }
                    break;
                case StatementType.While:
                    ValidateStatementWhile validateStatementWhile = (ValidateStatementWhile)statement;
                    uint offset4 = GetOffset();
                    if (validateStatementWhile.IsDoWhile)
                        EncodeStatement(validateStatementWhile.Body);
                    EncodeExpression(validateStatementWhile.Condition);
                    _writer.WriteByte(OpCode.JumpIfFalse);
                    uint offset5 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    if (!validateStatementWhile.IsDoWhile)
                        EncodeStatement(validateStatementWhile.Body);
                    _writer.WriteByte(OpCode.Jump);
                    _writer.WriteUInt32(offset4);
                    FixUpJumpOffset(offset5);
                    uint offset6 = GetOffset();
                    foreach (ValidateStatementBreak breakStatement in validateStatementWhile.BreakStatements)
                    {
                        if (breakStatement.IsContinue)
                            FixUpJumpOffset(breakStatement.JumpFixupOffset, offset4);
                        else
                            FixUpJumpOffset(breakStatement.JumpFixupOffset, offset6);
                    }
                    break;
                case StatementType.If:
                    ValidateStatementIf validateStatementIf = (ValidateStatementIf)statement;
                    EncodeExpression(validateStatementIf.Condition);
                    _writer.WriteByte(OpCode.JumpIfFalse);
                    uint offset7 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    EncodeStatement(validateStatementIf.StatementCompound);
                    FixUpJumpOffset(offset7);
                    break;
                case StatementType.IfElse:
                    ValidateStatementIfElse validateStatementIfElse = (ValidateStatementIfElse)statement;
                    EncodeExpression(validateStatementIfElse.Condition);
                    _writer.WriteByte(OpCode.JumpIfFalse);
                    uint offset8 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    EncodeStatement(validateStatementIfElse.StatementCompoundTrue);
                    _writer.WriteByte(OpCode.Jump);
                    uint offset9 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    FixUpJumpOffset(offset8);
                    EncodeStatement(validateStatementIfElse.StatementCompoundFalse);
                    FixUpJumpOffset(offset9);
                    break;
                case StatementType.Return:
                    ValidateStatementReturn validateStatementReturn = (ValidateStatementReturn)statement;
                    if (validateStatementReturn.Expression != null)
                        EncodeExpression(validateStatementReturn.Expression);
                    if (validateStatementReturn.IsTrailingReturn)
                        break;
                    EncodeScopedLocalsWipe(validateStatementReturn.ScopedLocalsToClear);
                    _writer.WriteByte(OpCode.Jump);
                    uint offset10 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    validateStatementReturn.TrackJumpFixupOffset(offset10);
                    break;
                case StatementType.ScopedLocal:
                    EncodeScopedLocal((ValidateStatementScopedLocal)statement);
                    break;
                case StatementType.Compound:
                    ValidateStatementCompound statementCompound = (ValidateStatementCompound)statement;
                    for (ValidateStatement statement1 = statementCompound.StatementList; statement1 != null; statement1 = statement1.Next)
                        EncodeStatement(statement1);
                    EncodeScopedLocalsWipe(statementCompound.ScopedLocalsToClear);
                    break;
                case StatementType.Break:
                    ValidateStatementBreak validateStatementBreak = (ValidateStatementBreak)statement;
                    EncodeScopedLocalsWipe(validateStatementBreak.ScopedLocalsToClear);
                    _writer.WriteByte(OpCode.Jump);
                    uint offset11 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    validateStatementBreak.TrackJumpFixupOffset(offset11);
                    break;
            }
        }

        private void EncodeExpression(ValidateExpression expression) => EncodeExpression(expression, null);

        private void EncodeExpression(
          ValidateExpression expression,
          ListenerEncodeMode listenerEncodeMode)
        {
            RecordLineNumber(expression);
            expression.TrackEncodingOffset(GetOffset());
            switch (expression.ExpressionType)
            {
                case ExpressionType.Assignment:
                    ValidateExpressionAssignment expressionAssignment = (ValidateExpressionAssignment)expression;
                    if (!expressionAssignment.IsIndexAssignment)
                        EncodeExpression(expressionAssignment.RValue);
                    EncodeExpression(expressionAssignment.LValue);
                    break;
                case ExpressionType.Symbol:
                    bool flag1 = true;
                    ValidateExpressionSymbol expressionSymbol = (ValidateExpressionSymbol)expression;
                    if (expressionSymbol.NotifyIndex >= 0 && listenerEncodeMode != null)
                    {
                        EncodeListenerHookup(listenerEncodeMode, expressionSymbol.NotifyIndex, ListenerType.Symbol, expressionSymbol.FoundSymbolIndex, expressionSymbol.IsNotifierRoot);
                        flag1 = !expressionSymbol.IsNotifierRoot;
                    }
                    if (!flag1)
                        break;
                    _writer.WriteByte(expressionSymbol.Usage != ExpressionUsage.RValue ? OpCode.WriteSymbolPeek : OpCode.LookupSymbol);
                    _writer.WriteUInt16(expressionSymbol.FoundSymbolIndex);
                    break;
                case ExpressionType.Call:
                    ValidateExpressionCall validateExpressionCall = (ValidateExpressionCall)expression;
                    bool flag2 = false;
                    if (validateExpressionCall.Target != null)
                    {
                        EncodeExpression(validateExpressionCall.Target, listenerEncodeMode);
                        flag2 = true;
                    }
                    switch (validateExpressionCall.FoundMemberType)
                    {
                        case SchemaType.Property:
                            bool flag3 = true;
                            if (validateExpressionCall.NotifyIndex >= 0 && listenerEncodeMode != null)
                            {
                                EncodeListenerHookup(listenerEncodeMode, validateExpressionCall.NotifyIndex, ListenerType.Property, validateExpressionCall.FoundMemberIndex, validateExpressionCall.IsNotifierRoot);
                                flag3 = !validateExpressionCall.IsNotifierRoot;
                            }
                            if (!flag3)
                                return;
                            OpCode opCode = validateExpressionCall.Usage != ExpressionUsage.RValue ? (flag2 ? OpCode.PropertyAssign : OpCode.PropertyAssignStatic) : (flag2 ? OpCode.PropertyGet : OpCode.PropertyGetStatic);
                            RecordLineNumber(expression);
                            _writer.WriteByte(opCode);
                            _writer.WriteUInt16(validateExpressionCall.FoundMemberIndex);
                            return;
                        case SchemaType.Method:
                            if (validateExpressionCall.ParameterList != ValidateParameter.EmptyList)
                            {
                                for (ValidateParameter validateParameter = validateExpressionCall.ParameterList; validateParameter != null; validateParameter = validateParameter.Next)
                                    EncodeExpression(validateParameter.Expression);
                            }
                            RecordLineNumber(expression);
                            _writer.WriteByte(validateExpressionCall.IsIndexAssignment ? (flag2 ? OpCode.MethodInvokePushLastParam : OpCode.MethodInvokeStaticPushLastParam) : (flag2 ? OpCode.MethodInvoke : OpCode.MethodInvokeStatic));
                            _writer.WriteUInt16(validateExpressionCall.FoundMemberIndex);
                            return;
                        case SchemaType.Event:
                            if (validateExpressionCall.NotifyIndex < 0 || listenerEncodeMode == null)
                                return;
                            EncodeListenerHookup(listenerEncodeMode, validateExpressionCall.NotifyIndex, ListenerType.Event, validateExpressionCall.FoundMemberIndex, validateExpressionCall.IsNotifierRoot);
                            return;
                        case SchemaType.CanonicalInstance:
                            EncodeCanonicalInstance(validateExpressionCall.FoundCanonicalInstance, validateExpressionCall.ObjectType, validateExpressionCall.MemberName);
                            return;
                        default:
                            return;
                    }
                case ExpressionType.Cast:
                    ValidateExpressionCast validateExpressionCast = (ValidateExpressionCast)expression;
                    EncodeExpression(validateExpressionCast.Castee, listenerEncodeMode);
                    RecordLineNumber(expression);
                    if (validateExpressionCast.FoundCastMethod == CastMethod.Cast)
                    {
                        _writer.WriteByte(OpCode.VerifyTypeCast);
                        _writer.WriteUInt16(validateExpressionCast.FoundTypeCastIndex);
                        break;
                    }
                    _writer.WriteByte(OpCode.ConvertType);
                    _writer.WriteUInt16(validateExpressionCast.FoundTypeCastIndex);
                    _writer.WriteUInt16(validateExpressionCast.FoundCasteeTypeIndex);
                    break;
                case ExpressionType.New:
                    ValidateExpressionNew validateExpressionNew = (ValidateExpressionNew)expression;
                    if (!validateExpressionNew.IsParameterizedConstruction)
                    {
                        _writer.WriteByte(OpCode.ConstructObject);
                        _writer.WriteUInt16(validateExpressionNew.FoundConstructTypeIndex);
                    }
                    else
                    {
                        for (ValidateParameter validateParameter = validateExpressionNew.ParameterList; validateParameter != null; validateParameter = validateParameter.Next)
                            EncodeExpression(validateParameter.Expression);
                        RecordLineNumber(expression);
                        _writer.WriteByte(OpCode.ConstructObjectParam);
                        _writer.WriteUInt16(validateExpressionNew.FoundConstructTypeIndex);
                        _writer.WriteUInt16(validateExpressionNew.FoundParameterizedConstructorIndex);
                    }
                    EncodeInitializeInstance(validateExpressionNew.FoundConstructType, validateExpressionNew.FoundConstructTypeIndex);
                    break;
                case ExpressionType.Operation:
                    ValidateExpressionOperation expressionOperation = (ValidateExpressionOperation)expression;
                    EncodeExpression(expressionOperation.LeftSide);
                    uint fixUpLocation = uint.MaxValue;
                    if (expressionOperation.FoundOperationTargetType == BooleanSchema.Type && (expressionOperation.Op == OperationType.LogicalAnd || expressionOperation.Op == OperationType.LogicalOr))
                    {
                        RecordLineNumber(expression);
                        _writer.WriteByte(expressionOperation.Op == OperationType.LogicalOr ? OpCode.JumpIfTruePeek : OpCode.JumpIfFalsePeek);
                        fixUpLocation = GetOffset();
                        _writer.WriteUInt32(uint.MaxValue);
                    }
                    if (expressionOperation.RightSide != null)
                        EncodeExpression(expressionOperation.RightSide);
                    RecordLineNumber(expression);
                    _writer.WriteByte(OpCode.Operation);
                    _writer.WriteUInt16(expressionOperation.FoundOperationTargetTypeIndex);
                    _writer.WriteByte((byte)expressionOperation.Op);
                    if (fixUpLocation == uint.MaxValue)
                        break;
                    FixUpJumpOffset(fixUpLocation);
                    break;
                case ExpressionType.IsCheck:
                    ValidateExpressionIsCheck expressionIsCheck = (ValidateExpressionIsCheck)expression;
                    EncodeExpression(expressionIsCheck.Expression);
                    RecordLineNumber(expression);
                    _writer.WriteByte(OpCode.IsCheck);
                    _writer.WriteUInt16(expressionIsCheck.TypeIdentifier.FoundTypeIndex);
                    break;
                case ExpressionType.As:
                    ValidateExpressionAs validateExpressionAs = (ValidateExpressionAs)expression;
                    EncodeExpression(validateExpressionAs.Expression);
                    RecordLineNumber(expression);
                    _writer.WriteByte(OpCode.As);
                    _writer.WriteUInt16(validateExpressionAs.TypeIdentifier.FoundTypeIndex);
                    break;
                case ExpressionType.Constant:
                    ValidateExpressionConstant expressionConstant = (ValidateExpressionConstant)expression;
                    if (expressionConstant.ConstantType != ConstantType.Null)
                    {
                        int rawValue = _constantsTable.Add(expressionConstant.ObjectType, expressionConstant.FoundConstant, MarkupConstantPersistMode.Binary);
                        _writer.WriteByte(OpCode.PushConstant);
                        _writer.WriteUInt16(rawValue);
                        break;
                    }
                    _writer.WriteByte(OpCode.PushNull);
                    break;
                case ExpressionType.DeclareTrigger:
                    EncodeExpression(((ValidateExpressionDeclareTrigger)expression).Expression);
                    break;
                case ExpressionType.TypeOf:
                    ValidateExpressionTypeOf expressionTypeOf = (ValidateExpressionTypeOf)expression;
                    _writer.WriteByte(OpCode.TypeOf);
                    _writer.WriteUInt16(expressionTypeOf.TypeIdentifier.FoundTypeIndex);
                    break;
                case ExpressionType.List:
                    ArrayList expressions = ((ValidateExpressionList)expression).Expressions;
                    for (int index = 0; index < expressions.Count; ++index)
                    {
                        ValidateExpression expression1 = (ValidateExpression)expressions[index];
                        EncodeExpression(expression1);
                        if (index < expressions.Count - 1 && expression1.ObjectType != VoidSchema.Type)
                            _writer.WriteByte(OpCode.DiscardValue);
                    }
                    break;
                case ExpressionType.Index:
                    EncodeExpression(((ValidateExpressionIndex)expression).CallExpression);
                    break;
                case ExpressionType.Ternary:
                    ValidateExpressionTernary expressionTernary = (ValidateExpressionTernary)expression;
                    EncodeExpression(expressionTernary.Condition);
                    _writer.WriteByte(OpCode.JumpIfFalse);
                    uint offset1 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    EncodeExpression(expressionTernary.TrueClause);
                    _writer.WriteByte(OpCode.Jump);
                    uint offset2 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    FixUpJumpOffset(offset1);
                    EncodeExpression(expressionTernary.FalseClause);
                    FixUpJumpOffset(offset2);
                    break;
                case ExpressionType.NullCoalescing:
                    ValidateExpressionNullCoalescing expressionNullCoalescing = (ValidateExpressionNullCoalescing)expression;
                    EncodeExpression(expressionNullCoalescing.Condition);
                    _writer.WriteByte(OpCode.JumpIfNullPeek);
                    uint offset3 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    _writer.WriteByte(OpCode.Jump);
                    uint offset4 = GetOffset();
                    _writer.WriteUInt32(uint.MaxValue);
                    FixUpJumpOffset(offset3);
                    _writer.WriteByte(OpCode.DiscardValue);
                    EncodeExpression(expressionNullCoalescing.NullClause);
                    FixUpJumpOffset(offset4);
                    break;
                case ExpressionType.This:
                case ExpressionType.BaseClass:
                    _writer.WriteByte(OpCode.PushThis);
                    break;
            }
        }

        private void EncodeScopedLocal(ValidateStatementScopedLocal statementScopedLocal)
        {
            if (statementScopedLocal.HasInitialAssignment)
                return;
            if (!statementScopedLocal.FoundType.IsNullAssignable)
            {
                _writer.WriteByte(OpCode.ConstructObject);
                _writer.WriteUInt16(statementScopedLocal.FoundTypeIndex);
            }
            else
                _writer.WriteByte(OpCode.PushNull);
            _writer.WriteByte(OpCode.WriteSymbol);
            _writer.WriteUInt16(statementScopedLocal.FoundSymbolIndex);
        }

        private void EncodeScopedLocalsWipe(Vector<int> scopedLocalsToClear)
        {
            if (scopedLocalsToClear == null)
                return;
            foreach (int rawValue in scopedLocalsToClear)
            {
                _writer.WriteByte(OpCode.ClearSymbol);
                _writer.WriteUInt16(rawValue);
            }
        }

        private bool EncodeListenerHookup(
          ListenerEncodeMode listenerEncodeMode,
          int localListenerIndex,
          ListenerType listenerType,
          int targetIndex,
          bool isRoot)
        {
            uint rawValue = listenerEncodeMode.SequentialListenerIndex((uint)localListenerIndex);
            if (isRoot)
            {
                _writer.WriteByte(OpCode.Listen);
                _writer.WriteUInt16(rawValue);
                _writer.WriteByte((byte)listenerType);
                _writer.WriteUInt16(targetIndex);
                _writer.WriteUInt32(listenerEncodeMode.ScriptId);
            }
            else
            {
                uint num = listenerEncodeMode.RunOnNonTailTrigger ? listenerEncodeMode.ScriptId : uint.MaxValue;
                _writer.WriteByte(OpCode.DestructiveListen);
                _writer.WriteUInt16(rawValue);
                _writer.WriteByte((byte)listenerType);
                _writer.WriteUInt16(targetIndex);
                _writer.WriteUInt32(num);
                _writer.WriteUInt32(listenerEncodeMode.RefreshId);
            }
            return isRoot;
        }

        private void EncodeScript(
          Vector<ValidateCode> actionList,
          out int totalInitialEvaluates,
          out int totalFinalEvaluates)
        {
            totalInitialEvaluates = 0;
            totalFinalEvaluates = 0;
            foreach (ValidateCode action in actionList)
            {
                if (action.InitialEvaluate)
                    ++totalInitialEvaluates;
                if (action.FinalEvaluate)
                    ++totalFinalEvaluates;
                EncodeCode(action);
            }
        }

        private void EncodeMethods(MarkupTypeSchema typeSchema, ArrayList methodList)
        {
            foreach (ValidateMethod method in methodList)
            {
                uint offset = GetOffset();
                EncodeCode(method.Body);
                uint codeOffset = typeSchema.EncodeScriptOffsetAsId(offset);
                method.MethodExport.SetCodeOffset(codeOffset);
            }
        }

        private void EncodeListenerGroupRefresh(ValidateClass cls)
        {
            Vector<TriggerRecord> triggerList = cls.TriggerList;
            ListenerEncodeMode listenerEncodeMode = new ListenerEncodeMode(cls.TypeExport);
            foreach (TriggerRecord triggerRecord in triggerList)
            {
                listenerEncodeMode.TriggerContainer = triggerRecord;
                EncodeExpression(triggerRecord.SourceExpression, listenerEncodeMode);
                _writer.WriteByte(37);
            }
        }

        private void EncodeListenerInitialize(ValidateClass cls)
        {
            _writer.WriteByte(OpCode.ConstructListenerStorage);
            _writer.WriteUInt16(cls.TypeExport.ListenerCount);
            Vector<TriggerRecord> triggerList = cls.TriggerList;
            uint[] refreshGroupOffsets = new uint[triggerList.Count];
            for (int index = 0; index < triggerList.Count; ++index)
            {
                TriggerRecord triggerRecord = triggerList[index];
                refreshGroupOffsets[index] = triggerRecord.SourceExpression.EncodeStartOffset;
            }
            cls.TypeExport.SetRefreshListenerGroupOffsets(refreshGroupOffsets);
        }

        private void EstablishInitialOrFinalEvaluateOffsets(
          ValidateClass cls,
          int totalEvaluates,
          bool isInitialEvaluate)
        {
            Vector<ValidateCode> actionList = cls.ActionList;
            uint[] numArray = new uint[totalEvaluates];
            int num = 0;
            foreach (ValidateCode validateCode in actionList)
            {
                if (isInitialEvaluate && validateCode.InitialEvaluate || !isInitialEvaluate && validateCode.FinalEvaluate)
                    numArray[num++] = validateCode.EncodeStartOffset;
            }
            if (isInitialEvaluate)
                cls.TypeExport.SetInitialEvaluateOffsets(numArray);
            else
                cls.TypeExport.SetFinalEvaluateOffsets(numArray);
        }

        private void EncodeTechniquesProperty(
          EffectClassTypeSchema typeExport,
          ValidateEffect validateEffect)
        {
            ValidateProperty validateProperty = validateEffect.FoundTechniquesValidateProperty;
            if (validateProperty == null)
                return;
            int length = 0;
            ValidateObjectTag validateObjectTag1 = validateProperty.Value as ValidateObjectTag;
            for (ValidateObjectTag validateObjectTag2 = validateObjectTag1; validateObjectTag2 != null; validateObjectTag2 = validateObjectTag2.Next)
                ++length;
            uint[] techniqueOffsets = new uint[length];
            int num = 0;
            for (ValidateObjectTag objectTag = validateObjectTag1; objectTag != null; objectTag = objectTag.Next)
            {
                techniqueOffsets[num++] = GetOffset();
                EncodeConstructObject(objectTag);
                _writer.WriteByte(OpCode.ReturnValue);
            }
            typeExport.SetTechniqueOffsets(techniqueOffsets);
            validateEffect.RemoveProperty(validateEffect.FoundTechniquesValidateProperty);
            if (validateEffect.FoundInstancePropertyAssignments == null)
                return;
            uint[] instancePropertyAssignments = new uint[length];
            for (int index = 0; index < length; ++index)
                instancePropertyAssignments[index] = GetOffset();
            foreach (KeyValueEntry<string, ValidateProperty> propertyAssignment in validateEffect.FoundInstancePropertyAssignments)
            {
                int elementSymbolIndex = validateEffect.GetElementSymbolIndex(propertyAssignment.Key);
                _writer.WriteByte(OpCode.LookupSymbol);
                _writer.WriteUInt16(elementSymbolIndex);
                for (ValidateProperty next = propertyAssignment.Value; next != null; next = next.Next)
                    EncodeInitializeProperty(next);
                _writer.WriteByte(OpCode.DiscardValue);
            }
            _writer.WriteByte(OpCode.ReturnVoid);
            typeExport.SetInstancePropertyAssignments(instancePropertyAssignments);
        }

        private uint GetOffset() => _writer.DataSize;

        private void FixUpJumpOffset(uint fixUpLocation, uint fixedOffset) => _writer.Overwrite(fixUpLocation, fixedOffset);

        private void FixUpJumpOffset(uint fixUpLocation) => FixUpJumpOffset(fixUpLocation, GetOffset());

        private void RecordLineNumber(Validate validate) => _lineNumberTable.AddRecord(GetOffset(), validate.Line, validate.Column);

        [Conditional("DEBUG")]
        private void DEBUG_EmitStart()
        {
        }

        [Conditional("DEBUG")]
        private void DEBUG_EmitStop(OpCode opCode) => Debug.Trace.IsCategoryEnabled(TraceCategory.MarkupEncoding);

        [Conditional("DEBUG")]
        private void DEBUG_EmitStop(OpCode opCode, object param) => Debug.Trace.IsCategoryEnabled(TraceCategory.MarkupEncoding);

        [Conditional("DEBUG")]
        private void DEBUG_EmitStop(OpCode opCode, object param, object param2) => Debug.Trace.IsCategoryEnabled(TraceCategory.MarkupEncoding);

        [Conditional("DEBUG")]
        private void DEBUG_EmitStop(OpCode opCode, object param, object param2, object param3) => Debug.Trace.IsCategoryEnabled(TraceCategory.MarkupEncoding);

        [Conditional("DEBUG")]
        private void DEBUG_EmitStop(
          OpCode opCode,
          object param,
          object param2,
          object param3,
          object param4)
        {
            Debug.Trace.IsCategoryEnabled(TraceCategory.MarkupEncoding);
        }

        [Conditional("DEBUG")]
        private void DEBUG_EmitStop(
          OpCode opCode,
          object param,
          object param2,
          object param3,
          object param4,
          object param5)
        {
            Debug.Trace.IsCategoryEnabled(TraceCategory.MarkupEncoding);
        }

        [Conditional("DEBUG")]
        private void DEBUG_EmitStopWorker(
          OpCode opCode,
          object data0,
          object data1,
          object data2,
          object data3,
          object data4)
        {
        }

        [Conditional("DEBUG")]
        private void DEBUG_ReportFixup(uint fixupLocation, uint value)
        {
        }

        private void DeclareDebugPoint(int line, int column)
        {
        }
    }
}
