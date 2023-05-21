using Microsoft.Iris.Data;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS;

public class BytesResource : Resource
{
    private byte[] _bytes;
    private string _identifier;
    private GCHandle _handle;

    public BytesResource(string uri, byte[] bytes, string identifier, bool forceSynchronous)
      : base(uri, forceSynchronous)
    {
        _bytes = bytes;
        _identifier = identifier;
    }

    public override string Identifier => _identifier;

    protected override void StartAcquisition(bool forceSynchronous)
    {
        _handle = GCHandle.Alloc(_bytes, GCHandleType.Pinned);
        NotifyAcquisitionComplete(_handle.AddrOfPinnedObject(), (uint)_bytes.Length, true, null);
    }

    protected override void CancelAcquisition()
    {
        if (!_handle.IsAllocated)
            return;

        _handle.Free();
    }
}
