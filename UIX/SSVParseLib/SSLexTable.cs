// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSLexTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Collections;

namespace SSVParseLib
{
    internal class SSLexTable
    {
        private const int SSLexTableHeaderSize = 36;
        private const int SSLexDfaTableHeaderSize = 40;
        private const int SSLexDfaKeywordTableHeaderSize = 40;
        private const int SSLexDfaClassTableHeaderSize = 12;
        private const int SSLexDfaClassTableEntryHeaderSize = 8;
        public const int SSLexStateInvalid = -1;
        private Stack m_stack;
        public SSLexSubtable[] m_subTables;

        public SSLexTable()
        {
            m_stack = new Stack();
            m_subTables = null;
        }

        public void findKeyword(SSLexLexeme z_lexeme) => throw new Exception("Code has been disabled.");

        public void gotoSubtable(int q_index)
        {
            m_stack.Pop();
            m_stack.Push(m_subTables[q_index]);
        }

        public void pushSubtable(int q_index) => m_stack.Push(m_subTables[q_index]);

        public void popSubtable() => m_stack.Pop();

        public int lookup(int q_state, int q_next) => ((SSLexSubtable)m_stack.Peek()).lookup(q_state, q_next);

        public SSLexFinalState lookupFinal(int q_state) => ((SSLexSubtable)m_stack.Peek()).lookupFinal(q_state);

        public void Reset()
        {
            m_stack.Clear();
            pushSubtable(0);
        }
    }
}
