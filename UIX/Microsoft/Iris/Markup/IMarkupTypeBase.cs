// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.IMarkupTypeBase
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System.Collections.Generic;

namespace Microsoft.Iris.Markup
{
    public interface IMarkupTypeBase : IDisposableOwner
    {
        TypeSchema TypeSchema { get; }

        void NotifyInitialized();

        object GetProperty(string name);

        void SetProperty(string name, object value);

        object ReadSymbol(SymbolReference symbolRef);

        void WriteSymbol(SymbolReference symbolRef, object value);

        MarkupListeners Listeners { get; set; }

        void ScheduleScriptRun(uint scriptId, bool ignoreErrors);

        object RunScript(uint scriptId, bool ignoreErrors, ParameterContext parameterContext);

        bool ScriptEnabled { get; }

        void NotifyScriptErrors();

        Dictionary<object, object> Storage { get; }
    }
}
