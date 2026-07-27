namespace Microsoft.Iris.Render.Text;

public sealed class TextStyleInfo
{
    public string FontFace { get; set; }
    public float FontSize { get; set; }
    public float AltFontSize { get; set; }
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Underline { get; set; }
    public ColorF Color { get; set; }
    public bool HasColor { get; set; }
    public float LineSpacing { get; set; }
    public float CharacterSpacing { get; set; }
    public bool EnableKerning { get; set; }
}
