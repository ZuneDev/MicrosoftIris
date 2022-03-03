// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9BrightnessElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9BrightnessElement
    {
        internal static void Generate(
          BrightnessElement efoBrightness,
          ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo = new VariableInfo()
            {
                ID = efoBrightness.BrightnessID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoBrightness.IsDynamicProperty("Brightness")
            };
            variableInfo.Name = variableInfo.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo.Type, efoBrightness.Name) : effectBuilder.GenerateGlobalConstant(variableInfo.Type, efoBrightness.Name);
            variableInfo.DefaultValue = efoBrightness.Brightness;
            effectBuilder.AddPropertyVariable(variableInfo);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    {{\r\n        // adjust brightness based on input\r\n        {0}.rgb += float3({1}, {1}, {1});\r\n    }}\r\n", effectBuilder.PixelShaderOutput, variableInfo.Name));
        }
    }
}
