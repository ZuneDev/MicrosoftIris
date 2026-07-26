using System;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Entry point for the in-process, OpenGL-backed renderer. Mirrors
    /// <see cref="Microsoft.Iris.Render.RenderApi.CreateEngine"/>; used instead of it
    /// because <c>RenderApi</c> is fixed to the native Iris engine and must not change.
    /// </summary>
    public static class OpenGLRenderApi
    {
        /// <summary>Create an OpenGL render engine for the given Iris engine info.</summary>
        public static IRenderEngine CreateEngine(IrisEngineInfo engineInfo, IRenderHost renderHost)
        {
            ArgumentNullException.ThrowIfNull(engineInfo);
            ArgumentNullException.ThrowIfNull(renderHost);
            return new GLRenderEngine(engineInfo, renderHost);
        }
    }
}
