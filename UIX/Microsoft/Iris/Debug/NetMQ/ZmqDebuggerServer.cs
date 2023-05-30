using NetMQ;
using NetMQ.Sockets;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Iris.Debug.NetMQ;

internal class ZmqDebuggerServer : IDebuggerServer, IDisposable
{
    public static IDebuggerServer Current { get; private set; }

    private readonly PairSocket _socket;
    private readonly byte[][] _messageFrame = new byte[2][];
    private readonly IFormatter _formatter;

    public ZmqDebuggerServer(string connectionUri)
    {
        _socket = new(connectionUri ?? DebugRemoting.DEFAULT_TCP_SERVER_URI);

        _formatter = DebugRemoting.CreateBsonFormatter();

        Current = this;
    }

    public void LogInterpreterOpCode(object context, Data.InterpreterEntry entry)
    {
        using MemoryStream entryStream = new();
        _formatter.Serialize(entryStream, entry);

        SendDebuggerMessage(DebuggerMessageType.InterpreterOpCode, entryStream.ToArray());
    }

    public void LogDispatcher(string message)
    {
        SendDebuggerMessage(DebuggerMessageType.DispatcherStep, message);
    }

    public void Dispose() => _socket.Dispose();

    private void SendDebuggerMessage(DebuggerMessageType type, string message, Encoding encoding = null)
    {
        var messageBytes = (encoding ?? Encoding.Unicode).GetBytes(message);
        SendDebuggerMessage(type, messageBytes);
    }

    private void SendDebuggerMessage(DebuggerMessageType type, byte[] bytes)
    {
        _messageFrame[0] = BitConverter.GetBytes((int)type);
        _messageFrame[1] = bytes;

        _socket.SendMultipartBytes(_messageFrame);
    }
}
