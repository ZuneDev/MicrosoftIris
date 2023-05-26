// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllLoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.CodeModel.Cpp
{
    [Serializable]
    internal class DllLoadResult : LoadResult
    {
        private DllLoadResultFactory _source;
        private LoadResultStatus _status;
        private LoadPass _loadPass;
        private IntPtr _schema;
        private Map<uint, TypeSchema> _userDefinedTypes;
        private Map<uint, TypeSchema> _intrinsicTypes;
        private static Map<uint, IntrinsicTypeData> s_intrinsicData;
        private ushort _component;
        private static ushort s_nextID = 1;
        private static LoadResult s_objectContext;

        public static void Startup()
        {
            DllLoadResultFactory.Startup();
            DllProxyServices.Startup();
            LoadIntrinsicTypeData();
        }

        private static void LoadIntrinsicTypeData()
        {
            s_intrinsicData = new Map<uint, IntrinsicTypeData>();
            s_intrinsicData[4294967294U] = new IntrinsicTypeData(BooleanSchema.Type);
            s_intrinsicData[4294967293U] = new IntrinsicTypeData(ByteSchema.Type);
            s_intrinsicData[4294967292U] = new IntrinsicTypeData(DoubleSchema.Type);
            s_intrinsicData[4294967285U] = new IntrinsicTypeData(ListSchema.Type, typeof(DllProxyList));
            s_intrinsicData[4294967284U] = new IntrinsicTypeData(ImageSchema.Type);
            s_intrinsicData[4294967283U] = new IntrinsicTypeData(Int32Schema.Type);
            s_intrinsicData[4294967282U] = new IntrinsicTypeData(Int64Schema.Type);
            s_intrinsicData[4294967280U] = new IntrinsicTypeData(ObjectSchema.Type);
            s_intrinsicData[4294967279U] = new IntrinsicTypeData(SingleSchema.Type);
            s_intrinsicData[4294967278U] = new IntrinsicTypeData(StringSchema.Type);
            s_intrinsicData[4294967277U] = new IntrinsicTypeData(VoidSchema.Type);
        }

        public static void Shutdown() => DllProxyServices.Shutdown();

        public static bool CheckNativeReturn(uint hr, string interfaceName)
        {
            if ((int)hr >= 0)
                return true;
            ErrorManager.ReportError("Schema API failure: A method on '{0}' failed with code '0x{1:X8}'", interfaceName, hr);
            return false;
        }

        private bool CheckNativeReturn(uint hr) => CheckNativeReturn(hr, "IUIXTypeSchema");

        public static void PushContext(LoadResult newContext) => s_objectContext = newContext;

        public static LoadResult CurrentContext => s_objectContext;

        public static void PopContext() => s_objectContext = null;

        public static TypeSchema MapType(uint typeID)
        {
            uint schemaComponent = UIXID.GetSchemaComponent(typeID);
            TypeSchema typeSchema = null;
            DllLoadResult dllLoadResult = null;
            if (schemaComponent != ushort.MaxValue)
            {
                dllLoadResult = DllLoadResultFactory.GetLoadResultByID(schemaComponent);
                if (dllLoadResult != null)
                    typeSchema = dllLoadResult.MapLocalType(typeID);
            }
            else if (CurrentContext is DllLoadResult currentContext)
                typeSchema = currentContext.MapIntrinsicType(typeID);
            if (typeSchema == null)
                ErrorManager.ReportError("Unable to find type with ID '0x{0:X8}' in '{1}'", typeID, dllLoadResult != null ? dllLoadResult.Uri : string.Empty);
            return typeSchema;
        }

        private TypeSchema MapIntrinsicType(uint typeID)
        {
            if (_intrinsicTypes == null)
                _intrinsicTypes = new Map<uint, TypeSchema>();
            TypeSchema typeSchema;
            IntrinsicTypeData intrinsicTypeData;
            if (!_intrinsicTypes.TryGetValue(typeID, out typeSchema) && s_intrinsicData.TryGetValue(typeID, out intrinsicTypeData))
            {
                typeSchema = !intrinsicTypeData.DemandCreateTypeSchema ? intrinsicTypeData.FrameworkEquivalent : new DllIntrinsicTypeSchema(this, typeID, intrinsicTypeData.FrameworkEquivalent);
                _intrinsicTypes[typeID] = typeSchema;
            }
            return typeSchema;
        }

        private TypeSchema MapLocalType(uint typeID)
        {
            TypeSchema typeSchema = null;
            _userDefinedTypes.TryGetValue(typeID, out typeSchema);
            return typeSchema;
        }

        public DllLoadResult(DllLoadResultFactory sourceFactory, IntPtr nativeSchema, string uri)
          : base(uri)
        {
            _source = sourceFactory;
            _status = LoadResultStatus.Loading;
            _schema = nativeSchema;
            _loadPass = LoadPass.Invalid;
            _component = s_nextID++;
        }

        public override void Load(LoadPass pass)
        {
            if (pass <= _loadPass || pass != LoadPass.DeclareTypes)
                return;
            _status = !SetSchemaID() || !LoadTypes() ? LoadResultStatus.Error : LoadResultStatus.Success;
            _loadPass = LoadPass.Done;
        }

        public ushort SchemaComponent => _component;

        private bool SetSchemaID() => CheckNativeReturn(NativeApi.SpSetSchemaID(_schema, SchemaComponent));

        private bool LoadTypes()
        {
            bool flag = false;
            UIXIDVerifier idVerifier = new UIXIDVerifier(this);
            uint typeCount;
            uint enumCount;
            if (CheckNativeReturn(NativeApi.SpQueryTypeCount(_schema, out typeCount)) && CheckNativeReturn(NativeApi.SpQueryEnumCount(_schema, out enumCount)))
            {
                int capacity = (int)typeCount + (int)enumCount;
                if (capacity > 0)
                {
                    _userDefinedTypes = new Map<uint, TypeSchema>(capacity);
                    if (!LoadClasses(typeCount, idVerifier) || !LoadEnums(enumCount, idVerifier) || !ResolveTypes(idVerifier))
                        goto label_4;
                }
                flag = true;
            }
        label_4:
            return flag;
        }

        private bool LoadClasses(uint numClasses, UIXIDVerifier idVerifier)
        {
            bool flag = false;
            if (numClasses > 0U)
            {
                for (uint index = 0; index < numClasses; ++index)
                {
                    if (!GetTypeSchema(index, idVerifier))
                        flag = true;
                }
            }
            return !flag;
        }

        private bool GetTypeSchema(uint index, UIXIDVerifier idVerifier)
        {
            bool flag = false;
            IntPtr type;
            uint ID;
            if (CheckNativeReturn(NativeApi.SpGetTypeSchema(_schema, index, out type, out ID)))
            {
                if (type != IntPtr.Zero)
                {
                    StoreType(new DllTypeSchema(this, ID, type), ID, true, idVerifier);
                    flag = idVerifier.RegisterID(ID);
                }
                else
                    ErrorManager.ReportError("NULL object returned from {0}", "IUIXSchema::GetType");
            }
            return flag;
        }

        private void StoreType(TypeSchema schema, uint ID, bool isClass, UIXIDVerifier idVerifier) => _userDefinedTypes[ID] = schema;

        private static void DEBUG_DumpType(object param)
        {
        }

        private bool LoadEnums(uint numEnums, UIXIDVerifier idVerifier)
        {
            bool flag = false;
            if (numEnums > 0U)
            {
                for (uint index = 0; index < numEnums; ++index)
                {
                    if (!GetEnumSchema(index, idVerifier))
                        flag = true;
                }
            }
            return !flag;
        }

        private bool GetEnumSchema(uint index, UIXIDVerifier idVerifier)
        {
            bool flag = false;
            IntPtr enumType;
            uint ID;
            if (CheckNativeReturn(NativeApi.SpGetEnumSchema(_schema, index, out enumType, out ID)))
            {
                if (enumType != IntPtr.Zero)
                {
                    StoreType(new DllEnumSchema(this, ID, enumType), ID, false, idVerifier);
                    flag = idVerifier.RegisterID(ID);
                }
                else
                    ErrorManager.ReportError("NULL object returned from {0}", "IUIXSchema::GetEnum");
            }
            return flag;
        }

        private bool ResolveTypes(UIXIDVerifier idVerifier)
        {
            bool flag = false;
            if (_userDefinedTypes != null && _userDefinedTypes.Count > 0)
            {
                foreach (TypeSchema typeSchema in _userDefinedTypes.Values)
                {
                    switch (typeSchema)
                    {
                        case DllTypeSchema dllTypeSchema:
                            if (!dllTypeSchema.Load(idVerifier))
                            {
                                flag = true;
                                continue;
                            }
                            continue;
                        case DllEnumSchema dllEnumSchema:
                            if (!dllEnumSchema.Load())
                            {
                                flag = true;
                                continue;
                            }
                            continue;
                        default:
                            flag = true;
                            continue;
                    }
                }
            }
            return !flag;
        }

        private static void DEBUG_DumpEnum(object param)
        {
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (_userDefinedTypes != null && _userDefinedTypes.Count > 0)
            {
                foreach (DisposableObject disposableObject in _userDefinedTypes.Values)
                    disposableObject.Dispose(this);
            }
            if (_intrinsicTypes != null && _intrinsicTypes.Count > 0)
            {
                foreach (TypeSchema typeSchema in _intrinsicTypes.Values)
                {
                    if (typeSchema is DllIntrinsicTypeSchema intrinsicTypeSchema)
                        intrinsicTypeSchema.Dispose(this);
                }
            }
            NativeApi.SpReleaseExternalObject(_schema);
            _schema = IntPtr.Zero;
            _source.NotifyLoadResultDisposed(this);
        }

        public override TypeSchema FindType(string name)
        {
            if (_userDefinedTypes != null && _userDefinedTypes.Count > 0)
            {
                foreach (TypeSchema typeSchema in _userDefinedTypes.Values)
                {
                    if (typeSchema.Name == name)
                        return typeSchema;
                }
            }
            return null;
        }

        public override LoadResultStatus Status => _status;

        public override LoadResult[] Dependencies => EmptyList;

        public override bool Cachable => true;

        public static Type RuntimeTypeForMarshalAs(uint marshalAs)
        {
            Type type = null;
            if (marshalAs == uint.MaxValue)
            {
                type = typeof(DllProxyObject);
            }
            else
            {
                IntrinsicTypeData intrinsicTypeData;
                if (s_intrinsicData.TryGetValue(marshalAs, out intrinsicTypeData))
                    type = intrinsicTypeData.MarshalAsRuntimeType;
            }
            return type;
        }

        private class IntrinsicTypeData
        {
            private TypeSchema _frameworkType;
            private Type _runtimeType;

            public IntrinsicTypeData(TypeSchema frameworkEquivalent)
              : this(frameworkEquivalent, null)
            {
            }

            public IntrinsicTypeData(TypeSchema frameworkEquivalent, Type runtimeMarshalAsType)
            {
                _frameworkType = frameworkEquivalent;
                _runtimeType = runtimeMarshalAsType;
            }

            public TypeSchema FrameworkEquivalent => _frameworkType;

            public Type MarshalAsRuntimeType => _runtimeType;

            public bool DemandCreateTypeSchema => MarshalAsRuntimeType != null;
        }
    }
}
