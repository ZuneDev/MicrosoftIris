using System;
using Microsoft.Iris.Render.Interop.Drawing;

namespace Microsoft.Iris.Render.Subsystems.Text;

// Text measurement for SpRichTextMeasure/SpSimpleTextMeasure/SpRichTextGetNaturalBounds.
//
// **This is the one genuinely approximate part of the whole UIXrender surface, and it is
// flagged rather than hidden.** Real measurement needs font loading plus glyph metrics
// and shaping. There is no such abstraction in Silk.NET, System.Drawing's is Windows-only
// (and a graphics API this project's dependency policy rules out), and adding a full text
// shaping stack (HarfBuzz/SixLabors.Fonts) is a much larger dependency decision than this
// pass should make unilaterally.
//
// So: metrics are derived from the requested font height using the ratios that hold for
// the overwhelming majority of Latin UI faces (Segoe UI, Verdana, Tahoma -- what Zune's
// markup actually asks for). Line height and baseline placement are close to exact; per-
// character advance is an average, so a measured string's *width* is approximate and will
// not match a real rasterizer.
//
// Consequence, stated plainly: layout driven by these numbers will be plausible but not
// pixel-accurate, and text will not currently rasterize at all (see
// RichTextApi.SpRichTextRasterize). Logged as the primary open question in
// logs/UIXrender/FullSurface.md.
// TODO: replace wholesale with a real font backend; do not build on these ratios.
internal static class TextMetrics
{
    // Typical for Latin UI faces: cap-to-em ratio ~0.7, ascent ~0.8 em, descent ~0.2 em,
    // default line gap ~1.2 em.
    private const float AverageAdvanceRatio = 0.55f;
    private const float AscentRatio = 0.80f;
    private const float LineHeightRatio = 1.20f;

    public static int LineHeight(float fontHeightPts) => (int)MathF.Ceiling(fontHeightPts * LineHeightRatio);

    public static int Ascent(float fontHeightPts) => (int)MathF.Round(fontHeightPts * AscentRatio);

    public static int AverageCharWidth(float fontHeightPts) => Math.Max(1, (int)MathF.Round(fontHeightPts * AverageAdvanceRatio));

    // Measures `text` with optional word wrapping into `constraintWidth` (0 = unconstrained).
    // Wrapping itself is real (break on whitespace, fall back to a hard break for a word
    // longer than the line) -- only the per-character width feeding it is approximate.
    public static Size Measure(string text, float fontHeightPts, bool wordWrap, int constraintWidth)
    {
        if (string.IsNullOrEmpty(text))
            return new Size(0, LineHeight(fontHeightPts));

        int charWidth = AverageCharWidth(fontHeightPts);
        int lineHeight = LineHeight(fontHeightPts);

        int maxCharsPerLine = wordWrap && constraintWidth > 0
            ? Math.Max(1, constraintWidth / charWidth)
            : int.MaxValue;

        int lines = 0;
        int widestLine = 0;

        foreach (string paragraph in text.Split('\n'))
        {
            string remaining = paragraph.TrimEnd('\r');
            do
            {
                int take = Math.Min(remaining.Length, maxCharsPerLine);
                if (take < remaining.Length)
                {
                    // Prefer breaking at the last space that fits.
                    int lastSpace = remaining.LastIndexOf(' ', Math.Max(0, take - 1));
                    if (lastSpace > 0)
                        take = lastSpace;
                }

                widestLine = Math.Max(widestLine, take * charWidth);
                lines++;
                remaining = remaining[take..].TrimStart(' ');
            }
            while (remaining.Length > 0);
        }

        return new Size(widestLine, Math.Max(1, lines) * lineHeight);
    }
}
