// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.EnumSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using System;
using System.Diagnostics;

namespace Microsoft.Iris.Markup
{
    internal class EnumSchema : TypeSchema
    {
        private string _name;
        private Type _enumType;
        protected Map<string, int> _nameToValueMap;
        private bool _isFlags;
        private string[] _names;
        private int[] _values;

        public EnumSchema(LoadResult owner)
          : base(owner)
        {
        }

        protected void Initialize(
          string enumName,
          Type runtimeType,
          bool isFlags,
          string[] names,
          int[] values)
        {
            _name = enumName;
            _enumType = runtimeType;
            _isFlags = isFlags;
            _names = names;
            _values = values;
        }

        protected virtual void InitializeNameToValueMap()
        {
            _nameToValueMap = new Map<string, int>(_names.Length);
            for (int index = 0; index < _names.Length; ++index)
                _nameToValueMap[_names[index]] = _values[index];
            _names = null;
            _values = null;
        }

        [Conditional("DEBUG")]
        protected void DEBUG_AssertInitialized()
        {
        }

        public Vector<string> Names
        {
            get
            {
                Vector<string> vector = new Vector<string>();
                foreach (string key in NameToValueMap.Keys)
                    vector.Add(key);
                return vector;
            }
        }

        protected Map<string, int> NameToValueMap
        {
            get
            {
                if (_nameToValueMap == null)
                    InitializeNameToValueMap();
                return _nameToValueMap;
            }
        }

        protected bool IsFlags => _isFlags;

        public override string Name => _name;

        public override string AlternateName => (string)null;

        public override TypeSchema Base => ObjectSchema.Type;

        public override bool Contractual => false;

        public override Type RuntimeType => _enumType;

        public override bool IsNativeAssignableFrom(object check) => RuntimeType.IsAssignableFrom(check.GetType());

        public override bool IsNativeAssignableFrom(TypeSchema check) => false;

        public override bool Disposable => false;

        public override object ConstructDefault() => EnumValueToObject(0);

        public override bool HasDefaultConstructor => true;

        public override void InitializeInstance(ref object instance)
        {
        }

        public override bool HasInitializer => false;

        public override ConstructorSchema FindConstructor(TypeSchema[] parameters) => (ConstructorSchema)null;

        public override PropertySchema FindProperty(string name) => (PropertySchema)null;

        public override MethodSchema FindMethod(string name, TypeSchema[] parameters) => (MethodSchema)null;

        public override EventSchema FindEvent(string name) => (EventSchema)null;

        public override object FindCanonicalInstance(string name)
        {
            int num;
            return NameToValue(name, out num) ? EnumValueToObject(num) : null;
        }

        public bool NameToValue(string name, out int value) => NameToValueMap.TryGetValue(name, out value);

        protected virtual object EnumValueToObject(int value) => Enum.ToObject(_enumType, value);

        protected virtual int ValueFromObject(object obj) => (int)obj;

        public override PropertySchema[] Properties => PropertySchema.EmptyList;

        private Result ConvertFromString(string value, out object instance)
        {
            instance = null;
            int num1 = 0;
            if (_isFlags && value.IndexOf(',') >= 0)
            {
                foreach (string name in StringUtility.SplitAndTrim(',', value))
                {
                    int num2;
                    if (!NameToValue(name, out num2))
                        return Result.Fail("Unable to convert \"{0}\" to type '{1}'", name, _name);
                    num1 |= num2;
                }
            }
            else if (!NameToValue(value, out num1))
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", value, _name);
            instance = EnumValueToObject(num1);
            return Result.Success;
        }

        public override Result TypeConverter(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result;
            if (StringSchema.Type.IsAssignableFrom(fromType))
                result = ConvertFromString((string)from, out instance);
            else if (Int32Schema.Type.IsAssignableFrom(fromType))
            {
                instance = EnumValueToObject((int)from);
                result = Result.Success;
            }
            else
            {
                instance = null;
                result = Result.Fail("Unsupported");
            }
            return result;
        }

        public override bool SupportsTypeConversion(TypeSchema fromType) => StringSchema.Type.IsAssignableFrom(fromType) || Int32Schema.Type.IsAssignableFrom(fromType);

        public override void EncodeBinary(ByteCodeWriter writer, object instance) => writer.WriteInt32(ValueFromObject(instance));

        public override object DecodeBinary(ByteCodeReader reader) => EnumValueToObject(reader.ReadInt32());

        public override bool SupportsBinaryEncoding => true;

        public override object PerformOperation(object left, object right, OperationType op)
        {
            bool flag = Equals(left, right);
            switch (op)
            {
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(flag);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(!flag);
                default:
                    return null;
            }
        }

        public override bool SupportsOperation(OperationType op)
        {
            switch (op)
            {
                case OperationType.RelationalEquals:
                case OperationType.RelationalNotEquals:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsNullAssignable => false;

        public override bool IsRuntimeImmutable => true;

        public override bool IsEnum => true;

        public override int FindTypeHint => -1;
    }
}
