// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.SourceMarkupLoader
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.Markup.Validation;
using Microsoft.Iris.Session;
using System.Collections.Generic;

namespace Microsoft.Iris.Markup
{
    internal class SourceMarkupLoader
    {
        private const string MePrefix = "me";
        public const string MeNamespace = "Me";
        private ParseResult _parseResult;
        private SourceMarkupLoadResult _loadResultTarget;
        private Vector _validateObjects = new Vector();
        private Vector _foundExportedTypes = new Vector();
        private Vector _foundAliasMappings;
        private bool _hasErrors;
        public Dictionary<object, object> _importedNamespaces = new Dictionary<object, object>();
        private bool _usingSharedBinaryDataTable;
        private SourceMarkupImportTables _importTables;
        private Dictionary<string, object> _referencedNamespaces = new Dictionary<string, object>();
        private LoadPass _currentValidationPass;

        public static SourceMarkupLoader Load(
          SourceMarkupLoadResult loadResult,
          Resource resource)
        {
            SourceMarkupLoader owner = new SourceMarkupLoader(loadResult);
            ParseResult parseResult = Parser.Invoke(owner, resource);
            if (parseResult.HasErrors)
                owner.MarkHasErrors();
            else
                owner.SetParseResult(parseResult);
            return owner;
        }

        protected SourceMarkupLoader(SourceMarkupLoadResult loadResultTarget) => _loadResultTarget = loadResultTarget;

        public SourceMarkupLoadResult LoadResultTarget => _loadResultTarget;

        public void SetParseResult(ParseResult parseResult) => _parseResult = parseResult;

        public LoadResult FindDependency(string prefix)
        {
            if (prefix == null)
                return MarkupSystem.UIXGlobal;
            object obj;
            _importedNamespaces.TryGetValue(prefix, out obj);
            return (LoadResult)obj;
        }

        public void Validate(LoadPass currentPass)
        {
            if (_currentValidationPass >= currentPass)
                return;
            _currentValidationPass = currentPass;
            if (_currentValidationPass == LoadPass.DeclareTypes)
            {
                if (_parseResult == null)
                    return;
                if (_parseResult.Root != "UIX")
                {
                    ReportError("Unknown markup format. Expecting <UIX> ... </UIX> root markup tags", -1, -1);
                    return;
                }
                if (_parseResult.Version != "http://schemas.microsoft.com/2007/uix")
                {
                    ReportError(string.Format("Unsupported version of markup: '{0}'", _parseResult.Version), -1, -1);
                    return;
                }
                if (_loadResultTarget.BinaryDataTable != null)
                {
                    _importTables = _loadResultTarget.BinaryDataTable.SourceMarkupImportTables;
                    _usingSharedBinaryDataTable = true;
                }
                else
                    _importTables = new SourceMarkupImportTables();
                for (ValidateNamespace validateNamespace = _parseResult.XmlnsList; validateNamespace != null; validateNamespace = validateNamespace.Next)
                {
                    LoadResult loadResult = validateNamespace.Validate();
                    if (loadResult != null)
                    {
                        _importedNamespaces[validateNamespace.Prefix] = loadResult;
                        TrackImportedLoadResult(loadResult);
                    }
                }
            }
            foreach (LoadResult loadResult in _importedNamespaces.Values)
            {
                if (loadResult != _loadResultTarget && loadResult != MarkupSystem.UIXGlobal)
                {
                    loadResult.Load(_currentValidationPass);
                    if (loadResult.Status == LoadResultStatus.Error)
                        MarkHasErrors();
                }
            }
            if (_parseResult != null && currentPass != LoadPass.Done)
            {
                foreach (ValidateClass validateClass in _parseResult.ClassList)
                    validateClass.Validate(_currentValidationPass);
                foreach (ValidateDataMapping dataMapping in _parseResult.DataMappingList)
                    dataMapping.Validate(_currentValidationPass);
                foreach (ValidateAlias alias in _parseResult.AliasList)
                    alias.Validate(_currentValidationPass);
                if (_currentValidationPass == LoadPass.Full)
                {
                    for (ValidateNamespace validateNamespace = _parseResult.XmlnsList; validateNamespace != null; validateNamespace = validateNamespace.Next)
                    {
                        if (!_referencedNamespaces.ContainsKey(validateNamespace.Prefix))
                            ErrorManager.ReportWarning(validateNamespace.Line, validateNamespace.Column, "Unreferenced namespace {0}", validateNamespace.Prefix);
                    }
                }
            }
            CompleteValidationPass();
        }

