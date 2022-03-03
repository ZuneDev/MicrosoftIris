// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9DesaturateElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9DesaturateElement
    {
        internal static void Generate(
          DesaturateElement efoDesaturate,
          ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo = new VariableInfo()
            {
                ID = efoDesaturate.DesaturateID,
                Type = Dx9VariableType.Float,
                DefaultValue = efoDesaturate.Desaturate,
                IsDynamic = efoDesaturate.IsDynamicProperty("Desaturate")
            };
            variableInfo.Name = variableInfo.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo.Type, efoDesaturate.Name) : effectBuilder.GenerateGlobalConstant(variableInfo.Type, efoDesaturate.Name);
            effectBuilder.AddPropertyVariable(variableInfo);
            string name = variableInfo.Name;
            effectBuilder.EmitPixelFragment(InvariantString.Format("    // Desaturate the value\r\n    {{\r\n        float3 vGray = {{.15, .55, .30}};\r\n        float fLuminance = dot({0}.rgb, vGray);\r\n        {0}.rgb = lerp({0}.rgb, fLuminance.rrr, {1});\r\n\r\n    }}\r\n", effectBuilder.PixelShaderOutput, name));
        }
    }
}
