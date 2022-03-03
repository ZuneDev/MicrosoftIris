// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9LightShaftElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9LightShaftElement
    {
        internal static void Generate(
          LightShaftElement efoLightShaft,
          ref Dx9EffectBuilder effectBuilder)
        {
            Dx9TextureInfo textureInfo = effectBuilder.GetTextureInfo(effectBuilder.ImageCount - 1);
            effectBuilder.AddRequirement(textureInfo, Dx9TextureRequirements.TexUVSize, null);
            effectBuilder.AddRequirement(textureInfo, Dx9TextureRequirements.TexUVRefPoint, null);
            VariableInfo variableInfo1 = new VariableInfo()
            {
                ID = efoLightShaft.PositionID,
                Type = Dx9VariableType.Vector3,
                IsDynamic = efoLightShaft.IsDynamicProperty("Position")
            };
            variableInfo1.Name = variableInfo1.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo1.Type, efoLightShaft.Name + "Position") : effectBuilder.GenerateGlobalConstant(variableInfo1.Type, efoLightShaft.Name + "Position");
            variableInfo1.DefaultValue = efoLightShaft.Position;
            effectBuilder.AddPropertyVariable(variableInfo1);
            VariableInfo variableInfo2 = new VariableInfo()
            {
                ID = efoLightShaft.DecayID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoLightShaft.IsDynamicProperty("Decay")
            };
            variableInfo2.Name = variableInfo2.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo2.Type, efoLightShaft.Name + "Decay") : effectBuilder.GenerateGlobalConstant(variableInfo2.Type, efoLightShaft.Name + "Decay");
            variableInfo2.DefaultValue = efoLightShaft.Decay;
            effectBuilder.AddPropertyVariable(variableInfo2);
            VariableInfo variableInfo3 = new VariableInfo()
            {
                ID = efoLightShaft.DensityID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoLightShaft.IsDynamicProperty("Density")
            };
            variableInfo3.Name = variableInfo3.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo3.Type, efoLightShaft.Name + "Density") : effectBuilder.GenerateGlobalConstant(variableInfo3.Type, efoLightShaft.Name + "Density");
            variableInfo3.DefaultValue = efoLightShaft.Density;
            effectBuilder.AddPropertyVariable(variableInfo3);
            VariableInfo variableInfo4 = new VariableInfo()
            {
                ID = efoLightShaft.FallOffID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoLightShaft.IsDynamicProperty("FallOff")
            };
            variableInfo4.Name = variableInfo4.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo4.Type, efoLightShaft.Name + "FallOff") : effectBuilder.GenerateGlobalConstant(variableInfo4.Type, efoLightShaft.Name + "FallOff");
            variableInfo4.DefaultValue = efoLightShaft.FallOff;
            effectBuilder.AddPropertyVariable(variableInfo4);
            VariableInfo variableInfo5 = new VariableInfo()
            {
                ID = efoLightShaft.IntensityID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoLightShaft.IsDynamicProperty("Intensity")
            };
            variableInfo5.Name = variableInfo5.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo5.Type, efoLightShaft.Name + "Intensity") : effectBuilder.GenerateGlobalConstant(variableInfo5.Type, efoLightShaft.Name + "Intensity");
            variableInfo5.DefaultValue = efoLightShaft.Intensity;
            effectBuilder.AddPropertyVariable(variableInfo5);
            VariableInfo variableInfo6 = new VariableInfo()
            {
                ID = efoLightShaft.WeightID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoLightShaft.IsDynamicProperty("Weight")
            };
            variableInfo6.Name = variableInfo6.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo6.Type, efoLightShaft.Name + "Weight") : effectBuilder.GenerateGlobalConstant(variableInfo6.Type, efoLightShaft.Name + "Weight");
            variableInfo6.DefaultValue = efoLightShaft.Weight;
            effectBuilder.AddPropertyVariable(variableInfo6);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    {{\r\n        float2 diff   = {2} - ({4} + {3}.xy * {5});\r\n        float2 rayStep = diff / 15 * {6};\r\n        float4 color  = tex2D({1}, {2});\r\n        float  fadeFactor = 1.0f;\r\n        for (int i=0; i<15; i++)\r\n        {{\r\n           {2} -= rayStep;\r\n           float4 sample = tex2D({1}, {2});\r\n           sample *= fadeFactor * {7};\r\n           color += sample;\r\n           fadeFactor *= {8};\r\n         }}\r\n        color.rgb *= {9};\r\n        {0} = color * ({10} - length(diff));\r\n    }}\r\n", effectBuilder.PixelShaderOutput, textureInfo.Sampler, textureInfo.TexCoordInput, variableInfo1.Name, textureInfo.TexUVRefPoint, textureInfo.TexUVSize, variableInfo3.Name, variableInfo6.Name, variableInfo2.Name, variableInfo5.Name, variableInfo4.Name));
        }
    }
}
