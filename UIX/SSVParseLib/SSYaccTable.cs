// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSYaccTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace SSVParseLib
{
    internal class SSYaccTable
    {
        private const int SSYaccTableHeaderSize = 20;
        private const int SSYaccTableEntrySize = 8;
        private const int SSYaccTableRowSize = 12;
        protected SSYaccTableRow[] m_rows;
        protected SSYaccTableProd[] m_prods;
        private SSLexSubtable[] m_lexSubtables;

        public SSYaccTable() => m_lexSubtables = null;

        public SSYaccTableRow lookupRow(int q_state) => m_rows[q_state];

        public SSYaccTableProd lookupProd(int q_index) => m_prods[q_index];

        public SSLexSubtable larTable(int q_entry) => m_lexSubtables[q_entry];
    }
}
