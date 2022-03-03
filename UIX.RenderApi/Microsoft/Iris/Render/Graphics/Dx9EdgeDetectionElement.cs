// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9EdgeDetectionElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9EdgeDetectionElement
    {
        internal static void Generate(
          EdgeDetectionElement efoEdgeDetection,
          ref Dx9EffectBuilder effectBuilder)
        {
            Dx9TextureInfo textureInfo = effectBuilder.GetTextureInfo(effectBuilder.ImageCount - 1);
            effectBuilder.AddRequirement(textureInfo, Dx9TextureRequirements.TexelSize, null);
            VariableInfo variableInfo = new VariableInfo()
            {
                ID = efoEdgeDetection.EdgeLimitID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoEdgeDetection.IsDynamicProperty("EdgeLimit")
            };
            variableInfo.Name = variableInfo.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo.Type, efoEdgeDetection.Name) : effectBuilder.GenerateGlobalConstant(variableInfo.Type, efoEdgeDetection.Name);
            variableInfo.DefaultValue = efoEdgeDetection.EdgeLimit;
            effectBuilder.AddPropertyVariable(variableInfo);
            effectBuilder.EmitPixelFragment(InvariantString.Format("     {{\r\n         // Sample neightbour pixel\r\n         float4 vBottomPixel = tex2D({1}, {2} + float2(0.0f, {3}.y));\r\n         float currentAvg = {0}.r + {0}.g + {0}.b;\r\n         currentAvg = currentAvg / 3.0f;\r\n         float bottomAvg = vBottomPixel.r + vBottomPixel.g + vBottomPixel.b;\r\n         bottomAvg = bottomAvg / 3.0f;\r\n         float diff = abs(currentAvg - bottomAvg);\r\n         // change color into black or white depending on what the delta is\r\n         if (diff < {4})\r\n         {{\r\n             {0}.rgb = 1.0f;\r\n         }}\r\n         else\r\n         {{\r\n             {0}.rgb = 0.0f;\r\n         }}\r\n     }}\r\n\r\n", effectBuilder.PixelShaderOutput, textureInfo.Sampler, textureInfo.TexCoordInput, textureInfo.TexelSize, variableInfo.Name));
        }
    }
}
