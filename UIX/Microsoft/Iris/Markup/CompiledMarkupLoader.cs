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

namespace Microsoft.Iris.Markup
{
    internal class CompiledMarkupLoader
    {
        private const uint c_LineNumberRecordSize = 12;
        private CompiledMarkupLoadResult _loadResultTarget;
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
        {
            return new CompiledMarkupLoader(loadResult, resource);
        }

        private CompiledMarkupLoader(CompiledMarkupLoadResult loadResultTarget, Resource resource)
        {
            _loadResultTarget = loadResultTarget;
            _reader = new ByteCodeReader(resource.Buffer, resource.Length, false);
            if (resource is DllResource)
                _reader.MarkAsInFixedMemory();
            uint num1 = _reader.ReadUInt32();
            uint num2 = _reader.ReadUInt32();
            if (num1 != 440551765U)
                ReportError("Invalid compiled UIX file");
            if (num2 == 1012U)
                return;
            ReportError("Compiled UIX file '{0}' was compiled for the runtime with version {1}, but the current runtime is version {2}", resource.Uri, num2.ToString(), 1012U.ToString());
        }

        public void Depersist(LoadPass currentPass)
        {
            if (_currentDepersistPass >= currentPass)
                return;
            _currentDepersistPass = currentPass;
            if (_binaryDataTableLoadResult != null)
                _binaryDataTableLoadResult.Load(_currentDepersistPass);
            if (_currentDepersistPass == LoadPass.DeclareTypes)
                DoLoadPassDeclareTypes();
            else if (_currentDepersistPass == LoadPass.PopulatePublicModel)
                DoLoadPassPopulatePublicModel();
            else if (_currentDepersistPass == LoadPass.Full)
            {
                DoLoadPassFull();
            }
            else
            {
                foreach (MarkupTypeSchema markupTypeSchema in _loadResultTarget.ExportTable)
                    markupTypeSchema.Seal();
                _reader = null;
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
            if (!HasErrors)
                ;
        }

        private void DoLoadPassPopulatePublicModel()
        {
            if (HasErrors)
                return;
            DepersistTypeImportTable();
            if (HasErrors)
                return;
            DepersistTypeExportDefinitions();
            if (!HasErrors)
                ;
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
            if (!HasErrors)
                ;
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
                    if (_binaryDataTable.SharedDependenciesTableWithBinaryDataTable == null)
                        _binaryDataTable.SharedDependenciesTableWithBinaryDataTable = new LoadResult[1]
                        {
               _binaryDataTableLoadResult
                        };
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
            ushort num = _reader.ReadUInt16();
            LoadResult[] dependenciesTable = new LoadResult[num];
            for (ushort index = 0; index < num; ++index)
            {
                _reader.ReadBool();
                string uri = ReadDataTableString();
                LoadResult loadResult = MarkupSystem.ResolveLoadResult(uri, _loadResultTarget.IslandReferences);
                if (loadResult == null || loadResult.Status == LoadResultStatus.Error)
                {
                    ReportError("Import of '{0}' failed", uri);
                    return;
                }
                dependenciesTable[index] = loadResult;
            }
            _loadResultTarget.SetDependenciesTable(dependenciesTable);
        }

        private void DepersistTypeExportDeclarations()
        {
            ushort num1 = _reader.ReadUInt16();
            TypeSchema[] exportTable = new TypeSchema[num1];
            for (int index = 0; index < num1; ++index)
            {
                string name = ReadDataTableString();
                MarkupTypeSchema markupTypeSchema = MarkupTypeSchema.Build(MarkupTypeToDefinition((MarkupType)_reader.ReadInt32()), _loadResultTarget, name);
                exportTable[index] = markupTypeSchema;
            }
            _loadResultTarget.SetExportTable(exportTable);
            ushort num2 = _reader.ReadUInt16();
            if (num2 <= 0)
                return;
            AliasMapping[] aliasTable = new AliasMapping[num2];
            for (int index1 = 0; index1 < num2; ++index1)
            {
                string alias = ReadDataTableString();
                ushort index2 = _reader.ReadUInt16();
                string targetType = ReadDataTableString();
                LoadResult dependent = MapIndexToDependent(index2);
                aliasTable[index1] = new AliasMapping(alias, dependent, targetType);
            }
            _loadResultTarget.SetAliasTable(aliasTable);
        }

        private void DepersistTypeImportTable()
        {
            if (_usingSharedDataTable)
                return;
            MarkupImportTables importTables = new MarkupImportTables();
            _loadResultTarget.BinaryDataTable.SetImportTables(importTables);
            ushort num = _reader.ReadUInt16();
            if (num <= 0)
                return;
            TypeSchema[] typeSchemaArray = new TypeSchema[num];
            for (ushort index = 0; index < num; ++index)
            {
                LoadResult dependent = MapIndexToDependent(_reader.ReadUInt16());
                string name = ReadDataTableString();
                TypeSchema type = dependent.FindType(name);
                if (type == null)
                    ReportError("Import of {0} named '{1}' from '{2}' failed", "type", name, dependent.Uri);
                else
                    typeSchemaArray[index] = type;
            }
            importTables.TypeImports = typeSchemaArray;
        }

        private void DepersistTypeExportDefinitions()
        {
            TypeSchema[] typeImports = _loadResultTarget.ImportTables.TypeImports;
            foreach (MarkupTypeSchema markupTypeSchema in _loadResultTarget.ExportTable)
            {
                MarkupType markupType = markupTypeSchema.MarkupType;
                TypeSchema definition = MarkupTypeToDefinition(markupType);
                uint typeDepth = _reader.ReadUInt16();
                markupTypeSchema.SetTypeDepth(typeDepth);
                if (typeDepth > 1U)
                {
                    ushort num = _reader.ReadUInt16();
                    TypeSchema typeSchema = typeImports[num];
                    markupTypeSchema.SetBaseType((MarkupTypeSchema)typeSchema);
                }
                uint offset1 = _reader.ReadUInt32();
                markupTypeSchema.SetInitializePropertiesOffset(offset1);
                uint offset2 = _reader.ReadUInt32();
                markupTypeSchema.SetInitializeLocalsInputOffset(offset2);
                uint offset3 = _reader.ReadUInt32();
                markupTypeSchema.SetInitializeContentOffset(offset3);
                markupTypeSchema.SetInitialEvaluateOffsets(ReadUInt32ArrayHelper());
                markupTypeSchema.SetFinalEvaluateOffsets(ReadUInt32ArrayHelper());
                markupTypeSchema.SetRefreshListenerGroupOffsets(ReadUInt32ArrayHelper());
                uint listenerCount = _reader.ReadUInt32();
                markupTypeSchema.SetListenerCount(listenerCount);
                uint num1 = _reader.ReadUInt32();
                if (num1 > 0U)
                {
                    SymbolReference[] symbolTable = new SymbolReference[num1];
                    for (int index = 0; index < num1; ++index)
                    {
                        SymbolReference symbolReference = new SymbolReference(ReadDataTableString(), (SymbolOrigin)_reader.ReadByte());
                        symbolTable[index] = symbolReference;
                    }
                    markupTypeSchema.SetSymbolReferenceTable(symbolTable);
                }
                DepersistInheritedSymbolTable(markupTypeSchema);
                int totalPropertiesAndLocalsCount = _reader.ReadInt32();
                markupTypeSchema.SetTotalPropertiesAndLocalsCount(totalPropertiesAndLocalsCount);
                if (markupType == MarkupType.UI)
                {
                    ushort num2 = _reader.ReadUInt16();
                    if (num2 > 0)
                    {
                        NamedContentRecord[] namedContentTable = new NamedContentRecord[num2];
                        for (int index = 0; index < num2; ++index)
                        {
                            string name = ReadDataTableString();
                            uint offset4 = _reader.ReadUInt32();
                            namedContentTable[index] = new NamedContentRecord(name);
                            namedContentTable[index].SetOffset(offset4);
                        }
                      ((UIClassTypeSchema)markupTypeSchema).SetNamedContentTable(namedContentTable);
                    }
                }
                else
                {
                    if (_reader.ReadBool())
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
                ushort num3 = _reader.ReadUInt16();
                if (num3 > 0)
                {
                    PropertySchema[] properties = new PropertySchema[num3];
                    for (int index1 = 0; index1 < num3; ++index1)
                    {
                        string name = ReadDataTableString();
                        bool requiredForCreation = _reader.ReadBool();
                        bool flag = _reader.ReadBool();
                        PropertyOverrideCriteriaTypeConstraint criteriaTypeConstraint = null;
                        if (flag)
                        {
                            ushort num2 = _reader.ReadUInt16();
                            ushort num4 = _reader.ReadUInt16();
                            TypeSchema constraint = typeImports[num2];
                            criteriaTypeConstraint = new PropertyOverrideCriteriaTypeConstraint(typeImports[num4], constraint);
                        }
                        ushort num5 = _reader.ReadUInt16();
                        TypeSchema propertyType = typeImports[num5];
                        MarkupPropertySchema markupPropertySchema = MarkupPropertySchema.Build(definition, markupTypeSchema, name, propertyType);
                        markupPropertySchema.SetRequiredForCreation(requiredForCreation);
                        markupPropertySchema.SetOverrideCriteria(criteriaTypeConstraint);
                        if (markupType == MarkupType.DataType)
                        {
                            MarkupDataTypePropertySchema typePropertySchema = (MarkupDataTypePropertySchema)markupPropertySchema;
                            ushort index2 = _reader.ReadUInt16();
                            if (index2 != ushort.MaxValue)
                                typePropertySchema.SetUnderlyingCollectionType(MapIndexToType(index2));
                        }
                        if (markupType == MarkupType.DataQuery)
                        {
                            MarkupDataQueryPropertySchema queryPropertySchema = (MarkupDataQueryPropertySchema)markupPropertySchema;
                            if (_reader.ReadBool())
                                queryPropertySchema.DefaultValue = queryPropertySchema.PropertyType.DecodeBinary(_reader);
                            queryPropertySchema.InvalidatesQuery = _reader.ReadBool();
                            ushort index2 = _reader.ReadUInt16();
                            if (index2 != ushort.MaxValue)
                                queryPropertySchema.SetUnderlyingCollectionType(MapIndexToType(index2));
                        }
                        properties[index1] = markupPropertySchema;
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
            ushort num1 = _reader.ReadUInt16();
            if (num1 > 0)
            {
                ConstructorSchema[] constructorSchemaArray = new ConstructorSchema[num1];
                for (int index1 = 0; index1 < num1; ++index1)
                {
                    TypeSchema type = MapIndexToType(_reader.ReadUInt16());
                    ushort num2 = _reader.ReadUInt16();
                    TypeSchema[] parameters = TypeSchema.EmptyList;
                    if (num2 > 0)
                    {
                        parameters = GetTempParameterArray(num2);
                        for (ushort index2 = 0; index2 < num2; ++index2)
                        {
                            ushort index3 = _reader.ReadUInt16();
                            parameters[index2] = MapIndexToType(index3);
                        }
                    }
                    ConstructorSchema constructor = type.FindConstructor(parameters);
                    if (constructor == null)
                        ReportError("Import of {0} named '{1}' from '{2}' failed", "constructor", type.Name, type.Owner.Uri);
                    else
                        constructorSchemaArray[index1] = constructor;
                }
                importTables.ConstructorImports = constructorSchemaArray;
            }
            ushort num3 = _reader.ReadUInt16();
            if (num3 > 0)
            {
                PropertySchema[] propertySchemaArray = new PropertySchema[num3];
                for (int index = 0; index < num3; ++index)
                {
                    TypeSchema type = MapIndexToType(_reader.ReadUInt16());
                    string name = ReadDataTableString();
                    PropertySchema property = type.FindProperty(name);
                    if (property == null)
                        ReportError("Import of {0} named '{1}' from '{2}' failed", "property", name, type.Name);
                    else
                        propertySchemaArray[index] = property;
                }
                importTables.PropertyImports = propertySchemaArray;
            }
            ushort num4 = _reader.ReadUInt16();
            if (num4 > 0)
            {
                MethodSchema[] methodSchemaArray = new MethodSchema[num4];
                for (int index1 = 0; index1 < num4; ++index1)
                {
                    MethodSchema methodSchema = null;
                    TypeSchema type = MapIndexToType(_reader.ReadUInt16());
                    if (!_reader.ReadBool())
                    {
                        string name = ReadDataTableString();
                        ushort num2 = _reader.ReadUInt16();
                        TypeSchema[] parameters = TypeSchema.EmptyList;
                        if (num2 > 0)
                        {
                            parameters = GetTempParameterArray(num2);
                            for (ushort index2 = 0; index2 < num2; ++index2)
                            {
                                ushort index3 = _reader.ReadUInt16();
                                parameters[index2] = MapIndexToType(index3);
                            }
                        }
                        methodSchema = type.FindMethod(name, parameters);
                        if (methodSchema == null)
                            ReportError("Import of {0} named '{1}' from '{2}' failed", "method", name, type.Name);
                    }
                    else
                    {
                        int num2 = _reader.ReadInt32();
                        if (type is MarkupTypeSchema markupTypeSchema)
                        {
                            foreach (MarkupMethodSchema virtualMethod in markupTypeSchema.VirtualMethods)
                            {
                                if (virtualMethod.VirtualId == num2)
                                {
                                    methodSchema = virtualMethod;
                                    break;
                                }
                            }
                        }
                        if (methodSchema == null)
                            ReportError("Import of virtual method with index {0} from '{1}' failed", num2.ToString(), type.Name);
                    }
                    methodSchemaArray[index1] = methodSchema;
                }
                importTables.MethodImports = methodSchemaArray;
            }
            ushort num5 = _reader.ReadUInt16();
            if (num5 <= 0)
                return;
            EventSchema[] eventSchemaArray = new EventSchema[num5];
            for (int index = 0; index < num5; ++index)
            {
                TypeSchema type = MapIndexToType(_reader.ReadUInt16());
                string name = ReadDataTableString();
                EventSchema eventSchema = type.FindEvent(name);
                if (eventSchema == null)
                    ReportError("Import of {0} named '{1}' from '{2}' failed", "event", name, type.Name);
                else
                    eventSchemaArray[index] = eventSchema;
            }
            importTables.EventImports = eventSchemaArray;
        }

        private void DepersistDataMappingsTable()
        {
            ushort num1 = _reader.ReadUInt16();
            if (num1 <= 0)
                return;
            MarkupDataMapping[] dataMappingsTable = new MarkupDataMapping[num1];
            for (int index1 = 0; index1 < num1; ++index1)
            {
                MarkupDataMapping markupDataMapping = new MarkupDataMapping(null);
                ushort index2 = _reader.ReadUInt16();
                markupDataMapping.TargetType = (MarkupDataTypeSchema)MapIndexToType(index2);
                markupDataMapping.Provider = ReadDataTableString();
                ushort num2 = _reader.ReadUInt16();
                markupDataMapping.Mappings = new MarkupDataMappingEntry[num2];
                for (int index3 = 0; index3 < num2; ++index3)
                {
                    MarkupDataMappingEntry dataMappingEntry = new MarkupDataMappingEntry();
                    dataMappingEntry.Source = ReadDataTableString();
                    dataMappingEntry.Target = ReadDataTableString();
                    ushort num3 = _reader.ReadUInt16();
                    dataMappingEntry.Property = (MarkupDataTypePropertySchema)_loadResultTarget.ImportTables.PropertyImports[num3];
                    dataMappingEntry.DefaultValue = !_reader.ReadBool() ? MarkupDataProvider.GetDefaultValueForType(dataMappingEntry.Property.PropertyType) : dataMappingEntry.Property.PropertyType.DecodeBinary(_reader);
                    markupDataMapping.Mappings[index3] = dataMappingEntry;
                }
                dataMappingsTable[index1] = markupDataMapping;

#if OPENZUNE
                if (Application.DebugSettings.GenerateDataMappingModels)
                    Application.DebugSettings.DataMappingModels.Add(
                        (markupDataMapping.Provider, markupDataMapping.TargetType.Name, markupDataMapping.GenerateModelCode())
                    );
#endif
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
                int num1 = (int)reader.ReadUInt32();
            }
            MarkupLoadResult owner = typeExport.Owner as MarkupLoadResult;
            MarkupBinaryDataTable binaryDataTable = owner.BinaryDataTable;
            uint num2 = reader.ReadUInt32();
            SymbolRecord[] symbolTable;
            if (num2 > 0U)
            {
                symbolTable = new SymbolRecord[num2];
                for (int index = 0; index < num2; ++index)
                {
                    SymbolRecord symbolRecord = new SymbolRecord();
                    symbolRecord.Name = ReadDataTableString(reader, binaryDataTable);
                    symbolRecord.SymbolOrigin = (SymbolOrigin)reader.ReadByte();
                    ushort num3 = reader.ReadUInt16();
                    symbolRecord.Type = owner.ImportTables.TypeImports[num3];
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
                uint num = _reader.ReadUInt32();
                _reader.CurrentOffset += num;
            }
            else
                DecodeInheritableSymbolTable(typeExport, _reader, IntPtr.Zero);
        }

        private void DepersistConstantsTable()
        {
            ushort num = _reader.ReadUInt16();
            if (num <= 0)
                return;
            object[] runtimeList = new object[num];
            MarkupConstantsTable constantsTable = new MarkupConstantsTable(runtimeList);
            if (!_reader.IsInFixedMemory)
            {
                _reader.CurrentOffset += (uint)((num + 1) * 4);
                for (int index = 0; index < num; ++index)
                {
                    object obj = DepersistConstant(_reader, _loadResultTarget);
                    runtimeList[index] = obj;
                }
            }
            else
            {
                ByteCodeReader constantsTableReader = new ByteCodeReader(_reader.CurrentAddress, ByteCodeReader.ReadUInt32(_reader.GetAddress(_reader.CurrentOffset + num * 4U)), false);
                constantsTable.SetConstantsTableReader(constantsTableReader, _loadResultTarget);
            }
            _loadResultTarget.BinaryDataTable.SetConstantsTable(constantsTable);
        }

        public static object DepersistConstant(ByteCodeReader reader, MarkupLoadResult loadResult)
        {
            ushort num = reader.ReadUInt16();
            TypeSchema typeImport = loadResult.ImportTables.TypeImports[num];
            MarkupConstantPersistMode constantPersistMode = (MarkupConstantPersistMode)reader.ReadByte();
            object instance = null;
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
            ushort num = reader.ReadUInt16();
            ulong[] runtimeList = new ulong[num];
            for (int index = 0; index < num; ++index)
                runtimeList[index] = reader.ReadUInt64();
            return new MarkupLineNumberTable(runtimeList);
        }

        public static MarkupLineNumberTable DecodeLineNumberTable(IntPtr address)
        {
            uint size = (uint)(ByteCodeReader.ReadUInt16(address) * 12 + 2);
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
            uint num = _objectSectionEnd - _objectSectionStart;
            ByteCodeReader reader;
            if (_reader.IsInFixedMemory)
            {
                reader = new ByteCodeReader(_reader.GetAddress(_objectSectionStart), num, false);
            }
            else
            {
                ByteCodeWriter byteCodeWriter = new ByteCodeWriter();
                byteCodeWriter.Write(_reader.GetAddress(_objectSectionStart), num);
                reader = byteCodeWriter.CreateReader();
            }
            _loadResultTarget.SetObjectSection(reader);
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
            LoadResult loadResult;
            switch (index)
            {
                case 65533:
                    loadResult = _loadResultTarget;
                    break;
                case 65534:
                    loadResult = MarkupSystem.UIXGlobal;
                    break;
                default:
                    loadResult = _binaryDataTableLoadResult == null ? _loadResultTarget.Dependencies[index] : _binaryDataTableLoadResult.Dependencies[index];
                    break;
            }
            return loadResult;
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
            uint num = _reader.ReadUInt32();
            if (num <= 0U)
                return null;
            uint[] numArray = new uint[num];
            for (int index = 0; index < num; ++index)
                numArray[index] = _reader.ReadUInt32();
            return numArray;
        }

        private string[] ReadStringArrayHelper()
        {
            uint num = _reader.ReadUInt32();
            if (num <= 0U)
                return null;
            string[] strArray = new string[num];
            for (int index = 0; index < num; ++index)
                strArray[index] = ReadDataTableString();
            return strArray;
        }

        private MethodSchema[] ReadMarkupMethodArrayHelper(TypeSchema markupTypeDefinition, MarkupTypeSchema typeExport)
        {
            MethodSchema[] methodSchemaArray = null;
            ushort num1 = _reader.ReadUInt16();
            if (num1 > 0)
            {
                methodSchemaArray = new MethodSchema[num1];
                for (int index1 = 0; index1 < num1; ++index1)
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

        private TypeSchema MarkupTypeToDefinition(MarkupType markupType)
        {
            switch (markupType)
            {
                case MarkupType.UI:
                    return UISchema.Type;
                case MarkupType.Effect:
                    return EffectSchema.Type;
                case MarkupType.DataType:
                    return DataTypeSchema.Type;
                case MarkupType.DataQuery:
                    return DataQuerySchema.Type;
                default:
                    return ClassSchema.Type;
            }
        }

        private TypeSchema[] GetTempParameterArray(int count)
        {
            if (_typeSchemaArrays == null)
                _typeSchemaArrays = new TypeSchema[5][];
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

        public static bool IsUIB(Resource resource)
        {
            bool flag = false;
            if (resource.Length > 4U && ByteCodeReader.ReadUInt32(resource.Buffer) == 440551765U)
                flag = true;
            return flag;
        }
    }
}
