using Microsoft.Iris.Debug.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Iris.Debug.SystemNet;

public class NetDebuggerClient : IDebuggerClient, IDisposable
{
    public Uri ConnectionUri { get; }

    private readonly Socket _socket;
    private readonly Queue<byte[]> _queue;
    private readonly IFormatter _formatter;

    public event EventHandler<InterpreterEntry> InterpreterStep;
    public event Action<string> DispatcherStep;

    public NetDebuggerClient(string connectionUri) : this(new Uri(connectionUri))
    {
    }

    public NetDebuggerClient(Uri connectionUri)
    {
        ConnectionUri = connectionUri ?? DebugRemoting.DEFAULT_TCP_URI;
        _socket = new(SocketType.Stream, ProtocolType.Tcp);
        _formatter = DebugRemoting.CreateBsonFormatter();

        System.Threading.Thread connectThread = new(ConnectLoop);
        connectThread.Start();
    }

    public void Dispose() => _socket.Dispose();

    private void QueueDebuggerMessage(DebuggerMessageFrame frame)
    {
        _queue.Enqueue(frame.ToBytes());
    }

    private void MessageRecieveLoop()
    {
        while (_socket.Connected)
        {
            DebuggerMessageFrame frame;
            while ((frame = DebugRemoting.ReceiveDebuggerMessage(_socket)) == null)
                if (!_socket.Connected) return;

            switch (frame.Type)
            {
                case DebuggerMessageType.InterpreterOpCode:
                    {
                        using MemoryStream stream = new(frame.Data);
                        var entry = (InterpreterEntry)_formatter.Deserialize(stream);
                        InterpreterStep?.Invoke(this, entry);
                    }
                    break;

                case DebuggerMessageType.DispatcherStep:
                    string message = frame.GetDataAsString();
                    DispatcherStep?.Invoke(message);
                    break;

                default:
                    Trace.WriteLine(TraceCategory.MarkupDebug, "Recieved unknown debugger message of type '{0}'.", frame.Type);
                    break;
            }
        }
    }

    private void ConnectLoop()
    {
        while (!_socket.Connected)
        {
            try
            {
                var endpoint = new IPEndPoint(IPAddress.Parse(ConnectionUri.Host), ConnectionUri.Port);
                _socket.Connect(endpoint);
            }
            catch { }
        }

        System.Threading.Thread receiveThread = new(MessageRecieveLoop);
        receiveThread.Start();
    }
}