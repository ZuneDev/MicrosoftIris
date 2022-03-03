// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.NativeXmlException
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.OS
{
    internal class NativeXmlException : SystemException
    {
        private uint _hr;
        private int _lineNumber;
        private int _linePosition;

        public NativeXmlException(string message, uint hr, int lineNumber, int linePosition)
          : base(message)
        {
            _hr = hr;
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        public uint HR => _hr;

        public int LineNumber => _lineNumber;

        public int LinePosition => _linePosition;
    }
}
