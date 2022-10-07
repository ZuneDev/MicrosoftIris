// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.EffectManager
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using System;

namespace Microsoft.Iris.Session
{
    public class EffectManager : IDisposable
    {
        public const string ColorEffectProperty = "ColorElem.Color";
        public const string ImageEffectProperty = "ImageElem.Image";
        private const string s_colorElement = "ColorElem";
        private const string s_imageElement = "ImageElem";
        private IRenderSession _renderSession;
        private bool _fDisposed;
        private IEffectTemplate _effectTemplateColor;
        private IEffectTemplate _effectTemplateImage;
        private IEffectTemplate _effectTemplateImageWithColor;

        public EffectManager(IRenderSession renderSession) => _renderSession = renderSession;

        public void Dispose() => Dispose(true);

        private void Dispose(bool fDisposing)
        {
            if (!_fDisposed && fDisposing)
            {
                if (_effectTemplateColor != null)
                {
                    _effectTemplateColor.UnregisterUsage(this);
                    _effectTemplateColor = null;
                }
                if (_effectTemplateImage != null)
                {
                    _effectTemplateImage.UnregisterUsage(this);
                    _effectTemplateImage = null;
                }
                if (_effectTemplateImageWithColor != null)
                {
                    _effectTemplateImageWithColor.UnregisterUsage(this);
                    _effectTemplateImageWithColor = null;
                }
                _renderSession = null;
            }
            _fDisposed = true;
        }

        public IEffectTemplate ColorEffectTemplate
        {
            get
            {
                if (_effectTemplateColor == null)
                {
                    _effectTemplateColor = _renderSession.CreateEffectTemplate(this, "ColorEffect");
                    ColorElement colorElement = new ColorElement("ColorElem");
                    _effectTemplateColor.AddEffectProperty("ColorElem.Color");
                    _effectTemplateColor.Build(colorElement);
                }
                return _effectTemplateColor;
            }
        }

        public IEffectTemplate ImageEffectTemplate
        {
            get
            {
                if (_effectTemplateImage == null)
                {
                    _effectTemplateImage = _renderSession.CreateEffectTemplate(this, "ImageEffect");
                    ImageElement imageElement = new ImageElement("ImageElem", null);
                    _effectTemplateImage.AddEffectProperty("ImageElem.Image");
                    _effectTemplateImage.Build(imageElement);
                }
                return _effectTemplateImage;
            }
        }

        public static IEffect CreateColorFillEffect(object oOwner, Color color)
        {
            IEffect instance = UISession.Default.EffectManager.ColorEffectTemplate.CreateInstance(oOwner);
            instance.SetProperty("ColorElem.Color", color.RenderConvert());
            return instance;
        }

        public static IEffect CreateBasicImageEffect(object oOwner, IImage renderImage)
        {
            IEffect instance = UISession.Default.EffectManager.ImageEffectTemplate.CreateInstance(oOwner);
            if (renderImage != null)
                instance.SetProperty("ImageElem.Image", renderImage);
            return instance;
        }
    }
}
