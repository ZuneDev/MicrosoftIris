// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ComplexImageElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class ComplexImageElement : EffectInput
    {
        internal const string ImagePropertyName = "Image";
        internal const string MinFilterPropertyName = "MinFilter";
        internal const string MagFilterPropertyName = "MagFilter";
        internal const string CoordinateMapIndexPropertyName = "CoordinateMapIndex";
        internal const string ImageIndexPropertyName = "ImageIndex";
        private IImage[] m_rgImages;
        private byte m_nImageID;
        private FilterMode m_modeMinFilter;
        private byte m_nMinFilterID;
        private FilterMode m_modeMagFilter;
        private byte m_nMagFilterID;
        private byte m_idxCoordinateMap;
        private byte m_nCoordinateMapID;
        private byte m_idxImageIndex;
        private byte m_nImageIndexID;

        public ComplexImageElement(
          string stName,
          IImage imgSource,
          FilterMode ftrMinification,
          FilterMode ftrMagnification,
          byte idxCoordinateMap)
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(imgSource != null, typeof(ArgumentNullException), nameof(imgSource));
            this.m_typeInput = EffectInputType.ComplexImage;
            this.m_stName = stName;
            this.m_modeMinFilter = ftrMinification;
            this.m_modeMagFilter = ftrMagnification;
            this.m_idxCoordinateMap = idxCoordinateMap;
            this.m_idxImageIndex = 0;
            this.Image = imgSource;
        }

        public ComplexImageElement(
          string stName,
          IImage[] imgSource,
          FilterMode ftrMinification,
          FilterMode ftrMagnification,
          byte idxCoordinateMap,
          byte idxImageIndex)
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentNullException), nameof(stName));
            Debug2.Validate(imgSource != null, typeof(ArgumentNullException), nameof(imgSource));
            Debug2.Validate(imgSource.Length > 0, typeof(ArgumentException), "ImgSource");
            Debug2.Validate(imgSource.Length > idxImageIndex, typeof(ArgumentOutOfRangeException), nameof(idxImageIndex));
            this.m_typeInput = EffectInputType.ComplexImage;
            this.m_stName = stName;
            this.m_modeMinFilter = ftrMinification;
            this.m_modeMagFilter = ftrMagnification;
            this.m_idxCoordinateMap = idxCoordinateMap;
            this.m_idxImageIndex = idxImageIndex;
            this.m_rgImages = imgSource;
        }

        public IImage Image
        {
            get => this.m_rgImages == null || this.m_rgImages.Length < 1 ? null : this.m_rgImages[0];
            set
            {
                this.m_rgImages = new IImage[1];
                this.m_rgImages[0] = value;
            }
        }

        public IImage[] Images
        {
            get => this.m_rgImages;
            set
            {
                Debug2.Validate(value.Length > 0, typeof(ArgumentException), nameof(Images));
                this.m_rgImages = value;
            }
        }

        internal byte ImageID => this.m_nImageID;

        public FilterMode MinFilter
        {
            get => this.m_modeMinFilter;
            set => this.m_modeMinFilter = value;
        }

        internal byte MinFilterID => this.m_nMinFilterID;

        public FilterMode MagFilter
        {
            get => this.m_modeMagFilter;
            set => this.m_modeMagFilter = value;
        }

        internal byte MagFilterID => this.m_nMagFilterID;

        public byte CoordinateMapIndex
        {
            get => this.m_idxCoordinateMap;
            set => this.m_idxCoordinateMap = value;
        }

        public byte CoordinateMapIndexID => this.m_nCoordinateMapID;

        public byte ImageIndex
        {
            get => this.m_idxImageIndex;
            set => this.m_idxImageIndex = value;
        }

        public byte ImageIndexID => this.m_nImageIndexID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            int num = base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "MinFilter", 5, ref this.m_nMinFilterID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "MagFilter", 5, ref this.m_nMagFilterID, ref nNextUniqueID);
            this.AddEffectProperty(dictionary, "CoordinateMapIndex");
            this.m_nCoordinateMapID = nNextUniqueID++;
            this.AddEffectProperty(dictionary, "Image");
            this.m_nImageID = nNextUniqueID++;
            this.AddEffectProperty(dictionary, "ImageIndex");
            this.m_nImageIndexID = nNextUniqueID++;
            return num;
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties)
        {
            if (!this.GenerateProperty("MinFilter", EffectPropertyType.Integer, (int)this.MinFilter, this.m_nMinFilterID, dictProperties) || !this.GenerateProperty("MagFilter", EffectPropertyType.Integer, (int)this.MagFilter, this.m_nMagFilterID, dictProperties) || (!this.GenerateProperty("CoordinateMapIndex", EffectPropertyType.Integer, (int)this.CoordinateMapIndex, this.m_nCoordinateMapID, dictProperties) || !this.GenerateProperty("ImageIndex", EffectPropertyType.Integer, (int)this.ImageIndex, this.m_nImageIndexID, dictProperties)))
                return false;
            SharedResource[] rgResources = new SharedResource[this.m_rgImages.Length];
            for (int index = 0; index < this.m_rgImages.Length; ++index)
                rgResources[index] = (SharedResource)this.m_rgImages[index];
            return this.GenerateProperty("Image", EffectPropertyType.ImageArray, new SharedResourceArray(rgResources), this.m_nImageID, dictProperties);
        }

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("MinFilter", this.MinFilterID, (byte)this.MinFilter, cacheKey);
            this.GeneratePropertyCacheKey("MagFilter", this.MagFilterID, (byte)this.MagFilter, cacheKey);
        }
    }
}
