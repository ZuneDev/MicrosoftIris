using System;
using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Text;

// Based on Microsoft.Iris.Render.Bitmaps.BitmapInformation
public abstract class FontResource : IDisposable
{
    public abstract HRESULT LoadFile(string filename);

    public abstract HRESULT LoadResource(string moduleName, string resourceId);

    public abstract HRESULT LoadBuffer(IntPtr pvSrc, int cbSize);

    public abstract void Dispose();
}
