// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupCompiler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Markup
{
    public class MarkupCompiler
    {
        private ByteCodeWriter _writer;
        private MarkupLoadResult _loadResult;
        private MarkupBinaryDataTable _binaryDataTable;
        private bool _usingSharedBinaryDataTable;
        private uint _objectSectionStartFixup;
        private uint _objectSectionEndFixup;
        private uint _lineNumberTableStartFixup;
        private uint _lineNumberTableEndFixup;
        private uint _binaryDataTableSectionOffsetFixup = uint.MaxValue;
        private bool _binaryDataTablePersisted;

        private MarkupCompiler()
        {
        }

        public static bool Compile(CompilerInput[] compilands, CompilerInput dataTableCompiland)
        {
            ErrorWatermark watermark1 = ErrorManager.Watermark;
            MarkupBinaryDataTable markupBinaryDataTable = null;
            if (dataTableCompiland.SourceFileName != null)
            {
                markupBinaryDataTable = new MarkupBinaryDataTable(dataTableCompiland.SourceFileName);
                MarkupConstantsTable constantsTable = new MarkupConstantsTable();
                markupBinaryDataTable.SetConstantsTable(constantsTable);
                markupBinaryDataTable.SetSourceMarkupImportTables(new SourceMarkupImportTables());
            }
            Vector vector = new Vector();
            foreach (CompilerInput compiland in compilands)
            {
                if (compiland.SourceFileName.IndexOf("://", StringComparison.Ordinal) != -1)
                    ErrorManager.ReportError($"'{compiland.SourceFileName}' is not a valid filename");
                string uri = "file://" + compiland.SourceFileName;
                LoadResult loadResult = MarkupSystem.ResolveLoadResult(uri, MarkupSystem.RootIslandId);
                if (loadResult != null && loadResult.Status != LoadResultStatus.Error)
                {
                    if (!(loadResult is MarkupLoadResult markupLoadResult) || !markupLoadResult.IsSource)
                    {
                        ErrorManager.ReportError($"'{uri}' is not markup, it cannot be compiled");
                    }
                    else
                    {
                        vector.Add(loadResult);
                        if (compiland.IdentityUri != null)
                            loadResult.SetCompilerReferenceName(compiland.IdentityUri);
                        if (markupBinaryDataTable != null)
                            markupLoadResult.SetBinaryDataTable(markupBinaryDataTable);
                    }
                }
            }
            foreach (LoadResult loadResult in vector)
                MarkupSystem.Load(loadResult.Uri, MarkupSystem.RootIslandId);
            if (!watermark1.ErrorsDetected)
            {
                for (int index = 0; index < compilands.Length; ++index)
                {
                    ErrorWatermark watermark2 = ErrorManager.Watermark;
                    ByteCodeWriter writer = Run((MarkupLoadResult)vector[index], markupBinaryDataTable);
                    if (!watermark2.ErrorsDetected)
                        SaveCompiledOutput(writer, compilands[index].OutputFileName);
                }
                if (markupBinaryDataTable != null)
                    SaveCompiledOutput(CompileBinaryDataTable(markupBinaryDataTable), dataTableCompiland.OutputFileName);
            }
            return !watermark1.ErrorsDetected;
        }

        private static void SaveCompiledOutput(ByteCodeWriter writer, string outputFile)
        {
            ErrorWatermark watermark = ErrorManager.Watermark;
            IntPtr invalidHandleValue = Win32Api.INVALID_HANDLE_VALUE;
            ByteCodeReader reader = writer.CreateReader();
            reader.DeclareOwner(typeof(MarkupSystem));
            IntPtr file = Win32Api.CreateFile(outputFile, 1073741824U, 0U, IntPtr.Zero, 2U, 0U, IntPtr.Zero);
            if (file == Win32Api.INVALID_HANDLE_VALUE)
                ErrorManager.ReportError("Unable to open output file '{0}'.  Error code {1}", outputFile, Marshal.GetLastWin32Error());
            if (!watermark.ErrorsDetected)
            {
                uint size = 0;
                IntPtr intPtr = reader.ToIntPtr(out size);
                uint lpNumberOfBytesWritten = 0;
                if (!Win32Api.WriteFile(file, intPtr, size, out lpNumberOfBytesWritten, IntPtr.Zero))
                    ErrorManager.ReportError("An error occurred while saving data to output file '{0}'.  Error code {1}", outputFile, Marshal.GetLastWin32Error());
            }
            if (file != Win32Api.INVALID_HANDLE_VALUE)
                Win32Api.CloseHandle(file);
            reader?.Dispose(typeof(MarkupSystem));
        }

        public static ByteCodeWriter Run(
          MarkupLoadResult loadResult,
          MarkupBinaryDataTable sharedBinaryDataTable)
        {
            return new MarkupCompiler().InternalRun(loadResult, sharedBinaryDataTable);
        }

        private ByteCodeWriter InternalRun(
          MarkupLoadResult loadResult,
          MarkupBinaryDataTable sharedBinaryDataTable)
        {
            _writer = new ByteCodeWriter();
            _loadResult = loadResult;
            PersistHeader();
            PersistTableOfContents(sharedBinaryDataTable);
            PersistDependencies();
            PersistTypeExportDeclarations();
            PersistTypeImportTable();
            PersistTypeExportDefinitions();
            PersistMemberImportTables();
            PersistDataMappingsTable();
            PersistConstantsTable();
            PersistBinaryDataTable();
            PersistObjectSection();
            PersistLineNumberTable();
            return _writer;
        }

        private void PersistHeader()
        {
            _writer.WriteUInt32(0x1A_42_49_55U);
            _writer.WriteUInt32(0x00_00_03_F4U);
        }

        private void PersistTableOfContents(MarkupBinaryDataTable sharedBinaryDataTable)
        {
            _objectSectionStartFixup = _writer.DataSize;
            _writer.WriteUInt32(uint.MaxValue);
            _objectSectionEndFixup = _writer.DataSize;
            _writer.WriteUInt32(uint.MaxValue);
            _lineNumberTableStartFixup = _writer.DataSize;
            _writer.WriteUInt32(uint.MaxValue);
            _lineNumberTableEndFixup = _writer.DataSize;
            _writer.WriteUInt32(uint.MaxValue);
            if (sharedBinaryDataTable != null)
            {
                _binaryDataTable = sharedBinaryDataTable;
                _writer.WriteString(_binaryDataTable.Uri);
                _usingSharedBinaryDataTable = true;
            }
            else
            {
                _writer.WriteString(null);
                _usingSharedBinaryDataTable = false;
                _binaryDataTable = _loadResult.BinaryDataTable;
                if (_binaryDataTable == null)
                    _binaryDataTable = new MarkupBinaryDataTable(null, 0);
                _binaryDataTableSectionOffsetFixup = _writer.DataSize;
                _writer.WriteUInt32(uint.MaxValue);
            }
        }

        private void PersistDependencies()
        {
            if (_usingSharedBinaryDataTable)
                return;
            _writer.WriteUInt16(_loadResult.Dependencies.Length);
            foreach (LoadResult dependency in _loadResult.Dependencies)
            {
                _writer.WriteBool(dependency is MarkupLoadResult);
                WriteDataTableString(dependency.GetCompilerReferenceName() ?? dependency.Uri);
            }
        }

        private void PersistTypeExportDeclarations()
        {
            _writer.WriteUInt16(_loadResult.ExportTable.Length);
            foreach (MarkupTypeSchema markupTypeSchema in _loadResult.ExportTable)
            {
                MarkupType markupType = markupTypeSchema.MarkupType;
                WriteDataTableString(markupTypeSchema.Name);
                _writer.WriteInt32((int)markupType);
            }
            if (_loadResult.AliasTable == null)
            {
                _writer.WriteUInt16(0);
            }
            else
            {
                _writer.WriteUInt16(_loadResult.AliasTable.Length);
                foreach (AliasMapping aliasMapping in _loadResult.AliasTable)
                {
                    WriteDataTableString(aliasMapping.Alias);
                    _writer.WriteUInt16(MapDependentToIndex(aliasMapping.LoadResult));
                    WriteDataTableString(aliasMapping.TargetType);
                }
            }
        }

        private void PersistTypeImportTable()
        {
            if (_usingSharedBinaryDataTable)
                return;
            _writer.WriteUInt16(_loadResult.ImportTables.TypeImports.Length);
            foreach (TypeSchema typeImport in _loadResult.ImportTables.TypeImports)
            {
                _writer.WriteUInt16(MapDependentToIndex(typeImport.Owner));
                WriteDataTableString(typeImport.Name);
            }
        }

        private void PersistTypeExportDefinitions()
        {
            foreach (MarkupTypeSchema markupTypeSchema in _loadResult.ExportTable)
            {
                MarkupType markupType = markupTypeSchema.MarkupType;
                _writer.WriteUInt16(markupTypeSchema.TypeDepth);
                if (markupTypeSchema.TypeDepth > 1U)
                    _writer.WriteUInt16(MapTypeToIndex(markupTypeSchema.Base));
                _writer.WriteUInt32(markupTypeSchema.InitializePropertiesOffset);
                _writer.WriteUInt32(markupTypeSchema.InitializeLocalsInputOffset);
                _writer.WriteUInt32(markupTypeSchema.InitializeContentOffset);
                WriteUInt32ArrayHelper(markupTypeSchema.InitialEvaluateOffsets, "InitialEvaluateOffsets");
                WriteUInt32ArrayHelper(markupTypeSchema.FinalEvaluateOffsets, "FinalEvaluateOffsets");
                WriteUInt32ArrayHelper(markupTypeSchema.RefreshGroupOffsets, "RefreshGroupOffsets");
                _writer.WriteUInt32(markupTypeSchema.ListenerCount);
                uint num1 = 0;
                if (markupTypeSchema.SymbolReferenceTable != null)
                    num1 = (uint)markupTypeSchema.SymbolReferenceTable.Length;
                _writer.WriteUInt32(num1);
                if (num1 != 0U)
                {
                    foreach (SymbolReference symbolReference in markupTypeSchema.SymbolReferenceTable)
                    {
                        WriteDataTableString(symbolReference.Symbol);
                        _writer.WriteByte((byte)symbolReference.Origin);
                    }
                }
                uint dataSize1 = _writer.DataSize;
                _writer.WriteUInt32(uint.MaxValue);
                uint dataSize2 = _writer.DataSize;
                uint num2 = 0;
                if (markupTypeSchema.InheritableSymbolsTable != null)
                    num2 = (uint)markupTypeSchema.InheritableSymbolsTable.Length;
                _writer.WriteUInt32(num2);
                if (num2 != 0U)
                {
                    foreach (SymbolRecord symbolRecord in markupTypeSchema.InheritableSymbolsTable)
                    {
                        WriteDataTableString(symbolRecord.Name);
                        _writer.WriteByte((byte)symbolRecord.SymbolOrigin);
                        _writer.WriteUInt16(MapTypeToIndex(symbolRecord.Type));
                    }
                }
                uint num3 = _writer.DataSize - dataSize2;
                _writer.Overwrite(dataSize1, num3);
                _writer.WriteInt32(markupTypeSchema.TotalPropertiesAndLocalsCount);
                if (markupType == MarkupType.UI)
                {
                    UIClassTypeSchema uiClassTypeSchema = (UIClassTypeSchema)markupTypeSchema;
                    ushort num4 = 0;
                    if (uiClassTypeSchema.NamedContentTable != null)
                        num4 = (ushort)uiClassTypeSchema.NamedContentTable.Length;
                    _writer.WriteUInt16(num4);
                    if (num4 != 0)
                    {
                        foreach (NamedContentRecord namedContentRecord in uiClassTypeSchema.NamedContentTable)
                        {
                            WriteDataTableString(namedContentRecord.Name);
                            _writer.WriteUInt32(namedContentRecord.Offset);
                        }
                    }
                }
                else
                {
                    _writer.WriteBool(((ClassTypeSchema)markupTypeSchema).IsShared);
                    if (markupType == MarkupType.Effect)
                    {
                        EffectClassTypeSchema effectClassTypeSchema = (EffectClassTypeSchema)markupTypeSchema;
                        WriteUInt32ArrayHelper(effectClassTypeSchema.TechniqueOffsets, "TechniqueOffsets");
                        WriteUInt32ArrayHelper(effectClassTypeSchema.InstancePropertyAssignments, "InstancePropertyAssignments");
                        WriteStringArrayHelper(effectClassTypeSchema.DynamicElementAssignments, "DynamicElementAssignments");
                        _writer.WriteInt32(effectClassTypeSchema.DefaultElementSymbolIndex);
                    }
                    if (markupType == MarkupType.DataQuery)
                    {
                        MarkupDataQuerySchema markupDataQuerySchema = (MarkupDataQuerySchema)markupTypeSchema;
                        WriteDataTableString(markupDataQuerySchema.ProviderName);
                        _writer.WriteUInt16(MapTypeToIndex(markupDataQuerySchema.ResultType));
                    }
                }
                PropertySchema[] properties = markupTypeSchema.Properties;
                _writer.WriteUInt16(properties.Length);
                foreach (MarkupPropertySchema markupPropertySchema in properties)
                {
                    WriteDataTableString(markupPropertySchema.Name);
                    _writer.WriteBool(markupPropertySchema.RequiredForCreation);
                    if (markupPropertySchema.OverrideCriteria != null)
                    {
                        PropertyOverrideCriteriaTypeConstraint overrideCriteria = (PropertyOverrideCriteriaTypeConstraint)markupPropertySchema.OverrideCriteria;
                        _writer.WriteBool(true);
                        ushort index1 = MapTypeToIndex(overrideCriteria.Constraint);
                        ushort index2 = MapTypeToIndex(overrideCriteria.Use);
                        _writer.WriteUInt16(index1);
                        _writer.WriteUInt16(index2);
                    }
                    else
                        _writer.WriteBool(false);
                    _writer.WriteUInt16(MapTypeToIndex(markupPropertySchema.PropertyType));
                    if (markupType == MarkupType.DataType)
                    {
                        MarkupDataTypePropertySchema typePropertySchema = (MarkupDataTypePropertySchema)markupPropertySchema;
                        _writer.WriteUInt16(typePropertySchema.AlternateType != null ? MapTypeToIndex(typePropertySchema.AlternateType) : ushort.MaxValue);
                    }
                    if (markupType == MarkupType.DataQuery)
                    {
                        MarkupDataQueryPropertySchema queryPropertySchema = (MarkupDataQueryPropertySchema)markupPropertySchema;
                        if (queryPropertySchema.DefaultValue != null)
                        {
                            _writer.WriteBool(true);
                            queryPropertySchema.PropertyType.EncodeBinary(_writer, queryPropertySchema.DefaultValue);
                        }
                        else
                            _writer.WriteBool(false);
                        _writer.WriteBool(queryPropertySchema.InvalidatesQuery);
                        _writer.WriteUInt16(queryPropertySchema.AlternateType != null ? MapTypeToIndex(queryPropertySchema.AlternateType) : ushort.MaxValue);
                    }
                }
                WriteMarkupMethodArrayHelper(markupTypeSchema.Methods, "METHODS");
                WriteMarkupMethodArrayHelper(markupTypeSchema.VirtualMethods, "VIRTUAL METHODS");
            }
        }

        private void PersistMemberImportTables()
        {
            if (_usingSharedBinaryDataTable)
                return;
            _writer.WriteUInt16(_loadResult.ImportTables.ConstructorImports.Length);
            foreach (ConstructorSchema constructorImport in _loadResult.ImportTables.ConstructorImports)
            {
                _writer.WriteUInt16(MapTypeToIndex(constructorImport.Owner));
                _writer.WriteUInt16(constructorImport.ParameterTypes.Length);
                foreach (TypeSchema parameterType in constructorImport.ParameterTypes)
                    _writer.WriteUInt16(MapTypeToIndex(parameterType));
            }
            _writer.WriteUInt16(_loadResult.ImportTables.PropertyImports.Length);
            foreach (PropertySchema propertyImport in _loadResult.ImportTables.PropertyImports)
            {
                _writer.WriteUInt16(MapTypeToIndex(propertyImport.Owner));
                WriteDataTableString(propertyImport.Name);
            }
            _writer.WriteUInt16(_loadResult.ImportTables.MethodImports.Length);
            foreach (MethodSchema methodImport in _loadResult.ImportTables.MethodImports)
            {
                _writer.WriteUInt16(MapTypeToIndex(methodImport.Owner));
                bool flag = false;
                MarkupMethodSchema markupMethodSchema = null;
                if (methodImport is MarkupMethodSchema)
                {
                    markupMethodSchema = (MarkupMethodSchema)methodImport;
                    flag = markupMethodSchema.IsVirtual && !markupMethodSchema.IsVirtualThunk;
                }
                _writer.WriteBool(flag);
                if (!flag)
                {
                    WriteDataTableString(methodImport.Name);
                    _writer.WriteUInt16(methodImport.ParameterTypes.Length);
                    foreach (TypeSchema parameterType in methodImport.ParameterTypes)
                        _writer.WriteUInt16(MapTypeToIndex(parameterType));
                }
                else
                    _writer.WriteInt32(markupMethodSchema.VirtualId);
            }
            _writer.WriteUInt16(_loadResult.ImportTables.EventImports.Length);
            foreach (EventSchema eventImport in _loadResult.ImportTables.EventImports)
            {
                _writer.WriteUInt16(MapTypeToIndex(eventImport.Owner));
                WriteDataTableString(eventImport.Name);
            }
        }

        private void PersistDataMappingsTable()
        {
            if (_loadResult.DataMappingsTable == null)
            {
                _writer.WriteUInt16(0);
            }
            else
            {
                _writer.WriteUInt16(_loadResult.DataMappingsTable.Length);
                foreach (MarkupDataMapping markupDataMapping in _loadResult.DataMappingsTable)
                {
                    _writer.WriteUInt16(MapTypeToIndex(markupDataMapping.TargetType));
                    WriteDataTableString(markupDataMapping.Provider);
                    _writer.WriteUInt16(markupDataMapping.Mappings.Length);
                    foreach (MarkupDataMappingEntry mapping in markupDataMapping.Mappings)
                    {
                        WriteDataTableString(mapping.Source);
                        WriteDataTableString(mapping.Target);
                        _writer.WriteUInt16(MapPropertyToIndex(mapping.Property));
                        if (mapping.DefaultValue != null && mapping.Property.PropertyType.SupportsBinaryEncoding)
                        {
                            _writer.WriteBool(true);
                            mapping.Property.PropertyType.EncodeBinary(_writer, mapping.DefaultValue);
                        }
                        else
                            _writer.WriteBool(false);
                    }
                }
            }
        }

        private void PersistConstantsTable()
        {
            if (!_usingSharedBinaryDataTable)
            {
                int length = _loadResult.ConstantsTable.PersistList.Length;
                _writer.WriteUInt16(length);
                uint dataSize = _writer.DataSize;
                uint offset = dataSize;
                for (int index = 0; index <= length; ++index)
                    _writer.WriteUInt32(uint.MaxValue);
                foreach (MarkupConstantPersist persist in _loadResult.ConstantsTable.PersistList)
                {
                    _writer.Overwrite(offset, _writer.DataSize - dataSize);
                    offset += 4U;
                    ushort index = MapTypeToIndex(persist.Type);
                    MarkupConstantPersistMode constantPersistMode = persist.Mode;
                    if (persist.Type == StringSchema.Type)
                        constantPersistMode = MarkupConstantPersistMode.FromString;
                    _writer.WriteUInt16(index);
                    _writer.WriteByte((byte)constantPersistMode);
                    Debug.Trace.WriteLine(Debug.TraceCategory.MarkupCompiler, $"[{index}] mode={constantPersistMode} data={persist.Data}");
                    switch (constantPersistMode)
                    {
                        case MarkupConstantPersistMode.Binary:
                            _loadResult.ImportTables.TypeImports[index].EncodeBinary(_writer, persist.Data);
                            break;
                        case MarkupConstantPersistMode.FromString:
                        case MarkupConstantPersistMode.Canonical:
                            WriteDataTableString((string)persist.Data);
                            break;
                    }
                }
                _writer.Overwrite(offset, _writer.DataSize - dataSize);
            }
            else
                _writer.WriteUInt16(0);
        }

        private void PersistBinaryDataTable()
        {
            if (_binaryDataTableSectionOffsetFixup != uint.MaxValue)
            {
                uint dataSize1 = _writer.DataSize;
                Vector<string> strings = _binaryDataTable.Strings;
                _writer.WriteInt32(strings.Count);
                uint dataSize2 = _writer.DataSize;
                uint offset = dataSize2;
                for (int index = 0; index <= strings.Count; ++index)
                    _writer.WriteUInt32(uint.MaxValue);
                foreach (string str in strings)
                {
                    _writer.Overwrite(offset, _writer.DataSize - dataSize2);
                    offset += 4U;
                    _writer.WriteString(str);
                }
                _writer.Overwrite(offset, _writer.DataSize - dataSize2);
                _writer.Overwrite(_binaryDataTableSectionOffsetFixup, dataSize1);
            }
            _binaryDataTablePersisted = true;
        }

        private void PersistLineNumberTable()
        {
            _writer.Overwrite(_lineNumberTableStartFixup, _writer.DataSize);
            _writer.WriteUInt16(_loadResult.LineNumberTable.PersistList.Length);
            foreach (ulong persist in _loadResult.LineNumberTable.PersistList)
                _writer.WriteUInt64(persist);
            _writer.Overwrite(_lineNumberTableEndFixup, _writer.DataSize);
        }

        private void PersistObjectSection()
        {
            _writer.Overwrite(_objectSectionStartFixup, _writer.DataSize);
            _writer.Write(_loadResult.ObjectSection);
            _loadResult.ObjectSection.ToIntPtr(out uint _);
            _writer.Overwrite(_objectSectionEndFixup, _writer.DataSize);
        }

        private ushort MapDependentToIndex(LoadResult dependent)
        {
            if (dependent == MarkupSystem.UIXGlobal)
                return 65534;
            if (dependent == _loadResult)
                return 65533;
            int num = _usingSharedBinaryDataTable ? _binaryDataTable.SourceMarkupImportTables.ImportedLoadResults.IndexOf(dependent) : Array.IndexOf<LoadResult>(_loadResult.Dependencies, dependent);
            return num >= 0 ? (ushort)num : ushort.MaxValue;
        }

        private ushort MapTypeToIndex(TypeSchema type)
        {
            for (ushort index = 0; index < (ushort)_loadResult.ImportTables.TypeImports.Length; ++index)
            {
                TypeSchema typeImport = _loadResult.ImportTables.TypeImports[index];
                if (type == typeImport)
                    return index;
            }
            return ushort.MaxValue;
        }

        private ushort MapPropertyToIndex(PropertySchema property)
        {
            for (ushort index = 0; index < (ushort)_loadResult.ImportTables.PropertyImports.Length; ++index)
            {
                PropertySchema propertyImport = _loadResult.ImportTables.PropertyImports[index];
                if (property == propertyImport)
                    return index;
            }
            return ushort.MaxValue;
        }

        private ushort MapMethodToIndex(MethodSchema method)
        {
            for (ushort index = 0; index < (ushort)_loadResult.ImportTables.MethodImports.Length; ++index)
            {
                MethodSchema methodImport = _loadResult.ImportTables.MethodImports[index];
                if (method == methodImport)
                    return index;
            }
            return ushort.MaxValue;
        }

        private void WriteDataTableString(string s) => _writer.WriteInt32(_binaryDataTable.GetIndexOrAdd(s));

        private void WriteUInt32ArrayHelper(uint[] array, string debugName)
        {
            uint num1 = 0;
            if (array != null)
                num1 = (uint)array.Length;
            _writer.WriteUInt32(num1);
            if (num1 == 0U)
                return;
            foreach (uint num2 in array)
                _writer.WriteUInt32(num2);
        }

        private void WriteStringArrayHelper(string[] array, string debugName)
        {
            uint num = 0;
            if (array != null)
                num = (uint)array.Length;
            _writer.WriteUInt32(num);
            if (num == 0U)
                return;
            foreach (string s in array)
                WriteDataTableString(s);
        }

        private void WriteMarkupMethodArrayHelper(MethodSchema[] methods, string debugName)
        {
            _writer.WriteUInt16(methods.Length);
            foreach (MarkupMethodSchema method in methods)
            {
                WriteDataTableString(method.Name);
                _writer.WriteUInt16(MapTypeToIndex(method.ReturnType));
                _writer.WriteInt32(method.ParameterTypes.Length);
                foreach (TypeSchema parameterType in method.ParameterTypes)
                    _writer.WriteUInt16(MapTypeToIndex(parameterType));
                WriteStringArrayHelper(method.ParameterNames, "ParameterNames");
                _writer.WriteInt32(method.VirtualId);
                _writer.WriteBool(method.IsVirtualThunk);
                _writer.WriteUInt32(method.CodeOffset);
            }
        }

        public static ByteCodeWriter CompileBinaryDataTable(
          MarkupBinaryDataTable binaryDataTable)
        {
            SourceMarkupLoadResult markupLoadResult = new SourceMarkupLoadResult(binaryDataTable.Uri);
            markupLoadResult.RegisterUsage(markupLoadResult);
            binaryDataTable.ConstantsTable.PrepareForRuntimeUse();
            MarkupImportTables importTables = binaryDataTable.SourceMarkupImportTables.PrepareImportTables();
            Vector importedLoadResults = binaryDataTable.SourceMarkupImportTables.ImportedLoadResults;
            LoadResult[] dependenciesTable = new LoadResult[importedLoadResults.Count];
            for (int index = 0; index < dependenciesTable.Length; ++index)
                dependenciesTable[index] = (LoadResult)importedLoadResults[index];
            markupLoadResult.SetBinaryDataTable(binaryDataTable);
            markupLoadResult.SetConstantsTable(binaryDataTable.ConstantsTable);
            markupLoadResult.SetDependenciesTable(dependenciesTable, false);
            markupLoadResult.SetImportTables(importTables);
            markupLoadResult.SetLineNumberTable(new MarkupLineNumberTable());
            markupLoadResult.LineNumberTable.PrepareForRuntimeUse();
            markupLoadResult.SetObjectSection(new ByteCodeWriter().CreateReader());
            ByteCodeWriter byteCodeWriter = Run(markupLoadResult, null);
            markupLoadResult.UnregisterUsage(markupLoadResult);
            return byteCodeWriter;
        }
    }
}
