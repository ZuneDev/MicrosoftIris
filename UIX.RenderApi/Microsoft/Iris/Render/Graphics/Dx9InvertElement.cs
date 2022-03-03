// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9InvertElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9InvertElement
    {
        internal static void Generate(InvertElement efoInvert, ref Dx9EffectBuilder effectBuilder) => effectBuilder.EmitPixelFragment(InvariantString.Format("    {{\r\n        {0}.rgb = 1.0f - {0}.rgb;\r\n    }}\r\n", effectBuilder.PixelShaderOutput));
    }
}
