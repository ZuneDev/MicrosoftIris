// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSLexSubtable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace SSVParseLib
{
    internal class SSLexSubtable
    {
        private const int SSLexStateInvalid = -1;
        private int m_size;
        private SSLexTableRow[] m_rows;
        private SSLexFinalState[] m_final;

        public SSLexSubtable(int q_numRows, int[] q_rows, int[] q_final)
        {
            m_size = q_numRows;
            int q_index1 = 0;
            int q_index2 = 0;
            m_rows = new SSLexTableRow[m_size];
            m_final = new SSLexFinalState[m_size];
            for (int index = 0; index < m_size; ++index)
            {
                m_rows[index] = new SSLexTableRow(q_rows, q_index1);
                m_final[index] = new SSLexFinalState(q_final, q_index2);
                q_index2 += 3;
                q_index1 += q_rows[q_index1] * 3 + 1;
            }
        }

        public int lookup(int q_state, int q_next) => m_rows[q_state].lookup(q_next);

        public SSLexFinalState lookupFinal(int q_state) => m_final[q_state];
    }
}
