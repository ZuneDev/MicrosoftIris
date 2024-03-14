using Microsoft.Iris;
using System.Runtime.InteropServices;

namespace IrisShell.UI;

public static class Win32MessageBox
{
    public static void Show(
      string text,
      string caption,
      Win32MessageBoxType type,
      DeferredInvokeHandler callback)
    {
        IntPtr winHandle = Application.Window.Handle;
        new Thread(args =>
        {
            int num = MessageBox(winHandle, text, caption, type);
            if (callback == null)
                return;
            var returnValue = (Win32MessageBoxReturnValue)num;
            Application.DeferredInvoke(callback, num);
        }).Start();
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int MessageBox(
      IntPtr hWnd,
      string text,
      string caption,
      Win32MessageBoxType type);
}

public enum Win32MessageBoxReturnValue
{
    OK = 1,
    CANCEL = 2,
    ABORT = 3,
    RETRY = 4,
    IGNORE = 5,
    YES = 6,
    NO = 7,
}

public enum Win32MessageBoxType
{
    MB_APPLMODAL = 0,
    MB_OK = 0,
    MB_OKCANCEL = 1,
    MB_ABORTRETRYIGNORE = 2,
    MB_YESNOCANCEL = 3,
    MB_YESNO = 4,
    MB_RETRYCANCEL = 5,
    MB_CANCELTRYCONTINUE = 6,
    MB_TYPEMASK = 15, // 0x0000000F
    MB_ICONERROR = 16, // 0x00000010
    MB_ICONHAND = 16, // 0x00000010
    MB_ICONSTOP = 16, // 0x00000010
    MB_ICONQUESTION = 32, // 0x00000020
    MB_ICONEXCLAMATION = 48, // 0x00000030
    MB_ICONWARNING = 48, // 0x00000030
    MB_ICONASTERISK = 64, // 0x00000040
    MB_ICONINFORMATION = 64, // 0x00000040
    MB_USERICON = 128, // 0x00000080
    MB_ICONMASK = 240, // 0x000000F0
    MB_DEFMASK = 3840, // 0x00000F00
    MB_SYSTEMMODAL = 4096, // 0x00001000
    MB_TASKMODAL = 8192, // 0x00002000
    MB_MODEMASK = 12288, // 0x00003000
    MB_HELP = 16384, // 0x00004000
    MB_NOFOCUS = 32768, // 0x00008000
    MB_MISCMASK = 49152, // 0x0000C000
    MB_SETFOREGROUND = 65536, // 0x00010000
    MB_DEFAULT_DESKTOP_ONLY = 131072, // 0x00020000
    MB_TOPMOST = 262144, // 0x00040000
    MB_RIGHT = 524288, // 0x00080000
    MB_RTLREADING = 1048576, // 0x00100000
    MB_SERVICE_NOTIFICATION = 2097152, // 0x00200000
}
