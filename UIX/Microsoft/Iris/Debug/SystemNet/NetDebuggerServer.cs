using Microsoft.Iris.Debug.Data;
using Microsoft.Iris.Markup;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug.SystemNet;

public class NetDebuggerServer : IDebuggerServer, IRemoteDebuggerState, IDisposable
{
    public static IDebuggerServer Current { get; private set; }

    private readonly ConcurrentQueue<byte[]> _outQueue = new();
    private readonly TcpListener _listener;
    private readonly IFormatter _formatter;
    private Socket _socket;
    private bool _disposed = false;
    private InterpreterCommand _uibCommand;

    public event Action<IDebuggerState, object> Connected;

    public string ConnectionString => ConnectionUri.ToString();

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

    public NetDebuggerServer(Uri connectionUri = null)
    {
        ConnectionUri = connectionUri ?? DebugRemoting.DEFAULT_TCP_URI;
        
        _listener = TcpListener.Create(ConnectionUri.Port);
        _listener.Start();

        _formatter = DebugRemoting.CreateBsonFormatter();

        Current = this;
    }

    public void Start()
    {
        System.Threading.Thread connectThread = new(ConnectLoop) { IsBackground = true };
        connectThread.Start();
    }

    public MarkupLineNumberEntry[] OnLineNumberTableRequested(string uri)
    {
        var loadResult = (MarkupLoadResult)LoadResultCache.Read(uri);
        var lineNumberTable = loadResult.LineNumberTable.DumpTable();

        return lineNumberTable;
    }

    public void LogInterpreterDecode(object context, InterpreterInstruction instruction)
    {
        QueueDebuggerMessage(new(0, DebuggerMessageType.InterpreterDecode, instruction, _formatter));
    }

    public void LogInterpreterExecute(object context, InterpreterEntry entry)
    {
        QueueDebuggerMessage(new(0, DebuggerMessageType.InterpreterExecute, entry, _formatter));
    }

    public void LogDispatcher(string message)
    {
        QueueDebuggerMessage(new(0, DebuggerMessageType.DispatcherStep, message, _formatter));
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
            while ((frame = DebugRemoting.ReceiveDebuggerMessage(_socket, _formatter)) == null)
                if (!_socket.Connected) return;

            switch (frame.Type)
            {
                case DebuggerMessageType.UpdateBreakpoint:
                    var breakpoint = frame.GetValue<Breakpoint>();
                    if (breakpoint.Enabled)
                        Application.DebugSettings.Breakpoints.Add(breakpoint);
                    else
                        Application.DebugSettings.Breakpoints.Remove(breakpoint);
                    break;

                case DebuggerMessageType.InterpreterCommand:
                    _uibCommand = frame.GetValue<InterpreterCommand>();
                    break;

                case DebuggerMessageType.LineNumberTable:
                    var lineNumberTable = OnLineNumberTableRequested(frame.GetValue<string>());

                    DebuggerMessageFrame<MarkupLineNumberEntry[]> responseFrame =
                        new(frame.TransactionId, DebuggerMessageType.LineNumberTable, lineNumberTable, _formatter);

                    QueueDebuggerMessage(responseFrame);
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
        Connected?.Invoke(this, _socket);

        System.Threading.Thread receiveThread = new(MessageReceiveLoop) { IsBackground = true };
        System.Threading.Thread sendThread = new(MessageSendLoop) { IsBackground = true };
        receiveThread.Start();
        sendThread.Start();
    }

    public void WaitForContinue()
    {
        while (Application.Debugger.DebuggerCommand == InterpreterCommand.Break) ;
    }
}
