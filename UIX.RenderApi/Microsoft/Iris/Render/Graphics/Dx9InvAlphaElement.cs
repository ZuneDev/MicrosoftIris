// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9InvAlphaElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9InvAlphaElement
    {
        internal static void Generate(EffectOperation efo, ref Dx9EffectBuilder effectBuilder)
        {
            string localVariable = effectBuilder.GenerateLocalVariable(Dx9VariableType.Float, "InvAlpha");
            effectBuilder.EmitPixelFragment(InvariantString.Format("    // Invert the alpha value\r\n    float {1} = 1.0 - {0}.a;\r\n    {0}.a = {1};\r\n\r\n", effectBuilder.PixelShaderOutput, localVariable));
        }
    }
}
