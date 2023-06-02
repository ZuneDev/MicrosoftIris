using Microsoft.Iris.Debug.Data;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug.SystemNet;

internal class NetDebuggerServer : IDebuggerServer, IDisposable
{
    public static IDebuggerServer Current { get; private set; }

    private readonly ConcurrentQueue<byte[]> _outQueue = new();
    private readonly TcpListener _listener;
    private readonly IFormatter _formatter;
    private Socket _socket;
    private bool _disposed = false;
    private InterpreterCommand _uibCommand = InterpreterCommand.Continue;

    public Uri ConnectionUri { get; }

    public InterpreterCommand DebuggerCommand
    {
        get => _uibCommand;
        set
        {
            var data = new[] { (byte)value };
            QueueDebuggerMessage(new(0, DebuggerMessageType.InterpreterCommand, data));
            _uibCommand = value;
        }
    }

    public NetDebuggerServer(string connectionUri) : this(new Uri(connectionUri))
    {
    }

    public NetDebuggerServer(Uri connectionUri)
    {
        ConnectionUri = connectionUri ?? DebugRemoting.DEFAULT_TCP_URI;

        _listener = TcpListener.Create(connectionUri.Port);
        _listener.Start();

        _formatter = DebugRemoting.CreateBsonFormatter();

        System.Threading.Thread connectThread = new(ConnectLoop) { IsBackground = true };
        connectThread.Start();

        Current = this;
    }

    public void LogInterpreterOpCode(object context, InterpreterEntry entry)
    {
        QueueDebuggerMessage(new(0, DebuggerMessageType.InterpreterOpCode, entry.Serialize(_formatter)));
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

    private void MessageReceiveLoop()
    {
        while (_socket.Connected)
        {
            DebuggerMessageFrame frame;
            while ((frame = DebugRemoting.ReceiveDebuggerMessage(_socket)) == null)
                if (!_socket.Connected) return;

            switch (frame.Type)
            {
                case DebuggerMessageType.UpdateBreakpoint:
                    var breakpoint = frame.DeserializeData<Breakpoint>(_formatter);
                    if (breakpoint.Enabled)
                        Application.DebugSettings.Breakpoints.Add(breakpoint);
                    else
                        Application.DebugSettings.Breakpoints.Remove(breakpoint);
                    break;

                case DebuggerMessageType.InterpreterCommand:
                    _uibCommand = (InterpreterCommand)frame.Data[0];
                    break;

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

            try
            {
                _socket.Send(BitConverter.GetBytes(frameBytes.Length));
                _socket.Send(frameBytes);
            }
            catch (SocketException) { }
        }
    }

    private void ConnectLoop()
    {
        while (!_listener.Pending())
            if (_disposed) return;

        _socket = _listener.AcceptSocket();

        System.Threading.Thread receiveThread = new(MessageReceiveLoop) { IsBackground = true };
        System.Threading.Thread sendThread = new(MessageSendLoop) { IsBackground = true };
        receiveThread.Start();
        sendThread.Start();
    }
}
