﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.InvAlphaSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class InvAlphaSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => new InvAlphaElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(123, "InvAlpha", null, 80, typeof(InvAlphaElement), UIXTypeFlags.None);

        public static void Pass2Initialize() => Type.Initialize(new DefaultConstructHandler(Construct), null, null, null, null, null, null, null, null, null, null, null);
    }
}
