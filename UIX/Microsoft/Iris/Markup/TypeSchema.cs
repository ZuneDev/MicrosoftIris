// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.TypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using System;
using System.Linq;

namespace Microsoft.Iris.Markup
{
    public abstract class TypeSchema : DisposableObject
    {
        private readonly LoadResult _owner;
        private readonly ulong _id;
        private Vector<TypeSchema> _equivalents;
        private static ulong s_uniqueId;
        private readonly static Map<ulong, TypeSchema> s_idToTypeSchema = new();
        public readonly static TypeSchema[] EmptyList = Array.Empty<TypeSchema>();

        public TypeSchema(LoadResult owner)
        {
            _owner = owner;
            _id = ++s_uniqueId;
            s_idToTypeSchema[_id] = this;
            DeclareOwner(owner);
        }

        protected override void OnDispose()
        {
            s_idToTypeSchema.Remove(_id);
            base.OnDispose();
        }

        public LoadResult Owner => _owner;

        public Vector<TypeSchema> Equivalents => _equivalents;

        public abstract string Name { get; }

        public abstract string AlternateName { get; }

        public abstract TypeSchema Base { get; }

        public abstract bool Contractual { get; }

        public abstract Type RuntimeType { get; }

        public abstract bool IsNativeAssignableFrom(object check);

        public abstract bool IsNativeAssignableFrom(TypeSchema check);

        public abstract bool Disposable { get; }

        public abstract object ConstructDefault();

        public abstract bool HasDefaultConstructor { get; }

        public abstract void InitializeInstance(ref object instance);

        public abstract bool HasInitializer { get; }

        public abstract ConstructorSchema FindConstructor(TypeSchema[] parameters);

        public abstract PropertySchema FindProperty(string name);

        public abstract MethodSchema FindMethod(string name, TypeSchema[] parameters);

        public abstract EventSchema FindEvent(string name);

        public abstract object FindCanonicalInstance(string name);

        public virtual ConstructorSchema[] Constructors => (ConstructorSchema[])null;

        public abstract PropertySchema[] Properties { get; }

        public virtual MethodSchema[] Methods => (MethodSchema[])null;

        public virtual EventSchema[] Events => (EventSchema[])null;

        public abstract Result TypeConverter(
          object from,
          TypeSchema fromType,
          out object instance);

        public abstract bool SupportsTypeConversion(TypeSchema fromType);

        public abstract void EncodeBinary(ByteCodeWriter writer, object instance);

        public abstract object DecodeBinary(ByteCodeReader reader);

        public abstract bool SupportsBinaryEncoding { get; }

        public abstract object PerformOperation(object left, object right, OperationType op);

        public abstract bool SupportsOperation(OperationType op);

        public abstract bool IsNullAssignable { get; }

        public abstract bool IsRuntimeImmutable { get; }

        public virtual bool IsEnum => false;

        public virtual bool IsStatic => false;

        public abstract int FindTypeHint { get; }

        public bool IsAssignableFrom(object check)
        {
            if (check == null)
                return IsNullAssignable;
            return check is ISchemaInfo schemaInfo ? IsAssignableFrom(schemaInfo.TypeSchema) : IsNativeAssignableFrom(check);
        }

        public bool IsAssignableFrom(TypeSchema checkSchema)
        {
            if (checkSchema == null)
                return false;
            if (checkSchema == NullSchema.Type && IsNullAssignable)
                return true;
            if (!Contractual)
            {
                for (TypeSchema typeSchema = checkSchema; typeSchema != null; typeSchema = typeSchema.Base)
                {
                    if (this == typeSchema)
                        return true;
                    if (typeSchema.Equivalents != null)
                    {
                        foreach (TypeSchema equivalent in typeSchema.Equivalents)
                        {
                            if (equivalent == this)
                                return true;
                        }
                    }
                }
            }
            return IsNativeAssignableFrom(checkSchema);
        }

        public PropertySchema FindPropertyDeep(string name)
        {
            for (TypeSchema typeSchema = this; typeSchema != null; typeSchema = typeSchema.Base)
            {
                PropertySchema property = typeSchema.FindProperty(name);
                if (property != null)
                    return property;
            }
            return null;
        }

        public Vector<string> FindRequiredPropertyNamesDeep()
        {
            Vector<string> list = new Vector<string>();
            FindRequiredPropertyNamesDeep(list);
            return list;
        }

