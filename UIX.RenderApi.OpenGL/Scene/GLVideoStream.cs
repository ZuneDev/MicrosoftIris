namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Placeholder video stream. Metadata is tracked but no decoding/presentation is
    /// wired up yet (stage-3 TODO: back this with a media pipeline and a GL texture).
    /// </summary>
    public sealed class GLVideoStream : SharedRenderObject, IVideoStream
    {
        private static int s_nextId = 1;

        public GLVideoStream() => StreamID = s_nextId++;

        public int StreamID { get; }
        public float ContentOverscan { get; set; }
        public int ContentAspectWidth => ContentWidth;
        public int ContentAspectHeight => ContentHeight;
        public int ContentHeight { get; internal set; }
        public int ContentWidth { get; internal set; }

        public event InvalidateContentHandler? InvalidateContentEvent;

        internal void RaiseInvalidateContent() => InvalidateContentEvent?.Invoke();
    }
}
