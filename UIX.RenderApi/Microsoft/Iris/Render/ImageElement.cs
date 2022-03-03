// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ImageElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class ImageElement : EffectInput
    {
        internal const string ImagePropertyName = "Image";
        internal const string UVOffsetPropertyName = "UVOffset";
        private IImage m_image;
        private byte m_nImageID;
        private Vector2 m_UVOffset;
        private byte m_nUVOffsetID;

        public ImageElement(string stName, IImage imgSource)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            this.m_stName = stName;
            this.m_image = imgSource;
        }

        public ImageElement()
        {
            this.m_stName = nameof(Image);
            this.m_typeInput = EffectInputType.Image;
        }

        public IImage Image
        {
            get => this.m_image;
            set => this.m_image = value;
        }

        internal byte ImageID => this.m_nImageID;

        public Vector2 UVOffset
        {
            get => this.m_UVOffset;
            set => this.m_UVOffset = value;
        }

        internal byte UVOffsetID => this.m_nUVOffsetID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            int num = base.PreProcessProperties(dictionary, ref nNextUniqueID);
            this.AddEffectProperty(dictionary, "Image");
            this.m_nImageID = nNextUniqueID++;
            return num + this.PreProcessProperty(dictionary, "UVOffset", 12, ref this.m_nUVOffsetID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties)
        {
            if (!this.GenerateProperty("Image", EffectPropertyType.Image, m_image, this.m_nImageID, dictProperties))
                return false;
            dictProperties[this.GeneratePropertyPath("Image")].IsDynamic = true;
            return this.GenerateProperty("UVOffset", EffectPropertyType.Vector2, m_UVOffset, this.m_nUVOffsetID, dictProperties);
        }

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("UVOffset", this.UVOffsetID, this.UVOffset, cacheKey);
        }
    }
}
