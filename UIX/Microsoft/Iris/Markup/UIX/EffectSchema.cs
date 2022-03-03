// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EffectSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EffectSchema
    {
        public static UIXTypeSchema Type;

        private static object GetTechniques(object instanceObj) => (object)null;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(69, "Effect", null, 29, typeof(EffectClass), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(69, "Techniques", 138, 77, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetTechniques), null, false);
            Type.Initialize(null, null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
