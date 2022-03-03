// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9InterpolateElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9InterpolateElement
    {
        internal static void Generate(
          InterpolateElement efiInterpolate,
          string stSource1,
          string stSource2,
          ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo = new VariableInfo()
            {
                ID = efiInterpolate.ValueID,
                Type = Dx9VariableType.Float,
                IsDynamic = efiInterpolate.IsDynamicProperty("Value")
            };
            variableInfo.Name = variableInfo.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo.Type, efiInterpolate.Name) : effectBuilder.GenerateGlobalConstant(variableInfo.Type, efiInterpolate.Name);
            variableInfo.DefaultValue = efiInterpolate.Value;
            effectBuilder.AddPropertyVariable(variableInfo);
            effectBuilder.PixelShaderOutput = effectBuilder.GenerateLocalVariable(Dx9VariableType.Vector4, efiInterpolate.Name);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    // Interpolate the result\r\n    float4 {0} = lerp({1}, {2}, {3});\r\n\r\n", effectBuilder.PixelShaderOutput, stSource1, stSource2, variableInfo.Name));
        }
    }
}
