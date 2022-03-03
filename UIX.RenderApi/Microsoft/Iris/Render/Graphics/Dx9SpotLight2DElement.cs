// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9SpotLight2DElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9SpotLight2DElement
    {
        internal static void Generate(
          SpotLight2DElement efiSpotlight2D,
          ref Dx9EffectBuilder effectBuilder)
        {
            VariableInfo variableInfo1 = new VariableInfo()
            {
                ID = efiSpotlight2D.PositionID,
                Type = Dx9VariableType.Vector3,
                IsDynamic = efiSpotlight2D.IsDynamicProperty("Position")
            };
            variableInfo1.Name = variableInfo1.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo1.Type, "Position") : effectBuilder.GenerateGlobalConstant(variableInfo1.Type, "Position");
            variableInfo1.DefaultValue = efiSpotlight2D.Position;
            effectBuilder.AddPropertyVariable(variableInfo1);
            VariableInfo variableInfo2 = new VariableInfo()
            {
                ID = efiSpotlight2D.DirectionAngleID,
                Type = Dx9VariableType.Float,
                IsDynamic = efiSpotlight2D.IsDynamicProperty("DirectionAngle")
            };
            variableInfo2.Name = variableInfo2.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo2.Type, "DirectionAngle") : effectBuilder.GenerateGlobalConstant(variableInfo2.Type, "DirectionAngle");
            variableInfo2.DefaultValue = efiSpotlight2D.DirectionAngle;
            effectBuilder.AddPropertyVariable(variableInfo2);
            VariableInfo variableInfo3 = new VariableInfo()
            {
                ID = efiSpotlight2D.LightColorID,
                Type = Dx9VariableType.Vector4,
                IsDynamic = efiSpotlight2D.IsDynamicProperty("LightColor")
            };
            variableInfo3.Name = variableInfo3.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo3.Type, "LightColor") : effectBuilder.GenerateGlobalConstant(variableInfo3.Type, "LightColor");
            variableInfo3.DefaultValue = efiSpotlight2D.LightColor.ToVector4();
            effectBuilder.AddPropertyVariable(variableInfo3);
            VariableInfo variableInfo4 = new VariableInfo()
            {
                ID = efiSpotlight2D.AmbientColorID,
                Type = Dx9VariableType.Vector4,
                IsDynamic = efiSpotlight2D.IsDynamicProperty("AmbientColor")
            };
            variableInfo4.Name = variableInfo4.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo4.Type, "AmbientColor") : effectBuilder.GenerateGlobalConstant(variableInfo4.Type, "AmbientColor");
            variableInfo4.DefaultValue = efiSpotlight2D.AmbientColor.ToVector4();
            effectBuilder.AddPropertyVariable(variableInfo4);
            VariableInfo variableInfo5 = new VariableInfo()
            {
                ID = efiSpotlight2D.InnerConeAngleID,
                Type = Dx9VariableType.Float,
                IsDynamic = efiSpotlight2D.IsDynamicProperty("InnerConeAngle")
            };
            variableInfo5.Name = variableInfo5.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo5.Type, "InnerConeAngle") : effectBuilder.GenerateGlobalConstant(variableInfo5.Type, "InnerConeAngle");
            variableInfo5.DefaultValue = efiSpotlight2D.InnerConeAngle;
            effectBuilder.AddPropertyVariable(variableInfo5);
            VariableInfo variableInfo6 = new VariableInfo()
            {
                ID = efiSpotlight2D.OuterConeAngleID,
                Type = Dx9VariableType.Float,
                IsDynamic = efiSpotlight2D.IsDynamicProperty("OuterConeAngle")
            };
            variableInfo6.Name = variableInfo6.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo6.Type, "OuterConeAngle") : effectBuilder.GenerateGlobalConstant(variableInfo6.Type, "OuterConeAngle");
            variableInfo6.DefaultValue = efiSpotlight2D.OuterConeAngle;
            effectBuilder.AddPropertyVariable(variableInfo6);
            VariableInfo variableInfo7 = new VariableInfo()
            {
                ID = efiSpotlight2D.IntensityID,
                Type = Dx9VariableType.Float,
                IsDynamic = efiSpotlight2D.IsDynamicProperty("Intensity")
            };
            variableInfo7.Name = variableInfo7.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo7.Type, "Intensity") : effectBuilder.GenerateGlobalConstant(variableInfo7.Type, "Intensity");
            variableInfo7.DefaultValue = efiSpotlight2D.Intensity;
            effectBuilder.AddPropertyVariable(variableInfo7);
            VariableInfo variableInfo8 = new VariableInfo()
            {
                ID = efiSpotlight2D.AttenuationID,
                Type = Dx9VariableType.Vector3,
                IsDynamic = efiSpotlight2D.IsDynamicProperty("Attenuation")
            };
            variableInfo8.Name = variableInfo8.IsDynamic ? effectBuilder.GenerateGlobalVariable(variableInfo8.Type, "Attenuation") : effectBuilder.GenerateGlobalConstant(variableInfo8.Type, "Attenuation");
            variableInfo8.DefaultValue = efiSpotlight2D.Attenuation;
            effectBuilder.AddPropertyVariable(variableInfo8);
            effectBuilder.AddRequirement(EffectRequirements.ViewPosition);
            effectBuilder.PixelShaderOutput = effectBuilder.GenerateLocalVariable(Dx9VariableType.Vector4, efiSpotlight2D.Name);
            string str = InvariantString.Format("    float4 {0} = {3};\r\n    {{\r\n       // Compute the direction of the spotlight and the distance from the current pixel\r\n       // 0.0001 prevents div by zero while using smoothstep interpolation\r\n       float  flCosOuterCone = cos(radians(({4}+{5}+0.0001)/2.0f));\r\n       float  flCosInnerCone = cos(radians({4}/2.0f));\r\n       float  flDirectionAngle = radians({2});\r\n       float2 vLightDir = {{cos(flDirectionAngle), -sin(flDirectionAngle)}};\r\n       float2 vLightPosition = {1}.xy - vLightDir;\r\n       float2 vPositionDelta = Input.ViewPos.xy - vLightPosition;\r\n\r\n       // Compute the cone region, attenuation and falloff\r\n       float flDist = length(vPositionDelta);\r\n       float flFalloff = {6} / ({7}.x + flDist*{7}.y + flDist*flDist*{7}.z);\r\n       float flCone = dot(normalize(vPositionDelta), vLightDir);\r\n\r\n       {0} *= smoothstep(flCosOuterCone, flCosInnerCone, flCone) * flFalloff;\r\n", effectBuilder.PixelShaderOutput, variableInfo1.Name, variableInfo2.Name, variableInfo3.Name, variableInfo5.Name, variableInfo6.Name, variableInfo7.Name, variableInfo8.Name);
            if (variableInfo4.IsDynamic || !efiSpotlight2D.AmbientColor.ToVector4().IsApproximate(Vector4.Zero))
                effectBuilder.EmitPixelFragment(InvariantString.Format("{0}       {1} += {2};\r\n    }}\r\n", str, effectBuilder.PixelShaderOutput, variableInfo4.Name));
            else
                effectBuilder.EmitPixelFragment(InvariantString.Format("{0}    }}\r\n", str));
        }
    }
}
