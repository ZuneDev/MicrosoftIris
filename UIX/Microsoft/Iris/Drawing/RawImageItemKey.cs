// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.RawImageItemKey
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render.Extensions;

namespace Microsoft.Iris.Drawing
{
    internal class RawImageItemKey : ImageCacheKey
    {
        private static int s_uniqueId;
        private int _uniqueId;

        public RawImageItemKey(string id)
          : base(id)
          => _uniqueId = ++s_uniqueId;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            return obj is RawImageItemKey rawImageItemKey && _uniqueId == rawImageItemKey._uniqueId;
        }

        public override int GetHashCode() => _uniqueId;
    }
}
