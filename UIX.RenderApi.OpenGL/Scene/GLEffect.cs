using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Template describing an effect (pixel-shader program in the original renderer).
    /// We treat effects as typed property bags; the built-in image path reads the first
    /// image property. Custom shader compilation is a stage-3 TODO.
    /// </summary>
    public sealed class GLEffectTemplate : SharedRenderObject, IEffectTemplate
    {
        private readonly List<string> m_properties = new List<string>();

        public GLEffectTemplate(string name) => Name = name;

        public string Name { get; }
        public bool IsBuilt { get; private set; }

        public void AddEffectProperty(string stPath) => m_properties.Add(stPath);

        public bool Build(EffectInput input)
        {
            // No shader compilation yet; mark as built so callers proceed. The image
            // path in GLSprite does not depend on a compiled program.
            IsBuilt = true;
            return true;
        }

        public IEffect CreateInstance(object objUser) => new GLEffect(this);
    }

    /// <summary>Instance of an <see cref="GLEffectTemplate"/> holding property values.</summary>
    public sealed class GLEffect : SharedRenderObject, IEffect
    {
        private readonly Dictionary<string, object> m_values = new Dictionary<string, object>();

        public GLEffect(GLEffectTemplate template) => Template = template;

        public string Name => Template.Name;
        IEffectTemplate IEffect.Template => Template;
        public GLEffectTemplate Template { get; }

        public void SetProperty(string stPropertyName, int nValue) => m_values[stPropertyName] = nValue;
        public void SetProperty(string stPropertyName, float flValue) => m_values[stPropertyName] = flValue;
        public void SetProperty(string stPropertyName, Vector2 vValue) => m_values[stPropertyName] = vValue;
        public void SetProperty(string stPropertyName, Vector3 vValue) => m_values[stPropertyName] = vValue;
        public void SetProperty(string stPropertyName, Vector4 vValue) => m_values[stPropertyName] = vValue;
        public void SetProperty(string stPropertyName, ColorF colorValue) => m_values[stPropertyName] = colorValue;
        public void SetProperty(string stPropertyName, IImage imgValue) => m_values[stPropertyName] = imgValue;
        public void SetProperty(string stPropertyName, IImage[] imgValue) => m_values[stPropertyName] = imgValue;
        public void SetProperty(string stPropertyName, IVideoStream streamValue) => m_values[stPropertyName] = streamValue;

        /// <summary>First image assigned to any property, used as the sprite's texture.</summary>
        internal GLImage? PrimaryImage
        {
            get
            {
                foreach (var v in m_values.Values)
                {
                    if (v is GLImage img)
                        return img;
                    if (v is IImage[] arr && arr.Length > 0 && arr[0] is GLImage first)
                        return first;
                }
                return null;
            }
        }

        // "ColorElem.Color" is the property key EffectManager.ColorEffectTemplate/
        // CreateColorFillEffect (UIX/Microsoft/Iris/Session/EffectManager.cs) builds its
        // single-color fill effect around, and it's what ViewItem.OnPaint sets on every
        // view item's background sprite (UIX/Microsoft/Iris/UI/ViewItem.cs) whenever the
        // item has a non-transparent background color -- i.e. most of the visible UI.
        // GLEffectTemplate doesn't compile real shader programs (stage-3 TODO), so this
        // is the one property path GLSprite needs to special-case for solid fills.
        internal ColorF? PrimaryColor
            => m_values.TryGetValue("ColorElem.Color", out var v) && v is ColorF c ? c : null;
    }
}
