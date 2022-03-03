// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.SimpleAnimationPropertyMap
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using System.Collections.Generic;

namespace Microsoft.Iris
{
    internal class SimpleAnimationPropertyMap : IAnimationPropertyMap
    {
        private IDictionary<string, int> _propertyNameToId;

        public SimpleAnimationPropertyMap(IDictionary<string, int> propertyNameToId) => _propertyNameToId = propertyNameToId;

        public uint GetPropertyId(string propertyName)
        {
            int num;
            _propertyNameToId.TryGetValue(propertyName, out num);
            return (uint)num;
        }

        public AnimationInputType GetPropertyType(string propertyName) => AnimationInputType.Float;
    }
}
