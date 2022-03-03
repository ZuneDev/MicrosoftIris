// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9EffectResource
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9EffectResource : SharedRenderObject, IRenderHandleOwner
    {
        private RenderSession m_session;
        private RemoteDx9EffectResource m_remoteEffect;
        private string m_stName;
        private Dx9EffectManager m_effectManager;
        private DataBufferTracker m_tracker;
        private Size m_sizeGutter;
        private static string k_stVertexShaderName = "ExecuteVertexShader";
        private static string k_stPixelShaderName = "ExecutePixelShader";

        internal Dx9EffectResource(
          RenderSession session,
          string stName,
          Dx9EffectManager effectManager)
        {
            this.m_session = session;
            this.m_stName = stName;
            this.m_effectManager = effectManager;
            this.m_tracker = MessagingSession.Current.CreateDataBufferTracker(this);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    if (this.m_tracker != null)
                        MessagingSession.Current.ReturnDataBufferTracker(this.m_tracker);
                    if (this.m_remoteEffect != null)
                        this.m_remoteEffect.Dispose();
                }
                this.m_tracker = null;
                this.m_remoteEffect = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        public RemoteDx9EffectResource RemoteStub => this.m_remoteEffect;

        private string PixelShaderVersion => this.m_effectManager.Device.PixelShaderProfile;

        private string VertexShaderVersion => this.m_effectManager.Device.VertexShaderProfile;

        internal Size GutterSize => this.m_sizeGutter;

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteEffect.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteEffect = null;

        internal bool Create(EffectInput element)
        {
            Dx9EffectBuilder effectBuilder = this.m_effectManager.EffectBuilder;
            effectBuilder.Clear();
            if (!this.ProcessEffectInput(element, ref effectBuilder))
                return false;
            this.GenerateEffect(effectBuilder);
            uint nBlobSize = 0;
            IntPtr pEffectBlob = IntPtr.Zero;
            IntPtr pBlobBuffer = IntPtr.Zero;
            if (!this.CompileEffect(effectBuilder, out pEffectBlob, out pBlobBuffer, out nBlobSize))
                return false;
            Vector<VariableInfo> propertyVariables = effectBuilder.PropertyVariables;
            if (propertyVariables != null)
            {
                int index = 0;
                while (index < propertyVariables.Count)
                {
                    if (!propertyVariables[index].IsDynamic)
                        propertyVariables.RemoveAt(index);
                    else
                        ++index;
                }
            }
            this.RemoteEffect(effectBuilder, pEffectBlob, pBlobBuffer, nBlobSize);
            return true;
        }

        private bool CompileEffect(
          Dx9EffectBuilder effectBuilder,
          out IntPtr pEffectBlob,
          out IntPtr pBlobBuffer,
          out uint nBlobSize)
        {
            bool flag = false;
            IntPtr pErrorString = IntPtr.Zero;
            IntPtr pErrorBuffer = IntPtr.Zero;
            pEffectBlob = IntPtr.Zero;
            pBlobBuffer = IntPtr.Zero;
            nBlobSize = 0U;
            try
            {
                EngineApi.IFC(EngineApi.SpDx9CompileEffect(effectBuilder.EffectDefinition, null, out pErrorString, out pErrorBuffer, out pEffectBlob, out nBlobSize, out pBlobBuffer));
                flag = true;
            }
            catch (Exception ex)
            {
                if (Marshal.PtrToStringAnsi(pErrorString) == null)
                    throw;
            }
            finally
            {
                if (pErrorBuffer != IntPtr.Zero)
                    Marshal.Release(pErrorBuffer);
            }
            return flag;
        }

        private void RemoteEffect(
          Dx9EffectBuilder effectBuilder,
          IntPtr pEffectBlob,
          IntPtr pBlobBuffer,
          uint nBlobSize)
        {
            DataBuffer dataBuffer = this.m_session.BuildDataBuffer(pEffectBlob, nBlobSize);
            this.m_tracker.Track(dataBuffer, new DataBufferTracker.CleanupEventHandler(OnReleaseLocalData), pBlobBuffer);
            dataBuffer.Commit();
            this.m_remoteEffect = this.m_session.BuildRemoteDx9EffectResource(this, this.m_effectManager.Device, dataBuffer);
            Vector<VariableInfo> propertyVariables = effectBuilder.PropertyVariables;
            if (propertyVariables != null)
            {
                foreach (VariableInfo variableInfo in propertyVariables)
                {
                    switch (variableInfo.Type)
                    {
                        case Dx9VariableType.Integer:
                            this.m_remoteEffect.SendAddProperty(variableInfo.Name, (int)variableInfo.DefaultValue);
                            continue;
                        case Dx9VariableType.Float:
                            this.m_remoteEffect.SendAddProperty(variableInfo.Name, (float)variableInfo.DefaultValue);
                            continue;
                        case Dx9VariableType.Vector2:
                            this.m_remoteEffect.SendAddProperty(variableInfo.Name, (Vector2)variableInfo.DefaultValue);
                            continue;
                        case Dx9VariableType.Vector3:
                            this.m_remoteEffect.SendAddProperty(variableInfo.Name, (Vector3)variableInfo.DefaultValue);
                            continue;
                        case Dx9VariableType.Vector4:
                            this.m_remoteEffect.SendAddProperty(variableInfo.Name, (Vector4)variableInfo.DefaultValue);
                            continue;
                        case Dx9VariableType.Texture:
                            TextureVariableInfo textureVariableInfo = (TextureVariableInfo)variableInfo;
                            this.m_remoteEffect.SendAddProperty(variableInfo.Name, textureVariableInfo.CoordinateMapID, textureVariableInfo.ImageIndexID);
                            continue;
                        default:
                            continue;
                    }
                }
            }
            this.m_remoteEffect.SendPostPropertiesAdded();
            this.m_remoteEffect.SendSetGutterSize(this.m_sizeGutter);
            if (effectBuilder.TextureInfoList == null)
                return;
            foreach (Dx9TextureInfo textureInfo in effectBuilder.TextureInfoList)
            {
                if (textureInfo.Requirements != 0)
                    this.m_remoteEffect.SendAddTextureRequirements(textureInfo.ImageIndex, textureInfo.Requirements, textureInfo.TexelSize, textureInfo.TexUVSize, textureInfo.TexUVRefPoint, textureInfo.DownsamplePropertyID);
            }
        }

        private static void OnReleaseLocalData(object sender, DataBufferTracker.CleanupEventArgs args)
        {
            IntPtr contextData = (IntPtr)args.ContextData;
            if (!(contextData != IntPtr.Zero))
                return;
            Marshal.Release(contextData);
            IntPtr zero = IntPtr.Zero;
        }

        private string GenerateFileName() => this.m_stName + ".fx";

        protected void EmitHeader(Dx9EffectBuilder effectBuilder)
        {
            effectBuilder.EmitEffectFragment("#include \"Common.fx\"\r\n");
            effectBuilder.EmitEffectFragment(effectBuilder.IncludesFragment + "\r\n\r\n");
            Vector<VariableInfo>[] vectorArray = new Vector<VariableInfo>[2]
            {
        effectBuilder.PropertyVariables,
        effectBuilder.GlobalVariables
            };
            for (int index = 0; index < vectorArray.Length; ++index)
            {
                if (vectorArray[index] != null)
                {
                    foreach (VariableInfo variableInfo in vectorArray[index])
                    {
                        if (variableInfo.Name != null)
                        {
                            switch (variableInfo.Type)
                            {
                                case Dx9VariableType.Integer:
                                    if (variableInfo.IsDynamic)
                                    {
                                        effectBuilder.EmitEffectFragment(InvariantString.Format("{0} {1};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name));
                                        continue;
                                    }
                                    effectBuilder.EmitEffectFragment(InvariantString.Format("static const {0} {1} = {2};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name, (int)variableInfo.DefaultValue));
                                    continue;
                                case Dx9VariableType.Float:
                                    if (variableInfo.IsDynamic)
                                    {
                                        effectBuilder.EmitEffectFragment(InvariantString.Format("{0} {1};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name));
                                        continue;
                                    }
                                    effectBuilder.EmitEffectFragment(InvariantString.Format("static const {0} {1} = {2};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name, (float)variableInfo.DefaultValue));
                                    continue;
                                case Dx9VariableType.Vector2:
                                    if (variableInfo.IsDynamic)
                                    {
                                        effectBuilder.EmitEffectFragment(InvariantString.Format("{0} {1};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name));
                                        continue;
                                    }
                                    effectBuilder.EmitEffectFragment(InvariantString.Format("static const {0} {1} = {2};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name, ((Vector2)variableInfo.DefaultValue).ToDxShaderString()));
                                    continue;
                                case Dx9VariableType.Vector3:
                                    if (variableInfo.IsDynamic)
                                    {
                                        effectBuilder.EmitEffectFragment(InvariantString.Format("{0} {1};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name));
                                        continue;
                                    }
                                    effectBuilder.EmitEffectFragment(InvariantString.Format("static const {0} {1} = {2};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name, ((Vector3)variableInfo.DefaultValue).ToDxShaderString()));
                                    continue;
                                case Dx9VariableType.Vector4:
                                    if (variableInfo.IsDynamic)
                                    {
                                        effectBuilder.EmitEffectFragment(InvariantString.Format("{0} {1};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name));
                                        continue;
                                    }
                                    effectBuilder.EmitEffectFragment(InvariantString.Format("static const {0} {1} = {2};\r\n", effectBuilder.ConvertToString(variableInfo.Type), variableInfo.Name, ((Vector4)variableInfo.DefaultValue).ToDxShaderString()));
                                    continue;
                                case Dx9VariableType.Texture:
                                    TextureVariableInfo textureVariableInfo = (TextureVariableInfo)variableInfo;
                                    effectBuilder.EmitEffectFragment(InvariantString.Format("\r\n{0} {1};\r\n", effectBuilder.ConvertToString(textureVariableInfo.Type), textureVariableInfo.Name));
                                    effectBuilder.EmitEffectFragment(InvariantString.Format("sampler {0} = \r\n    sampler_state\r\n    {{\r\n        Texture   = <{1}>;\r\n        MinFilter = {2};\r\n        MagFilter = {3};\r\n        AddressU  = Clamp;\r\n        AddressV  = Clamp;\r\n    }};\r\n\r\n", textureVariableInfo.SamplerName, textureVariableInfo.Name, textureVariableInfo.MinFilter, textureVariableInfo.MagFilter));
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
            effectBuilder.EmitEffectFragment("\r\n");
        }

        protected void EmitShaders(Dx9EffectBuilder effectBuilder)
        {
            effectBuilder.EmitEffectFragment(effectBuilder.ShaderDeclarations);
            effectBuilder.EmitEffectFragment(InvariantString.Format("VS_OUTPUT {0}(VS_INPUT Input)\r\n{{\r\n    VS_OUTPUT Output;\r\n\r\n{1}\r\n    return Output;\r\n}}\r\n\r\n", k_stVertexShaderName, effectBuilder.VertexShader));
            effectBuilder.EmitEffectFragment(InvariantString.Format("PS_OUTPUT {0}(VS_OUTPUT Input)\r\n{{\r\n    PS_OUTPUT Output;\r\n\r\n{1}    // Include Vertex color contribution\r\n    Output.Color = {2} * Input.Color;\r\n\r\n    // Return the final result\r\n    return Output;\r\n}}\r\n\r\n", k_stPixelShaderName, effectBuilder.PixelShader, effectBuilder.PixelShaderOutput));
        }

        protected void EmitTechnique(Dx9EffectBuilder effectBuilder) => effectBuilder.EmitEffectFragment(InvariantString.Format("technique GeneratedTechnique\r\n{{\r\n    pass P0\r\n    {{\r\n        // Shaders\r\n        PixelShader   = compile {0} {1}();\r\n        VertexShader  = compile {2} {3}();\r\n    }}\r\n}}\r\n", PixelShaderVersion, k_stPixelShaderName, VertexShaderVersion, k_stVertexShaderName));

        private void GenerateEffect(Dx9EffectBuilder effectBuilder)
        {
            this.EmitHeader(effectBuilder);
            this.EmitShaders(effectBuilder);
            this.EmitTechnique(effectBuilder);
        }

        private bool ProcessEffectInput(EffectInput efiCurrent, ref Dx9EffectBuilder effectBuilder)
        {
            bool flag = true;
            switch (efiCurrent.Type)
            {
                case EffectInputType.Color:
                    Dx9ColorElement.Generate((ColorElement)efiCurrent, ref effectBuilder);
                    break;
                case EffectInputType.Image:
                    Dx9ImageElement.Generate((ImageElement)efiCurrent, ref effectBuilder);
                    break;
                case EffectInputType.Blend:
                    BlendElement efiBlend = (BlendElement)efiCurrent;
                    this.ProcessEffectInput(efiBlend.Input1, ref effectBuilder);
                    string pixelShaderOutput1 = effectBuilder.PixelShaderOutput;
                    this.ProcessEffectInput(efiBlend.Input2, ref effectBuilder);
                    string pixelShaderOutput2 = effectBuilder.PixelShaderOutput;
                    Dx9BlendElement.Generate(efiBlend, pixelShaderOutput1, pixelShaderOutput2, ref effectBuilder);
                    break;
                case EffectInputType.Interpolate:
                    InterpolateElement efiInterpolate = (InterpolateElement)efiCurrent;
                    this.ProcessEffectInput(efiInterpolate.Input1, ref effectBuilder);
                    string pixelShaderOutput3 = effectBuilder.PixelShaderOutput;
                    this.ProcessEffectInput(efiInterpolate.Input2, ref effectBuilder);
                    string pixelShaderOutput4 = effectBuilder.PixelShaderOutput;
                    Dx9InterpolateElement.Generate(efiInterpolate, pixelShaderOutput3, pixelShaderOutput4, ref effectBuilder);
                    break;
                case EffectInputType.Layer:
                    flag = this.ProcessEffectLayer((EffectLayer)efiCurrent, ref effectBuilder);
                    break;
                case EffectInputType.ComplexImage:
                    Dx9ComplexImageElement.Generate((ComplexImageElement)efiCurrent, ref effectBuilder);
                    break;
                case EffectInputType.Video:
                    Dx9VideoElement.Generate((VideoElement)efiCurrent, ref effectBuilder);
                    break;
                case EffectInputType.Spotlight2D:
                    Dx9SpotLight2DElement.Generate((SpotLight2DElement)efiCurrent, ref effectBuilder);
                    break;
                case EffectInputType.PointLight2D:
                    Dx9PointLight2DElement.Generate((PointLight2DElement)efiCurrent, ref effectBuilder);
                    break;
                case EffectInputType.Destination:
                    Dx9DestinationElement.Generate((DestinationElement)efiCurrent, ref effectBuilder);
                    this.m_sizeGutter.Width = Math.Max(this.m_sizeGutter.Width, 1);
                    this.m_sizeGutter.Height = Math.Max(this.m_sizeGutter.Height, 1);
                    break;
            }
            return flag && effectBuilder.Validate();
        }

        private bool ProcessEffectLayer(EffectLayer eflCurrent, ref Dx9EffectBuilder effectBuilder)
        {
            if (!this.ProcessEffectInput(eflCurrent.Input, ref effectBuilder))
                return false;
            if (eflCurrent.Operations != null)
            {
                foreach (EffectOperation operation in eflCurrent.Operations)
                {
                    switch (operation.Type)
                    {
                        case EffectOperationType.Brightness:
                            Dx9BrightnessElement.Generate((BrightnessElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.Contrast:
                            Dx9ContrastElement.Generate((ContrastElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.Desaturate:
                            Dx9DesaturateElement.Generate((DesaturateElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.EdgeDetection:
                            Dx9EdgeDetectionElement.Generate((EdgeDetectionElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.Emboss:
                            Dx9EmbossElement.Generate((EmbossElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.GaussianBlur:
                            Debug2.Validate(eflCurrent.Input.Type == EffectInputType.Image || eflCurrent.Input.Type == EffectInputType.Video || eflCurrent.Input.Type == EffectInputType.Destination, typeof(InvalidOperationException), "GaussianBlurElement only valid for inputs with ImageElement inputs");
                            GaussianBlurElement efoBlur = (GaussianBlurElement)operation;
                            Dx9GaussianBlurElement.Generate(efoBlur, ref effectBuilder);
                            this.m_sizeGutter.Width = Math.Max(this.m_sizeGutter.Width, efoBlur.KernelRadius + 1);
                            this.m_sizeGutter.Height = Math.Max(this.m_sizeGutter.Height, efoBlur.KernelRadius + 1);
                            continue;
                        case EffectOperationType.HSL:
                            Dx9HSLElement.Generate((HSLElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.HSV:
                            Dx9HSVElement.Generate((HSVElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.InvAlpha:
                            Dx9InvAlphaElement.Generate(operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.InvColor:
                            Dx9InvColorElement.Generate(operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.Invert:
                            Dx9InvertElement.Generate((InvertElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.LightShaft:
                            Debug2.Validate(eflCurrent.Input.Type == EffectInputType.Image || eflCurrent.Input.Type == EffectInputType.Video || eflCurrent.Input.Type == EffectInputType.Destination, typeof(InvalidOperationException), "LightShaftElement only valid for inputs with ImageElement inputs");
                            Dx9LightShaftElement.Generate((LightShaftElement)operation, ref effectBuilder);
                            continue;
                        case EffectOperationType.Sepia:
                            Dx9SepiaElement.Generate((SepiaElement)operation, ref effectBuilder);
                            continue;
                        default:
                            continue;
                    }
                }
            }
            return true;
        }
    }
}
