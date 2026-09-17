// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.InterpreterContext
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System.Collections.Generic;

namespace Microsoft.Iris.Markup
{
    public class InterpreterContext : IErrorContextSource
    {
        private ParameterContext _parameterContext;
        private Map<object, object> _scopedLocals;
        private static Stack<InterpreterContext> s_cache = new();

        private InterpreterContext()
        {
        }

        public string GetErrorContextDescription() => MarkupType.Owner.ErrorContextUri;

        public void GetErrorPosition(ref int line, ref int column)
        {
            uint currentOffset = LoadResult.ObjectSection.CurrentOffset;
            if (currentOffset > 0U)
                --currentOffset;
            LoadResult.LineNumberTable.TryLookup(currentOffset, out line, out column);
        }

        public IMarkupTypeBase Instance { get; private set; }

        public MarkupTypeSchema MarkupType { get; private set; }

        public uint InitialBytecodeOffset { get; private set; }

        public MarkupLoadResult LoadResult { get; private set; }

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
                    obj = Instance.ReadSymbol(symbolRef);
                    break;
            }
            return obj;
        }

        public void WriteSymbol(SymbolReference symbolRef, object value)
        {
            switch (symbolRef.Origin)
            {
                case SymbolOrigin.ScopedLocal:
                    _scopedLocals ??= new Map<object, object>();
                    _scopedLocals[symbolRef.Symbol] = value;
                    break;
                case SymbolOrigin.Parameter:
                    _parameterContext.WriteParameter(symbolRef.Symbol, value);
                    break;
                default:
                    Instance.WriteSymbol(symbolRef, value);
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
                interpreterContext = s_cache.Pop();
            interpreterContext ??= new InterpreterContext();
            
            interpreterContext.Instance = instance;
            interpreterContext.InitialBytecodeOffset = initialBytecodeOffset;
            interpreterContext.MarkupType = type;
            interpreterContext.LoadResult = (MarkupLoadResult)type.Owner;
            interpreterContext._parameterContext = parameterContext;
            
            Application.Debugger?.LogInterpreterEnter(interpreterContext);
            
            return interpreterContext;
        }

        public static void Release(InterpreterContext context)
        {
            context.Instance = null;
            context.MarkupType = null;
            context.LoadResult = null;
            context.InitialBytecodeOffset = 0U;
            context._parameterContext = new ParameterContext(null, null);
            context._scopedLocals?.Clear();

            Application.Debugger?.LogInterpreterExit(context);

            s_cache.Push(context);
        }

        public override string ToString()
        {
            int line = 0;
            int column = 0;
            GetErrorPosition(ref line, ref column);
            return $"{GetErrorContextDescription()} ({line}, {column})";
        }
    }
}
