// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.TextureVariableInfo
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Graphics
{
    internal class TextureVariableInfo : VariableInfo
    {
        internal string SamplerName;
        internal string MinFilter;
        internal string MagFilter;
        internal int CoordinateMapID;
        internal int ImageIndexID;

        public TextureVariableInfo() => this.IsDynamic = true;
    }
}
