// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSLexFinalState
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace SSVParseLib
{
    internal class SSLexFinalState
    {
        private const int m_flagContextStart = 1;
        private const int m_flagStartOfLine = 2;
        private const int m_flagPop = 8;
        private const int m_flagFinal = 16;
        private const int m_flagPush = 32;
        private const int m_flagIgnore = 64;
        private const int m_flagContextEnd = 128;
        private const int m_flagReduce = 256;
        private const int m_flagKeyword = 512;
        private const int m_flagParseToken = 1024;
        private int m_flags;
        private int m_token;
        private int m_pushIndex;

        public SSLexFinalState(int[] q_final, int q_index)
        {
            m_token = q_final[q_index];
            m_pushIndex = q_final[q_index + 1];
            m_flags = q_final[q_index + 2];
        }

        public int token() => m_token;

        public int pushIndex() => m_pushIndex;

        public bool isPop() => (m_flags & 8) != 0;

        public bool isPush() => (m_flags & 32) != 0;

        public bool isFinal() => (m_flags & 16) != 0;

        public bool isIgnore() => (m_flags & 64) != 0;

        public bool isReduce() => (m_flags & 256) != 0;

        public bool isContextEnd() => (m_flags & 128) != 0;

        public bool isStartOfLine() => (m_flags & 2) != 0;

        public bool isContextStart() => (m_flags & 1) != 0;

        public bool isKeyword() => (m_flags & 512) != 0;
    }
}
