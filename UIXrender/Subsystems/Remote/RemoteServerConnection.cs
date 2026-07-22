using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.Protocol;

namespace Microsoft.Iris.Render.Subsystems.Remote;

// Backs the four SpRemote* exports (Protocol/EngineApi.cs) for the out-of-process
// "RemoteChannel" path. Real, working transport over .NET sockets/named pipes -- TCP and
// PIPE are genuine duplex byte streams so one connection object serves as both the
// "send" and "receive" stream handle the managed side round-trips back to us; VC is
// left unimplemented (E_NOTIMPL) since its meaning was never recovered from the
// decompiled managed code (see logs/UIXrender/Architecture.md open question #2) -- not
// guessed at further. Once connected, wire format matches the local channel: each frame
// is a length-prefixed BufferInfo + payload, dispatched into the same
// EngineService/ContextRegistry local channel uses, so remote and local contexts are
// indistinguishable to the rest of the engine.
//
// Handle model (matters for correctness -- RemoteChannel.Connect releases both stream
// handles right after ServerInit, then disposes the session later): CreateServerStreams
// hands out *two distinct* GCHandles to one connection with a refcount of 2; ServerInit
// adds a third (the session). Each SpObjectRelease / SpRemoteServerUninit frees its own
// handle and decrements; the connection's sockets are torn down exactly once, when the
// last reference goes. The receive callback is a pointer-free BufferReceivedHandler,
// exactly like RenderThread's -- the native shim adapts a raw function pointer into one,
// the managed-direct caller adapts its own delegate, and this class never sees a pointer.
internal sealed class RemoteServerConnection : IDisposable
{
    private TcpListener _tcpListener;
    private NamedPipeServerStream _pipe;
    private UdpClient _udp;
    private IPEndPoint _udpRemote;
    private Stream _stream;
    private ContextID _contextId;
    private Thread _readThread;
    private volatile bool _running;
    private int _refCount;

