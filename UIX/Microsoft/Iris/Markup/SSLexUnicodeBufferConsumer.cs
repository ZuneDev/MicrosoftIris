// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.SSLexUnicodeBufferConsumer
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using SSVParseLib;
using System;

namespace Microsoft.Iris.Markup
{
    internal class SSLexUnicodeBufferConsumer : SSLexConsumer
    {
        private char[] _buffer;
        private uint _length;
        private string _prefix;

        public SSLexUnicodeBufferConsumer(char[] buffer, uint length)
          : this(buffer, length, "")
        {
        }

        public SSLexUnicodeBufferConsumer(IntPtr intPtr, uint length)
          : this(intPtr, length, "")
        {
        }

        public unsafe SSLexUnicodeBufferConsumer(char[] buffer, uint length, string prefix)
        {
            _buffer = buffer;
            _length = length;
            _prefix = prefix;
        }

        public unsafe SSLexUnicodeBufferConsumer(IntPtr intPtr, uint length, string prefix)
        {
            _buffer = new char[length];
            System.Runtime.InteropServices.Marshal.Copy(intPtr, _buffer, 0, (int)length);
            _length = length;
            _prefix = prefix;
        }

        public override bool getNext() => GetCharAt(m_index, out m_current);

        public override unsafe string getSubstring(int start, int length)
        {
            if (start + length <= _prefix.Length)
            {
                return _prefix.Substring(start, length);
            }
            else
            {
                fixed (char* ptr = _buffer)
                {
                    return NativeApi.PtrToStringUni(new IntPtr(ptr + start - _prefix.Length), length);
                }
            }
        }

        public unsafe bool GetCharAt(int position, out char ch)
        {
            if (position < _prefix.Length)
            {
                ch = _prefix[m_index];
                return true;
            }
            int index = position - _prefix.Length;
            if (index < _length)
            {
                ch = _buffer[index];
                return true;
            }
            ch = char.MinValue;
            return false;
        }
    }
}
