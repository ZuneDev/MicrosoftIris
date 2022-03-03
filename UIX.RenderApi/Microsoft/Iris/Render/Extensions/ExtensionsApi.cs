// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.ExtensionsApi
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Iris.Render.Extensions
{
    [SuppressUnmanagedCodeSecurity]
    public static class ExtensionsApi
    {
        private const string s_stExtensionsDll = "UIXRender.dll";
        internal const ushort WAVE_FORMAT_PCM = 1;

        [DllImport("UIXRender.dll", CharSet = CharSet.Auto)]
        internal static extern HRESULT SpBitmapLoadFile(
          string stFileName,
          [MarshalAs(UnmanagedType.LPStruct), In] ImageRequirements req,
          ExtensionsApi.BitmapOptions nOptions,
          out HSpBitmap hBmp,
          out ImageInformation info);

        [DllImport("UIXRender.dll", CharSet = CharSet.Auto)]
        internal static extern HRESULT SpBitmapLoadRaw(
          Size sizeActualPxl,
          int nStride,
          SurfaceFormat nFormat,
          IntPtr pvData,
          [MarshalAs(UnmanagedType.LPStruct), In] ImageRequirements req,
          ExtensionsApi.BitmapOptions nOptions,
          out HSpBitmap hBmp,
          out ImageInformation info);

        [DllImport("UIXRender.dll", CharSet = CharSet.Auto)]
        internal static extern HRESULT SpBitmapLoadResource(
          Win32Api.HINSTANCE hinst,
          string stName,
          int nType,
          [MarshalAs(UnmanagedType.LPStruct), In] ImageRequirements req,
          ExtensionsApi.BitmapOptions nOptions,
          out HSpBitmap hBmp,
          out ImageInformation info);

        [DllImport("UIXRender.dll", CharSet = CharSet.Auto)]
        internal static extern HRESULT SpBitmapLoadBuffer(
          IntPtr pvSrc,
          uint cbSize,
          [MarshalAs(UnmanagedType.LPStruct), In] ImageRequirements req,
          ExtensionsApi.BitmapOptions nOptions,
          out HSpBitmap hBmp,
          out ImageInformation info);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpBitmapDelete(HSpBitmap hBmp);

        [DllImport("UIXRender.dll", CharSet = CharSet.Auto)]
        internal static extern HRESULT SpSoundLoadBuffer(
          IntPtr pBuffer,
          int dwSize,
          ExtensionsApi.SoundOptions options,
          out ExtensionsApi.HSpSound hSound,
          out ExtensionsApi.SoundInformation info);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpSoundDispose(
          ExtensionsApi.HSpSound hSound,
          ExtensionsApi.SoundInformation info);

        [System.Flags]
        internal enum BitmapOptions
        {
            None = 0,
            Decode = 1,
            Flip = 2,
            Valid = Flip | Decode, // 0x00000003
        }

        [System.Flags]
        internal enum SoundOptions
        {
            None = 0,
            Decode = 1,
            BigEndian = 2,
            Valid = BigEndian | Decode, // 0x00000003
        }

        [ComVisible(false)]
        public struct SoundHeader
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbExtraData;
            public uint cbDataSize;
        }

        [ComVisible(false)]
        public struct SoundData
        {
            public IntPtr rgData;
        }

        [ComVisible(false)]
        public struct SoundInformation
        {
            public ExtensionsApi.SoundHeader Header;
            public ExtensionsApi.SoundData Data;
            public static ExtensionsApi.SoundInformation NULL = new ExtensionsApi.SoundInformation();
        }

        [ComVisible(false)]
        public struct HSpSound
        {
            public IntPtr h;
            public static readonly ExtensionsApi.HSpSound NULL = new ExtensionsApi.HSpSound();

            public static bool operator ==(ExtensionsApi.HSpSound hA, ExtensionsApi.HSpSound hB) => hA.h == hB.h;

            public static bool operator !=(ExtensionsApi.HSpSound hA, ExtensionsApi.HSpSound hB) => hA.h != hB.h;

            public override bool Equals(object oCompare) => oCompare is ExtensionsApi.HSpSound hspSound && this.h == hspSound.h;

            public override int GetHashCode() => (int)this.h.ToInt64();
        }
    }
}
