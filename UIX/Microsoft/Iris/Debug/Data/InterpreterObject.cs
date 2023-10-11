using System;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterObject
{
    private object _value;

    public string Name { get; set; }

    public Type Type { get; set; }

    public object Value
    {
        get => _value;
        set
        {
            if (value is byte[] bytes)
            {
                if (Type == typeof(uint))
                    _value = BitConverter.ToUInt32(bytes, 0);
                else if (Type == typeof(ulong))
                    _value = BitConverter.ToUInt64(bytes, 0);
            }
            else
            {
                _value = value;
            }
        }
    }

    public InstructionObjectSource Source { get; set; }

    public int TableIndex { get; set; } = -1;

    public InterpreterObject(string name, Type type, object value, InstructionObjectSource source, int tableIndex = -1)
    {
        Name = name;
        Type = type;
        Value = value;
        Source = source;
        TableIndex = tableIndex;
    }

    public InterpreterObject(object value) : this(null, value?.GetType(), value, InstructionObjectSource.Dynamic)
    {
    }

    public InterpreterObject() { }

    public override string ToString()
    {
        StringBuilder sb = new();
        ToString(sb, true);
        return sb.ToString();
    }

    public void ToString(StringBuilder sb, bool includeType = false, bool includeSource = false)
    {
        if (includeType)
        {
            sb.Append(Type?.Name);
            sb.Append(' ');
        }

        if (includeSource)
        {
            if (Source == InstructionObjectSource.Inline)
                sb.Append(Value ?? "NULL");
            else
                sb.Append(Source);

            if (TableIndex != -1)
            {
                sb.Append('[');
                sb.Append(TableIndex);
                sb.Append(']');
            }
        }
        else
        {
            sb.Append(Value ?? "NULL");
        }
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
