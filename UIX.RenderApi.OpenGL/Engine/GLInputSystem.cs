using Microsoft.Iris.Input;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Holds the current raw-input callback sink and mouse-capture site. The input
    /// translator (see <see cref="GLInputTranslator"/>) reads <see cref="Callbacks"/>
    /// each event and dispatches translated Silk.NET input to it.
    /// </summary>
    public sealed class GLInputSystem : IInputSystem
    {
        public IRawInputCallbacks? Callbacks { get; private set; }

        /// <summary>
        /// The visual that has grabbed the mouse via <see cref="IRenderWindow.SetCapture"/>,
        /// or null. When set, mouse events are routed to it regardless of hit-testing.
        /// </summary>
        public IRawInputSite? CaptureSite { get; set; }

        public void RegisterRawInputCallbacks(IRawInputCallbacks handlers) => Callbacks = handlers;

        public void UnregisterRawInputCallbacks() => Callbacks = null;
    }
}
