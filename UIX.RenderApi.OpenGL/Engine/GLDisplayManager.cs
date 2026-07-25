using System.Collections.Generic;
using Silk.NET.Windowing;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Enumerates monitors via Silk.NET and exposes them as <see cref="IDisplay"/>s.
    /// </summary>
    public sealed class GLDisplayManager : IDisplayManager
    {
        private readonly List<GLDisplay> m_displays = new List<GLDisplay>();

        public GLDisplayManager(IView view)
        {
            IMonitor? primary = Monitor.GetMainMonitor(view);
            foreach (IMonitor monitor in Monitor.GetMonitors(view))
            {
                bool isPrimary = primary != null && monitor.Index == primary.Index;
                var display = new GLDisplay(monitor, isPrimary);
                if (isPrimary)
                    m_displays.Insert(0, display);
                else
                    m_displays.Add(display);
            }

            if (m_displays.Count == 0 && primary != null)
                m_displays.Add(new GLDisplay(primary, true));
        }

        public IDisplay PrimaryDisplay => m_displays[0];

        public DisplayMode[] ExtraModes { get; set; } = DisplayMode.EmptyModes;

        public IDisplay DisplayFromDeviceName(string stDeviceName)
        {
            foreach (GLDisplay d in m_displays)
            {
                if (d.DeviceName == stDeviceName)
                    return d;
            }
            return PrimaryDisplay;
        }
    }
}
