// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSYaccTableRowEntry
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace SSVParseLib
{
    internal class SSYaccTableRowEntry
    {
        private const int SSYaccActionShift = 0;
        private const int SSYaccActionError = 1;
        private const int SSYaccActionReduce = 2;
        private const int SSYaccActionAccept = 3;
        private const int SSYaccActionConflict = 4;
        private const uint SSYaccTableEntryFlagSync = 2147483648;
        private const int SSYaccTableEntryFlagShift = 1073741824;
        private const int SSYaccTableEntryFlagReduce = 536870912;
        private const int SSYaccTableEntryFlagAccept = 268435456;
        private const int SSYaccTableEntryFlagConflict = 134217728;
        private const uint SSYaccTableEntryFlagMask = 4160749568;
        private int m_token;
        private int m_entry;
        private int m_action;
        private bool m_sync;

        public SSYaccTableRowEntry(int q_token, int q_entry, int q_action, int q_sync)
        {
            m_token = q_token;
            m_entry = q_entry;
            m_action = q_action;
            m_sync = q_sync != 0;
        }

        public int action() => m_action;

        public int entry() => m_entry;

        public int token() => m_token;
    }
}
