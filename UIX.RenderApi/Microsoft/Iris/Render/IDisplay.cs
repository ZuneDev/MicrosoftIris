// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IDisplay
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public interface IDisplay
    {
        string DeviceName { get; }

        Rectangle ScreenArea { get; }

        Rectangle WorkArea { get; }

        bool IsPrimary { get; }

        TvFormat TvFormat { get; }

        bool TvMode { get; }

        Size LogicalFullScreenResolution { get; }

        DisplayMode[] SupportedModes { get; }

        DisplayMode[] ExtraModes { get; }

        DisplayMode[] AllModes { get; }

        DisplayMode CurrentMode { get; }

        DisplayMode DesktopMode { get; }

        string MonitorPnP { get; }

        bool ValidateDisplayMode(
          DisplayMode modeDesired,
          DisplayModeFlags nCheck,
          bool fAllowAllModes,
          out DisplayMode modeComplete,
          out DisplayModeFlags nCompleteCheck);

        bool ChangeFullScreenResolution(DisplayMode modeChanges, DisplayModeFlags nValid);
    }
}
