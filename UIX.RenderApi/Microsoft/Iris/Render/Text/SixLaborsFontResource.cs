using System;
using System.IO;
using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Text;

public sealed class SixLaborsFontResource : FontResource
{
    public override HRESULT LoadFile(string filename) => Try(() =>
    {
        using var stream = File.OpenRead(filename);
        SixLaborsFontRegistry.Collection.Add(stream);
    });

    public override HRESULT LoadResource(string moduleName, string resourceId) =>
        // No cross-platform equivalent of a Win32 module/resource handle exists.
        // Mirrors ImageSharpBitmapInformation._LoadResource's same limitation.
        Try(() => throw new NotImplementedException());

    public override unsafe HRESULT LoadBuffer(IntPtr pvSrc, int cbSize) => Try(() =>
    {
        ReadOnlySpan<byte> buffer = new(pvSrc.ToPointer(), cbSize);
        using var stream = new MemoryStream(buffer.ToArray());

        var isCollection = buffer[..4].SequenceEqual("ttcf"u8);
        if (isCollection)
            SixLaborsFontRegistry.Collection.AddCollection(stream);
        else
            SixLaborsFontRegistry.Collection.Add(stream);
    });

    public override void Dispose()
    {
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
