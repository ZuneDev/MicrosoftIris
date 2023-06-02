using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug;

/// <summary>
/// Provides helpers for things that are common between debug clients, servers, and transports.
/// </summary>
public static class DebugRemoting
{
    public const string DEFAULT_TCP_CLIENT_URI = ">tcp://127.0.0.1:5555,@tcp://127.0.0.1:5556";
    public const string DEFAULT_TCP_SERVER_URI = "@tcp://127.0.0.1:5555,>tcp://127.0.0.1:5556";

    public static readonly Uri DEFAULT_TCP_URI = new("tcp://127.0.0.1:5555");

    internal static IFormatter CreateBsonFormatter() => new BsonFormatter(new StreamingContext(StreamingContextStates.Remoting));

    internal static Data.DebuggerMessageFrame ReceiveDebuggerMessage(Socket socket)
    {
        try
        {
            byte[] sizeBuffer = new byte[sizeof(int)];
            int bytesReceived = socket.Receive(sizeBuffer);

            // Reached end of stream, no bytes to recieve
            if (bytesReceived == 0)
                return null;

            int frameLength = BitConverter.ToInt32(sizeBuffer, 0);

            byte[] frameBytes = new byte[frameLength];
            socket.Receive(frameBytes, frameLength, SocketFlags.None);

            return new(frameBytes);
        }
        catch (SocketException)
        {
            return null;
        }
    }

    internal static byte[] Serialize(this object obj, IFormatter formatter)
    {
        using MemoryStream dataStream = new();
        formatter.Serialize(dataStream, obj);
        return dataStream.ToArray();
    }
}
