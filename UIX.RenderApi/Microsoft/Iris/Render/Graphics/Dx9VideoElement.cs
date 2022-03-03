// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9VideoElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9VideoElement
    {
        internal static void Generate(VideoElement efiVideo, ref Dx9EffectBuilder effectBuilder)
        {
            Dx9TextureInfo dx9TextureInfo = effectBuilder.AllocateTexture();
            TextureVariableInfo textureVariableInfo = new TextureVariableInfo();
            textureVariableInfo.ID = efiVideo.VideoStreamID;
            textureVariableInfo.Type = Dx9VariableType.Texture;
            textureVariableInfo.Name = effectBuilder.GenerateGlobalVariable(textureVariableInfo.Type, efiVideo.Name);
            textureVariableInfo.DefaultValue = efiVideo.VideoStream;
            textureVariableInfo.SamplerName = dx9TextureInfo.Sampler;
            textureVariableInfo.MinFilter = "Linear";
            textureVariableInfo.MagFilter = "Linear";
            textureVariableInfo.CoordinateMapID = -1;
            textureVariableInfo.ImageIndexID = -1;
            effectBuilder.AddPropertyVariable(textureVariableInfo);
            effectBuilder.PixelShaderOutput = effectBuilder.GenerateLocalVariable(Dx9VariableType.Vector4, efiVideo.Name);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    // Load the surface\r\n    float4 {0} = tex2D({1}, {2});\r\n\r\n", effectBuilder.PixelShaderOutput, dx9TextureInfo.Sampler, dx9TextureInfo.TexCoordInput));
        }
    }
}
