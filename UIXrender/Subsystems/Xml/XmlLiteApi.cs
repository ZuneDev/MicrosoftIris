#if NETCOREAPP

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.XmlLite;

namespace Microsoft.Iris.Render.Subsystems.Xml;

// [UnmanagedCallersOnly] exports for the SpXmlLite* family in UIX/Microsoft/Iris/OS/NativeApi.cs.
//
// Two behaviours below are read off the *caller* (UIX/Microsoft/Iris/OS/NativeXmlReader.cs)
// rather than assumed, because getting either backwards would produce an infinite loop or
// a silently truncated parse:
//  * `SpXmlLiteRead` / `SpXmlLiteMoveToFirstAttribute` / `SpXmlLiteMoveToNextAttribute`
//    are each tested with `SUCCEEDED(...)`, so "nothing more to read" must be reported as
//    a **failing** HRESULT, not S_FALSE (which SUCCEEDED would accept as "keep going").
//  * the `length` passed to `SpXmlLiteCreateXmlReader` is a **byte** count, not a
//    character count: NativeXmlReader's string overload passes `content.Length * 2` over
//    a pinned UTF-16 string, and its Resource overload passes a raw file buffer length.
public static unsafe class XmlLiteApi
{
    private static uint OK => (uint)HRESULT.S_OK.hr;
    private static uint Fail => unchecked((uint)HRESULT.E_FAIL.hr);
    private static uint InvalidArg => unchecked((uint)HRESULT.E_INVALIDARG.hr);

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteCreateXmlReader")]
    public static uint SpXmlLiteCreateXmlReader(IntPtr buffer, int length, int isFragment, IntPtr* xmlReader)
    {
        if (xmlReader == null)
            return InvalidArg;

        *xmlReader = IntPtr.Zero;
        if (buffer == IntPtr.Zero || length <= 0)
            return InvalidArg;

        try
        {
            string text = DecodeBuffer((byte*)buffer, length);
            *xmlReader = HandleTable.Alloc(XmlLiteReader.Create(text, isFragment != 0));
            return OK;
        }
        catch (XmlException)
        {
            return Fail;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteDeleteXmlReader")]
    public static void SpXmlLiteDeleteXmlReader(IntPtr xmlReader) => HandleTable.Free(xmlReader);

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteRead")]
    public static uint SpXmlLiteRead(IntPtr xmlReader, NativeXmlNodeType* nodeType)
    {
        if (nodeType == null || !HandleTable.TryGet(xmlReader, out XmlLiteReader reader))
            return InvalidArg;

        try
        {
            // Failure at EOF is deliberate -- see the file comment.
            return reader.Read(out NativeXmlNodeType type) ? Assign(nodeType, type, OK) : Assign(nodeType, type, Fail);
        }
        catch (XmlException)
        {
            *nodeType = NativeXmlNodeType.None;
            return Fail;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteMoveToFirstAttribute")]
    public static uint SpXmlLiteMoveToFirstAttribute(IntPtr xmlReader) =>
        HandleTable.TryGet(xmlReader, out XmlLiteReader reader)
            ? (reader.MoveToFirstAttribute() ? OK : Fail)
            : InvalidArg;

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteMoveToNextAttribute")]
    public static uint SpXmlLiteMoveToNextAttribute(IntPtr xmlReader) =>
        HandleTable.TryGet(xmlReader, out XmlLiteReader reader)
            ? (reader.MoveToNextAttribute() ? OK : Fail)
            : InvalidArg;

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteIsEmptyElement")]
    public static int SpXmlLiteIsEmptyElement(IntPtr xmlReader) =>
        HandleTable.TryGet(xmlReader, out XmlLiteReader reader) && reader.IsEmptyElement ? 1 : 0;

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteGetQualifiedName")]
    public static uint SpXmlLiteGetQualifiedName(IntPtr xmlReader, IntPtr* name, uint* length) =>
        ReturnString(xmlReader, name, length, static r => r.QualifiedName);

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteGetLocalName")]
    public static uint SpXmlLiteGetLocalName(IntPtr xmlReader, IntPtr* name, uint* length) =>
        ReturnString(xmlReader, name, length, static r => r.LocalName);

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteGetPrefix")]
    public static uint SpXmlLiteGetPrefix(IntPtr xmlReader, IntPtr* prefix, uint* length) =>
        ReturnString(xmlReader, prefix, length, static r => r.Prefix);

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteGetValue")]
    public static uint SpXmlLiteGetValue(IntPtr xmlReader, IntPtr* value, uint* length) =>
        ReturnString(xmlReader, value, length, static r => r.Value);

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteGetLineNumber")]
    public static uint SpXmlLiteGetLineNumber(IntPtr xmlReader, uint* lineNumber)
    {
        if (lineNumber == null || !HandleTable.TryGet(xmlReader, out XmlLiteReader reader))
            return InvalidArg;
        *lineNumber = reader.LineNumber;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpXmlLiteGetLinePosition")]
    public static uint SpXmlLiteGetLinePosition(IntPtr xmlReader, uint* linePosition)
    {
        if (linePosition == null || !HandleTable.TryGet(xmlReader, out XmlLiteReader reader))
            return InvalidArg;
        *linePosition = reader.LinePosition;
        return OK;
    }

    // ---- helpers ---------------------------------------------------------------------

    private static uint Assign(NativeXmlNodeType* target, NativeXmlNodeType value, uint result)
    {
        *target = value;
        return result;
    }

    // The returned pointer must stay valid until at least the caller's next call, and the
    // managed side never frees it (NativeApi.PtrToStringUni just reads it) -- so these go
    // through the intern pool, which also keeps repeated element/attribute names from
    // allocating anew on every node.
    private static uint ReturnString(IntPtr xmlReader, IntPtr* target, uint* length, Func<XmlLiteReader, string> select)
    {
        if (target == null || length == null || !HandleTable.TryGet(xmlReader, out XmlLiteReader reader))
            return InvalidArg;

        string value = select(reader) ?? string.Empty;
        *target = (IntPtr)NativeString.InternUni(value);
        *length = (uint)value.Length;
        return OK;
    }

    // The buffer is bytes of unknown encoding: NativeXmlReader feeds it either a pinned
    // UTF-16 string (no BOM) or a raw file buffer (typically UTF-8, possibly with a BOM).
    // Real XmlLite sniffs the encoding itself, so this does the same rather than assuming
    // one: BOM first, then the "every second byte is zero" pattern that distinguishes
    // BOM-less UTF-16 ASCII text, else UTF-8. Documented assumption -- see
    // logs/UIXrender/FullSurface.md.
    private static string DecodeBuffer(byte* buffer, int byteLength)
    {
        var bytes = new ReadOnlySpan<byte>(buffer, byteLength);

        if (byteLength >= 2)
        {
            if (bytes[0] == 0xFF && bytes[1] == 0xFE)
                return Encoding.Unicode.GetString(bytes[2..]);
            if (bytes[0] == 0xFE && bytes[1] == 0xFF)
                return Encoding.BigEndianUnicode.GetString(bytes[2..]);
        }

        if (byteLength >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return Encoding.UTF8.GetString(bytes[3..]);

        if (LooksLikeUtf16LittleEndian(bytes))
            return Encoding.Unicode.GetString(bytes);

        return Encoding.UTF8.GetString(bytes);
    }

    private static bool LooksLikeUtf16LittleEndian(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 2 || (bytes.Length & 1) != 0)
            return false;

        int sampled = Math.Min(bytes.Length, 32);
        for (int i = 1; i < sampled; i += 2)
        {
            if (bytes[i] != 0)
                return false;
        }
        return true;
    }
}

#endif
