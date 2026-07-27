// Based on Microsoft.Iris.Render.Extensions.BitmapInformation

using System;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Bitmaps;

public abstract class BitmapInformation : IDisposable
{
    public ImageInformation ImageInfo { get; protected set; }

    public abstract HRESULT LoadFile(string filename, ImageRequirements req);

    public abstract HRESULT LoadResource(string moduleName, string resourceId, ImageRequirements req);

    public abstract HRESULT LoadBuffer(IntPtr pvSrc, int cbSize, ImageRequirements req);
    
    public abstract HRESULT LoadRaw(Size sizeActualPxl, int nStride, SurfaceFormat nFormat, nint pvData,
        ImageRequirements req);
    
    public abstract HRESULT LoadHeader(IntPtr pvSrc, int cbSize, ImageRequirements req);
    
    public abstract void Dispose();
}