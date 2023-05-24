using Microsoft.Iris.Debug.Data;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Microsoft.Iris.Debug;

public class ZmqDebuggerClient : IDebuggerClient, IDisposable
{
    private readonly SubscriberSocket _subSocket;

    public event EventHandler<InterpreterEntry> InterpreterStep;
    public event Action<string> DispatcherStep;

    public ZmqDebuggerClient(string connectionUri)
    {
        _subSocket = new();
        _subSocket.Connect(connectionUri);
        _subSocket.SubscribeToAnyTopic();

        System.Threading.Thread th = new(MessageRecieveLoop);
        th.Start();
    }

    public void Dispose() => _subSocket.Dispose();

    private void MessageRecieveLoop()
    {
        while (!_subSocket.IsDisposed)
        {
            var type = RecieveDebuggerMessage(_subSocket, out var bytes);

            switch (type)
            {
                case DebuggerMessageType.InterpreterOpCode:
                    {
                        using MemoryStream stream = new(bytes);
                        var entry = (InterpreterEntry)new BinaryFormatter().Deserialize(stream);
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

    private static DebuggerMessageType RecieveDebuggerMessage(SubscriberSocket socket, out byte[] bytes)
    {
        var frames = socket.ReceiveMultipartBytes(2);

        bytes = frames[1];
        return (DebuggerMessageType)BitConverter.ToInt32(frames[0], 0);
    }
}
