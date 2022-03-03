// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9ComplexImageElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9ComplexImageElement
    {
        internal static void Generate(ComplexImageElement efiImage, ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo1 = new VariableInfo()
            {
                ID = efiImage.MinFilterID,
                Type = Dx9VariableType.Integer,
                IsDynamic = efiImage.IsDynamicProperty("MinFilter")
            };
            variableInfo1.Name = variableInfo1.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo1.Type, efiImage.Name + "MinFilter") : effectBuilder.GenerateGlobalConstant(variableInfo1.Type, efiImage.Name + "MinFilter");
            variableInfo1.DefaultValue = (int)efiImage.MinFilter;
            effectBuilder.AddPropertyVariable(variableInfo1);
            string name1 = variableInfo1.Name;
            VariableInfo variableInfo2 = new VariableInfo()
            {
                ID = efiImage.MagFilterID,
                Type = Dx9VariableType.Integer,
                IsDynamic = efiImage.IsDynamicProperty("MagFilter")
            };
            variableInfo2.Name = variableInfo2.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo2.Type, efiImage.Name + "MagFilter") : effectBuilder.GenerateGlobalConstant(variableInfo2.Type, efiImage.Name + "MagFilter");
            variableInfo2.DefaultValue = (int)efiImage.MagFilter;
            effectBuilder.AddPropertyVariable(variableInfo2);
            string name2 = variableInfo2.Name;
            effectBuilder.AddPropertyVariable(new VariableInfo()
            {
                ID = efiImage.CoordinateMapIndexID,
                Type = Dx9VariableType.Integer,
                IsDynamic = efiImage.IsDynamicProperty("CoordinateMapIndex"),
                Name = null,
                DefaultValue = (int)efiImage.CoordinateMapIndex
            });
            Dx9TextureInfo dx9TextureInfo = effectBuilder.AllocateTexture();
            TextureVariableInfo textureVariableInfo = new TextureVariableInfo();
            textureVariableInfo.ID = efiImage.ImageID;
            textureVariableInfo.Type = Dx9VariableType.Texture;
            textureVariableInfo.Name = effectBuilder.GenerateGlobalVariable(textureVariableInfo.Type, efiImage.Name);
            textureVariableInfo.DefaultValue = efiImage.Image;
            textureVariableInfo.SamplerName = dx9TextureInfo.Sampler;
            textureVariableInfo.MinFilter = InvariantString.Format("<{0}>", name1);
            textureVariableInfo.MagFilter = InvariantString.Format("<{0}>", name2);
            textureVariableInfo.CoordinateMapID = efiImage.CoordinateMapIndexID;
            textureVariableInfo.ImageIndexID = efiImage.ImageIndexID;
            effectBuilder.AddPropertyVariable(textureVariableInfo);
            effectBuilder.PixelShaderOutput = effectBuilder.GenerateLocalVariable(Dx9VariableType.Vector4, efiImage.Name);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    // Load the image\r\n    float4 {0} = tex2D({1}, {2});\r\n\r\n", effectBuilder.PixelShaderOutput, dx9TextureInfo.Sampler, dx9TextureInfo.TexCoordInput));
            effectBuilder.AddPropertyVariable(new VariableInfo()
            {
                ID = efiImage.ImageIndexID,
                Type = Dx9VariableType.Integer,
                IsDynamic = efiImage.IsDynamicProperty("ImageIndex"),
                Name = null,
                DefaultValue = (int)efiImage.ImageIndex
            });
        }
    }
}
