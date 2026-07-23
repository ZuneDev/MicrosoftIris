using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Iris.Render.Subsystems.Text;

// Resolves a TextStyle font-face name (e.g. "Segoe UI") to a LoadedFont, in priority
// order: fonts registered at runtime (SpLoadFontResource), then TrueType files discovered
// in the platform's font directories, then a first-available fallback. Results are cached
// by the requested face name so repeated measures don't re-scan or re-load.
//
// Platform font-directory discovery is OS file-path logic, not a graphics API, so it's
// gated the ordinary way (CLAUDE.md's platform rule) and degrades honestly: if nothing
// resolves, Resolve returns null and callers fall back to the ratio-based TextMetrics
// (measurement) or report E_NOTIMPL (rasterization) rather than faking output.
internal static class FontStore
{
    private static readonly object s_lock = new();
    private static readonly ConcurrentDictionary<string, LoadedFont> s_byFace = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, LoadedFont> s_registered = new(StringComparer.OrdinalIgnoreCase);

    // filename-key (normalized, no extension) -> full path; built once, lazily.
    private static Dictionary<string, string> s_systemIndex;

    // Registers font bytes under a family name (backs SpLoadFontResource). Takes priority
    // over system fonts for that family.
    public static bool Register(string family, byte[] ttf)
    {
        if (string.IsNullOrEmpty(family))
            return false;

        LoadedFont font = LoadedFont.TryLoad(family, ttf);
        if (font == null)
            return false;

        s_registered[Normalize(family)] = font;
        s_byFace[family] = font; // fast-path this exact name
        return true;
    }

    // Resolves a face name to a font, or null if none is available. `faceName` may be null
    // (the caller has no specific face), in which case the fallback font is used.
    public static LoadedFont Resolve(string faceName)
    {
        string key = faceName ?? "";
        if (s_byFace.TryGetValue(key, out LoadedFont cached))
            return cached;

        lock (s_lock)
        {
            if (s_byFace.TryGetValue(key, out cached))
                return cached;

            LoadedFont resolved = ResolveUncached(faceName);
            if (resolved != null)
                s_byFace[key] = resolved;
            return resolved;
        }
    }

    private static LoadedFont ResolveUncached(string faceName)
    {
        string norm = Normalize(faceName);

        if (!string.IsNullOrEmpty(norm) && s_registered.TryGetValue(norm, out LoadedFont reg))
            return reg;

        Dictionary<string, string> index = SystemIndex();

        // Exact normalized match first (e.g. "segoeui"), then a prefix/contains match
        // (so "Segoe UI" also finds "segoeui" / "SegoeUI-Regular").
        if (!string.IsNullOrEmpty(norm))
        {
            if (index.TryGetValue(norm, out string exact) && TryLoadFile(faceName, exact, out LoadedFont f))
                return f;

            string best = index
                .Where(kv => kv.Key.StartsWith(norm, StringComparison.Ordinal) || kv.Key.Contains(norm))
                .OrderBy(kv => kv.Key.Length) // shortest ~= the plain "Regular" variant
                .Select(kv => kv.Value)
                .FirstOrDefault();

            if (best != null && TryLoadFile(faceName, best, out LoadedFont matched))
                return matched;
        }

        // Fallback: a common sans-serif if present, else any discovered face.
        foreach (string preferred in new[] { "dejavusans", "liberationsans", "arial", "segoeui", "verdana", "notosans", "roboto" })
        {
            if (index.TryGetValue(preferred, out string path) && TryLoadFile(faceName ?? preferred, path, out LoadedFont pref))
                return pref;
        }

        foreach (string any in index.Values)
        {
            if (TryLoadFile(faceName ?? "default", any, out LoadedFont anyFont))
                return anyFont;
        }

        return null;
    }

    private static bool TryLoadFile(string faceName, string path, out LoadedFont font)
    {
        font = null;
        try
        {
            font = LoadedFont.TryLoad(faceName, File.ReadAllBytes(path));
            return font != null;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static Dictionary<string, string> SystemIndex()
    {
        if (s_systemIndex != null)
            return s_systemIndex;

        var index = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string dir in FontDirectories())
        {
            if (!Directory.Exists(dir))
                continue;

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(dir, "*.ttf", SearchOption.AllDirectories);
            }
            catch (IOException)
            {
                continue;
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }

            foreach (string file in files)
            {
                string key = Normalize(Path.GetFileNameWithoutExtension(file));
                // First writer wins, so earlier (more preferred) directories take priority.
                index.TryAdd(key, file);
            }
        }

        s_systemIndex = index;
        return index;
    }

    private static IEnumerable<string> FontDirectories()
    {
#if WINDOWS
        yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
        yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Fonts");
#else
        // Linux + macOS common locations. TODO: consult fontconfig on Linux for a
        // face-name->file mapping instead of filename heuristics, once a suitable
        // abstraction is available.
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        yield return "/usr/share/fonts";
        yield return "/usr/local/share/fonts";
        yield return Path.Combine(home, ".fonts");
        yield return Path.Combine(home, ".local", "share", "fonts");
        yield return "/System/Library/Fonts";
        yield return "/Library/Fonts";
        yield return Path.Combine(home, "Library", "Fonts");
#endif
    }

    private static string Normalize(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "";

        Span<char> buffer = name.Length <= 128 ? stackalloc char[name.Length] : new char[name.Length];
        int n = 0;
        foreach (char c in name)
        {
            if (char.IsLetterOrDigit(c))
                buffer[n++] = char.ToLowerInvariant(c);
        }
        return new string(buffer[..n]);
    }
}
