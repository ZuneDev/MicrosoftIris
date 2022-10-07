// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIClassTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.Markup
{
    public class UIClassTypeSchema : MarkupTypeSchema, IDynamicConstructionSchema
    {
        private NamedContentRecord[] _namedContentTable;

        public UIClassTypeSchema(MarkupLoadResult owner, string name)
          : base(owner, name)
        {
        }

        public override MarkupType MarkupType => MarkupType.UI;

        protected override TypeSchema DefaultBase => HostSchema.Type;

        public override Type RuntimeType => typeof(Host);

        public override object ConstructDefault() => new Host(this);

        object IDynamicConstructionSchema.ConstructDefault(
          TypeSchema replacedType)
        {
            return new Host((UIClassTypeSchema)replacedType, this);
        }

        public UIClass ConstructUI() => new UIClass(this);

        public ViewItem ConstructNamedContent(
          string name,
          IMarkupTypeBase markupType,
          ParameterContext parameterContext)
        {
            ErrorManager.EnterContext(markupType.TypeSchema.Owner.ErrorContextUri);
            try
            {
                uint scriptOffset = uint.MaxValue;
                if (_namedContentTable != null)
                {
                    foreach (NamedContentRecord namedContentRecord in _namedContentTable)
                    {
                        if (namedContentRecord.Name == name)
                        {
                            scriptOffset = namedContentRecord.Offset;
                            break;
                        }
                    }
                }
                if (scriptOffset == uint.MaxValue)
                {
                    UIClassTypeSchema markupTypeBase = (UIClassTypeSchema)MarkupTypeBase;
                    if (markupTypeBase != null)
                        return markupTypeBase.ConstructNamedContent(name, markupType, parameterContext);
                }
                if (scriptOffset != uint.MaxValue)
                {
                    object obj = RunAtOffset(markupType, scriptOffset, parameterContext);
                    if (obj != Interpreter.ScriptError)
                        return (ViewItem)obj;
                    ErrorManager.ReportWarning("Script runtime failure: Scripting errors have prevented '{0}' named content from being constructed", name);
                }
                return null;
            }
            finally
            {
                ErrorManager.ExitContext();
            }
        }

        public override void InitializeInstance(ref object instance)
        {
            Host ownerHost = instance as Host;
            UIClass childUi = ownerHost.ChildUI;
            childUi.DeclareHost(ownerHost);
            InitializeInstance(ownerHost.ChildUI);
            if (childUi.RootItem != null)
                ownerHost.Children.Add(childUi.RootItem);
            childUi.DeclareOwner(ownerHost);
        }

        public NamedContentRecord[] NamedContentTable => _namedContentTable;

        public void SetNamedContentTable(NamedContentRecord[] namedContentTable) => _namedContentTable = namedContentTable;
    }
}
