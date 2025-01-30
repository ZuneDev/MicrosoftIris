// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.CompiledMarkupLoader
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Markup;

internal class CompiledMarkupLoader
{
    /// <summary>
    /// The first four bytes of a valid compiled UIX file.
    /// </summary>
    /// <remarks>
    /// Reads UIX^Z in ASCII.
    /// </remarks>
    public const uint UIB_FILE_MAGIC = 0x1A_42_49_55U;

    private const uint c_LineNumberRecordSize = 12;
    private readonly CompiledMarkupLoadResult _loadResultTarget;
    private ByteCodeReader _reader;
    private bool _hasErrors;
    private LoadPass _currentDepersistPass;
    private MarkupLoadResult _binaryDataTableLoadResult;
    private MarkupBinaryDataTable _binaryDataTable;
    private bool _usingSharedDataTable;
    private uint _objectSectionStart;
    private uint _objectSectionEnd;
    private uint _lineNumberTableStart;
    private uint _lineNumberTableEnd;
    private TypeSchema[][] _typeSchemaArrays;

    public static CompiledMarkupLoader Load(CompiledMarkupLoadResult loadResult, Resource resource)
        => new(loadResult, resource);

    private CompiledMarkupLoader(CompiledMarkupLoadResult loadResultTarget, Resource resource)
    {
        _loadResultTarget = loadResultTarget;
        _reader = new ByteCodeReader(resource.Buffer, resource.Length, false);

        if (resource is DllResource || resource is ClrDllResource)
            _reader.MarkAsInFixedMemory();

        uint fileMagic = _reader.ReadUInt32();
        if (fileMagic != UIB_FILE_MAGIC)
            ReportError("Invalid compiled UIX file");

        uint uixFileVersion = _reader.ReadUInt32();
        if (uixFileVersion != 1012U)
            ReportError("Compiled UIX file '{0}' was compiled for the runtime with version {1}, but the current runtime is version {2}", resource.Uri, uixFileVersion.ToString(), 1012U.ToString());
    }

    public void Depersist(LoadPass currentPass)
    {
        if (_currentDepersistPass >= currentPass)
            return;

        _currentDepersistPass = currentPass;
        _binaryDataTableLoadResult?.Load(_currentDepersistPass);

        switch (_currentDepersistPass)
        {
            case LoadPass.DeclareTypes:
                DoLoadPassDeclareTypes();
                break;

            case LoadPass.PopulatePublicModel:
                DoLoadPassPopulatePublicModel();
                break;

            case LoadPass.Full:
                DoLoadPassFull();
                break;

            default:
                foreach (MarkupTypeSchema markupTypeSchema in _loadResultTarget.ExportTable)
                    markupTypeSchema.Seal();
                _reader = null;
                break;
        }

        foreach (LoadResult dependency in _loadResultTarget.Dependencies)
        {
            if (dependency != _loadResultTarget)
            {
                dependency.Load(_currentDepersistPass);

                if (dependency.Status == LoadResultStatus.Error)
                    MarkHasErrors();
            }
        }
    }

    private void DoLoadPassDeclareTypes()
    {
        if (HasErrors)
            return;
        
        DepersistTableOfContents();
        if (HasErrors)
            return;
        
        DepersistDependencies();
        if (HasErrors)
            return;

        DepersistTypeExportDeclarations();
    }

    private void DoLoadPassPopulatePublicModel()
    {
        if (HasErrors)
            return;

        DepersistTypeImportTable();
        if (HasErrors)
            return;

        DepersistTypeExportDefinitions();
    }

    private void DoLoadPassFull()
    {
        if (HasErrors)
            return;

        DepersistMemberImportTables();
        if (HasErrors)
            return;

        DepersistDataMappingsTable();
        if (HasErrors)
            return;

        DepersistConstantsTable();
        if (HasErrors)
            return;

        DepersistLineNumberTable();
        if (HasErrors)
            return;

        DepersistObjectSection();
    }

