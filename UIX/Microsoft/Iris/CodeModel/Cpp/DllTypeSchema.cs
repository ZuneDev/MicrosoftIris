// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using System;
using System.Diagnostics;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllTypeSchema : DllTypeSchemaBase
    {
        private const uint UIX_E_SCHEMA_ERRORS_REPORTED = 2147774470;
        private IntPtr _type;
        private uint _defaultConstructorID;

        public DllTypeSchema(DllLoadResult owner, uint ID, IntPtr typeSchema)
          : base(owner, ID)
          => _type = typeSchema;

        public bool Load(UIXIDVerifier idVerifier)
        {
            DllLoadResult.PushContext(OwnerLoadResult);
            bool flag = QueryTypeName() && QueryIsRuntimeImmutable() && (QueryBaseType() && QueryConstructors(idVerifier)) && (QueryProperties(idVerifier) && QueryMethods(idVerifier) && QueryEvents(idVerifier)) && QueryMarshalAs();
            DllLoadResult.PopContext();
            return flag;
        }

        [Conditional("DEBUG")]
        public void DEBUG_DumpType()
        {
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpConstructors()
        {
            bool flag = false;
            if (HasDefaultConstructor)
                flag = true;
            if (_constructors != null)
            {
                foreach (DllConstructorSchema constructorSchema in _constructors.Values)
                    ;
                flag = true;
            }
            int num = flag ? 1 : 0;
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpProperties()
        {
            if (_properties == null)
                return;
            foreach (DllPropertySchema dllPropertySchema in _properties.Values)
                ;
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpMethods()
        {
            if (_methods == null)
                return;
            foreach (DllMethodSchema dllMethodSchema in _methods.Values)
                ;
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpEvents()
        {
            if (_events == null)
                return;
            foreach (DllEventSchema dllEventSchema in _events.Values)
                ;
        }

        private unsafe bool QueryTypeName()
        {
            bool flag = false;
            char* name;
            if (CheckNativeReturn(NativeApi.SpQueryTypeName(_type, out name)))
            {
                _name = new string(name);
                flag = true;
            }
            return flag;
        }

        private bool QueryIsRuntimeImmutable()
        {
            bool flag = false;
            bool isRuntimeImmutable;
            if (CheckNativeReturn(NativeApi.SpIsRuntimeImmutable(_type, out isRuntimeImmutable)))
            {
                SetBit(Bits.IsRuntimeImmutable, isRuntimeImmutable);
                flag = true;
            }
            return flag;
        }

        private bool QueryBaseType()
        {
            bool flag = false;
            uint baseTypeID;
            if (CheckNativeReturn(NativeApi.SpQueryBaseType(_type, out baseTypeID)))
            {
                _baseType = DllLoadResult.MapType(baseTypeID);
                flag = true;
            }
            return flag;
        }

        private bool QueryConstructors(UIXIDVerifier idVerifier)
        {
            bool flag1 = false;
            uint count;
            if (CheckNativeReturn(NativeApi.SpQueryConstructorCount(_type, out count)))
            {
                bool flag2 = false;
                _constructors = new Map<MethodSignatureKey, DllConstructorSchema>((int)count);
                for (uint index = 0; index < count; ++index)
                {
                    if (!LoadConstructor(index, idVerifier))
                        flag2 = true;
                }
                flag1 = !flag2;
            }
            return flag1;
        }

        private bool LoadConstructor(uint index, UIXIDVerifier idVerifier)
        {
            bool flag = false;
            IntPtr constructor;
            uint ID;
            if (CheckNativeReturn(NativeApi.SpGetConstructorSchema(_type, index, out constructor, out ID)))
            {
                if (constructor != IntPtr.Zero)
                {
                    DllConstructorSchema constructorSchema = new DllConstructorSchema(this, ID);
                    flag = constructorSchema.Load(constructor);
                    if (flag)
                    {
                        _constructors[new MethodSignatureKey(constructorSchema.ParameterTypes)] = constructorSchema;
                        if (constructorSchema.ParameterTypes.Length == 0)
                        {
                            SetBit(Bits.HasDefaultConstructor, true);
                            _defaultConstructorID = ID;
                        }
                        flag = idVerifier.RegisterID(ID);
                    }
                    else
                        constructorSchema.Dispose(this);
                    NativeApi.SpReleaseExternalObject(constructor);
                }
                else
                    ErrorManager.ReportError("NULL object returned from {0}", "IUIXType::GetConstructor");
            }
            return flag;
        }

        private bool QueryProperties(UIXIDVerifier idVerifier)
        {
            bool flag1 = false;
            uint count;
            if (CheckNativeReturn(NativeApi.SpQueryPropertyCount(_type, out count)))
            {
                bool flag2 = false;
                _properties = new Map<uint, DllPropertySchema>((int)count);
                for (uint index = 0; index < count; ++index)
                {
                    if (!LoadProperty(index, idVerifier))
                        flag2 = true;
                }
                flag1 = !flag2;
            }
            return flag1;
        }

        private bool LoadProperty(uint index, UIXIDVerifier idVerifier)
        {
            bool flag = false;
            IntPtr property;
            uint ID;
            if (CheckNativeReturn(NativeApi.SpGetPropertySchema(_type, index, out property, out ID)))
            {
                if (property != IntPtr.Zero)
                {
                    DllPropertySchema dllPropertySchema = new DllPropertySchema(this, ID);
                    flag = dllPropertySchema.Load(property);
                    if (flag)
                    {
                        _properties[ID] = dllPropertySchema;
                        flag = idVerifier.RegisterID(ID);
                    }
                    NativeApi.SpReleaseExternalObject(property);
                }
                else
                    ErrorManager.ReportError("NULL object returned from {0}", "IUIXType::GetProperty");
            }
            return flag;
        }

        private bool QueryMethods(UIXIDVerifier idVerifier)
        {
            bool flag1 = false;
            uint count;
            if (CheckNativeReturn(NativeApi.SpQueryMethodCount(_type, out count)))
            {
                bool flag2 = false;
                _methods = new Map<MethodSignatureKey, DllMethodSchema>((int)count);
                for (uint index = 0; index < count; ++index)
                {
                    if (!LoadMethod(index, idVerifier))
                        flag2 = true;
                }
                flag1 = !flag2;
            }
            return flag1;
        }

        private bool LoadMethod(uint index, UIXIDVerifier idVerifier)
        {
            bool flag = false;
            IntPtr method;
            uint ID;
            if (CheckNativeReturn(NativeApi.SpGetMethodSchema(_type, index, out method, out ID)))
            {
                if (method != IntPtr.Zero)
                {
                    DllMethodSchema dllMethodSchema = new DllMethodSchema(this, ID);
                    flag = dllMethodSchema.Load(method);
                    if (flag)
                    {
                        _methods[new MethodSignatureKey(dllMethodSchema.Name, dllMethodSchema.ParameterTypes)] = dllMethodSchema;
                        flag = idVerifier.RegisterID(ID);
                    }
                    else
                        dllMethodSchema.Dispose(this);
                    NativeApi.SpReleaseExternalObject(method);
                }
                else
                    ErrorManager.ReportError("NULL object returned from {0}", "IUIXType::GetMethod");
            }
            return flag;
        }

        private bool QueryEvents(UIXIDVerifier idVerifier)
        {
            bool flag1 = false;
            uint count;
            if (CheckNativeReturn(NativeApi.SpQueryEventCount(_type, out count)))
            {
                bool flag2 = false;
                _events = new Map<uint, DllEventSchema>((int)count);
                for (uint index = 0; index < count; ++index)
                {
                    if (!LoadEvent(index, idVerifier))
                        flag2 = true;
                }
                flag1 = !flag2;
            }
            return flag1;
        }

        private bool QueryMarshalAs()
        {
            uint interopEquivalent;
            bool flag = CheckNativeReturn(NativeApi.SpGetMarshalAs(_type, out interopEquivalent));
            if (flag)
                flag = ValidateMarshalAs(interopEquivalent);
            if (flag)
                flag = RegisterMarshalAsEquivalents(interopEquivalent);
            return flag;
        }

        private bool RegisterMarshalAsEquivalents(uint declaredMarshalAs)
        {
            bool flag = true;
            switch (declaredMarshalAs)
            {
                case 4294967281:
                case 4294967286:
                case 4294967287:
                case 4294967288:
                case 4294967289:
                case 4294967290:
                case 4294967291:
                case uint.MaxValue:
                    return flag;
                case 4294967285:
                    RegisterOneWayEquivalence(this, ListSchema.Type);
                    goto case 4294967281;
                default:
                    ErrorManager.ReportError("Invalid MarshalAs '{0}' returned from IUIXType::MarshalAs", _marshalAs);
                    flag = false;
                    goto case 4294967281;
            }
        }

        private bool ValidateMarshalAs(uint declaredMarshalAs)
        {
            bool flag = true;
            uint num = uint.MaxValue;
            TypeSchema typeSchema = Base;
            DllTypeSchemaBase dllTypeSchemaBase;
            for (dllTypeSchemaBase = typeSchema as DllTypeSchemaBase; typeSchema != null && dllTypeSchemaBase == null; dllTypeSchemaBase = typeSchema as DllTypeSchemaBase)
                typeSchema = typeSchema.Base;
            if (dllTypeSchemaBase != null)
                num = dllTypeSchemaBase.MarshalAs;
            if (num != uint.MaxValue)
            {
                if (declaredMarshalAs != uint.MaxValue)
                {
                    ErrorManager.ReportError("Invalid MarshalAs '{0}' on type '{1}'.  Only a single MarshalAs value is permitted through a type heirarchy, and '{2}' was already declared.", declaredMarshalAs, Name, num);
                    flag = false;
                }
                else
                    declaredMarshalAs = num;
            }
            _marshalAs = declaredMarshalAs;
            return flag;
        }

        private bool LoadEvent(uint index, UIXIDVerifier idVerifier)
        {
            bool flag = false;
            IntPtr eventObj;
            uint ID;
            if (CheckNativeReturn(NativeApi.SpGetEventSchema(_type, index, out eventObj, out ID)))
            {
                if (eventObj != IntPtr.Zero)
                {
                    DllEventSchema dllEventSchema = new DllEventSchema(this, ID);
                    flag = dllEventSchema.Load(eventObj);
                    if (flag)
                    {
                        _events[ID] = dllEventSchema;
                        flag = idVerifier.RegisterID(ID);
                    }
                    else
                        dllEventSchema.Dispose(this);
                    NativeApi.SpReleaseExternalObject(eventObj);
                }
                else
                    ErrorManager.ReportError("NULL object returned from {0}", "IUIXType::GetEvent");
            }
            return flag;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            NativeApi.SpReleaseExternalObject(_type);
            _type = IntPtr.Zero;
        }

        public string MapChangeID(uint id)
        {
            string name = null;
            DllTypeSchema dllTypeSchema = this;
            while (dllTypeSchema != null && !dllTypeSchema.MapChangeIDWorker(id, out name))
                dllTypeSchema = dllTypeSchema._baseType as DllTypeSchema;
            if (name == null)
                ErrorManager.ReportError("ChangeNotification received for ID '0x{0:X8}' which isn't a property or event on '{1}'", id, Name);
            return name;
        }

        private bool MapChangeIDWorker(uint id, out string name)
        {
            bool flag = false;
            name = null;
            if ((int)UIXID.GetSchemaComponent(id) == OwnerLoadResult.SchemaComponent)
            {
                DllPropertySchema dllPropertySchema;
                if (_properties != null && _properties.TryGetValue(id, out dllPropertySchema))
                {
                    name = dllPropertySchema.Name;
                    flag = true;
                }
                else
                {
                    DllEventSchema dllEventSchema;
                    if (_events != null && _events.TryGetValue(id, out dllEventSchema))
                    {
                        name = dllEventSchema.Name;
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public override object ConstructDefault() => Construct(_defaultConstructorID, null);

        public unsafe object Construct(uint constructorID, object[] parameters)
        {
            DllProxyObject dllProxyObject = null;
            UIXVariant* uixVariantPtr = null;
            int count = 0;
            if (parameters != null)
            {
                count = parameters.Length;
                // ISSUE: untyped stack allocation
                var uixVariantStack = stackalloc UIXVariant[sizeof(UIXVariant) * count];
                uixVariantPtr = uixVariantStack;
                UIXVariant.MarshalObjectArray(parameters, uixVariantPtr);
            }
            ErrorWatermark watermark = ErrorManager.Watermark;
            IntPtr nativeObject;
            uint hr = NativeApi.SpInvokeConstructor(_type, constructorID, uixVariantPtr, (uint)count, out nativeObject);
            if (NativeApi.SUCCEEDED(hr))
                dllProxyObject = DllProxyObject.Wrap(nativeObject);
            else
                ReportError(hr, ErrorContext.Construct, this, watermark);
            if ((IntPtr)uixVariantPtr != IntPtr.Zero)
                UIXVariant.CleanupMarshalledObjects(uixVariantPtr, count);
            return dllProxyObject;
        }

        public object GetPropertyValue(object instance, DllPropertySchema property)
        {
            IntPtr nativeObject = IntPtr.Zero;
            if (!property.IsStatic)
                nativeObject = ((DllProxyObject)instance).NativeObject;
            ErrorWatermark watermark = ErrorManager.Watermark;
            object obj = null;
            UIXVariant propertyValue1;
            uint propertyValue2 = NativeApi.SpGetPropertyValue(_type, nativeObject, property.ID, out propertyValue1);
            if (NativeApi.SUCCEEDED(propertyValue2))
                obj = UIXVariant.GetValue(propertyValue1, Owner);
            else
                ReportError(propertyValue2, ErrorContext.PropertyGet, property, watermark);
            return obj;
        }

        public unsafe void SetPropertyValue(object instance, DllPropertySchema property, object value)
        {
            // ISSUE: untyped stack allocation
            UIXVariant* uixVariantPtr = stackalloc UIXVariant[sizeof(UIXVariant)];
            UIXVariant.MarshalObject(value, uixVariantPtr);
            IntPtr nativeObject = IntPtr.Zero;
            if (!property.IsStatic)
                nativeObject = ((DllProxyObject)instance).NativeObject;
            ErrorWatermark watermark = ErrorManager.Watermark;
            uint hr = NativeApi.SpSetPropertyValue(_type, nativeObject, property.ID, uixVariantPtr);
            if (NativeApi.FAILED(hr))
                ReportError(hr, ErrorContext.PropertySet, property, watermark);
            UIXVariant.CleanupMarshalledObject(uixVariantPtr);
        }

        public unsafe object InvokeMethod(object instance, DllMethodSchema method, object[] parameters)
        {
            object obj = null;
            UIXVariant* uixVariantPtr = null;
            int count = 0;
            if (parameters != null && parameters.Length > 0)
            {
                count = parameters.Length;
                // ISSUE: untyped stack allocation
                var uixVariantStack = stackalloc UIXVariant[sizeof(UIXVariant) * count];
                uixVariantPtr = uixVariantStack;
                UIXVariant.MarshalObjectArray(parameters, uixVariantPtr);
            }
            IntPtr nativeObject = IntPtr.Zero;
            if (!method.IsStatic)
                nativeObject = ((DllProxyObject)instance).NativeObject;
            ErrorWatermark watermark = ErrorManager.Watermark;
            UIXVariant returnValue;
            uint hr = NativeApi.SpInvokeMethod(_type, nativeObject, method.ID, uixVariantPtr, (uint)count, out returnValue);
            if (NativeApi.SUCCEEDED(hr))
                obj = UIXVariant.GetValue(returnValue, Owner);
            else
                ReportError(hr, ErrorContext.MethodInvoke, method, watermark);
            if ((IntPtr)uixVariantPtr != IntPtr.Zero)
                UIXVariant.CleanupMarshalledObjects(uixVariantPtr, count);
            return obj;
        }

        public string InvokeToString(DllProxyObject proxy)
        {
            string str = null;
            ErrorWatermark watermark = ErrorManager.Watermark;
            IntPtr nativeStringObject;
            uint hr = NativeApi.SpInvokeToString(_type, proxy.NativeObject, out nativeStringObject);
            if (NativeApi.SUCCEEDED(hr))
                str = DllProxyServices.GetString(nativeStringObject);
            else
                ReportError(hr, ErrorContext.ToString, this, watermark);
            return str;
        }

        private DllLoadResult OwnerLoadResult => (DllLoadResult)Owner;

        private void ReportError(
          uint hr,
          DllTypeSchema.ErrorContext contextType,
          object context,
          ErrorWatermark watermark)
        {
            if (hr == 2147774470U && watermark.ErrorsDetected)
                return;
            string message;
            switch (contextType)
            {
                case ErrorContext.MethodInvoke:
                    DllMethodSchema dllMethodSchema = (DllMethodSchema)context;
                    message = string.Format("Error 0x{0:X8} occurred invoking method {1}.{2} from {3}.", hr, dllMethodSchema.Owner.Name, dllMethodSchema.Name, dllMethodSchema.Owner.Owner.Uri);
                    break;
                case ErrorContext.ToString:
                    DllTypeSchema dllTypeSchema1 = (DllTypeSchema)context;
                    message = string.Format("Error 0x{0:X8} occurred invoking ToString on type {1} from {2}.", hr, dllTypeSchema1.Name, dllTypeSchema1.Owner.Uri);
                    break;
                case ErrorContext.PropertyGet:
                    DllPropertySchema dllPropertySchema1 = (DllPropertySchema)context;
                    message = string.Format("Error 0x{0:X8} occurred reading property {1}.{2} from {3}.", hr, dllPropertySchema1.Owner.Name, dllPropertySchema1.Name, dllPropertySchema1.Owner.Owner.Uri);
                    break;
                case ErrorContext.PropertySet:
                    DllPropertySchema dllPropertySchema2 = (DllPropertySchema)context;
                    message = string.Format("Error 0x{0:X8} occurred writing property {1}.{2} from {3}.", hr, dllPropertySchema2.Owner.Name, dllPropertySchema2.Name, dllPropertySchema2.Owner.Owner.Uri);
                    break;
                case ErrorContext.Construct:
                    DllTypeSchema dllTypeSchema2 = (DllTypeSchema)context;
                    message = string.Format("Error 0x{0:X8} occurred constructing object of type {1} from {2}.", hr, dllTypeSchema2.Name, dllTypeSchema2.Owner.Uri);
                    break;
                default:
                    message = "Internal error";
                    break;
            }
            ErrorManager.ReportError(message);
        }

        private bool CheckNativeReturn(uint hr) => DllLoadResult.CheckNativeReturn(hr, "IUIXType");

        private enum ErrorContext
        {
            MethodInvoke,
            ToString,
            PropertyGet,
            PropertySet,
            Construct,
        }
    }
}
