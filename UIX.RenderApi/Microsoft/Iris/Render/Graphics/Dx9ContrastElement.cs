// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9ContrastElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9ContrastElement
    {
        internal static void Generate(ContrastElement efoContrast, ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo = new VariableInfo()
            {
                ID = efoContrast.ContrastID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoContrast.IsDynamicProperty("Contrast")
            };
            variableInfo.Name = variableInfo.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo.Type, efoContrast.Name) : effectBuilder.GenerateGlobalConstant(variableInfo.Type, efoContrast.Name);
            variableInfo.DefaultValue = efoContrast.Contrast;
            effectBuilder.AddPropertyVariable(variableInfo);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    {{\r\n        // apply the gain factor \r\n        {0}.rgb -= float3(0.5f, 0.5f, 0.5f)\r\n;        {0}.rgb *= float3({1}, {1}, {1});\r\n        {0}.rgb += float3(0.5f, 0.5f, 0.5f)\r\n;    }}\r\n", effectBuilder.PixelShaderOutput, variableInfo.Name));
        }
    }
}
