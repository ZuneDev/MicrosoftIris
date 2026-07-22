using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Interop.Drawing;

namespace Microsoft.Iris.Render.Interop.Text;

// Bit-for-bit mirror of Microsoft.Iris.Drawing.TextStyle.MarshalledData
// (UIX/Microsoft/Iris/Drawing/TextStyle.cs).
[System.Flags]
public enum TextStyleSetFlags
{
    None = 0,
    FontFace = 1,
    FontHeight = 2,
    Bold = 4,
    Italic = 8,
    Underline = 16,
    LineSpacing = 32,
    TextColor = 64,
    EnableKerning = 128,
    CharacterSpacing = 256,
    AltFontHeight = 512,
    BoldValue = 65536,
    ItalicValue = 131072,
    UnderlineValue = 262144,
    EnableKerningValue = 524288,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TextStyleData
{
    public TextStyleSetFlags flags;
    public char* fontFace;
    public float fontHeightPts;
    public float altFontHeightPts;
    public float lineSpacing;
    public float characterSpacing;
    public Color textColor;
}
