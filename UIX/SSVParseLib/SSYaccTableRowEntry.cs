// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSYaccTableRowEntry
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace SSVParseLib
{
    internal enum SSYaccAction
    {
        Shift, Error, Reduce, Accept, Conflict
    }

    internal class SSYaccTableRowEntry
    {
        [Flags]
        internal enum SSYaccTableEntryFlag : uint
        {
            Sync = 0x8000_0000,
            Shift = 0x4000_0000,
            Reduce = 0x2000_0000,
            Accept = 0x1000_0000,
            Conflict = 0x0800_0000,
            Mask = 0xF800_0000,
        }

        private int m_token;
        private int m_entry;
        private SSYaccAction m_action;
        private bool m_sync;

        public SSYaccTableRowEntry(int q_token, int q_entry, SSYaccAction q_action, int q_sync)
        {
            m_token = q_token;
            m_entry = q_entry;
            m_action = q_action;
            m_sync = q_sync != 0;
        }

        public SSYaccAction action() => m_action;

        public int entry() => m_entry;

        public int token() => m_token;
    }
}
