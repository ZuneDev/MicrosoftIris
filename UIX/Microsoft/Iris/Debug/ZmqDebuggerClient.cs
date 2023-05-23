using Microsoft.Iris.Debug.Data;
using NetMQ;
using NetMQ.Sockets;
using System;
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
                    string entry = Encoding.Unicode.GetString(bytes);
                    InterpreterStep?.Invoke(this, null);
                    break;

                case DebuggerMessageType.DispatcherStep:
                    string message = Encoding.Unicode.GetString(bytes);
                    DispatcherStep?.Invoke(message);
                    break;
            }
        }
    }

    private DebuggerMessageType RecieveDebuggerMessage(SubscriberSocket socket, out byte[] bytes)
    {
        var frames = socket.ReceiveMultipartBytes(2);

        bytes = frames[1];
        return (DebuggerMessageType)BitConverter.ToInt32(frames[0], 0);
    }
}
