// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9SepiaElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9SepiaElement
    {
        internal static void Generate(SepiaElement efoSepia, ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo1 = new VariableInfo()
            {
                ID = efoSepia.LightColorID,
                Type = Dx9VariableType.Vector4,
                IsDynamic = efoSepia.IsDynamicProperty("LightColor")
            };
            variableInfo1.Name = variableInfo1.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo1.Type, efoSepia.Name + "LightColor") : effectBuilder.GenerateGlobalConstant(variableInfo1.Type, efoSepia.Name + "LightColor");
            variableInfo1.DefaultValue = efoSepia.LightColor.ToVector4();
            effectBuilder.AddPropertyVariable(variableInfo1);
            VariableInfo variableInfo2 = new VariableInfo()
            {
                ID = efoSepia.DarkColorID,
                Type = Dx9VariableType.Vector4,
                IsDynamic = efoSepia.IsDynamicProperty("DarkColor")
            };
            variableInfo2.Name = variableInfo2.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo2.Type, efoSepia.Name + "DarkColor") : effectBuilder.GenerateGlobalConstant(variableInfo2.Type, efoSepia.Name + "DarkColor");
            variableInfo2.DefaultValue = efoSepia.DarkColor.ToVector4();
            effectBuilder.AddPropertyVariable(variableInfo2);
            VariableInfo variableInfo3 = new VariableInfo()
            {
                ID = efoSepia.DesaturateID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoSepia.IsDynamicProperty("Desaturate")
            };
            variableInfo3.Name = variableInfo3.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo3.Type, efoSepia.Name + "Desaturate") : effectBuilder.GenerateGlobalConstant(variableInfo3.Type, efoSepia.Name + "Desaturate");
            variableInfo3.DefaultValue = efoSepia.Desaturate;
            effectBuilder.AddPropertyVariable(variableInfo3);
            VariableInfo variableInfo4 = new VariableInfo()
            {
                ID = efoSepia.ToneID,
                Type = Dx9VariableType.Float,
                IsDynamic = efoSepia.IsDynamicProperty("Tone")
            };
            variableInfo4.Name = variableInfo4.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo4.Type, efoSepia.Name + "Tone") : effectBuilder.GenerateGlobalConstant(variableInfo4.Type, efoSepia.Name + "Tone");
            variableInfo4.DefaultValue = efoSepia.Tone;
            effectBuilder.AddPropertyVariable(variableInfo4);
            effectBuilder.EmitPixelFragment(InvariantString.Format("    {{\r\n        {0}.rgb *= {1}.rgb;\r\n        float3 grayer = float3(0.3f, 0.59f, 0.11f);\r\n        float  gray = dot({0}.rgb, grayer);\r\n        float3 muted = lerp({0}.rgb, gray.xxx, {3});\r\n        float3 sepia = lerp({2}.rgb, {1}.rgb, gray);\r\n        {0}.rgb = lerp(muted, sepia, {4});\r\n    }}\r\n", effectBuilder.PixelShaderOutput, variableInfo1.Name, variableInfo2.Name, variableInfo3.Name, variableInfo4.Name));
        }
    }
}
