// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.InterpreterContext
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.Markup
{
    internal class InterpreterContext : IErrorContextSource
    {
        private MarkupTypeSchema _type;
        private IMarkupTypeBase _instance;
        private MarkupLoadResult _loadResult;
        private uint _initialBytecodeOffset;
        private ParameterContext _parameterContext;
        private Map<object, object> _scopedLocals;
        private static Stack s_cache = new Stack();
        public static bool UseDecompile = false;
        public static List<System.Xml.XmlDocument> DecompileResults = new List<System.Xml.XmlDocument>();

        private InterpreterContext()
        {
        }

        string IErrorContextSource.GetErrorContextDescription() => _type.Owner.ErrorContextUri;

        void IErrorContextSource.GetErrorPosition(ref int line, ref int column)
        {
            uint currentOffset = _loadResult.ObjectSection.CurrentOffset;
            if (currentOffset > 0U)
                --currentOffset;
            _loadResult.LineNumberTable.Lookup(currentOffset, out line, out column);
        }

        public IMarkupTypeBase Instance => _instance;

        public MarkupTypeSchema MarkupType => _type;

        public uint InitialBytecodeOffset => _initialBytecodeOffset;

        public MarkupLoadResult LoadResult => _loadResult;

        public object ReadSymbol(SymbolReference symbolRef)
        {
            object obj = null;
            switch (symbolRef.Origin)
            {
                case SymbolOrigin.ScopedLocal:
                    _scopedLocals.TryGetValue(symbolRef.Symbol, out obj);
                    break;
                case SymbolOrigin.Parameter:
                    obj = _parameterContext.ReadParameter(symbolRef.Symbol);
                    break;
                default:
                    obj = _instance.ReadSymbol(symbolRef);
                    break;
            }
            return obj;
        }

        public void WriteSymbol(SymbolReference symbolRef, object value)
        {
            switch (symbolRef.Origin)
            {
                case SymbolOrigin.ScopedLocal:
                    if (_scopedLocals == null)
                        _scopedLocals = new Map<object, object>();
                    _scopedLocals[symbolRef.Symbol] = value;
                    break;
                case SymbolOrigin.Parameter:
                    _parameterContext.WriteParameter(symbolRef.Symbol, value);
                    break;
                default:
                    _instance.WriteSymbol(symbolRef, value);
                    break;
            }
        }

        public void ClearSymbol(SymbolReference symbolRef)
        {
            if (symbolRef.Origin != SymbolOrigin.ScopedLocal)
                return;
            _scopedLocals.Remove(symbolRef.Symbol);
        }

        public static InterpreterContext Acquire(
          IMarkupTypeBase instance,
          MarkupTypeSchema type,
          uint initialBytecodeOffset,
          ParameterContext parameterContext)
        {
            InterpreterContext interpreterContext = null;
            if (s_cache.Count != 0)
                interpreterContext = (InterpreterContext)s_cache.Pop();
            if (interpreterContext == null)
                interpreterContext = new InterpreterContext();
            interpreterContext._instance = instance;
            interpreterContext._initialBytecodeOffset = initialBytecodeOffset;
            interpreterContext._type = type;
            interpreterContext._loadResult = (MarkupLoadResult)type.Owner;
            interpreterContext._parameterContext = parameterContext;
            return interpreterContext;
        }

        public static void Release(InterpreterContext context)
        {
            context._instance = null;
            context._type = null;
            context._loadResult = null;
            context._initialBytecodeOffset = 0U;
            context._parameterContext = new ParameterContext(null, null);
            if (context._scopedLocals != null)
                context._scopedLocals.Clear();
            s_cache.Push(context);
        }

        public override string ToString()
        {
            int line = 0;
            int column = 0;
            ((IErrorContextSource)this).GetErrorPosition(ref line, ref column);
            return string.Format("{0} ({1}, {2})", ((IErrorContextSource)this).GetErrorContextDescription(), line, column);
        }
    }
}
