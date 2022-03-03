// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupBinaryDataTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal class MarkupBinaryDataTable
    {
        private string _uri;
        private Vector<string> _strings;
        private MarkupConstantsTable _constantsTable;
        private MarkupImportTables _importTables;
        private LoadResult[] _sharedDependenciesTable;
        private SourceMarkupImportTables _sourceMarkupImportTables;
        private ByteCodeReader _stringTableReader;

        public MarkupBinaryDataTable(string uri, int stringCount)
        {
            _uri = uri;
            _strings = new Vector<string>(stringCount);
            _strings.ExpandTo(stringCount);
        }

        public MarkupBinaryDataTable(string uri)
          : this(uri, 0)
        {
        }

        public void SetStringTableReader(ByteCodeReader stringTableReader) => _stringTableReader = stringTableReader;

        public string Uri => _uri;

        public int GetIndexOrAdd(string s)
        {
            int num = _strings.IndexOf(s);
            if (num == -1)
            {
                _strings.Add(s);
                num = _strings.Count - 1;
            }
            return num;
        }

        public string GetStringByIndex(int index)
        {
            string str = _strings[index];
            if (str == null)
            {
                _stringTableReader.CurrentOffset = (uint)(index * 4);
                _stringTableReader.CurrentOffset = _stringTableReader.ReadUInt32();
                str = _stringTableReader.ReadString();
                _strings[index] = str;
            }
            return str;
        }

        public Vector<string> Strings => _strings;

        public MarkupConstantsTable ConstantsTable => _constantsTable;

        public MarkupImportTables ImportTables => _importTables;

        public SourceMarkupImportTables SourceMarkupImportTables => _sourceMarkupImportTables;

        public void SetConstantsTable(MarkupConstantsTable constantsTable) => _constantsTable = constantsTable;

        public void SetImportTables(MarkupImportTables importTables) => _importTables = importTables;

        public void SetSourceMarkupImportTables(SourceMarkupImportTables sourceMarkupImportTables) => _sourceMarkupImportTables = sourceMarkupImportTables;

        public LoadResult[] SharedDependenciesTableWithBinaryDataTable
        {
            get => _sharedDependenciesTable;
            set => _sharedDependenciesTable = value;
        }
    }
}
