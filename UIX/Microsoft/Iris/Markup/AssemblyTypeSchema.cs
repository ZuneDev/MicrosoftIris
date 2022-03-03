// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.AssemblyTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Iris.Markup
{
    internal class AssemblyTypeSchema : TypeSchema
    {
        private Type _type;
        private TypeSchema _baseType;
        private bool _isDisposable;
        private bool _notifiesOnChange;
        private bool _isFrameworkEnum;
        private Map<MethodSignatureKey, ConstructorSchema> _constructorCache = new Map<MethodSignatureKey, ConstructorSchema>();
        private Map<string, PropertySchema> _propertyCache = new Map<string, PropertySchema>();
        private Map<MethodSignatureKey, MethodSchema> _methodCache = new Map<MethodSignatureKey, MethodSchema>();
        private Map<string, EventSchema> _eventCache = new Map<string, EventSchema>();
        private Map<Type, ConstructorInfo[]> s_constructorInfosCache = new Map<Type, ConstructorInfo[]>();
        private Map<Type, MethodInfo[]> s_methodInfosCache = new Map<Type, MethodInfo[]>();

        protected AssemblyTypeSchema(Type type, TypeSchema baseType)
          : base(AssemblyLoadResult.MapAssembly(type.Assembly, type.Namespace))
        {
            _type = type;
            _baseType = baseType;
            _isFrameworkEnum = type.IsEnum && Enum.GetUnderlyingType(type) == typeof(int);
            _notifiesOnChange = typeof(INotifyPropertyChanged).IsAssignableFrom(_type);
            _isDisposable = typeof(IDisposable).IsAssignableFrom(_type);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (KeyValueEntry<MethodSignatureKey, ConstructorSchema> keyValueEntry in _constructorCache)
                keyValueEntry.Value.Dispose(this);
            foreach (KeyValueEntry<string, PropertySchema> keyValueEntry in _propertyCache)
                keyValueEntry.Value.Dispose(this);
            foreach (KeyValueEntry<MethodSignatureKey, MethodSchema> keyValueEntry in _methodCache)
                keyValueEntry.Value.Dispose(this);
            foreach (KeyValueEntry<string, EventSchema> keyValueEntry in _eventCache)
                keyValueEntry.Value.Dispose(this);
        }

        public override string Name
        {
            get
            {
                string str = _type.Name;
                if (_type.IsGenericType)
                    str = _type.ToString().Substring(((AssemblyLoadResult)Owner).Namespace.Length + 1);
                return str;
            }
        }

        public override string AlternateName => (string)null;

        public override TypeSchema Base
        {
            get
            {
                if (_baseType == null && this != AssemblyLoadResult.ObjectTypeSchema)
                {
                    if (_type.BaseType != null)
                        _baseType = AssemblyLoadResult.MapType(_type.BaseType);
                    else if (_type.IsInterface)
                        _baseType = AssemblyLoadResult.ObjectTypeSchema;
                }
                return _baseType;
            }
        }

        public override bool Contractual => _type.IsInterface;

        public override Type RuntimeType => _type;

        public Type InternalType => _type;

        public override bool IsNativeAssignableFrom(object check) => InternalType.IsAssignableFrom(AssemblyLoadResult.UnwrapObject(check).GetType());

        public override bool IsNativeAssignableFrom(TypeSchema checkSchema)
        {
            Type c = checkSchema.RuntimeType;
            if (checkSchema is AssemblyTypeSchema assemblyTypeSchema)
                c = assemblyTypeSchema.InternalType;
            return InternalType.IsAssignableFrom(c);
        }

        public override bool Disposable => _isDisposable;

        public override object ConstructDefault() => AssemblyLoadResult.WrapObject(this, Activator.CreateInstance(_type));

        public override bool HasDefaultConstructor => _type.IsValueType || _type.GetConstructor(Type.EmptyTypes) != null;

        public override bool HasInitializer => false;

        public override void InitializeInstance(ref object instance)
        {
        }

        public override ConstructorSchema FindConstructor(TypeSchema[] parameters)
        {
            ConstructorSchema constructorSchema;
            if (!_constructorCache.TryGetValue(new MethodSignatureKey(parameters), out constructorSchema))
            {
                ConstructorInfo[] constructors;
                if (!s_constructorInfosCache.TryGetValue(_type, out constructors))
                {
                    constructors = _type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                    s_constructorInfosCache[_type] = constructors;
                }
                if (constructors != null)
                {
                    foreach (ConstructorInfo constructorInfo in constructors)
                    {
                        TypeSchema[] schemaParameters = null;
                        if (CheckForMethodSignatureMatch(constructorInfo, null, parameters, out schemaParameters))
                        {
                            constructorSchema = new AssemblyConstructorSchema(this, constructorInfo, schemaParameters);
                            _constructorCache[new MethodSignatureKey(schemaParameters)] = constructorSchema;
                            break;
                        }
                    }
                }
            }
            return constructorSchema;
        }

        public override PropertySchema FindProperty(string name)
        {
            PropertySchema propertySchema;
            if (!_propertyCache.TryGetValue(name, out propertySchema))
            {
                PropertyInfo propertyHelper = GetPropertyHelper(_type, name);
                if (propertyHelper != null)
                {
                    propertySchema = new AssemblyPropertySchema(this, propertyHelper);
                    _propertyCache[name] = propertySchema;
                }
            }
            return propertySchema;
        }

        private static PropertyInfo GetPropertyHelper(Type type, string name)
        {
            PropertyInfo propertyInfo = null;
            try
            {
                propertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            }
            catch (AmbiguousMatchException ex)
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                for (int index = 0; index < properties.Length; ++index)
                {
                    if (properties[index].Name == name)
                    {
                        propertyInfo = properties[index];
                        break;
                    }
                }
            }
            if (propertyInfo == null && type.IsInterface)
            {
                foreach (Type type1 in type.GetInterfaces())
                {
                    propertyInfo = GetPropertyHelper(type1, name);
                    if (propertyInfo != null)
                        break;
                }
            }
            return propertyInfo;
        }

        public override MethodSchema FindMethod(string name, TypeSchema[] parameters)
        {
            MethodSchema methodSchema;
            if (!_methodCache.TryGetValue(new MethodSignatureKey(name, parameters), out methodSchema))
            {
                MethodInfo[] methodInfoArray = null;
                if (!s_methodInfosCache.TryGetValue(_type, out methodInfoArray))
                {
                    methodInfoArray = _type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                    s_methodInfosCache[_type] = methodInfoArray;
                }
                if (methodInfoArray != null)
                {
                    foreach (MethodInfo methodInfo in methodInfoArray)
                    {
                        TypeSchema[] schemaParameters;
                        if (CheckForMethodSignatureMatch(methodInfo, name, parameters, out schemaParameters))
                        {
                            methodSchema = new AssemblyMethodSchema(this, methodInfo, schemaParameters);
                            _methodCache[new MethodSignatureKey(name, schemaParameters)] = methodSchema;
                            break;
                        }
                    }
                }
            }
            return methodSchema;
        }

        private bool CheckForMethodSignatureMatch(
          MethodBase candidateMember,
          string name,
          TypeSchema[] parameters,
          out TypeSchema[] schemaParameters)
        {
            schemaParameters = null;
            if (name != null && candidateMember.Name != name)
                return false;
            ParameterInfo[] parameters1 = candidateMember.GetParameters();
            if (parameters1.Length != parameters.Length)
                return false;
            TypeSchema[] typeSchemaArray = new TypeSchema[parameters1.Length];
            for (int index = 0; index < parameters.Length; ++index)
            {
                typeSchemaArray[index] = AssemblyLoadResult.MapType(parameters1[index].ParameterType);
                if (!typeSchemaArray[index].IsAssignableFrom(parameters[index]))
                    return false;
            }
            schemaParameters = typeSchemaArray;
            return true;
        }

        public override EventSchema FindEvent(string name)
        {
            EventSchema eventSchema;
            if (!_eventCache.TryGetValue(name, out eventSchema))
            {
                EventInfo eventInfo = _type.GetEvent(name);
                if (eventInfo != null)
                {
                    eventSchema = new AssemblyEventSchema(this, eventInfo);
                    _eventCache[name] = eventSchema;
                }
            }
            return eventSchema;
        }

        public override object FindCanonicalInstance(string name)
        {
            if (_type.IsEnum)
            {
                try
                {
                    return Enum.Parse(_type, name);
                }
                catch (ArgumentException ex)
                {
                }
            }
            return null;
        }

        public override PropertySchema[] Properties => PropertySchema.EmptyList;

        public override Result TypeConverter(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            if (_isFrameworkEnum && Int32Schema.Type.IsAssignableFrom(fromType))
            {
                instance = Enum.ToObject(_type, (int)from);
                return Result.Success;
            }
            instance = null;
            return Result.Fail("Unimplemented");
        }

        public override bool SupportsTypeConversion(TypeSchema fromType) => _isFrameworkEnum && Int32Schema.Type.IsAssignableFrom(fromType);

        public override void EncodeBinary(ByteCodeWriter writer, object instance)
        {
            if (_type.IsEnum)
            {
                writer.WriteInt32((int)instance);
            }
            else
            {
                if (_type != typeof(Guid))
                    return;
                writer.Write(((Guid)instance).ToByteArray(), 16U);
            }
        }

        public override object DecodeBinary(ByteCodeReader reader)
        {
            object obj;
            if (_type.IsEnum)
                obj = Enum.ToObject(_type, reader.ReadInt32());
            else if (_type == typeof(Guid))
            {
                byte[] b = new byte[16];
                for (int index = 0; index < 16; ++index)
                    b[index] = reader.ReadByte();
                obj = new Guid(b);
            }
            else
                obj = null;
            return obj;
        }

        public override bool SupportsBinaryEncoding
        {
            get
            {
                if (_type == typeof(Guid))
                    return true;
                return _type.IsEnum && Enum.GetUnderlyingType(_type) == typeof(int);
            }
        }

        public override object PerformOperation(object left, object right, OperationType op)
        {
            bool flag = left != null ? left.Equals(right) : right == null || right.Equals(left);
            return BooleanBoxes.Box(op == OperationType.RelationalEquals ? flag : !flag);
        }

        public override bool SupportsOperation(OperationType op) => op == OperationType.RelationalEquals || op == OperationType.RelationalNotEquals;

        public override bool IsNullAssignable => !_type.IsValueType;

        public override int FindTypeHint => -1;

        public bool NotifiesOnChange => _notifiesOnChange;

        public override bool IsRuntimeImmutable => _type.IsEnum;

        public override bool IsEnum => _isFrameworkEnum;
    }
}
