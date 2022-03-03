// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9ColorElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9ColorElement
    {
        internal static void Generate(ColorElement efiColor, ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo = new VariableInfo()
            {
                ID = efiColor.ColorID,
                Type = Dx9VariableType.Vector4,
                IsDynamic = efiColor.IsDynamicProperty("Color")
            };
            variableInfo.Name = variableInfo.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo.Type, efiColor.Name) : effectBuilder.GenerateGlobalConstant(variableInfo.Type, efiColor.Name);
            variableInfo.DefaultValue = efiColor.Color.ToVector4();
            effectBuilder.AddPropertyVariable(variableInfo);
            string name = variableInfo.Name;
            effectBuilder.PixelShaderOutput = effectBuilder.GenerateLocalVariable(Dx9VariableType.Vector4, efiColor.Name);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    // Load the color\r\n    float4 {0} = {1};\r\n\r\n", effectBuilder.PixelShaderOutput, name));
        }
    }
}