    private void DepersistTableOfContents()
    {
        _objectSectionStart = _reader.ReadUInt32();
        _objectSectionEnd = _reader.ReadUInt32();
        _lineNumberTableStart = _reader.ReadUInt32();
        _lineNumberTableEnd = _reader.ReadUInt32();

        string uri = _reader.ReadString();
        if (uri != null)
        {
            _binaryDataTableLoadResult = MarkupSystem.ResolveLoadResult(uri, _loadResultTarget.IslandReferences) as MarkupLoadResult;

            if (_binaryDataTableLoadResult != null)
            {
                _binaryDataTableLoadResult.Load(_currentDepersistPass);
                _binaryDataTable = _binaryDataTableLoadResult.BinaryDataTable;
                _loadResultTarget.SetBinaryDataTable(_binaryDataTable);

                _binaryDataTable.SharedDependenciesTableWithBinaryDataTable ??= new[] { _binaryDataTableLoadResult };
                _loadResultTarget.SetDependenciesTable(_binaryDataTable.SharedDependenciesTableWithBinaryDataTable);

                _usingSharedDataTable = true;
            }
            else
                MarkHasErrors();
        }
        else
            DepersistBinaryDataTable(_reader.ReadUInt32());
    }

    private void DepersistDependencies()
    {
        if (_usingSharedDataTable)
            return;

        ushort dependencyCount = _reader.ReadUInt16();
        LoadResult[] dependenciesTable = new LoadResult[dependencyCount];
        for (ushort dependencyIndex = 0; dependencyIndex < dependencyCount; ++dependencyIndex)
        {
            _reader.ReadBool();
            string uri = ReadDataTableString();

            LoadResult loadResult = MarkupSystem.ResolveLoadResult(uri, _loadResultTarget.IslandReferences);
            if (loadResult == null || loadResult.Status == LoadResultStatus.Error)
            {
                ReportError("Import of '{0}' failed", uri);
                return;
            }
            
            dependenciesTable[dependencyIndex] = loadResult;
        }

        _loadResultTarget.SetDependenciesTable(dependenciesTable);
    }

    private void DepersistTypeExportDeclarations()
    {
        ushort exportCount = _reader.ReadUInt16();
        TypeSchema[] exportTable = new TypeSchema[exportCount];
        for (int exportIndex = 0; exportIndex < exportCount; ++exportIndex)
        {
            string name = ReadDataTableString();
            MarkupType exportType = (MarkupType)_reader.ReadInt32();

            MarkupTypeSchema markupTypeSchema = MarkupTypeSchema.Build(TypeSchema.MarkupTypeToDefinition(exportType), _loadResultTarget, name);
            exportTable[exportIndex] = markupTypeSchema;
        }
        _loadResultTarget.SetExportTable(exportTable);
        
        ushort aliasCount = _reader.ReadUInt16();
        if (aliasCount <= 0)
            return;

        AliasMapping[] aliasTable = new AliasMapping[aliasCount];
        for (int aliasIndex = 0; aliasIndex < aliasCount; ++aliasIndex)
        {
            string alias = ReadDataTableString();

            ushort targetIndex = _reader.ReadUInt16();
            string targetType = ReadDataTableString();

            LoadResult dependent = MapIndexToDependent(targetIndex);
            aliasTable[aliasIndex] = new AliasMapping(alias, dependent, targetType);
        }
        _loadResultTarget.SetAliasTable(aliasTable);
    }

    private void DepersistTypeImportTable()
    {
        if (_usingSharedDataTable)
            return;

        MarkupImportTables importTables = new();
        _loadResultTarget.BinaryDataTable.SetImportTables(importTables);

        ushort typeCount = _reader.ReadUInt16();
        if (typeCount <= 0)
            return;

        TypeSchema[] typeSchemaArray = new TypeSchema[typeCount];
        for (ushort typeIndex = 0; typeIndex < typeCount; ++typeIndex)
        {
            LoadResult dependent = MapIndexToDependent(_reader.ReadUInt16());
            string name = ReadDataTableString();

            TypeSchema type = dependent.FindType(name);
            if (type == null)
                ReportError("Import of {0} named '{1}' from '{2}' failed", "type", name, dependent.Uri);
            else
                typeSchemaArray[typeIndex] = type;
        }

        importTables.TypeImports = typeSchemaArray;
    }

