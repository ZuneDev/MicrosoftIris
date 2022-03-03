// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt.IHwndHostWindowCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt
{
    internal interface IHwndHostWindowCallback
    {
        void OnHandleChanged(RENDERHANDLE target, HWND m_hWnd);
    }
}
