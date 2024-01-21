using System;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug.Data;

public class DebuggerMessageFrame
{
    private readonly IFormatter _formatter;
    private object _value;
    private byte[] _data;

    public DebuggerMessageFrame(long transactionId, DebuggerMessageType type, IFormatter formatter)
    {
        _formatter = formatter;
        TransactionId = transactionId;
        Type = type;
    }

    public DebuggerMessageFrame(long transactionId, DebuggerMessageType type, object value, IFormatter formatter)
        : this(transactionId, type, formatter)
    {
        Value = value;
    }

    public DebuggerMessageFrame(long transactionId, DebuggerMessageType type, byte[] data, IFormatter formatter)
        : this(transactionId, type, formatter)
    {
        Data = data;
    }

    public DebuggerMessageFrame(byte[] bytes, IFormatter formatter)
    {
        _formatter = formatter;

        TransactionId = BitConverter.ToInt64(bytes, 0);
        Type = (DebuggerMessageType)BitConverter.ToInt32(bytes, sizeof(long));

        int dataOffset = sizeof(long) + sizeof(DebuggerMessageType);
#if NETCOREAPP2_1_OR_GREATER
        Data = bytes.AsSpan(dataOffset).ToArray();
#else
        Data = new byte[bytes.Length - dataOffset];
        bytes.CopyTo(Data, dataOffset);
#endif
    }

    public long TransactionId { get; set; }

    public DebuggerMessageType Type { get; set; }

    public object Value
    {
        get
        {
            if (_value is null)
            {
                if (_data is null) throw new ArgumentException("Either a value or data must be specified.");

                using MemoryStream stream = new(_data);
                _value = _formatter.Deserialize(stream);
            }

            return _value;
        }
        set
        {
            _value = value;
            _data = null;
        }
    }

    public byte[] Data
    {
        get
        {
            if (_data is null)
            {
                if (_value is null) throw new ArgumentException("Either a value or data must be specified.");

                _data = _value.Serialize(_formatter);
            }

            return _data;
        }
        set
        {
            _data = value;
            _value = default;
        }
    }

    public byte[] ToBytes()
    {
        byte[] bytes = new byte[Data.Length + sizeof(DebuggerMessageType) + sizeof(long)];

#if NET5_0_OR_GREATER
        var span = bytes.AsSpan();
        BitConverter.TryWriteBytes(span, TransactionId);
        BitConverter.TryWriteBytes(span[sizeof(long)..], (int)Type);
#else
        var idBytes = BitConverter.GetBytes(TransactionId);
        idBytes.CopyTo(bytes, 0);

        var typeBytes = BitConverter.GetBytes((int)Type);
        typeBytes.CopyTo(bytes, sizeof(long));
#endif

        Data.CopyTo(bytes, sizeof(long) + sizeof(DebuggerMessageType));

        return bytes;
    }

    public virtual T GetValue<T>(IFormatter formatter = null)
    {
        using MemoryStream stream = new(Data);
        return (T)(formatter ?? _formatter).Deserialize(stream);
    }

    public DebuggerMessageFrame<T> Deserialize<T>(IFormatter formatter = null)
        => new(TransactionId, Type, GetValue<T>(formatter), formatter ?? _formatter);
}

public class DebuggerMessageFrame<T> : DebuggerMessageFrame
{
    public DebuggerMessageFrame(byte[] bytes, IFormatter formatter) : base(bytes, formatter)
    {
    }

    public DebuggerMessageFrame(long transactionId, DebuggerMessageType type, IFormatter formatter) : base(transactionId, type, formatter)
    {
    }

    public DebuggerMessageFrame(long transactionId, DebuggerMessageType type, T value, IFormatter formatter) : base(transactionId, type, value, formatter)
    {
    }

    public DebuggerMessageFrame(long transactionId, DebuggerMessageType type, byte[] data, IFormatter formatter) : base(transactionId, type, data, formatter)
    {
    }

    public T GetValue() => (T)Value;
}
