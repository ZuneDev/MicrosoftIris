// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Debug.RegistryKey
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using System;
using System.Security;

namespace Microsoft.Iris.Debug
{
    [SuppressUnmanagedCodeSecurity]
    internal class RegistryKey
    {
        private IntPtr _hkey;
        public static IntPtr HKEY_CLASSES_ROOT = new IntPtr(int.MinValue);
        public static IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
        public static IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);

        private RegistryKey(IntPtr hkey) => _hkey = hkey;

        ~RegistryKey() => Close();

        public static RegistryKey Open(IntPtr hive, string path) => OpenWorker(hive, path);

        private static RegistryKey OpenWorker(IntPtr parentKey, string path)
        {
            RegistryKey registryKey = null;
            IntPtr result;
            if (Win32Api.RegOpenKeyExW(parentKey, path, 0, 131097, out result) == 0)
                registryKey = new RegistryKey(result);
            return registryKey;
        }

        public RegistryKey OpenSubKey(string path) => OpenWorker(_hkey, path);

        public bool ReadByte(string valueName, out byte value)
        {
            bool flag = false;
            value = 0;
            int num = 0;
            if (ReadInt(valueName, out num) && num >= 0 && num <= byte.MaxValue)
            {
                value = (byte)num;
                flag = true;
            }
            return flag;
        }

        public bool ReadBool(string valueName, out bool value)
        {
            bool flag = false;
            value = false;
            int num = 0;
            if (ReadInt(valueName, out num))
            {
                value = num != 0;
                flag = true;
            }
            return flag;
        }

        public bool ReadInt(string valueName, out int value)
        {
            bool flag = false;
            value = 0;
            int lpcbData = 4;
            int lpType;
            int lpData;
            if (Win32Api.RegQueryValueExW(_hkey, valueName, IntPtr.Zero, out lpType, out lpData, ref lpcbData) == 0 && lpType == 4)
            {
                value = lpData;
                flag = true;
            }
            return flag;
        }

        public bool ReadString(string valueName, out string value)
        {
            bool flag = false;
            value = null;
            int lpcbData = 0;
            int lpType;
            if (Win32Api.RegQueryValueExW(_hkey, valueName, IntPtr.Zero, out lpType, null, ref lpcbData) == 0 && lpType == 1)
                flag = ReadStringValueWorker(valueName, lpcbData, out value);
            return flag;
        }

        private bool ReadStringValueWorker(string valueName, int dataBytes, out string value)
        {
            value = null;
            bool flag = false;
            int length = dataBytes / 2;
            char[] lpData = new char[length];
            int lpType;
            if (Win32Api.RegQueryValueExW(_hkey, valueName, IntPtr.Zero, out lpType, lpData, ref dataBytes) == 0 && lpType == 1)
            {
                if (lpData[length - 1] == char.MinValue)
                    --length;
                value = new string(lpData, 0, length);
                flag = true;
            }
            return flag;
        }

        public void Close()
        {
            Win32Api.RegCloseKey(_hkey);
            _hkey = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
