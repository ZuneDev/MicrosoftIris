// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.AssemblyLoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Markup
{
    [Serializable]
    internal class AssemblyLoadResult : LoadResult
    {
        private const string c_AssemblyProtocol = "assembly://";
        private Assembly _assembly;
        private string _namespace;
        private string _namespacePrefix;
        public static TypeSchema ObjectTypeSchema;
        public static TypeSchema ListTypeSchema;
        public static TypeSchema EnumeratorTypeSchema;
        public static TypeSchema DictionaryTypeSchema;
        public static TypeSchema CommandTypeSchema;
        public static TypeSchema ValueRangeTypeSchema;
        private static Map s_typeCache = new(32);
        private static Map<MapAssemblyKey, AssemblyLoadResult> s_assemblyCache = new();

        protected AssemblyLoadResult(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _namespace = info.GetString(nameof(Namespace));
            _assembly = info.GetValue<Assembly>(nameof(Assembly));
        }

        private static LoadResult Create(string uri)
        {
            LoadResult loadResult = null;
            string valueName = uri.Substring(c_AssemblyProtocol.Length);
            if (valueName.IndexOf('/') == -1)
            {
                ErrorManager.ReportError("Invalid assembly reference '{0}'.  URI must contain a forward slash after the assembly name", uri);
                return null;
            }
            string leftName;
            string rightName;
            SplitAtLastWhack(valueName, out leftName, out rightName);
            AssemblyName name = null;
            try
            {
                name = new AssemblyName(leftName);
            }
            catch (COMException ex)
            {
            }
            catch (FileLoadException ex)
            {
            }
            if (name == null)
            {
                name = new AssemblyName();
                name.CodeBase = leftName;
            }
            if (name != null)
            {
                Exception assemblyLoadException;
                Assembly assembly = FindAssembly(name, out assemblyLoadException);
                if (assembly != null)
                    loadResult = MapAssembly(assembly, rightName);
                else if (assemblyLoadException != null)
                    ErrorManager.ReportError("Failure loading assembly: '{0}'", assemblyLoadException.Message);
                else
                    ErrorManager.ReportError("Failure loading assembly");
            }
            return loadResult;
        }

        public static void Startup()
        {
            AssemblyObjectProxyHelper.InitializeStatics();
            MarkupSystem.RegisterFactoryByProtocol(c_AssemblyProtocol, new CreateLoadResultHandler(Create));
            Map typeCache1 = s_typeCache;
            Type type1 = typeof(object);
            FrameworkCompatibleAssemblyPrimitiveTypeSchema primitiveTypeSchema;
            ObjectTypeSchema = primitiveTypeSchema = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(ObjectSchema.Type);
            TypeSchema typeA1 = primitiveTypeSchema;
            typeCache1[type1] = primitiveTypeSchema;
            TypeSchema.RegisterTwoWayEquivalence(typeA1, ObjectSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(void)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(VoidSchema.Type)), VoidSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(bool)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(BooleanSchema.Type)), BooleanSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(byte)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(ByteSchema.Type)), ByteSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(char)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(CharSchema.Type)), CharSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(double)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(DoubleSchema.Type)), DoubleSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(string)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(StringSchema.Type)), StringSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(float)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(SingleSchema.Type)), SingleSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(int)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(Int32Schema.Type)), Int32Schema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(long)] = new FrameworkCompatibleAssemblyPrimitiveTypeSchema(Int64Schema.Type)), Int64Schema.Type);
            Map typeCache2 = s_typeCache;
            Type type2 = typeof(IList);
            FrameworkCompatibleAssemblyTypeSchema assemblyTypeSchema1;
            ListTypeSchema = assemblyTypeSchema1 = new FrameworkCompatibleAssemblyTypeSchema(typeof(IList), typeof(IList), typeof(ArrayList));
            TypeSchema typeA2 = assemblyTypeSchema1;
            typeCache2[type2] = assemblyTypeSchema1;
            TypeSchema.RegisterTwoWayEquivalence(typeA2, ListSchema.Type);
            Map typeCache3 = s_typeCache;
            Type type3 = typeof(IEnumerator);
            FrameworkCompatibleAssemblyTypeSchema assemblyTypeSchema2;
            EnumeratorTypeSchema = assemblyTypeSchema2 = new FrameworkCompatibleAssemblyTypeSchema(typeof(IEnumerator), typeof(IEnumerator));
            TypeSchema typeA3 = assemblyTypeSchema2;
            typeCache3[type3] = assemblyTypeSchema2;
            TypeSchema.RegisterTwoWayEquivalence(typeA3, EnumeratorSchema.Type);
            Map typeCache4 = s_typeCache;
            Type type4 = typeof(IDictionary);
            FrameworkCompatibleAssemblyTypeSchema assemblyTypeSchema3;
            DictionaryTypeSchema = assemblyTypeSchema3 = new FrameworkCompatibleAssemblyTypeSchema(typeof(IDictionary), AssemblyObjectProxyHelper.ProxyDictionaryType, typeof(Dictionary<object, object>));
            TypeSchema producer1 = assemblyTypeSchema3;
            typeCache4[type4] = assemblyTypeSchema3;
            TypeSchema.RegisterOneWayEquivalence(producer1, DictionarySchema.Type);
            Map typeCache5 = s_typeCache;
            Type type5 = typeof(ICommand);
            FrameworkCompatibleAssemblyTypeSchema assemblyTypeSchema4;
            CommandTypeSchema = assemblyTypeSchema4 = new FrameworkCompatibleAssemblyTypeSchema(typeof(ICommand), AssemblyObjectProxyHelper.ProxyCommandType);
            TypeSchema producer2 = assemblyTypeSchema4;
            typeCache5[type5] = assemblyTypeSchema4;
            TypeSchema.RegisterOneWayEquivalence(producer2, CommandSchema.Type);
            Map typeCache6 = s_typeCache;
            Type type6 = typeof(IValueRange);
            FrameworkCompatibleAssemblyTypeSchema assemblyTypeSchema5;
            ValueRangeTypeSchema = assemblyTypeSchema5 = new FrameworkCompatibleAssemblyTypeSchema(typeof(IValueRange), AssemblyObjectProxyHelper.ProxyValueRangeType);
            TypeSchema producer3 = assemblyTypeSchema5;
            typeCache6[type6] = assemblyTypeSchema5;
            TypeSchema.RegisterOneWayEquivalence(producer3, ValueRangeSchema.Type);
            TypeSchema.RegisterOneWayEquivalence((TypeSchema)(s_typeCache[typeof(Group)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(Group), typeof(IUIGroup))), GroupSchema.Type);
            TypeSchema.RegisterOneWayEquivalence((TypeSchema)(s_typeCache[typeof(Image)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(Image), typeof(UIImage))), ImageSchema.Type);
            TypeSchema.RegisterOneWayEquivalence((TypeSchema)(s_typeCache[typeof(Type)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(Type), typeof(TypeSchema), null, ObjectTypeSchema)), TypeSchemaDefinition.Type);
            TypeSchema.RegisterOneWayEquivalence((TypeSchema)(s_typeCache[typeof(VideoStream)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(VideoStream))), VideoStreamSchema.Type);
            TypeSchema producer4;
            s_typeCache[typeof(Choice)] = (FrameworkCompatibleAssemblyTypeSchema)(producer4 = new FrameworkCompatibleAssemblyTypeSchema(typeof(Choice)));
            TypeSchema.RegisterOneWayEquivalence(producer4, ChoiceSchema.Type);
            TypeSchema.RegisterOneWayEquivalence(producer4, ValueRangeSchema.Type);
            TypeSchema.RegisterOneWayEquivalence((TypeSchema)(s_typeCache[typeof(BooleanChoice)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(BooleanChoice))), BooleanChoiceSchema.Type);
            TypeSchema producer5;
            s_typeCache[typeof(RangedValue)] = (FrameworkCompatibleAssemblyTypeSchema)(producer5 = new FrameworkCompatibleAssemblyTypeSchema(typeof(RangedValue)));
            TypeSchema.RegisterOneWayEquivalence(producer5, RangedValueSchema.Type);
            TypeSchema.RegisterOneWayEquivalence(producer5, ValueRangeSchema.Type);
            TypeSchema.RegisterOneWayEquivalence((TypeSchema)(s_typeCache[typeof(IntRangedValue)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(IntRangedValue))), IntRangedValueSchema.Type);
            TypeSchema.RegisterOneWayEquivalence((TypeSchema)(s_typeCache[typeof(ByteRangedValue)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(ByteRangedValue))), ByteRangedValueSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(DataProviderQuery)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(DataProviderQuery), typeof(MarkupDataQuery))), MarkupDataQueryInstanceSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(DataProviderObject)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(DataProviderObject), typeof(MarkupDataType))), MarkupDataTypeInstanceSchema.Type);
            TypeSchema.RegisterTwoWayEquivalence((TypeSchema)(s_typeCache[typeof(DataProviderQueryStatus)] = new FrameworkCompatibleAssemblyTypeSchema(typeof(DataProviderQueryStatus))), UIXLoadResultExports.DataQueryStatusType);
        }

        public static void Shutdown()
        {
            foreach (SharedDisposableObject disposableObject in s_assemblyCache.Values)
                disposableObject.UnregisterUsage(s_assemblyCache);
            s_assemblyCache.Clear();
            s_assemblyCache = null;
            foreach (AssemblyTypeSchema assemblyTypeSchema in s_typeCache.Values)
            {
                AssemblyLoadResult owner = (AssemblyLoadResult)assemblyTypeSchema.Owner;
                assemblyTypeSchema.Dispose(owner);
            }
            s_typeCache.Clear();
            s_typeCache = null;
        }

        public string Namespace => _namespace;

        public override LoadResultStatus Status => LoadResultStatus.Success;

        public override TypeSchema FindType(string name)
        {
            Type type = _assembly.GetType(_namespacePrefix + name, false);
            if (type == null)
                return null;
            if (type.IsVisible)
                return MapType(type);
            ErrorManager.ReportError("Type '{0}' is not public in '{1}'", name, _assembly);
            return null;
        }

        public static AssemblyLoadResult MapAssembly(Assembly assembly, string ns)
        {
            MapAssemblyKey key = new MapAssemblyKey(assembly, ns);
            AssemblyLoadResult assemblyLoadResult;
            if (!s_assemblyCache.TryGetValue(key, out assemblyLoadResult))
            {
                string uri = c_AssemblyProtocol + assembly.FullName;
                if (ns != null)
                    uri = uri + "/" + ns;
                assemblyLoadResult = new AssemblyLoadResult(assembly, ns, uri);
                s_assemblyCache[key] = assemblyLoadResult;
                assemblyLoadResult.RegisterUsage(s_assemblyCache);
            }
            return assemblyLoadResult;
        }

        public static AssemblyTypeSchema MapType(Type type)
        {
            object obj;
            AssemblyTypeSchema assemblyTypeSchema;
            if (s_typeCache.TryGetValue(type, out obj))
            {
                assemblyTypeSchema = (AssemblyTypeSchema)obj;
            }
            else
            {
                assemblyTypeSchema = AssemblyObjectProxyHelper.CreateProxySchema(type);
                s_typeCache[type] = assemblyTypeSchema;
            }
            return assemblyTypeSchema;
        }

        public static Type MapType(TypeSchema typeSchema)
        {
            if (typeSchema is AssemblyTypeSchema assemblyTypeSchema)
                return assemblyTypeSchema.InternalType;
            for (TypeSchema typeSchema1 = typeSchema; typeSchema1 != null; typeSchema1 = typeSchema1.Base)
            {
                if (typeSchema1.Equivalents != null)
                {
                    foreach (TypeSchema equivalent in typeSchema1.Equivalents)
                    {
                        if (equivalent is AssemblyTypeSchema assemblyTypeSchemaB)
                            return assemblyTypeSchemaB.InternalType;
                    }
                }
            }
            return null;
        }

        internal static Type[] MapTypeList(TypeSchema[] typeSchemaList)
        {
            Type[] typeArray = new Type[typeSchemaList.Length];
            for (int index = 0; index < typeSchemaList.Length; ++index)
            {
                typeArray[index] = MapType(typeSchemaList[index]);
                if (typeArray[index] == null)
                    return null;
            }
            return typeArray;
        }

        internal static TypeSchema[] MapTypeList(Type[] typeList)
        {
            TypeSchema[] typeSchemaArray = new TypeSchema[typeList.Length];
            for (int index = 0; index < typeList.Length; ++index)
            {
                typeSchemaArray[index] = MapType(typeList[index]);
                if (typeSchemaArray[index] == null)
                    return null;
            }
            return typeSchemaArray;
        }

        internal static object WrapObject(TypeSchema typeSchema, object instance) => AssemblyObjectProxyHelper.WrapObject(typeSchema, instance);

        internal static object WrapObject(object instance) => AssemblyObjectProxyHelper.WrapObject(null, instance);

        internal static object UnwrapObject(object instance) => AssemblyObjectProxyHelper.UnwrapObject(instance);

        internal static object[] UnwrapObjectList(object[] instanceList)
        {
            if (instanceList == null)
                return null;
            object[] objArray = new object[instanceList.Length];
            for (int index = 0; index < objArray.Length; ++index)
                objArray[index] = UnwrapObject(instanceList[index]);
            return objArray;
        }

        private static void SplitAtLastWhack(
          string valueName,
          out string leftName,
          out string rightName)
        {
            int length = valueName.LastIndexOf('/');
            if (length != -1)
            {
                leftName = valueName.Substring(0, length);
                rightName = valueName.Substring(length + 1);
            }
            else
            {
                leftName = valueName;
                rightName = null;
            }
        }

        public static Assembly FindAssembly(
          AssemblyName name,
          out Exception assemblyLoadException)
        {
            Assembly assembly = null;
            assemblyLoadException = null;
            try
            {
                assembly = Assembly.Load(name);
            }
            catch (FileLoadException ex)
            {
                assemblyLoadException = ex;
            }
            catch (BadImageFormatException ex)
            {
                assemblyLoadException = ex;
            }
            catch (FileNotFoundException ex)
            {
                assemblyLoadException = ex;
            }
            return assembly;
        }

        public override string GetCompilerReferenceName() => base.GetCompilerReferenceName() ?? $"{c_AssemblyProtocol}{_assembly.GetName().Name}/{_namespace}";

        internal Assembly Assembly => _assembly;

        private AssemblyLoadResult(Assembly assembly, string ns, string uri)
          : base(uri)
        {
            _assembly = assembly;
            _namespace = ns;
            if (ns == null)
                return;
            _namespacePrefix = ns + ".";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Namespace), Namespace);
            info.AddValue(nameof(Assembly), Assembly);
        }

        internal struct MapAssemblyKey
        {
            private Assembly _assembly;
            private string _namespace;

            public MapAssemblyKey(Assembly assembly, string ns)
            {
                _assembly = assembly;
                _namespace = ns;
            }

            public override bool Equals(object obj)
            {
                MapAssemblyKey mapAssemblyKey = (MapAssemblyKey)obj;
                return _assembly == mapAssemblyKey._assembly && _namespace == mapAssemblyKey._namespace;
            }

            public override int GetHashCode()
            {
                int hashCode = _assembly.GetHashCode();
                if (_namespace != null)
                    hashCode ^= _namespace.GetHashCode();
                return hashCode;
            }
        }
    }
}
