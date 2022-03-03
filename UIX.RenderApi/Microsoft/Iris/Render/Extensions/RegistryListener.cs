// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.RegistryListener
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Libraries.OS;
using Microsoft.Iris.Render.Internal;
using System;
using System.Globalization;
using System.Security;

namespace Microsoft.Iris.Render.Extensions
{
    [SuppressUnmanagedCodeSecurity]
    internal class RegistryListener : IDisposable
    {
        private Win32Api.WaitOrTimerCallBack m_RegistryChangeCallback;
        private IntPtr m_hWaitHandle = IntPtr.Zero;
        private Win32Api.HANDLE m_hRegEvent = Win32Api.HANDLE.NULL;
        private IntPtr m_hkey = IntPtr.Zero;
        private string m_stRegKey;

        public RegistryListener(string stKey)
        {
            this.m_stRegKey = stKey;
            this.m_RegistryChangeCallback = new Win32Api.WaitOrTimerCallBack(this.OnRegistryChanged);
        }

        ~RegistryListener()
        {
        }

        void IDisposable.Dispose() => this.StopListeningForRegistryChangeNotifications();

        public bool ReadValueFromRegistry(string st, out int nValue)
        {
            nValue = 0;
            RegistryKey registryKey = RegistryKey.Open(RegistryKey.HKEY_CURRENT_USER, this.m_stRegKey);
            if (registryKey == null)
                return false;
            bool flag = registryKey.ReadInt(st, out nValue);
            registryKey.Close();
            return flag;
        }

        public bool ReadValueFromRegistry(string st, out byte bValue)
        {
            int nValue;
            bool flag = this.ReadValueFromRegistry(st, out nValue);
            bValue = (byte)nValue;
            return flag;
        }

        public bool ReadValueFromRegistry(string st, out bool fValue)
        {
            int nValue;
            bool flag = this.ReadValueFromRegistry(st, out nValue);
            fValue = nValue != 0;
            return flag;
        }

        internal void StartListeningForRegistryChanges()
        {
            lock (this)
            {
                this.StopListeningForRegistryChangeNotifications();
                this.m_hkey = IntPtr.Zero;
                if (Win32Api.RegOpenKeyExW(Win32Api.HKEY_CURRENT_USER, this.m_stRegKey, 0, 16, out this.m_hkey) != 0)
                    return;
                this.m_hRegEvent = Win32Api.CreateEvent(IntPtr.Zero, false, false, null);
                int num = Win32Api.RegNotifyChangeKeyValue(this.m_hkey, true, 15, this.m_hRegEvent, true);
                if (num != 0)
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "Unable to register key change handler: {0} (due to error {1})", m_stRegKey, num));
                IntPtr phNewWaitObject;
                if (Win32Api.RegisterWaitForSingleObject(out phNewWaitObject, this.m_hRegEvent.h, this.m_RegistryChangeCallback, IntPtr.Zero, uint.MaxValue, 4U) == 0)
                {
                    this.StopListeningForRegistryChangeNotifications();
                    throw new Exception("Unable to register wait for object.");
                }
                this.m_hWaitHandle = phNewWaitObject;
            }
        }

        private void StopListeningForRegistryChangeNotifications()
        {
            lock (this)
            {
                if (this.m_hWaitHandle != IntPtr.Zero)
                {
                    Win32Api.UnregisterWait(this.m_hWaitHandle);
                    this.m_hWaitHandle = IntPtr.Zero;
                }
                if (this.m_hRegEvent != Win32Api.HANDLE.NULL)
                {
                    Win32Api.CloseHandle(this.m_hRegEvent);
                    this.m_hRegEvent = Win32Api.HANDLE.NULL;
                }
                if (!(this.m_hkey != IntPtr.Zero))
                    return;
                Win32Api.RegCloseKey(this.m_hkey);
                this.m_hkey = IntPtr.Zero;
            }
        }

        public event EventHandler RegistryChanged;

        private void OnRegistryChanged(IntPtr ptrParameter, byte b)
        {
            this.StopListeningForRegistryChangeNotifications();
            if (this.RegistryChanged == null)
                return;
            this.RegistryChanged(this, EventArgs.Empty);
        }
    }
}