        public void CompleteValidationPass()
        {
            if (_currentValidationPass == LoadPass.DeclareTypes)
            {
                _loadResultTarget.SetExportTable(PrepareExportTable());
                _loadResultTarget.SetAliasTable(PrepareAliasTable());
            }
            else if (_currentValidationPass == LoadPass.PopulatePublicModel)
            {
                if (_parseResult == null)
                    return;
                foreach (ValidateClass validateClass in _parseResult.ClassList)
                {
                    if (validateClass.TypeExport != null)
                        validateClass.TypeExport.BuildProperties();
                }
            }
            else
            {
                if (_currentValidationPass != LoadPass.Full)
                    return;
                if (HasErrors)
                    _loadResultTarget.MarkLoadFailed();
                MarkupImportTables importTables = null;
                if (_importTables != null)
                {
                    importTables = _importTables.PrepareImportTables();
                    _loadResultTarget.SetImportTables(importTables);
                }
                MarkupLineNumberTable lineNumberTable = new MarkupLineNumberTable();
                MarkupConstantsTable constantsTable = _loadResultTarget.BinaryDataTable == null ? new MarkupConstantsTable() : _loadResultTarget.BinaryDataTable.ConstantsTable;
                _loadResultTarget.SetDataMappingsTable(PrepareDataMappingTable());
                _loadResultTarget.ValidationComplete();
                ByteCodeReader reader = null;
                if (!HasErrors)
                    reader = new MarkupEncoder(importTables, constantsTable, lineNumberTable).EncodeOBJECTSection(_parseResult, _loadResultTarget.Uri, null);
                if (!_usingSharedBinaryDataTable)
                {
                    constantsTable.PrepareForRuntimeUse();
                    _loadResultTarget.SetConstantsTable(constantsTable);
                }
                lineNumberTable.PrepareForRuntimeUse();
                _loadResultTarget.SetLineNumberTable(lineNumberTable);
                if (reader != null)
                    _loadResultTarget.SetObjectSection(reader);
                _loadResultTarget.SetDependenciesTable(PrepareDependenciesTable());
                if (!MarkupSystem.TrackAdditionalMetadata)
                    _parseResult = null;
                foreach (DisposableObject validateObject in _validateObjects)
                    validateObject.Dispose(this);
            }
        }

        public int RegisterExportedType(MarkupTypeSchema type)
        {
            int count = _foundExportedTypes.Count;
            _foundExportedTypes.Add(type);
            return count;
        }

        public int RegisterAlias(string alias, LoadResult loadResult, string targetType)
        {
            TrackImportedLoadResult(loadResult);
            if (_foundAliasMappings == null)
                _foundAliasMappings = new Vector();
            int count = _foundAliasMappings.Count;
            _foundAliasMappings.Add(new AliasMapping(alias, loadResult, targetType));
            return count;
        }

        public bool IsTypeNameTaken(string typeName)
        {
            foreach (TypeSchema foundExportedType in _foundExportedTypes)
            {
                if (foundExportedType.Name == typeName)
                    return true;
            }
            if (_foundAliasMappings != null)
            {
                foreach (AliasMapping foundAliasMapping in _foundAliasMappings)
                {
                    if (foundAliasMapping.Alias == typeName)
                        return true;
                }
            }
            return false;
        }

        public void TrackValidateObject(Microsoft.Iris.Markup.Validation.Validate validate)
        {
            _validateObjects.Add(validate);
            validate.DeclareOwner(this);
        }

