// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9EffectBuilder
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render.Internal;
using System.IO;
using System.Text;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9EffectBuilder
    {
        private string m_stPixelOutput;
        private StringBuilder m_sbPixelShader;
        private StringBuilder m_sbVertexShader;
        private StringBuilder m_sbDeclarations;
        private StringBuilder m_sbEffect;
        private Vector<VariableInfo> m_alPropertyVariables;
        private int m_nUniqueVariableID;
        private Vector<Dx9TextureInfo> m_alTextureInfos;
        private Vector<VariableInfo> m_alGlobalVariables;
        private Vector<string> m_alIncludes;
        private int m_cMaxSimultaneousTextures;
        private int m_cMaxRenderTargets;
        private int m_nRequirements;

        internal Dx9EffectBuilder(GraphicsCaps caps)
        {
            this.m_cMaxSimultaneousTextures = caps.MaxSimultaneousTextures;
            this.m_cMaxRenderTargets = caps.AvailableRenderTargets;
        }

        internal void EmitPixelFragment(string stData)
        {
            if (this.m_sbPixelShader == null)
                this.m_sbPixelShader = new StringBuilder(128);
            this.m_sbPixelShader.Append(stData);
        }

        internal void EmitEffectFragment(string stData)
        {
            if (this.m_sbEffect == null)
                this.m_sbEffect = new StringBuilder(512);
            this.m_sbEffect.Append(stData);
        }

        internal void EmitIncludesFragment(string stData)
        {
            if (this.m_alIncludes == null)
                this.m_alIncludes = new Vector<string>();
            if (this.m_alIncludes.Contains(stData))
                return;
            this.m_alIncludes.Add(stData);
        }

        internal string GenerateGlobalConstant(Dx9VariableType typeProperty, string stName) => this.GenerateVariableName(VariableScope.Const, typeProperty, stName);

        internal string GenerateLocalVariable(Dx9VariableType typeProperty, string stName) => this.GenerateVariableName(VariableScope.Local, typeProperty, stName);

        internal string GenerateGlobalVariable(Dx9VariableType typeProperty, string stName) => this.GenerateVariableName(VariableScope.Global, typeProperty, stName);

        internal void AddPropertyVariable(VariableInfo variableInfo)
        {
            if (this.m_alPropertyVariables == null)
                this.m_alPropertyVariables = new Vector<VariableInfo>();
            this.m_alPropertyVariables.Add(variableInfo);
        }

        internal void AddGlobalVariable(VariableInfo variableInfo)
        {
            if (this.m_alGlobalVariables == null)
                this.m_alGlobalVariables = new Vector<VariableInfo>();
            this.m_alGlobalVariables.Add(variableInfo);
        }

        private string GenerateVariableName(
          VariableScope scope,
          Dx9VariableType typeProperty,
          string stName)
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            switch (scope)
            {
                case VariableScope.Global:
                    stringBuilder.Append("g_");
                    break;
                case VariableScope.Const:
                    stringBuilder.Append("k_");
                    break;
            }
            switch (typeProperty)
            {
                case Dx9VariableType.Integer:
                    stringBuilder.Append("n");
                    break;
                case Dx9VariableType.Float:
                    stringBuilder.Append("fl");
                    break;
                case Dx9VariableType.Vector2:
                case Dx9VariableType.Vector3:
                case Dx9VariableType.Vector4:
                    stringBuilder.Append("v");
                    break;
                case Dx9VariableType.Texture:
                    stringBuilder.Append("tex");
                    break;
            }
            if (stName != null)
            {
                if (stName.Length > 15)
                    stName = stName.Substring(0, 15);
                stringBuilder.Append(stName);
            }
            else
                stringBuilder.Append("Temp");
            return stringBuilder.ToString() + this.m_nUniqueVariableID++;
        }

        internal void Clear()
        {
            this.m_stPixelOutput = null;
            this.m_nUniqueVariableID = 0;
            this.m_nRequirements = 0;
            this.m_sbPixelShader = null;
            this.m_sbVertexShader = null;
            this.m_sbDeclarations = null;
            this.m_sbEffect = null;
            if (this.m_alPropertyVariables != null)
                this.m_alPropertyVariables.Clear();
            if (this.m_alGlobalVariables != null)
                this.m_alGlobalVariables.Clear();
            if (this.m_alTextureInfos != null)
                this.m_alTextureInfos.Clear();
            if (this.m_alIncludes == null)
                return;
            this.m_alIncludes.Clear();
        }

        internal string ConvertToString(Dx9VariableType type)
        {
            switch (type)
            {
                case Dx9VariableType.Integer:
                    return "int";
                case Dx9VariableType.Float:
                    return "float";
                case Dx9VariableType.Vector2:
                    return "float2";
                case Dx9VariableType.Vector3:
                    return "float3";
                case Dx9VariableType.Vector4:
                    return "float4";
                case Dx9VariableType.Texture:
                    return "texture";
                default:
                    return "";
            }
        }

        internal string ConvertToString(FilterMode mode)
        {
            switch (mode)
            {
                case FilterMode.Point:
                    return "Point";
                case FilterMode.Linear:
                    return "Linear";
                default:
                    return "";
            }
        }

        internal bool Validate() => this.ImageCount <= this.m_cMaxSimultaneousTextures && this.GetRenderTargetCount() <= this.m_cMaxRenderTargets;

        internal Dx9TextureInfo AllocateTexture()
        {
            Dx9TextureInfo dx9TextureInfo = new Dx9TextureInfo()
            {
                ImageIndex = this.ImageCount
            };
            dx9TextureInfo.TexCoordInput = InvariantString.Format("{0}{1}", "vTexCoord", dx9TextureInfo.ImageIndex);
            dx9TextureInfo.Sampler = InvariantString.Format("{0}{1}", "Sampler", dx9TextureInfo.ImageIndex);
            dx9TextureInfo.DownsamplePropertyID = -1;
            if (this.m_alTextureInfos == null)
                this.m_alTextureInfos = new Vector<Dx9TextureInfo>();
            this.m_alTextureInfos.Add(dx9TextureInfo);
            return dx9TextureInfo;
        }

        internal void AddRequirement(
          Dx9TextureInfo textureInfo,
          Dx9TextureRequirements type,
          object oValue)
        {
            textureInfo.Requirements |= (int)type;
            switch (type)
            {
                case Dx9TextureRequirements.TexelSize:
                    if (textureInfo.TexelSize != null)
                        break;
                    string stName1 = InvariantString.Format("{0}{1}", "TexelSize", textureInfo.ImageIndex);
                    VariableInfo variableInfo1 = new VariableInfo()
                    {
                        IsDynamic = true,
                        ID = -1,
                        Type = Dx9VariableType.Vector2
                    };
                    variableInfo1.Name = this.GenerateGlobalVariable(variableInfo1.Type, stName1);
                    variableInfo1.DefaultValue = new Vector2(0.0f, 0.0f);
                    this.AddGlobalVariable(variableInfo1);
                    textureInfo.TexelSize = variableInfo1.Name;
                    break;
                case Dx9TextureRequirements.SampleBackbuffer:
                    textureInfo.DownsamplePropertyID = (int)oValue;
                    break;
                case Dx9TextureRequirements.TexUVSize:
                    if (textureInfo.TexUVSize != null)
                        break;
                    string stName2 = InvariantString.Format("{0}{1}", "TexUVSize", textureInfo.ImageIndex);
                    VariableInfo variableInfo2 = new VariableInfo()
                    {
                        IsDynamic = true,
                        ID = -1,
                        Type = Dx9VariableType.Vector2
                    };
                    variableInfo2.Name = this.GenerateGlobalVariable(variableInfo2.Type, stName2);
                    variableInfo2.DefaultValue = new Vector2(0.0f, 0.0f);
                    this.AddGlobalVariable(variableInfo2);
                    textureInfo.TexUVSize = variableInfo2.Name;
                    break;
                case Dx9TextureRequirements.TexUVRefPoint:
                    if (textureInfo.TexUVRefPoint != null)
                        break;
                    string stName3 = InvariantString.Format("{0}{1}", "TexUVRefPoint", textureInfo.ImageIndex);
                    VariableInfo variableInfo3 = new VariableInfo()
                    {
                        IsDynamic = true,
                        ID = -1,
                        Type = Dx9VariableType.Vector2
                    };
                    variableInfo3.Name = this.GenerateGlobalVariable(variableInfo3.Type, stName3);
                    variableInfo3.DefaultValue = new Vector2(0.0f, 0.0f);
                    this.AddGlobalVariable(variableInfo3);
                    textureInfo.TexUVRefPoint = variableInfo3.Name;
                    break;
            }
        }

        internal void AddRequirement(EffectRequirements requirement) => Bits.SetFlag(ref this.m_nRequirements, (int)requirement);

        internal Dx9TextureInfo GetTextureInfo(int idxTexture) => this.m_alTextureInfos[idxTexture];

        internal string PixelShaderOutput
        {
            get => this.m_stPixelOutput;
            set => this.m_stPixelOutput = value;
        }

        internal string ShaderDeclarations => this.GenerateDeclarations();

        internal string IncludesFragment
        {
            get
            {
                if (this.m_alIncludes == null)
                    return "";
                StringBuilder stringBuilder = new StringBuilder();
                for (int index = 0; index < this.m_alIncludes.Count; ++index)
                    stringBuilder.AppendFormat("#include {0}\r\n", this.m_alIncludes[index]);
                return stringBuilder.ToString();
            }
        }

        internal string PixelShader => this.GeneratePixelShader();

        internal string VertexShader => this.GenerateVertexShader();

        internal Vector<VariableInfo> PropertyVariables => this.m_alPropertyVariables;

        internal Vector<VariableInfo> GlobalVariables => this.m_alGlobalVariables;

        internal string EffectDefinition => this.m_sbEffect.ToString();

        internal int ImageCount => this.m_alTextureInfos == null ? 0 : this.m_alTextureInfos.Count;

        internal Vector<Dx9TextureInfo> TextureInfoList => this.m_alTextureInfos;

        internal void SaveEffect(string stFilename)
        {
            StreamWriter streamWriter = new StreamWriter(stFilename);
            streamWriter.WriteLine(this.m_sbEffect.ToString());
            streamWriter.Close();
        }

        private string GenerateDeclarations()
        {
            if (this.m_sbDeclarations == null)
                this.m_sbDeclarations = new StringBuilder(128);
            if (this.m_sbDeclarations.Length == 0)
            {
                this.m_sbDeclarations.Append("struct PS_OUTPUT\r\n{\r\n    float4 Color       : COLOR;             // Object space position\r\n};\r\n\r\n");
                this.m_sbDeclarations.Append("struct VS_INPUT\r\n{\r\n    float4 ObjPos       : POSITION;         // Object space position\r\n    float4 Color        : COLOR;            // Vertex color\r\n");
                for (int index = 0; index < this.ImageCount; ++index)
                    this.m_sbDeclarations.AppendFormat("    float2 TexCoord{0}    : TEXCOORD{0};        // Texture {0} UV coordinates\r\n", index);
                this.m_sbDeclarations.Append("};\r\n\r\n");
                this.m_sbDeclarations.Append("struct VS_OUTPUT\r\n{\r\n    float4 ProjPos      : POSITION;         // Projected space position\r\n    float4 Color        : COLOR;            // Vertex color\r\n");
                if (Bits.TestFlag(this.m_nRequirements, 1))
                    this.m_sbDeclarations.Append("    float3 ViewPos      : TEXCOORD5;        // View space position\r\n");
                for (int index = 0; index < this.ImageCount; ++index)
                {
                    if (Bits.TestFlag(this.m_alTextureInfos[index].Requirements, 16))
                        this.m_sbDeclarations.AppendFormat("    float3 TexCoord{0}    : TEXCOORD{0};        // Texture {0} UVW coordinates\r\n", index);
                    else
                        this.m_sbDeclarations.AppendFormat("    float2 TexCoord{0}    : TEXCOORD{0};        // Texture {0} UV coordinates\r\n", index);
                }
                this.m_sbDeclarations.Append("};\r\n\r\n");
            }
            return this.m_sbDeclarations.ToString();
        }

        private string GenerateVertexShader()
        {
            if (this.m_sbVertexShader == null)
                this.m_sbVertexShader = new StringBuilder(128);
            if (this.m_sbVertexShader.Length == 0)
            {
                this.m_sbVertexShader.Append("    //\r\n    // Compute the accumulated WVP matrix.  If the separate matrices are not referenced\r\n    // by effect elements, the D3DX preshader will combine these on the CPU and set the\r\n    // result via VS constants.\r\n    //\r\n\r\n    float4x4 matAccumulated = mul(g_matWorldView, g_matProjection);\r\n\r\n\r\n    //\r\n    // Setup the resulting device vertex.\r\n    //\r\n\r\n    Output.ProjPos     = mul(Input.ObjPos, matAccumulated);\r\n    Output.Color       = Input.Color;\r\n");
                if (Bits.TestFlag(this.m_nRequirements, 1))
                    this.m_sbVertexShader.Append("    Output.ViewPos    = mul(Input.ObjPos, g_matWorldView);\r\n");
                for (int index = 0; index < this.ImageCount; ++index)
                {
                    if (Bits.TestFlag(this.m_alTextureInfos[index].Requirements, 16))
                        this.m_sbVertexShader.AppendFormat("    Output.TexCoord{0}   = float3(Input.TexCoord{0}.x * Output.ProjPos.w, Input.TexCoord{0}.y * Output.ProjPos.w, Output.ProjPos.w);\r\n", index);
                    else
                        this.m_sbVertexShader.AppendFormat("    Output.TexCoord{0}   = Input.TexCoord{0};\r\n", index);
                }
            }
            return this.m_sbVertexShader.ToString();
        }

        private string GeneratePixelShader()
        {
            StringBuilder stringBuilder = new StringBuilder(128);
            for (int index = 0; index < this.ImageCount; ++index)
            {
                Dx9TextureInfo alTextureInfo = this.m_alTextureInfos[index];
                if (Bits.TestFlag(alTextureInfo.Requirements, 16))
                    stringBuilder.AppendFormat("    float2 {0}   = Input.TexCoord{1}.xy / Input.TexCoord{1}.z;\r\n", alTextureInfo.TexCoordInput, index);
                else
                    stringBuilder.AppendFormat("    float2 {0}   = Input.TexCoord{1};\r\n", alTextureInfo.TexCoordInput, index);
            }
            stringBuilder.Append("\r\n");
            stringBuilder.Append(this.m_sbPixelShader.ToString());
            return stringBuilder.ToString();
        }

        private int GetRenderTargetCount()
        {
            if (this.m_alTextureInfos == null)
                return 0;
            int num = 0;
            foreach (Dx9TextureInfo alTextureInfo in this.m_alTextureInfos)
            {
                if ((alTextureInfo.Requirements & 2) > 0)
                {
                    ++num;
                    break;
                }
            }
            return num;
        }
    }
}
