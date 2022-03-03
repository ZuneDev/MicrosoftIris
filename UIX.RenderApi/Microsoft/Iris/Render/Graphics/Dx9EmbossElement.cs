// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9EmbossElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9EmbossElement
    {
        internal static void Generate(EmbossElement efoEmboss, ref Dx9EffectBuilder effectBuilder)
        {
            Dx9TextureInfo textureInfo = effectBuilder.GetTextureInfo(effectBuilder.ImageCount - 1);
            effectBuilder.AddRequirement(textureInfo, Dx9TextureRequirements.TexelSize, null);
            VariableInfo variableInfo1 = new VariableInfo()
            {
                ID = efoEmboss.DirectionID,
                Type = Dx9VariableType.Integer,
                IsDynamic = efoEmboss.IsDynamicProperty("Direction")
            };
            variableInfo1.Name = variableInfo1.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo1.Type, efoEmboss.Name + "Direction") : effectBuilder.GenerateGlobalConstant(variableInfo1.Type, efoEmboss.Name + "Direction");
            variableInfo1.DefaultValue = efoEmboss.Direction;
            effectBuilder.AddPropertyVariable(variableInfo1);
            VariableInfo variableInfo2 = new VariableInfo()
            {
                ID = efoEmboss.GrayLevelID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoEmboss.IsDynamicProperty("GrayLevel")
            };
            variableInfo2.Name = variableInfo2.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo2.Type, efoEmboss.Name + "GrayLevel") : effectBuilder.GenerateGlobalConstant(variableInfo2.Type, efoEmboss.Name + "GrayLevel");
            variableInfo2.DefaultValue = efoEmboss.GrayLevel;
            effectBuilder.AddPropertyVariable(variableInfo2);
            effectBuilder.EmitPixelFragment(InvariantString.Format("     {{\r\n         // Sample neightbour pixel\r\n         float4 vNeightbourPixel;\r\n         if ({5} == 0)\r\n         {{\r\n            vNeightbourPixel = tex2D({1}, {2} + float2(-{3}.x, -{3}.y));\r\n         }}\r\n         else\r\n         {{\r\n            vNeightbourPixel = tex2D({1}, {2} + float2(+{3}.x, +{3}.y));\r\n         }}\r\n         float4 vDiff = {0} - vNeightbourPixel;\r\n         float maxDiffColor = vDiff.r;\r\n         if (abs(vDiff.g) > abs(maxDiffColor))\r\n         {{\r\n             maxDiffColor = vDiff.g;\r\n         }}\r\n         if (abs(vDiff.b) > abs(maxDiffColor))\r\n         {{\r\n             maxDiffColor = vDiff.b;\r\n         }}\r\n         maxDiffColor += {4};\r\n         float minGray = min(maxDiffColor, 1.0f);\r\n         float grayLevel = max(minGray, 0.0f);\r\n         {0}.rgb = grayLevel;\r\n     }}\r\n\r\n", effectBuilder.PixelShaderOutput, textureInfo.Sampler, textureInfo.TexCoordInput, textureInfo.TexelSize, variableInfo2.Name, variableInfo1.Name));
        }
    }
}
