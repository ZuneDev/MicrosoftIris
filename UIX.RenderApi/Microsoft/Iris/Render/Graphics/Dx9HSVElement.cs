// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9HSVElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9HSVElement
    {
        internal static void Generate(HSVElement efoHSV, ref Dx9EffectBuilder effectBuilder)
        {
            effectBuilder.EmitIncludesFragment("\"HSV.fx\"");
            VariableInfo variableInfo1 = new VariableInfo()
            {
                ID = efoHSV.HueID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoHSV.IsDynamicProperty("Hue")
            };
            variableInfo1.Name = variableInfo1.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo1.Type, efoHSV.Name + "Hue") : effectBuilder.GenerateGlobalConstant(variableInfo1.Type, efoHSV.Name + "Hue");
            variableInfo1.DefaultValue = efoHSV.Hue;
            effectBuilder.AddPropertyVariable(variableInfo1);
            VariableInfo variableInfo2 = new VariableInfo()
            {
                ID = efoHSV.SaturationID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoHSV.IsDynamicProperty("Saturation")
            };
            variableInfo2.Name = variableInfo2.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo2.Type, efoHSV.Name + "Saturation") : effectBuilder.GenerateGlobalConstant(variableInfo2.Type, efoHSV.Name + "Saturation");
            variableInfo2.DefaultValue = efoHSV.Saturation;
            effectBuilder.AddPropertyVariable(variableInfo2);
            VariableInfo variableInfo3 = new VariableInfo()
            {
                ID = efoHSV.ValueID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoHSV.IsDynamicProperty("Value")
            };
            variableInfo3.Name = variableInfo3.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo3.Type, efoHSV.Name + "Value") : effectBuilder.GenerateGlobalConstant(variableInfo3.Type, efoHSV.Name + "Value");
            variableInfo3.DefaultValue = efoHSV.Value;
            effectBuilder.AddPropertyVariable(variableInfo3);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    {{\r\n        // convert to HSV color space\r\n        float3 fHSV = RGBToHSV({0});\r\n\r\n        // adjust Hue, Sat and Value\r\n        fHSV *= float3(1.0f, {2}, {3});\r\n        fHSV[0] += {0};\r\n        fHSV = saturate(fHSV);\r\n\r\n        // convert back to RGB space\r\n        float3 fBackToRGB = HSVToRGB(fHSV);\r\n        {0}.rgb = fBackToRGB;\r\n    }}\r\n", effectBuilder.PixelShaderOutput, variableInfo1.Name, variableInfo2.Name, variableInfo3.Name));
        }
    }
}
