// Based on decompilation with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.BitmapInformation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;

namespace Microsoft.Iris.Render.Bitmaps;

internal sealed class SpBitmapInformation : BitmapInformation
{
    public HSpBitmap hBitmap;

    public override HRESULT LoadFile(string filename, ImageRequirements req)
    {
        var nOptions = ExtensionsApi.BitmapOptions.Decode;
        if (req.Flippable)
            nOptions |= ExtensionsApi.BitmapOptions.Flip;

        var hresult = ExtensionsApi.SpBitmapLoadFile(filename, req, nOptions,
            out hBitmap, out var imageInformation);
        
        if (hresult.IsSuccess())
        {
            ImageInfo = imageInformation;
        }
        else
        {
            ImageInfo = default;
            hBitmap = default;
        }
        
        return hresult;
    }

    public override HRESULT LoadResource(string moduleName, string resourceId, ImageRequirements req)
    {
        var hinst = ModuleManager.Instance.LoadModule(moduleName);
        if (hinst == Win32Api.HINSTANCE.NULL)
            return 0x80070006;
            
        var nOptions = ExtensionsApi.BitmapOptions.Decode;
        if (req.Flippable)
            nOptions |= ExtensionsApi.BitmapOptions.Flip;
            
        var hresult = ExtensionsApi.SpBitmapLoadResource(hinst, resourceId, 10, req, nOptions,
            out hBitmap, out var imageInformation);

        if (hresult.IsSuccess())
        {
            ImageInfo = imageInformation;
        }
        else
        {
            ImageInfo = default;
            hBitmap = default;
        }

        return hresult;
    }

    public override HRESULT LoadBuffer(IntPtr pvSrc, int cbSize, ImageRequirements req)
    {
        var nOptions = ExtensionsApi.BitmapOptions.Decode;
        if (req.Flippable)
            nOptions |= ExtensionsApi.BitmapOptions.Flip;
            
        var hresult = ExtensionsApi.SpBitmapLoadBuffer(pvSrc, (uint)cbSize, req, nOptions,
            out hBitmap, out var imageInformation);
        
        if (hresult.IsSuccess())
        {
            ImageInfo = imageInformation;
        }
        else
        {
            ImageInfo = default;
            hBitmap = default;
        }
        
        return hresult;
    }

    public override HRESULT LoadRaw(Size sizeActualPxl, int nStride, SurfaceFormat nFormat, IntPtr pvData, ImageRequirements req)
    {
        var nOptions = ExtensionsApi.BitmapOptions.Decode;
        if (req.Flippable)
            nOptions |= ExtensionsApi.BitmapOptions.Flip;
            
        var hresult = ExtensionsApi.SpBitmapLoadRaw(sizeActualPxl, nStride, nFormat, pvData, req, nOptions,
            out hBitmap, out var imageInformation);
        
        if (hresult.IsSuccess())
        {
            ImageInfo = imageInformation;
        }
        else
        {
            ImageInfo = default;
            hBitmap = default;
        }
        
        return hresult;
    }

    public override HRESULT LoadHeader(IntPtr pvSrc, int cbSize, ImageRequirements req)
    {
        var hresult = ExtensionsApi.SpBitmapLoadBuffer(pvSrc, (uint)cbSize, req, ExtensionsApi.BitmapOptions.None,
            out hBitmap, out var imageInformation);
        
        if (hresult.IsSuccess())
        {
            ImageInfo = imageInformation;
        }
        else
        {
            ImageInfo = default;
            hBitmap = default;
        }
        
        return hresult;
    }

    public override void Dispose()
    {
        if (hBitmap == HSpBitmap.NULL)
            return;
            
        EngineApi.IFC(ExtensionsApi.SpBitmapDelete(hBitmap));
        hBitmap = HSpBitmap.NULL;
    }
}