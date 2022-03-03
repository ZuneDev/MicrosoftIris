// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIClassMethodSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup
{
    internal class UIClassMethodSchema : MarkupMethodSchema
    {
        public UIClassMethodSchema(
          UIClassTypeSchema owner,
          string name,
          TypeSchema returnType,
          TypeSchema[] parameterTypes,
          string[] parameterNames)
          : base(owner, name, returnType, parameterTypes, parameterNames)
        {
        }

        protected override IMarkupTypeBase GetMarkupTypeBase(object instance)
        {
            if (!(instance is IMarkupTypeBase markupTypeBase))
            {
                markupTypeBase = ((Host)instance).ChildUI;
                if (markupTypeBase == null)
                    ErrorManager.ReportError("Host '{0}' is currently not hosting a UI and therefore cannot invoke methods", instance);
            }
            return markupTypeBase;
        }
    }
}
