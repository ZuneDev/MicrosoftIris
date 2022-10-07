// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupLineNumberTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Diagnostics;

namespace Microsoft.Iris.Markup
{
    public class MarkupLineNumberTable
    {
        private const ulong OFFSET_MASK = 4194303;
        private const uint OFFSET_MAX = 4194303;
        private const ulong LINE_MASK = 8796088827904;
        private const int LINE_SHIFT = 22;
        private const uint LINE_MAX = 2097151;
        private const ulong COLUMN_MASK = 18446735277616529408;
        private const int COLUMN_SHIFT = 43;
        private const uint COLUMN_MAX = 2097151;
        private Vector<ulong> _lookupTable;
        private ulong[] _runtimeList;

        public MarkupLineNumberTable() => _lookupTable = new Vector<ulong>();

        public MarkupLineNumberTable(ulong[] runtimeList) => _runtimeList = runtimeList;

        public void AddRecord(uint offset, int line, int column)
        {
            ulong num = Pack(offset, line, column);
            if (_lookupTable.Count > 0 && (int)UnpackOffset(_lookupTable[_lookupTable.Count - 1]) == (int)offset)
                _lookupTable[_lookupTable.Count - 1] = num;
            else
                _lookupTable.Add(num);
        }

        public void PrepareForRuntimeUse()
        {
            _runtimeList = new ulong[_lookupTable.Count];
            for (int index = 0; index < _lookupTable.Count; ++index)
                _runtimeList[index] = _lookupTable[index];
            _lookupTable = null;
        }

        public void Lookup(uint offset, out int line, out int column)
        {
            int length = _runtimeList.Length;
            int index = 0;
            while (index < length && (offset < UnpackOffset(_runtimeList[index]) || index != length - 1 && offset >= UnpackOffset(_runtimeList[index + 1])))
                ++index;
            if (index < length)
            {
                line = UnpackLine(_runtimeList[index]);
                column = UnpackColumn(_runtimeList[index]);
            }
            else
            {
                line = -1;
                column = -1;
            }
        }

        internal ulong[] PersistList => _runtimeList;

        [Conditional("DEBUG")]
        public void DEBUG_DumpTable()
        {
        }

        private static ulong Pack(uint offset, int line, int column) => (ulong)(offset | (long)line << 22 | (long)column << 43);

        private static uint UnpackOffset(ulong value) => (uint)(value & 4194303UL);

        private static int UnpackLine(ulong value) => (int)((value & 8796088827904UL) >> 22);

        private static int UnpackColumn(ulong value) => (int)((value & 18446735277616529408UL) >> 43);
    }
}