        public void TrackImportedLoadResult(LoadResult loadResult)
        {
            if (loadResult == MarkupSystem.UIXGlobal)
                return;
            for (int index = 0; index < _importTables.ImportedLoadResults.Count; ++index)
            {
                if ((LoadResult)_importTables.ImportedLoadResults[index] == loadResult)
                    return;
            }
            _importTables.ImportedLoadResults.Add(loadResult);
        }

        public int TrackImportedType(TypeSchema type)
        {
            TrackImportedLoadResult(type.Owner);
            return TrackImportedSchema(_importTables.ImportedTypes, type);
        }

        public int TrackImportedConstructor(ConstructorSchema constructor)
        {
            int num = TrackImportedSchema(_importTables.ImportedConstructors, constructor);
            TrackImportedType(constructor.Owner);
            foreach (TypeSchema parameterType in constructor.ParameterTypes)
                TrackImportedType(parameterType);
            return num;
        }

        public int TrackImportedProperty(PropertySchema property)
        {
            int num = TrackImportedSchema(_importTables.ImportedProperties, property);
            TrackImportedType(property.Owner);
            return num;
        }

        public int TrackImportedMethod(MethodSchema method)
        {
            int num = TrackImportedSchema(_importTables.ImportedMethods, method);
            TrackImportedType(method.Owner);
            foreach (TypeSchema parameterType in method.ParameterTypes)
                TrackImportedType(parameterType);
            return num;
        }

        public int TrackImportedEvent(EventSchema evt)
        {
            int num = TrackImportedSchema(_importTables.ImportedEvents, evt);
            TrackImportedType(evt.Owner);
            return num;
        }

        public int TrackImportedSchema(Vector importList, object schema)
        {
            for (int index = 0; index < importList.Count; ++index)
            {
                if (importList[index] == schema)
                    return index;
            }
            int count = importList.Count;
            importList.Add(schema);
            return count;
        }

        public void ReportError(string error, int line, int column)
        {
            MarkHasErrors();
            ErrorManager.ReportError(line, column, error);
        }

        public void MarkHasErrors()
        {
            if (_hasErrors)
                return;
            _hasErrors = true;
            _loadResultTarget.MarkLoadFailed();
        }

        public LoadResult[] PrepareDependenciesTable()
        {
            LoadResult[] loadResultArray = LoadResult.EmptyList;
            int length = 0;
            if (_importTables != null)
            {
                foreach (LoadResult importedLoadResult in _importTables.ImportedLoadResults)
                {
                    if (importedLoadResult != _loadResultTarget)
                        ++length;
                }
            }
            if (length != 0)
            {
                loadResultArray = new LoadResult[length];
                int index = 0;
                foreach (LoadResult importedLoadResult in _importTables.ImportedLoadResults)
                {
                    if (importedLoadResult != _loadResultTarget)
                    {
                        loadResultArray[index] = importedLoadResult;
                        ++index;
                    }
                }
            }
            return loadResultArray;
        }

        public TypeSchema[] PrepareExportTable()
        {
            TypeSchema[] typeSchemaArray = TypeSchema.EmptyList;
            if (_foundExportedTypes.Count > 0)
            {
                typeSchemaArray = new TypeSchema[_foundExportedTypes.Count];
                for (int index = 0; index < _foundExportedTypes.Count; ++index)
                {
                    MarkupTypeSchema foundExportedType = (MarkupTypeSchema)_foundExportedTypes[index];
                    typeSchemaArray[index] = foundExportedType;
                }
            }
            return typeSchemaArray;
        }

        private AliasMapping[] PrepareAliasTable()
        {
            AliasMapping[] aliasMappingArray = null;
            if (_foundAliasMappings != null)
            {
                aliasMappingArray = new AliasMapping[_foundAliasMappings.Count];
                for (int index = 0; index < _foundAliasMappings.Count; ++index)
                    aliasMappingArray[index] = (AliasMapping)_foundAliasMappings[index];
            }
            return aliasMappingArray;
        }

