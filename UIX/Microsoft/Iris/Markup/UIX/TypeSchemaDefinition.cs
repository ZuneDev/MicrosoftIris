// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TypeSchemaDefinition
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TypeSchemaDefinition
    {
        public static UIXTypeSchema Type;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(225, "Type", null, 153, typeof(TypeSchema), UIXTypeFlags.Immutable);

        public static void Pass2Initialize() => Type.Initialize(null, null, null, null, null, null, null, null, null, null, null, null);
    }
}
