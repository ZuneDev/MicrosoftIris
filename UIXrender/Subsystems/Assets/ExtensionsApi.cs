using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.Drawing;
using Microsoft.Iris.Render.Interop.Extensions;
using StbImageSharp;

namespace Microsoft.Iris.Render.Subsystems.Assets;

// [UnmanagedCallersOnly] exports matching the asset-loading DllImports in
// UIX.RenderApi/Microsoft/Iris/Render/Extensions/ExtensionsApi.cs.
//
// Note ImageRequirements is a *class* with [StructLayout(Sequential)] on the managed side
// and is passed [MarshalAs(UnmanagedType.LPStruct)], i.e. the callee receives a pointer
// to its fields -- hence ImageRequirements* here, not a by-value struct.
public static unsafe class ExtensionsApi
{
    [UnmanagedCallersOnly(EntryPoint = "SpBitmapLoadFile")]
    public static HRESULT SpBitmapLoadFile(char* stFileName, ImageRequirements* req, BitmapOptions nOptions, HSpBitmap* hBmp, ImageInformation* info)
    {
        if (hBmp == null || info == null)
            return HRESULT.E_INVALIDARG;

        string path = NativeString.UniToString(stFileName);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return HRESULT.E_FAIL;

        try
        {
            using FileStream stream = File.OpenRead(path);
            return CompleteDecode(ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha), req, hBmp, info);
        }
        catch (Exception e) when (e is IOException or ArgumentException)
        {
            return HRESULT.E_FAIL;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpBitmapLoadBuffer")]
    public static HRESULT SpBitmapLoadBuffer(IntPtr pvSrc, uint cbSize, ImageRequirements* req, BitmapOptions nOptions, HSpBitmap* hBmp, ImageInformation* info)
    {
        if (hBmp == null || info == null || pvSrc == IntPtr.Zero)
            return HRESULT.E_INVALIDARG;

        try
        {
            using var stream = new UnmanagedMemoryStream((byte*)pvSrc, cbSize);
            return CompleteDecode(ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha), req, hBmp, info);
        }
        catch (Exception e) when (e is IOException or ArgumentException)
        {
            return HRESULT.E_FAIL;
        }
    }

    // Raw pixels need no decoder -- this path just takes ownership of a copy, so it is
    // fully real on every platform.
    [UnmanagedCallersOnly(EntryPoint = "SpBitmapLoadRaw")]
    public static HRESULT SpBitmapLoadRaw(Size sizeActualPxl, int nStride, SurfaceFormat nFormat, IntPtr pvData, ImageRequirements* req, BitmapOptions nOptions, HSpBitmap* hBmp, ImageInformation* info)
    {
        if (hBmp == null || info == null || pvData == IntPtr.Zero)
            return HRESULT.E_INVALIDARG;

        var bitmap = LoadedBitmap.FromRaw(sizeActualPxl, nStride, nFormat, pvData);
        hBmp->h = HandleTable.Alloc(bitmap);
        *info = bitmap.ToInformation();
        return HRESULT.S_OK;
    }

    // Win32 resource sections (HINSTANCE + resource name) are a PE/Windows loader concept
    // with no cross-platform equivalent, and this project has no resource-section reader
    // of its own yet. Reports failure rather than inventing a lookup.
    // TODO: implement once SpLoadBinaryResource's resource store (Subsystems/Modules)
    // gains real PE resource parsing -- this should then read through that, not duplicate it.
    [UnmanagedCallersOnly(EntryPoint = "SpBitmapLoadResource")]
    public static HRESULT SpBitmapLoadResource(IntPtr hinst, char* stName, int nType, ImageRequirements* req, BitmapOptions nOptions, HSpBitmap* hBmp, ImageInformation* info)
    {
        if (hBmp == null || info == null)
            return HRESULT.E_INVALIDARG;

        *hBmp = HSpBitmap.NULL;
        *info = default;
        return HRESULT.E_NOTIMPL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpBitmapDelete")]
    public static HRESULT SpBitmapDelete(HSpBitmap hBmp)
    {
        HandleTable.Free(hBmp.h);
        return HRESULT.S_OK;
    }

    // Real: parses a RIFF/WAVE container and hands back the PCM payload. No codec is
    // needed because the managed side's only declared format constant is
    // WAVE_FORMAT_PCM (ExtensionsApi.cs), i.e. uncompressed samples.
    [UnmanagedCallersOnly(EntryPoint = "SpSoundLoadBuffer")]
    public static HRESULT SpSoundLoadBuffer(IntPtr pBuffer, int dwSize, SoundOptions options, HSpSound* hSound, SoundInformation* info)
    {
        if (hSound == null || info == null || pBuffer == IntPtr.Zero || dwSize <= 0)
            return HRESULT.E_INVALIDARG;

        var source = new ReadOnlySpan<byte>((void*)pBuffer, dwSize);
        if (!WaveParser.TryParse(source, out SoundHeader header, out byte[] samples))
            return HRESULT.E_FAIL;

        IntPtr payload = Marshal.AllocHGlobal(samples.Length);
        Marshal.Copy(samples, 0, payload, samples.Length);

        var sound = new LoadedSound(payload);
        hSound->h = HandleTable.Alloc(sound);
        *info = new SoundInformation { header = header, data = new SoundData { rgData = payload } };
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpSoundDispose")]
    public static HRESULT SpSoundDispose(HSpSound hSound, SoundInformation info)
    {
        HandleTable.Free(hSound.h);
        return HRESULT.S_OK;
    }

    private static HRESULT CompleteDecode(ImageResult image, ImageRequirements* req, HSpBitmap* hBmp, ImageInformation* info)
    {
        if (image == null)
            return HRESULT.E_FAIL;

        ImageRequirements requirements = req != null ? *req : default;
        var bitmap = LoadedBitmap.FromDecoded(image, requirements);
        hBmp->h = HandleTable.Alloc(bitmap);
        *info = bitmap.ToInformation();
        return HRESULT.S_OK;
    }
}

internal sealed class LoadedSound(IntPtr samples) : IDisposable
{
    private IntPtr _samples = samples;

    public void Dispose()
    {
        if (_samples != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_samples);
            _samples = IntPtr.Zero;
        }
    }
}
