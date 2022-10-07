// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Markup
{
    public abstract class MarkupTypeSchema : TypeSchema
    {
        private const int c_TypeDepthShift = 27;
        private const uint c_ScriptOffsetMask = 134217727;
        public const uint InvalidScriptId = 4294967295;
        private MarkupLoadResult _owner;
        private string _name;
        private MarkupTypeSchema _baseType;
        private uint _typeDepth;
        private int _totalPropertiesAndLocalsCount;
        private PropertySchema[] _properties = PropertySchema.EmptyList;
        private MethodSchema[] _methods = MethodSchema.EmptyList;
        private Map<MethodSignatureKey, MethodSchema> _methodLookupTable;
        private MethodSchema[] _virtualMethods = MethodSchema.EmptyList;
        private SymbolReference[] _symbolReferenceTable;
        private IntPtr _addressOfInheritableSymbolsTable;
        private SymbolRecord[] _inheritableSymbolsTable;
        private uint _initializePropertiesOffset = uint.MaxValue;
        private uint _initializeLocalsInputOffset = uint.MaxValue;
        private uint _initializeContentOffset = uint.MaxValue;
        private uint[] _refreshGroupOffsets;
        private uint[] _initialEvaluateOffsets;
        private uint[] _finalEvaluateOffsets;
        private uint _listenerCount;
        private bool _sealed;
        private object _loadData;
        private static object[] EmptyParameterList = new object[0];

        public static MarkupTypeSchema Build(
          TypeSchema markupTypeDefinition,
          MarkupLoadResult owner,
          string name)
        {
            if (markupTypeDefinition == ClassSchema.Type)
                return new ClassTypeSchema(owner, name);
            if (markupTypeDefinition == UISchema.Type)
                return new UIClassTypeSchema(owner, name);
            if (markupTypeDefinition == EffectSchema.Type)
                return new EffectClassTypeSchema(owner, name);
            if (markupTypeDefinition == DataTypeSchema.Type)
                return new MarkupDataTypeSchema(owner, name);
            return markupTypeDefinition == DataQuerySchema.Type ? new MarkupDataQuerySchema(owner, name) : null;
        }

        public MarkupTypeSchema(MarkupLoadResult owner, string name)
          : base(owner)
        {
            _owner = owner;
            _name = name;
        }

        public abstract MarkupType MarkupType { get; }

        public void SetBaseType(MarkupTypeSchema baseType) => _baseType = baseType;

        public object LoadData
        {
            get => _loadData;
            set => _loadData = value;
        }

        public virtual void BuildProperties()
        {
        }

        public void Seal()
        {
            if (_sealed)
                return;
            SealWorker();
        }

        protected virtual void SealWorker()
        {
            _sealed = true;
            _loadData = null;
            if (_baseType == null)
            {
                _typeDepth = 1U;
            }
            else
            {
                _baseType.Seal();
                _typeDepth = _baseType._typeDepth + 1U;
            }
        }

        public bool Sealed => _sealed;

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (DisposableObject property in _properties)
                property.Dispose(this);
            foreach (DisposableObject method in _methods)
                method.Dispose(this);
            foreach (DisposableObject virtualMethod in _virtualMethods)
                virtualMethod.Dispose(this);
        }

        public override string Name => _name;

        public override string AlternateName => (string)null;

        public override TypeSchema Base => _baseType == null ? DefaultBase : _baseType;

        public override bool Contractual => false;

        public override bool IsNativeAssignableFrom(object check) => false;

        public override bool IsNativeAssignableFrom(TypeSchema checkSchema) => false;

        public MarkupTypeSchema MarkupTypeBase => _baseType;

        protected abstract TypeSchema DefaultBase { get; }

        public override bool Disposable => true;

        public override bool HasDefaultConstructor => true;

        public object Run(
          IMarkupTypeBase markupType,
          uint scriptId,
          bool ignoreErrors,
          ParameterContext parameterContext)
        {
            ErrorManager.EnterContext(markupType.TypeSchema.Owner.ErrorContextUri, ignoreErrors);
            MarkupTypeSchema markupTypeSchema = this;
            uint num = scriptId >> 27;
            while ((int)num != (int)markupTypeSchema._typeDepth)
                markupTypeSchema = markupTypeSchema._baseType;
            object obj = markupTypeSchema.RunAtOffset(markupType, scriptId & 0x07FFFFFFU, parameterContext);
            ErrorManager.ExitContext();
            return obj;
        }

        protected object RunAtOffset(
          IMarkupTypeBase markupType,
          uint scriptOffset,
          ParameterContext parameterContext)
        {
            object obj = null;
            if (markupType.ScriptEnabled)
            {
                InterpreterContext context = InterpreterContext.Acquire(markupType, this, scriptOffset, parameterContext);
                obj = Interpreter.Run(context);
                if (context != null)
                    InterpreterContext.Release(context);
            }
            if (obj == Interpreter.ScriptError && !ErrorManager.IgnoringErrors)
                markupType.NotifyScriptErrors();
            return obj;
        }

        protected object RunAtOffset(IMarkupTypeBase markupType, uint scriptOffset) => RunAtOffset(markupType, scriptOffset, new ParameterContext());

        public override ConstructorSchema FindConstructor(TypeSchema[] parameters) => (ConstructorSchema)null;

        public override PropertySchema FindProperty(string name)
        {
            for (int index = 0; index < _properties.Length; ++index)
            {
                PropertySchema property = _properties[index];
                if (name == property.Name)
                    return property;
            }
            return null;
        }

        public override PropertySchema[] Properties => _properties;

        public override MethodSchema[] Methods => _methods;

        public override MethodSchema FindMethod(string name, TypeSchema[] parameters)
        {
            MethodSchema methodSchema = null;
            if (_methodLookupTable != null)
                _methodLookupTable.TryGetValue(new MethodSignatureKey(name, parameters), out methodSchema);
            return methodSchema;
        }

        public MethodSchema[] VirtualMethods => _virtualMethods;

        public override bool HasInitializer => true;

        protected void InitializeInstance(IMarkupTypeBase classBase)
        {
            if (!InitializePropertiesLocalsInputContent(classBase, true))
                return;
            RefreshAllListeners(classBase);
            if (!RunInitialEvaluates(classBase))
                return;
            classBase.NotifyInitialized();
        }

        private bool InitializePropertiesLocalsInputContent(
          IMarkupTypeBase classBase,
          bool shouldInitializeContent)
        {
            ErrorManager.EnterContext(this);
            try
            {
                if (!RunInitializeScript(classBase, _initializePropertiesOffset))
                    return false;
                if (_baseType != null)
                {
                    bool shouldInitializeContent1 = shouldInitializeContent && _initializeContentOffset == uint.MaxValue;
                    if (!_baseType.InitializePropertiesLocalsInputContent(classBase, shouldInitializeContent1))
                        return false;
                }
                if (!RunInitializeScript(classBase, _initializeLocalsInputOffset))
                    return false;
                if (shouldInitializeContent)
                {
                    if (!RunInitializeScript(classBase, _initializeContentOffset))
                        return false;
                }
            }
            finally
            {
                ErrorManager.ExitContext();
            }
            return true;
        }

        private void RefreshAllListeners(IMarkupTypeBase scriptHost)
        {
            if (_baseType != null)
                _baseType.RefreshAllListeners(scriptHost);
            RunOffsets(scriptHost, _refreshGroupOffsets, true);
        }

        protected virtual bool RunInitialEvaluates(IMarkupTypeBase scriptHost) => (_baseType == null || _baseType.RunInitialEvaluates(scriptHost)) && RunOffsets(scriptHost, _initialEvaluateOffsets);

        public bool RunFinalEvaluates(IMarkupTypeBase scriptHost) => (_baseType == null || _baseType.RunFinalEvaluates(scriptHost)) && RunOffsets(scriptHost, _finalEvaluateOffsets);

        private bool RunOffsets(IMarkupTypeBase scriptHost, uint[] scriptOffsets) => RunOffsets(scriptHost, scriptOffsets, false);

        private bool RunOffsets(IMarkupTypeBase scriptHost, uint[] scriptOffsets, bool ignoreErrors)
        {
            bool flag = true;
            if (scriptOffsets != null)
            {
                ErrorManager.EnterContext(this, ignoreErrors);
                foreach (uint scriptOffset in scriptOffsets)
                {
                    if (!RunInitializeScript(scriptHost, scriptOffset))
                    {
                        flag = false;
                        break;
                    }
                }
                ErrorManager.ExitContext();
            }
            return flag;
        }

        protected bool RunInitializeScript(IMarkupTypeBase scriptHost, uint scriptOffset)
        {
            if (scriptOffset == uint.MaxValue || RunAtOffset(scriptHost, scriptOffset) != Interpreter.ScriptError || ErrorManager.IgnoringErrors)
                return true;
            ErrorManager.ReportWarning("Script runtime failure: Scripting errors have prevented '{0}' from properly initializing and will affect its operation", _name);
            return false;
        }

        public override EventSchema FindEvent(string name) => (EventSchema)null;

        public override object FindCanonicalInstance(string name) => (object)null;

        public override Result TypeConverter(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            instance = null;
            return Result.Fail("Type conversion is not available for '{0}'", _name);
        }

        public override bool SupportsTypeConversion(TypeSchema fromType) => false;

        public override bool SupportsBinaryEncoding => false;

        public override void EncodeBinary(ByteCodeWriter writer, object instance)
        {
        }

        public override object DecodeBinary(ByteCodeReader reader) => (object)null;

        public override object PerformOperation(object left, object right, OperationType op) => (object)null;

        public override bool SupportsOperation(OperationType op) => false;

        public override bool IsNullAssignable => true;

        public override bool IsRuntimeImmutable => false;

        public override int FindTypeHint => -1;

        public SymbolReference[] SymbolReferenceTable => _symbolReferenceTable;

        public SymbolRecord[] InheritableSymbolsTable
        {
            get
            {
                if (_inheritableSymbolsTable == null)
                {
                    if (_addressOfInheritableSymbolsTable != IntPtr.Zero)
                        CompiledMarkupLoader.DecodeInheritableSymbolTable(this, null, _addressOfInheritableSymbolsTable);
                    else
                        _inheritableSymbolsTable = SymbolRecord.EmptyList;
                }
                return _inheritableSymbolsTable;
            }
        }

        public uint TypeDepth => _typeDepth;

        public int TotalPropertiesAndLocalsCount => _totalPropertiesAndLocalsCount;

        public uint InitializePropertiesOffset => _initializePropertiesOffset;

        public uint InitializeLocalsInputOffset => _initializeLocalsInputOffset;

        public uint InitializeContentOffset => _initializeContentOffset;

        public uint[] RefreshGroupOffsets => _refreshGroupOffsets;

        public uint[] InitialEvaluateOffsets => _initialEvaluateOffsets;

        public uint[] FinalEvaluateOffsets => _finalEvaluateOffsets;

        public uint ListenerCount => _listenerCount;

        public uint TotalListenerCount => _listenerCount + (_baseType == null ? 0U : _baseType.TotalListenerCount);

        public uint EncodeScriptOffsetAsId(uint scriptOffset) => scriptOffset | _typeDepth << 27;

        public string LocallyUniqueId => _typeDepth.ToString();

        public void SetTypeDepth(uint typeDepth) => _typeDepth = typeDepth;

        public void SetPropertyList(PropertySchema[] properties) => _properties = properties;

        public void SetMethodList(MethodSchema[] methods)
        {
            _methods = methods;
            _methodLookupTable = new Map<MethodSignatureKey, MethodSchema>(methods.Length);
            foreach (MethodSchema method in _methods)
                _methodLookupTable[new MethodSignatureKey(method.Name, method.ParameterTypes)] = method;
        }

        public void SetVirtualMethodList(MethodSchema[] virtualMethods) => _virtualMethods = virtualMethods;

        public void SetSymbolReferenceTable(SymbolReference[] symbolTable) => _symbolReferenceTable = symbolTable;

        public void SetInheritableSymbolsTable(SymbolRecord[] symbolTable) => _inheritableSymbolsTable = symbolTable;

        public void SetAddressOfInheritableSymbolTable(IntPtr address) => _addressOfInheritableSymbolsTable = address;

        public void SetRefreshListenerGroupOffsets(uint[] refreshGroupOffsets) => _refreshGroupOffsets = refreshGroupOffsets;

        public void SetInitialEvaluateOffsets(uint[] initialEvaluateOffsets) => _initialEvaluateOffsets = initialEvaluateOffsets;

        public void SetFinalEvaluateOffsets(uint[] finalEvaluateOffsets) => _finalEvaluateOffsets = finalEvaluateOffsets;

        public void SetListenerCount(uint listenerCount) => _listenerCount = listenerCount;

        public void SetTotalPropertiesAndLocalsCount(int totalPropertiesAndLocalsCount) => _totalPropertiesAndLocalsCount = totalPropertiesAndLocalsCount;

        public void SetInitializePropertiesOffset(uint offset) => _initializePropertiesOffset = offset;

        public void SetInitializeLocalsInputOffset(uint offset) => _initializeLocalsInputOffset = offset;

        public void SetInitializeContentOffset(uint offset) => _initializeContentOffset = offset;
    }
}