        private MarkupDataMapping[] PrepareDataMappingTable()
        {
            MarkupDataMapping[] dataMappingTable = null;
            int length = PrepareDataMappingTableHelper(null);
            if (length > 0)
            {
                dataMappingTable = new MarkupDataMapping[length];
                PrepareDataMappingTableHelper(dataMappingTable);
            }
            return dataMappingTable;
        }

        private int PrepareDataMappingTableHelper(MarkupDataMapping[] dataMappingTable)
        {
            int dataMappingCount = 0;
            if (_parseResult != null)
            {
                foreach (ValidateClass validateClass in _parseResult.ClassList)
                {
                    if (validateClass is ValidateDataType validateDataType)
                        AddDataMappingsHelper(validateDataType.FoundDataMappingSet, dataMappingTable, ref dataMappingCount);
                }
                foreach (ValidateDataMapping dataMapping in _parseResult.DataMappingList)
                    AddDataMappingsHelper(dataMapping.FoundDataMappingSet, dataMappingTable, ref dataMappingCount);
            }
            return dataMappingCount;
        }

        private void AddDataMappingsHelper(
          Vector<MarkupDataMapping> dataMappingSet,
          MarkupDataMapping[] dataMappingTable,
          ref int dataMappingCount)
        {
            if (dataMappingSet == null)
                return;
            foreach (MarkupDataMapping dataMapping in dataMappingSet)
            {
                if (dataMappingTable != null)
                    dataMappingTable[dataMappingCount] = dataMapping;
                ++dataMappingCount;
            }
        }

        public void NotifyTypeIdentifierFound(string prefix, string type)
        {
            if (prefix == null)
                return;
            _referencedNamespaces[prefix] = null;
        }

        public ValidateObjectTag CreateObjectTagValidator(
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int offset,
          bool isRootTag)
        {
            ValidateObjectTag validateObjectTag = null;
            if (isRootTag)
            {
                if (typeIdentifier.TypeName == ClassSchema.Type.Name)
                    validateObjectTag = new ValidateClass(this, typeIdentifier, line, offset);
                else if (typeIdentifier.TypeName == UISchema.Type.Name)
                    validateObjectTag = new ValidateUI(this, typeIdentifier, line, offset);
                else if (typeIdentifier.TypeName == EffectSchema.Type.Name)
                    validateObjectTag = new ValidateEffect(this, typeIdentifier, line, offset);
                else if (typeIdentifier.TypeName == AliasSchema.Type.Name)
                    validateObjectTag = new ValidateAlias(this, typeIdentifier, line, offset);
                else if (typeIdentifier.TypeName == DataTypeSchema.Type.Name)
                    validateObjectTag = new ValidateDataType(this, typeIdentifier, line, offset);
                else if (typeIdentifier.TypeName == DataQuerySchema.Type.Name)
                    validateObjectTag = new ValidateDataQuery(this, typeIdentifier, line, offset);
                else if (typeIdentifier.TypeName == DataMappingSchema.Type.Name)
                    validateObjectTag = new ValidateDataMapping(this, typeIdentifier, line, offset);
            }
            else if (typeIdentifier.Prefix == null)
            {
                if (typeIdentifier.TypeName == TypeConstraintSchema.Type.Name)
                {
                    validateObjectTag = new ValidateTypeConstraint(this, typeIdentifier, line, offset);
                }
                else
                {
                    TypeSchema type = MarkupSystem.UIXGlobal.FindType(typeIdentifier.TypeName);
                    if (EffectElementSchema.Type.IsAssignableFrom(type))
                        validateObjectTag = new ValidateEffectElement(this, typeIdentifier, line, offset);
                }
            }
            if (validateObjectTag == null)
                validateObjectTag = new ValidateObjectTag(this, typeIdentifier, line, offset);
            return validateObjectTag;
        }

        public void NotifyClassParseComplete(string className)
        {
        }

        public bool HasErrors => _hasErrors;

        internal ParseResult ParseResult => _parseResult;
    }
}
