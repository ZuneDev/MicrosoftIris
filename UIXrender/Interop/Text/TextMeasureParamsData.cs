using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Interop.Drawing;

namespace Microsoft.Iris.Render.Interop.Text;

// Bit-for-bit mirror of Microsoft.Iris.Drawing.TextMeasureParams.MarshalledData/FormattedRange
// (UIX/Microsoft/Iris/Drawing/TextMeasureParams.cs).
[System.Flags]
public enum TextMeasureFlags : byte
{
    None = 0,
    Content = 1,
    IsRtl = 2,
    WordWrap = 4,
    WordWrapValue = 8,
    PasswordMasked = 16,
    TrimLeftSideBearing = 32,
    FormatOnly = 64,
}

[StructLayout(LayoutKind.Sequential)]
public struct FormattedRange
{
    public int firstCharacter;
    public int lastCharacter;
    public Color color;
    public int styleIndex;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TextMeasureParamsData
{
    public TextMeasureFlags flags;
    public byte alignment;
    public char passwordChar;
    public char* content;
    public float scale;
    public SizeF constraint;
    public TextStyleData* pTextStyle;
    public int formattedRangeCount;
    public FormattedRange* pFormattedRanges;
    public int formattedRangeStylesCount;
    public TextStyleData* pFormattedRangeStyles;
}