    public static HRESULT CreateServerStreams(TransportProtocol protocol, string sessionName, out IntPtr sendHandle, out IntPtr receiveHandle)
    {
        sendHandle = IntPtr.Zero;
        receiveHandle = IntPtr.Zero;

        var connection = new RemoteServerConnection();
        switch (protocol)
        {
            case TransportProtocol.TCP:
                connection._tcpListener = new TcpListener(IPAddress.Any, 0);
                connection._tcpListener.Start();
                break;
            case TransportProtocol.PIPE:
                connection._pipe = new NamedPipeServerStream(string.IsNullOrEmpty(sessionName) ? "UIXrender" : sessionName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                break;
            case TransportProtocol.UDP:
                connection._udp = new UdpClient(0);
                break;
            case TransportProtocol.VC:
            default:
                // TODO: "VC" transport semantics were never recovered -- see
                // logs/UIXrender/Architecture.md open question #2. Not guessed at.
                connection.Dispose();
                return HRESULT.E_NOTIMPL;
        }

        // Two distinct handles to the one duplex connection (send + receive), so releasing
        // each is a separate, non-double-freeing operation. See the handle-model comment.
        connection._refCount = 2;
        sendHandle = GCHandle.ToIntPtr(GCHandle.Alloc(connection, GCHandleType.Normal));
        receiveHandle = GCHandle.ToIntPtr(GCHandle.Alloc(connection, GCHandleType.Normal));
        return HRESULT.S_OK;
    }

    public static HRESULT WaitConnected(TransportProtocol protocol, IntPtr sendHandle)
    {
        if (sendHandle == IntPtr.Zero)
            return HRESULT.E_INVALIDARG;

        var connection = (RemoteServerConnection)GCHandle.FromIntPtr(sendHandle).Target!;
        try
        {
            switch (protocol)
            {
                case TransportProtocol.TCP:
                    connection._stream = connection._tcpListener.AcceptTcpClient().GetStream();
                    break;
                case TransportProtocol.PIPE:
                    connection._pipe.WaitForConnection();
                    connection._stream = connection._pipe;
                    break;
                case TransportProtocol.UDP:
                    // First datagram received "connects" the client, matching TCP/PIPE's
                    // blocking-until-a-client-attaches semantics as closely as UDP allows.
                    var buffer = connection._udp.Receive(ref connection._udpRemote);
                    connection._udp.Connect(connection._udpRemote);
                    connection._stream = connection._udp.Client is { } sock ? new NetworkStream(sock) : null;
                    break;
                default:
                    return HRESULT.E_NOTIMPL;
            }
        }
        catch (SocketException)
        {
            return HRESULT.E_FAIL;
        }
        catch (IOException)
        {
            return HRESULT.E_FAIL;
        }

        return HRESULT.S_OK;
    }

    // onRemoteToLocal receives frames arriving from the peer; null means "drop them"
    // (RemoteChannel connects without a receive callback -- it drives the wire in one
    // direction only). Pointer-free by design: the caller adapts whatever callback
    // representation it has (native function pointer, managed delegate) into this shape.
    public static HRESULT ServerInit(IntPtr sendHandle, ContextID context, BufferReceivedHandler onRemoteToLocal, out IntPtr pSession)
    {
        pSession = IntPtr.Zero;
        if (sendHandle == IntPtr.Zero)
            return HRESULT.E_INVALIDARG;

        var connection = (RemoteServerConnection)GCHandle.FromIntPtr(sendHandle).Target!;
        if (connection._stream == null)
            return HRESULT.E_FAIL;

        connection._contextId = context;

        // Local -> remote: anything sent to this context is framed onto the wire.
        ContextRegistry.Register(context, connection.WriteFrame);

        connection._running = true;
        connection._readThread = new Thread(() => connection.ReadLoop(onRemoteToLocal)) { IsBackground = true, Name = "UIXrender.RemoteServerConnection" };
        connection._readThread.Start();

        Interlocked.Increment(ref connection._refCount);
        pSession = GCHandle.ToIntPtr(GCHandle.Alloc(connection, GCHandleType.Normal));
        return HRESULT.S_OK;
    }

    public static HRESULT ServerUninit(IntPtr pSession, bool forceShutdown, out ShutdownReason reason)
    {
        reason = ShutdownReason.SelfShutdown;
        if (pSession == IntPtr.Zero)
            return HRESULT.E_INVALIDARG;

        // Stop the reader promptly (this is the "don't wait for a graceful peer shutdown"
        // meaning of forceShutdown), then drop the session's reference.
        if (GCHandle.FromIntPtr(pSession).Target is RemoteServerConnection connection)
            connection._running = false;

        ReleaseHandle(pSession);
        return HRESULT.S_OK;
    }

    // Backs the managed-direct SpObjectRelease on a stream handle: drops one reference and
    // frees that handle, disposing the connection only when the last reference goes.
    public static void ReleaseHandle(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return;

        GCHandle gc = GCHandle.FromIntPtr(handle);
        var connection = gc.Target as RemoteServerConnection;
        gc.Free();

        if (connection != null && Interlocked.Decrement(ref connection._refCount) == 0)
            connection.Dispose();
    }

    private unsafe void WriteFrame(ContextID source, RENDERHANDLE bufferHandle, BufferFlags flags, ReadOnlySpan<byte> data)
    {
        if (_stream == null)
            return;

        var info = new BufferInfo
        {
            idContextSrc = source,
            idContextDest = _contextId,
            idBuffer = bufferHandle,
            nFlags = flags,
            cbSizeBuffer = (uint)data.Length,
        };

        Span<byte> header = stackalloc byte[sizeof(uint) + sizeof(BufferInfo)];
        BitConverter.TryWriteBytes(header, (uint)sizeof(BufferInfo) + (uint)data.Length);
        fixed (byte* pHeader = header)
            *(BufferInfo*)(pHeader + sizeof(uint)) = info;

        lock (this)
        {
            _stream.Write(header);
            _stream.Write(data);
            _stream.Flush();
        }
    }

    private unsafe void ReadLoop(BufferReceivedHandler onRemoteToLocal)
    {
        var lengthBuffer = new byte[sizeof(uint)];
        var infoBuffer = new byte[sizeof(BufferInfo)];

        try
        {
            while (_running)
            {
                if (!ReadExact(lengthBuffer))
                    break;
                uint totalSize = BitConverter.ToUInt32(lengthBuffer);
                if (!ReadExact(infoBuffer))
                    break;

                BufferInfo info;
                fixed (byte* p = infoBuffer)
                    info = *(BufferInfo*)p;

                uint payloadSize = totalSize - (uint)sizeof(BufferInfo);
                var payload = payloadSize > 0 ? new byte[payloadSize] : Array.Empty<byte>();
                if (payloadSize > 0 && !ReadExact(payload))
                    break;

                onRemoteToLocal?.Invoke(info.idContextSrc, info.idBuffer, info.nFlags, payload);
            }
        }
        catch (IOException) { }
        catch (ObjectDisposedException) { }
    }

    private bool ReadExact(byte[] buffer)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int read = _stream.Read(buffer, offset, buffer.Length - offset);
            if (read <= 0)
                return false;
            offset += read;
        }
        return true;
    }

    public void Dispose()
    {
        _running = false;
        ContextRegistry.Unregister(_contextId);
        _stream?.Dispose();
        _tcpListener?.Stop();
        _pipe?.Dispose();
        _udp?.Dispose();
        _readThread?.Join(TimeSpan.FromSeconds(1));
    }
}
