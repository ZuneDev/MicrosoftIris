// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9DestinationElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9DestinationElement
    {
        internal static void Generate(
          DestinationElement efiDestination,
          ref Dx9EffectBuilder effectBuilder)
        {
            Dx9TextureInfo textureInfo = effectBuilder.AllocateTexture();
            effectBuilder.AddRequirement(textureInfo, Dx9TextureRequirements.SampleBackbuffer, (int)efiDestination.DownsampleID);
            effectBuilder.AddRequirement(textureInfo, Dx9TextureRequirements.DisablePerspectiveCorrection, null);
            effectBuilder.AddRequirement(textureInfo, Dx9TextureRequirements.TexelSize, null);
            TextureVariableInfo textureVariableInfo = new TextureVariableInfo();
            textureVariableInfo.ID = efiDestination.DestinationID;
            textureVariableInfo.Type = Dx9VariableType.Texture;
            textureVariableInfo.Name = effectBuilder.GenerateGlobalVariable(textureVariableInfo.Type, efiDestination.Name);
            textureVariableInfo.DefaultValue = null;
            textureVariableInfo.SamplerName = textureInfo.Sampler;
            textureVariableInfo.MinFilter = "Linear";
            textureVariableInfo.MagFilter = "Linear";
            textureVariableInfo.CoordinateMapID = -1;
            textureVariableInfo.ImageIndexID = -1;
            effectBuilder.AddPropertyVariable(textureVariableInfo);
            effectBuilder.AddPropertyVariable(new VariableInfo()
            {
                ID = efiDestination.DownsampleID,
                Type = Dx9VariableType.Float,
                IsDynamic = efiDestination.IsDynamicProperty("Downsample"),
                Name = null,
                DefaultValue = efiDestination.Downsample
            });
            VariableInfo variableInfo = new VariableInfo()
            {
                ID = efiDestination.UVOffsetID,
                Type = Dx9VariableType.Vector2,
                IsDynamic = efiDestination.IsDynamicProperty("UVOffset")
            };
            variableInfo.Name = variableInfo.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo.Type, "UVOffset") : effectBuilder.GenerateGlobalConstant(variableInfo.Type, "UVOffset");
            variableInfo.DefaultValue = efiDestination.UVOffset;
            effectBuilder.AddPropertyVariable(variableInfo);
            effectBuilder.PixelShaderOutput = effectBuilder.GenerateLocalVariable(Dx9VariableType.Texture, efiDestination.Name);
            if (variableInfo.IsDynamic || !efiDestination.UVOffset.IsApproximate(Vector2.Zero))
                effectBuilder.EmitPixelFragment(InvariantString.Format("    // Load the image\r\n    float4 {0} = tex2D({1}, {2} + float2({3}.x * {4}.x, {3}.y * {4}.y));\r\n\r\n", effectBuilder.PixelShaderOutput, textureInfo.Sampler, textureInfo.TexCoordInput, textureInfo.TexelSize, variableInfo.Name));
            else
                effectBuilder.EmitPixelFragment(InvariantString.Format("    // Load the image\r\n    float4 {0} = tex2D({1}, {2});\r\n\r\n", effectBuilder.PixelShaderOutput, textureInfo.Sampler, textureInfo.TexCoordInput));
        }
    }
}
