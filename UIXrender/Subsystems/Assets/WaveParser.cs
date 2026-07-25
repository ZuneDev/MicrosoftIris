using System;
using System.Buffers.Binary;
using Microsoft.Iris.Render.Interop.Extensions;

namespace Microsoft.Iris.Render.Subsystems.Assets;

// Real RIFF/WAVE container parser for SpSoundLoadBuffer. Deliberately minimal and
// allocation-light: it locates the "fmt " and "data" chunks, fills the SoundHeader the
// managed side already declares (ExtensionsApi.SoundHeader -- field-for-field a
// WAVEFORMATEX plus the data size), and returns the raw sample bytes.
internal static class WaveParser
{
    private const ushort WAVE_FORMAT_PCM = 1;

#if NETCOREAPP
    public static bool TryParse(ReadOnlySpan<byte> source, out SoundHeader header, out byte[] samples)
    {
        header = default;
        samples = Array.Empty<byte>();

        // "RIFF" <size> "WAVE" then a chunk list.
        if (source.Length < 12 ||
            !source[..4].SequenceEqual("RIFF"u8) ||
            !source.Slice(8, 4).SequenceEqual("WAVE"u8))
            return false;

        bool haveFormat = false;
        int offset = 12;

        while (offset + 8 <= source.Length)
        {
            ReadOnlySpan<byte> chunkId = source.Slice(offset, 4);
            uint chunkSize = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(offset + 4, 4));
            int body = offset + 8;

            if (body + chunkSize > source.Length)
                chunkSize = (uint)(source.Length - body);

            if (chunkId.SequenceEqual("fmt "u8) && chunkSize >= 16)
            {
                ReadOnlySpan<byte> fmt = source.Slice(body, (int)chunkSize);
                header.formatTag = BinaryPrimitives.ReadUInt16LittleEndian(fmt[..2]);
                header.channels = BinaryPrimitives.ReadUInt16LittleEndian(fmt.Slice(2, 2));
                header.samplesPerSec = BinaryPrimitives.ReadUInt32LittleEndian(fmt.Slice(4, 4));
                header.avgBytesPerSec = BinaryPrimitives.ReadUInt32LittleEndian(fmt.Slice(8, 4));
                header.blockAlign = BinaryPrimitives.ReadUInt16LittleEndian(fmt.Slice(12, 2));
                header.bitsPerSample = BinaryPrimitives.ReadUInt16LittleEndian(fmt.Slice(14, 2));
                header.cbExtraData = chunkSize >= 18 ? BinaryPrimitives.ReadUInt16LittleEndian(fmt.Slice(16, 2)) : (ushort)0;
                haveFormat = true;
            }
            else if (chunkId.SequenceEqual("data"u8))
            {
                samples = source.Slice(body, (int)chunkSize).ToArray();
                header.cbDataSize = chunkSize;
            }

            // Chunks are word-aligned: an odd-sized chunk is followed by a pad byte.
            offset = body + (int)chunkSize + ((chunkSize & 1) != 0 ? 1 : 0);
        }

        // Only uncompressed PCM is representable through this surface -- the managed side
        // declares WAVE_FORMAT_PCM as its sole format constant, so anything else would be
        // silently misinterpreted downstream rather than merely unsupported.
        return haveFormat && samples.Length > 0 && header.formatTag == WAVE_FORMAT_PCM;
    }
#endif

    public static bool TryParse(byte[] source, out SoundHeader header, out byte[] samples)
    {
        header = default;
        samples = Array.Empty<byte>();

        // "RIFF" <size> "WAVE" then a chunk list.
        if (source.Length < 12 ||
            !(source[0] == 'R' && source[1] == 'I' && source[2] == 'F' && source[3] == 'F') ||
            !(source[8] == 'W' && source[9] == 'A' && source[10] == 'V' && source[11] == 'E'))
            return false;

        bool haveFormat = false;
        int offset = 12;

        while (offset + 8 <= source.Length)
        {
            string chunkId = System.Text.Encoding.UTF8.GetString(source, offset, 4);
            int body = offset + 8;

            var chunkSizeOffset = offset + 4;
            //uint chunkSize = source[chunkSizeOffset] | source[chunkSizeOffset + 1];
            uint chunkSize = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(offset + 4, 4));

            if (body + chunkSize > source.Length)
                chunkSize = (uint)(source.Length - body);

            if (chunkId == "fmt " && chunkSize >= 16)
            {
                ReadOnlySpan<byte> fmt = source.Slice(body, (int)chunkSize);
                header.formatTag = BinaryPrimitives.ReadUInt16LittleEndian(fmt[..2]);
                header.channels = BinaryPrimitives.ReadUInt16LittleEndian(fmt.Slice(2, 2));
                header.samplesPerSec = BinaryPrimitives.ReadUInt32LittleEndian(fmt.Slice(4, 4));
                header.avgBytesPerSec = BinaryPrimitives.ReadUInt32LittleEndian(fmt.Slice(8, 4));
                header.blockAlign = BinaryPrimitives.ReadUInt16LittleEndian(fmt.Slice(12, 2));
                header.bitsPerSample = BinaryPrimitives.ReadUInt16LittleEndian(fmt.Slice(14, 2));
                header.cbExtraData = chunkSize >= 18 ? BinaryPrimitives.ReadUInt16LittleEndian(fmt.Slice(16, 2)) : (ushort)0;
                haveFormat = true;
            }
            else if (chunkId == "data")
            {
                samples = source.Slice(body, (int)chunkSize).ToArray();
                header.cbDataSize = chunkSize;
            }

            // Chunks are word-aligned: an odd-sized chunk is followed by a pad byte.
            offset = body + (int)chunkSize + ((chunkSize & 1) != 0 ? 1 : 0);
        }

        // Only uncompressed PCM is representable through this surface -- the managed side
        // declares WAVE_FORMAT_PCM as its sole format constant, so anything else would be
        // silently misinterpreted downstream rather than merely unsupported.
        return haveFormat && samples.Length > 0 && header.formatTag == WAVE_FORMAT_PCM;
    }
}
