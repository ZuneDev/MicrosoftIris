using System;
using System.Collections;
using System.Linq;
using Microsoft.Iris.Debug;
using Microsoft.Iris.Debug.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup
{
    // Token: 0x0200018B RID: 395
    internal class Interpreter
    {
        // Token: 0x06000F21 RID: 3873 RVA: 0x00029E78 File Offset: 0x00028E78
        public static object Run(InterpreterContext context)
        {
            object result = null;
            ByteCodeReader byteCodeReader = null;
            long num = -1L;
            bool isErrored = true;
            ErrorManager.EnterContext(context);
            try
            {
                byteCodeReader = context.LoadResult.ObjectSection;
                num = (long)(ulong)byteCodeReader.CurrentOffset;
                byteCodeReader.CurrentOffset = context.InitialBytecodeOffset;
                result = Run(context, byteCodeReader);
                isErrored = false;
            }
            finally
            {
                if (isErrored)
                    ExceptionContext = context.ToString();
                ErrorManager.ExitContext();

                if (byteCodeReader != null && num != -1L)
                    byteCodeReader.CurrentOffset = (uint)num;
            }
            return result;
        }
        
        // Token: 0x06000F22 RID: 3874 RVA: 0x00029EF8 File Offset: 0x00028EF8
        private static object Run(InterpreterContext context, ByteCodeReader reader)
        {
            MarkupLoadResult loadResult = context.LoadResult;
            IMarkupTypeBase instance = context.Instance;
            MarkupImportTables importTables = loadResult.ImportTables;
            MarkupConstantsTable constantsTable = loadResult.ConstantsTable;
            SymbolReference[] symbolReferenceTable = context.MarkupType.SymbolReferenceTable;
            ErrorWatermark watermark = ErrorManager.Watermark;

            Trace.IsCategoryEnabled(TraceCategory.Markup);

            Stack stack = _stack;
            int count = stack.Count;
            if (instance != null)
            {
                stack.Push(instance);
            }

            bool errorsDetected = false;
            object result = null;
            bool wasInDebugState = false;
            bool debugging = Application.Debugger != null;
            
            void OnDecode(InterpreterEntry entry)
            {
                if (!debugging)
                    return;
                
                Application.Debugger.LogInterpreterDecode(context, entry.Instruction);

                // Fetch line and column numbers from the table
                uint offset = entry.Instruction.Offset;
                int line = -1, column = -1;
                context.LoadResult.LineNumberTable.TryLookup(offset, out line, out column);

                bool ShouldBreak(Breakpoint b) =>
                    b.Enabled && b.Equals(loadResult, line, column, offset);

                // Check if a breakpoint has been set at this location
                bool shouldBreakHere = Application.DebugSettings.Breakpoints.Any(ShouldBreak);
                if (shouldBreakHere)
                    Application.Debugger.DebuggerCommand = InterpreterCommand.Break;

                // Stop execution while the debugger is in break mode
                Application.Debugger.WaitForContinue();

                // If the debugger requested a single step, immediately set the debugger
                // to break again for the next instruction
                if (Application.Debugger.DebuggerCommand == InterpreterCommand.Step)
                    Application.Debugger.DebuggerCommand = InterpreterCommand.Break;
            }

            while (!errorsDetected)
            {
                OpCode opCode = (OpCode)reader.ReadByte();
                InterpreterEntry entry = new(new(opCode, reader.CurrentOffset, loadResult.Uri));

                switch (opCode)
                {
                    case OpCode.ConstructObject:
                        {
                            int typeIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(typeIndex);
                            OnDecode(entry);
                            
                            TypeSchema typeSchema = importTables.TypeImports[typeIndex];
                            entry.Parameters.Add(new InterpreterObject("type", typeof(TypeSchema),
                                typeSchema, InstructionObjectSource.TypeImports, typeIndex));

                            object newObj = typeSchema.ConstructDefault();
                            
                            ReportErrorOnNull(newObj, "Construction", typeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                RegisterDisposable(newObj, typeSchema, instance);

                                stack.Push(newObj);
                                entry.ReturnValues.Add(new(newObj));
                            }
                            break;
                        }
                    case OpCode.ConstructObjectIndirect:
                        {
                            int assignmentTypeIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(assignmentTypeIndex);
                            OnDecode(entry);
                            
                            TypeSchema assignmentTypeSchema = importTables.TypeImports[assignmentTypeIndex];
                            entry.Parameters.Add(new("assignmentType", typeof(TypeSchema),
                                assignmentTypeSchema, InstructionObjectSource.TypeImports, assignmentTypeIndex));

                            TypeSchema targetTypeSchema = (TypeSchema)stack.Pop();
                            entry.Parameters.Add(new("targetType", typeof(TypeSchema),
                                targetTypeSchema, InstructionObjectSource.Stack));

                            if (!assignmentTypeSchema.IsAssignableFrom(targetTypeSchema))
                            {
                                ErrorManager.ReportError("Script runtime failure: Dynamic construction type override failed. Attempting to construct '{0}' in place of '{1}'", (targetTypeSchema != null) ? targetTypeSchema.Name : "null", assignmentTypeSchema.Name);
                                if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                    break;
                            }

                            object newObj = targetTypeSchema is IDynamicConstructionSchema dynamicConstructionSchema
                                ? dynamicConstructionSchema.ConstructDefault(assignmentTypeSchema)
                                : targetTypeSchema.ConstructDefault();

                            ReportErrorOnNull(newObj, "Construction", targetTypeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                RegisterDisposable(newObj, targetTypeSchema, instance);

                                stack.Push(newObj);
                                entry.ReturnValues.Add(new(newObj));
                            }
                            break;
                        }
                    case OpCode.ConstructObjectParam:
                        {
                            int targetTypeIndex = reader.ReadUInt16();
                            int constructorIndex = reader.ReadUInt16();
                            
                            entry.Instruction.Operands.Add(targetTypeIndex);
                            entry.Instruction.Operands.Add(constructorIndex);
                            OnDecode(entry);
                            
                            TypeSchema targetTypeSchema = importTables.TypeImports[targetTypeIndex];
                            ConstructorSchema constructorSchema = importTables.ConstructorImports[constructorIndex];

                            int parameterCount = constructorSchema.ParameterTypes.Length;
                            object[] array = ParameterListAllocator.Alloc(parameterCount);
                            for (parameterCount--; parameterCount >= 0; parameterCount--)
                                array[parameterCount] = stack.Pop();
                            entry.Parameters.Add(new("ctorParams", typeof(object[]),
                                array, InstructionObjectSource.Stack));

                            object newObj = constructorSchema.Construct(array);

                            ReportErrorOnNull(newObj, "Construction", targetTypeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                RegisterDisposable(newObj, targetTypeSchema, instance);

                                stack.Push(newObj);
                                entry.ReturnValues.Add(new(newObj));

                                ParameterListAllocator.Free(array);
                            }
                            break;
                        }
                    case OpCode.ConstructFromString:
                        {
                            int typeIndex = reader.ReadUInt16();
                            int stringIndex = reader.ReadUInt16();
                            
                            entry.Instruction.Operands.Add(typeIndex);
                            entry.Instruction.Operands.Add(stringIndex);
                            OnDecode(entry);
                            
                            TypeSchema typeSchema = importTables.TypeImports[typeIndex];
                            entry.Parameters.Add(new("type", typeof(TypeSchema),
                                typeSchema, InstructionObjectSource.TypeImports, typeIndex));

                            string fromString = (string)constantsTable.Get(stringIndex);
                            entry.Parameters.Add(new("fromString", typeof(string),
                                fromString, InstructionObjectSource.Constants, stringIndex));

                            typeSchema.TypeConverter(fromString, StringSchema.Type, out object newObj);

                            ReportErrorOnNull(newObj, "Construction", typeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                RegisterDisposable(newObj, typeSchema, instance);

                                stack.Push(newObj);
                                entry.ReturnValues.Add(new(newObj));
                            }
                            break;
                        }
                    case OpCode.ConstructFromBinary:
                        {
                            int typeIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(typeIndex);
                            OnDecode(entry);
                            
                            TypeSchema typeSchema = importTables.TypeImports[typeIndex];
                            entry.Parameters.Add(new("type", typeof(TypeSchema),
                                typeSchema, InstructionObjectSource.TypeImports, typeIndex));

                            uint blobStart = reader.CurrentOffset;
                            object newObj = typeSchema.DecodeBinary(reader);
                            if (debugging)
                            {
                                // Potentially expensive operation, only do this if a debugger is attached
                                uint blobEnd = reader.CurrentOffset;
                                uint blobLength = blobEnd - blobStart;

                                byte[] blob = new byte[blobLength];
                                reader.CurrentOffset = blobStart;
                                while (reader.CurrentOffset < blobEnd)
                                    blob[reader.CurrentOffset - blobStart] = reader.ReadByte();

                                entry.Parameters.Add(new("blob", typeof(byte[]),
                                    blob, InstructionObjectSource.Inline, 0));
                            }

                            ReportErrorOnNull(newObj, "Construction", typeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                RegisterDisposable(newObj, typeSchema, instance);

                                stack.Push(newObj);
                                entry.ReturnValues.Add(new(newObj));
                            }
                            break;
                        }
                    case OpCode.InitializeInstance:
                        {
                            int typeIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(typeIndex);
                            OnDecode(entry);
                            
                            TypeSchema typeSchema = importTables.TypeImports[typeIndex];
                            entry.Parameters.Add(new("type", typeof(TypeSchema),
                                typeSchema, InstructionObjectSource.TypeImports, typeIndex));

                            object objToInit = stack.Pop();
                            entry.Parameters.Add(new("objToInit", typeof(object),
                                objToInit, InstructionObjectSource.Stack));

                            typeSchema.InitializeInstance(ref objToInit);

                            ReportErrorOnNull(objToInit, "Initialize", typeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                stack.Push(objToInit);
                                entry.ReturnValues.Add(new(objToInit));
                            }
                            break;
                        }
                    case OpCode.InitializeInstanceIndirect:
                        {
                            OnDecode(entry);
                            
                            TypeSchema typeSchema = (TypeSchema)stack.Pop();
                            entry.Parameters.Add(new("type", typeof(TypeSchema),
                                typeSchema, InstructionObjectSource.Stack));

                            object objToInit = stack.Pop();
                            entry.Parameters.Add(new("objToInit", typeof(object),
                                objToInit, InstructionObjectSource.Stack));

                            typeSchema.InitializeInstance(ref objToInit);

                            ReportErrorOnNull(objToInit, "Initialize", typeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                stack.Push(objToInit);
                                entry.ReturnValues.Add(new(objToInit));
                            }
                            break;
                        }
                    case OpCode.LookupSymbol:
                        {
                            int symbolRefIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(symbolRefIndex);
                            OnDecode(entry);
                            
                            SymbolReference symbolRef = symbolReferenceTable[symbolRefIndex];
                            entry.Parameters.Add(new("symbolRef", typeof(SymbolReference),
                                symbolRef, InstructionObjectSource.SymbolReference, symbolRefIndex));

                            object symbol = context.ReadSymbol(symbolRef);

                            stack.Push(symbol);
                            entry.ReturnValues.Add(new(symbol));

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.WriteSymbol:
                    case OpCode.WriteSymbolPeek:
                        {
                            int symbolRefIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(symbolRefIndex);
                            OnDecode(entry);
                            
                            object symbol = (opCode == OpCode.WriteSymbolPeek) ? stack.Peek() : stack.Pop();
                            entry.Parameters.Add(new("symbol", typeof(object),
                                symbol, InstructionObjectSource.Stack));

                            SymbolReference symbolRef = symbolReferenceTable[symbolRefIndex];
                            entry.Parameters.Add(new("symbolRef", typeof(SymbolReference),
                                symbolRef, InstructionObjectSource.SymbolReference, symbolRefIndex));

                            context.WriteSymbol(symbolRef, symbol);

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.ClearSymbol:
                        {
                            int symbolRefIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(symbolRefIndex);
                            OnDecode(entry);
                            
                            SymbolReference symbolRef = symbolReferenceTable[symbolRefIndex];
                            entry.Parameters.Add(new("symbolRef", typeof(SymbolReference),
                                symbolRef, InstructionObjectSource.SymbolReference, symbolRefIndex));

                            context.ClearSymbol(symbolRef);

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.PropertyInitialize:
                    case OpCode.PropertyInitializeIndirect:
                        {
                            int propertyIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(propertyIndex);
                            OnDecode(entry);
                            
                            bool isIndirect = opCode == OpCode.PropertyInitializeIndirect;
                            TypeSchema parentTypeSchema = null;
                            if (isIndirect)
                            {
                                parentTypeSchema = (TypeSchema)stack.Pop();
                                entry.Parameters.Add(new("parentType", typeof(TypeSchema),
                                    parentTypeSchema, InstructionObjectSource.Stack));
                            }

                            PropertySchema propertySchema = importTables.PropertyImports[propertyIndex];
                            entry.Parameters.Add(new("property", typeof(PropertySchema),
                                propertySchema, InstructionObjectSource.PropertyImports, propertyIndex));

                            object propertyValue = stack.Pop();
                            entry.Parameters.Add(new("value", typeof(object),
                                propertyValue, InstructionObjectSource.Stack));

                            object parentObj = stack.Pop();
                            entry.Parameters.Add(new("parent", typeof(object),
                                parentObj, InstructionObjectSource.Stack));

                            ReportErrorOnNull(parentObj, "Property Set", propertySchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                if (isIndirect)
                                {
                                    PropertySchema inheritedPropertySchema = parentTypeSchema.FindPropertyDeep(propertySchema.Name);
                                    if (inheritedPropertySchema != propertySchema)
                                    {
                                        TypeSchema propertyType = inheritedPropertySchema.PropertyType;
                                        if (!propertyType.IsAssignableFrom(propertyValue))
                                        {
                                            string param = TypeSchema.NameFromInstance(propertyValue);
                                            ErrorManager.ReportError("Script runtime failure: Incompatible value for property '{0}' supplied (expecting values of type '{1}' but got '{2}') while constructing runtime replacement type '{3}' (original type '{4}')", propertySchema.Name, propertyType.Name, param, parentTypeSchema.Name, propertySchema.Owner.Name);
                                            result = ScriptError;
                                        }
                                        if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                        {
                                            break;
                                        }
                                    }
                                }

                                propertySchema.SetValue(ref parentObj, propertyValue);

                                if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                    stack.Push(parentObj);
                                    entry.ReturnValues.Add(new(parentObj));
                                }
                            }
                            break;
                        }
                    case OpCode.PropertyListAdd:
                        {
                            int propertyIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(propertyIndex);
                            OnDecode(entry);
                            
                            object objToAdd = stack.Pop();
                            entry.Parameters.Add(new("objToAdd", typeof(object),
                                objToAdd, InstructionObjectSource.Stack));

                            object collection = GetCollection(stack.Peek(), importTables, propertyIndex, out var propertySchema);
                            entry.Parameters.Add(new("collection", typeof(IList),
                                collection, InstructionObjectSource.Dynamic));
                            if (propertySchema != null)
                                entry.Parameters.Add(new("collectionProperty", typeof(PropertySchema),
                                    propertySchema, InstructionObjectSource.TypeImports, propertyIndex));

                            ReportErrorOnNull(collection, "List Add");
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                ((IList)collection).Add(objToAdd);
                                if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.PropertyDictionaryAdd:
                        {
                            int propertyIndex = reader.ReadUInt16();
                            int keyIndex = reader.ReadUInt16();
                            
                            entry.Instruction.Operands.Add(propertyIndex);
                            entry.Instruction.Operands.Add(keyIndex);
                            OnDecode(entry);

                            string key = (string)constantsTable.Get(keyIndex);
                            entry.Parameters.Add(new("key", typeof(string),
                                key, InstructionObjectSource.Constants, keyIndex));

                            object objToAdd = stack.Pop();
                            entry.Parameters.Add(new("objToAdd", typeof(object),
                                objToAdd, InstructionObjectSource.Stack));

                            object dictionary = GetCollection(stack.Peek(), importTables, propertyIndex, out var propertySchema);
                            entry.Parameters.Add(new("dictionary", typeof(IDictionary),
                                dictionary, InstructionObjectSource.Dynamic));
                            if (propertySchema != null)
                                entry.Parameters.Add(new("dictionaryProperty", typeof(PropertySchema),
                                    propertySchema, InstructionObjectSource.TypeImports, propertyIndex));

                            ReportErrorOnNull(dictionary, "Dictionary Add");
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                ((IDictionary)dictionary)[key] = objToAdd;
                                if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.PropertyAssign:
                    case OpCode.PropertyAssignStatic:
                        {
                            int propertyIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(propertyIndex);
                            OnDecode(entry);
                            
                            PropertySchema propertySchema = importTables.PropertyImports[propertyIndex];
                            entry.Parameters.Add(new("property", typeof(PropertySchema),
                                propertySchema, InstructionObjectSource.PropertyImports, propertyIndex));

                            object parentObj = null;

                            if (opCode == OpCode.PropertyAssign)
                            {
                                parentObj = stack.Pop();
                                entry.Parameters.Add(new("parent", typeof(object),
                                    parentObj, InstructionObjectSource.Stack));

                                ReportErrorOnNullOrDisposed(parentObj, "Property Set", propertySchema.Name, propertySchema.Owner);
                                if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                    break;
                                }
                            }

                            object propertyValue = stack.Peek();
                            entry.Parameters.Add(new("value", typeof(object),
                                propertyValue, InstructionObjectSource.Stack, 1));

                            propertySchema.SetValue(ref parentObj, propertyValue);

                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected) && Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.PropertyGet:
                    case OpCode.PropertyGetPeek:
                    case OpCode.PropertyGetStatic:
                        {
                            int propertyIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(propertyIndex);
                            OnDecode(entry);
                            
                            PropertySchema propertySchema = importTables.PropertyImports[propertyIndex];
                            entry.Parameters.Add(new("property", typeof(PropertySchema),
                                propertySchema, InstructionObjectSource.PropertyImports, propertyIndex));

                            object parentObj = null;

                            if (opCode != OpCode.PropertyGetStatic)
                            {
                                parentObj = opCode == OpCode.PropertyGet ? stack.Pop() : stack.Peek();
                                entry.Parameters.Add(new("parent", typeof(object),
                                    parentObj, InstructionObjectSource.Stack));

                                ReportErrorOnNullOrDisposed(parentObj, "Property Get", propertySchema.Name, propertySchema.Owner);
                                if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                    break;
                                }
                            }

                            object propertyValue = propertySchema.GetValue(parentObj);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                stack.Push(propertyValue);
                                entry.ReturnValues.Add(new(propertyValue));

                                if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.MethodInvoke:
                    case OpCode.MethodInvokePeek:
                    case OpCode.MethodInvokeStatic:
                    case OpCode.MethodInvokePushLastParam:
                    case OpCode.MethodInvokeStaticPushLastParam:
                        {
                            int methodIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(methodIndex);
                            OnDecode(entry);
                            
                            MethodSchema methodSchema = importTables.MethodImports[methodIndex];

                            int parameterCount = methodSchema.ParameterTypes.Length;
                            object[] parameters = ParameterListAllocator.Alloc(parameterCount);
                            for (parameterCount--; parameterCount >= 0; parameterCount--)
                                parameters[parameterCount] = stack.Pop();
                            entry.Parameters.Add(new("parameters", typeof(object[]),
                                parameters, InstructionObjectSource.Stack));

                            object parentObj = null;
                            bool isStatic = opCode == OpCode.MethodInvokeStatic || opCode == OpCode.MethodInvokeStaticPushLastParam;
                            bool peek = opCode == OpCode.MethodInvokePeek;
                            bool pushLastParam = opCode == OpCode.MethodInvokePushLastParam || opCode == OpCode.MethodInvokeStaticPushLastParam;

                            if (!isStatic)
                            {
                                parentObj = !peek ? stack.Pop() : stack.Peek();
                                entry.Parameters.Add(new("parent", typeof(object),
                                    parentObj, InstructionObjectSource.Stack));

                                ReportErrorOnNullOrDisposed(parentObj, "Method Invoke", methodSchema.Name, methodSchema.Owner);
                                if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                    break;
                                }
                            }

                            object methodResult = methodSchema.Invoke(parentObj, parameters);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                if (!pushLastParam)
                                {
                                    if (methodSchema.ReturnType != VoidSchema.Type)
                                    {
                                        stack.Push(methodResult);
                                        entry.ReturnValues.Add(new(methodResult));
                                    }
                                }
                                else
                                {
                                    stack.Push(parameters[parameters.Length - 1]);
                                }

                                ParameterListAllocator.Free(parameters);
                            }
                            break;
                        }
                    case OpCode.VerifyTypeCast:
                        {
                            int typeIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(typeIndex);
                            OnDecode(entry);
                            
                            TypeSchema typeSchema = importTables.TypeImports[typeIndex];
                            entry.Parameters.Add(new("type", typeof(TypeSchema),
                                typeSchema, InstructionObjectSource.TypeImports, typeIndex));

                            object objToCheck = stack.Peek();
                            entry.Parameters.Add(new("objToCheck", typeof(object),
                                objToCheck, InstructionObjectSource.Stack, 1));

                            if (objToCheck != null)
                            {
                                if (!typeSchema.IsAssignableFrom(objToCheck))
                                {
                                    string runtimeTypeName = TypeSchema.NameFromInstance(objToCheck);
                                    string name = typeSchema.Name;
                                    ErrorManager.ReportError("Script runtime failure: Invalid type cast while attempting to cast an instance with a runtime type of '{0}' to '{1}'", runtimeTypeName, name);
                                    result = ScriptError;
                                }

                                if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                }
                            }
                            else if (!typeSchema.IsNullAssignable)
                            {
                                ReportErrorOnNull(objToCheck, "Verify Type Cast", typeSchema.Name);
                                if (ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.ConvertType:
                        {
                            int toTypeIndex = reader.ReadUInt16();
                            int fromTypeIndex = reader.ReadUInt16();
                            
                            entry.Instruction.Operands.Add(toTypeIndex);
                            entry.Instruction.Operands.Add(fromTypeIndex);
                            OnDecode(entry);
                            
                            TypeSchema toTypeSchema = importTables.TypeImports[toTypeIndex];
                            entry.Parameters.Add(new("toType", typeof(TypeSchema),
                                toTypeSchema, InstructionObjectSource.TypeImports, toTypeIndex));

                            TypeSchema fromTypeSchema = importTables.TypeImports[fromTypeIndex];
                            entry.Parameters.Add(new("fromType", typeof(TypeSchema),
                                fromTypeSchema, InstructionObjectSource.TypeImports, fromTypeIndex));

                            object objToConvert = stack.Pop();
                            entry.Parameters.Add(new("objToConvert", typeof(object),
                                objToConvert, InstructionObjectSource.Stack));

                            ReportErrorOnNull(objToConvert, "Type Conversion", toTypeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                Result castResult = toTypeSchema.TypeConverter(objToConvert, fromTypeSchema, out object obj14);
                                if (castResult.Failed)
                                    ErrorManager.ReportError("Script runtime failure: Type conversion failed while attempting to convert to '{0}' ({1})", toTypeSchema.Name, castResult.Error);

                                if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                                {
                                    stack.Push(obj14);
                                    entry.ReturnValues.Add(new(obj14));
                                }
                            }
                            break;
                        }
                    case OpCode.Operation:
                        {
                            int opHostIndex = reader.ReadUInt16();
                            OperationType op = (OperationType)reader.ReadByte();
                            
                            entry.Instruction.Operands.Add(opHostIndex);
                            entry.Instruction.Operands.Add(op);
                            OnDecode(entry);
                            
                            TypeSchema opHost = importTables.TypeImports[opHostIndex];
                            entry.Parameters.Add(new("opHost", typeof(TypeSchema),
                                opHost, InstructionObjectSource.TypeImports, opHostIndex));

                            entry.Parameters.Add(new("op", typeof(OperationType),
                                op, InstructionObjectSource.Inline));

                            object right = null;
                            if (!TypeSchema.IsUnaryOperation(op))
                            {
                                right = stack.Pop();
                                entry.Parameters.Add(new("right", typeof(object),
                                    right, InstructionObjectSource.Stack));
                            }

                            object left = stack.Pop();
                            entry.Parameters.Add(new("left", typeof(object),
                                left, InstructionObjectSource.Stack));

                            object opResult = opHost.PerformOperationDeep(left, right, op);

                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                stack.Push(opResult);
                                entry.ReturnValues.Add(new(opResult));

                                if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.IsCheck:
                        {
                            int targetTypeIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(targetTypeIndex);
                            OnDecode(entry);
                            
                            TypeSchema targetTypeSchema = importTables.TypeImports[targetTypeIndex];
                            entry.Parameters.Add(new("targetType", typeof(TypeSchema),
                                targetTypeSchema, InstructionObjectSource.TypeImports, targetTypeIndex));

                            object objToCheck = stack.Pop();
                            entry.Parameters.Add(new("objToCheck", typeof(object),
                                objToCheck, InstructionObjectSource.Stack));

                            bool checkResult = false;
                            if (objToCheck != null)
                                checkResult = targetTypeSchema.IsAssignableFrom(objToCheck);

                            stack.Push(BooleanBoxes.Box(checkResult));
                            entry.ReturnValues.Add(new(checkResult));

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.As:
                        {
                            int targetTypeIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(targetTypeIndex);
                            OnDecode(entry);
                            
                            TypeSchema targetTypeSchema = importTables.TypeImports[targetTypeIndex];
                            entry.Parameters.Add(new("targetType", typeof(TypeSchema),
                                targetTypeSchema, InstructionObjectSource.TypeImports, targetTypeIndex));

                            object objToCheck = stack.Peek();
                            entry.Parameters.Add(new("objToCheck", typeof(object),
                                objToCheck, InstructionObjectSource.Stack, 1));

                            if (objToCheck != null && !targetTypeSchema.IsAssignableFrom(objToCheck))
                            {
                                stack.Pop();
                                stack.Push(null);
                            }
                            else if (debugging)
                            {
                                // The opcode isn't implemented like this,
                                // but technically As returns a value.
                                entry.ReturnValues.Add(new(objToCheck));
                            }

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.TypeOf:
                        {
                            int typeIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(typeIndex);
                            OnDecode(entry);
                            
                            entry.Parameters.Add(new("typeIndex", typeof(ushort),
                                typeIndex, InstructionObjectSource.Inline));

                            TypeSchema typeSchema = importTables.TypeImports[typeIndex];

                            stack.Push(typeSchema);
                            entry.ReturnValues.Add(new(typeSchema));
                            break;
                        }
                    case OpCode.PushNull:
                        OnDecode(entry);
                        stack.Push(null);
                        entry.ReturnValues.Add(new(null));
                        break;
                    case OpCode.PushConstant:
                        {
                            int constantIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(constantIndex);
                            OnDecode(entry);
                            
                            entry.Parameters.Add(new("constantIndex", typeof(ushort),
                                constantIndex, InstructionObjectSource.Inline));

                            object constant = constantsTable.Get(constantIndex);

                            stack.Push(constant);
                            entry.ReturnValues.Add(new(constant));
                            break;
                        }
                    case OpCode.PushThis:
                        OnDecode(entry);
                        stack.Push(instance);
                        entry.ReturnValues.Add(new(instance));
                        break;
                    case OpCode.DiscardValue:
                        OnDecode(entry);
                        stack.Pop();
                        break;
                    case OpCode.ReturnValue:
                        {
                            OnDecode(entry);
                            
                            object thisResult = stack.Pop();
                            entry.ReturnValues.Add(new(thisResult));

                            result = thisResult;
                            errorsDetected = true;
                            break;
                        }
                    case OpCode.ReturnVoid:
                        OnDecode(entry);
                        result = VoidReturnValue;
                        errorsDetected = true;
                        break;
                    case OpCode.JumpIfFalse:
                    case OpCode.JumpIfFalsePeek:
                    case OpCode.JumpIfTruePeek:
                        {
                            uint jumpTo = reader.ReadUInt32();
                            entry.Instruction.Operands.Add(jumpTo);
                            OnDecode(entry);
                            
                            entry.Parameters.Add(new("jumpTo", typeof(uint),
                                jumpTo, InstructionObjectSource.Inline));

                            bool value = (bool)((opCode == OpCode.JumpIfFalse) ? stack.Pop() : stack.Peek());
                            entry.Parameters.Add(new("value", typeof(bool),
                                value, InstructionObjectSource.Stack));

                            bool jumpIfTrue = opCode == OpCode.JumpIfTruePeek;
                            if (jumpIfTrue == value)
                            {
                                reader.CurrentOffset = jumpTo;

                                if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.JumpIfDictionaryContains:
                        {
                            ushort propertyIndex = reader.ReadUInt16();
                            ushort keyIndex = reader.ReadUInt16();
                            uint jumpTo = reader.ReadUInt32();
                            
                            entry.Instruction.Operands.Add(propertyIndex);
                            entry.Instruction.Operands.Add(keyIndex);
                            entry.Instruction.Operands.Add(jumpTo);
                            OnDecode(entry);

                            entry.Parameters.Add(new("jumpTo", typeof(uint),
                                jumpTo, InstructionObjectSource.Inline));

                            string key = (string)constantsTable.Get(keyIndex);
                            entry.Parameters.Add(new("key", typeof(string),
                                key, InstructionObjectSource.Constants, keyIndex));

                            object dictionary = GetCollection(stack.Peek(), importTables, propertyIndex, out var propertySchema);
                            entry.Parameters.Add(new("dictionary", typeof(IDictionary),
                                dictionary, InstructionObjectSource.Dynamic));
                            if (propertySchema != null)
                                entry.Parameters.Add(new("dictionaryProperty", typeof(PropertySchema),
                                    propertySchema, InstructionObjectSource.TypeImports, propertyIndex));

                            ReportErrorOnNull(dictionary, "Dictionary Contains");
                            if (!ErrorsDetected(watermark, ref result, ref errorsDetected))
                            {
                                bool dictionaryContains = ((IDictionary)dictionary).Contains(key);

                                Trace.IsCategoryEnabled(TraceCategory.Markup);
                                if (dictionaryContains)
                                {
                                    reader.CurrentOffset = jumpTo;

                                    if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                    {
                                    }
                                }
                            }
                            break;
                        }
                    case OpCode.JumpIfNullPeek:
                        {
                            uint jumpTo = reader.ReadUInt32();
                            entry.Instruction.Operands.Add(jumpTo);
                            OnDecode(entry);
                            
                            entry.Parameters.Add(new("jumpTo", typeof(uint),
                                jumpTo, InstructionObjectSource.Inline));

                            object objToCheck = stack.Peek();
                            entry.Parameters.Add(new("objToCheck", typeof(object),
                                objToCheck, InstructionObjectSource.Stack));

                            Trace.IsCategoryEnabled(TraceCategory.Markup);
                            if (objToCheck == null)
                            {
                                reader.CurrentOffset = jumpTo;

                                if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.Jump:
                        {
                            uint jumpTo = reader.ReadUInt32();
                            entry.Instruction.Operands.Add(jumpTo);
                            OnDecode(entry);
                            
                            entry.Parameters.Add(new("jumpTo", typeof(uint),
                                jumpTo, InstructionObjectSource.Inline));

                            reader.CurrentOffset = jumpTo;

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.ConstructListenerStorage:
                        {
                            int listenerCount = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(listenerCount);
                            OnDecode(entry);
                            
                            entry.Parameters.Add(new("listenerCount", typeof(int),
                                listenerCount, InstructionObjectSource.Inline));

                            if (instance.Listeners == null)
                            {
                                MarkupListeners markupListeners = new(listenerCount);
                                markupListeners.DeclareOwner(instance);
                                instance.Listeners = markupListeners;
                            }
                            else
                            {
                                instance.Listeners.AddEntries(listenerCount);
                            }

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.Listen:
                    case OpCode.DestructiveListen:
                        {
                            int listenerIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(listenerIndex);
                            entry.Parameters.Add(new("listenerIndex", typeof(int),
                                listenerIndex, InstructionObjectSource.Inline));

                            ListenerType listenerType = (ListenerType)reader.ReadByte();
                            entry.Instruction.Operands.Add(listenerType);
                            entry.Parameters.Add(new("listenerType", typeof(ListenerType),
                                listenerType, InstructionObjectSource.Inline));

                            int watchIndex = reader.ReadUInt16();
                            entry.Instruction.Operands.Add(watchIndex);

                            uint handlerOffset = reader.ReadUInt32();
                            entry.Instruction.Operands.Add(handlerOffset);
                            entry.Parameters.Add(new("handlerOffset", typeof(uint),
                                handlerOffset, InstructionObjectSource.Inline));

                            uint refreshOffset = uint.MaxValue;
                            if (opCode == OpCode.DestructiveListen)
                            {
                                refreshOffset = reader.ReadUInt32();
                                entry.Instruction.Operands.Add(refreshOffset);
                                entry.Parameters.Add(new("refreshOffset", typeof(uint),
                                    refreshOffset, InstructionObjectSource.Inline));
                            }
                            
                            OnDecode(entry);
                            
                            string watch = null;
                            InstructionObjectSource watchSource = InstructionObjectSource.Dynamic;
                            switch (listenerType)
                            {
                                case ListenerType.Property:
                                    watch = importTables.PropertyImports[watchIndex].Name;
                                    watchSource = InstructionObjectSource.PropertyImports;
                                    break;
                                case ListenerType.Event:
                                    watch = importTables.EventImports[watchIndex].Name;
                                    watchSource = InstructionObjectSource.EventImports;
                                    break;
                                case ListenerType.Symbol:
                                    watch = symbolReferenceTable[watchIndex].Symbol;
                                    watchSource = InstructionObjectSource.SymbolReference;
                                    break;
                            }
                            entry.Parameters.Add(new("watch", typeof(string),
                                watch, watchSource, watchIndex));

                            object handlerObj = stack.Peek();
                            entry.Parameters.Add(new("handlerObj", typeof(object),
                                handlerObj, InstructionObjectSource.Stack, 1));

                            if (handlerObj is INotifyObject notifier)
                            {
                                MarkupListeners listeners = instance.Listeners;
                                listeners.RefreshListener(listenerIndex, notifier, watch, instance, handlerOffset, refreshOffset);
                            }
                            else
                            {
                                Trace.IsCategoryEnabled(TraceCategory.Markup);
                            }
                            break;
                        }
                    case OpCode.EnterDebugState:
                        {
                            int breakpointIndex = reader.ReadInt32();
                            entry.Instruction.Operands.Add(breakpointIndex);
                            OnDecode(entry);
                            
                            entry.Parameters.Add(new("breakpointIndex", typeof(int),
                                breakpointIndex, InstructionObjectSource.Inline));

                            if (MarkupSystem.IsDebuggingEnabled(2))
                            {
                                wasInDebugState = MarkupDebugHelper.EnterDebugState(wasInDebugState, loadResult, breakpointIndex, instance.Storage);
                            }
                            break;
                        }
                }

                Application.Debugger?.LogInterpreterExecute(context, entry);
            }
            while (stack.Count > count)
            {
                stack.Pop();
            }
            return result;
        }

        // Token: 0x06000F23 RID: 3875 RVA: 0x0002ADA7 File Offset: 0x00029DA7
        private static bool ErrorsDetected(ErrorWatermark watermark, ref object result, ref bool done)
        {
            if (watermark.ErrorsDetected)
            {
                result = ScriptError;
                done = true;
                return true;
            }
            return false;
        }

        // Token: 0x06000F24 RID: 3876 RVA: 0x0002ADC0 File Offset: 0x00029DC0
        private static void RegisterDisposable(object instance, TypeSchema type, IMarkupTypeBase root)
        {
            if (type.Disposable && root != null)
            {
                IDisposableObject disposableObject = (IDisposableObject)instance;
                root.RegisterDisposable(disposableObject);
                disposableObject.DeclareOwner(root);
            }
        }

        // Token: 0x06000F25 RID: 3877 RVA: 0x0002ADF0 File Offset: 0x00029DF0
        private static object GetCollection(object stackInstance, MarkupImportTables importTables, int propertyIndex, out PropertySchema propertySchema)
        {
            object result = null;
            if (propertyIndex == 0xFFFF)
            {
                result = stackInstance;
                propertySchema = null;
            }
            else
            {
                propertySchema = importTables.PropertyImports[propertyIndex];
                ReportErrorOnNull(stackInstance, "Property Get", propertySchema.Name);
                if (stackInstance != null)
                    result = propertySchema.GetValue(stackInstance);
            }
            return result;
        }

        // Token: 0x06000F26 RID: 3878 RVA: 0x0002AE31 File Offset: 0x00029E31
        private static void ReportErrorOnNull(object instance, string operation, string member)
        {
            if (instance == null)
            {
                ErrorManager.ReportError("Script runtime failure: Null-reference while attempting a '{0}' of '{1}' on a null instance", operation, member);
            }
        }

        // Token: 0x06000F27 RID: 3879 RVA: 0x0002AE42 File Offset: 0x00029E42
        private static void ReportErrorOnNull(object instance, string operation)
        {
            if (instance == null)
            {
                ErrorManager.ReportError("Script runtime failure: Null-reference while attempting a '{0}'", operation);
            }
        }

        // Token: 0x06000F28 RID: 3880 RVA: 0x0002AE54 File Offset: 0x00029E54
        private static void ReportErrorOnNullOrDisposed(object instance, string operation, string member, TypeSchema typeSchema)
        {
            if (instance == null)
            {
                ErrorManager.ReportError("Script runtime failure: Null-reference while attempting a '{0}' of '{1}' on a null instance", operation, member);
                return;
            }
            if (typeSchema.Disposable)
            {
                IDisposableObject disposableObject = (IDisposableObject)instance;
                if (disposableObject.IsDisposed)
                {
                    ErrorManager.ReportError("Script runtime failure: Attempting a '{0}' of '{1}' on an object '{2}' that has already been disposed", operation, member, TypeSchema.NameFromInstance(instance));
                }
            }
        }

        // Token: 0x04000947 RID: 2375
        public const uint InvalidOffset = 0xFFFFFFFFU;

        // Token: 0x04000948 RID: 2376
        private static object VoidReturnValue = new object();

        // Token: 0x04000949 RID: 2377
        private static Stack _stack = new Stack();

        // Token: 0x0400094A RID: 2378
        public static object ScriptError = new Interpreter.ScriptErrorObject();

        // Token: 0x0400094B RID: 2379
        public static string ExceptionContext;

        // Token: 0x0200018C RID: 396
        private class ScriptErrorObject
        {
        }

        // Token: 0x0200018D RID: 397
        private struct ParameterListAllocator
        {
            // Token: 0x06000F2C RID: 3884 RVA: 0x0002AECC File Offset: 0x00029ECC
            public static object[] Alloc(int count)
            {
                object[] array;
                if (count == 0)
                {
                    array = s_params0;
                }
                else if (count < MAX_CACHED_LIST_SIZE)
                {
                    array = s_cachedLists[count];
                    if (array != null)
                    {
                        s_cachedLists[count] = null;
                    }
                    else
                    {
                        array = new object[count];
                    }
                }
                else
                {
                    array = new object[count];
                }
                return array;
            }

            // Token: 0x06000F2D RID: 3885 RVA: 0x0002AF10 File Offset: 0x00029F10
            public static void Free(object[] paramList)
            {
                int num = paramList.Length;
                if (num != 0 && num < MAX_CACHED_LIST_SIZE && s_cachedLists[num] == null)
                {
                    Array.Clear(paramList, 0, paramList.Length);
                    s_cachedLists[num] = paramList;
                }
            }

            // Token: 0x0400094C RID: 2380
            private const int MAX_CACHED_LIST_SIZE = 20;

            // Token: 0x0400094D RID: 2381
            private static object[] s_params0 = new object[0];

            // Token: 0x0400094E RID: 2382
            private static object[][] s_cachedLists = new object[MAX_CACHED_LIST_SIZE][];
        }
    }
}
