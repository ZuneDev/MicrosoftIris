using Microsoft.Iris.Debug.Data;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug.SystemNet;

public class NetDebuggerClient : IDebuggerClient, IDisposable
{
    private readonly Socket _socket;
    private readonly ConcurrentQueue<byte[]> _outQueue = new();
    private readonly IFormatter _formatter;
    private InterpreterCommand _uibCommand;

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

    public event EventHandler<InterpreterInstruction> InterpreterDecode;
    public event EventHandler<InterpreterEntry> InterpreterExecute;
    public event Action<string> DispatcherStep;
    public event Action<InterpreterCommand> InterpreterStateChanged;
    public event Action<IDebuggerState, object> Connected;

    public NetDebuggerClient(string connectionUri) : this(connectionUri is null ? null : new Uri(connectionUri))
    {
    }

    public NetDebuggerClient(Uri connectionUri)
    {
        ConnectionUri = connectionUri ?? DebugRemoting.DEFAULT_TCP_URI;
        _socket = new(SocketType.Stream, ProtocolType.Tcp);
        _formatter = DebugRemoting.CreateBsonFormatter();

        System.Threading.Thread connectThread = new(ConnectLoop) { IsBackground = true };
        connectThread.Start();
    }

    public void Dispose()
    {
        if (_socket != null)
        {
            if (_socket.Connected)
                _socket.Disconnect(false);
            _socket.Close();
        }
    }

    public void UpdateBreakpoint(Breakpoint breakpoint)
    {
        QueueDebuggerMessage(new(0, DebuggerMessageType.UpdateBreakpoint, breakpoint.Serialize(_formatter)));
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
                case DebuggerMessageType.InterpreterDecode:
                    var decEntry = frame.DeserializeData<InterpreterInstruction>(_formatter);
                    InterpreterDecode?.Invoke(this, decEntry);
                    break;
                
                case DebuggerMessageType.InterpreterExecute:
                    var execEntry = frame.DeserializeData<InterpreterEntry>(_formatter);
                    InterpreterExecute?.Invoke(this, execEntry);
                    break;

                case DebuggerMessageType.DispatcherStep:
                    string message = frame.GetDataAsString();
                    DispatcherStep?.Invoke(message);
                    break;

                case DebuggerMessageType.InterpreterCommand:
                    _uibCommand = (InterpreterCommand)frame.Data[0];
                    InterpreterStateChanged?.Invoke(_uibCommand);
                    break;

                default:
                    Trace.WriteLine(TraceCategory.MarkupDebug, "Received unknown debugger message of type '{0}'.", frame.Type);
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
        while (!_socket.Connected)
        {
            try
            {
                var endpoint = new IPEndPoint(IPAddress.Parse(ConnectionUri.Host), ConnectionUri.Port);
                _socket.Connect(endpoint);
            }
            catch { }
        }

        Connected?.Invoke(this, _socket);

        System.Threading.Thread receiveThread = new(MessageReceiveLoop) { IsBackground = true };
        System.Threading.Thread sendThread = new(MessageSendLoop) { IsBackground = true };
        receiveThread.Start();
        sendThread.Start();
    }
}