using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Iris.Data.Registry
{
    // TODO: back this with a real persistent store (e.g. a JSON/INI file under
    // XDG_CONFIG_HOME) once non-Windows platform support is prioritized; for now
    // values only live for the lifetime of the process, matching the previous
    // no-op stub behavior except that values round-trip within a single run.
    internal sealed class InMemoryRegistryProvider : IRegistryProvider
    {
        private readonly Dictionary<string, object> m_values = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, InMemoryRegistryProvider> m_subKeys = new(StringComparer.OrdinalIgnoreCase);

        public bool GetBoolValue(string valueName, bool defaultValue) => Get(valueName, defaultValue);
        public void SetBoolValue(string valueName, bool value) => Set(valueName, value);

        public int GetIntValue(string valueName, int defaultValue) => Get(valueName, defaultValue);
        public void SetIntValue(string valueName, int value) => Set(valueName, value);

        public long GetInt64Value(string valueName, long defaultValue) => Get(valueName, defaultValue);
        public void SetInt64Value(string valueName, long value) => Set(valueName, value);

        public double GetDoubleValue(string valueName, double defaultValue) => Get(valueName, defaultValue);
        public void SetDoubleValue(string valueName, double value) => Set(valueName, value);

        public DateTime GetDateTimeValue(string valueName, DateTime defaultValue) => Get(valueName, defaultValue);
        public void SetDateTimeValue(string valueName, DateTime value) => Set(valueName, value);

        public string? GetStringValue(string valueName, string? defaultValue) => Get(valueName, defaultValue);
        public void SetStringValue(string valueName, string value) => Set(valueName, value);

        public IList<string>? GetStringListValue(string valueName) => Get<IList<string>?>(valueName, null);
        public void SetStringListValue(string valueName, IList<string> value) => Set(valueName, value);

        public byte[]? GetBinaryValue(string valueName) => Get<byte[]?>(valueName, null);
        public void SetBinaryValue(string valueName, byte[] value) => Set(valueName, value);

        public IEnumerable<string> GetSubKeyNames()
        {
            lock (m_subKeys)
                return m_subKeys.Keys.ToList();
        }

        // Auto-vivifies, unlike the real registry: this store has no concept of
        // key existence independent of a value being set, so "open" and
        // "create" collapse into the same operation here.
        public IRegistryProvider OpenSubKey(string name, bool writable = false)
        {
            lock (m_subKeys)
            {
                if (!m_subKeys.TryGetValue(name, out InMemoryRegistryProvider? sub))
                    m_subKeys[name] = sub = new InMemoryRegistryProvider();
                return sub;
            }
        }

        private T Get<T>(string valueName, T defaultValue)
        {
            lock (m_values)
                return m_values.TryGetValue(valueName, out object? stored) && stored is T typed ? typed : defaultValue;
        }

        private void Set(string valueName, object value)
        {
            lock (m_values)
                m_values[valueName] = value;
        }

        public void Dispose() { }
    }
}
