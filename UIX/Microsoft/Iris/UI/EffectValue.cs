// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.EffectValue
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.UI
{
    public struct EffectValue
    {
        private object _value;
        private EffectValueType _type;

        public EffectValue(object value, EffectValueType type)
        {
            _value = value;
            _type = type;
        }

        public void SetValueOnEffect(IEffect effect, string property) => SetValueOnEffect(effect, property, _value, _type);

        public static void SetValueOnEffect(
          IEffect effect,
          string property,
          object value,
          EffectValueType type)
        {
            switch (type)
            {
                case EffectValueType.Int:
                    effect.SetProperty(property, (int)value);
                    break;
                case EffectValueType.Float:
                    effect.SetProperty(property, (float)value);
                    break;
                case EffectValueType.UIImage:
                    IImage renderImage = ((UIImage)value)?.RenderImage;
                    effect.SetProperty(property, renderImage);
                    break;
                case EffectValueType.IUIVideoStream:
                    if (!(value is Microsoft.Iris.VideoStream videoStream))
                        break;
                    effect.SetProperty(property, videoStream.RenderStream);
                    break;
                case EffectValueType.Color:
                    effect.SetProperty(property, ((Color)value).RenderConvert());
                    break;
                case EffectValueType.Vector3:
                    effect.SetProperty(property, (Vector3)value);
                    break;
            }
        }
    }
}
