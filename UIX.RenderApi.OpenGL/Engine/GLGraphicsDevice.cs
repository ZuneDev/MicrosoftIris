using System;
using Silk.NET.OpenGL;

namespace Microsoft.Iris.Render.OpenGL.Engine
{
    /// <summary>
    /// Graphics device wrapping the live OpenGL context.
    /// </summary>
    public sealed class GLGraphicsDevice : IGraphicsDevice
    {
        private readonly GL m_gl;
        private readonly Action m_renderNow;
        private string? m_captureFileName;

        public GLGraphicsDevice(GL gl, GraphicsRenderingQuality quality, Action renderNow)
        {
            m_gl = gl;
            m_renderNow = renderNow;
            RenderingQuality = quality;

            m_gl.GetInteger(GetPName.MaxTextureSize, out int maxTex);
            if (maxTex <= 0)
                maxTex = 2048;
            MaximumImageSize = new Size(maxTex, maxTex);
        }

        // The RenderApi enum has no OpenGL member; we present as the hardware-accelerated
        // path (Direct3D9) since the UI branches on GDI-vs-accelerated, not the exact API.
        // TODO(stage 3): extend GraphicsDeviceType if a distinct OpenGL identity is needed.
        public GraphicsDeviceType DeviceType => GraphicsDeviceType.Direct3D9;

        public Size MaximumImageSize { get; }
        public bool IsVideoComposited => false;
        public GraphicsRenderingQuality RenderingQuality { get; }

        public event BackBufferCapturedHandler? BackBufferCapturedEvent;

        public void RenderNowIfPossible() => m_renderNow();

        public void BeginCaptureBackBuffer(string stFileName) => m_captureFileName = stFileName;

        public void EndCaptureBackBuffer()
        {
            // TODO(stage 3): read back the framebuffer to m_captureFileName. For now we
            // only signal completion so callers waiting on the event proceed.
            m_captureFileName = null;
            BackBufferCapturedEvent?.Invoke();
        }

        // The context is not lost/reset in the windowed in-process model.
        public void TriggerDeviceReset() { }
    }
}
