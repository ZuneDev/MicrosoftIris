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
            if (engineInfo == null) throw new ArgumentNullException(nameof(engineInfo));
            if (renderHost == null) throw new ArgumentNullException(nameof(renderHost));
            return new GLRenderEngine(engineInfo, renderHost);
        }

        /// <summary>
        /// Convenience overload matching <c>RenderApi.CreateEngine</c>'s signature.
        /// Only <see cref="IrisEngineInfo"/> is supported (the sole EngineType).
        /// </summary>
        public static IRenderEngine CreateEngine(EngineInfo engineInfo, IRenderHost renderHost)
        {
            if (engineInfo is not IrisEngineInfo iris)
                throw new ArgumentException("Only IrisEngineInfo is supported.", nameof(engineInfo));
            return CreateEngine(iris, renderHost);
        }
    }
}
