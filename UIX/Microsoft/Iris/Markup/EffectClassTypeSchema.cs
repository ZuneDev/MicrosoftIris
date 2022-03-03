// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.EffectClassTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Markup
{
    internal class EffectClassTypeSchema : ClassTypeSchema
    {
        private uint[] _techniqueOffsets;
        private uint[] _instancePropertyAssignments;
        private string[] _dynamicElementAssignments;
        private int _defaultElementSymbolIndex = -1;
        private IEffectTemplate _effectTemplate;
        private int _templateIndexBuilt;
        private PropertySchema _defaultProperty;

        public EffectClassTypeSchema(MarkupLoadResult owner, string name)
          : base(owner, name)
        {
        }

        protected override void SealWorker()
        {
            base.SealWorker();
            _defaultProperty = FindPropertyDeep("Default");
        }

        public override MarkupType MarkupType => MarkupType.Effect;

        protected override void OnDispose()
        {
            base.OnDispose();
            if (_effectTemplate == null)
                return;
            _effectTemplate.UnregisterUsage(this);
            _effectTemplate = null;
        }

        protected override TypeSchema DefaultBase => EffectInstanceSchema.Type;

        public override Type RuntimeType => typeof(EffectClass);

        public string DefaultElementSymbol => _defaultElementSymbolIndex >= 0 ? SymbolReferenceTable[_defaultElementSymbolIndex].Symbol : null;

        protected override Class ConstructNewInstance()
        {
            EnsureEffectTemplate();
            return new EffectClass(this, _effectTemplate);
        }

        protected override bool RunInitialEvaluates(IMarkupTypeBase scriptHost)
        {
            bool flag = true;
            if (_instancePropertyAssignments != null && _templateIndexBuilt >= 0)
            {
                ErrorManager.EnterContext(this);
                flag = RunInitializeScript(scriptHost, _instancePropertyAssignments[_templateIndexBuilt]);
                ErrorManager.ExitContext();
            }
            return flag && base.RunInitialEvaluates(scriptHost);
        }

        private void EnsureEffectTemplate()
        {
            if (_effectTemplate != null)
                return;
            _effectTemplate = UISession.Default.RenderSession.CreateEffectTemplate(this, Name);
            ErrorManager.EnterContext(this);
            Class @class = new Class(this);
            @class.DeclareOwner(this);
            _templateIndexBuilt = -1;
            for (int index = 0; index < _techniqueOffsets.Length; ++index)
            {
                object obj = RunAtOffset(@class, _techniqueOffsets[index]);
                if (obj == Interpreter.ScriptError)
                {
                    if (!ErrorManager.IgnoringErrors)
                        ErrorManager.ReportWarning("Script runtime failure: Scripting errors have prevented '{0}' from properly initializing and will affect its operation", Name);
                }
                else
                {
                    EffectInput input = (EffectInput)obj;
                    if (_dynamicElementAssignments != null)
                    {
                        foreach (string elementAssignment in _dynamicElementAssignments)
                            _effectTemplate.AddEffectProperty(elementAssignment);
                    }
                    if (_effectTemplate.Build(input))
                    {
                        _templateIndexBuilt = index;
                        break;
                    }
                }
            }
            @class.Dispose(this);
            ErrorManager.ExitContext();
        }

        public uint[] TechniqueOffsets => _techniqueOffsets;

        public uint[] InstancePropertyAssignments => _instancePropertyAssignments;

        public string[] DynamicElementAssignments => _dynamicElementAssignments;

        public int DefaultElementSymbolIndex => _defaultElementSymbolIndex;

        public void SetTechniqueOffsets(uint[] techniqueOffsets) => _techniqueOffsets = techniqueOffsets;

        public void SetInstancePropertyAssignments(uint[] instancePropertyAssignments) => _instancePropertyAssignments = instancePropertyAssignments;

        public void SetDynamicElementAssignments(string[] dynamicElementAssignments) => _dynamicElementAssignments = dynamicElementAssignments;

        public void SetDefaultElementSymbolIndex(int symbolIndex) => _defaultElementSymbolIndex = symbolIndex;
    }
}