    private void DepersistTypeExportDefinitions()
    {
        TypeSchema[] typeImports = _loadResultTarget.ImportTables.TypeImports;
        foreach (MarkupTypeSchema markupTypeSchema in _loadResultTarget.ExportTable)
        {
            MarkupType markupType = markupTypeSchema.MarkupType;
            TypeSchema definition = TypeSchema.MarkupTypeToDefinition(markupType);

            uint typeDepth = _reader.ReadUInt16();
            markupTypeSchema.SetTypeDepth(typeDepth);
            if (typeDepth > 1U)
            {
                ushort baseTypeIndex = _reader.ReadUInt16();
                TypeSchema typeSchema = typeImports[baseTypeIndex];
                markupTypeSchema.SetBaseType((MarkupTypeSchema)typeSchema);
            }

            uint initPropertiesOffset = _reader.ReadUInt32();
            markupTypeSchema.SetInitializePropertiesOffset(initPropertiesOffset);

            uint initLocalsInputOffset = _reader.ReadUInt32();
            markupTypeSchema.SetInitializeLocalsInputOffset(initLocalsInputOffset);

            uint initContentOffset = _reader.ReadUInt32();
            markupTypeSchema.SetInitializeContentOffset(initContentOffset);

            markupTypeSchema.SetInitialEvaluateOffsets(ReadUInt32ArrayHelper());
            markupTypeSchema.SetFinalEvaluateOffsets(ReadUInt32ArrayHelper());
            markupTypeSchema.SetRefreshListenerGroupOffsets(ReadUInt32ArrayHelper());

            uint listenerCount = _reader.ReadUInt32();
            markupTypeSchema.SetListenerCount(listenerCount);

            uint symbolReferenceCount = _reader.ReadUInt32();
            if (symbolReferenceCount > 0U)
            {
                SymbolReference[] symbolTable = new SymbolReference[symbolReferenceCount];
                for (int referenceIndex = 0; referenceIndex < symbolReferenceCount; ++referenceIndex)
                {
                    SymbolReference symbolReference = new(ReadDataTableString(), (SymbolOrigin)_reader.ReadByte());
                    symbolTable[referenceIndex] = symbolReference;
                }
                markupTypeSchema.SetSymbolReferenceTable(symbolTable);
            }

            DepersistInheritedSymbolTable(markupTypeSchema);

            int totalPropertiesAndLocalsCount = _reader.ReadInt32();
            markupTypeSchema.SetTotalPropertiesAndLocalsCount(totalPropertiesAndLocalsCount);

            if (markupType == MarkupType.UI)
            {
                ushort namedContentCount = _reader.ReadUInt16();
                if (namedContentCount > 0)
                {
                    NamedContentRecord[] namedContentTable = new NamedContentRecord[namedContentCount];
                    for (int namedContentIndex = 0; namedContentIndex < namedContentCount; ++namedContentIndex)
                    {
                        string name = ReadDataTableString();
                        NamedContentRecord contentRecord = new(name);

                        uint contentOffset = _reader.ReadUInt32();
                        contentRecord.SetOffset(contentOffset);

                        namedContentTable[namedContentIndex] = contentRecord;
                    }

                    ((UIClassTypeSchema)markupTypeSchema).SetNamedContentTable(namedContentTable);
                }
            }
            else
            {
                bool shareable = _reader.ReadBool();
                if (shareable)
                    ((ClassTypeSchema)markupTypeSchema).MarkShareable();

                if (markupType == MarkupType.Effect)
                {
                    EffectClassTypeSchema effectClassTypeSchema = (EffectClassTypeSchema)markupTypeSchema;

                    effectClassTypeSchema.SetTechniqueOffsets(ReadUInt32ArrayHelper());
                    effectClassTypeSchema.SetInstancePropertyAssignments(ReadUInt32ArrayHelper());
                    effectClassTypeSchema.SetDynamicElementAssignments(ReadStringArrayHelper());

                    int symbolIndex = _reader.ReadInt32();
                    effectClassTypeSchema.SetDefaultElementSymbolIndex(symbolIndex);
                }
                if (markupType == MarkupType.DataQuery)
                {
                    MarkupDataQuerySchema markupDataQuerySchema = (MarkupDataQuerySchema)markupTypeSchema;

                    markupDataQuerySchema.ProviderName = ReadDataTableString();

                    ushort index = _reader.ReadUInt16();
                    markupDataQuerySchema.ResultType = MapIndexToType(index);
                }
            }

            ushort propertyCount = _reader.ReadUInt16();
            if (propertyCount > 0)
            {
                PropertySchema[] properties = new PropertySchema[propertyCount];
                for (int propertyIndex = 0; propertyIndex < propertyCount; ++propertyIndex)
                {
                    string name = ReadDataTableString();
                    bool requiredForCreation = _reader.ReadBool();

                    bool isConstrained = _reader.ReadBool();
                    PropertyOverrideCriteriaTypeConstraint criteriaTypeConstraint = null;
                    if (isConstrained)
                    {
                        ushort constraintTypeIndex = _reader.ReadUInt16();
                        TypeSchema constraint = typeImports[constraintTypeIndex];

                        ushort propertyTypeConstraintIndex = _reader.ReadUInt16();
                        criteriaTypeConstraint = new PropertyOverrideCriteriaTypeConstraint(typeImports[propertyTypeConstraintIndex], constraint);
                    }

                    ushort propertyTypeIndex = _reader.ReadUInt16();
                    TypeSchema propertyType = typeImports[propertyTypeIndex];

                    MarkupPropertySchema markupPropertySchema = MarkupPropertySchema.Build(definition, markupTypeSchema, name, propertyType);
                    markupPropertySchema.SetRequiredForCreation(requiredForCreation);
                    markupPropertySchema.SetOverrideCriteria(criteriaTypeConstraint);

                    if (markupType == MarkupType.DataType)
                    {
                        MarkupDataTypePropertySchema typePropertySchema = (MarkupDataTypePropertySchema)markupPropertySchema;

                        ushort alternateTypeIndex = _reader.ReadUInt16();
                        if (alternateTypeIndex != ushort.MaxValue)
                            typePropertySchema.SetUnderlyingCollectionType(MapIndexToType(alternateTypeIndex));
                    }
                    if (markupType == MarkupType.DataQuery)
                    {
                        MarkupDataQueryPropertySchema queryPropertySchema = (MarkupDataQueryPropertySchema)markupPropertySchema;

                        bool queryPropertyHasDefault = _reader.ReadBool();
                        if (queryPropertyHasDefault)
                            queryPropertySchema.DefaultValue = queryPropertySchema.PropertyType.DecodeBinary(_reader);

                        queryPropertySchema.InvalidatesQuery = _reader.ReadBool();

                        ushort alternateTypeIndex = _reader.ReadUInt16();
                        if (alternateTypeIndex != ushort.MaxValue)
                            queryPropertySchema.SetUnderlyingCollectionType(MapIndexToType(alternateTypeIndex));
                    }

                    properties[propertyIndex] = markupPropertySchema;
                }

                markupTypeSchema.SetPropertyList(properties);
            }

            MethodSchema[] methods = ReadMarkupMethodArrayHelper(definition, markupTypeSchema);
            if (methods != null)
                markupTypeSchema.SetMethodList(methods);

            MethodSchema[] virtualMethods = ReadMarkupMethodArrayHelper(definition, markupTypeSchema);
            if (virtualMethods != null)
                markupTypeSchema.SetVirtualMethodList(virtualMethods);
        }

        foreach (MarkupTypeSchema markupTypeSchema in _loadResultTarget.ExportTable)
            markupTypeSchema.BuildProperties();
    }

