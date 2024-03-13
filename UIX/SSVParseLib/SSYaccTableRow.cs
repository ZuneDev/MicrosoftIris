// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSYaccTableRow
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace SSVParseLib
{
    internal class SSYaccTableRow
    {
        private const int SSYaccTableEntrySize = 8;
        private const int SSYaccTableRowFlagSync = 1;
        private const int SSYaccTableRowFlagError = 2;
        private const int SSYaccTableRowFlagSyncAll = 4;
        private int m_goto;
        private int m_action;
        private bool m_sync;
        private bool m_error;
        private bool m_syncAll;
        private SSYaccTableRowEntry[] m_entries;

        public SSYaccTableRow(int[] q_data, int q_index)
        {
            m_action = q_data[q_index];
            m_goto = q_data[q_index + 1];
            m_error = q_data[q_index + 2] != 0;
            m_syncAll = q_data[q_index + 3] != 0;
            m_sync = q_data[q_index + 4] != 0;
            m_entries = new SSYaccTableRowEntry[numEntries()];
            q_index += 5;
            for (int index = 0; index < numEntries(); ++index)
            {
                var token = (ParserLexToken)q_data[q_index];
                var entry = q_data[q_index + 1];
                var action = (SSYaccAction)q_data[q_index + 2];
                var sync = q_data[q_index + 3];
                m_entries[index] = new SSYaccTableRowEntry((int)token, entry, action, sync);
                q_index += 4;
            }
        }

        public SSYaccTableRowEntry lookupAction(int q_index)
        {
            for (int index = 0; index < m_action; ++index)
            {
                if (m_entries[index].token() == q_index)
                    return m_entries[index];
            }
            return null;
        }

        public SSYaccTableRowEntry lookupGoto(int q_index)
        {
            for (int action = m_action; action < m_action + m_goto; ++action)
            {
                if (m_entries[action].token() == q_index)
                    return m_entries[action];
            }
            return null;
        }

        public bool hasError() => m_error;

        public int numEntries() => m_goto + m_action + (hasError() ? 1 : 0);
    }
}