        private void FindRequiredPropertyNamesDeep(Vector<string> list)
        {
            if (Base != null)
                Base.FindRequiredPropertyNamesDeep(list);
            foreach (PropertySchema property in Properties)
            {
                int index = list.IndexOf(property.Name);
                if (index != -1)
                {
                    if (!property.RequiredForCreation)
                        list.RemoveAt(index);
                }
                else if (property.RequiredForCreation)
                    list.Add(property.Name);
            }
        }

        public MethodSchema FindMethodDeep(string name, TypeSchema[] parameters)
        {
            for (TypeSchema typeSchema = this; typeSchema != null; typeSchema = typeSchema.Base)
            {
                MethodSchema method = typeSchema.FindMethod(name, parameters);
                if (method != null)
                    return method;
            }
            return null;
        }

        public EventSchema FindEventDeep(string name)
        {
            for (TypeSchema typeSchema = this; typeSchema != null; typeSchema = typeSchema.Base)
            {
                EventSchema eventSchema = typeSchema.FindEvent(name);
                if (eventSchema != null)
                    return eventSchema;
            }
            return null;
        }

        public bool SupportsOperationDeep(OperationType op)
        {
            for (TypeSchema typeSchema = this; typeSchema != null; typeSchema = typeSchema.Base)
            {
                if (typeSchema.SupportsOperation(op))
                    return true;
            }
            return false;
        }

        public object PerformOperationDeep(object left, object right, OperationType op)
        {
            for (TypeSchema typeSchema = this; typeSchema != null; typeSchema = typeSchema.Base)
            {
                if (typeSchema.SupportsOperation(op))
                    return typeSchema.PerformOperation(left, right, op);
            }
            return null;
        }

        public static bool IsUnaryOperation(OperationType op) => op == OperationType.LogicalNot || op == OperationType.MathNegate || op == OperationType.PostIncrement || op == OperationType.PostDecrement;

        public static void RegisterOneWayEquivalence(TypeSchema producer, TypeSchema consumer)
        {
            if (producer._equivalents == null)
                producer._equivalents = new Vector<TypeSchema>();
            producer._equivalents.Add(consumer);
        }

        public static void RegisterTwoWayEquivalence(TypeSchema typeA, TypeSchema typeB)
        {
            RegisterOneWayEquivalence(typeA, typeB);
            RegisterOneWayEquivalence(typeB, typeA);
        }

        public void ShareEquivalents(Vector<TypeSchema> equivalents) => _equivalents = equivalents;

        public virtual string ErrorContextDescription => $"{Name} (Owner='{Owner.Uri ?? "Unavailable"}')";

        public static string NameFromInstance(object instance) => !(instance is ISchemaInfo schemaInfo) ? instance.GetType().Name : schemaInfo.TypeSchema.Name;

        public ulong UniqueId => _id;

        public static TypeSchema LookupById(ulong id)
        {
            TypeSchema typeSchema;
            s_idToTypeSchema.TryGetValue(id, out typeSchema);
            return typeSchema;
        }

        private static bool IsBasicType(TypeSchema type) =>
            type == null || type.RuntimeType.Assembly.FullName.Contains("CoreLib") || type.Properties.Length == 0;

        public static void GenerateModelCode(TypeSchema dataType)
        {
            string code =
                "using Microsoft.Iris.Markup;\r\n\r\n" +
                "namespace Microsoft.Zune.Schemas;\r\n\r\n" +
                "public class " + dataType.Name;

            if (!IsBasicType(dataType.Base))
            {
                code += $" : {dataType.Base.Name}";
                GenerateModelCode(dataType.Base);
            }
            else
            {
                code += $" : MarkupDataType";
            }

            code += "\r\n{\r\n";
            code += $"    public {dataType.Name}(MarkupTypeSchema schema) : base(schema)\r\n";
            code += "    {\r\n";
            code += "    }\r\n\r\n";

            foreach (var prop in dataType.Properties)
            {
                string propType = prop.PropertyType.AlternateName ?? prop.PropertyType.Name;

                code += $"    public {propType} {prop.Name}\r\n";
                code += "    {\r\n";
                code += $"        get => GetProperty<{propType}>();\r\n";
                code += $"        set => SetProperty(value);\r\n";
                code += "    }\r\n\r\n";

                if (!IsBasicType(prop.PropertyType) && !Application.DebugSettings.DataMappingModels.Any(m => m.Type == propType))
                    GenerateModelCode(prop.PropertyType);
            }

            code += "}";

            Application.DebugSettings.DataMappingModels.Add(new(dataType.RuntimeType.Name, dataType.Name, code));
        }

        public override string ToString() => $"typeof({Name})";
    }
}