    private void DepersistMemberImportTables()
    {
        if (_usingSharedDataTable)
            return;

        MarkupImportTables importTables = _loadResultTarget.ImportTables;

        ushort constructorCount = _reader.ReadUInt16();
        if (constructorCount > 0)
        {
            ConstructorSchema[] constructorSchemaArray = new ConstructorSchema[constructorCount];
            for (int constructorIndex = 0; constructorIndex < constructorCount; ++constructorIndex)
            {
                TypeSchema type = MapIndexToType(_reader.ReadUInt16());

                ushort parameterCount = _reader.ReadUInt16();
                TypeSchema[] parameters = TypeSchema.EmptyList;
                if (parameterCount > 0)
                {
                    parameters = GetTempParameterArray(parameterCount);
                    for (ushort parameterIndex = 0; parameterIndex < parameterCount; ++parameterIndex)
                    {
                        ushort parameterTypeIndex = _reader.ReadUInt16();
                        parameters[parameterIndex] = MapIndexToType(parameterTypeIndex);
                    }
                }

                ConstructorSchema constructor = type.FindConstructor(parameters);
                if (constructor == null)
                    ReportError("Import of {0} named '{1}' from '{2}' failed", "constructor", type.Name, type.Owner.Uri);
                else
                    constructorSchemaArray[constructorIndex] = constructor;
            }
            importTables.ConstructorImports = constructorSchemaArray;
        }

        ushort propertyCount = _reader.ReadUInt16();
        if (propertyCount > 0)
        {
            PropertySchema[] propertySchemaArray = new PropertySchema[propertyCount];
            for (int propertyIndex = 0; propertyIndex < propertyCount; ++propertyIndex)
            {
                TypeSchema type = MapIndexToType(_reader.ReadUInt16());

                string name = ReadDataTableString();

                PropertySchema property = type.FindProperty(name);
                if (property == null)
                    ReportError("Import of {0} named '{1}' from '{2}' failed", "property", name, type.Name);
                else
                    propertySchemaArray[propertyIndex] = property;
            }
            importTables.PropertyImports = propertySchemaArray;
        }

        ushort methodCount = _reader.ReadUInt16();
        if (methodCount > 0)
        {
            MethodSchema[] methodSchemaArray = new MethodSchema[methodCount];
            for (int methodIndex = 0; methodIndex < methodCount; ++methodIndex)
            {
                MethodSchema methodSchema = null;
                TypeSchema type = MapIndexToType(_reader.ReadUInt16());

                bool isVirtual = _reader.ReadBool();
                if (!isVirtual)
                {
                    string name = ReadDataTableString();

                    ushort parameterCount = _reader.ReadUInt16();
                    TypeSchema[] parameters = TypeSchema.EmptyList;
                    if (parameterCount > 0)
                    {
                        parameters = GetTempParameterArray(parameterCount);
                        for (ushort parameterIndex = 0; parameterIndex < parameterCount; ++parameterIndex)
                        {
                            ushort parameterTypeIndex = _reader.ReadUInt16();
                            parameters[parameterIndex] = MapIndexToType(parameterTypeIndex);
                        }
                    }

                    methodSchema = type.FindMethod(name, parameters);
                    if (methodSchema == null)
                        ReportError("Import of {0} named '{1}' from '{2}' failed", "method", name, type.Name);
                }
                else
                {
                    int virtualId = _reader.ReadInt32();
                    if (type is MarkupTypeSchema markupTypeSchema)
                    {
                        foreach (MarkupMethodSchema virtualMethod in markupTypeSchema.VirtualMethods)
                        {
                            if (virtualMethod.VirtualId == virtualId)
                            {
                                methodSchema = virtualMethod;
                                break;
                            }
                        }
                    }
                    if (methodSchema == null)
                        ReportError("Import of virtual method with index {0} from '{1}' failed", virtualId.ToString(), type.Name);
                }
                methodSchemaArray[methodIndex] = methodSchema;
            }
            importTables.MethodImports = methodSchemaArray;
        }

        ushort eventCount = _reader.ReadUInt16();
        if (eventCount <= 0)
            return;

        EventSchema[] eventSchemaArray = new EventSchema[eventCount];
        for (int eventIndex = 0; eventIndex < eventCount; ++eventIndex)
        {
            TypeSchema type = MapIndexToType(_reader.ReadUInt16());
            string name = ReadDataTableString();

            EventSchema eventSchema = type.FindEvent(name);
            if (eventSchema == null)
                ReportError("Import of {0} named '{1}' from '{2}' failed", "event", name, type.Name);
            else
                eventSchemaArray[eventIndex] = eventSchema;
        }

        importTables.EventImports = eventSchemaArray;
    }

