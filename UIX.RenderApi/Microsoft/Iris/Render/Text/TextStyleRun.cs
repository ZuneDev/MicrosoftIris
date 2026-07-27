namespace Microsoft.Iris.Render.Text;

// Cross-platform counterpart to Microsoft.Iris.Drawing.TextMeasureParams.FormattedRange:
// describes a sub-range of a text block (e.g. a differently-colored hyperlink
// run) that uses its own TextStyleInfo instead of the surrounding base style.
// Both FirstCharacter and LastCharacter are inclusive, matching
// TextMeasureParams.FormattedRange. See TextDocument.Measure's formatted-range
// overload and logs/ for the rationale.
public sealed class TextStyleRun
{
    public int FirstCharacter { get; set; }
    public int LastCharacter { get; set; }
    public TextStyleInfo Style { get; set; }
}
