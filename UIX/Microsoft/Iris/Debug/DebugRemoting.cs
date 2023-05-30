using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug;

/// <summary>
/// Provides helpers for things that are common between debug clients, servers, and transports.
/// </summary>
public static class DebugRemoting
{
    public const string DEFAULT_TCP_CLIENT_URI = ">tcp://127.0.0.1:5555,@tcp://127.0.0.1:55556";

    public const string DEFAULT_TCP_SERVER_URI = "@tcp://127.0.0.1:5555,>tcp://127.0.0.1:55556";

    internal static IFormatter CreateBsonFormatter() => new BsonFormatter(new StreamingContext(StreamingContextStates.Remoting));
}
