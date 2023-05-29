using Microsoft.Iris.Debug.Data;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Iris.Debug;

public class ZmqDebuggerClient : IDebuggerClient, IDisposable
{
    private List<byte[]> _frames = new(2);
    private readonly SubscriberSocket _subSocket;
    private readonly IFormatter _formatter;

    public string ConnectionUri { get; }

    public event EventHandler<InterpreterEntry> InterpreterStep;
    public event Action<string> DispatcherStep;

    public ZmqDebuggerClient(string connectionUri)
    {
        ConnectionUri = connectionUri;

        _subSocket = new();
        _subSocket.Connect(connectionUri);
        _subSocket.SubscribeToAnyTopic();

        _formatter = ZmqDebuggerServer.CreateFormatter();

        System.Threading.Thread th = new(MessageRecieveLoop);
        th.Start();
    }

    public void Dispose() => _subSocket.Dispose();

    private void MessageRecieveLoop()
    {
        while (!_subSocket.IsDisposed)
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
        if (!_subSocket.IsDisposed && _subSocket.TryReceiveMultipartBytes(ref _frames, 2))
        {
            type = (DebuggerMessageType)BitConverter.ToInt32(_frames[0], 0);
            bytes = _frames[1];
            return true;
        }

        type = default;
        bytes = null;
        return false;
    }
}
