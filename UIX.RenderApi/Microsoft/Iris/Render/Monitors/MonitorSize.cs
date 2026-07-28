// Based on decompilation from JetBrains decompiler
// Type: ZuneUI.MonitorSize
// Assembly: ZuneShell, Version=4.7.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: FC8028F3-A47B-4FB4-B35B-11D1752D8264
// Assembly location: C:\Program Files\Zune\ZuneShell.dll

namespace Microsoft.Iris.Render.Monitors;

public struct MonitorSize
{
    public readonly Rectangle TotalArea;
    public readonly Rectangle WorkArea;

    public MonitorSize(Rectangle rcTotal, Rectangle rcWork)
    {
        TotalArea = rcTotal;
        WorkArea = rcWork;
    }
}