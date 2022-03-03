// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9InvColorElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9InvColorElement
    {
        internal static void Generate(EffectOperation efo, ref Dx9EffectBuilder effectBuilder) => effectBuilder.EmitPixelFragment(InvariantString.Format("    // Invert the color value\r\n    {0}.rgb = float3(1.0-{0}.r, 1.0-{0}.g, 1.0-{0}.b);\r\n\r\n", effectBuilder.PixelShaderOutput));
    }
}
