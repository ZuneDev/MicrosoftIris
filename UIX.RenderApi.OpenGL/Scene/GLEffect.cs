using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// Template describing an effect (pixel-shader program in the original renderer).
    /// We treat effects as typed property bags; the built-in image path reads the first
    /// image property. Custom shader compilation is a stage-3 TODO.
    /// </summary>
    public sealed class GLEffectTemplate : SharedRenderObject, IEffectTemplate
    {
        private readonly List<string> m_properties = new List<string>();

        public GLEffectTemplate(object syncRoot, string name) : base(syncRoot) => Name = name;

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

        public IEffect CreateInstance(object objUser) => new GLEffect(SyncRoot, this);
    }

    /// <summary>Instance of an <see cref="GLEffectTemplate"/> holding property values.</summary>
    public sealed class GLEffect : SharedRenderObject, IEffect
    {
        private readonly Dictionary<string, object> m_values = new Dictionary<string, object>();

        public GLEffect(object syncRoot, GLEffectTemplate template) : base(syncRoot) => Template = template;

        public string Name => Template.Name;
        IEffectTemplate IEffect.Template => Template;
        public GLEffectTemplate Template { get; }

        // m_values is written by app-side SetProperty *and* by the render thread's
        // animation pulse (AnimationTargetApplier.Apply -> SetProperty, every frame
        // a target animation is playing), and enumerated every frame by
        // PrimaryImage/PrimaryColor from GLSprite.Render -- an unguarded Dictionary
        // under that traffic is a concurrent-modification-during-enumeration crash,
        // not just a stale-read risk.
        public void SetProperty(string stPropertyName, int nValue) { lock (SyncRoot) m_values[stPropertyName] = nValue; }
        public void SetProperty(string stPropertyName, float flValue) { lock (SyncRoot) m_values[stPropertyName] = flValue; }
        public void SetProperty(string stPropertyName, Vector2 vValue) { lock (SyncRoot) m_values[stPropertyName] = vValue; }
        public void SetProperty(string stPropertyName, Vector3 vValue) { lock (SyncRoot) m_values[stPropertyName] = vValue; }
        public void SetProperty(string stPropertyName, Vector4 vValue) { lock (SyncRoot) m_values[stPropertyName] = vValue; }
        public void SetProperty(string stPropertyName, ColorF colorValue) { lock (SyncRoot) m_values[stPropertyName] = colorValue; }
        public void SetProperty(string stPropertyName, IImage imgValue) { lock (SyncRoot) m_values[stPropertyName] = imgValue; }
        public void SetProperty(string stPropertyName, IImage[] imgValue) { lock (SyncRoot) m_values[stPropertyName] = imgValue; }
        public void SetProperty(string stPropertyName, IVideoStream streamValue) { lock (SyncRoot) m_values[stPropertyName] = streamValue; }

        /// <summary>First image assigned to any property, used as the sprite's texture.</summary>
        internal GLImage? PrimaryImage
        {
            get
            {
                lock (SyncRoot)
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
        }

        // "ColorElem.Color" is the property key EffectManager.ColorEffectTemplate/
        // CreateColorFillEffect (UIX/Microsoft/Iris/Session/EffectManager.cs) builds its
        // single-color fill effect around, and it's what ViewItem.OnPaint sets on every
        // view item's background sprite (UIX/Microsoft/Iris/UI/ViewItem.cs) whenever the
        // item has a non-transparent background color -- i.e. most of the visible UI.
        // GLEffectTemplate doesn't compile real shader programs (stage-3 TODO), so this
        // is the one property path GLSprite needs to special-case for solid fills.
        internal ColorF? PrimaryColor
        {
            get { lock (SyncRoot) return m_values.TryGetValue("ColorElem.Color", out var v) && v is ColorF c ? c : null; }
        }
    }
}
