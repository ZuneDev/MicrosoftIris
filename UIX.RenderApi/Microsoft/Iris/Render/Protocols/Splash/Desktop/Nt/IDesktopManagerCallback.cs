// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt.IDesktopManagerCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt
{
    internal interface IDesktopManagerCallback
    {
        void OnEndDisplayModes(RENDERHANDLE target);

        void OnDisplayMode(RENDERHANDLE target, RenderDisplayMode mode, bool fSupported);

        void OnBeginDisplayModes(RENDERHANDLE target);

        void OnEndEnumMonitorInfo(RENDERHANDLE target);

        unsafe void OnMonitorInfo(RENDERHANDLE target, Message* info);

        void OnBeginEnumMonitorInfo(RENDERHANDLE target);
    }
}
