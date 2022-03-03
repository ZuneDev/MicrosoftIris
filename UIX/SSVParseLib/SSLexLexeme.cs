// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSLexLexeme
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace SSVParseLib
{
    internal class SSLexLexeme
    {
        private const string c_TextForEOF = "eof";
        private int m_token;
        private int m_length;
        private int m_line;
        private int m_column;
        private int m_start;

        private SSLexLexeme()
        {
            m_line = 0;
            m_token = 0;
            m_length = 0;
            m_column = 0;
        }

        public SSLexLexeme(SSLexConsumer q_consumer)
        {
            m_token = 0;
            m_line = q_consumer.line();
            m_column = q_consumer.offset();
            m_length = q_consumer.lexemeLength();
            m_start = q_consumer.m_bufferLexemeStart;
        }

        public SSLexLexeme(SSLexConsumer q_consumer, SSLexFinalState q_final, SSLexMark q_mark)
        {
            m_token = q_final.token();
            m_line = q_consumer.line();
            m_column = q_consumer.offset();
            m_length = q_consumer.lexemeLength(q_mark);
            m_start = q_consumer.m_bufferLexemeStart;
        }

        public int line() => m_line;

        public int token() => m_token;

        public int offset() => m_column;

        public int length() => m_length;

        public string GetValue(SSLex lex) => m_token == -1 ? "eof" : lex.consumer().getSubstring(m_start, m_length);

        public string GetTrimmedValue(SSLex lex, int trimLeft, int trimRight)
        {
            int start = m_start + trimLeft;
            int length = m_length - (trimLeft + trimRight);
            return lex.consumer().getSubstring(start, length);
        }

        public static SSLexLexeme CreateEOFLexeme(SSLex lex) => new SSLexLexeme()
        {
            m_token = -1,
            m_line = lex.consumer().line(),
            m_column = lex.consumer().offset(),
            m_length = "eof".Length
        };
    }
}
