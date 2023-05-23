using NetMQ;
using NetMQ.Sockets;
using System;
using System.Text;

namespace Microsoft.Iris.Debug;

internal class ZmqDebuggerServer : IDebuggerServer, IDisposable
{
    public static IDebuggerServer Current { get; private set; }

    private readonly PublisherSocket _pubSocket;
    private readonly byte[][] _messageFrame = new byte[2][];

    public ZmqDebuggerServer(string connectionUri)
    {
        _pubSocket = new();
        _pubSocket.Bind(connectionUri);

        Current = this;
    }

    public void LogInterpreterOpCode(object context, Data.InterpreterEntry entry)
    {
        SendDebuggerMessage(DebuggerMessageType.InterpreterOpCode, entry.ToString());
    }

    public void LogDispatcher(string message)
    {
        SendDebuggerMessage(DebuggerMessageType.DispatcherStep, message);
    }

    public void Dispose() => _pubSocket.Dispose();

    private void SendDebuggerMessage(DebuggerMessageType type, string message, Encoding encoding = null)
    {
        var messageBytes = (encoding ?? Encoding.Unicode).GetBytes(message);
        SendDebuggerMessage(type, messageBytes);
    }

    private void SendDebuggerMessage(DebuggerMessageType type, byte[] bytes)
    {
        _messageFrame[0] = BitConverter.GetBytes((int)type);
        _messageFrame[1] = bytes;

        _pubSocket.SendMultipartBytes(_messageFrame);
    }
}
