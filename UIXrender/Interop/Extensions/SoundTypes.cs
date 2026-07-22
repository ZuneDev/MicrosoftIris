using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Extensions;

// Bit-for-bit mirrors of ExtensionsApi.cs's sound types.
[Flags]
public enum SoundOptions
{
    None = 0,
    Decode = 1,
    BigEndian = 2,
    Valid = BigEndian | Decode,
}

[StructLayout(LayoutKind.Sequential)]
public struct SoundHeader
{
    public ushort formatTag;
    public ushort channels;
    public uint samplesPerSec;
    public uint avgBytesPerSec;
    public ushort blockAlign;
    public ushort bitsPerSample;
    public ushort cbExtraData;
    public uint cbDataSize;
}

[StructLayout(LayoutKind.Sequential)]
public struct SoundData
{
    public IntPtr rgData;
}

[StructLayout(LayoutKind.Sequential)]
public struct SoundInformation
{
    public SoundHeader header;
    public SoundData data;
}

[StructLayout(LayoutKind.Sequential)]
public struct HSpSound
{
    public IntPtr h;
    public static readonly HSpSound NULL = new();
}
