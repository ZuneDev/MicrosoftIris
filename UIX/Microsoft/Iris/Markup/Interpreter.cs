using System;
using System.Collections;
using System.Linq;
using System.Xml;
using Microsoft.Iris.Debug;
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
            bool flag = true;
            ErrorManager.EnterContext(context);
            try
            {
                byteCodeReader = context.LoadResult.ObjectSection;
                num = (long)((ulong)byteCodeReader.CurrentOffset);
                byteCodeReader.CurrentOffset = context.InitialBytecodeOffset;
                if (Application.DebugSettings.UseDecompiler)
                    result = RunDecompile(context, byteCodeReader);
                else
                    result = Run(context, byteCodeReader);
                flag = false;
            }
            finally
            {
                if (flag)
                {
                    ExceptionContext = context.ToString();
                }
                ErrorManager.ExitContext();
                if (byteCodeReader != null && num != -1L)
                {
                    byteCodeReader.CurrentOffset = (uint)num;
                }
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
            Trace.IsCategoryEnabled(TraceCategory.Markup);
            Stack stack = _stack;
            int count = stack.Count;
            if (instance != null)
            {
                stack.Push(instance);
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
                            ReportErrorOnNull(obj, "Construction", typeSchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj, typeSchema, instance);
                                stack.Push(obj);
                            }
                            break;
                        }
                    case OpCode.ConstructObjectIndirect:
                        {
                            int num2 = reader.ReadUInt16();
                            TypeSchema typeSchema2 = importTables.TypeImports[num2];
                            TypeSchema typeSchema3 = (TypeSchema)stack.Pop();
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
                            ReportErrorOnNull(obj2, "Construction", typeSchema3.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj2, typeSchema3, instance);
                                stack.Push(obj2);
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
                            }
                            object obj3 = constructorSchema.Construct(array);
                            ReportErrorOnNull(obj3, "Construction", typeSchema4.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj3, typeSchema4, instance);
                                stack.Push(obj3);
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
                            ReportErrorOnNull(obj4, "Construction", typeSchema5.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj4, typeSchema5, instance);
                                stack.Push(obj4);
                            }
                            break;
                        }
                    case OpCode.ConstructFromBinary:
                        {
                            int num6 = reader.ReadUInt16();
                            TypeSchema typeSchema6 = importTables.TypeImports[num6];
                            object obj5 = typeSchema6.DecodeBinary(reader);
                            ReportErrorOnNull(obj5, "Construction", typeSchema6.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                RegisterDisposable(obj5, typeSchema6, instance);
                                stack.Push(obj5);
                            }
                            break;
                        }
                    case OpCode.InitializeInstance:
                        {
                            int num7 = reader.ReadUInt16();
                            TypeSchema typeSchema7 = importTables.TypeImports[num7];
                            object obj6 = stack.Pop();
                            typeSchema7.InitializeInstance(ref obj6);
                            ReportErrorOnNull(obj6, "Initialize", typeSchema7.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                stack.Push(obj6);
                            }
                            break;
                        }
                    case OpCode.InitializeInstanceIndirect:
                        {
                            TypeSchema typeSchema8 = (TypeSchema)stack.Pop();
                            object obj7 = stack.Pop();
                            typeSchema8.InitializeInstance(ref obj7);
                            ReportErrorOnNull(obj7, "Initialize", typeSchema8.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                stack.Push(obj7);
                            }
                            break;
                        }
                    case OpCode.LookupSymbol:
                        {
                            int num8 = reader.ReadUInt16();
                            SymbolReference symbolRef = symbolReferenceTable[num8];
                            object obj8 = context.ReadSymbol(symbolRef);
                            stack.Push(obj8);
                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.WriteSymbol:
                    case OpCode.WriteSymbolPeek:
                        {
                            object value = (opCode == OpCode.WriteSymbolPeek) ? stack.Peek() : stack.Pop();
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
                            bool flag2 = opCode == OpCode.PropertyInitializeIndirect;
                            TypeSchema typeSchema9 = null;
                            if (flag2)
                            {
                                typeSchema9 = (TypeSchema)stack.Pop();
                            }
                            int num11 = reader.ReadUInt16();
                            PropertySchema propertySchema = importTables.PropertyImports[num11];
                            object obj9 = stack.Pop();
                            object obj10 = stack.Pop();
                            ReportErrorOnNull(obj10, "Property Set", propertySchema.Name);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                if (flag2)
                                {
                                    PropertySchema propertySchema2 = typeSchema9.FindPropertyDeep(propertySchema.Name);
                                    if (propertySchema2 != propertySchema)
                                    {
                                        TypeSchema propertyType = propertySchema2.PropertyType;
                                        if (!propertyType.IsAssignableFrom(obj9))
                                        {
                                            string param = TypeSchema.NameFromInstance(obj9);
                                            ErrorManager.ReportError("Script runtime failure: Incompatible value for property '{0}' supplied (expecting values of type '{1}' but got '{2}') while constructing runtime replacement type '{3}' (original type '{4}')", propertySchema.Name, propertyType.Name, param, typeSchema9.Name, propertySchema.Owner.Name);
                                            result = ScriptError;
                                        }
                                        if (ErrorsDetected(watermark, ref result, ref flag))
                                        {
                                            break;
                                        }
                                    }
                                }
                                propertySchema.SetValue(ref obj10, obj9);
                                if (!ErrorsDetected(watermark, ref result, ref flag))
                                {
                                    stack.Push(obj10);
                                }
                            }
                            break;
                        }
                    case OpCode.PropertyListAdd:
                        {
                            int propertyIndex = reader.ReadUInt16();
                            object value2 = stack.Pop();
                            object collection = GetCollection(stack.Peek(), importTables, propertyIndex);
                            ReportErrorOnNull(collection, "List Add");
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                ((IList)collection).Add(value2);
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
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
                            object collection2 = GetCollection(stack.Peek(), importTables, propertyIndex2);
                            ReportErrorOnNull(collection2, "Dictionary Add");
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                ((IDictionary)collection2)[key] = value3;
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
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
                                instance3 = ((opCode == OpCode.PropertyGet) ? stack.Pop() : stack.Peek());
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
                            object obj11 = methodSchema.Invoke(instance4, array2);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                if (!flag5)
                                {
                                    if (methodSchema.ReturnType != VoidSchema.Type)
                                    {
                                        stack.Push(obj11);
                                    }
                                }
                                else
                                {
                                    stack.Push(array2[array2.Length - 1]);
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
                            }
                            object left = stack.Pop();
                            object obj15 = typeSchema12.PerformOperationDeep(left, right, op);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                stack.Push(obj15);
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
                            bool value6 = false;
                            if (obj16 != null)
                            {
                                value6 = typeSchema13.IsAssignableFrom(obj16);
                            }
                            stack.Push(BooleanBoxes.Box(value6));
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
                            break;
                        }
                    case OpCode.PushNull:
                        stack.Push(null);
                        break;
                    case OpCode.PushConstant:
                        {
                            int index3 = reader.ReadUInt16();
                            object obj19 = constantsTable.Get(index3);
                            stack.Push(obj19);
                            break;
                        }
                    case OpCode.PushThis:
                        stack.Push(instance);
                        break;
                    case OpCode.DiscardValue:
                        stack.Pop();
                        break;
                    case OpCode.ReturnValue:
                        {
                            object obj20 = stack.Pop();
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
                            object collection3 = GetCollection(stack.Peek(), importTables, propertyIndex3);
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
        private static object GetCollection(object stackInstance, MarkupImportTables importTables, int propertyIndex)
        {
            object result = null;
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
                }
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
        private static Stack _xmlStack = new Stack();

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
                else if (count < 20)
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
                if (num != 0 && num < 20 && s_cachedLists[num] == null)
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
            private static object[][] s_cachedLists = new object[20][];
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
            Stack stack = _stack;
            Stack xmlStack = _xmlStack;
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
                var entry = new Debug.Data.InterpreterEntry(opCode);

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

                            entry.Arguments.Add(new Debug.Data.OpCodeArgument(
                                "type", typeof(TypeSchema), typeSchema));

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
                            TypeSchema typeSchemaXml3 = (TypeSchema)xmlStack.Pop();
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
                            entry.Arguments.Add(new Debug.Data.OpCodeArgument(
                                "symbolRef", typeof(SymbolReference), symbolRef));

                            object obj8 = context.ReadSymbol(symbolRef);
                            var objXml8 = XmlDoc.CreateElement(obj8.GetType().Name);

                            stack.Push(obj8);
                            xmlStack.Push(objXml8);
                            entry.ReturnValues.Add(obj8);

                            if (Trace.IsCategoryEnabled(TraceCategory.Markup))
                            {
                            }
                            break;
                        }
                    case OpCode.WriteSymbol:
                    case OpCode.WriteSymbolPeek:
                        {
                            object value = (opCode == OpCode.WriteSymbolPeek) ? stack.Peek() : stack.Pop();
                            int num9 = reader.ReadUInt16();
                            SymbolReference symbolRef2 = symbolReferenceTable[num9];

                            entry.Arguments.Add(new Debug.Data.OpCodeArgument(
                                "symbolRef", typeof(SymbolReference), symbolRef2));
                            entry.Arguments.Add(new Debug.Data.OpCodeArgument(
                                "value", typeof(object), value));

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
                            }
                            int num11 = reader.ReadUInt16();
                            PropertySchema propertySchema = importTables.PropertyImports[num11];
                            object propValue = stack.Pop();
                            object destObj = stack.Pop();
                            object propValueXml = xmlStack.Pop();
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
                                if (propValueXml is XmlElement propValueXmlElem)
                                {
                                    var propXmlElem = XmlDoc.CreateElement(propertySchema.Name);
                                    propXmlElem.AppendChild(propValueXmlElem);
                                    destObjXml.AppendChild(propXmlElem);
                                }
                                else
                                {
                                    destObjXml.SetAttribute(propertySchema.Name, propValueXml.ToString());
                                }
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
                            XmlElement valueXml2 = (XmlElement)xmlStack.Pop();
                            object collection = GetCollectionDecompile(stack.Peek(), importTables, propertyIndex, out var propertySchema);
                            ReportErrorOnNull(collection, "List Add");
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                ((IList)collection).Add(value2);
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                }

                                // Add value to list
                                var destObjXml = (XmlElement)xmlStack.Peek();
                                // Create element if needed
                                if (!(destObjXml.SelectSingleNode(propertySchema.Name) is XmlElement propXmlElem2))
                                {
                                    propXmlElem2 = XmlDoc.CreateElement(propertySchema.Name);
                                    destObjXml.AppendChild(propXmlElem2);
                                }
                                propXmlElem2.AppendChild(valueXml2);
                            }
                            break;
                        }
                    case OpCode.PropertyDictionaryAdd:
                        {
                            int propertyIndex2 = reader.ReadUInt16();
                            int index2 = reader.ReadUInt16();
                            string key = (string)constantsTable.Get(index2);
                            object value3 = stack.Pop();
                            XmlElement valueXml3 = (XmlElement)xmlStack.Pop();
                            object collection2 = GetCollectionDecompile(stack.Peek(), importTables, propertyIndex2, out var propertySchema2);
                            ReportErrorOnNull(collection2, "Dictionary Add");
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                ((IDictionary)collection2)[key] = value3;
                                if (ErrorsDetected(watermark, ref result, ref flag))
                                {
                                }

                                // Add value to dictionary
                                var destObjXml2 = (XmlElement)xmlStack.Peek();
                                // Create element if needed
                                if (propertySchema2 != null)
                                {
                                    XmlElement propXmlElem2 = destObjXml2.SelectSingleNode(propertySchema2.Name) as XmlElement;
                                    if (propXmlElem2 == null)
                                    {
                                        propXmlElem2 = XmlDoc.CreateElement(propertySchema2.Name);
                                        destObjXml2.AppendChild(propXmlElem2);
                                    }
                                    propXmlElem2.AppendChild(valueXml3);
                                }
                                valueXml3.SetAttribute("Name", key);
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
                                instance3 = ((opCode == OpCode.PropertyGet) ? stack.Pop() : stack.Peek());
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
                            object obj11 = methodSchema.Invoke(instance4, array2);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                if (!flag5)
                                {
                                    if (methodSchema.ReturnType != VoidSchema.Type)
                                    {
                                        stack.Push(obj11);
                                    }
                                }
                                else
                                {
                                    stack.Push(array2[array2.Length - 1]);
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
                            }
                            object left = stack.Pop();
                            object obj15 = typeSchema12.PerformOperationDeep(left, right, op);
                            if (!ErrorsDetected(watermark, ref result, ref flag))
                            {
                                stack.Push(obj15);
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
                            bool value6 = false;
                            if (obj16 != null)
                            {
                                value6 = typeSchema13.IsAssignableFrom(obj16);
                            }
                            stack.Push(BooleanBoxes.Box(value6));
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
                            break;
                        }
                    case OpCode.PushNull:
                        stack.Push(null);
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
                        break;
                    case OpCode.ReturnValue:
                        {
                            object obj20 = stack.Pop();
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
                            object collection3 = GetCollection(stack.Peek(), importTables, propertyIndex3);
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

                Application.DebugSettings.Bridge.LogInterpreterOpCode(opCode, entry);

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
            Application.DebugSettings.DecompileResults.Add(XmlDoc);
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

        private static object GenerateXmlRepresentation(object obj)
        {
            var objType = obj.GetType();
            if (objType.IsPrimitive || objType.IsEnum || objType == typeof(string))
                return obj;
            else
            {
                var objXml = XmlDoc.CreateElement(objType.Name);
                // Use reflection to recursively generate XML representations for each property
                foreach (System.Reflection.PropertyInfo prop in objType.GetProperties())
                {
                    if (!prop.CanWrite)
                        continue;

                    object value = prop.GetValue(obj, null);
                    object defaultValue = prop.PropertyType.IsValueType ? Activator.CreateInstance(prop.PropertyType) : null;
                    if (value.Equals(defaultValue))
                        continue;

                    var propValue = GenerateXmlRepresentation(value);
                    if (propValue is XmlElement propValueXml)
                    {
                        var propXml = XmlDoc.CreateElement(prop.Name);
                        propXml.AppendChild(propValueXml);
                        objXml.AppendChild(propXml);
                    }
                    else
                    {
                        // Primitive type, set as attribute
                        objXml.SetAttribute(prop.Name, propValue.ToString());
                    }
                }
                return objXml;
            }
        }

        private static void CleanUpXmlDoc()
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(XmlDoc.NameTable);
            var hosts = XmlDoc.DocumentElement.SelectNodes("*//Host",  nsManager);
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
