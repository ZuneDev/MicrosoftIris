using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop;

// Bit-for-bit mirror of the decompiled Microsoft.Iris.Render.Internal.HRESULT
// (UIX.RenderApi/Microsoft/Iris/Render/Internal/HRESULT.cs) -- a single int, so every
// [UnmanagedCallersOnly] method returning HRESULT here matches what EngineApi.cs's
// [DllImport] declarations expect byte-for-byte. The S_OK/E_* constants aren't part of
// the original decompiled surface (nothing to break by adding them -- this is new code
// with no existing dependents), added here purely as return-value conveniences.
[StructLayout(LayoutKind.Sequential)]
public struct HRESULT
{
    public static readonly HRESULT S_OK = new(0);
    public static readonly HRESULT E_FAIL = new(unchecked((int)0x80004005));
    public static readonly HRESULT E_INVALIDARG = new(unchecked((int)0x80070057));
    public static readonly HRESULT E_NOTIMPL = new(unchecked((int)0x80004001));

    public int hr;

    public HRESULT(int hr) => this.hr = hr;

    public static bool operator ==(HRESULT a, HRESULT b) => a.hr == b.hr;
    public static bool operator !=(HRESULT a, HRESULT b) => a.hr != b.hr;

    public override bool Equals(object obj) => obj is HRESULT other && hr == other.hr;
    public override int GetHashCode() => hr;

    public bool IsError() => hr < 0;
    public bool IsSuccess() => hr >= 0;
}
