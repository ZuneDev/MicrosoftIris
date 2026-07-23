using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Win32;

// Bit-for-bit mirror of Microsoft.Iris.OS.Win32Api.LOGFONTW_STRUCT
// (UIX/Microsoft/Iris/OS/Win32Api.cs), including its 32-char inline face-name buffer.
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public unsafe struct LOGFONTW
{
    public const int LF_FACESIZE = 32;

    public int lfHeight;
    public int lfWidth;
    public int lfEscapement;
    public int lfOrientation;
    public int lfWeight;
    public byte lfItalic;
    public byte lfUnderline;
    public byte lfStrikeOut;
    public byte lfCharSet;
    public byte lfOutPrecision;
    public byte lfClipPrecision;
    public byte lfQuality;
    public byte lfPitchAndFamily;
    public fixed char lfFaceName[LF_FACESIZE];
}
