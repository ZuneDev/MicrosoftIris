using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Os;

// [UnmanagedCallersOnly] exports for the remaining miscellaneous OS entry points in
// UIX/Microsoft/Iris/OS/NativeApi.cs: DPI, mouse cursor metrics, drag-and-drop, the
// notification window, IME plumbing, registry change notification, and the deferred
// invoke helper.
//
// These are OS APIs, not graphics APIs, so per CLAUDE.md they are implemented for real on
// Windows behind `#if WINDOWS` with a documented fallback + TODO elsewhere -- rather than
// pulling in a windowing toolkit (see logs/UIXrender/FullSurface.md, decision 2, for why
// Silk.NET.Windowing/Input are the wrong tool for a library loaded into a host that
// already owns its window and pump).
public static unsafe class SystemApi
{
    // The Win32 "default" logical DPI. Every Iris layout calculation is relative to this,
    // so it is the correct neutral answer where the real value can't be queried -- not a
    // placeholder number.
    private const int DefaultDpi = 96;

    private static uint OK => (uint)HRESULT.S_OK.hr;

    [UnmanagedCallersOnly(EntryPoint = "SpGetDpi")]
    public static int SpGetDpi()
    {
#if WINDOWS
        return (int)Win32.GetDpiForSystem();
#else
        // TODO: query the platform's scale factor (Xft.dpi / GSettings text-scaling-factor
        // on X11, wl_output scale on Wayland) once this project has a display abstraction.
        return DefaultDpi;
#endif
    }

    // Reports the system cursor height and the hotspot's Y offset -- used by Iris to align
    // a custom-drawn cursor with the OS one.
    [UnmanagedCallersOnly(EntryPoint = "SpGetMouseCursorInfo")]
    public static void SpGetMouseCursorInfo(int* height, int* hotY)
    {
        if (height == null || hotY == null)
            return;

#if WINDOWS
        const int SM_CYCURSOR = 14;
        int cursorHeight = Win32.GetSystemMetrics(SM_CYCURSOR);
        *height = cursorHeight > 0 ? cursorHeight : 32;
        // The hotspot of the standard arrow cursor sits at its top-left, so the Y offset
        // is zero; a themed cursor can differ but Win32 exposes no metric for it without
        // loading and inspecting the cursor bitmap itself.
        *hotY = 0;
#else
        // TODO: no cross-platform cursor-metrics abstraction available yet. 32px is the
        // near-universal default cursor size on both Windows and common Linux themes.
        *height = 32;
        *hotY = 0;
#endif
    }

    // Enumerates the file names in a shell drag-and-drop data object, invoking the
    // caller's callback once per file. The data object is an IDataObject COM pointer,
    // which only exists on Windows.
    // TODO: wire to the platform's drag-and-drop protocol (XDND / wl_data_device) when a
    // cross-platform windowing layer exists.
    [UnmanagedCallersOnly(EntryPoint = "SpExtractDroppedFileNames")]
    public static int SpExtractDroppedFileNames(IntPtr punk, IntPtr callback) => 0;

