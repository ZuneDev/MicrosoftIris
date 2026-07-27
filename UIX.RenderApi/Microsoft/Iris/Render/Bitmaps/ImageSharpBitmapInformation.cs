using System;
using System.IO;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.Render.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Microsoft.Iris.Render.Bitmaps;

public sealed class ImageSharpBitmapInformation : BitmapInformation
{
    private Image _image;

    public override HRESULT LoadFile(string filename, ImageRequirements req) =>
        Try(() => _LoadFile(filename, req));

    public override HRESULT LoadResource(string moduleName, string resourceId, ImageRequirements req) =>
        Try(() => _LoadResource(moduleName, resourceId, req));

    public override HRESULT LoadBuffer(nint pvSrc, int cbSize, ImageRequirements req) =>
        Try(() => _LoadBuffer(pvSrc, cbSize, req));

    public override HRESULT LoadRaw(Size sizeActualPxl, int nStride, SurfaceFormat nFormat, IntPtr pvData, ImageRequirements req) =>
        Try(() => _LoadRaw(sizeActualPxl, nStride, nFormat, pvData, req));

    public override HRESULT LoadHeader(nint pvSrc, int cbSize, ImageRequirements req) =>
        Try(() => _LoadHeader(pvSrc, cbSize, req));

    public override void Dispose()
    {
        _image?.Dispose();
        _image = null;
    }

    private void _LoadFile(string filename, ImageRequirements req)
    {
        _image = Image.Load(filename);
        
        ApplyImageTransitions(req);
        UpdateImageInfo(_image);
    }

    private void _LoadResource(string moduleName, string resourceId, ImageRequirements req)
    {
        throw new NotImplementedException();
    }

    private unsafe void _LoadBuffer(nint pvSrc, int cbSize, ImageRequirements req)
    {
        ReadOnlySpan<byte> fileBuffer = new(pvSrc.ToPointer(), cbSize);
        _image = Image.Load(fileBuffer).Clone(_ => {});
        
        ApplyImageTransitions(req);
        UpdateImageInfo(_image);
    }

    private void _LoadRaw(Size sizeActualPxl, int nStride, SurfaceFormat nFormat, IntPtr pvData, ImageRequirements req)
    {
        throw new NotImplementedException();
    }
    
    private unsafe void _LoadHeader(nint pvSrc, int cbSize, ImageRequirements req)
    {
        ReadOnlySpan<byte> fileBuffer = new(pvSrc.ToPointer(), cbSize);
        var imageInfo = Image.Identify(fileBuffer);
        
        UpdateImageInfo(imageInfo);
    }
    
    private void ApplyImageTransitions(ImageRequirements req)
    {
        if (req.Flippable)
            _image.Mutate(x => x.Flip(FlipMode.Horizontal));
    }

    private void UpdateImageInfo(Image image) => UpdateImageInfo(new ImageInfo(image.Size, image.Metadata));
    
    private void UpdateImageInfo(ImageInfo imageInfo)
    {
        Size imageSize = new(imageInfo.Width, imageInfo.Height);
        
        // TODO: Read pixel data format
        SurfaceFormat format = SurfaceFormat.None;
        
        ImageInfo = new ImageInformation
        {
            Header = new ImageHeader
            {
                sizeActualPxl = imageSize,
                sizeOriginalPxl = imageSize,
                nStride = imageInfo.PixelType.BitsPerPixel / 8,
                nFormat = format
            },
            Data = default
        };
    }
    
    private static HRESULT Try(Action action)
    {
        try
        {
            action();
        }
        catch (FileNotFoundException)
        {
            return 0x80070002;
        }
        catch (ArgumentNullException)
        {
            return 0x80004003;
        }
        catch (ArgumentException)
        {
            return 0x80070057;
        }
        catch (NotImplementedException)
        {
            return 0x80004001;
        }
        catch
        {
            return HRESULT.E_FAIL;
        }
        
        return HRESULT.S_OK;
    }
}