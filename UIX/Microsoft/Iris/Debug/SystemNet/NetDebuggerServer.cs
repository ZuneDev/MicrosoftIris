using Microsoft.Iris.Debug.Data;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug.SystemNet;

internal class NetDebuggerServer : IDebuggerServer, IDisposable
{
    public static IDebuggerServer Current { get; private set; }

    public Uri ConnectionUri { get; }

    private readonly ConcurrentQueue<byte[]> _outQueue = new();
    private readonly TcpListener _listener;
    private readonly IFormatter _formatter;
    private Socket _socket;
    private bool _disposed = false;

    public NetDebuggerServer(string connectionUri) : this(new Uri(connectionUri))
    {
    }

    public NetDebuggerServer(Uri connectionUri)
    {
        ConnectionUri = connectionUri ?? DebugRemoting.DEFAULT_TCP_URI;

        _listener = TcpListener.Create(connectionUri.Port);
        _listener.Start();

        _formatter = DebugRemoting.CreateBsonFormatter();

        System.Threading.Thread connectThread = new(ConnectLoop);
        connectThread.Start();

        Current = this;
    }

    public void LogInterpreterOpCode(object context, InterpreterEntry entry)
    {
        using MemoryStream entryStream = new();
        _formatter.Serialize(entryStream, entry);

        QueueDebuggerMessage(new(0, DebuggerMessageType.InterpreterOpCode, entryStream.ToArray()));
    }

    public void LogDispatcher(string message)
    {
        QueueDebuggerMessage(new(0, DebuggerMessageType.DispatcherStep, message));
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _listener.Stop();
        _socket?.Dispose();
    }

    private void QueueDebuggerMessage(DebuggerMessageFrame frame)
    {
        _outQueue.Enqueue(frame.ToBytes());
    }

    private void MessageRecieveLoop()
    {
        while (_socket.Connected)
        {
            DebuggerMessageFrame frame;
            while ((frame = DebugRemoting.ReceiveDebuggerMessage(_socket)) == null)
                ;

            switch (frame.Type)
            {
                default:
                    Trace.WriteLine(TraceCategory.MarkupDebug, "Recieved unknown debugger message of type '{0}'.", frame.Type);
                    break;
            }
        }
    }

    private void MessageSendLoop()
    {
        while (_socket.Connected)
        {
            byte[] frameBytes;
            while (!_outQueue.TryDequeue(out frameBytes)) ;

            _socket.Send(BitConverter.GetBytes(frameBytes.Length));
            _socket.Send(frameBytes);
        }
    }

    private void ConnectLoop()
    {
        while (!_listener.Pending())
            if (_disposed) return;

        _socket = _listener.AcceptSocket();

        System.Threading.Thread receiveThread = new(MessageRecieveLoop);
        System.Threading.Thread sendThread = new(MessageSendLoop);
        receiveThread.Start();
        sendThread.Start();
    }
}
