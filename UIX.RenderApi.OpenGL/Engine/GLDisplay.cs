using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// A monitor, described from a Silk.NET <see cref="IMonitor"/>. Resolution changes are
    /// reported as unsupported (the in-process renderer runs windowed).
    /// </summary>
    public sealed class GLDisplay : IDisplay
    {
        public GLDisplay(IMonitor monitor, bool isPrimary)
        {
            DeviceName = monitor.Name ?? $"Monitor{monitor.Index}";
            Rectangle<int> b = monitor.Bounds;
            ScreenArea = new Rectangle(b.Origin.X, b.Origin.Y, b.Size.X, b.Size.Y);
            WorkArea = ScreenArea;
            IsPrimary = isPrimary;

            var size = new Size(b.Size.X, b.Size.Y);
            LogicalFullScreenResolution = size;
            CurrentMode = new DisplayMode
            {
                sizePhysicalPxl = size,
                sizeLogicalPxl = size,
                nRefreshRate = monitor.VideoMode.RefreshRate ?? 60,
                fInterlaced = false,
                fTvMode = false,
            };
        }

        public string DeviceName { get; }
        public Rectangle ScreenArea { get; }
        public Rectangle WorkArea { get; }
        public bool IsPrimary { get; }
        public TvFormat TvFormat => TvFormat.None;
        public bool TvMode => false;
        public Size LogicalFullScreenResolution { get; }

        public DisplayMode[] SupportedModes => new[] { CurrentMode };
        public DisplayMode[] ExtraModes => DisplayMode.EmptyModes;
        public DisplayMode[] AllModes => SupportedModes;
        public DisplayMode CurrentMode { get; }
        public DisplayMode DesktopMode => CurrentMode;
        public string MonitorPnP => DeviceName;

        public bool ValidateDisplayMode(
            DisplayMode modeDesired,
            DisplayModeFlags nCheck,
            bool fAllowAllModes,
            out DisplayMode modeComplete,
            out DisplayModeFlags nCompleteCheck)
        {
            // We only expose the desktop mode, so echo it back as the completed mode.
            modeComplete = CurrentMode;
            nCompleteCheck = nCheck;
            return true;
        }

        // Windowed in-process rendering cannot switch the monitor's resolution.
        public bool ChangeFullScreenResolution(DisplayMode modeChanges, DisplayModeFlags nValid) => false;
    }
}
