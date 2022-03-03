// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IRenderWindow
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;

namespace Microsoft.Iris.Render
{
    public interface IRenderWindow
    {
        int Left { get; }

        int Top { get; }

        int Right { get; }

        int Bottom { get; }

        int Width { get; }

        int Height { get; }

        HWND WindowHandle { get; }

        Size ClientSize { get; set; }

        Size InitialClientSize { get; set; }

        FormPlacement InitialPlacement { set; }

        FormPlacement FinalPlacement { get; }

        int MinResizeWidth { get; set; }

        int MaxResizeWidth { get; set; }

        Point Position { get; set; }

        string Text { get; set; }

        Cursor Cursor { get; set; }

        Cursor IdleCursor { get; set; }

        bool Visible { get; set; }

        bool IsLoaded { get; }

        ColorF BackgroundColor { get; set; }

        bool EnableExternalDragDrop { get; set; }

        bool IsDragInProgress { get; set; }

        IDisplay CurrentDisplay { get; set; }

        bool FullScreenExclusive { get; set; }

        bool ActivationState { get; }

        WindowState WindowState { get; set; }

        FormStyleInfo Styles { get; set; }

        HWND AppNotifyWindow { set; }

        IVisualContainer VisualRoot { get; }

        void Initialize();

        void SetIcon(string sModuleName, uint nResourceID, IconFlags nOptions);

        void SetEdgeImages(bool fActiveEdges, ShadowEdgePart[] edges);

        void SetWindowOptions(WindowOptions options, bool enable);

        void SetMouseIdleOptions(Size sizeMouseIdleTolerance, uint nMouseIdleDelay);

        void SetCapture(IRawInputSite captureSite, bool state);

        void SetDragDropResult(uint nDragOverResult, uint nDragDropResult);

        void ClientToScreen(ref Point point);

        void ScreenToClient(ref Point point);

        void ForceMouseIdle(bool fIdle);

        void LockMouseActive(bool fActive);

        void RefreshHitTarget();

        void TakeFocus();

        void TakeForeground(bool fForce);

        void BringToTop();

        void Restore();

        void TemporarilyExitExclusiveMode();

        void Close(FormCloseReason fcrCloseReason);

        IHwndHostWindow CreateHwndHostWindow();

        event LocationChangedHandler LocationChangedEvent;

        event SizeChangedHandler SizeChangedEvent;

        event MonitorChangedHandler MonitorChangedEvent;

        event WindowStateChangedHandler WindowStateChangedEvent;

        event SysCommandHandler SysCommandEvent;

        event MouseIdleHandler MouseIdleEvent;

        event ShowHandler ShowEvent;

        event ActivationChangeHandler ActivationChangeEvent;

        event SessionActivateHandler SessionActivateEvent;

        event SessionConnectHandler SessionConnectEvent;

        event SetFocusHandler SetFocusEvent;

        event LoadHandler LoadEvent;

        event CloseHandler CloseEvent;

        event CloseRequestHandler CloseRequestEvent;

        event ForwardMessageHandler ForwardMessageEvent;
    }
}