    // The notification window is a hidden, message-only window the original used to
    // receive broadcast messages. It has no cross-platform analogue, and the callback is
    // only ever invoked for NotificationType.GetObject.
    // TODO: implement per-platform if any consumer starts depending on the notifications.
    [UnmanagedCallersOnly(EntryPoint = "SpCreateNotifyWindow")]
    public static HRESULT SpCreateNotifyWindow(IntPtr* handle, IntPtr callback)
    {
        if (handle == null)
            return HRESULT.E_INVALIDARG;
        *handle = IntPtr.Zero;
        return HRESULT.E_NOTIMPL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDestroyNotifyWindow")]
    public static void SpDestroyNotifyWindow() { }

    // IME (input method editor) composition forwarding is a Win32 message-loop concept
    // (WM_IME_STARTCOMPOSITION/WM_IME_ENDCOMPOSITION, which NativeApi.cs declares as
    // constants). The callbacks are held so registration/unregistration round-trips
    // correctly and a token is genuinely issued, but nothing posts to them without a
    // message pump to hook.
    // TODO: forward real composition events once this project owns a window/message loop.
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, IntPtr> s_imeCallbacks = new();
    private static uint s_nextImeToken;

    [UnmanagedCallersOnly(EntryPoint = "SpRegisterImeCallbacks")]
    public static HRESULT SpRegisterImeCallbacks(IntPtr pImeCallbacks, uint* dwToken)
    {
        if (dwToken == null)
            return HRESULT.E_INVALIDARG;

        uint token = System.Threading.Interlocked.Increment(ref s_nextImeToken);
        Interop.Com.ComVtable.AddRef(pImeCallbacks);
        s_imeCallbacks[token] = pImeCallbacks;
        *dwToken = token;
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpUnregisterImeCallbacks")]
    public static HRESULT SpUnregisterImeCallbacks(uint dwToken)
    {
        if (!s_imeCallbacks.TryRemove(dwToken, out IntPtr callbacks))
            return HRESULT.E_INVALIDARG;

        Interop.Com.ComVtable.Release(callbacks);
        return HRESULT.S_OK;
    }

    // Real: fans the message out to every registered IImeCallbacks
    // (OnImeMessageReceived is its single method, hence the first method slot).
    [UnmanagedCallersOnly(EntryPoint = "SpPostDeferredImeMessage")]
    public static HRESULT SpPostDeferredImeMessage(uint message, UIntPtr wParam, UIntPtr lParam)
    {
        foreach (IntPtr callbacks in s_imeCallbacks.Values)
        {
            void* fn = Interop.Com.ComVtable.Slot(callbacks, Interop.Com.ComVtable.FirstMethodSlot);
            if (fn != null)
                ((delegate* unmanaged<IntPtr, uint, UIntPtr, UIntPtr, int>)fn)(callbacks, message, wParam, lParam);
        }
        return HRESULT.S_OK;
    }

    // Registry-change notification is inherently a Windows concept (the managed callers
    // are Configuration classes reading HKLM/HKCU).
    [UnmanagedCallersOnly(EntryPoint = "SpRegNotifyChangeKey")]
    public static HRESULT SpRegNotifyChangeKey(IntPtr hkey, char* wszPath, IntPtr callback, IntPtr* handle)
    {
        if (handle == null)
            return HRESULT.E_INVALIDARG;

        *handle = IntPtr.Zero;
#if WINDOWS
        var watcher = RegistryChangeWatcher.Create(hkey, NativeString.UniToString(wszPath), callback);
        if (watcher == null)
            return HRESULT.E_FAIL;

        *handle = HandleTable.Alloc(watcher);
        return HRESULT.S_OK;
#else
        // TODO: there is no registry on non-Windows platforms; a future settings
        // abstraction (see the ZuneDBApi registry abstraction work) should back this.
        return HRESULT.E_NOTIMPL;
#endif
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRegRevokeNotifyChangeKey")]
    public static HRESULT SpRegRevokeNotifyChangeKey(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return HRESULT.E_INVALIDARG;

        HandleTable.Free(handle);
        return HRESULT.S_OK;
    }

    // Real: invokes the supplied callback immediately on the calling thread. The
    // "deferred" in the name refers to the *caller* having deferred it to this point --
    // NativeApi.cs's own declaration takes the callback and its data with no scheduling
    // parameters, so there is nothing to schedule against here.
    [UnmanagedCallersOnly(EntryPoint = "SpCallDeferredInvokeProc")]
    public static void SpCallDeferredInvokeProc(IntPtr pfnCallback, IntPtr pvCallbackData)
    {
        if (pfnCallback != IntPtr.Zero)
            ((delegate* unmanaged<IntPtr, void>)pfnCallback)(pvCallbackData);
    }
}
