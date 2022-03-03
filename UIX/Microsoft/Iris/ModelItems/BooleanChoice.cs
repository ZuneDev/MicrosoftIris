// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.BooleanChoice
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using System.Collections;

namespace Microsoft.Iris.ModelItems
{
    internal class BooleanChoice :
      Choice,
      IUIBooleanChoice,
      IUIChoice,
      IUIValueRange,
      IDisposableObject,
      INotifyObject
    {
        private static object[] s_defaultOptions = new object[2];

        static BooleanChoice()
        {
            s_defaultOptions[0] = BooleanBoxes.FalseBox;
            s_defaultOptions[1] = BooleanBoxes.TrueBox;
        }

        public BooleanChoice()
        {
            _options = s_defaultOptions;
            _chosen = 0;
        }

        public override bool ValidateOptionsList(IList options, out string error)
        {
            if (options != null && options.Count == 2)
                return base.ValidateOptionsList(options, out error);
            error = string.Format("Script runtime failure: Invalid '{0}' value  for '{1}'", options, "Options");
            return false;
        }
    }
}
