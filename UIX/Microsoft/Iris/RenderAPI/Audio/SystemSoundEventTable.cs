// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.Audio.SystemSoundEventTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Debug;

namespace Microsoft.Iris.RenderAPI.Audio
{
    internal class SystemSoundEventTable
    {
        private static readonly string s_RegistryParentKey = "AppEvents\\Schemes\\Apps\\.Default";
        private Map<SystemSoundEvent, SystemSoundEventTable.SystemSound> m_systemSoundDictionary;

        public SystemSoundEventTable()
        {
            m_systemSoundDictionary = new Map<SystemSoundEvent, SystemSoundEventTable.SystemSound>();
            Add(SystemSoundEvent.Asterisk, "SystemAsterisk");
            Add(SystemSoundEvent.CloseProgram, "CloseProgram");
            Add(SystemSoundEvent.CriticalBatteryAlarm, "CriticalBatteryAlarm");
            Add(SystemSoundEvent.CriticalStop, "SystemHand");
            Add(SystemSoundEvent.DefaultBeep, ".Default");
            Add(SystemSoundEvent.DeviceConnect, "DeviceConnect");
            Add(SystemSoundEvent.DeviceDisconnect, "DeviceDisconnect");
            Add(SystemSoundEvent.DeviceFailedToConnect, "DeviceFail");
            Add(SystemSoundEvent.Exclamation, "SystemExclamation");
            Add(SystemSoundEvent.ExitWindows, "SystemExit");
            Add(SystemSoundEvent.LowBatteryAlarm, "LowBatteryAlarm");
            Add(SystemSoundEvent.Maximize, "Maximize");
            Add(SystemSoundEvent.MenuCommand, "MenuCommand");
            Add(SystemSoundEvent.MenuPopup, "MenuPopup");
            Add(SystemSoundEvent.Minimize, "Minimize");
            Add(SystemSoundEvent.NewFaxNotification, "FaxBeep");
            Add(SystemSoundEvent.NewMailNotification, "MailBeep");
            Add(SystemSoundEvent.OpenProgram, "Open");
            Add(SystemSoundEvent.PrintComplete, "PrintComplete");
            Add(SystemSoundEvent.ProgramError, "AppGPFault");
            Add(SystemSoundEvent.Question, "SystemQuestion");
            Add(SystemSoundEvent.RestoreDown, "RestoreDown");
            Add(SystemSoundEvent.RestoreUp, "RestoreUp");
            Add(SystemSoundEvent.Select, "CCSelect");
            Add(SystemSoundEvent.ShowToolbarBand, "ShowBand");
            Add(SystemSoundEvent.StartWindows, "SystemStart");
            Add(SystemSoundEvent.SystemNotification, "SystemNotification");
            Add(SystemSoundEvent.WindowsLogoff, "WindowsLogoff");
            Add(SystemSoundEvent.WindowsLogon, "WindowsLogon");
            Refresh();
        }

        public void Refresh()
        {
            RegistryKey registryKey1 = RegistryKey.Open(RegistryKey.HKEY_CURRENT_USER, s_RegistryParentKey);
            if (registryKey1 == null)
                return;
            foreach (SystemSoundEventTable.SystemSound systemSound in m_systemSoundDictionary.Values)
            {
                RegistryKey registryKey2 = registryKey1.OpenSubKey(systemSound.RegistrySubKey + "\\.Current");
                if (registryKey2 != null)
                {
                    registryKey2.ReadString(null, out systemSound.FilePath);
                    registryKey2.Close();
                }
            }
            registryKey1.Close();
        }

        public string GetFilePath(SystemSoundEvent systemSoundEvent) => m_systemSoundDictionary[systemSoundEvent].FilePath;

        private void Add(SystemSoundEvent systemSoundEvent, string registrySubKey) => m_systemSoundDictionary.Add(systemSoundEvent, new SystemSoundEventTable.SystemSound()
        {
            Event = systemSoundEvent,
            RegistrySubKey = registrySubKey
        });

        internal class SystemSound
        {
            public SystemSoundEvent Event;
            public string RegistrySubKey;
            public string FilePath;
        }
    }
}
