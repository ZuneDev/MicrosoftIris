namespace Microsoft.Iris.Render.Interop.Protocol;

// Bit-for-bit mirrors of UIX.RenderApi/Microsoft/Iris/Render/Protocol/{ShutdownReason,TransportProtocol}.cs.
public enum ShutdownReason
{
    NoReason,
    SelfShutdown,
    PeerShutdown,
    TransportClosed,
    TransportFailure,
    GenericFailure,
}

public enum TransportProtocol
{
    None = 0,
    Min = 1,
    VC = 1,
    TCP = 2,
    UDP = 3,
    Max = 4,
    PIPE = 4,
}
