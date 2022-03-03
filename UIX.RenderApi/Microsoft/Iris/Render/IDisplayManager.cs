// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IDisplayManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public interface IDisplayManager
    {
        IDisplay PrimaryDisplay { get; }

        DisplayMode[] ExtraModes { get; set; }

        IDisplay DisplayFromDeviceName(string stDeviceName);
    }
}
