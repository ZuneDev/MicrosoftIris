// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.NativeXmlNodeType
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.OS
{
    internal enum NativeXmlNodeType
    {
        None = 0,
        Element = 1,
        Attribute = 2,
        Text = 3,
        CDATA = 4,
        ProcessingInstruction = 7,
        Comment = 8,
        DocumentType = 10, // 0x0000000A
        Whitespace = 13, // 0x0000000D
        EndElement = 15, // 0x0000000F
        XmlDeclaration = 17, // 0x00000011
    }
}
