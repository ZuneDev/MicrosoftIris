// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIXTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System;

namespace Microsoft.Iris.Markup
{
    internal class UIXTypeSchema : TypeSchema
    {
        private short _typeID;
        private short _baseTypeID;
        private string _name;
        private string _alternateName;
        private TypeSchema _baseType;
        private DefaultConstructHandler _defaultConstructor;
        private ConstructorSchema[] _constructors;
        private PropertySchema[] _properties;
        private MethodSchema[] _methods;
        private EventSchema[] _events;
        private TypeConverterHandler _typeConverter;
        private SupportsTypeConversionHandler _supportsTypeConversion;
        private EncodeBinaryHandler _encodeBinary;
        private DecodeBinaryHandler _decodeBinary;
        private PerformOperationHandler _performOperation;
        private SupportsOperationHandler _supportsOperation;
        private FindCanonicalInstanceHandler _findCanonicalInstance;
        private Type _instanceType;
        private UIXTypeFlags _flags;
        private static object[] EmptyParameterList = new object[0];

        public UIXTypeSchema(
          short typeID,
          string name,
          string alternateName,
          short baseTypeID,
          Type instanceType,
          UIXTypeFlags flags)
          : base(MarkupSystem.UIXGlobal)
        {
            _typeID = typeID;
            _baseTypeID = baseTypeID;
            _name = name;
            _alternateName = alternateName;
            _instanceType = instanceType;
            _flags = flags;
            UIXTypes.RegisterTypeForID(typeID, this);
        }

        public void Initialize(
          DefaultConstructHandler defaultConstructor,
          ConstructorSchema[] constructors,
          PropertySchema[] properties,
          MethodSchema[] methods,
          EventSchema[] events,
          FindCanonicalInstanceHandler findCanonicalInstance,
          TypeConverterHandler typeConverter,
          SupportsTypeConversionHandler supportsTypeConversion,
          EncodeBinaryHandler encodeBinary,
          DecodeBinaryHandler decodeBinary,
          PerformOperationHandler performOperation,
          SupportsOperationHandler supportsOperation)
        {
            if (constructors == null)
                constructors = ConstructorSchema.EmptyList;
            if (properties == null)
                properties = PropertySchema.EmptyList;
            if (methods == null)
                methods = MethodSchema.EmptyList;
            if (events == null)
                events = EventSchema.EmptyList;
            _defaultConstructor = defaultConstructor;
            _constructors = constructors;
            _properties = properties;
            _methods = methods;
            _events = events;
            _findCanonicalInstance = findCanonicalInstance;
            _typeConverter = typeConverter;
            _supportsTypeConversion = supportsTypeConversion;
            _encodeBinary = encodeBinary;
            _decodeBinary = decodeBinary;
            _performOperation = performOperation;
            _supportsOperation = supportsOperation;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (DisposableObject constructor in _constructors)
                constructor.Dispose(this);
            foreach (DisposableObject property in _properties)
                property.Dispose(this);
            foreach (DisposableObject method in _methods)
                method.Dispose(this);
            foreach (DisposableObject disposableObject in _events)
                disposableObject.Dispose(this);
        }

        public override string Name => _name;

        public override string AlternateName => _alternateName;

        public override TypeSchema Base
        {
            get
            {
                if (_baseTypeID != -1 && _baseType == null)
                    _baseType = UIXTypes.MapIDToType(_baseTypeID);
                return _baseType;
            }
        }

        public override bool Contractual => false;

        public override Type RuntimeType => _instanceType;

        public override bool IsNativeAssignableFrom(object check) => RuntimeType.IsAssignableFrom(check.GetType());

        public override bool IsNativeAssignableFrom(TypeSchema checkSchema) => false;

        public override bool Disposable => (_flags & UIXTypeFlags.Disposable) != UIXTypeFlags.None;

        public override bool IsStatic => (_flags & UIXTypeFlags.Static) != UIXTypeFlags.None;

        public override object ConstructDefault() => _defaultConstructor();

        public override bool HasDefaultConstructor => _defaultConstructor != null;

        public override bool HasInitializer => false;

        public override void InitializeInstance(ref object instance)
        {
        }

        public override ConstructorSchema FindConstructor(TypeSchema[] parameters)
        {
            for (int index1 = 0; index1 < _constructors.Length; ++index1)
            {
                ConstructorSchema constructor = _constructors[index1];
                TypeSchema[] parameterTypes = constructor.ParameterTypes;
                if (parameters.Length == parameterTypes.Length)
                {
                    bool flag = true;
                    for (int index2 = 0; index2 < parameters.Length; ++index2)
                    {
                        if (!parameterTypes[index2].IsAssignableFrom(parameters[index2]))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                        return constructor;
                }
            }
            return null;
        }

        public override PropertySchema FindProperty(string name)
        {
            for (int index = 0; index < _properties.Length; ++index)
            {
                PropertySchema property = _properties[index];
                if (name == property.Name)
                    return property;
            }
            return null;
        }

        public override ConstructorSchema[] Constructors => _constructors;

        public override PropertySchema[] Properties => _properties;

        public override MethodSchema[] Methods => _methods;

        public override EventSchema[] Events => _events;

        public override MethodSchema FindMethod(string name, TypeSchema[] parameters)
        {
            for (int index1 = 0; index1 < _methods.Length; ++index1)
            {
                MethodSchema method = _methods[index1];
                if (name == method.Name)
                {
                    TypeSchema[] parameterTypes = method.ParameterTypes;
                    if (parameters.Length == parameterTypes.Length)
                    {
                        bool flag = true;
                        for (int index2 = 0; index2 < parameters.Length; ++index2)
                        {
                            if (!parameterTypes[index2].IsAssignableFrom(parameters[index2]))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                            return method;
                    }
                }
            }
            return null;
        }

        public override EventSchema FindEvent(string name)
        {
            for (int index = 0; index < _events.Length; ++index)
            {
                EventSchema eventSchema = _events[index];
                if (name == eventSchema.Name)
                    return eventSchema;
            }
            return null;
        }

        public override object FindCanonicalInstance(string name) => _findCanonicalInstance != null ? _findCanonicalInstance(name) : null;

        public override Result TypeConverter(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            return _typeConverter(from, fromType, out instance);
        }

        public override bool SupportsTypeConversion(TypeSchema fromType) => _supportsTypeConversion != null && _supportsTypeConversion(fromType);

        public override void EncodeBinary(ByteCodeWriter writer, object instance) => _encodeBinary(writer, instance);

        public override object DecodeBinary(ByteCodeReader reader) => _decodeBinary(reader);

        public override bool SupportsBinaryEncoding => _encodeBinary != null;

        public override int FindTypeHint => _typeID;

        public override object PerformOperation(object left, object right, OperationType op)
        {
            object obj = null;
            if (_performOperation != null)
                obj = _performOperation(left, right, op);
            return obj;
        }

        public override bool SupportsOperation(OperationType op) => _supportsOperation != null && _supportsOperation(op);

        public override bool IsNullAssignable => !_instanceType.IsValueType;

        public override bool IsRuntimeImmutable => (_flags & UIXTypeFlags.Immutable) != UIXTypeFlags.None;

        public override string ErrorContextDescription => Name;
    }
}
