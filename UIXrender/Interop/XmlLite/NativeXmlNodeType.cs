namespace Microsoft.Iris.Render.Interop.XmlLite;

// Bit-for-bit mirror of Microsoft.Iris.OS.NativeXmlNodeType (UIX/Microsoft/Iris/OS/NativeXmlNodeType.cs).
public enum NativeXmlNodeType
{
    None = 0,
    Element = 1,
    Attribute = 2,
    Text = 3,
    CDATA = 4,
    ProcessingInstruction = 7,
    Comment = 8,
    DocumentType = 10,
    Whitespace = 13,
    EndElement = 15,
    XmlDeclaration = 17,
}
