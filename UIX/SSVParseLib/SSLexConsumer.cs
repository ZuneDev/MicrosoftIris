// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSLexConsumer
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace SSVParseLib
{
    internal abstract class SSLexConsumer
    {
        private int m_docOriginLine;
        private int m_docOriginColumn;
        private int m_line;
        private int m_start;
        protected int m_index;
        private int m_offset;
        public char m_current;
        private int m_scanLine;
        private int m_scanOffset;
        public int m_bufferLexemeStart;
        private char m_bof;
        private bool m_endOfData;

        public abstract bool getNext();

        public abstract string getSubstring(int start, int length);

        public bool next()
        {
            if (m_endOfData)
                return false;
            if ((m_current = m_bof) != char.MinValue)
            {
                m_bof = char.MinValue;
                return true;
            }
            if (!getNext())
            {
                ++m_index;
                m_endOfData = true;
                return false;
            }
            ++m_index;
            if (m_current == '\n')
            {
                ++m_scanLine;
                m_scanOffset = 1;
            }
            else
                ++m_scanOffset;
            return true;
        }

        public int line() => m_docOriginLine + m_line;

        public int offset() => m_line != 0 ? m_offset : m_docOriginColumn + m_offset;

        public char getCurrent() => m_current;

        public SSLexMark mark() => new SSLexMark(m_scanLine, m_scanOffset, m_index);

        public void flushEndOfLine(ref SSLexMark? q_mark)
        {
            SSLexMark ssLexMark = new SSLexMark(q_mark.Value.m_line - 1, q_mark.Value.m_offset, q_mark.Value.m_index - 1);
            q_mark = new SSLexMark?(ssLexMark);
        }

        public void flushStartOfLine(ref SSLexMark? q_mark)
        {
            ++m_line;
            ++m_start;
            SSLexMark ssLexMark = new SSLexMark(q_mark.Value.m_line - 1, q_mark.Value.m_offset, q_mark.Value.m_index);
            q_mark = new SSLexMark?(ssLexMark);
            m_offset = 1;
        }

        public virtual void flushLexeme(SSLexMark q_mark)
        {
            m_start = m_index = q_mark.m_index;
            m_line += q_mark.m_line;
            m_offset = q_mark.m_offset;
            m_scanLine = 0;
            m_scanOffset = q_mark.m_offset;
            m_bufferLexemeStart = m_index;
        }

        public void flushLexeme()
        {
            m_start = m_index;
            m_line += m_scanLine;
            m_offset = m_scanOffset;
            m_scanLine = 0;
            m_bufferLexemeStart = m_index;
        }

        public int lexemeLength() => m_index - m_start;

        public int lexemeLength(SSLexMark q_mark) => q_mark.index() - m_start;

        public string lexemeBuffer() => getSubstring(m_bufferLexemeStart, lexemeLength());

        public string lexemeBuffer(SSLexMark q_mark) => getSubstring(m_bufferLexemeStart, lexemeLength(q_mark));

        public void SetDocumentOffset(int line, int column)
        {
            m_docOriginLine = line;
            m_docOriginColumn = column;
        }
    }
}
