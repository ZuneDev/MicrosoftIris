using System;
using System.IO;
using System.Xml;
using Microsoft.Iris.Render.Interop.XmlLite;

namespace Microsoft.Iris.Render.Subsystems.Xml;

// Backs the SpXmlLite* family (UIX/Microsoft/Iris/OS/NativeApi.cs). The original was a
// thin wrapper over Windows' XmlLite COM reader; this is the same *contract* over
// System.Xml.XmlReader, which is in-box, cross-platform and behaviourally equivalent for
// the pull-parsing subset the surface exposes (read a node, inspect name/prefix/value,
// walk attributes, report line/position).
//
// One real semantic difference to be aware of and handled here, not papered over: XmlLite
// reports attributes as nodes you move to with MoveToFirst/NextAttribute and then read
// via the *same* GetLocalName/GetValue accessors, and System.Xml.XmlReader works exactly
// that way too -- so the mapping is direct. What is *not* direct is IsEmptyElement, which
// XmlReader only reports on the element node itself; it's captured on each read rather
// than queried lazily, so it stays correct after moving to an attribute.
internal sealed class XmlLiteReader : IDisposable
{
    private readonly XmlReader _reader;
    private readonly IXmlLineInfo _lineInfo;

    private XmlLiteReader(XmlReader reader)
    {
        _reader = reader;
        _lineInfo = reader as IXmlLineInfo;
    }

    public bool IsEmptyElement { get; private set; }

    public static XmlLiteReader Create(string text, bool isFragment)
    {
        var settings = new XmlReaderSettings
        {
            // A "fragment" has no single root element; ConformanceLevel.Fragment is
            // exactly XmlLite's isFragment flag.
            ConformanceLevel = isFragment ? ConformanceLevel.Fragment : ConformanceLevel.Document,
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreWhitespace = false,
            CloseInput = true,
        };

        return new XmlLiteReader(XmlReader.Create(new StringReader(text), settings));
    }

    public bool Read(out NativeXmlNodeType nodeType)
    {
        if (!_reader.Read())
        {
            nodeType = NativeXmlNodeType.None;
            IsEmptyElement = false;
            return false;
        }

        IsEmptyElement = _reader.NodeType == XmlNodeType.Element && _reader.IsEmptyElement;
        nodeType = Map(_reader.NodeType);
        return true;
    }

    public bool MoveToFirstAttribute() => _reader.MoveToFirstAttribute();

    public bool MoveToNextAttribute() => _reader.MoveToNextAttribute();

    public string LocalName => _reader.LocalName;
    public string Prefix => _reader.Prefix;
    public string QualifiedName => _reader.Name;
    public string Value => _reader.Value;

    public uint LineNumber => (uint)(_lineInfo?.LineNumber ?? 0);
    public uint LinePosition => (uint)(_lineInfo?.LinePosition ?? 0);

    // NativeXmlNodeType's values are System.Xml.XmlNodeType's own numbering minus the
    // members Iris doesn't use (verified against UIX/Microsoft/Iris/OS/NativeXmlNodeType.cs),
    // so this could be a cast -- it's written out so an unmapped node type degrades to
    // None instead of producing a value the managed side has no case for.
    private static NativeXmlNodeType Map(XmlNodeType type) => type switch
    {
        XmlNodeType.Element => NativeXmlNodeType.Element,
        XmlNodeType.Attribute => NativeXmlNodeType.Attribute,
        XmlNodeType.Text => NativeXmlNodeType.Text,
        XmlNodeType.CDATA => NativeXmlNodeType.CDATA,
        XmlNodeType.ProcessingInstruction => NativeXmlNodeType.ProcessingInstruction,
        XmlNodeType.Comment => NativeXmlNodeType.Comment,
        XmlNodeType.DocumentType => NativeXmlNodeType.DocumentType,
        XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace => NativeXmlNodeType.Whitespace,
        XmlNodeType.EndElement => NativeXmlNodeType.EndElement,
        XmlNodeType.XmlDeclaration => NativeXmlNodeType.XmlDeclaration,
        _ => NativeXmlNodeType.None,
    };

    public void Dispose() => _reader.Dispose();
}
