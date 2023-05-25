using System;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterObject : ISerializable
{
    public string Name { get; set; }

    public Type Type { get; set; }

    public object Value { get; set; }

    public InstructionObjectSource Source { get; set; }

    public int TableIndex { get; set; } = -1;

    protected bool CanSerializeValue => Value is ISerializable;

    public InterpreterObject(string name, Type type, object value, InstructionObjectSource source, int tableIndex = -1)
    {
        Name = name;
        Type = type;
        Value = value;
        Source = source;
        TableIndex = tableIndex;
    }

    public InterpreterObject(object value) : this(null, value?.GetType() ?? typeof(object), value, InstructionObjectSource.Dynamic)
    {
    }

    protected InterpreterObject(SerializationInfo info, StreamingContext context)
    {
        Name = info.GetString(nameof(Name));
        Type = Type.GetType(info.GetString(nameof(Type)), false);
        Source = (InstructionObjectSource)info.GetByte(nameof(Source));
        TableIndex = info.GetInt32(nameof(TableIndex));

        bool canSerializeValue = info.GetBoolean(nameof(CanSerializeValue));
        Type valueSerializeType = canSerializeValue ? Type : typeof(string);
        Value = info.GetValue(nameof(Value), valueSerializeType);
    }

    public override string ToString() => $"{Type} {Value}";

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Name), Name);
        info.AddValue(nameof(Type), Type.FullName);
        info.AddValue(nameof(Source), (byte)Source);
        info.AddValue(nameof(TableIndex), TableIndex);
        info.AddValue(nameof(CanSerializeValue), CanSerializeValue);
        info.AddValue(nameof(Value), CanSerializeValue ? Value : Value?.ToString());
    }
}

/// <summary>
/// Indicates where the argument value came from.
/// </summary>
public enum InstructionObjectSource : byte
{
    Dynamic,
    Stack,
    Inline,
    Exports,
    Alias,
    DataMappings,
    BinaryData,
    Constants,
    SymbolReference,

    Imports = 1 << 4,
    TypeImports = Imports | 1,
    ConstructorImports = Imports | 2,
    PropertyImports = Imports | 3,
    MethodImports = Imports | 4,
    EventImports = Imports | 5,
}
