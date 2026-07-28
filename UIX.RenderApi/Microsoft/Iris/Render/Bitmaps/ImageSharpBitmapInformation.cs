using System;
using System.Buffers;
using System.IO;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.Render.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Microsoft.Iris.Render.Bitmaps;

public sealed class ImageSharpBitmapInformation : BitmapInformation
{
    private Image _image;
    private MemoryHandle? _pixelHandle;

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
        _pixelHandle?.Dispose();
        _pixelHandle = null;
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
        _image = Image.Load(fileBuffer);

        ApplyImageTransitions(req);
        UpdateImageInfo(_image);
    }

    private void _LoadRaw(Size sizeActualPxl, int nStride, SurfaceFormat nFormat, IntPtr pvData, ImageRequirements req)
    {
        throw new NotImplementedException();
    }
    
    private unsafe void _LoadHeader(nint pvSrc, int cbSize, ImageRequirements req)
    {
        _LoadBuffer(pvSrc, cbSize, req);
        return;
        
        ReadOnlySpan<byte> fileBuffer = new(pvSrc.ToPointer(), cbSize);
        var imageInfo = Image.Identify(fileBuffer);
        
        UpdateImageInfo(imageInfo, 0);
    }
    
    private void ApplyImageTransitions(ImageRequirements req)
    {
        if (req.Flippable)
            _image.Mutate(x => x.Flip(FlipMode.Horizontal));
    }

    private unsafe void UpdateImageInfo(Image image)
    {
        // Must clone into a field (not a bare expression result): DangerousTryGetSinglePixelMemory
        // hands back a pointer into the image's own backing buffer, so the image has to outlive
        // the pointer. A clone with no surviving reference is eligible for GC as soon as its last
        // IL use passes, which can be before callers are done reading through the raw pointer we
        // hand back via ImageInfo.Data.rgData - producing garbage pixels or a crash, not reliably.
        var converted = image.CloneAs<Bgra32>(new Configuration { PreferContiguousImageBuffers = true });
        if (!ReferenceEquals(converted, image))
            image.Dispose();
        _image = converted;

        _pixelHandle?.Dispose();
        _pixelHandle = null;
        if (!converted.DangerousTryGetSinglePixelMemory(out var memory))
            return;

        _pixelHandle = memory.Pin();
        UpdateImageInfo(new ImageInfo(converted.Size, converted.Metadata), (nint)_pixelHandle.Value.Pointer);
    }

    private void UpdateImageInfo(ImageInfo imageInfo, nint buffer)
    {
        Size imageSize = new(imageInfo.Width, imageInfo.Height);

        // This overload is only ever called with the Bgra32-converted clone's Size/Metadata
        // (see UpdateImageInfo(Image)) and a pointer into that same clone's tightly-packed
        // buffer (DangerousTryGetSinglePixelMemory only succeeds when contiguous, since we
        // request PreferContiguousImageBuffers), so both format and stride are fixed: 4
        // bytes/pixel, no row padding. imageInfo.PixelType reflects the *original* decoded
        // file's bit depth (e.g. 24bpp JPEG, 8bpp indexed PNG), not this buffer's -- using it
        // here previously produced a bogus stride (and it was bytes-per-pixel, not
        // bytes-per-row, missing the "* width" besides), so every row past the first read
        // from the wrong offset.
        ImageInfo = new ImageInformation
        {
            Header = new ImageHeader
            {
                sizeActualPxl = imageSize,
                sizeOriginalPxl = imageSize,
                nStride = imageSize.Width * 4,
                nFormat = SurfaceFormat.ARGB32
            },
            Data = new ImageData
            {
                rgData = buffer
            }
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