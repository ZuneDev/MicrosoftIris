using Microsoft.Iris.Debug;
using Microsoft.Iris.Debug.Data;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Iris.Debug.NetMQ;

public class ZmqDebuggerClient : IDebuggerClient, IDisposable
{
    private List<byte[]> _frames = new(2);
    private readonly PairSocket _socket;
    private readonly IFormatter _formatter;

    public string ConnectionUri { get; }

    public event EventHandler<InterpreterEntry> InterpreterStep;
    public event Action<string> DispatcherStep;

    public ZmqDebuggerClient(string connectionUri)
    {
        ConnectionUri = connectionUri ?? DebugRemoting.DEFAULT_TCP_CLIENT_URI;

        _socket = new();
        _socket.Connect(connectionUri);

        _formatter = DebugRemoting.CreateBsonFormatter();

        System.Threading.Thread th = new(MessageRecieveLoop);
        th.Start();
    }

    public void Dispose() => _socket.Dispose();

    private void MessageRecieveLoop()
    {
        while (!_socket.IsDisposed)
        {
            DebuggerMessageType type;
            byte[] bytes;
            while (!TryRecieveDebuggerMessage(out type, out bytes))
                ;

            switch (type)
            {
                case DebuggerMessageType.InterpreterOpCode:
                    {
                        using MemoryStream stream = new(bytes);
                        var entry = (InterpreterEntry)_formatter.Deserialize(stream);
                        InterpreterStep?.Invoke(this, entry);
                    }
                    break;

                case DebuggerMessageType.DispatcherStep:
                    string message = Encoding.Unicode.GetString(bytes);
                    DispatcherStep?.Invoke(message);
                    break;
            }
        }
    }

    private bool TryRecieveDebuggerMessage(out DebuggerMessageType type, out byte[] bytes)
    {
        if (!_socket.IsDisposed && _socket.TryReceiveMultipartBytes(ref _frames, 2))
        {
            type = (DebuggerMessageType)BitConverter.ToInt32(_frames[0], 0);
            bytes = _frames[1];
            return true;
        }

        type = default;
        bytes = null;
        return false;
    }

    internal static IFormatter CreateFormatter() => new BsonFormatter(new StreamingContext(StreamingContextStates.Remoting));
}
