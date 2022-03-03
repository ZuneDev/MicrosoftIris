// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ISoundData
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    public interface ISoundData
    {
        SoundDataFormat Format { get; }

        uint ChannelCount { get; }

        uint SampleRate { get; }

        uint SampleSize { get; }

        uint SampleCount { get; }

        IntPtr AcquireContent();

        void ReleaseContent();
    }
}
