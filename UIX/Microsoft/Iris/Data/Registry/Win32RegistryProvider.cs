#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace Microsoft.Iris.Data.Registry
{
    /// <summary>
    /// Real <see cref="IRegistryProvider"/> implementation that passes through
    /// to the Windows registry.
    /// </summary>
    internal sealed class Win32RegistryProvider : IRegistryProvider
    {
        private readonly RegistryKey m_key;

        private Win32RegistryProvider(RegistryKey key)
        {
            m_key = key;
        }

        /// <summary>
        /// Opens (or, if <paramref name="createIfMissing"/>, creates) <paramref name="subKeyPath"/>
        /// under <paramref name="hive"/>. Returns <see langword="null"/> if it
        /// doesn't exist and <paramref name="createIfMissing"/> is <see langword="false"/>.
        /// </summary>
        internal static Win32RegistryProvider? Open(RegistryHive hive, string subKeyPath, bool writable, bool createIfMissing)
        {
            using RegistryKey root = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
            RegistryKey? key = createIfMissing
                ? root.CreateSubKey(subKeyPath, writable: true)
                : root.OpenSubKey(subKeyPath, writable);
            return key is null ? null : new Win32RegistryProvider(key);
        }

        public bool GetBoolValue(string valueName, bool defaultValue) =>
            GetIntValue(valueName, defaultValue ? 1 : 0) != 0;

        public void SetBoolValue(string valueName, bool value) =>
            SetIntValue(valueName, value ? 1 : 0);

        public int GetIntValue(string valueName, int defaultValue) =>
            m_key.GetValue(valueName, defaultValue) is int i ? i : defaultValue;

        public void SetIntValue(string valueName, int value) =>
            m_key.SetValue(valueName, value, RegistryValueKind.DWord);

        public long GetInt64Value(string valueName, long defaultValue) =>
            m_key.GetValue(valueName, defaultValue) is long l ? l : defaultValue;

        public void SetInt64Value(string valueName, long value) =>
            m_key.SetValue(valueName, value, RegistryValueKind.QWord);

        // The registry has no native floating-point type; round-trip the bit
        // pattern through a QWord rather than a culture-sensitive string.
        public double GetDoubleValue(string valueName, double defaultValue) =>
            m_key.GetValue(valueName, null) is long bits ? BitConverter.Int64BitsToDouble(bits) : defaultValue;

        public void SetDoubleValue(string valueName, double value) =>
            m_key.SetValue(valueName, BitConverter.DoubleToInt64Bits(value), RegistryValueKind.QWord);

        // Same reasoning as GetDoubleValue: store the round-trippable binary
        // form (DateTime.ToBinary/FromBinary preserves Kind) as a QWord.
        public DateTime GetDateTimeValue(string valueName, DateTime defaultValue) =>
            m_key.GetValue(valueName, null) is long ticks ? DateTime.FromBinary(ticks) : defaultValue;

        public void SetDateTimeValue(string valueName, DateTime value) =>
            m_key.SetValue(valueName, value.ToBinary(), RegistryValueKind.QWord);

        public string? GetStringValue(string valueName, string? defaultValue) =>
            m_key.GetValue(valueName, defaultValue) as string ?? defaultValue;

        public void SetStringValue(string valueName, string value) =>
            m_key.SetValue(valueName, value, RegistryValueKind.String);

        public IList<string>? GetStringListValue(string valueName) =>
            m_key.GetValue(valueName) is string[] arr ? arr : null;

        public void SetStringListValue(string valueName, IList<string> value) =>
            m_key.SetValue(valueName, value.ToArray(), RegistryValueKind.MultiString);

        public byte[]? GetBinaryValue(string valueName) =>
            m_key.GetValue(valueName) as byte[];

        public void SetBinaryValue(string valueName, byte[] value) =>
            m_key.SetValue(valueName, value, RegistryValueKind.Binary);

        public IEnumerable<string> GetSubKeyNames() => m_key.GetSubKeyNames();

        public IRegistryProvider? OpenSubKey(string name, bool writable = false)
        {
            RegistryKey? sub = m_key.OpenSubKey(name, writable);
            return sub is null ? null : new Win32RegistryProvider(sub);
        }

        public void Dispose() => m_key.Dispose();
    }
}
#endif
