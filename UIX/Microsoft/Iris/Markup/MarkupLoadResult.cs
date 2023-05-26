// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupLoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Markup
{
    [Serializable]
    public abstract class MarkupLoadResult : LoadResult
    {
        private LoadResult[] _dependenciesTable = EmptyList;
        private ByteCodeReader _reader;
        protected MarkupBinaryDataTable _binaryDataTable;
        protected MarkupLineNumberTable _lineNumberTable;
        private TypeSchema[] _exportTable = TypeSchema.EmptyList;
        private AliasMapping[] _aliasTable;
        private Map<string, TypeSchema> _resolvedAliases;
        private MarkupDataMapping[] _dataMappingsTable;
        private LoadResultStatus _status;

        public static LoadResult Create(string uri, Resource resource) => !CompiledMarkupLoader.IsUIB(resource) ? new SourceMarkupLoadResult(resource, uri) : (LoadResult)new CompiledMarkupLoadResult(resource, uri);

        public MarkupLoadResult(string uri)
          : base(uri)
          => _status = LoadResultStatus.Loading;

        protected MarkupLoadResult(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _dependenciesTable = info.GetValue<LoadResult[]>(nameof(Dependencies));
            _exportTable = info.GetValue<TypeSchema[]>(nameof(ExportTable));
            _aliasTable = info.GetValue<AliasMapping[]>(nameof(AliasTable));
            _dataMappingsTable = info.GetValue<MarkupDataMapping[]>(nameof(DataMappingsTable));
            _binaryDataTable = info.GetValue<MarkupBinaryDataTable>(nameof(BinaryDataTable));
            _lineNumberTable = info.GetValue<MarkupLineNumberTable>(nameof(LineNumberTable));
        }

        private void OnLoaded()
        {
            if (_dataMappingsTable == null)
                return;
            foreach (MarkupDataMapping mapping in _dataMappingsTable)
                MarkupDataProvider.AddDataMapping(mapping);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (_dataMappingsTable != null)
            {
                foreach (MarkupDataMapping mapping in _dataMappingsTable)
                    MarkupDataProvider.RemoveDataMapping(mapping);
            }
            if (_reader != null)
                _reader.Dispose(this);
            foreach (DisposableObject disposableObject in _exportTable)
                disposableObject.Dispose(this);
            _exportTable = null;
        }

        public override TypeSchema FindType(string name)
        {
            TypeSchema typeSchema = FindTypeWorker(name);
            if (typeSchema == null)
            {
                typeSchema = ResolveAlias(name);
                if (typeSchema != null)
                    _resolvedAliases[name] = typeSchema;
            }
            return typeSchema;
        }

        private TypeSchema FindTypeWorker(string name)
        {
            for (int index = 0; index < _exportTable.Length; ++index)
            {
                TypeSchema typeSchema = _exportTable[index];
                if (name == typeSchema.Name)
                    return typeSchema;
            }
            TypeSchema typeSchema1;
            return _resolvedAliases != null && _resolvedAliases.TryGetValue(name, out typeSchema1) ? typeSchema1 : null;
        }

        private TypeSchema ResolveAlias(string name)
        {
            int num = 100;
            var markupLoadResult = this;
            while (num-- > 0)
            {
                AliasMapping aliasMapping1 = null;
                if (markupLoadResult._aliasTable != null)
                {
                    foreach (AliasMapping aliasMapping2 in markupLoadResult._aliasTable)
                    {
                        if (aliasMapping2.Alias == name)
                        {
                            aliasMapping1 = aliasMapping2;
                            break;
                        }
                    }
                }
                if (aliasMapping1 != null)
                {
                    if (!(aliasMapping1.LoadResult is MarkupLoadResult))
                        return aliasMapping1.LoadResult.FindType(aliasMapping1.TargetType);
                    markupLoadResult = (MarkupLoadResult)aliasMapping1.LoadResult;
                    TypeSchema typeWorker = markupLoadResult.FindTypeWorker(aliasMapping1.TargetType);
                    if (typeWorker != null)
                        return typeWorker;
                    name = aliasMapping1.TargetType;
                }
                else
                    break;
            }
            if (num <= 0)
                ErrorManager.ReportError("Alias cycle detected: {0} {1}", ToString(), name);
            return null;
        }

        public abstract bool IsSource { get; }

        public override LoadResult[] Dependencies => _dependenciesTable;

        public abstract MarkupImportTables ImportTables { get; }

        public ByteCodeReader ObjectSection => _reader;

        public override TypeSchema[] ExportTable => _exportTable;

        public AliasMapping[] AliasTable => _aliasTable;

        public MarkupDataMapping[] DataMappingsTable => _dataMappingsTable;

        public MarkupBinaryDataTable BinaryDataTable => _binaryDataTable;

        public abstract MarkupConstantsTable ConstantsTable { get; }

        public virtual MarkupLineNumberTable LineNumberTable => _lineNumberTable;

        public void SetDependenciesTable(LoadResult[] dependenciesTable, bool registerDependencies)
        {
            _dependenciesTable = dependenciesTable;
            if (!registerDependencies)
                return;
            RegisterDependenciesUsage();
        }

        public void SetDependenciesTable(LoadResult[] dependenciesTable) => SetDependenciesTable(dependenciesTable, true);

        public void SetObjectSection(ByteCodeReader reader)
        {
            reader.DeclareOwner(this);
            _reader = reader;
        }

        public void SetBinaryDataTable(MarkupBinaryDataTable binaryDataTable) => _binaryDataTable = binaryDataTable;

        public void SetLineNumberTable(MarkupLineNumberTable lineNumberTable) => _lineNumberTable = lineNumberTable;

        public void SetExportTable(TypeSchema[] exportTable) => _exportTable = exportTable;

        public void SetAliasTable(AliasMapping[] aliasTable)
        {
            _aliasTable = aliasTable;
            if (_aliasTable == null)
                return;
            _resolvedAliases = new Map<string, TypeSchema>(_aliasTable.Length);
        }

        public void SetDataMappingsTable(MarkupDataMapping[] dataMappingsTable) => _dataMappingsTable = dataMappingsTable;

        public override LoadResultStatus Status => _status;

        public void SetStatus(LoadResultStatus status)
        {
            if (_status == status)
                return;
            _status = status;
            if (_status != LoadResultStatus.Success)
                return;
            OnLoaded();
        }

        public virtual void MarkLoadFailed() => _status = LoadResultStatus.Error;

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(IsSource), IsSource);
            info.AddValue(nameof(ImportTables), ImportTables);
            info.AddValue(nameof(AliasTable), AliasTable);
            info.AddValue(nameof(DataMappingsTable), DataMappingsTable);
            info.AddValue(nameof(BinaryDataTable), BinaryDataTable);
            info.AddValue(nameof(ConstantsTable), ConstantsTable);
            info.AddValue(nameof(LineNumberTable), LineNumberTable);
        }
    }
}
