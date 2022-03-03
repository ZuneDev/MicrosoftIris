// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.NativeXmlReader
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Markup;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS
{
    internal struct NativeXmlReader : IDisposable
    {
        private IntPtr _reader;
        private GCHandle _gcHandle;
        private NativeXmlNodeType _prevNodeType;
        private NativeXmlNodeType _curNodeType;
        private int _lineNumber;
        private int _linePosition;
        private bool _beforeFirstAttribute;

        public NativeXmlReader(Resource resource)
          : this(false)
          => Init(resource.Buffer, (int)resource.Length, false);

        public NativeXmlReader(string content, bool isFragment)
          : this(false)
        {
            _gcHandle = GCHandle.Alloc(content, GCHandleType.Pinned);
            Init(_gcHandle.AddrOfPinnedObject(), content.Length * 2, isFragment);
        }

        private NativeXmlReader(bool placeHolder)
        {
            _lineNumber = -1;
            _linePosition = -1;
            _beforeFirstAttribute = true;
            _reader = IntPtr.Zero;
            _gcHandle = new GCHandle();
            _curNodeType = NativeXmlNodeType.None;
            _prevNodeType = NativeXmlNodeType.None;
        }

        private void Init(IntPtr buffer, int length, bool isFragment)
        {
            int xmlReader = (int)NativeApi.SpXmlLiteCreateXmlReader(buffer, length, isFragment, out _reader);
        }

        public void Dispose()
        {
            NativeApi.SpXmlLiteDeleteXmlReader(_reader);
            _reader = IntPtr.Zero;
            if (!_gcHandle.IsAllocated)
                return;
            _gcHandle.Free();
        }

        public bool Read(out NativeXmlNodeType nodeType)
        {
            _beforeFirstAttribute = true;
            _lineNumber = InternalLineNumber;
            _linePosition = InternalLinePosition;
            bool flag = SUCCEEDED(NativeApi.SpXmlLiteRead(_reader, out nodeType));
            _prevNodeType = _curNodeType;
            _curNodeType = nodeType;
            return flag;
        }

        public bool ReadAttribute()
        {
            if (_beforeFirstAttribute)
                return MoveToFirstAttribute();
            bool nextAttribute = MoveToNextAttribute();
            if (!nextAttribute)
                _beforeFirstAttribute = true;
            return nextAttribute;
        }

        private bool MoveToFirstAttribute()
        {
            _beforeFirstAttribute = false;
            _lineNumber = InternalLineNumber;
            _linePosition = InternalLinePosition;
            return SUCCEEDED(NativeApi.SpXmlLiteMoveToFirstAttribute(_reader));
        }

        private bool MoveToNextAttribute()
        {
            _lineNumber = InternalLineNumber;
            _linePosition = InternalLinePosition;
            return SUCCEEDED(NativeApi.SpXmlLiteMoveToNextAttribute(_reader));
        }

        public bool IsEmptyElement => NativeApi.SpXmlLiteIsEmptyElement(_reader);

        public string Name
        {
            get
            {
                IntPtr name;
                uint length;
                IFC(NativeApi.SpXmlLiteGetQualifiedName(_reader, out name, out length));
                return NativeApi.PtrToStringUni(name, (int)length);
            }
        }

        public string LocalName
        {
            get
            {
                IntPtr name;
                uint length;
                IFC(NativeApi.SpXmlLiteGetLocalName(_reader, out name, out length));
                return NativeApi.PtrToStringUni(name, (int)length);
            }
        }

        public string Prefix
        {
            get
            {
                IntPtr prefix;
                uint length;
                IFC(NativeApi.SpXmlLiteGetPrefix(_reader, out prefix, out length));
                return NativeApi.PtrToStringUni(prefix, (int)length);
            }
        }

        public string Value
        {
            get
            {
                IntPtr psz;
                uint length;
                IFC(NativeApi.SpXmlLiteGetValue(_reader, out psz, out length));
                return NativeApi.PtrToStringUni(psz, (int)length);
            }
        }

        public unsafe bool IsInlineExpression
        {
            get
            {
                IntPtr num;
                uint length;
                IFC(NativeApi.SpXmlLiteGetValue(_reader, out num, out length));
                char* pointer = (char*)num.ToPointer();
                return length > 0U && *pointer == '{' && pointer[length - 1U] == '}';
            }
        }

        public SSLexUnicodeBufferConsumer LexConsumerForValueWithPrefix(string prefix)
        {
            IntPtr buffer;
            uint length;
            IFC(NativeApi.SpXmlLiteGetValue(_reader, out buffer, out length));
            SSLexUnicodeBufferConsumer unicodeBufferConsumer = new SSLexUnicodeBufferConsumer(buffer, length, prefix);
            int lineNumber = _lineNumber;
            int column = _linePosition - prefix.Length + 1;
            if (_curNodeType == NativeXmlNodeType.CDATA && _prevNodeType != NativeXmlNodeType.Whitespace)
                column += 9;
            unicodeBufferConsumer.SetDocumentOffset(lineNumber, column);
            return unicodeBufferConsumer;
        }

        public int LineNumber => _lineNumber;

        public int LinePosition => _linePosition;

        private int InternalLineNumber
        {
            get
            {
                uint lineNumber;
                IFC(NativeApi.SpXmlLiteGetLineNumber(_reader, out lineNumber));
                return (int)lineNumber;
            }
        }

        private int InternalLinePosition
        {
            get
            {
                uint linePosition;
                IFC(NativeApi.SpXmlLiteGetLinePosition(_reader, out linePosition));
                return (int)linePosition;
            }
        }

        private void IFC(uint hr) => SUCCEEDED(hr);

        private bool SUCCEEDED(uint hr)
        {
            if (hr == 0U)
                return true;
            if (hr == 1U)
                return false;
            ThrowXmlException(hr);
            return false;
        }

        private void ThrowXmlException(uint hr)
        {
            string message = null;
            switch (hr)
            {
                case 3222069277:
                    message = "Character in character entity is not a decimal digit as was expected.";
                    break;
                case 3222069278:
                    message = "Character in character entity is not a hexadecimal digit as was expected.";
                    break;
                case 3222069279:
                    message = "Character entity has invalid Unicode value.";
                    break;
                case 3222072833:
                    message = "Unexpected end of input";
                    break;
                case 3222072834:
                    message = "Unrecognized encoding";
                    break;
                case 3222072835:
                    message = "Unable to switch the encoding";
                    break;
                case 3222072836:
                    message = "Unrecognized input signature";
                    break;
                case 3222072865:
                    message = "Whitespace expected";
                    break;
                case 3222072866:
                    message = "Semicolon expected";
                    break;
                case 3222072867:
                    message = "'>' expected";
                    break;
                case 3222072868:
                    message = "Quote expected";
                    break;
                case 3222072869:
                    message = "Equal expected";
                    break;
                case 3222072870:
                    message = "Well-formedness constraint: no '<' in attribute value";
                    break;
                case 3222072871:
                    message = "Hexadecimal digit expected";
                    break;
                case 3222072872:
                    message = "Decimal digit expected";
                    break;
                case 3222072873:
                    message = "'[' expected";
                    break;
                case 3222072874:
                    message = "'(' expected";
                    break;
                case 3222072875:
                    message = "Illegal xml character";
                    break;
                case 3222072876:
                    message = "Illegal name character";
                    break;
                case 3222072877:
                    message = "Incorrect document syntax";
                    break;
                case 3222072878:
                    message = "Incorrect CDATA section syntax";
                    break;
                case 3222072879:
                    message = "Incorrect comment syntax";
                    break;
                case 3222072880:
                    message = "Incorrect conditional section syntax";
                    break;
                case 3222072881:
                    message = "Incorrect ATTLIST declaration syntax";
                    break;
                case 3222072882:
                    message = "Incorrect DOCTYPE declaration syntax";
                    break;
                case 3222072883:
                    message = "Incorrect ELEMENT declaration syntax";
                    break;
                case 3222072884:
                    message = "Incorrect ENTITY declaration syntax";
                    break;
                case 3222072885:
                    message = "Incorrect NOTATION declaration syntax";
                    break;
                case 3222072886:
                    message = "NDATA expected";
                    break;
                case 3222072887:
                    message = "PUBLIC expected";
                    break;
                case 3222072888:
                    message = "SYSTEM expected";
                    break;
                case 3222072889:
                    message = "Name expected";
                    break;
                case 3222072890:
                    message = "One root element";
                    break;
                case 3222072891:
                    message = "Well-formedness constraint: element type match";
                    break;
                case 3222072892:
                    message = "Well-formedness constraint: unique attribute spec";
                    break;
                case 3222072893:
                    message = "Text/xmldecl not at the beginning of input";
                    break;
                case 3222072894:
                    message = "Leading \"xml\"";
                    break;
                case 3222072895:
                    message = "Incorrect text declaration syntax";
                    break;
                case 3222072896:
                    message = "Incorrect xml declaration syntax";
                    break;
                case 3222072897:
                    message = "Incorrect encoding name syntax";
                    break;
                case 3222072898:
                    message = "Incorrect public identifier syntax";
                    break;
                case 3222072899:
                    message = "Well-formedness constraint: pes in internal subset";
                    break;
                case 3222072900:
                    message = "Well-formedness constraint: pes between declarations";
                    break;
                case 3222072901:
                    message = "Well-formedness constraint: no recursion";
                    break;
                case 3222072902:
                    message = "Entity content not well formed";
                    break;
                case 3222072903:
                    message = "Well-formedness constraint: undeclared entity ";
                    break;
                case 3222072904:
                    message = "Well-formedness constraint: parsed entity";
                    break;
                case 3222072905:
                    message = "Well-formedness constraint: no external entity references";
                    break;
                case 3222072906:
                    message = "Incorrect processing instruction syntax";
                    break;
                case 3222072907:
                    message = "Incorrect system identifier syntax";
                    break;
                case 3222072908:
                    message = "'?' expected";
                    break;
                case 3222072909:
                    message = "No ']]>' in element content";
                    break;
                case 3222072910:
                    message = "Not all chunks of value have been read";
                    break;
                case 3222072911:
                    message = "DTD was found but is prohibited";
                    break;
                case 3222072912:
                    message = "xml:space attribute with invalid value";
                    break;
                case 3222072929:
                    message = "Illegal qualified name character";
                    break;
                case 3222072930:
                    message = "Multiple colons in qualified name";
                    break;
                case 3222072931:
                    message = "Colon in name";
                    break;
                case 3222072932:
                    message = "Declared prefix";
                    break;
                case 3222072933:
                    message = "Undeclared prefix";
                    break;
                case 3222072934:
                    message = "Non default namespace with empty uri";
                    break;
                case 3222072935:
                    message = "\"xml\" prefix is reserved and must have the http://www.w3.org/XML/1998/namespace URI";
                    break;
                case 3222072936:
                    message = "\"xmlns\" prefix is reserved for use by XML";
                    break;
                case 3222072937:
                    message = "xml namespace URI (http://www.w3.org/XML/1998/namespace) must be assigned only to prefix \"xml\"";
                    break;
                case 3222072938:
                    message = "xmlns namespace URI (http://www.w3.org/2000/xmlns/) is reserved and must not be used";
                    break;
                case 3222072961:
                    message = "Element depth exceeds limit in XmlReaderProperty_MaxElementDepth";
                    break;
                case 3222072962:
                    message = "Entity expansion exceeds limit in XmlReaderProperty_MaxEntityExpansion";
                    break;
                case 3222073089:
                    message = "Writer: specified string is not whitespace";
                    break;
                case 3222073090:
                    message = "Writer: namespace prefix is already declared with a different namespace";
                    break;
                case 3222073091:
                    message = "Writer: It is not allowed to declare a namespace prefix with empty URI (for example xmlns:p=””).";
                    break;
                case 3222073092:
                    message = "Writer: duplicate attribute";
                    break;
                case 3222073093:
                    message = "Writer: can not redefine the xmlns prefix";
                    break;
                case 3222073094:
                    message = "Writer: xml prefix must have the http://www.w3.org/XML/1998/namespace URI";
                    break;
                case 3222073095:
                    message = "Writer: xml namespace URI (http://www.w3.org/XML/1998/namespace) must be assigned only to prefix \"xml\"";
                    break;
                case 3222073096:
                    message = "Writer: xmlns namespace URI (http://www.w3.org/2000/xmlns/) is reserved and must not be used";
                    break;
                case 3222073097:
                    message = "Writer: namespace is not declared";
                    break;
                case 3222073098:
                    message = "Writer: invalid value of xml:space attribute (allowed values are \"default\" and \"preserve\")";
                    break;
                case 3222073099:
                    message = "Writer: performing the requested action would result in invalid XML document";
                    break;
                case 3222073100:
                    message = "Writer: input contains invalid or incomplete surrogate pair";
                    break;
            }
            if (message != null)
            {
                int lineNumber = _reader == IntPtr.Zero ? -1 : InternalLineNumber;
                int linePosition = _reader == IntPtr.Zero ? -1 : InternalLinePosition;
                throw new NativeXmlException(message, hr, lineNumber, linePosition);
            }
            throw new NativeXmlException("Unknown error", hr, -1, -1);
        }
    }
}
