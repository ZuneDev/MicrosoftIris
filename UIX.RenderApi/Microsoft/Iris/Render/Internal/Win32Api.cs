// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.Win32Api
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Iris.Render.Internal
{
    [SuppressUnmanagedCodeSecurity]
    internal static class Win32Api
    {
        public const int IDC_ARROW = 32512;
        public const int IDC_IBEAM = 32513;
        public const int IDC_WAIT = 32514;
        public const int IDC_CROSS = 32515;
        public const int IDC_UPARROW = 32516;
        public const int IDC_SIZENWSE = 32642;
        public const int IDC_SIZENESW = 32643;
        public const int IDC_SIZEWE = 32644;
        public const int IDC_SIZENS = 32645;
        public const int IDC_SIZEALL = 32646;
        public const int IDC_NO = 32648;
        public const int IDC_HAND = 32649;
        public const int IDC_APPSTARTING = 32550;
        public const int IDC_HELP = 32651;
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;
        public const int RT_RCDATA = 10;
        public const uint EVENT_MODIFY_STATE = 2;
        public const uint LOAD_LIBRARY_AS_DATAFILE = 2;
        public const int REG_NOTIFY_CHANGE_NAME = 1;
        public const int REG_NOTIFY_CHANGE_ATTRIBUTES = 2;
        public const int REG_NOTIFY_CHANGE_LAST_SET = 4;
        public const int REG_NOTIFY_CHANGE_SECURITY = 8;
        public const int KEY_NOTIFY = 16;
        public const int KEY_READ = 131097;
        public const int REG_SZ = 1;
        public const int REG_DWORD = 4;
        public const uint STATUS_ABANDONED_WAIT_0 = 128;
        public const uint WAIT_ABANDONED = 128;
        public const uint WAIT_TIMEOUT = 258;
        public const uint WAIT_OBJECT_0 = 0;
        public const uint INFINITE = 4294967295;
        public const uint ERROR_SUCCESS = 0;
        public const uint ERROR_FILE_NOT_FOUND = 2;
        public const uint ERROR_PATH_NOT_FOUND = 3;
        public const uint ERROR_ACCESS_DENIED = 5;
        public const uint ERROR_INVALID_HANDLE = 6;
        public const uint ERROR_NOT_ENOUGH_MEMORY = 8;
        public const uint ERROR_OUTOFMEMORY = 14;
        public const uint ERROR_NOT_SUPPORTED = 50;
        public const uint ERROR_INVALID_PARAMETER = 87;
        public const uint ERROR_CALL_NOT_IMPLEMENTED = 120;
        public const uint ERROR_NOT_FOUND = 1168;
        public const uint ERROR_NO_MATCH = 1169;
        public const uint ERROR_SET_NOT_FOUND = 1170;
        public const uint PM_REMOVE = 1;
        public const uint WM_QUIT = 18;
        public const uint WTS_CONSOLE_CONNECT = 1;
        public const uint WTS_CONSOLE_DISCONNECT = 2;
        public const uint WTS_REMOTE_CONNECT = 3;
        public const uint WTS_REMOTE_DISCONNECT = 4;
        public const uint WTS_SESSION_LOGON = 5;
        public const uint WTS_SESSION_LOGOFF = 6;
        public const uint WTS_SESSION_LOCK = 7;
        public const uint WTS_SESSION_UNLOCK = 8;
        public static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
        public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private static readonly string[] s_rgsMessageNames = new string[0];

        public static Win32Api.COLORREF ARGB(byte a, byte r, byte g, byte b) => new Win32Api.COLORREF((uint)(a << 24 | b << 16 | g << 8) | r);

        public static Win32Api.COLORREF RGB(byte r, byte g, byte b) => new Win32Api.COLORREF((uint)(b << 16 | g << 8) | r);

        public static Win32Api.COLORREF RGB(int r, int g, int b) => RGB((byte)r, (byte)g, (byte)b);

        public static int GetRValue(Win32Api.COLORREF cr) => (int)cr.cr & byte.MaxValue;

        public static int GetGValue(Win32Api.COLORREF cr) => (int)((cr.cr & 65280U) >> 8);

        public static int GetBValue(Win32Api.COLORREF cr) => (int)((cr.cr & 16711680U) >> 16);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool TranslateMessage(ref Win32Api.MSG msg);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DispatchMessage(ref Win32Api.MSG msg);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextW", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(HWND hWnd, StringBuilder text, int nMax);

        [DllImport("user32.dll", EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(HWND hWnd, StringBuilder text, int nMax);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(HWND hwnd, [In, Out] ref Win32Api.POINT pt);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(HWND hwnd, [In, Out] ref Win32Api.POINT pt);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int metric);

        [DllImport("kernel32.dll")]
        internal static extern Win32Api.HANDLE CreateEvent(
          IntPtr lpSecurityAttributes,
          bool bManualReset,
          bool bInitialState,
          string lpName);

        [DllImport("kernel32.dll")]
        internal static extern bool CloseHandle(Win32Api.HANDLE hObject);

        [DllImport("kernel32.dll")]
        public static extern void OutputDebugString(string lpOutputString);

        [DllImport("kernel32.dll")]
        public static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll")]
        public static extern void DebugBreak();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern Win32Api.HINSTANCE LoadLibraryEx(
          string stModuleName,
          Win32Api.HANDLE hFile,
          uint dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern bool FreeLibrary(Win32Api.HINSTANCE hinst);

        [DllImport("kernel32.dll", EntryPoint = "FindResourceW", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindResource(IntPtr hinst, string resource, IntPtr type);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadResource(IntPtr hinst, IntPtr i);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr i);

        [DllImport("kernel32.dll")]
        public static extern int SizeofResource(IntPtr hinst, IntPtr i);

        [DllImport("advapi32.dll")]
        internal static extern int RegNotifyChangeKeyValue(
          IntPtr hkey,
          bool bWatchSubtree,
          int dwNotifyFilter,
          Win32Api.HANDLE hEvent,
          bool fAsynchronous);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int RegOpenKeyExW(
          IntPtr hkey,
          string szSubKey,
          int dwOptions,
          int samDesired,
          out IntPtr phkResult);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegQueryValueExW(
          IntPtr hKey,
          string lpValueName,
          IntPtr reserved,
          out int lpType,
          out int lpData,
          ref int lpcbData);

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegQueryValueExW(
          IntPtr hKey,
          string lpValueName,
          IntPtr reserved,
          out int lpType,
          char[] lpData,
          ref int lpcbData);

        [DllImport("advapi32.dll")]
        internal static extern int RegCloseKey(IntPtr hkey);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out Win32Api.SYSTEM_INFO info);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWaitForSingleObject(
          out IntPtr phNewWaitObject,
          IntPtr hObject,
          Win32Api.WaitOrTimerCallBack Callback,
          IntPtr Context,
          uint dwMilliseconds,
          uint dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnregisterWait(IntPtr hWaitHandle);

        internal static uint GetSystemPageSize()
        {
            Win32Api.SYSTEM_INFO info;
            GetSystemInfo(out info);
            return info.dwPageSize;
        }

        internal static string DumpMessage(uint uMsg) => DumpMessageWorker(uMsg) ?? "UNKNOWN MESSAGE";

        private static string DumpMessageWorker(uint uMsg)
        {
            InitMessageDump();
            if (uMsg >= s_rgsMessageNames.Length)
                return null;
            string rgsMessageName = s_rgsMessageNames[uMsg];
            if (rgsMessageName != null)
                return rgsMessageName;
            uint num = uMsg;
            while (uMsg > 0U)
            {
                --num;
                rgsMessageName = s_rgsMessageNames[num];
                if (rgsMessageName != null)
                    break;
            }
            if (rgsMessageName == null)
                return null;
            return string.Format(CultureInfo.InvariantCulture, "{0} + {1}", rgsMessageName, (uint)((int)uMsg - (int)num));
        }

        private static void InitMessageDump()
        {
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class CRITICAL_SECTION
        {
            public IntPtr DebugInfo;
            public long LockCount;
            public long RecursionCount;
            public int OwningThread;
            public int LockSemaphore;
            public IntPtr SpinCount;
        }

        [Serializable]
        internal struct HANDLE
        {
            public IntPtr h;

            public bool IsValid => this.h != INVALID_HANDLE_VALUE;

            public static Win32Api.HANDLE NULL => new Win32Api.HANDLE()
            {
                h = IntPtr.Zero
            };

            public static Win32Api.HANDLE INVALID => new Win32Api.HANDLE()
            {
                h = INVALID_HANDLE_VALUE
            };

            public static bool operator ==(Win32Api.HANDLE hl, Win32Api.HANDLE hr) => hl.h == hr.h;

            public static bool operator !=(Win32Api.HANDLE hl, Win32Api.HANDLE hr) => hl.h != hr.h;

            public override bool Equals(object oCompare) => this.h == (IntPtr)oCompare;

            public override int GetHashCode() => (int)this.h.ToInt64();
        }

        internal struct HINSTANCE
        {
            public IntPtr h;

            public static Win32Api.HINSTANCE NULL => new Win32Api.HINSTANCE()
            {
                h = IntPtr.Zero
            };

            public static bool operator ==(Win32Api.HINSTANCE hl, Win32Api.HINSTANCE hr) => hl.h == hr.h;

            public static bool operator !=(Win32Api.HINSTANCE hl, Win32Api.HINSTANCE hr) => hl.h != hr.h;

            public override bool Equals(object oCompare) => this.h == (IntPtr)oCompare;

            public override int GetHashCode() => (int)this.h.ToInt64();
        }

        internal struct MSG
        {
            public HWND hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int pt_x;
            public int pt_y;
        }

        [ComVisible(false)]
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        public struct SIZE
        {
            public int cx;
            public int cy;

            public SIZE(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [Serializable]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [Serializable]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(Win32Api.RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            public int Width
            {
                get => this.right - this.left;
                set => this.right = this.left + value;
            }

            public int Height
            {
                get => this.bottom - this.top;
                set => this.bottom = this.top + value;
            }

            public static Win32Api.RECT FromXYWH(int x, int y, int width, int height) => new Win32Api.RECT(x, y, x + width, y + height);
        }

        public struct COLORREF
        {
            public uint cr;

            public COLORREF(uint cr) => this.cr = cr;

            public COLORREF(Win32Api.COLORREF cr) => this.cr = cr.cr;
        }

        public delegate void WaitOrTimerCallBack(IntPtr lpParameter, byte f);
    }
}
