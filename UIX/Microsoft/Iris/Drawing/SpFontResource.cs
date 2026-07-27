using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Text;

namespace Microsoft.Iris.Drawing;

// Windows backend for Microsoft.Iris.Render.Text.FontResource, living here
// (rather than alongside the abstract class in UIX.RenderApi) so it can reuse
// Microsoft.Iris.OS.NativeApi's existing P/Invoke declarations instead of
// duplicating them in an assembly NativeApi isn't visible from. Registered
// with FontResourceLoader by TextBackendRegistration.
internal sealed class SpFontResource : FontResource
{
    [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int AddFontResourceExW(string lpszFilename, uint fl, IntPtr pdv);

    private const uint FR_PRIVATE = 0x10;

    public override HRESULT LoadFile(string filename)
    {
        var addedCount = AddFontResourceExW(filename, FR_PRIVATE, IntPtr.Zero);
        return addedCount > 0 ? HRESULT.S_OK : HRESULT.E_FAIL;
    }

    public override HRESULT LoadResource(string moduleName, string resourceId) =>
        NativeApi.SpLoadFontResource(moduleName, resourceId) ? HRESULT.S_OK : HRESULT.E_FAIL;

    public override HRESULT LoadBuffer(IntPtr pvSrc, int cbSize)
    {
        uint cFonts = 0;
        var hFont = Win32Api.AddFontMemResourceEx(pvSrc, (uint)cbSize, IntPtr.Zero, ref cFonts);
        return hFont != IntPtr.Zero ? HRESULT.S_OK : HRESULT.E_FAIL;
    }

    public override void Dispose()
    {
    }
}
