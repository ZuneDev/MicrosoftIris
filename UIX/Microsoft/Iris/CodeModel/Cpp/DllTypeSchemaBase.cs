// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllTypeSchemaBase
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using System;
using System.Collections.Specialized;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllTypeSchemaBase : TypeSchema
    {
        private BitVector32 _bits;
        protected uint _typeID;
        protected Map<MethodSignatureKey, DllConstructorSchema> _constructors;
        protected Map<uint, DllPropertySchema> _properties;
        protected Map<MethodSignatureKey, DllMethodSchema> _methods;
        protected Map<uint, DllEventSchema> _events;
        protected string _name;
        protected TypeSchema _baseType;
        protected uint _marshalAs;

        protected DllTypeSchemaBase(DllLoadResult owner, uint ID)
          : base(owner)
          => _typeID = ID;

        protected override void OnDispose()
        {
            base.OnDispose();
            if (_constructors != null)
            {
                foreach (KeyValueEntry<MethodSignatureKey, DllConstructorSchema> constructor in _constructors)
                {
                    if (constructor.Value != null)
                        constructor.Value.Dispose(this);
                }
                _constructors = null;
            }
            if (_properties != null)
            {
                foreach (DllPropertySchema dllPropertySchema in _properties.Values)
                    dllPropertySchema?.Dispose(this);
                _properties = null;
            }
            if (_methods != null)
            {
                foreach (KeyValueEntry<MethodSignatureKey, DllMethodSchema> method in _methods)
                {
                    if (method.Value != null)
                        method.Value.Dispose(this);
                }
                _methods = null;
            }
            if (_events == null)
                return;
            foreach (DllEventSchema dllEventSchema in _events.Values)
                dllEventSchema?.Dispose(this);
        }

        public override object ConstructDefault() => (object)null;

        public override int FindTypeHint => (int)_typeID;

        public override bool HasDefaultConstructor => GetBit(Bits.HasDefaultConstructor);

        public override bool IsRuntimeImmutable => GetBit(Bits.IsRuntimeImmutable);

        public override ConstructorSchema FindConstructor(TypeSchema[] parameters)
        {
            DllConstructorSchema constructorSchema = null;
            if (_constructors != null)
                _constructors.TryGetValue(new MethodSignatureKey(parameters), out constructorSchema);
            return constructorSchema;
        }

        public override PropertySchema FindProperty(string name)
        {
            if (_properties != null)
            {
                foreach (DllPropertySchema dllPropertySchema in _properties.Values)
                {
                    if (dllPropertySchema != null && dllPropertySchema.Name == name)
                        return dllPropertySchema;
                }
            }
            if (Equivalents != null)
            {
                foreach (TypeSchema equivalent in Equivalents)
                {
                    PropertySchema property = equivalent.FindProperty(name);
                    if (property != null)
                        return property;
                }
            }
            return null;
        }

        public override MethodSchema FindMethod(string name, TypeSchema[] parameters)
        {
            if (_methods != null)
            {
                DllMethodSchema dllMethodSchema = null;
                if (_methods.TryGetValue(new MethodSignatureKey(name, parameters), out dllMethodSchema))
                    return dllMethodSchema;
            }
            if (Equivalents != null)
            {
                foreach (TypeSchema equivalent in Equivalents)
                {
                    MethodSchema method = equivalent.FindMethod(name, parameters);
                    if (method != null)
                        return method;
                }
            }
            return null;
        }

        public override EventSchema FindEvent(string name)
        {
            DllEventSchema dllEventSchema1 = null;
            if (_events != null)
            {
                foreach (DllEventSchema dllEventSchema2 in _events.Values)
                {
                    if (dllEventSchema2 != null && dllEventSchema2.Name == name)
                    {
                        dllEventSchema1 = dllEventSchema2;
                        break;
                    }
                }
            }
            return dllEventSchema1;
        }

        public override PropertySchema[] Properties
        {
            get
            {
                PropertySchema[] propertySchemaArray = PropertySchema.EmptyList;
                if (_properties != null && _properties.Count > 0)
                {
                    propertySchemaArray = new PropertySchema[_properties.Count];
                    int num = 0;
                    foreach (PropertySchema propertySchema in _properties.Values)
                        propertySchemaArray[num++] = propertySchema;
                }
                return propertySchemaArray;
            }
        }

        public uint MarshalAs => _marshalAs;

        public override string Name => _name;

        public override string AlternateName => (string)null;

        public override TypeSchema Base => _baseType;

        public override Type RuntimeType => DllLoadResult.RuntimeTypeForMarshalAs(_marshalAs);

        public override Result TypeConverter(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            instance = null;
            return Result.Fail("Not implemented");
        }

        public override bool SupportsTypeConversion(TypeSchema fromType) => false;

        public override void EncodeBinary(ByteCodeWriter writer, object instance)
        {
        }

        public override object DecodeBinary(ByteCodeReader reader) => (object)null;

        public override bool SupportsBinaryEncoding => false;

        public override object PerformOperation(object left, object right, OperationType op) => (object)null;

        public override bool SupportsOperation(OperationType op) => false;

        public override bool IsNullAssignable => true;

        public override object FindCanonicalInstance(string name) => (object)null;

        public override void InitializeInstance(ref object instance)
        {
        }

        public override bool HasInitializer => false;

        public override bool Contractual => false;

        public override bool IsNativeAssignableFrom(object check) => false;

        public override bool IsNativeAssignableFrom(TypeSchema check) => false;

        public override bool Disposable => true;

        protected bool GetBit(DllTypeSchemaBase.Bits lookupBit) => _bits[(int)lookupBit];

        protected void SetBit(DllTypeSchemaBase.Bits changeBit, bool value) => _bits[(int)changeBit] = value;

        protected enum Bits : uint
        {
            HasDefaultConstructor = 1,
            IsRuntimeImmutable = 2,
        }
    }
}
