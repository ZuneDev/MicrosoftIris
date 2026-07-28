using System;
using System.Collections.Generic;

namespace Microsoft.Iris.Data.Registry
{
    /// <summary>
    /// Abstraction over the named-value store backing a single
    /// <see cref="CConfigurationManagedBase"/> instance. Decouples configuration
    /// classes from any specific persistence mechanism (Windows registry,
    /// in-memory, etc), so the same property-getter/setter logic works on every
    /// platform.
    /// </summary>
    public interface IRegistryProvider : IDisposable
    {
        bool GetBoolValue(string valueName, bool defaultValue);
        void SetBoolValue(string valueName, bool value);

        int GetIntValue(string valueName, int defaultValue);
        void SetIntValue(string valueName, int value);

        long GetInt64Value(string valueName, long defaultValue);
        void SetInt64Value(string valueName, long value);

        double GetDoubleValue(string valueName, double defaultValue);
        void SetDoubleValue(string valueName, double value);

        DateTime GetDateTimeValue(string valueName, DateTime defaultValue);
        void SetDateTimeValue(string valueName, DateTime value);

        string? GetStringValue(string valueName, string? defaultValue);
        void SetStringValue(string valueName, string value);

        IList<string>? GetStringListValue(string valueName);
        void SetStringListValue(string valueName, IList<string> value);

        byte[]? GetBinaryValue(string valueName);
        void SetBinaryValue(string valueName, byte[] value);

        IEnumerable<string> GetSubKeyNames();

        /// <summary>
        /// Opens a direct child of this key, or returns <see langword="null"/>
        /// if it does not exist (never creates one).
        /// </summary>
        IRegistryProvider? OpenSubKey(string name, bool writable = false);
    }
}
