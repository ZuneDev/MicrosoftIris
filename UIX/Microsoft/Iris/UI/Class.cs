// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.Class
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Iris.UI
{
    public class Class :
      DisposableObject,
      INotifyObject,
      IMarkupTypeBase,
      IDisposableOwner,
      ISchemaInfo
    {
        public const string ClassStateReservedSymbolName = "Class";
        public const string ThisReservedSymbolName = "this";
        private MarkupTypeSchema _typeSchema;
        protected Dictionary<object, object> _storage;
        private Vector<IDisposableObject> _disposables;
        private MarkupListeners _listeners;
        protected NotifyService _notifier = new NotifyService();
        private ScriptRunScheduler _scriptRunScheduler = new ScriptRunScheduler();
        private bool _scriptEnabled;
        private static DeferredHandler s_executePendingScriptsHandler = new DeferredHandler(ExecutePendingScripts);

        public Class(MarkupTypeSchema type)
        {
            _typeSchema = type;
            _storage = new Dictionary<object, object>(type.TotalPropertiesAndLocalsCount);
            _scriptEnabled = true;
        }

        protected override void OnDispose()
        {
            _typeSchema.RunFinalEvaluates(this);
            base.OnDispose();
            _scriptEnabled = false;
            if (_listeners != null)
            {
                _listeners.Dispose(this);
                _listeners = null;
            }
            _notifier.ClearListeners();
            _storage.Clear();
            if (_disposables == null)
                return;
            for (int index = 0; index < _disposables.Count; ++index)
                _disposables[index].Dispose(this);
        }

        public void RegisterDisposable(IDisposableObject disposable)
        {
            if (_disposables == null)
                _disposables = new Vector<IDisposableObject>();
            _disposables.Add(disposable);
        }

        public bool UnregisterDisposable(ref IDisposableObject disposable)
        {
            if (_disposables != null)
            {
                int index = _disposables.IndexOf(disposable);
                if (index != -1)
                {
                    disposable = _disposables[index];
                    _disposables.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }

        public TypeSchema TypeSchema => _typeSchema;

        public virtual void NotifyInitialized()
        {
        }

        void INotifyObject.AddListener(Listener listener) => _notifier.AddListener(listener);

        public virtual object ReadSymbol(SymbolReference symbolRef)
        {
            object obj = null;
            switch (symbolRef.Origin)
            {
                case SymbolOrigin.Properties:
                case SymbolOrigin.Locals:
                    obj = _storage[symbolRef.Symbol];
                    break;
                case SymbolOrigin.Reserved:
                    if (symbolRef.Symbol == nameof(Class) || symbolRef.Symbol == "this")
                    {
                        obj = this;
                        break;
                    }
                    break;
            }
            return obj;
        }

        public virtual void WriteSymbol(SymbolReference symbolRef, object value) => SetProperty(symbolRef.Symbol, value);

        // TODO: UNDO ME
        public virtual object GetProperty(string name) => _storage.ContainsKey(name) ? _storage[name] : null;

        public virtual void SetProperty(string name, object value)
        {
            if (_storage.ContainsKey(name))
                return;
            _storage[name] = value;

            if (Utility.IsEqual(_storage[name], value))
                _notifier.Fire(name);
        }

        public MarkupListeners Listeners
        {
            get => _listeners;
            set => _listeners = value;
        }

        public Dictionary<object, object> Storage => _storage;

        public void ScheduleScriptRun(uint scriptId, bool ignoreErrors)
        {
            if (!_scriptRunScheduler.Pending)
                DeferredCall.Post(DispatchPriority.Script, s_executePendingScriptsHandler, this);
            _scriptRunScheduler.ScheduleRun(scriptId, ignoreErrors);
        }

        private static void ExecutePendingScripts(object args)
        {
            Class @class = (Class)args;
            @class._scriptRunScheduler.Execute(@class);
        }

        public object RunScript(uint scriptId, bool ignoreErrors, ParameterContext parameterContext) => _typeSchema.Run(this, scriptId, ignoreErrors, parameterContext);

        public void NotifyScriptErrors()
        {
            _scriptEnabled = false;
            ErrorManager.ReportWarning("Script runtime failure: Scripting has been disabled for '{0}' due to runtime scripting errors", _typeSchema.Name);
        }

        public bool ScriptEnabled => _scriptEnabled;

        public override string ToString() => _typeSchema.ToString();

        [Conditional("DEBUG")]
        public void DEBUG_MarkInitialized()
        {
        }
    }
}
