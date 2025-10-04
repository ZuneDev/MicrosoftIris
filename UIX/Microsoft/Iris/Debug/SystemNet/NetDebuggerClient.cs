using Microsoft.Iris.Debug.Data;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug.SystemNet;

public class NetDebuggerClient : IDebuggerClient, IRemoteDebuggerState, IDisposable
{
    private readonly Socket _socket;
    private readonly ConcurrentQueue<byte[]> _outQueue = new();
    private readonly ConcurrentDictionary<long, Action<object>> _requests = new();
    private readonly IFormatter _formatter;

    private InterpreterCommand _uibCommand;
    private long _nextFreeTransactionId = 1;

    public Uri ConnectionUri { get; }

    public InterpreterCommand DebuggerCommand
    {
        get => _uibCommand;
        set
        {
            QueueDebuggerMessage(new(0, DebuggerMessageType.InterpreterCommand, value, _formatter));
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
    }

    public void Start()
    {
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

    public void RequestLineNumberTable(string uri, Action<MarkupLineNumberEntry[]> callback)
    {
        DebuggerMessageFrame frame = new(GetNextTransactionId(), DebuggerMessageType.LineNumberTable, uri, _formatter);
        _requests.TryAdd(frame.TransactionId, o => callback((MarkupLineNumberEntry[])o));
        QueueDebuggerMessage(frame);
    }

    public void UpdateBreakpoint(Breakpoint breakpoint)
    {
        QueueDebuggerMessage(new(0, DebuggerMessageType.UpdateBreakpoint, breakpoint, _formatter));
    }

    private void QueueDebuggerMessage(DebuggerMessageFrame frame)
    {
        _outQueue.Enqueue(frame.ToBytes());
    }

    private long GetNextTransactionId()
    {
        var currentId = _nextFreeTransactionId;

        if (currentId == -1)
            _nextFreeTransactionId += 2;
        else if (currentId == long.MaxValue)
            _nextFreeTransactionId = long.MinValue;
        else
            ++_nextFreeTransactionId;

        return currentId;
    }

    private void MessageReceiveLoop()
    {
        while (_socket.Connected)
        {
            DebuggerMessageFrame frame;
            while ((frame = DebugRemoting.ReceiveDebuggerMessage(_socket, _formatter)) == null)
                if (!_socket.Connected) return;

            switch (frame.Type)
            {
                case DebuggerMessageType.InterpreterDecode:
                    var decEntry = frame.GetValue<InterpreterInstruction>();
                    InterpreterDecode?.Invoke(this, decEntry);
                    break;

                case DebuggerMessageType.InterpreterExecute:
                    var execEntry = frame.GetValue<InterpreterEntry>();
                    InterpreterExecute?.Invoke(this, execEntry);
                    break;

                case DebuggerMessageType.DispatcherStep:
                    string message = frame.GetValue<string>();
                    DispatcherStep?.Invoke(message);
                    break;

                case DebuggerMessageType.InterpreterCommand:
                    _uibCommand = frame.GetValue<InterpreterCommand>();
                    InterpreterStateChanged?.Invoke(_uibCommand);
                    break;

                case DebuggerMessageType.LineNumberTable:
                    if (_requests.TryRemove(frame.TransactionId, out var callback))
                        callback(frame.GetValue<MarkupLineNumberEntry[]>());
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