    private void DepersistDataMappingsTable()
    {
        ushort dataMapCount = _reader.ReadUInt16();
        if (dataMapCount <= 0)
            return;

        MarkupDataMapping[] dataMappingsTable = new MarkupDataMapping[dataMapCount];
        for (int dataMapIndex = 0; dataMapIndex < dataMapCount; ++dataMapIndex)
        {
            MarkupDataMapping markupDataMapping = new(null);

            ushort targetTypeIndex = _reader.ReadUInt16();
            markupDataMapping.TargetType = (MarkupDataTypeSchema)MapIndexToType(targetTypeIndex);
            markupDataMapping.Provider = ReadDataTableString();

            ushort entryCount = _reader.ReadUInt16();
            markupDataMapping.Mappings = new MarkupDataMappingEntry[entryCount];
            for (int entryIndex = 0; entryIndex < entryCount; ++entryIndex)
            {
                MarkupDataMappingEntry dataMappingEntry = new()
                {
                    Source = ReadDataTableString(),
                    Target = ReadDataTableString()
                };

                ushort entryPropertyIndex = _reader.ReadUInt16();
                dataMappingEntry.Property = (MarkupDataTypePropertySchema)_loadResultTarget.ImportTables.PropertyImports[entryPropertyIndex];

                bool hasCustomDefault = _reader.ReadBool();
                dataMappingEntry.DefaultValue = !hasCustomDefault
                    ? MarkupDataProvider.GetDefaultValueForType(dataMappingEntry.Property.PropertyType)
                    : dataMappingEntry.Property.PropertyType.DecodeBinary(_reader);

                markupDataMapping.Mappings[entryIndex] = dataMappingEntry;
            }
            dataMappingsTable[dataMapIndex] = markupDataMapping;

            if (Application.DebugSettings.GenerateDataMappingModels)
                Application.DebugSettings.DataMappingModels.Add(
                    new Debug.Data.DataMappingModel(markupDataMapping.Provider, markupDataMapping.TargetType.Name, markupDataMapping.GenerateModelCode())
                );
        }
        _loadResultTarget.SetDataMappingsTable(dataMappingsTable);
    }

