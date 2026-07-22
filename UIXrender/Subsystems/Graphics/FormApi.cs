using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Graphics;

// [UnmanagedCallersOnly] exports matching UIX.RenderApi/Microsoft/Iris/Render/Internal/FormApi.cs.
//
// Real, and legitimately a no-op pair rather than a stub: these existed so the original
// C++ code could call GdiplusStartup/GdiplusShutdown before using GDI+ for image decoding
// and text rasterization. This reimplementation decodes images with StbImageSharp (pure
// managed, see logs/UIXrender/FullSurface.md, decision 3) and never initialises GDI+ at
// all, so there is genuinely nothing to start up -- reporting success is the correct
// answer, not a deferral. Recorded explicitly so a future reader doesn't mistake these
// for unimplemented.
public static class FormApi
{
    [UnmanagedCallersOnly(EntryPoint = "SpGdiplusInit")]
    public static HRESULT SpGdiplusInit() => HRESULT.S_OK;

    [UnmanagedCallersOnly(EntryPoint = "SpGdiplusUninit")]
    public static HRESULT SpGdiplusUninit() => HRESULT.S_OK;
}
