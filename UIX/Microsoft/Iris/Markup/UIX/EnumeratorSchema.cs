// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EnumeratorSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EnumeratorSchema
    {
        public static UIXTypeSchema Type;

        private static object GetCurrent(object instanceObj) => ((IEnumerator)instanceObj).Current;

        private static object CallMoveNext(object instanceObj, object[] parameters) => BooleanBoxes.Box(((IEnumerator)instanceObj).MoveNext());

        private static object CallReset(object instanceObj, object[] parameters)
        {
            ((IEnumerator)instanceObj).Reset();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(86, "Enumerator", null, 153, typeof(IEnumerator), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(86, "Current", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCurrent), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(86, "MoveNext", null, 15, new InvokeHandler(CallMoveNext), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(86, "Reset", null, 240, new InvokeHandler(CallReset), false);
            Type.Initialize(null, null, new PropertySchema[1]
            {
         uixPropertySchema
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, null, null, null, null, null, null, null, null);
        }
    }
}
