using Microsoft.Win32;

namespace Microsoft.Iris.Data.Registry
{
    /// <summary>
    /// Selects the <see cref="IRegistryProvider"/> implementation for the
    /// current platform.
    /// </summary>
    public static class RegistryProviderFactory
    {
        // TODO: the original native ZuneDBApi.dll's actual root registry path
        // is not yet recovered (needs Ghidra inspection of the string constants
        // it passes to the Win32 registry APIs). "Software\Microsoft\Zune" is
        // the fewest-predicates assumption per CLAUDE.md (standard
        // HKCU/HKLM\Software\Microsoft\<Product> convention) — logged in
        // logs/Microsoft.Zune/Configuration/RegistryAbstraction.md. Corroborated
        // by ZuneShell/ZuneUI/Shell.cs's decompiled SettingsRegistryPath
        // ("HKEY_CURRENT_USER\Software\Microsoft\Zune\Shell"), which is exactly
        // this root plus a subkey.
        private const string ZuneRootKeyPath = "Software\\Microsoft\\Zune";

        /// <summary>
        /// Opens (creating it if missing) a subkey of the Zune configuration
        /// root — <c>Software\Microsoft\Zune\&lt;subKeyPath&gt;</c> — for
        /// read/write access. Used by app-settings consumers such as
        /// <see cref="Microsoft.Zune.Configuration.CConfigurationManagedBase"/>.
        /// </summary>
        public static IRegistryProvider Create(RegistryHive hive, string subKeyPath)
        {
            string fullPath = CombineZuneRoot(subKeyPath);
#if WINDOWS
            return Win32RegistryProvider.Open(hive, fullPath, writable: true, createIfMissing: true)
                ?? throw new InvalidOperationException($"Unable to open or create registry key '{fullPath}'.");
#else
            return new InMemoryRegistryProvider();
#endif
        }

        /// <summary>
        /// Opens an arbitrary absolute registry path — not necessarily under
        /// Zune's own configuration tree — returning <see langword="null"/> if
        /// it does not exist. Never creates the key.
        /// </summary>
        public static IRegistryProvider? TryOpen(RegistryHive hive, string subKeyPath, bool writable = false)
        {
#if WINDOWS
            return Win32RegistryProvider.Open(hive, subKeyPath, writable, createIfMissing: false);
#else
            return null;
#endif
        }

        private static string CombineZuneRoot(string subKeyPath) =>
            string.IsNullOrEmpty(subKeyPath) ? ZuneRootKeyPath : ZuneRootKeyPath + "\\" + subKeyPath;
    }
}
