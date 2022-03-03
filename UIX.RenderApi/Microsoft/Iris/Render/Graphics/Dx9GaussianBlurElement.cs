// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9GaussianBlurElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9GaussianBlurElement
    {
        private static void GenerateWeights1D(int nKernelRadius, double flDeviation, double[] arKernel)
        {
            Debug2.Validate(flDeviation > 0.0, typeof(ArgumentOutOfRangeException), "Standard deviation must be > 0.0");
            Debug2.Validate(nKernelRadius >= 1, typeof(ArgumentOutOfRangeException), "Kernel radius must be >= 1");
            int num1 = 1 + 2 * nKernelRadius;
            double num2 = 0.0;
            Debug2.Validate(arKernel != null && num1 == arKernel.GetLength(0), typeof(ArgumentException), nameof(arKernel));
            double num3 = 2.0 * (flDeviation * flDeviation);
            double num4 = Math.Sqrt(2.0 * Math.PI) * flDeviation;
            for (int index = 0; index < num1; ++index)
            {
                double num5 = index - nKernelRadius;
                double num6 = Math.Exp(-(num5 * num5) / num3);
                arKernel[index] = num6 / num4;
                num2 += arKernel[index];
            }
            for (int index = 0; index < num1; ++index)
                arKernel[index] /= num2;
        }

        private static void GenerateWeights2D(
          int nKernelRadius,
          double flDeviation,
          double[,] arKernel)
        {
            Debug2.Validate(flDeviation > 0.0, typeof(ArgumentOutOfRangeException), "Standard deviation must be > 0.0");
            Debug2.Validate(nKernelRadius >= 1, typeof(ArgumentOutOfRangeException), "Kernel radius must be >= 1");
            double num1 = 0.0;
            int num2 = 1 + 2 * nKernelRadius;
            Debug2.Validate(arKernel != null && num2 == arKernel.GetLength(0) && num2 == arKernel.GetLength(1), typeof(ArgumentException), nameof(arKernel));
            double num3 = flDeviation * flDeviation;
            double num4 = 2.0 * num3;
            double num5 = 2.0 * Math.PI * num3;
            for (int index1 = 0; index1 < num2; ++index1)
            {
                for (int index2 = 0; index2 < num2; ++index2)
                {
                    double num6 = index2 - nKernelRadius;
                    double num7 = index1 - nKernelRadius;
                    double num8 = Math.Exp(-(num6 * num6 + num7 * num7) / num4);
                    arKernel[index2, index1] = num8 / num5;
                    num1 += arKernel[index2, index1];
                }
            }
            for (int index1 = 0; index1 < num2; ++index1)
            {
                for (int index2 = 0; index2 < num2; ++index2)
                    arKernel[index2, index1] /= num1;
            }
        }

        internal static void Generate(GaussianBlurElement efoBlur, ref Dx9EffectBuilder effectBuilder)
        {
            Dx9TextureInfo textureInfo = effectBuilder.GetTextureInfo(effectBuilder.ImageCount - 1);
            effectBuilder.AddRequirement(textureInfo, Dx9TextureRequirements.TexelSize, null);
            switch (efoBlur.Mode)
            {
                case GaussianBlurMode.Normal:
                    int length1 = 1 + efoBlur.KernelRadius * 2;
                    double[,] arKernel1 = new double[length1, length1];
                    GenerateWeights2D(efoBlur.KernelRadius, efoBlur.Bluriness, arKernel1);
                    effectBuilder.EmitPixelFragment("    {\r\n        // Gaussian filter\r\n        float4 vBlur = 0;\r\n");
                    for (int index1 = 0; index1 < length1; ++index1)
                    {
                        for (int index2 = 0; index2 < length1; ++index2)
                        {
                            int num1 = index2 - efoBlur.KernelRadius;
                            int num2 = index1 - efoBlur.KernelRadius;
                            if (num1 == 0 && num2 == 0)
                                effectBuilder.EmitPixelFragment(InvariantString.Format("        vBlur += {0} * {1};\r\n", effectBuilder.PixelShaderOutput, arKernel1[index2, index1]));
                            else
                                effectBuilder.EmitPixelFragment(InvariantString.Format("        vBlur += tex2D({0}, {1} + float2({2}.x * {3}, {2}.y * {4})) * {5};\r\n", textureInfo.Sampler, textureInfo.TexCoordInput, textureInfo.TexelSize, num1, num2, arKernel1[index2, index1]));
                        }
                    }
                    effectBuilder.EmitPixelFragment(InvariantString.Format("        {0} = vBlur;\r\n    }}\r\n\r\n", effectBuilder.PixelShaderOutput));
                    break;
                case GaussianBlurMode.Horizontal:
                case GaussianBlurMode.Vertical:
                    bool flag = efoBlur.Mode == GaussianBlurMode.Horizontal;
                    int length2 = 1 + efoBlur.KernelRadius * 2;
                    double[] arKernel2 = new double[length2];
                    GenerateWeights1D(efoBlur.KernelRadius, efoBlur.Bluriness, arKernel2);
                    effectBuilder.EmitPixelFragment("    {\r\n        // Gaussian filter\r\n        float4 vBlur = 0;\r\n");
                    for (int index = 0; index < length2; ++index)
                    {
                        int num = index - efoBlur.KernelRadius;
                        if (num == 0)
                            effectBuilder.EmitPixelFragment(InvariantString.Format("        vBlur += {0} * {1};\r\n", effectBuilder.PixelShaderOutput, arKernel2[index]));
                        else if (flag)
                            effectBuilder.EmitPixelFragment(InvariantString.Format("        vBlur += tex2D({0}, {1} + float2({2}.x * {3}, 0)) * {4};\r\n", textureInfo.Sampler, textureInfo.TexCoordInput, textureInfo.TexelSize, num, arKernel2[index]));
                        else
                            effectBuilder.EmitPixelFragment(InvariantString.Format("        vBlur += tex2D({0}, {1} + float2(0, {2}.y * {3})) * {4};\r\n", textureInfo.Sampler, textureInfo.TexCoordInput, textureInfo.TexelSize, num, arKernel2[index]));
                    }
                    effectBuilder.EmitPixelFragment(InvariantString.Format("        {0} = vBlur;\r\n    }}\r\n\r\n", effectBuilder.PixelShaderOutput));
                    break;
                default:
                    Debug2.Validate(false, typeof(InvalidOperationException), "Unknown blur mode");
                    break;
            }
        }
    }
}
