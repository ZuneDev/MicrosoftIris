using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// An externally-driven animation input. Providers publish named values that
    /// animations can reference. We store the published values; wiring them into
    /// keyframe evaluation is a stage-3 TODO alongside full animation support.
    /// </summary>
    public sealed class GLExternalAnimationInput : SharedRenderObject, IExternalAnimationInput
    {
        private static uint s_nextId = 1;

        public GLExternalAnimationInput(IAnimationPropertyMap? propertyMap)
        {
            UniqueId = s_nextId++;
            PropertyMap = propertyMap;
        }

        public uint UniqueId { get; }
        internal IAnimationPropertyMap? PropertyMap { get; }

        public IAnimationInputProvider CreateProvider(object objUser) => new GLAnimationInputProvider();
    }

    public sealed class GLAnimationInputProvider : SharedRenderObject, IAnimationInputProvider
    {
        private readonly Dictionary<string, object> m_values = new Dictionary<string, object>();

        public void PublishFloat(string propertyName, float value) => m_values[propertyName] = value;
        public void PublishVector2(string propertyName, Vector2 value) => m_values[propertyName] = value;
        public void PublishVector3(string propertyName, Vector3 value) => m_values[propertyName] = value;
        public void PublishVector4(string propertyName, Vector4 value) => m_values[propertyName] = value;
        public void PublishQuaternion(string propertyName, Quaternion value) => m_values[propertyName] = value;

        public void RevokeFloat(string propertyName) => m_values.Remove(propertyName);
        public void RevokeVector2(string propertyName) => m_values.Remove(propertyName);
        public void RevokeVector3(string propertyName) => m_values.Remove(propertyName);
        public void RevokeVector4(string propertyName) => m_values.Remove(propertyName);
        public void RevokeQuaternion(string propertyName) => m_values.Remove(propertyName);
    }
}
