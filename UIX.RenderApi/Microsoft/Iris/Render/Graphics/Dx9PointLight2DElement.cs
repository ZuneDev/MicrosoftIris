// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9PointLight2DElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9PointLight2DElement
    {
        internal static void Generate(
          PointLight2DElement efiPointLight2D,
          ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo1 = new VariableInfo()
            {
                ID = efiPointLight2D.PositionID,
                Type = Dx9VariableType.Vector3,
                IsDynamic = efiPointLight2D.IsDynamicProperty("Position")
            };
            variableInfo1.Name = variableInfo1.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo1.Type, "Position") : effectBuilder.GenerateGlobalConstant(variableInfo1.Type, "Position");
            variableInfo1.DefaultValue = efiPointLight2D.Position;
            effectBuilder.AddPropertyVariable(variableInfo1);
            VariableInfo variableInfo2 = new VariableInfo()
            {
                ID = efiPointLight2D.RadiusID,
                Type = Dx9VariableType.Float,
                IsDynamic = efiPointLight2D.IsDynamicProperty("Radius")
            };
            variableInfo2.Name = variableInfo2.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo2.Type, "Radius") : effectBuilder.GenerateGlobalConstant(variableInfo2.Type, "Radius");
            variableInfo2.DefaultValue = efiPointLight2D.Radius;
            effectBuilder.AddPropertyVariable(variableInfo2);
            VariableInfo variableInfo3 = new VariableInfo()
            {
                ID = efiPointLight2D.LightColorID,
                Type = Dx9VariableType.Vector4,
                IsDynamic = efiPointLight2D.IsDynamicProperty("LightColor")
            };
            variableInfo3.Name = variableInfo3.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo3.Type, "LightColor") : effectBuilder.GenerateGlobalConstant(variableInfo3.Type, "LightColor");
            variableInfo3.DefaultValue = efiPointLight2D.LightColor.ToVector4();
            effectBuilder.AddPropertyVariable(variableInfo3);
            VariableInfo variableInfo4 = new VariableInfo()
            {
                ID = efiPointLight2D.AmbientColorID,
                Type = Dx9VariableType.Vector4,
                IsDynamic = efiPointLight2D.IsDynamicProperty("AmbientColor")
            };
            variableInfo4.Name = variableInfo4.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo4.Type, "AmbientColor") : effectBuilder.GenerateGlobalConstant(variableInfo4.Type, "AmbientColor");
            variableInfo4.DefaultValue = efiPointLight2D.AmbientColor.ToVector4();
            effectBuilder.AddPropertyVariable(variableInfo4);
            VariableInfo variableInfo5 = new VariableInfo()
            {
                ID = efiPointLight2D.AttenuationID,
                Type = Dx9VariableType.Vector3,
                IsDynamic = efiPointLight2D.IsDynamicProperty("Attenuation")
            };
            variableInfo5.Name = variableInfo5.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo5.Type, "Attenuation") : effectBuilder.GenerateGlobalConstant(variableInfo5.Type, "Attenuation");
            variableInfo5.DefaultValue = efiPointLight2D.Attenuation;
            effectBuilder.AddPropertyVariable(variableInfo5);
            effectBuilder.AddRequirement(EffectRequirements.ViewPosition);
            effectBuilder.PixelShaderOutput = effectBuilder.GenerateLocalVariable(Dx9VariableType.Vector4, efiPointLight2D.Name);
            string str = InvariantString.Format("    float4 {0} = {3};\r\n    {{\r\n         float2 vPositionDelta = {1}.xy - Input.ViewPos.xy;\r\n         float flDist = length(vPositionDelta);\r\n         float flFalloff = saturate({2} / ({4}.x + flDist*{4}.y + flDist*flDist*{4}.z));\r\n         {0} *= flFalloff;\r\n", effectBuilder.PixelShaderOutput, variableInfo1.Name, variableInfo2.Name, variableInfo3.Name, variableInfo5.Name);
            if (variableInfo4.IsDynamic || !efiPointLight2D.AmbientColor.ToVector4().IsApproximate(Vector4.Zero))
                effectBuilder.EmitPixelFragment(InvariantString.Format("{0}         {1} += {2};\r\n    }}\r\n", str, effectBuilder.PixelShaderOutput, variableInfo4.Name));
            else
                effectBuilder.EmitPixelFragment(InvariantString.Format("{0}    }}\r\n", str));
        }
    }
}
