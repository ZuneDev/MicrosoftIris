// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Desktop.IFormWindowCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Protocols.Splash.Desktop
{
    internal interface IFormWindowCallback
    {
        void OnRendererSuspended(RENDERHANDLE target, bool fEnabled);

        void OnNativeScreensave(RENDERHANDLE target, bool fStartScreensave);

        void OnShellShutdownHook(RENDERHANDLE target, ushort uIdMsg);

        void OnSetFocus(RENDERHANDLE target, bool focused, HWND hwndFocusChange);

        void OnDropComplete(RENDERHANDLE target);

        void OnPartialDrop(RENDERHANDLE target, string file);

        unsafe void OnStateChange(RENDERHANDLE target, Message* stateInfo);

        void OnTerminalSessionChange(RENDERHANDLE target, IntPtr wParam, IntPtr lParam);

        void OnPrivateSysCommand(RENDERHANDLE target, IntPtr wParam, IntPtr lParam);

        void OnMouseIdle(RENDERHANDLE target, bool fNewIdle);

        void OnCloseRequested(RENDERHANDLE target);

        void OnLoad(RENDERHANDLE target);

        void OnWindowDestroyed(
          RENDERHANDLE target,
          uint nFinalShow,
          Rectangle rcLastPosition,
          Point ptFinalMaxPosition);

        void OnWindowCreated(RENDERHANDLE target, HWND m_hWnd);
    }
}