    public static void DecodeInheritableSymbolTable(MarkupTypeSchema typeExport, ByteCodeReader reader, IntPtr startAddress)
    {
        if (reader == null)
        {
            uint size = ByteCodeReader.ReadUInt32(startAddress);
            reader = new ByteCodeReader(new IntPtr(startAddress.ToInt64() + 4L), size, false);
        }
        else
        {
            _ = reader.ReadUInt32();
        }

        MarkupLoadResult owner = typeExport.Owner as MarkupLoadResult;
        MarkupBinaryDataTable binaryDataTable = owner.BinaryDataTable;

        uint symbolCount = reader.ReadUInt32();
        SymbolRecord[] symbolTable;
        if (symbolCount > 0U)
        {
            symbolTable = new SymbolRecord[symbolCount];
            for (int index = 0; index < symbolCount; ++index)
            {
                SymbolRecord symbolRecord = new()
                {
                    Name = ReadDataTableString(reader, binaryDataTable),
                    SymbolOrigin = (SymbolOrigin)reader.ReadByte()
                };

                ushort ownerTypeIndex = reader.ReadUInt16();
                symbolRecord.Type = owner.ImportTables.TypeImports[ownerTypeIndex];

                symbolTable[index] = symbolRecord;
            }
        }
        else
            symbolTable = SymbolRecord.EmptyList;

        typeExport.SetInheritableSymbolsTable(symbolTable);
    }

    private void DepersistInheritedSymbolTable(MarkupTypeSchema typeExport)
    {
        if (_reader.IsInFixedMemory)
        {
            typeExport.SetAddressOfInheritableSymbolTable(_reader.CurrentAddress);
            uint tableLength = _reader.ReadUInt32();
            _reader.CurrentOffset += tableLength;
        }
        else
            DecodeInheritableSymbolTable(typeExport, _reader, IntPtr.Zero);
    }

    private void DepersistConstantsTable()
    {
        ushort constantCount = _reader.ReadUInt16();
        if (constantCount <= 0)
            return;

        object[] runtimeList = new object[constantCount];
        MarkupConstantsTable constantsTable = new(runtimeList);

        if (!_reader.IsInFixedMemory)
        {
            _reader.CurrentOffset += (uint)((constantCount + 1) * 4);
            for (int index = 0; index < constantCount; ++index)
            {
                object value = DepersistConstant(_reader, _loadResultTarget);
                runtimeList[index] = value;
            }
        }
        else
        {
            var sizeAddress = _reader.GetAddress(_reader.CurrentOffset + constantCount * 4U);
            ByteCodeReader constantsTableReader = new(_reader.CurrentAddress, ByteCodeReader.ReadUInt32(sizeAddress), false);
            constantsTable.SetConstantsTableReader(constantsTableReader, _loadResultTarget);
        }

        _loadResultTarget.BinaryDataTable.SetConstantsTable(constantsTable);
    }

