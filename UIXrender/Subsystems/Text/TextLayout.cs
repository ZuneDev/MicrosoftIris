using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Iris.Render.Interop.Drawing;

namespace Microsoft.Iris.Render.Subsystems.Text;

// Real text measurement over a LoadedFont's actual glyph advances + kerning, with the
// ratio-based TextMetrics kept as the fallback for when no font resolves (headless with
// no fonts, an unrecognised face, etc.). Word wrapping is real either way; only the
// per-glyph width feeding it differs (real advances vs. an average).
internal static class TextLayout
{
    public static int LineHeight(LoadedFont font, float px) =>
        font != null ? (int)MathF.Ceiling(font.LineHeightPx(font.ScaleForPixelHeight(px))) : TextMetrics.LineHeight(px);

    public static int Ascent(LoadedFont font, float px) =>
        font != null ? (int)MathF.Round(font.AscentPx(font.ScaleForPixelHeight(px))) : TextMetrics.Ascent(px);

    public static Size Measure(LoadedFont font, string text, float px, bool wordWrap, int constraintWidth)
    {
        if (font == null)
            return TextMetrics.Measure(text, px, wordWrap, constraintWidth);

        float scale = font.ScaleForPixelHeight(px);
        int lineHeight = (int)MathF.Ceiling(font.LineHeightPx(scale));

        if (string.IsNullOrEmpty(text))
            return new Size(0, lineHeight);

        int lines = 0;
        int widest = 0;

        foreach (string paragraph in text.Split('\n'))
        {
            string line = paragraph.TrimEnd('\r');
            if (!wordWrap || constraintWidth <= 0)
            {
                widest = Math.Max(widest, MeasureWidth(font, scale, line));
                lines++;
            }
            else
            {
                foreach (int lineWidth in WrapWidths(font, scale, line, constraintWidth))
                {
                    widest = Math.Max(widest, lineWidth);
                    lines++;
                }
            }
        }

        return new Size(widest, Math.Max(1, lines) * lineHeight);
    }

    // Pixel width of one line, honouring kerning between adjacent glyphs.
    public static int MeasureWidth(LoadedFont font, float scale, string line)
    {
        float width = 0f;
        int previous = 0;
        foreach (System.Text.Rune rune in line.EnumerateRunes())
        {
            int cp = rune.Value;
            if (previous != 0)
                width += font.KerningPx(previous, cp, scale);
            width += font.AdvancePx(cp, scale);
            previous = cp;
        }
        return (int)MathF.Ceiling(width);
    }

    // Greedy word wrap: fill each line with whole words until the next won't fit, then
    // break. A single word wider than the constraint overflows onto its own line (matching
    // the ratio fallback's behaviour) rather than being split mid-word.
    private static IEnumerable<int> WrapWidths(LoadedFont font, float scale, string paragraph, int constraintWidth)
    {
        if (paragraph.Length == 0)
        {
            yield return 0;
            yield break;
        }

        float spaceWidth = font.AdvancePx(' ', scale);
        var current = new StringBuilder();
        int currentWidth = 0;

        foreach (string word in paragraph.Split(' '))
        {
            int wordWidth = MeasureWidth(font, scale, word);
            int withSpace = current.Length == 0 ? wordWidth : currentWidth + (int)MathF.Ceiling(spaceWidth) + wordWidth;

            if (current.Length != 0 && withSpace > constraintWidth)
            {
                yield return currentWidth;
                current.Clear();
                current.Append(word);
                currentWidth = wordWidth;
            }
            else
            {
                if (current.Length != 0)
                    current.Append(' ');
                current.Append(word);
                currentWidth = withSpace;
            }
        }

        yield return currentWidth;
    }
}
