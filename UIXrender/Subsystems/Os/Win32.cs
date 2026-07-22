#if WINDOWS
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Iris.Render.Subsystems.Os;

// Real Win32 entry points used by the platform-gated branches of SystemApi. These are OS
// APIs (DPI, system metrics, registry notification), not graphics APIs -- see
// logs/UIXrender/FullSurface.md, decision 2.
internal static partial class Win32
{
    [LibraryImport("user32.dll")]
    public static partial uint GetDpiForSystem();

    [LibraryImport("user32.dll")]
    public static partial int GetSystemMetrics(int nIndex);

    [LibraryImport("advapi32.dll", EntryPoint = "RegOpenKeyExW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial int RegOpenKeyEx(IntPtr hKey, string subKey, uint options, uint samDesired, out IntPtr result);

    [LibraryImport("advapi32.dll")]
    public static partial int RegCloseKey(IntPtr hKey);

    [LibraryImport("advapi32.dll", EntryPoint = "RegNotifyChangeKeyValue")]
    public static partial int RegNotifyChangeKeyValue(IntPtr hKey, [MarshalAs(UnmanagedType.Bool)] bool watchSubtree, uint notifyFilter, IntPtr hEvent, [MarshalAs(UnmanagedType.Bool)] bool asynchronous);
}

// Backs SpRegNotifyChangeKey: opens the requested subkey, then parks a background thread
// on a real RegNotifyChangeKeyValue event, re-arming after each notification (the Win32
// API is one-shot) and invoking the caller's RegChangeCallback each time.
internal sealed unsafe class RegistryChangeWatcher : IDisposable
{
    private const uint KEY_NOTIFY = 0x0010;
    private const uint REG_NOTIFY_CHANGE_NAME = 0x00000001;
    private const uint REG_NOTIFY_CHANGE_LAST_SET = 0x00000004;

    private readonly IntPtr _key;
    private readonly IntPtr _callback;
    private readonly ManualResetEventSlim _stopping = new(false);
    private readonly AutoResetEvent _changed = new(false);
    private readonly Thread _thread;

    private RegistryChangeWatcher(IntPtr key, IntPtr callback)
    {
        _key = key;
        _callback = callback;
        _thread = new Thread(Run) { IsBackground = true, Name = "UIXrender.RegistryChangeWatcher" };
        _thread.Start();
    }

    public static RegistryChangeWatcher Create(IntPtr hkey, string path, IntPtr callback)
    {
        if (Win32.RegOpenKeyEx(hkey, path ?? string.Empty, 0, KEY_NOTIFY, out IntPtr key) != 0)
            return null;

        return new RegistryChangeWatcher(key, callback);
    }

    private void Run()
    {
        WaitHandle[] handles = [_changed, _stopping.WaitHandle];

        while (!_stopping.IsSet)
        {
            if (Win32.RegNotifyChangeKeyValue(_key, watchSubtree: true, REG_NOTIFY_CHANGE_NAME | REG_NOTIFY_CHANGE_LAST_SET, _changed.SafeWaitHandle.DangerousGetHandle(), asynchronous: true) != 0)
                return;

            if (WaitHandle.WaitAny(handles) != 0 || _stopping.IsSet)
                return;

            if (_callback != IntPtr.Zero)
                ((delegate* unmanaged<void>)_callback)();
        }
    }

    public void Dispose()
    {
        _stopping.Set();
        _thread.Join(TimeSpan.FromSeconds(1));
        Win32.RegCloseKey(_key);
        _changed.Dispose();
        _stopping.Dispose();
    }
}
#endif