    public static object DepersistConstant(ByteCodeReader reader, MarkupLoadResult loadResult)
    {
        ushort typeImportIndex = reader.ReadUInt16();
        TypeSchema typeImport = loadResult.ImportTables.TypeImports[typeImportIndex];

        object instance = null;
        MarkupConstantPersistMode constantPersistMode = (MarkupConstantPersistMode)reader.ReadByte();
        switch (constantPersistMode)
        {
            case MarkupConstantPersistMode.Binary:
                instance = typeImport.DecodeBinary(reader);
                break;
            case MarkupConstantPersistMode.FromString:
                string str = ReadDataTableString(reader, loadResult.BinaryDataTable);
                typeImport.TypeConverter(str, StringSchema.Type, out instance);
                break;
            case MarkupConstantPersistMode.Canonical:
                string name = ReadDataTableString(reader, loadResult.BinaryDataTable);
                instance = typeImport.FindCanonicalInstance(name);
                break;
        }
        return instance;
    }

    private static MarkupLineNumberTable DecodeLineNumberTable(ByteCodeReader reader)
    {
        ushort lineNumberCount = reader.ReadUInt16();

        ulong[] runtimeList = new ulong[lineNumberCount];
        for (int index = 0; index < lineNumberCount; ++index)
            runtimeList[index] = reader.ReadUInt64();

        return new MarkupLineNumberTable(runtimeList);
    }

    public static MarkupLineNumberTable DecodeLineNumberTable(IntPtr address)
    {
        uint size = ByteCodeReader.ReadUInt16(address) * c_LineNumberRecordSize + 2;
        return DecodeLineNumberTable(new ByteCodeReader(address, size, false));
    }

    private void DepersistLineNumberTable()
    {
        if (_reader.IsInFixedMemory)
        {
            _loadResultTarget.SetAddressOfLineNumberData(_reader.GetAddress(_lineNumberTableStart));
        }
        else
        {
            uint currentOffset = _reader.CurrentOffset;
            _reader.CurrentOffset = _lineNumberTableStart;
            _loadResultTarget.SetLineNumberTable(DecodeLineNumberTable(_reader));
            _reader.CurrentOffset = currentOffset;
        }
    }

    private void DepersistObjectSection()
    {
        uint sectionSize = _objectSectionEnd - _objectSectionStart;
        ByteCodeReader sectionReader;

        if (_reader.IsInFixedMemory)
        {
            sectionReader = new ByteCodeReader(_reader.GetAddress(_objectSectionStart), sectionSize, false);
        }
        else
        {
            ByteCodeWriter byteCodeWriter = new();
            byteCodeWriter.Write(_reader.GetAddress(_objectSectionStart), sectionSize);
            sectionReader = byteCodeWriter.CreateReader();
        }

        _loadResultTarget.SetObjectSection(sectionReader);
    }

    private void DepersistBinaryDataTable(uint binaryDataTableOffset)
    {
        uint currentOffset = _reader.CurrentOffset;
        _reader.CurrentOffset = binaryDataTableOffset;

        int stringCount = _reader.ReadInt32();
        _binaryDataTable = new MarkupBinaryDataTable(null, stringCount);

        if (!_reader.IsInFixedMemory)
        {
            _reader.CurrentOffset += (uint)((stringCount + 1) * 4);
            Vector<string> strings = _binaryDataTable.Strings;
            for (int index = 0; index < stringCount; ++index)
            {
                string str = _reader.ReadString();
                strings[index] = str;
            }
        }
        else
            _binaryDataTable.SetStringTableReader(new ByteCodeReader(_reader.CurrentAddress, ByteCodeReader.ReadUInt32(_reader.GetAddress(_reader.CurrentOffset + (uint)(stringCount * 4))), false));

        _loadResultTarget.SetBinaryDataTable(_binaryDataTable);
        _reader.CurrentOffset = currentOffset;
    }

