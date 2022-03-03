// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.EffectElementWrapper
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.VideoPlayback;

namespace Microsoft.Iris.UI
{
    internal class EffectElementWrapper
    {
        private EffectClass _class;
        private string _elementName;
        private static Map<int, string> s_propertyMap;

        public EffectElementWrapper(EffectClass cls, string elementName)
        {
            _class = cls;
            _elementName = elementName;
        }

        public void SetProperty(string propertyName, int value) => _class.SetRenderEffectProperty(MakeEffectPropertyName(propertyName), new EffectValue(value, EffectValueType.Int));

        public void SetProperty(string propertyName, float value) => _class.SetRenderEffectProperty(MakeEffectPropertyName(propertyName), new EffectValue(value, EffectValueType.Float));

        public void SetProperty(string propertyName, UIImage value) => _class.SetRenderEffectProperty(MakeEffectPropertyName(propertyName), new EffectValue(value, EffectValueType.UIImage));

        public void SetProperty(string propertyName, IUIVideoStream value) => _class.SetRenderEffectProperty(MakeEffectPropertyName(propertyName), new EffectValue(value, EffectValueType.IUIVideoStream));

        public void SetProperty(string propertyName, Color value) => _class.SetRenderEffectProperty(MakeEffectPropertyName(propertyName), new EffectValue(value, EffectValueType.Color));

        public void SetProperty(string propertyName, Vector2 value) => _class.SetRenderEffectProperty(MakeEffectPropertyName(propertyName), new EffectValue(value, EffectValueType.Vector2));

        public void SetProperty(string propertyName, Vector3 value) => _class.SetRenderEffectProperty(MakeEffectPropertyName(propertyName), new EffectValue(value, EffectValueType.Vector3));

        public void PlayAnimation(EffectProperty property, EffectAnimation animation) => _class.PlayAnimation(MakeEffectPropertyName(property), animation);

        private string MakeEffectPropertyName(string propertyName) => MakeEffectPropertyName(_elementName, propertyName);

        private string MakeEffectPropertyName(EffectProperty property) => MakeEffectPropertyName(_elementName, property);

        public static string MakeEffectPropertyName(string elementName, EffectProperty property)
        {
            EnsurePropertyMap();
            return MakeEffectPropertyName(elementName, s_propertyMap[(int)property]);
        }

        public static string MakeEffectPropertyName(string elementName, string propertyName) => elementName + "." + propertyName;

        private static void EnsurePropertyMap()
        {
            if (s_propertyMap != null)
                return;
            s_propertyMap = new Map<int, string>();
            s_propertyMap[2] = "Attenuation";
            s_propertyMap[3] = "Brightness";
            s_propertyMap[4] = "Color";
            s_propertyMap[14] = "InnerConeAngle";
            s_propertyMap[18] = "OuterConeAngle";
            s_propertyMap[5] = "Contrast";
            s_propertyMap[6] = "DarkColor";
            s_propertyMap[7] = "Decay";
            s_propertyMap[8] = "Density";
            s_propertyMap[9] = "Desaturate";
            s_propertyMap[10] = "DirectionAngle";
            s_propertyMap[11] = "EdgeLimit";
            s_propertyMap[12] = "FallOff";
            s_propertyMap[13] = "Hue";
            s_propertyMap[15] = "Intensity";
            s_propertyMap[16] = "LightColor";
            s_propertyMap[1] = "AmbientColor";
            s_propertyMap[17] = "Lightness";
            s_propertyMap[19] = "Position";
            s_propertyMap[20] = "Radius";
            s_propertyMap[21] = "Saturation";
            s_propertyMap[22] = "Tone";
            s_propertyMap[23] = "Weight";
            s_propertyMap[24] = "Value";
            s_propertyMap[25] = "Downsample";
        }
    }
}
