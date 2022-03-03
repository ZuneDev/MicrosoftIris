// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.GaussianBlurElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class GaussianBlurElement : EffectOperation
    {
        internal const string ModePropertyName = "Mode";
        internal const string KernelRadiusPropertyName = "KernelRadius";
        internal const string BlurinessPropertyName = "Bluriness";
        private GaussianBlurMode m_blurMode;
        private byte m_nModeID;
        private int m_nKernelRadius;
        private byte m_nKernelRadiusID;
        private float m_flBluriness;
        private byte m_nBlurinessID;

        public GaussianBlurElement(GaussianBlurMode blurMode, int nKernalRadius, float flBluriness)
        {
            Debug2.Validate(flBluriness > 0.0 && flBluriness <= 6.0, typeof(ArgumentOutOfRangeException), "Valid range for Bluriness is (0,6]");
            Debug2.Validate(nKernalRadius >= 1 && nKernalRadius <= 6, typeof(ArgumentOutOfRangeException), "Valid range for KernelRadius is [1,6]");
            this.m_blurMode = blurMode;
            this.m_nKernelRadius = nKernalRadius;
            this.m_flBluriness = flBluriness;
            this.m_typeOperation = EffectOperationType.GaussianBlur;
        }

        public GaussianBlurElement()
        {
            this.m_blurMode = GaussianBlurMode.Normal;
            this.m_nKernelRadius = 1;
            this.m_flBluriness = 1.5f;
            this.m_typeOperation = EffectOperationType.GaussianBlur;
        }

        public GaussianBlurMode Mode
        {
            get => this.m_blurMode;
            set => this.m_blurMode = value;
        }

        public int KernelRadius
        {
            get => this.m_nKernelRadius;
            set
            {
                Debug2.Validate(value >= 1 && value <= 6, typeof(ArgumentOutOfRangeException), "Valid range for KernelRadius is [1,6]");
                this.m_nKernelRadius = value;
            }
        }

        public float Bluriness
        {
            get => this.m_flBluriness;
            set
            {
                Debug2.Validate(value > 0.0 && value <= 6.0, typeof(ArgumentOutOfRangeException), "Valid range for Bluriness is (0,6]");
                this.m_flBluriness = value;
            }
        }

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Mode", 5, ref this.m_nModeID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "KernelRadius", 5, ref this.m_nKernelRadiusID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Bluriness", 8, ref this.m_nBlurinessID, ref nNextUniqueID);
        }

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Mode", this.m_nModeID, (byte)this.m_blurMode, cacheKey);
            this.GeneratePropertyCacheKey("KernelRadius", this.m_nKernelRadiusID, this.m_nKernelRadius, cacheKey);
            this.GeneratePropertyCacheKey("Bluriness", this.m_nBlurinessID, this.m_flBluriness, cacheKey);
        }
    }
}
