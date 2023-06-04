using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

public class DebuggerMessageFrame
{
    public DebuggerMessageFrame() { }

    public DebuggerMessageFrame(long transactionId, DebuggerMessageType type, byte[] data)
    {
        TransactionId = transactionId;
        Type = type;
        Data = data;
    }

    public DebuggerMessageFrame(long transactionId, DebuggerMessageType type, string message, Encoding encoding = null)
        : this(transactionId, type, (encoding ?? Encoding.UTF8).GetBytes(message))
    {
    }

    public DebuggerMessageFrame(byte[] bytes)
    {
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

    public byte[] Data { get; set; }

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

    public string GetDataAsString(Encoding encoding = null) => (encoding ?? Encoding.UTF8).GetString(Data);

    public T DeserializeData<T>(IFormatter formatter)
    {
        using MemoryStream stream = new(Data);
        return (T)formatter.Deserialize(stream);
    }
}
