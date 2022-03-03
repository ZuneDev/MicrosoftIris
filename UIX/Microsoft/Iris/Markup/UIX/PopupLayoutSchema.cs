// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.PopupLayoutSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class PopupLayoutSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => new PopupLayout();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(161, "PopupLayout", null, 132, typeof(PopupLayout), UIXTypeFlags.Immutable);

        public static void Pass2Initialize() => Type.Initialize(new DefaultConstructHandler(Construct), null, null, null, null, null, null, null, null, null, null, null);
    }
}
