using System;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Placeholder host-window used to embed native HWND content inside the scene.
    /// Cross-platform HWND hosting has no in-process GL equivalent, so this tracks
    /// state only. Stage-3 TODO: platform-gated child-surface embedding.
    /// </summary>
    public sealed class GLHwndHostWindow : IHwndHostWindow
    {
        public ColorF BackgroundColor { get; set; }
        public Point ClientPosition { get; set; }
        public IntPtr Hwnd => IntPtr.Zero;
        public bool Visible { get; set; }
        public Size WindowSize { get; set; }

        public event EventHandler? OnHandleChanged;

        public void Dispose() => OnHandleChanged = null;
    }
}
