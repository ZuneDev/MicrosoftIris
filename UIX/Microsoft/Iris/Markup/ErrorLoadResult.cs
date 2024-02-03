// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ErrorLoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Markup
{
    [Serializable]
    public class ErrorLoadResult : LoadResult
    {
        public ErrorLoadResult(string uri)
          : base(uri)
        {
        }

        public override TypeSchema FindType(string name) => null;

        public override LoadResultStatus Status => LoadResultStatus.Error;
    }
}
