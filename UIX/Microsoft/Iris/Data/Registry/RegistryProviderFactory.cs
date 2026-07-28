using System;
using Microsoft.Win32;

namespace Microsoft.Iris.Data.Registry
{
    /// <summary>
    /// Selects the <see cref="IRegistryProvider"/> implementation for the
    /// current platform. Purely a hive/path-based key opener — callers that
    /// want a product-specific root (e.g. Zune's <c>Software\Microsoft\Zune</c>
    /// convention) should layer that prefixing on top of this, rather than
    /// have it live here, so this stays reusable by any consumer of the
    /// abstraction (both <c>ZuneDBApi</c> and <c>UIXControls</c>).
    /// </summary>
    public static class RegistryProviderFactory
    {
        /// <summary>
        /// Opens (creating it if missing) <paramref name="subKeyPath"/> under
        /// <paramref name="hive"/> for read/write access.
        /// </summary>
        public static IRegistryProvider Open(RegistryHive hive, string subKeyPath, bool writable = true)
        {
#if WINDOWS
            return Win32RegistryProvider.Open(hive, subKeyPath, writable, createIfMissing: true)
                ?? throw new InvalidOperationException($"Unable to open or create registry key '{subKeyPath}'.");
#else
            return InMemoryRegistryProvider.GetOrCreate(hive, subKeyPath);
#endif
        }

        /// <summary>
        /// Opens <paramref name="subKeyPath"/> under <paramref name="hive"/>,
        /// returning <see langword="null"/> if it does not exist. Never
        /// creates the key.
        /// </summary>
        public static IRegistryProvider? TryOpen(RegistryHive hive, string subKeyPath, bool writable = false)
        {
#if WINDOWS
            return Win32RegistryProvider.Open(hive, subKeyPath, writable, createIfMissing: false);
#else
            return InMemoryRegistryProvider.TryGet(hive, subKeyPath);
#endif
        }

        /// <summary>
        /// Opens a key given a single absolute path string of the same form
        /// accepted by <see cref="Registry.GetValue(string, string, object)"/>
        /// / <see cref="Registry.SetValue(string, string, object)"/> — a hive
        /// name (e.g. <c>HKEY_CURRENT_USER</c>) followed by the subkey path.
        /// Lets callers that only ever deal with one flat path string (like
        /// <c>UIXControls.RegistryHelper</c>) use this abstraction without
        /// having to track a separate hive/subkey pair themselves.
        /// </summary>
        public static IRegistryProvider? TryOpen(string fullPath, bool writable = false, bool createIfMissing = false)
        {
            (RegistryHive hive, string subKeyPath) = ParsePath(fullPath);
            return createIfMissing ? Open(hive, subKeyPath, writable) : TryOpen(hive, subKeyPath, writable);
        }

        private static readonly (string Prefix, RegistryHive Hive)[] s_hivePrefixes =
        {
            ("HKEY_CURRENT_USER", RegistryHive.CurrentUser),
            ("HKEY_LOCAL_MACHINE", RegistryHive.LocalMachine),
            ("HKEY_CLASSES_ROOT", RegistryHive.ClassesRoot),
            ("HKEY_USERS", RegistryHive.Users),
            ("HKEY_CURRENT_CONFIG", RegistryHive.CurrentConfig),
            ("HKEY_PERFORMANCE_DATA", RegistryHive.PerformanceData),
        };

        private static (RegistryHive Hive, string SubKeyPath) ParsePath(string fullPath)
        {
            foreach ((string prefix, RegistryHive hive) in s_hivePrefixes)
            {
                if (fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return (hive, fullPath[prefix.Length..].TrimStart('\\'));
            }

            throw new ArgumentException(
                $"'{fullPath}' does not start with a valid registry hive name (e.g. HKEY_CURRENT_USER).",
                nameof(fullPath));
        }
    }
}
