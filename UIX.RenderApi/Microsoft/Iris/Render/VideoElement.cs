// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.VideoElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class VideoElement : EffectInput
    {
        internal const string VideoPropertyName = "Video";
        private Microsoft.Iris.Render.VideoStream m_videoStream;
        private byte m_nVideoStreamID;

        public VideoElement(string stName, IVideoStream videoStream)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            this.m_stName = stName;
            this.m_videoStream = videoStream as Microsoft.Iris.Render.VideoStream;
        }

        public VideoElement() => this.m_typeInput = EffectInputType.Video;

        public IVideoStream VideoStream
        {
            get => m_videoStream;
            set => this.m_videoStream = value as Microsoft.Iris.Render.VideoStream;
        }

        internal byte VideoStreamID => this.m_nVideoStreamID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            int num = base.PreProcessProperties(dictionary, ref nNextUniqueID);
            this.AddEffectProperty(dictionary, "Video");
            this.m_nVideoStreamID = nNextUniqueID++;
            return num;
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties)
        {
            if (!this.GenerateProperty("Video", EffectPropertyType.Video, VideoStream, this.m_nVideoStreamID, dictProperties))
                return false;
            dictProperties[this.GeneratePropertyPath("Video")].IsDynamic = true;
            return true;
        }
    }
}