    private LoadResult MapIndexToDependent(ushort index)
    {
        return index switch
        {
            65533 => _loadResultTarget,
            65534 => MarkupSystem.UIXGlobal,
            _ => _binaryDataTableLoadResult == null
                ? _loadResultTarget.Dependencies[index]
                : _binaryDataTableLoadResult.Dependencies[index],
        };
    }

    private TypeSchema MapIndexToType(ushort index) => _loadResultTarget.ImportTables.TypeImports[index];

    private string ReadDataTableString() => _binaryDataTable.GetStringByIndex(_reader.ReadInt32());

    private static string ReadDataTableString(ByteCodeReader reader, MarkupBinaryDataTable binaryDataTable)
    {
        int index = reader.ReadInt32();
        return binaryDataTable.GetStringByIndex(index);
    }

    private uint[] ReadUInt32ArrayHelper()
    {
        uint uintCount = _reader.ReadUInt32();
        if (uintCount <= 0U)
            return null;

        uint[] numArray = new uint[uintCount];
        for (int index = 0; index < uintCount; ++index)
            numArray[index] = _reader.ReadUInt32();

        return numArray;
    }

    private string[] ReadStringArrayHelper()
    {
        uint stringCount = _reader.ReadUInt32();
        if (stringCount <= 0U)
            return null;

        string[] strArray = new string[stringCount];
        for (int index = 0; index < stringCount; ++index)
            strArray[index] = ReadDataTableString();

        return strArray;
    }

    private MethodSchema[] ReadMarkupMethodArrayHelper(TypeSchema markupTypeDefinition, MarkupTypeSchema typeExport)
    {
        MethodSchema[] methodSchemaArray = null;
        ushort methodCount = _reader.ReadUInt16();
        if (methodCount > 0)
        {
            methodSchemaArray = new MethodSchema[methodCount];
            for (int index1 = 0; index1 < methodCount; ++index1)
            {
                string name = ReadDataTableString();
                TypeSchema type = MapIndexToType(_reader.ReadUInt16());
                uint num2 = _reader.ReadUInt32();
                TypeSchema[] parameterTypes = TypeSchema.EmptyList;
                if (num2 > 0U)
                {
                    parameterTypes = new TypeSchema[num2];
                    for (int index2 = 0; index2 < num2; ++index2)
                    {
                        ushort index3 = _reader.ReadUInt16();
                        parameterTypes[index2] = MapIndexToType(index3);
                    }
                }
                string[] parameterNames = ReadStringArrayHelper() ?? MarkupMethodSchema.s_emptyStringArray;
                int virtualId = _reader.ReadInt32();
                bool isVirtualThunk = _reader.ReadBool();
                uint codeOffset = _reader.ReadUInt32();
                MarkupMethodSchema markupMethodSchema = MarkupMethodSchema.Build(markupTypeDefinition, typeExport, name, type, parameterTypes, parameterNames, isVirtualThunk);
                markupMethodSchema.SetCodeOffset(codeOffset);
                markupMethodSchema.SetVirtualId(virtualId);
                methodSchemaArray[index1] = markupMethodSchema;
            }
        }
        return methodSchemaArray;
    }

    private TypeSchema[] GetTempParameterArray(int count)
    {
        _typeSchemaArrays ??= new TypeSchema[5][];
        if (count >= _typeSchemaArrays.Length)
            return new TypeSchema[count];

        int index = count - 1;
        if (_typeSchemaArrays[index] == null)
            _typeSchemaArrays[index] = new TypeSchema[count];

        return _typeSchemaArrays[index];
    }

    public void ReportError(string error, string param0, string param1, string param2) => ReportError(string.Format(error, param0, param1, param2));

    public void ReportError(string error, string param0, string param1) => ReportError(string.Format(error, param0, param1));

    public void ReportError(string error, string param0) => ReportError(string.Format(error, param0));

    public void ReportError(string error)
    {
        MarkHasErrors();
        ErrorManager.ReportError(error);
    }

    public void MarkHasErrors()
    {
        if (_hasErrors)
            return;
        _hasErrors = true;
        _loadResultTarget.MarkLoadFailed();
    }

    public bool HasErrors => _hasErrors;

    public static bool IsUIB(Resource resource) => resource.Length > 4U && ByteCodeReader.ReadUInt32(resource.Buffer) == UIB_FILE_MAGIC;
}
