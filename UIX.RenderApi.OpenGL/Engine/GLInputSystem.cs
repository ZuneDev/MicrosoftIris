namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Holds the current raw-input callback sink. The window (see <see cref="GLRenderWindow"/>)
    /// translates Silk.NET input events and dispatches them here.
    /// </summary>
    public sealed class GLInputSystem : IInputSystem
    {
        public IRawInputCallbacks? Callbacks { get; private set; }

        public void RegisterRawInputCallbacks(IRawInputCallbacks handlers) => Callbacks = handlers;

        public void UnregisterRawInputCallbacks() => Callbacks = null;
    }
}
