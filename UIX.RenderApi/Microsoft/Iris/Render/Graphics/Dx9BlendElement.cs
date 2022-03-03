// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9BlendElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render.Common;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9BlendElement
    {
        internal static void Generate(
          BlendElement efiBlend,
          string stBlendOutput1,
          string stBlendOutput2,
          ref Dx9EffectBuilder effectBuilder)
        {
            effectBuilder.PixelShaderOutput = effectBuilder.GenerateLocalVariable(Dx9VariableType.Vector4, efiBlend.Name);
            string str1 = null;
            switch (efiBlend.ColorOperation)
            {
                case ColorOperation.Add:
                    str1 = InvariantString.Format("    // Blend the RGB - result\r\n    float4 {0};\r\n    {0}.rgb = {1}.rgb + {2}.rgb;\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case ColorOperation.Subtract:
                    str1 = InvariantString.Format("    // Blend the RGB - result\r\n    float4 {0};\r\n    {0}.rgb = float3(abs({1}.rgb - {2}.rgb));\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case ColorOperation.Multiply:
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float4 {0};\r\n    {0}.rgb = {1}.rgb * {2}.rgb;\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case ColorOperation.Lighten:
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float4 {0};\r\n    {0}.rgb = max({1}.rgb, {2}.rgb);\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case ColorOperation.Darken:
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float4 {0};\r\n    {0}.rgb = min({1}.rgb, {2}.rgb);\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case ColorOperation.ColorDodge:
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float4 {0};\r\n    {0}.rgb = {1}.rgb / (1 - {2}.rgb);\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case ColorOperation.ColorBurn:
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float4 {0};\r\n    {0}.rgb = 1 - ((1 - {1}.rgb) / {2}.rgb);\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case ColorOperation.Screen:
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float4 {0};\r\n    {0}.rgb = 1 - ((1 - {1}.rgb) * (1 - {2}.rgb));\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case ColorOperation.Overlay:
                    string localVariable1 = effectBuilder.GenerateLocalVariable(Dx9VariableType.Float, "trueClause");
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float3 {3} = ceil({1}.rgb - 0.5);\r\n    float4 {0};\r\n    {0}.rgb = {3}.rgb * (1 - (1 - 2 * ({1}.rgb - 0.5)) * (1 - {2}.rgb)) \r\n                 + (1 - {3}.rgb) * (2 * {1}.rgb * {2}.rgb);\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2, localVariable1);
                    break;
                case ColorOperation.SoftLight:
                    string localVariable2 = effectBuilder.GenerateLocalVariable(Dx9VariableType.Float, "trueClause");
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float3 {3} = ceil({2}.rgb - 0.5);\r\n    float4 {0};\r\n    {0}.rgb = {3}.rgb * (1 - (2 * (1 - (({1}.rgb / 2) + 0.25)) * (1 - {2}.rgb)))\r\n                 + (1 - {3}.rgb) * (2 * ({1}.rgb / 2 + 0.25) * {2}.rgb);\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2, localVariable2);
                    break;
                case ColorOperation.HardLight:
                    string localVariable3 = effectBuilder.GenerateLocalVariable(Dx9VariableType.Float, "trueClause");
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float3 {3} = ceil({2}.rgb - 0.5);\r\n    float4 {0};\r\n    {0}.rgb = {3}.rgb * (1 - (1 - 2 * ({2}.rgb - 0.5)) * (1 - {1}.rgb)) \r\n                 + (1 - {3}.rgb) * (2 * {1}.rgb * {2}.rgb);\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2, localVariable3);
                    break;
                case ColorOperation.LinearBurn:
                    str1 = InvariantString.Format("    // Blend the RGB result\r\n    float4 {0};\r\n    {0}.rgb = {1}.rgb + {2}.rgb - 1;\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                default:
                    Debug2.Throw(false, "Unknown color blend operation");
                    break;
            }
            string str2 = null;
            switch (efiBlend.AlphaOperation)
            {
                case AlphaOperation.Add:
                    str2 = InvariantString.Format("    // Blend the Alpha result\r\n    {0}.a = {1}.a + {2}.a;\r\n\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case AlphaOperation.Subtract:
                    str2 = InvariantString.Format("    // Blend the Alpha result\r\n    {0}.a = {1}.a - {2}.a;\r\n\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case AlphaOperation.Multiply:
                    str2 = InvariantString.Format("    // Blend the Alpha result\r\n    {0}.a = {1}.a * {2}.a;\r\n\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1, stBlendOutput2);
                    break;
                case AlphaOperation.Source1:
                    str2 = InvariantString.Format("    // Blend the Alpha result\r\n    {0}.a = {1}.a;\r\n\r\n", effectBuilder.PixelShaderOutput, stBlendOutput1);
                    break;
                case AlphaOperation.Source2:
                    str2 = InvariantString.Format("    // Blend the Alpha result\r\n    {0}.a = {1}.a;\r\n\r\n", effectBuilder.PixelShaderOutput, stBlendOutput2);
                    break;
                default:
                    Debug2.Throw(false, "Unknown alpha blend operation");
                    break;
            }
            effectBuilder.EmitPixelFragment(str1 + str2);
        }
    }
}
