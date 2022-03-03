// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValueApplyMode
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Markup.Validation
{
    [Flags]
    internal enum ValueApplyMode
    {
        SingleValueSet = 0,
        MultiValueList = 1,
        MultiValueDictionary = 2,
        CollectionAdd = 4,
        CollectionPopulateAndSet = 8,
    }
}
