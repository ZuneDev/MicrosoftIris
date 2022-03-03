// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.Win32Api
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Iris.OS
{
    [SuppressUnmanagedCodeSecurity]
    internal static class Win32Api
    {
        public const uint GENERIC_READ = 2147483648;
        public const uint GENERIC_WRITE = 1073741824;
        public const uint FILE_SHARE_READ = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint INVALID_FILE_SIZE = 4294967295;
        public const uint FILE_ATTRIBUTE_NORMAL = 128;
        public const uint NONE = 0;
        public const uint ERROR_SUCCESS = 0;
        private const uint ERROR_FILE_NOT_FOUND = 2;
        private const uint ERROR_PATH_NOT_FOUND = 3;
        private const uint ERROR_ACCESS_DENIED = 5;
        private const uint ERROR_INVALID_HANDLE = 6;
        private const uint ERROR_NOT_ENOUGH_MEMORY = 8;
        private const uint ERROR_OUTOFMEMORY = 14;
        private const uint ERROR_NOT_SUPPORTED = 50;
        private const uint ERROR_INVALID_PARAMETER = 87;
        private const uint ERROR_CALL_NOT_IMPLEMENTED = 120;
        private const uint ERROR_MORE_DATA = 234;
        private const uint ERROR_SERVICE_ALREADY_RUNNING = 1056;
        private const uint ERROR_NOT_FOUND = 1168;
        private const uint ERROR_NO_MATCH = 1169;
        private const uint ERROR_SET_NOT_FOUND = 1170;
        private const uint ERROR_SERVICE_DATABASE_LOCKED = 1055;
        public const int KEY_READ = 131097;
        public const int REG_SZ = 1;
        public const int REG_DWORD = 4;
        public const uint WS_OVERLAPPED = 0;
        public const uint WS_POPUP = 2147483648;
        public const uint WS_CLIPSIBLINGS = 67108864;
        public const uint WS_CLIPCHILDREN = 33554432;
        public const uint WS_CAPTION = 12582912;
        public const uint WS_BORDER = 8388608;
        public const uint WS_SYSMENU = 524288;
        public const uint WS_THICKFRAME = 262144;
        public const uint WS_MINIMIZEBOX = 131072;
        public const uint WS_MAXIMIZEBOX = 65536;
        public const uint WS_EX_TOPMOST = 8;
        public const uint WS_EX_CONTROLPARENT = 65536;
        public const uint WS_EX_APPWINDOW = 262144;
        public const uint WS_EX_TOOLWINDOW = 128;
        public const uint SW_SHOWNORMAL = 1;
        public const uint SW_SHOWMINIMIZED = 2;
        public const uint SW_SHOWMAXIMIZED = 3;
        public const int KEYEVENTF_KEYUP = 2;
        public const uint APPCOMMAND_BROWSER_BACKWARD = 1;
        public const uint APPCOMMAND_VOLUME_MUTE = 8;
        public const uint APPCOMMAND_VOLUME_DOWN = 9;
        public const uint APPCOMMAND_VOLUME_UP = 10;
        public const uint APPCOMMAND_MEDIA_NEXTTRACK = 11;
        public const uint APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        public const uint APPCOMMAND_MEDIA_STOP = 13;
        public const uint APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        public const uint APPCOMMAND_MEDIA_PLAY = 46;
        public const uint APPCOMMAND_MEDIA_PAUSE = 47;
        public const uint APPCOMMAND_MEDIA_RECORD = 48;
        public const uint APPCOMMAND_MEDIA_FAST_FORWARD = 49;
        public const uint APPCOMMAND_MEDIA_REWIND = 50;
        public const uint WM_FORWARDMSG = 895;
        public const int RT_RCDATA = 10;
        public const int RT_HTML = 23;
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;
        public const int SM_CXDRAG = 68;
        public const int SM_CYDRAG = 69;
        public const int SPI_GETMENUDROPALIGNMENT = 27;
        public const uint SPI_GETKEYBOARDSPEED = 10;
        public const uint SPI_GETKEYBOARDDELAY = 22;
        public const uint SPI_GETCARETWIDTH = 8198;
        public const int INPUT_KEYBOARD = 1;
        public const int LAYOUT_LTR = 0;
        public const int LAYOUT_RTL = 1;
        public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private static readonly string[] s_rgsMessageNames = new string[0];

        static Win32Api() => InitMessageDump();

        public static void IFWIN32(bool fResult)
        {
            if (fResult)
                return;
            uint lastWin32Error = (uint)Marshal.GetLastWin32Error();
            switch (lastWin32Error)
            {
                case 0:
                    throw new Win32Exception(0, "Generic failure of Win32 function that did not provide error information");
                case 2:
                case 3:
                    throw new FileNotFoundException();
                case 5:
                    throw new UnauthorizedAccessException();
                case 6:
                case 87:
                    throw new ArgumentException();
                case 8:
                case 14:
                    throw new OutOfMemoryException();
                case 50:
                case 120:
                    throw new NotImplementedException();
                case 1168:
                case 1169:
                case 1170:
                    throw new ArgumentOutOfRangeException();
                default:
                    Marshal.ThrowExceptionForHR((int)lastWin32Error & ushort.MaxValue | 458752 | int.MinValue);
                    break;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
          string lpFileName,
          uint dwDesiredAccess,
          uint dwShareMode,
          IntPtr lpSecurityAttributes,
          uint dwCreationDisposition,
          uint dwFlagsAndAttributes,
          IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern uint GetFileSize(IntPtr hFile, IntPtr lpFileSizeHigh);

        [DllImport("kernel32.dll")]
        public static extern bool ReadFile(
          IntPtr hFile,
          IntPtr lpBuffer,
          uint nNumberOfBytesToRead,
          out uint lpNumberOfBytesRead,
          IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(
          IntPtr hFile,
          IntPtr lpBuffer,
          uint nNumberOfBytesToWrite,
          out uint lpNumberOfBytesWritten,
          IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern void OutputDebugString(string lpOutputString);

        [DllImport("kernel32.dll")]
        public static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll")]
        public static extern void DebugBreak();

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegOpenKeyExW(
          IntPtr hkey,
          string lpSubKey,
          int options,
          int securityMask,
          out IntPtr result);

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegQueryValueExW(
          IntPtr hKey,
          string lpValueName,
          IntPtr reserved,
          out int lpType,
          char[] lpData,
          ref int lpcbData);

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegQueryValueExW(
          IntPtr hKey,
          string lpValueName,
          IntPtr reserved,
          out int lpType,
          out int lpData,
          ref int lpcbData);

        [DllImport("Advapi32.dll")]
        public static extern int RegCloseKey(IntPtr hkey);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(
          IntPtr hWnd,
          uint Msg,
          IntPtr wParam,
          IntPtr lParam);

        public static string DumpMessage(uint uMsg) => DumpMessageWorker(uMsg) ?? "UNKNOWN MESSAGE";

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
            return rgsMessageName == null ? null : InvariantString.Format("{0} + {1}", rgsMessageName, (uint)((int)uMsg - (int)num));
        }

        private static void InitMessageDump()
        {
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int metric);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SystemParametersInfo(
          uint uiAction,
          uint uiParam,
          out int pParam,
          int nWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SystemParametersInfo(
          uint uiAction,
          uint uiParam,
          out bool pParam,
          int nWinIni);

        public static int GetCaretWidth()
        {
            int pParam;
            if (!SystemParametersInfo(8198U, 0U, out pParam, 0))
                pParam = 1;
            return pParam;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetCaretBlinkTime();

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, Win32Api.INPUT[] pInputs, int cbSize);

        public static int GetDefaultKeyDelay()
        {
            int pParam;
            if (!SystemParametersInfo(22U, 0U, out pParam, 0))
                pParam = 1;
            return (pParam + 1) * 250;
        }

        public static int GetDefaultKeyRepeat()
        {
            int pParam;
            if (!SystemParametersInfo(10U, 0U, out pParam, 0))
                pParam = 1;
            return 31000 / (62 + 28 * pParam);
        }

        public static bool GetMenuDropAlignment()
        {
            bool pParam;
            if (!SystemParametersInfo(27U, 0U, out pParam, 0))
                pParam = false;
            return pParam;
        }

        [DllImport("user32.dll")]
        public static extern bool GetProcessDefaultLayout(out int pdwDefaultLayout);

        [Serializable]
        public struct HANDLE
        {
            public IntPtr h;

            public static Win32Api.HANDLE NULL => new Win32Api.HANDLE()
            {
                h = IntPtr.Zero
            };

            public static bool operator ==(Win32Api.HANDLE hl, Win32Api.HANDLE hr) => hl.h == hr.h;

            public static bool operator !=(Win32Api.HANDLE hl, Win32Api.HANDLE hr) => hl.h != hr.h;

            public override bool Equals(object oCompare) => h == (IntPtr)oCompare;

            public override int GetHashCode() => (int)h.ToInt64();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LOGFONTW_STRUCT
        {
            public const int LF_FACESIZE = 32;
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            public Win32Api.LOGFONTW_STRUCT.FACENAME lfFaceName;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct FACENAME
            {
                private char name00;
                private char name01;
                private char name02;
                private char name03;
                private char name04;
                private char name05;
                private char name06;
                private char name07;
                private char name08;
                private char name09;
                private char name10;
                private char name11;
                private char name12;
                private char name13;
                private char name14;
                private char name15;
                private char name16;
                private char name17;
                private char name18;
                private char name19;
                private char name20;
                private char name21;
                private char name22;
                private char name23;
                private char name24;
                private char name25;
                private char name26;
                private char name27;
                private char name28;
                private char name29;
                private char name30;
                private char name31;
            }
        }

        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int pt_x;
            public int pt_y;

            public override string ToString() => InvariantString.Format("{0} -> {1}, wp=0x{2,0:x} lp=0x{3,0:x}", DumpMessage(message), hwnd, wParam, lParam);
        }

        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
            public uint Padding1;
            public uint Padding2;
        }

        public struct INPUT
        {
            public int type;
            public Win32Api.KEYBDINPUT data;
        }
    }
}
