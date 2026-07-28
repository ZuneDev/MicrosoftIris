namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// Camera parameters for a visual container. Stored verbatim; the current renderer
    /// composites in an orthographic screen space, so perspective cameras are recorded
    /// but not yet applied (stage-3 TODO for true 3D containers).
    /// </summary>
    public sealed class GLCamera : SharedRenderObject, ICamera
    {
        public Vector3 Eye { get; set; }
        public Vector3 At { get; set; }
        public Vector3 Up { get; set; } = new Vector3(0f, 1f, 0f);
        public float Zn { get; set; } = 1f;
        public bool Perspective { get; set; }
    }
}
