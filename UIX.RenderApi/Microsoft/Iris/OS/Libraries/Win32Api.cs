// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.Libraries.Win32Api
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Iris.OS.Libraries
{
    [SuppressUnmanagedCodeSecurity]
    internal sealed class Win32Api
    {
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
        public const uint ERROR_MORE_DATA = 234;
        public const uint ERROR_SERVICE_ALREADY_RUNNING = 1056;
        public const uint ERROR_NOT_FOUND = 1168;
        public const uint ERROR_NO_MATCH = 1169;
        public const uint ERROR_SET_NOT_FOUND = 1170;
        public const uint ERROR_SERVICE_DATABASE_LOCKED = 1055;
        public const int KEY_READ = 131097;
        public const int REG_SZ = 1;
        public const int REG_DWORD = 4;

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
    }
}
