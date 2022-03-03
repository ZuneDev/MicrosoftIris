// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TextScrollModelSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TextScrollModelSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => new TextScrollModel();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(218, "TextScrollModel", null, 183, typeof(TextScrollModel), UIXTypeFlags.None);

        public static void Pass2Initialize() => Type.Initialize(new DefaultConstructHandler(Construct), null, null, null, null, null, null, null, null, null, null, null);
    }
}
