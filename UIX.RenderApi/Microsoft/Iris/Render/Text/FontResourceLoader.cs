using System;

namespace Microsoft.Iris.Render.Text;

// Based on Microsoft.Iris.Render.Extensions.ImageLoader
//
// Unlike ImageLoader's SpBitmapInformation/ImageSharpBitmapInformation split
// (both of which live in this assembly), the Windows font/text backend needs
// the marshalling machinery already implemented in Microsoft.Iris.OS.NativeApi,
// which lives in the downstream UIX project (UIX -> UIX.RenderApi, not the
// other way around). So instead of a #if WINDOWS factory switch, UIX registers
// its backend here via RegisterWindowsBackend from a module initializer gated
// on the WINDOWS compile constant - see Microsoft.Iris.OS.TextBackendRegistration.
// On any platform where nothing registers a backend, this falls back to the
// SixLabors.Fonts-based implementation.
public static class FontResourceLoader
{
    private static Func<FontResource> s_windowsBackend;

    public static void RegisterWindowsBackend(Func<FontResource> factory) => s_windowsBackend = factory;

    private static FontResource Create() => s_windowsBackend?.Invoke() ?? new SixLaborsFontResource();

    public static bool LoadFromFile(string filename)
    {
        using var fontResource = Create();
        return fontResource.LoadFile(filename).IsSuccess();
    }

    public static bool LoadFromModuleResource(string moduleName, string resourceId)
    {
        using var fontResource = Create();
        return fontResource.LoadResource(moduleName, resourceId).IsSuccess();
    }

    public static bool LoadFromBuffer(IntPtr pvSrc, int cbSize)
    {
        using var fontResource = Create();
        return fontResource.LoadBuffer(pvSrc, cbSize).IsSuccess();
    }
}
