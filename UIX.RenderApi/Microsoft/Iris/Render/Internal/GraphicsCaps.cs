// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.GraphicsCaps
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Internal
{
    [ComVisible(false)]
    [Serializable]
    internal struct GraphicsCaps
    {
        internal uint DeviceType;
        internal int MaxSimultaneousTextures;
        internal int MaxTextureWidth;
        internal int MaxTextureHeight;
        internal ulong DedicatedVideoMemory;
        internal ulong TotalVideoMemory;
        internal uint PixelShaderProfile;
        internal uint VertexShaderProfile;
        internal int DriverWarning;
        internal int AvailableRenderTargets;
    }
}
