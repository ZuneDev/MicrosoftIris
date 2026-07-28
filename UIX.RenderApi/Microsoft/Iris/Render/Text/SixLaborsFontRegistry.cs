using SixLabors.Fonts;

namespace Microsoft.Iris.Render.Text;

// Shared font collection fonts get registered into (by SixLaborsFontResource)
// and resolved from (by SixLaborsTextDocument), analogous to how the Windows
// backend registers fonts process-wide via AddFontMemResourceEx/SpLoadFontResource.
internal static class SixLaborsFontRegistry
{
    public static FontCollection Collection { get; } = new();

    public static bool TryGetFamily(string name, out FontFamily family)
    {
        if (!string.IsNullOrEmpty(name) && Collection.TryGet(name, out family))
            return true;

        family = default;
        return false;
    }
}
