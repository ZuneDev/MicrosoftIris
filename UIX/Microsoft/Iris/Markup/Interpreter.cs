using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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
                if (Application.DebugSettings.UseDecompiler)
                    result = RunDecompile(context, byteCodeReader);
                else
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

            while (!errorsDetected)
            {
                OpCode opCode = (OpCode)reader.ReadByte();
                InterpreterEntry entry = new(new(opCode, reader.CurrentOffset, loadResult.Uri));

                if (debugging)
                {
                    // Fetch line and column numbers from the table
                    if (context.LoadResult.LineNumberTable.TryLookup(reader.CurrentOffset, out int line, out int column))
                    {
                        bool ShouldBreak(Breakpoint b)
                            => b.Enabled && b.Equals(loadResult.Uri, line, column, reader.CurrentOffset);

                        // Check if a breakpoint has been set at this location
                        bool shouldBreakHere = Application.DebugSettings.Breakpoints.Any(ShouldBreak);
                        if (shouldBreakHere)
                            Application.Debugger.DebuggerCommand = InterpreterCommand.Break;
                    }

                    // Stop exeuction while the debugger is in break mode
                    while (Application.Debugger.DebuggerCommand == InterpreterCommand.Break) ;

                    // If the debugger requested a single step, immediately set the debugger
                    // to break again for the next instruction
                    if (Application.Debugger.DebuggerCommand == InterpreterCommand.Step)
                        Application.Debugger.DebuggerCommand = InterpreterCommand.Break;
                }

                switch (opCode)
                {
                    case OpCode.ConstructObject:
                        {
                            int typeIndex = reader.ReadUInt16();
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
                            TypeSchema targetTypeSchema = importTables.TypeImports[targetTypeIndex];

                            int constructorIndex = reader.ReadUInt16();
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
                            TypeSchema typeSchema = importTables.TypeImports[typeIndex];
                            entry.Parameters.Add(new("type", typeof(TypeSchema),
                                typeSchema, InstructionObjectSource.TypeImports, typeIndex));

                            int stringIndex = reader.ReadUInt16();
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
                            object symbol = (opCode == OpCode.WriteSymbolPeek) ? stack.Peek() : stack.Pop();
                            entry.Parameters.Add(new("symbol", typeof(object),
                                symbol, InstructionObjectSource.Stack));

                            int symbolRefIndex = reader.ReadUInt16();
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
                            bool isIndirect = opCode == OpCode.PropertyInitializeIndirect;
                            TypeSchema parentTypeSchema = null;
                            if (isIndirect)
                            {
                                parentTypeSchema = (TypeSchema)stack.Pop();
                                entry.Parameters.Add(new("parentType", typeof(TypeSchema),
                                    parentTypeSchema, InstructionObjectSource.Stack));
                            }

                            int propertyIndex = reader.ReadUInt16();
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
                            object objToAdd = stack.Pop();
                            entry.Parameters.Add(new("objToAdd", typeof(object),
                                objToAdd, InstructionObjectSource.Stack));

                            int propertyIndex = reader.ReadUInt16();
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
                            TypeSchema toTypeSchema = importTables.TypeImports[toTypeIndex];
                            entry.Parameters.Add(new("toType", typeof(TypeSchema),
                                toTypeSchema, InstructionObjectSource.TypeImports, toTypeIndex));

                            int fromTypeIndex = reader.ReadUInt16();
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
                            TypeSchema opHost = importTables.TypeImports[opHostIndex];
                            entry.Parameters.Add(new("opHost", typeof(TypeSchema),
                                opHost, InstructionObjectSource.TypeImports, opHostIndex));

                            OperationType op = (OperationType)reader.ReadByte();
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
                            entry.Parameters.Add(new("typeIndex", typeof(ushort),
                                typeIndex, InstructionObjectSource.Inline));

                            TypeSchema typeSchema = importTables.TypeImports[typeIndex];

                            stack.Push(typeSchema);
                            entry.ReturnValues.Add(new(typeSchema));
                            break;
                        }
                    case OpCode.PushNull:
                        stack.Push(null);
                        entry.ReturnValues.Add(new(null));
                        break;
                    case OpCode.PushConstant:
                        {
                            int constantIndex = reader.ReadUInt16();
                            entry.Parameters.Add(new("constantIndex", typeof(ushort),
                                constantIndex, InstructionObjectSource.Inline));

                            object constant = constantsTable.Get(constantIndex);

                            stack.Push(constant);
                            entry.ReturnValues.Add(new(constant));
                            break;
                        }
                    case OpCode.PushThis:
                        stack.Push(instance);
                        entry.ReturnValues.Add(new(instance));
                        break;
                    case OpCode.DiscardValue:
                        stack.Pop();
                        break;
                    case OpCode.ReturnValue:
                        {
                            object thisResult = stack.Pop();
                            entry.ReturnValues.Add(new(thisResult));

                            result = thisResult;
                            errorsDetected = true;
                            break;
                        }
                    case OpCode.ReturnVoid:
                        result = VoidReturnValue;
                        errorsDetected = true;
                        break;
                    case OpCode.JumpIfFalse:
                    case OpCode.JumpIfFalsePeek:
                    case OpCode.JumpIfTruePeek:
                        {
                            uint jumpTo = reader.ReadUInt32();
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
                            entry.Parameters.Add(new("listenerIndex", typeof(int),
                                listenerIndex, InstructionObjectSource.Inline));

                            ListenerType listenerType = (ListenerType)reader.ReadByte();
                            entry.Parameters.Add(new("listenerType", typeof(ListenerType),
                                listenerType, InstructionObjectSource.Inline));

                            int watchIndex = reader.ReadUInt16();

                            uint handlerOffset = reader.ReadUInt32();
                            entry.Parameters.Add(new("handlerOffset", typeof(uint),
                                handlerOffset, InstructionObjectSource.Inline));

                            uint refreshOffset = uint.MaxValue;
                            if (opCode == OpCode.DestructiveListen)
                            {
                                refreshOffset = reader.ReadUInt32();
                                entry.Parameters.Add(new("refreshOffset", typeof(uint),
                                    refreshOffset, InstructionObjectSource.Inline));
                            }
                            
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
                            entry.Parameters.Add(new("breakpointIndex", typeof(int),
                                breakpointIndex, InstructionObjectSource.Inline));

                            if (MarkupSystem.IsDebuggingEnabled(2))
                            {
                                wasInDebugState = MarkupDebugHelper.EnterDebugState(wasInDebugState, loadResult, breakpointIndex, instance.Storage);
                            }
                            break;
                        }
                }

                Application.Debugger?.LogInterpreterOpCode(context, entry);
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
        public const uint InvalidOffset = 4294967295U;

        // Token: 0x04000948 RID: 2376
        private static object VoidReturnValue = new object();

        // Token: 0x04000949 RID: 2377
        private static Stack _stack = new Stack();
        private static Stack<XmlNode> _xmlStack = new Stack<XmlNode>();

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

        private static XmlElement UIXElem;

        private static XmlDocument _xmlDoc;
        public static XmlDocument XmlDoc
        {
            get
            {
                if (_xmlDoc == null)
                {
                    _xmlDoc = new XmlDocument();
                    UIXElem = _xmlDoc.CreateElement("UIX");
                    UIXElem.SetAttribute("xmlns", "http://schemas.microsoft.com/2007/uix");
                    _xmlDoc.AppendChild(UIXElem);
                }
                return _xmlDoc;
            }
        }

        /// <summary>
        /// Attempts to generate source UIX from a compiled result
        /// </summary>
        private static object RunDecompile(InterpreterContext context, ByteCodeReader reader)
        {
            MarkupLoadResult loadResult = context.LoadResult;
            IMarkupTypeBase instance = context.Instance;
            MarkupImportTables importTables = loadResult.ImportTables;
            MarkupConstantsTable constantsTable = loadResult.ConstantsTable;
            SymbolReference[] symbolReferenceTable = context.MarkupType.SymbolReferenceTable;

            Trace.IsCategoryEnabled(TraceCategory.Markup);
            bool decompile = Application.DebugSettings.UseDecompiler;

            Stack stack = _stack;
            Stack<XmlNode> xmlStack = _xmlStack;
            int count = stack.Count;

            if (instance != null)
            {
                stack.Push(instance);

                // Make sure XmlDoc has been initialized
                _ = XmlDoc;

                // Add UI container to root
                string key = instance.TypeSchema.Name;
                XmlElement instanceXml = UIXElem.ChildNodes.OfType<XmlElement>()
                    .FirstOrDefault(elem => elem?.GetAttribute("Name") == key);
                if (instanceXml == null)
                {
                    instanceXml = XmlDoc.CreateElement("UI");
                    instanceXml.SetAttribute("Name", key);
                    UIXElem.AppendChild(instanceXml);
                }
                xmlStack.Push(instanceXml);
            }
            ErrorWatermark watermark = ErrorManager.Watermark;
            bool flag = false;
            object result = null;
            bool wasInDebugState = false;
            while (!flag)
            {
                OpCode opCode = (OpCode)reader.ReadByte();

                switch (opCode)
                {
                    case OpCode.ConstructObject:
                        {
                            int num = reader.ReadUInt16();
                            TypeSchema typeSchema = importTables.TypeImports[num];
                            object obj = typeSchema.ConstructDefault();

                            string typeName = obj.GetType().Name;
                            int idxTilde = typeName.IndexOf('`');
                            if (idxTilde >= 0)
                                typeName = typeName.Substring(0, idxTilde);
                            var objXml = XmlDoc.CreateElement(typeName);

                            ReportErrorOnNull(obj, "Construction", typeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj, typeSchema, instance);
                                stack.Push(obj);
                                xmlStack.Push(objXml);
                            }
                            break;
                        }
                    case OpCode.ConstructObjectIndirect:
                        {
                            int num2 = reader.ReadUInt16();
                            TypeSchema typeSchema2 = importTables.TypeImports[num2];
                            TypeSchema typeSchema3 = (TypeSchema)stack.Pop();
                            xmlStack.Pop();
                            if (!typeSchema2.IsAssignableFrom(typeSchema3))
                            {
                                ErrorManager.ReportError("Script runtime failure: Dynamic construction type override failed. Attempting to construct '{0}' in place of '{1}'", (typeSchema3 != null) ? typeSchema3.Name : "null", typeSchema2.Name);
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                    break;
                                }
                            }
                            IDynamicConstructionSchema dynamicConstructionSchema = typeSchema3 as IDynamicConstructionSchema;
                            object obj2;
                            if (dynamicConstructionSchema != null)
                            {
                                obj2 = dynamicConstructionSchema.ConstructDefault(typeSchema2);
                            }
                            else
                            {
                                obj2 = typeSchema3.ConstructDefault();
                            }
                            var objXml2 = XmlDoc.CreateElement(obj2.GetType().Name);
                            ReportErrorOnNull(obj2, "Construction", typeSchema3.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj2, typeSchema3, instance);
                                stack.Push(obj2);
                                xmlStack.Push(objXml2);
                            }
                            break;
                        }
                    case OpCode.ConstructObjectParam:
                        {
                            int num3 = reader.ReadUInt16();
                            TypeSchema typeSchema4 = importTables.TypeImports[num3];
                            int num4 = reader.ReadUInt16();
                            ConstructorSchema constructorSchema = importTables.ConstructorImports[num4];
                            int i = constructorSchema.ParameterTypes.Length;
                            object[] array = ParameterListAllocator.Alloc(i);
                            for (i--; i >= 0; i--)
                            {
                                array[i] = stack.Pop();
                                xmlStack.Pop(); // TODO
                            }
                            object obj3 = constructorSchema.Construct(array);
                            var objXml3 = XmlDoc.CreateElement(obj3.GetType().Name);
                            ReportErrorOnNull(obj3, "Construction", typeSchema4.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj3, typeSchema4, instance);
                                stack.Push(obj3);
                                xmlStack.Push(objXml3);
                                ParameterListAllocator.Free(array);
                            }
                            break;
                        }
                    case OpCode.ConstructFromString:
                        {
                            int num5 = reader.ReadUInt16();
                            TypeSchema typeSchema5 = importTables.TypeImports[num5];
                            int index = reader.ReadUInt16();
                            string from = (string)constantsTable.Get(index);
                            object obj4;
                            typeSchema5.TypeConverter(from, StringSchema.Type, out obj4);
                            var objXml4 = XmlDoc.CreateElement(obj4.GetType().Name);
                            ReportErrorOnNull(obj4, "Construction", typeSchema5.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj4, typeSchema5, instance);
                                stack.Push(obj4);
                                xmlStack.Push(objXml4);
                            }
                            break;
                        }
                    case OpCode.ConstructFromBinary:
                        {
                            int num6 = reader.ReadUInt16();
                            TypeSchema typeSchema6 = importTables.TypeImports[num6];
                            object obj5 = typeSchema6.DecodeBinary(reader);
                            var objXml5 = XmlDoc.CreateElement(obj5.GetType().Name);
                            ReportErrorOnNull(obj5, "Construction", typeSchema6.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj5, typeSchema6, instance);
                                stack.Push(obj5);
                                xmlStack.Push(objXml5);
                            }
                            break;
                        }
                    case OpCode.InitializeInstance:
                        {
                            int num7 = reader.ReadUInt16();
                            TypeSchema typeSchema7 = importTables.TypeImports[num7];
                            object obj6 = stack.Pop();
                            var objXml6 = (XmlElement)xmlStack.Pop();
                            typeSchema7.InitializeInstance(ref obj6);
                            objXml6.SetAttribute("Name", typeSchema7.Name);
                            ReportErrorOnNull(obj6, "Initialize", typeSchema7.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                stack.Push(obj6);
                                xmlStack.Push(objXml6);
                            }
                            break;
                        }
                    case OpCode.InitializeInstanceIndirect:
                        {
                            TypeSchema typeSchema8 = (TypeSchema)stack.Pop();
                            object obj7 = stack.Pop();
                            typeSchema8.InitializeInstance(ref obj7);
                            xmlStack.Pop();
                            var objXml7 = (XmlElement)xmlStack.Pop();
                            objXml7.SetAttribute("Name", typeSchema8.Name);
                            ReportErrorOnNull(obj7, "Initialize", typeSchema8.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                stack.Push(obj7);
                                xmlStack.Push(objXml7);
                            }
                            break;
                        }
                    case OpCode.LookupSymbol:
                        {
                            int num8 = reader.ReadUInt16();
                            SymbolReference symbolRef = symbolReferenceTable[num8];

                            object obj8 = context.ReadSymbol(symbolRef);
                            var objXml8 = GenerateXmlRepresentation(obj8);

                            stack.Push(obj8);
                            xmlStack.Push(objXml8);

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.WriteSymbol:
                    case OpCode.WriteSymbolPeek:
                        {
                            object value = (opCode == OpCode.WriteSymbolPeek) ? stack.Peek() : stack.Pop();
                            if (opCode == OpCode.WriteSymbol) xmlStack.Pop();
                            int num9 = reader.ReadUInt16();
                            SymbolReference symbolRef2 = symbolReferenceTable[num9];

                            context.WriteSymbol(symbolRef2, value);

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.ClearSymbol:
                        {
                            int num10 = reader.ReadUInt16();
                            SymbolReference symbolRef3 = symbolReferenceTable[num10];
                            context.ClearSymbol(symbolRef3);
                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.PropertyInitialize:
                    case OpCode.PropertyInitializeIndirect:
                        {
                            bool isPropInitIndirect = opCode == OpCode.PropertyInitializeIndirect;
                            TypeSchema typeSchema9 = null;
                            if (isPropInitIndirect)
                            {
                                typeSchema9 = (TypeSchema)stack.Pop();
                                xmlStack.Pop();
                            }
                            int num11 = reader.ReadUInt16();
                            PropertySchema propertySchema = importTables.PropertyImports[num11];
                            object propValue = stack.Pop();
                            object destObj = stack.Pop();
                            XmlNode propValueXml = xmlStack.Pop();
                            var destObjXml = (XmlElement)xmlStack.Pop();
                            ReportErrorOnNull(destObj, "Property Set", propertySchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                if (isPropInitIndirect)
                                {
                                    PropertySchema propertySchema2 = typeSchema9.FindPropertyDeep(propertySchema.Name);
                                    if (propertySchema2 != propertySchema)
                                    {
                                        TypeSchema propertyType = propertySchema2.PropertyType;
                                        if (!propertyType.IsAssignableFrom(propValue))
                                        {
                                            string param = TypeSchema.NameFromInstance(propValue);
                                            ErrorManager.ReportError("Script runtime failure: Incompatible value for property '{0}' supplied (expecting values of type '{1}' but got '{2}') while constructing runtime replacement type '{3}' (original type '{4}')", propertySchema.Name, propertyType.Name, param, typeSchema9.Name, propertySchema.Owner.Name);
                                            result = ScriptError;
                                        }
                                        if (ErrorsDetected(watermark, ref result, ref flag))
                                        {
                                            break;
                                        }
                                    }
                                }

                                propertySchema.SetValue(ref destObj, propValue);
                                if (!IsDefaultValue(propValue))
                                    SetXmlPropValue(destObjXml, propertySchema.Name, propValueXml);

                                if (!ErrorsDetected(watermark, ref result, ref flag))
                                {
                                    stack.Push(destObj);
                                    xmlStack.Push(destObjXml);
                                }
                            }
                            break;
                        }
                    case OpCode.PropertyListAdd:
                        {
                            int propertyIndex = reader.ReadUInt16();
                            object value2 = stack.Pop();
                            XmlNode valueXml2 = xmlStack.Pop();
                            object collection = GetCollectionDecompile(stack.Peek(), importTables, propertyIndex, out var propertySchema);
                            ReportErrorOnNull(collection, "List Add");
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                ((IList)collection).Add(value2);
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                }

                                // Add value to list
                                var destObjXml = xmlStack.Peek();
                                if (!IsDefaultValue(value2) && valueXml2 != null)
                                {
                                    if (propertySchema != null)
                                    {
                                        // Create element if needed
                                        if (destObjXml.SelectSingleNode(propertySchema.Name) is not XmlElement propXmlElem2)
                                        {
                                            propXmlElem2 = XmlDoc.CreateElement(propertySchema.Name);
                                            destObjXml.AppendChild(propXmlElem2);
                                        }
                                        propXmlElem2.AppendChild(valueXml2);
                                    }
                                    else
                                    {
                                        // Add straight to parent element
                                        var valueXmlElem2 = XmlDoc.CreateElement("String");
                                        valueXmlElem2.AppendChild(valueXml2);
                                        destObjXml.AppendChild(valueXmlElem2);
                                    }
                                }
                            }
                            break;
                        }
                    case OpCode.PropertyDictionaryAdd:
                        {
                            int propertyIndex2 = reader.ReadUInt16();
                            int index2 = reader.ReadUInt16();
                            string key = (string)constantsTable.Get(index2);
                            object value3 = stack.Pop();
                            XmlNode valueXml3 = xmlStack.Pop();
                            object collection2 = GetCollectionDecompile(stack.Peek(), importTables, propertyIndex2, out var propertySchema2);
                            ReportErrorOnNull(collection2, "Dictionary Add");
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                ((IDictionary)collection2)[key] = value3;
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                }

                                if (!IsDefaultValue(value3) && valueXml3 != null)
                                {
                                    // Prepare value XML element
                                    if (valueXml3 is not XmlElement valueXmlElem3)
                                    {
                                        // Need to create an XML element from the node
                                        valueXmlElem3 = XmlDoc.CreateElement(value3.GetType().Name);
                                        valueXmlElem3.AppendChild(valueXml3);
                                    }

                                    // Add value to dictionary
                                    var destObjXml2 = xmlStack.Peek();
                                    // Create element if needed
                                    if (propertySchema2 != null)
                                    {
                                        if (destObjXml2.SelectSingleNode(propertySchema2.Name) is not XmlElement propXmlElem2)
                                        {
                                            propXmlElem2 = XmlDoc.CreateElement(propertySchema2.Name);
                                            destObjXml2.AppendChild(propXmlElem2);
                                        }
                                        propXmlElem2.AppendChild(valueXmlElem3);
                                    }
                                    valueXmlElem3.SetAttribute("Name", key);
                                }
                            }
                            break;
                        }
                    case OpCode.PropertyAssign:
                    case OpCode.PropertyAssignStatic:
                        {
                            int num12 = reader.ReadUInt16();
                            PropertySchema propertySchema3 = importTables.PropertyImports[num12];
                            object instance2 = null;
                            if (opCode == OpCode.PropertyAssign)
                            {
                                instance2 = stack.Pop();
                                ReportErrorOnNullOrDisposed(instance2, "Property Set", propertySchema3.Name, propertySchema3.Owner);
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                    break;
                                }
                            }
                            object value4 = stack.Peek();
                            propertySchema3.SetValue(ref instance2, value4);

                            if (opCode == OpCode.PropertyAssign)
                            {
                                var xmlInstance2 = (XmlElement)xmlStack.Pop();
                                SetXmlPropValue(xmlInstance2, propertySchema3.Name, value4);
                            }

                            if (!ErrorsDetected(watermark, ref result, ref flag) && Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.PropertyGet:
                    case OpCode.PropertyGetPeek:
                    case OpCode.PropertyGetStatic:
                        {
                            int num13 = reader.ReadUInt16();
                            PropertySchema propertySchema4 = importTables.PropertyImports[num13];

                            object instance3 = null;

                            if (opCode != OpCode.PropertyGetStatic)
                            {
                                bool pop = opCode == OpCode.PropertyGet;
                                instance3 = pop ? stack.Pop() : stack.Peek();
                                if (pop) xmlStack.Pop();

                                ReportErrorOnNullOrDisposed(instance3, "Property Get", propertySchema4.Name, propertySchema4.Owner);
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                    break;
                                }
                            }
                            object value5 = propertySchema4.GetValue(instance3);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                stack.Push(value5);
                                xmlStack.Push(GenerateXmlRepresentation(value5));
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
                            int num14 = reader.ReadUInt16();
                            MethodSchema methodSchema = importTables.MethodImports[num14];
                            int j = methodSchema.ParameterTypes.Length;
                            object[] array2 = ParameterListAllocator.Alloc(j);
                            for (j--; j >= 0; j--)
                            {
                                array2[j] = stack.Pop();
                                xmlStack.Pop();
                            }
                            object instance4 = null;
                            bool flag3 = opCode != OpCode.MethodInvokeStatic && opCode != OpCode.MethodInvokeStaticPushLastParam;
                            bool flag4 = opCode == OpCode.MethodInvokePeek;
                            bool flag5 = opCode == OpCode.MethodInvokePushLastParam || opCode == OpCode.MethodInvokeStaticPushLastParam;
                            if (flag3)
                            {
                                if (!flag4)
                                {
                                    instance4 = stack.Pop();
                                    xmlStack.Pop();
                                }
                                else
                                {
                                    instance4 = stack.Peek();
                                }
                                ReportErrorOnNullOrDisposed(instance4, "Method Invoke", methodSchema.Name, methodSchema.Owner);
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                    break;
                                }
                            }

                            if (methodSchema.Name == "PlayOrPreview")
                                System.Diagnostics.Debugger.Break();

                            object obj11 = methodSchema.Invoke(instance4, array2);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                if (!flag5)
                                {
                                    if (methodSchema.ReturnType != VoidSchema.Type)
                                    {
                                        stack.Push(obj11);
                                        xmlStack.Push(GenerateXmlRepresentation(obj11));
                                    }
                                }
                                else
                                {
                                    var lastParam = array2[array2.Length - 1];
                                    stack.Push(lastParam);
                                    xmlStack.Push(GenerateXmlRepresentation(lastParam));
                                }
                                ParameterListAllocator.Free(array2);
                            }
                            break;
                        }
                    case OpCode.VerifyTypeCast:
                        {
                            int num15 = reader.ReadUInt16();
                            TypeSchema typeSchema10 = importTables.TypeImports[num15];
                            object obj12 = stack.Peek();
                            if (obj12 != null)
                            {
                                if (!typeSchema10.IsAssignableFrom(obj12))
                                {
                                    string param2 = TypeSchema.NameFromInstance(obj12);
                                    string name = typeSchema10.Name;
                                    ErrorManager.ReportError("Script runtime failure: Invalid type cast while attempting to cast an instance with a runtime type of '{0}' to '{1}'", param2, name);
                                    result = ScriptError;
                                }
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                }
                            }
                            else if (!typeSchema10.IsNullAssignable)
                            {
                                ReportErrorOnNull(obj12, "Verify Type Cast", typeSchema10.Name);
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.ConvertType:
                        {
                            int num16 = reader.ReadUInt16();
                            TypeSchema typeSchema11 = importTables.TypeImports[num16];
                            int num17 = reader.ReadUInt16();
                            TypeSchema fromType = importTables.TypeImports[num17];
                            object obj13 = stack.Pop();
                            var xmlObj13 = xmlStack.Pop();
                            ReportErrorOnNull(obj13, "Type Conversion", typeSchema11.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                object obj14;
                                Result result2 = typeSchema11.TypeConverter(obj13, fromType, out obj14);
                                if (result2.Failed)
                                {
                                    ErrorManager.ReportError("Script runtime failure: Type conversion failed while attempting to convert to '{0}' ({1})", typeSchema11.Name, result2.Error);
                                }
                                if (!ErrorsDetected(watermark, ref result, ref flag))
                                {
                                    stack.Push(obj14);
                                    xmlStack.Push(GenerateXmlRepresentation(obj14));
                                }
                            }
                            break;
                        }
                    case OpCode.Operation:
                        {
                            int num18 = reader.ReadUInt16();
                            TypeSchema typeSchema12 = importTables.TypeImports[num18];
                            OperationType op = (OperationType)reader.ReadByte();
                            object right = null;
                            if (!TypeSchema.IsUnaryOperation(op))
                            {
                                right = stack.Pop();
                                xmlStack.Pop();
                            }
                            object left = stack.Pop();
                            xmlStack.Pop();
                            object obj15 = typeSchema12.PerformOperationDeep(left, right, op);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                stack.Push(obj15);
                                xmlStack.Push(GenerateXmlRepresentation(obj15));
                                if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.IsCheck:
                        {
                            int num19 = reader.ReadUInt16();
                            TypeSchema typeSchema13 = importTables.TypeImports[num19];
                            object obj16 = stack.Pop();
                            var xmlObj16 = xmlStack.Pop();
                            bool value6 = false;
                            if (obj16 != null)
                            {
                                value6 = typeSchema13.IsAssignableFrom(obj16);
                            }
                            stack.Push(BooleanBoxes.Box(value6));
                            xmlStack.Push(GenerateXmlRepresentation(value6));
                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.As:
                        {
                            int num20 = reader.ReadUInt16();
                            TypeSchema typeSchema14 = importTables.TypeImports[num20];
                            object obj17 = stack.Peek();
                            if (obj17 != null && !typeSchema14.IsAssignableFrom(obj17))
                            {
                                stack.Pop();
                                stack.Push(null);
                                xmlStack.Pop();
                                xmlStack.Push(GenerateXmlRepresentation(null));
                            }
                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.TypeOf:
                        {
                            int num21 = reader.ReadUInt16();
                            TypeSchema obj18 = importTables.TypeImports[num21];
                            stack.Push(obj18);
                            xmlStack.Push(GenerateXmlRepresentation(obj18));
                            break;
                        }
                    case OpCode.PushNull:
                        stack.Push(null);
                        xmlStack.Push(GenerateXmlRepresentation(null));
                        break;
                    case OpCode.PushConstant:
                        {
                            int index3 = reader.ReadUInt16();
                            object obj19 = constantsTable.Get(index3);
                            stack.Push(obj19);
                            xmlStack.Push(GenerateXmlRepresentation(obj19));
                            break;
                        }
                    case OpCode.PushThis:
                        stack.Push(instance);
                        xmlStack.Push(GenerateXmlRepresentation(instance));
                        break;
                    case OpCode.DiscardValue:
                        stack.Pop();
                        xmlStack.Pop();
                        break;
                    case OpCode.ReturnValue:
                        {
                            object obj20 = stack.Pop();
                            xmlStack.Pop();
                            result = obj20;
                            flag = true;
                            break;
                        }
                    case OpCode.ReturnVoid:
                        result = VoidReturnValue;
                        flag = true;
                        break;
                    case OpCode.JumpIfFalse:
                    case OpCode.JumpIfFalsePeek:
                    case OpCode.JumpIfTruePeek:
                        {
                            uint currentOffset = reader.ReadUInt32();
                            object obj21 = (opCode == OpCode.JumpIfFalse) ? stack.Pop() : stack.Peek();
                            if (opCode == OpCode.JumpIfFalse) xmlStack.Pop();
                            bool flag6 = (bool)obj21;
                            bool flag7 = opCode == OpCode.JumpIfTruePeek;
                            Trace.IsCategoryEnabled(TraceCategory.Markup);
                            if (flag7 == flag6)
                            {
                                reader.CurrentOffset = currentOffset;
                                if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.JumpIfDictionaryContains:
                        {
                            ushort propertyIndex3 = reader.ReadUInt16();
                            ushort index4 = reader.ReadUInt16();
                            uint currentOffset2 = reader.ReadUInt32();
                            string key2 = (string)constantsTable.Get(index4);
                            object collection3 = GetCollection(stack.Peek(), importTables, propertyIndex3, out _);
                            ReportErrorOnNull(collection3, "Dictionary Contains");
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                bool flag8 = ((IDictionary)collection3).Contains(key2);
                                Trace.IsCategoryEnabled(TraceCategory.Markup);
                                if (flag8)
                                {
                                    reader.CurrentOffset = currentOffset2;
                                    if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                    {
                                    }
                                }
                            }
                            break;
                        }
                    case OpCode.JumpIfNullPeek:
                        {
                            uint currentOffset3 = reader.ReadUInt32();
                            object obj22 = stack.Peek();
                            Trace.IsCategoryEnabled(TraceCategory.Markup);
                            if (obj22 == null)
                            {
                                reader.CurrentOffset = currentOffset3;
                                if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                                {
                                }
                            }
                            break;
                        }
                    case OpCode.Jump:
                        {
                            uint currentOffset4 = reader.ReadUInt32();
                            reader.CurrentOffset = currentOffset4;
                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.ConstructListenerStorage:
                        {
                            int listenerCount = reader.ReadUInt16();
                            if (instance.Listeners == null)
                            {
                                MarkupListeners markupListeners = new MarkupListeners(listenerCount);
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
                            int index5 = reader.ReadUInt16();
                            ListenerType listenerType = (ListenerType)reader.ReadByte();
                            int num22 = reader.ReadUInt16();
                            uint scriptOffset = reader.ReadUInt32();
                            uint refreshOffset = uint.MaxValue;
                            if (opCode == OpCode.DestructiveListen)
                            {
                                refreshOffset = reader.ReadUInt32();
                            }
                            string watch = null;
                            switch (listenerType)
                            {
                                case ListenerType.Property:
                                    watch = importTables.PropertyImports[num22].Name;
                                    break;
                                case ListenerType.Event:
                                    watch = importTables.EventImports[num22].Name;
                                    break;
                                case ListenerType.Symbol:
                                    watch = symbolReferenceTable[num22].Symbol;
                                    break;
                            }
                            object obj23 = stack.Peek();
                            if (obj23 != null)
                            {
                                INotifyObject notifier = (INotifyObject)obj23;
                                Trace.IsCategoryEnabled(TraceCategory.Markup);
                                MarkupListeners listeners = instance.Listeners;
                                listeners.RefreshListener(index5, notifier, watch, instance, scriptOffset, refreshOffset);
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
                            if (MarkupSystem.IsDebuggingEnabled(2))
                            {
                                wasInDebugState = MarkupDebugHelper.EnterDebugState(wasInDebugState, loadResult, breakpointIndex, instance.Storage);
                            }
                            break;
                        }
                }

                if (stack.Count != xmlStack.Count)
                    throw new InvalidOperationException(
                        $"Invalid stacks, stack:{stack.Count} != xmlStack{xmlStack.Count}! " +
                        $"Check implementations for {opCode}");
            }
            while (stack.Count > count)
            {
                stack.Pop();
                xmlStack.Pop();
            }

            CleanUpXmlDoc();
            Application.DebugSettings.DecompileResults.Add(new(loadResult.Uri, XmlDoc));
            return result;
        }

        // Token: 0x06000F25 RID: 3877 RVA: 0x0002ADF0 File Offset: 0x00029DF0
        private static object GetCollectionDecompile(object stackInstance, MarkupImportTables importTables, int propertyIndex, out PropertySchema prop)
        {
            object result = null;
            prop = null;
            if (propertyIndex == 65535)
            {
                result = stackInstance;
            }
            else
            {
                PropertySchema propertySchema = importTables.PropertyImports[propertyIndex];
                ReportErrorOnNull(stackInstance, "Property Get", propertySchema.Name);
                if (stackInstance != null)
                {
                    result = propertySchema.GetValue(stackInstance);
                    prop = propertySchema;
                }
            }
            return result;
        }

        private static XmlNode GenerateXmlRepresentation(object obj, int recursionLevel = 5)
        {
            if (obj == null || recursionLevel < 0)
            {
                return null;
            }

            var objType = obj.GetType();

            if (objType.IsPrimitive || objType.IsEnum || objType == typeof(string))
            {
                return XmlDoc.CreateTextNode(obj.ToString());
            }
            else if (obj is AssemblyObjectProxyHelper.IAssemblyProxyObject objProxy
                && obj != objProxy.AssemblyObject)
            {
                // This is a proxy object, meaning it doesn't actually hold any values itself.
                return GenerateXmlRepresentation(objProxy.AssemblyObject, recursionLevel);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(objType))
            {
                // This is a list or array
                var objXml = XmlDoc.CreateElement("List");
                foreach (object item in obj as IEnumerable)
                {
                    var itemXml = GenerateXmlRepresentation(item, recursionLevel);
                    if (itemXml != null)
                        objXml.AppendChild(itemXml);
                }
                return objXml;
            }
            else
            {
                var objXml = XmlDoc.CreateElement(objType.Name);

                // Use reflection to recursively generate XML representations for each property
                foreach (System.Reflection.PropertyInfo prop in objType.GetProperties())
                {
                    if (!prop.CanWrite || !prop.CanRead)
                        continue;

                    try
                    {
                        object value = prop.GetValue(obj, null);
                        if (IsDefaultValue(value))
                            continue;

                        SetXmlPropValue(objXml, prop.Name, value, recursionLevel - 1);
                    }
                    catch
                    {
                        // Ignore errors, sometimes things aren't initialized yet
                        continue;
                    }
                }

                return objXml;
            }
        }

        private static void SetXmlPropValue(XmlElement objXml, string propName, object value, int recursionLevel = 10)
        {
            var valueXml = GenerateXmlRepresentation(value, recursionLevel);
            SetXmlPropValue(objXml, propName, valueXml);
        }

        private static void SetXmlPropValue(XmlElement objXml, string propName, XmlNode valueXml)
        {
            if (valueXml is XmlElement propValueXml)
            {
                var propXml = XmlDoc.CreateElement(propName);
                propXml.AppendChild(propValueXml);
                objXml.AppendChild(propXml);
            }
            else if (valueXml != null)
            {
                // Primitive type, set as attribute
                objXml.SetAttribute(propName, valueXml.InnerText);
            }
        }

        private static bool IsDefaultValue(object value)
        {
            if (value == null)
                return true;

            var type = value.GetType();
            object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
            return value?.Equals(defaultValue) ?? true;
        }

        private static void CleanUpXmlDoc()
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(XmlDoc.NameTable);
            var hosts = XmlDoc.DocumentElement.SelectNodes("*//Host", nsManager);
            foreach (XmlNode node in hosts)
            {
                if (!(node is XmlElement host))
                    continue;

                if (host.Attributes.Count == 1 && host.HasAttribute("Name"))
                {
                    string uiElemName = host.GetAttribute("Name");
                    nsManager.AddNamespace("me", "Me");
                    XmlDoc.DocumentElement.SetAttribute("xmlns:me", "Me");

                    XmlElement uiElem = XmlDoc.CreateElement("me", uiElemName, "Me");
                    host.ParentNode.ReplaceChild(uiElem, host);
                }
            }
        }
    }
}
